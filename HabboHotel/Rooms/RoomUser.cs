﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MySqlX.XDevAPI;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.Games.Freeze;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;


namespace Plus.HabboHotel.Rooms
{
    public class RoomUser
    {
        public bool AllowOverride;
        public BotAI BotAI;
        public RoomBot BotData;
        public bool CanWalk;
        public int CarryItemId; //byte
        public int CarryTimer; //byte
        public int ChatSpamCount;
        public int ChatSpamTicks = 16;
        public ItemEffectType CurrentItemEffect;
        public int DanceId;
        public bool FastWalking = false;
        public bool SuperFastWalking = false;
        public int FreezeCounter;
        public int FreezeLives;
        public bool Freezed;
        public bool Frozen;
        public int GateId;

        public int GoalX; //byte
        public int GoalY; //byte
        public int HabboId;
        public int HorseId = 0;
        public int IdleTime; //byte
        public bool InteractingGate;
        public int InternalRoomId;
        public bool IsAsleep;
        public bool IsWalking;
        public int LastBubble = 0;
        public double LastInteraction;
        public Item LastItem = null;
        public int LockedTilesCount;
        public bool petspawned;
        public List<Vector2D> Path = new();
        public bool PathRecalcNeeded;
        public int PathStep = 1;
        public bool CarrotPlucked;
        public Pet PetData;

        public int PrevTime;
        public bool RidingHorse = false;
        public int RoomId;
        public int RotBody; //byte
        public int RotHead; //byte

        public bool SetStep;
        public int SetX; //byte
        public int SetY; //byte
        public double SetZ;
        public double SignTime;
        public byte SqState;
        public int TeleDelay; //byte
        public bool TeleportEnabled;
        public bool UpdateNeeded;
        public int VirtualId;

        public int X; //byte
        public int Y; //byte
        public double Z;

        public FreezePowerUp BanzaiPowerUp;
        public bool IsLying = false;
        public bool IsSitting = false;
        private GameClient _mClient;
        private Room _mRoom;
        public bool MoonwalkEnabled = false;
        public bool ShieldActive;
        public int ShieldCounter;
        public Team Team;
        public bool FreezeInteracting;
        public int UserId;
        public bool IsJumping;

        public bool IsRolling = false;
        public int RollerDelay = 0;

        public int LLPartner = 0;
        public double TimeInRoom;

        #region RP Related
        internal int HungerTime;
        internal int EnergyTime;
        internal int EnergyTimer;
        internal int Assault;
        internal int Stunned;
        internal int DeadTimer;
        internal bool calledon;
        internal string LastMessage = "";
        internal int TaxiDest;
        internal int TaxiTime;
        internal int healtimer;
        internal bool spawnbot;
        internal bool ic;
        internal int SendRoom;
        internal bool reset;
        internal bool bypass;
        internal int PetDelay;
        internal int GameDelay = 50;
        internal int EatItem;
        internal int maxhealth = 100;
        internal int Faint;
        internal int Losehealth;
        internal int RockID;
        internal int RockTimer;
        internal int CarrotID;
        internal int CarrotTimer;
        internal int delay;
        internal int delayroom;
        internal int cine_tile;
        internal int cine_timer;
        internal int jailgate;
        internal int jailgate_timer;

        #endregion

        public RoomUser(int habboId, int roomId, int virtualId, Room room)
        {
            Freezed = false;
            HabboId = habboId;
            RoomId = roomId;
            VirtualId = virtualId;
            IdleTime = 0;

            X = 0;
            Y = 0;
            Z = 0;
            PrevTime = 0;
            RotHead = 0;
            RotBody = 0;
            UpdateNeeded = true;
            Statusses = new Dictionary<string, string>();

            TeleDelay = -1;
            _mRoom = room;

            AllowOverride = false;
            CanWalk = true;

            SqState = 3;

            InternalRoomId = 0;
            CurrentItemEffect = ItemEffectType.None;

            FreezeLives = 0;
            InteractingGate = false;
            GateId = 0;
            LastInteraction = 0;
            LockedTilesCount = 0;

            IsJumping = false;
            TimeInRoom = 0;

            TradeId = 0;
            TradePartner = 0;
            IsTrading = false;
        }

        public Point Coordinate => new(X, Y);

        public bool IsPet => IsBot && BotData.IsPet;

        public int CurrentEffect => GetClient().GetHabbo().Effects().CurrentEffect;

        public bool IsDancing => DanceId >= 1;

        public bool IsTrading { get; set; }

        public int TradePartner { get; set; }

        public int TradeId { get; set; }

        public Dictionary<string, string> Statusses { get; }

