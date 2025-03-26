using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DotNetty.Transport.Channels;
using Plus.Communication.Encryption.Crypto.Prng;
using Plus.Communication.Packets;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.BuildersClub;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Permissions;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.UserData;
using Plus.Network.Codec;
using Plus.RolePlay;
using Plus.RolePlay.Timer;

namespace Plus.HabboHotel.GameClients
{
    public class GameClient
    {
        private RPData RP;
        private RolePlayTimer RPTimer;
        private Habbo _habbo;
        public string MachineId;
        public ARC4 RC4Client = null;
        private bool _disconnected;
      //  private ConnectionInformation _connection;
        private readonly IChannelHandlerContext _channel;

        public int PingCount { get; set; }

        public GameClient(IChannelHandlerContext context)
        {
            _channel = context;
            
        }
        public void log(string message)
        {
            
        }
        public bool TryAuthenticate(string authTicket)
        {
            try
            {
                UserData userData = UserDataFactory.GetUserData(authTicket, out byte errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking

                //Let's have a quick search for a ban before we successfully authenticate..
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.User != null)
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(userData.User.Username, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.User.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                #endregion

                if (userData.User == null) //Possible NPE
                {
                    return false;
                }

                PlusEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.UserId, userData.User.Username);
                _habbo = userData.User;
                if (_habbo != null)
                {
                    userData.User.Init(this, userData);
                    CreateStats();
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        log("" + _habbo.Username + " just logged in");
                        dbClient.SetQuery("SELECT * FROM stats WHERE id = '" + _habbo.Id + "'");
                        DataRow data = dbClient.GetRow();
                        RP = new RPData(data, this);
                    }
                    SendPacket(new AuthenticationOkComposer());
                    SendPacket(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    if (userData.User.HomeRoom != 2 && GetRolePlay().Dead)
                        userData.User.HomeRoom = 2;
                    SendPacket(new NavigatorSettingsComposer(_habbo.HomeRoom));
                   // Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(userData.User.HomeRoom);
                
                    //if (Room == null)
                     //   userData.User.HomeRoom = 1;

                    SendPacket(new FavouritesComposer(userData.User.FavoriteRooms));
                    SendPacket(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingParts));
                    SendPacket(new UserRightsComposer(_habbo.Rank));
                    SendPacket(new AvailabilityStatusComposer());
                    SendPacket(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendPacket(new BuildersClubMembershipComposer());
                    SendPacket(new CfhTopicsInitComposer(PlusEnvironment.GetGame().GetModerationManager().UserActionPresets));

                    SendPacket(new BadgeDefinitionsComposer(PlusEnvironment.GetGame().GetAchievementManager().Achievements));
                    SendPacket(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendBarState)));
                    //SendMessage(new TalentTrackLevelComposer());

                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (_habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }


                    if (PlusEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup group))
                    {
                        if (!string.IsNullOrEmpty(group.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(group.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(group.Badge, true, this);
                    }

                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(_habbo.VipRank, out SubscriptionData subData))
                    {
                        if (!string.IsNullOrEmpty(subData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(subData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(subData.Badge, true, this);
                        }
                    }

                    if (!PlusEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        PlusEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.Look = PlusEnvironment.GetFigureManager().ProcessFigure(_habbo.Look, _habbo.Gender, _habbo.GetClothing().GetClothingParts, true);
                    _habbo.InitProcess();

                    if (userData.User.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendPacket(new ModeratorInitComposer(
                            PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                            PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                            PlusEnvironment.GetGame().GetModerationManager().GetTickets));
                    }
                    if (GetRolePlay().Dead)
                    {
                        if (GetRolePlay().MaxEnergy < 100)
                            GetRolePlay().MaxEnergy = 100;
                        GetRolePlay().prevMotto = userData.User.Motto;
                        GetRolePlay().habbo.Motto = "[Dead] " + userData.User.Motto;
                       // SendNotifi("You logged out while unconscious and have been transported to the nearest hospital!");
                    }
                    else if (GetRolePlay().Jailed > 0 || GetRolePlay().JailedSec > 0)
                    {
                        var WL = GetHabbo().GetClientManager().GetWL(GetHabbo().Username, 0);
                        //SendNotifi("You logged out while arrested, you have " + GetRolePlay().Jailed + " minute(s) and " + GetRolePlay().JailedSec + " second(s) left!");
                        if (WL != null)
                            GetHabbo().GetClientManager().RemoveWL(WL.ID);
                    }
                    if (PlusEnvironment.GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                        SendPacket(new MotdNotificationComposer(PlusEnvironment.GetLanguageManager().TryGetValue("user.login.message")));

                    PlusEnvironment.GetGame().GetRewardManager().CheckRewards(this);
                    return true;
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            return false;
        }

        public void SendWhisper(string message, int colour = 0)
        {
            if (GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser user = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (user == null)
                return;

            SendPacket(new WhisperComposer(user.VirtualId, message, 0, (colour == 0 ? user.LastBubble : colour)));
        }

        public void SendNotification(string message)
        {
            SendPacket(new BroadcastMessageAlertComposer(message));
        }

        public void SendPacket(MessageComposer message)
        {
            _channel.WriteAndFlushAsync(message);
        }

        public async void SendPacketsAsync(List<MessageComposer> messages)
        {
            foreach (MessageComposer message in messages)
            {
                await _channel.WriteAsync(message);
            }

            _channel.Flush();
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public RPData GetRolePlay()
        {
            return RP;
        }
        public RolePlayTimer RpTimer()
        {
            return RPTimer; 
        }
        public void CreateStats()
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT login FROM users WHERE id = '" + _habbo.Id + "'");
                int login = DB.GetInteger();
                if (login == 1)
                {
                    DB.RunQuery("UPDATE users SET login = '0' WHERE id = '" + _habbo.Id + "'");
                    DB.RunQuery("INSERT INTO `stats` (`id`) VALUES ('" + _habbo.Id + "')");
                    DB.RunQuery("INSERT INTO `inventory` (`id`) VALUES ('" + _habbo.Id + "')");
                    DB.RunQuery("INSERT INTO `bank` (`id`) VALUES ('" + _habbo.Id + "')");
                    DB.RunQuery("INSERT INTO `marco` (`id`) VALUES ('" + _habbo.Id + "')");
                }
            }
        }
        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (!_disconnected)
            {
               
                _channel?.CloseAsync();
                _disconnected = true;
            }
        }
        public void SendNotifi(string Message)
        {
       
            SendPacket(new VoucherRedeemErrorComposer(Convert.ToInt32(Message)));
        }

        public void Dispose(bool dc)
        {
            if (dc && GetHabbo() != null)
            {
                if (GetHabbo().FriendCount > 0)
                {
                    foreach (MessengerBuddy buddy in GetHabbo().GetMessenger().GetFriends())
                    {
                        if (buddy.IsOnline && buddy.Client != this)
                            buddy.Client.GetRolePlay().SendWeb("{\"name\":\"sidealert\", \"evnt\":\"offline\", \"name1\":\"" + GetHabbo().Username + "\", \"color\":\"" + GetRolePlay().Color + "\"}");
                    }
                }
                log("" + GetHabbo().Username + " logged out");
                if (GetRolePlay().onduty)
                {
                    GetRolePlay().onduty = false;
                    GetHabbo().GetClientManager().STAFFactive--;
                }
                if (GetRolePlay().roomUser.IsWalking)
                    GetRolePlay().roomUser.ClearMovement(true);
                if (GetRolePlay().Gang > 0)
                    GetRolePlay().SaveGangStats();
                if (GetRolePlay().WeedTimer > 0)
                    GetRolePlay().RPCache(23);
                GetRolePlay().SetHomeRoom();
                if (GetRolePlay().EscortID > 0)
                {
                    var user = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetRolePlay().EscortID);
                    if (user != null && user.GetClient().GetRolePlay().JobManager.Job == 1 && user.GetClient().GetRolePlay().JobManager.Working)
                        GetRolePlay().Jailed = 5;
                }
                if (GetRolePlay().EscortID > 0 || GetRolePlay().Escorting > 0)
                    GetRolePlay().EndEscort();
               // GetRolePlay().Room.SendPacket(new SleepComposer(GetRolePlay().roomUser, true));
                GetRolePlay().roomUser.IsAsleep = true;
                GetRolePlay().roomUser.Stunned = 60;
                GetRolePlay().roomUser.OnChat(34, "" + GetHabbo().Username + " is logging out", false);
                GetRolePlay().AutoLogout = 8;
                GetRolePlay().roomUser.ApplyEffect(13);
                GetRolePlay().RPCache(24);
                GetRolePlay().RPCache(10);
                if (GetRolePlay().ExCon)
                    GetRolePlay().RPCache(30);
                if (GetRolePlay().JobManager.Working)
                    GetRolePlay().Stopwork(false);
                if (GetRolePlay().GameType == "Brawl")
                    GetRolePlay().Room.Brawl -= 1;
                if (GetRolePlay().Trade.Trading && !GetRolePlay().roomUser.IsTrading)
                {
                    RoomUser User = GetRolePlay().Room.GetRoomUserManager().GetRoomUserByHabbo(GetRolePlay().TradeTarget);
                    User.GetClient().GetRolePlay().Trade.StopTrade();
                    User.GetClient().SendWhisper(GetHabbo().Username + " canceled the trade!");
                    GetRolePlay().Trade.TradeTimer = 0;
                    User.GetClient().GetRolePlay().Trade.TradeTimer = 0;
                    GetRolePlay().Trade.StopTrade();
                }
                GetRolePlay().Inventory.ItemCache(0, true);
                if (GetRolePlay().Storage.Curstorage > 0)
                    GetRolePlay().Storage.StorageCache(0, true);
            }
            else
            {
                if (GetHabbo() != null)
                    GetHabbo().OnDisconnect();
                this.MachineId = string.Empty;
                this._disconnected = true;
                this._habbo = null;
                this.RP = null;
               // this._connection = null;
                this.RC4Client = null;
               // this._packetParser = null;
            }

        }

        public void EnableEncryption(byte[] sharedKey)
        {
            _channel.Channel.Pipeline.AddFirst("gameCrypto", new EncryptionDecoder(sharedKey));
        }


      
    }
}