        public bool NeedsAutoKick
        {
            get
            {
                if (IsBot)
                    return false;

                if (GetClient() == null || GetClient().GetHabbo() == null)
                    return true;

                if (GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || GetRoom().OwnerId == HabboId)
                    return false;

                if (GetRoom().Id == 1649919)
                    return false;

                if (IdleTime >= 7200)
                    return true;

                return false;
            }
        }

        public bool IsBot => BotData != null;

        public string GetUsername()
        {
            if (IsBot)
                return string.Empty;

            if (GetClient() != null)
            {
                if (GetClient().GetHabbo() != null)
                {
                    return GetClient().GetHabbo().Username;
                }

                return PlusEnvironment.GetUsernameById(HabboId);
            }

            return PlusEnvironment.GetUsernameById(HabboId);
        }

        public void UnIdle()
        {
            if (!IsBot)
            {
                if (GetClient() != null && GetClient().GetHabbo() != null)
                    GetClient().GetHabbo().TimeAfk = 0;
            }

            IdleTime = 0;

            if (IsAsleep)
            {
                IsAsleep = false;
                GetRoom().SendPacket(new SleepComposer(VirtualId, false));
            }
        }

        public void Dispose()
        {
            Statusses.Clear();
            _mRoom = null;
            _mClient = null;
        }

        public void Chat(string message, int colour = 0)
        {
            if (GetRoom() == null)
                return;

            if (!IsBot)
                return;

            if (IsPet)
            {
                foreach (RoomUser user in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (user == null || user.IsBot)
                        continue;

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null)
                        return;

                    if (!user.GetClient().GetHabbo().AllowPetSpeech)
                        user.GetClient().SendPacket(new ChatComposer(VirtualId, message, 0, 0));
                }
            }
            else
            {
                foreach (RoomUser user in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (user == null || user.IsBot)
                        continue;
                    _mRoom.GetRoomUserManager().GetRoomUserByRank(2);
                    // Staff Get Blue Chat Bubble

                    //var This = session.GetRolePlay();
                    //if (Thishabbo.Rank >= 4)
                    //{
                    //    user.GetClient().SendPacket(new ChatComposer(VirtualId, message, 0, 21));
                    //}

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null)
                        return;

                    if (!user.GetClient().GetHabbo().AllowBotSpeech)
                        user.GetClient().SendPacket(new ChatComposer(VirtualId, message, 0, (colour == 0 ? 2 : colour)));
                }
            }
        }



        public void HandleSpamTicks()
        {
            if (ChatSpamTicks >= 0)
            {
                ChatSpamTicks--;

                if (ChatSpamTicks == -1)
                {
                    ChatSpamCount = 0;
                }
            }
        }

        public bool IncrementAndCheckFlood(out int muteTime)
        {
            muteTime = 0;

            ChatSpamCount++;
            if (ChatSpamTicks == -1)
                ChatSpamTicks = 8;
            else if (ChatSpamCount >= 6)
            {
                if (GetClient().GetHabbo().GetPermissions().HasRight("events_staff"))
                    muteTime = 3;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("gold_vip"))
                    muteTime = 7;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("silver_vip"))
                    muteTime = 10;
                else
                    muteTime = 20;

                GetClient().GetHabbo().FloodTime = PlusEnvironment.GetUnixTimestamp() + muteTime;

                ChatSpamCount = 0;
                return true;
            }

            return false;
        }

        public void OnChat(int colour, string message, bool shout)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null || _mRoom == null)
                return;

            if (_mRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSays, GetClient().GetHabbo(), message))
                return;

            GetClient().GetHabbo().HasSpoken = true;

            if (_mRoom.WordFilterList.Count > 0 && !GetClient().GetHabbo().GetPermissions().HasRight("word_filter_override"))
            {
                message = _mRoom.GetFilter().CheckMessage(message);
            }

            MessageComposer packet;
            if (shout)
                packet = new ShoutComposer(VirtualId, message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(message), colour);
            else
                packet = new ChatComposer(VirtualId, message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(message), colour);


            if (GetClient().GetHabbo().TentId > 0)
            {
                _mRoom.SendToTent(GetClient().GetHabbo().Id, GetClient().GetHabbo().TentId, packet);

                packet = new WhisperComposer(VirtualId, "[Tent Chat] " + message, 0, colour);

                List<RoomUser> toNotify = _mRoom.GetRoomUserManager().GetRoomUserByRank(2);

                if (toNotify.Count > 0)
                {
                    foreach (RoomUser user in toNotify)
                    {
                        if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null ||
                            user.GetClient().GetHabbo().TentId == GetClient().GetHabbo().TentId)
                        {
                            continue;
                        }

                        user.GetClient().SendPacket(packet);
                    }
                }
            }
            else
            {
                foreach (RoomUser user in _mRoom.GetRoomUserManager().GetRoomUsers().ToList())
                {
                    if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetIgnores().IgnoredUserIds().Contains(_mClient.GetHabbo().Id))
                        continue;

                    if (_mRoom.ChatDistance > 0 && Gamemap.TileDistance(X, Y, user.X, user.Y) > _mRoom.ChatDistance)
                        continue;

                    user.GetClient().SendPacket(packet);
                }
            }

            #region Pets/Bots responces

            if (shout)
            {
                foreach (RoomUser user in _mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!user.IsBot)
                        continue;

                    if (user.IsBot)
                        user.BotAI.OnUserShout(this, message);
                }
            }
            else
            {
                foreach (RoomUser user in _mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!user.IsBot)
                        continue;

                    if (user.IsBot)
                        user.BotAI.OnUserSay(this, message);
                }
            }

            #endregion
        }

        public void ClearMovement(bool update)
        {
            IsWalking = false;
            Statusses.Remove("mv");
            GoalX = 0;
            GoalY = 0;
            SetStep = false;
            SetX = 0;
            SetY = 0;
            SetZ = 0;

            if (update)
            {
                UpdateNeeded = true;
            }
        }

        public void MoveTo(Point c)
        {
            MoveTo(c.X, c.Y);
        }
        public void Jail(int time, string name)
        {
            if (GetClient().GetRolePlay().Inventory.Equip2 != "null")
                GetClient().GetRolePlay().WebHandler.Handle("equip", "", "e2");
            if (GetClient().GetRolePlay().Inventory.Equip1 != "null")
                GetClient().GetRolePlay().WebHandler.Handle("equip", "", "e1");
            if (GetClient().GetRolePlay().JobManager.Working)
                GetClient().GetRolePlay().Stopwork(false);
            GetClient().GetRolePlay().Assault = false;
            GetClient().GetRolePlay().Jailed = time;
            GetClient().GetRolePlay().Arrest(true);
            PlusEnvironment.GetGame().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"arrest\", \"name1\":\"" + name + "\", \"name2\":\"" + GetUsername() + "\"}");
        }
        public void MoveTo(int pX, int pY, bool pOverride)
        {
            if (TeleportEnabled)
            {
                UnIdle();
                GetRoom().SendPacket(GetRoom().GetRoomItemHandler().UpdateUserOnRoller(this, new Point(pX, pY), 0, GetRoom().GetGameMap().SqAbsoluteHeight(GoalX, GoalY)));
                if (Statusses.ContainsKey("sit"))
                    Z -= 0.35;
                UpdateNeeded = true;
                return;
            }

            if ((GetRoom().GetGameMap().SquareHasUsers(pX, pY) && !pOverride) || Frozen)
                return;

            UnIdle();

            GoalX = pX;
            GoalY = pY;
            PathRecalcNeeded = true;
            FreezeInteracting = false;
        }

        public void MoveTo(int pX, int pY)
        {
            MoveTo(pX, pY, false);
        }

        public void UnlockWalking()
        {
            AllowOverride = false;
            CanWalk = true;
        }


        public void SetPos(int pX, int pY, double pZ)
        {

            if (this.HabboId > 0)
            {
                var This = GetClient().GetRolePlay();
                if (This.item > 0)
                    CarryItem(This.item, This.itemtimer);
                if (This.Loggedin)
                   
                This.DisplayRoomInfo = true;
               This.LoadStats();
                if (This.Dead)
                {
                    This.BedSetup();
                    X = This.RoomX;
                    Y = This.RoomY;
                    Z = pZ;
                    RotBody = This.Rotate;
                    RotHead = This.Rotate;
                    Statusses.Add("lay", "2.0 null");
                    Z -= 0.35;
                    UpdateNeeded = true;
                    Stunned = 9999;
                    if (This.Loggedin)
                        This.Loggedin = false;
                }
                else if (This.Loggedin)
                {
                    This.Loggedin = false;
                    if (GetRoom().Id == 9 && This.Jailed == 0 && This.JailedSec == 0)
                    {
                        // set coordinate for new logins to spawn in jail
                        X = GetRoom().GetGameMap().Model.DoorX;
                        Y = GetRoom().GetGameMap().Model.DoorY;
                        Z = GetRoom().GetGameMap().Model.DoorZ;
                    }
                    else
                    {
                        if (GetRoom().GetGameMap().Path(This.RoomX, This.RoomY, this))
                        {
                            X = This.RoomX;
                            Y = This.RoomY;
                            Z = pZ;
                        }
                        else
                        {

                            X = GetRoom().GetGameMap().Model.DoorX;
                            Y = GetRoom().GetGameMap().Model.DoorY;
                            Z = GetRoom().GetGameMap().Model.DoorZ;
                        }
                    }
                    This.RoomX = 0;
                    This.RoomY = 0;
                }
                else
                {
                    X = pX;
                    Y = pY;
                    Z = pZ;

                }
            }
            else
            {
                X = pX;
                Y = pY;
                Z = pZ;
            }
        }

        public void CarryItem(int Item, int timer = 0)
        {
            CarryItemId = Item;

            if (Item > 0)
            {
                if (timer > 0)
                    CarryTimer = timer;
                else CarryTimer = 240;
                if (HabboId > 0)
                {
                    if (timer == 0)
                    {
                        GetClient().GetRolePlay().item = Item;
                        GetClient().GetRolePlay().itemtimer = 240;
                    }
                }
            }
            else
            {
                if (!IsBot)
                {
                    GetClient().GetRolePlay().item = 0;
                    GetClient().GetRolePlay().itemtimer = 0;
                }

                CarryTimer = 0;
                EatItem = 0;
            }
            if (!IsBot && (CurrentEffect == 592 || CurrentEffect == 591))
                ApplyEffect(0);
            GetRoom().SendPacket(new CarryObjectComposer(VirtualId, Item));
        }


        public void SetRot(int rotation, bool headOnly)
        {
            if (Statusses.ContainsKey("lay") || IsWalking)
            {
                return;
            }

            int diff = RotBody - rotation;

            RotHead = RotBody;

            if (Statusses.ContainsKey("sit") || headOnly)
            {
                if (RotBody == 2 || RotBody == 4)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
                else if (RotBody == 0 || RotBody == 6)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
            }
            else if (diff <= -2 || diff >= 2)
            {
                RotHead = rotation;
                RotBody = rotation;
            }
            else
            {
                RotHead = rotation;
            }

            UpdateNeeded = true;
        }

        public bool HasStatus(string key)
        {
            return Statusses.ContainsKey(key);
        }

        public void RemoveStatus(string key)
        {
            if (HasStatus(key))
                Statusses.Remove(key);
        }

        public void SetStatus(string key, string value = "")
        {
            if (Statusses.ContainsKey(key))
            {
                Statusses[key] = value;
            }
            else
            {
                Statusses.Add(key, value);
            }
        }

        public void ApplyEffect(int effectId)
        {
            if (IsBot)
            {
                _mRoom.SendPacket(new AvatarEffectComposer(VirtualId, effectId));
                return;
            }

            if (IsBot || GetClient() == null || GetClient().GetHabbo() == null || GetClient().GetHabbo().Effects() == null)
                return;

            GetClient().GetHabbo().Effects().ApplyEffect(effectId);
        }

        public Point SquareInFront
        {
            get
            {
                var sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    sq.Y--;
                }
                else if (RotBody == 2)
                {
                    sq.X++;
                }
                else if (RotBody == 4)
                {
                    sq.Y++;
                }
                else if (RotBody == 6)
                {
                    sq.X--;
                }

                return sq;
            }
        }

        public Point SquareBehind
        {
            get
            {
                var sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    sq.Y++;
                }
                else if (RotBody == 2)
                {
                    sq.X--;
                }
                else if (RotBody == 4)
                {
                    sq.Y--;
                }
                else if (RotBody == 6)
                {
                    sq.X++;
                }

                return sq;
            }
        }

        public Point SquareLeft
        {
            get
            {
                var sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    sq.X++;
                }
                else if (RotBody == 2)
                {
                    sq.Y--;
                }
                else if (RotBody == 4)
                {
                    sq.X--;
                }
                else if (RotBody == 6)
                {
                    sq.Y++;
                }

                return sq;
            }
        }

        public Point SquareRight
        {
            get
            {
                var sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    sq.X--;
                }
                else if (RotBody == 2)
                {
                    sq.Y++;
                }
                else if (RotBody == 4)
                {
                    sq.X++;
                }
                else if (RotBody == 6)
                {
                    sq.Y--;
                }

                return sq;
            }
        }

        public GameClient GetClient()
        {
            if (IsBot)
            {
                return null;
            }

            if (_mClient == null)
                _mClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(HabboId);
            return _mClient;
        }

        public Room GetRoom()
        {
            if (_mRoom == null)
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out _mRoom))
                    return _mRoom;

            return _mRoom;
        }

        
    }
}