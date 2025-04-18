﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Org.BouncyCastle.Asn1.X509;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;
using Plus.HabboHotel.Rooms.Trading;
using Plus.RolePlay;
using Plus.Utilities;

namespace Plus.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        private Room _room;
        private ConcurrentDictionary<int, RoomUser> _users;
        public ConcurrentDictionary<int, RoomUser> _bots;
        public ConcurrentDictionary<int, RoomUser> _pets;

        private int _primaryPrivateUserId;
        private int _secondaryPrivateUserId;

        public int UserCount;

        public RoomUserManager(Room room)
        {
            _room = room;
            _users = new ConcurrentDictionary<int, RoomUser>();
            _pets = new ConcurrentDictionary<int, RoomUser>();
            _bots = new ConcurrentDictionary<int, RoomUser>();

            _primaryPrivateUserId = 0;
            _secondaryPrivateUserId = 0;

            PetCount = 0;
            UserCount = 0;
        }

        public RoomUser DeployBot(RoomBot bot, Pet pet)
        {
            RoomUser user = new(0, _room.RoomId, _primaryPrivateUserId++, _room);
            bot.VirtualId = _primaryPrivateUserId;

            int personalId = _secondaryPrivateUserId++;
            user.InternalRoomId = personalId;
            _users.TryAdd(personalId, user);

            DynamicRoomModel model = _room.GetGameMap().Model;

            if ((bot.X > 0 && bot.Y > 0) && bot.X < model.MapSizeX && bot.Y < model.MapSizeY)
            {
                user.SetPos(bot.X, bot.Y, bot.Z);
                user.SetRot(bot.Rot, false);
            }
            else
            {
                bot.X = model.DoorX;
                bot.Y = model.DoorY;

                user.SetPos(model.DoorX, model.DoorY, model.DoorZ);
                user.SetRot(model.DoorOrientation, false);
            }

            user.BotData = bot;
            user.BotAI = bot.GenerateBotAI(user.VirtualId);

            if (user.IsPet)
            {
                user.BotAI.Init(bot.BotId, user.VirtualId, _room.RoomId, user, _room);
                user.PetData = pet;
                user.PetData.VirtualId = user.VirtualId;
            }
            else
                user.BotAI.Init(bot.BotId, user.VirtualId, _room.RoomId, user, _room);

            user.UpdateNeeded = true;

            _room.SendPacket(new UsersComposer(user));

            if (user.IsPet)
            {
                if (_pets.ContainsKey(user.PetData.PetId))
                    _pets[user.PetData.PetId] = user;
                else
                    _pets.TryAdd(user.PetData.PetId, user);

                PetCount++;
            }
            else if (user.IsBot)
            {
                if (_bots.ContainsKey(user.BotData.BotId))
                    _bots[user.BotData.BotId] = user;
                else
                    _bots.TryAdd(user.BotData.Id, user);

                _room.SendPacket(new DanceComposer(user.VirtualId, user.BotData.DanceId));
            }

            return user;
        }

        public void RemoveBot(int virtualId, bool kicked)
        {
            RoomUser user = GetRoomUserByVirtualId(virtualId);
            if (user == null || !user.IsBot)
                return;

            if (user.IsPet)
            {
                _pets.TryRemove(user.PetData.PetId, out RoomUser pet);
                PetCount--;
            }
            else
            {
                _bots.TryRemove(user.BotData.Id, out RoomUser bot);
            }

            user.BotAI.OnSelfLeaveRoom(kicked);

            _room.SendPacket(new UserRemoveComposer(user.VirtualId));

            if (_users != null)
                _users.TryRemove(user.InternalRoomId, out RoomUser toRemove);

            OnRemove(user);
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return _room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault();
        }

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (_room == null)
                return false;

            if (Session == null)
                return false;

            if (Session.GetHabbo().CurrentRoom == null)
                return false;

            #region Old Stuff
            RoomUser User = new RoomUser(Session.GetHabbo().Id, _room.RoomId, _primaryPrivateUserId++, _room);

            if (User == null || User.GetClient() == null)
                return false;

            User.UserId = Session.GetHabbo().Id;

            Session.GetHabbo().TentId = 0;

            int PersonalID = _secondaryPrivateUserId++;
            User.InternalRoomId = PersonalID;


            Session.GetHabbo().CurrentRoomId = _room.RoomId;
            if (!this._users.TryAdd(PersonalID, User))
                return false;
            #endregion

            DynamicRoomModel Model = _room.GetGameMap().Model;
            if (Model == null)
                return false;

            if (!_room.PetMorphsAllowed && Session.GetHabbo().PetId != 0)
                Session.GetHabbo().PetId = 0;
            if (_room.Id == 170 && Session.GetRolePlay().GameType == "Mafia")
            {
                var This = Session.GetRolePlay();
                User.ApplyEffect(4);
                This.TempEffect = 5;
                if (This.Team == "Blue")
                {
                    User.SetPos(7, 4, User.Z);
                    This.Rotate = 6;
                }
                else if (This.Team == "Green")
                {
                    User.SetPos(8, 10, User.Z);
                    This.Rotate = 2;
                }
                This.habbo.GetClientManager().GameInSession = true;
            }
            else if (!User.IsBot && (Session.GetHabbo().IsTeleporting || Session.GetHabbo().IsHopping || Session.GetHabbo().Arrowtele || Session.GetHabbo().Vaulttele > 0))
            {
                Item Item = null;
                if (Session.GetHabbo().IsTeleporting || User.GetClient().GetHabbo().Arrowtele)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().TeleportId);
                else if (Session.GetHabbo().IsHopping)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().HopperId);

                if (Item != null || Session.GetHabbo().Vaulttele > 0)
                {
                    if (Session.GetHabbo().IsTeleporting)
                    {
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                    }
                    else if (Session.GetHabbo().IsHopping)
                    {
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        User.AllowOverride = false;
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                    }
                    else if (Session.GetHabbo().Arrowtele)
                    {
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        Session.GetHabbo().Arrowtele = false;
                        Session.GetHabbo().TeleportId = 0;
                    }
                    else if (Session.GetHabbo().Vaulttele > 0)
                    {
                        if (Session.GetHabbo().Vaulttele == 5)
                            User.SetPos(10, 16, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 6)
                            User.SetPos(11, 16, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 7)
                            User.SetPos(12, 16, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 8)
                            User.SetPos(13, 16, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 1)
                            User.SetPos(6, 13, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 2)
                            User.SetPos(7, 13, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 3)
                            User.SetPos(8, 13, User.GetRoom().GetGameMap().Model.DoorZ);
                        else if (Session.GetHabbo().Vaulttele == 4)
                            User.SetPos(9, 13, User.GetRoom().GetGameMap().Model.DoorZ);
                        if (Session.GetHabbo().Vaulttele > 0 && Session.GetHabbo().Vaulttele < 5)
                            User.SetRot(0, false);
                        else User.SetRot(4, false);
                        Session.GetHabbo().Vaulttele = 0;
                        Session.GetHabbo().TeleportId = 0;
                    }
                }
                else
                {
                    User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ - 1);
                    User.SetRot(Model.DoorOrientation, false);
                }
            }
            else
            {
                #region escort postion and rotation
                if (!User.IsBot && !User.IsPet && User.GetClient().GetRolePlay().EscortID > 0)
                {
                    #region user
                    var user = GetRoomUserByHabbo(User.GetClient().GetRolePlay().EscortID);
                    int x = 0;
                    int y = 0;
                    if (user.GetClient().GetRolePlay().JobManager.Job == 7 && user.GetClient().GetRolePlay().JobManager.Working)
                    {
                        x = user.X;
                        y = user.Y;
                    }
                    else
                    {
                        if (user.RotBody == 4)
                        {
                            x = user.X;
                            y = user.Y + 1;
                        }
                        if (user.RotBody == 6)
                        {
                            x = user.X - 1;
                            y = user.Y;
                        }
                        if (user.RotBody == 0)
                        {
                            x = user.X;
                            y = user.Y - 1;
                        }
                        if (user.RotBody == 2)
                        {
                            x = user.X + 1;
                            y = user.Y;
                        }
                        if (user.RotBody == 5)
                        {
                            x = user.X - 1;
                            y = user.Y + 1;
                        }
                        if (user.RotBody == 1)
                        {
                            x = user.X + 1;
                            y = user.Y - 1;
                        }
                        if (user.RotBody == 3)
                        {
                            x = user.X + 1;
                            y = user.Y + 1;
                        }
                        if (user.RotBody == 7)
                        {
                            x = user.X - 1;
                            y = user.Y - 1;
                        }
                    }
                    if (!_room.GetGameMap().Path(x, y, User) && !user.GetClient().GetHabbo().InRoom)
                    {
                        x = Model.DoorX;
                        y = Model.DoorY;
                    }
                    User.SetPos(x, y, Model.DoorZ);
                    User.SetRot(user.RotBody, false);
                    #endregion
                }
                else if (!User.IsBot && !User.IsPet && User.GetClient().GetRolePlay().BotEscort > 0)
                {
                    #region bot
                    foreach (RoomUser user in _room.GetRoomUserManager()._bots.Values.ToList())
                        if (user.BotData.Id == User.GetClient().GetRolePlay().BotEscort)
                        {
                            int x = 0;
                            int y = 0;
                            if (user.RotBody == 4)
                            {
                                x = user.X;
                                y = user.Y + 1;
                            }
                            if (user.RotBody == 6)
                            {
                                x = user.X - 1;
                                y = user.Y;
                            }
                            if (user.RotBody == 0)
                            {
                                x = user.X;
                                y = user.Y - 1;
                            }
                            if (user.RotBody == 2)
                            {
                                x = user.X + 1;
                                y = user.Y;
                            }
                            if (user.RotBody == 5)
                            {
                                x = user.X - 1;
                                y = user.Y + 1;
                            }
                            if (user.RotBody == 1)
                            {
                                x = user.X + 1;
                                y = user.Y - 1;
                            }
                            if (user.RotBody == 3)
                            {
                                x = user.X + 1;
                                y = user.Y + 1;
                            }
                            if (user.RotBody == 7)
                            {
                                x = user.X - 1;
                                y = user.Y - 1;
                            }
                            User.SetPos(x, y, Model.DoorZ);
                            User.SetRot(user.RotBody, false);
                        }
                    #endregion
                }
                #endregion

                else
                {
                    User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                    User.SetRot(Model.DoorOrientation, false);
                }
            }

            _room.SendPacket(new UsersComposer(User));

            //Below = done
            if (_room.CheckRights(Session, true))
            {
                User.SetStatus("flatctrl", "useradmin");
                Session.SendPacket(new YouAreOwnerComposer());
                Session.SendPacket(new YouAreControllerComposer(4));
            }
            else if (_room.CheckRights(Session, false) && _room.Group == null)
            {
                User.SetStatus("flatctrl", "1");
                Session.SendPacket(new YouAreControllerComposer(1));
            }
            else if (_room.Group != null && _room.CheckRights(Session, false, true))
            {
                User.SetStatus("flatctrl", "3");
                Session.SendPacket(new YouAreControllerComposer(3));
            }
            else
                Session.SendPacket(new YouAreNotControllerComposer());

            User.UpdateNeeded = true;

            if (Session.GetHabbo().GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().DisableForcedEffects)
                Session.GetHabbo().Effects().ApplyEffect(102);

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);
            }
            return true;
        }

        public void RemoveUserFromRoom(GameClient session, bool notifyUser, bool notifyKick = false)
        {
            try
            {
                if (_room == null)
                    return;

                if (session == null || session.GetHabbo() == null)
                    return;

                if (notifyKick)
                    session.SendPacket(new GenericErrorComposer(4008));

                if (notifyUser)
                    session.SendPacket(new CloseConnectionComposer());

                if (session.GetHabbo().TentId > 0)
                    session.GetHabbo().TentId = 0;

                RoomUser user = GetRoomUserByHabbo(session.GetHabbo().Id);
                if (user != null)
                {
                    if (user.RidingHorse)
                    {
                        user.RidingHorse = false;
                        RoomUser userRiding = GetRoomUserByVirtualId(user.HorseId);
                        if (userRiding != null)
                        {
                            userRiding.RidingHorse = false;
                            userRiding.HorseId = 0;
                        }
                    }

                    if (user.Team != Team.None)
                    {
                        TeamManager team = _room.GetTeamManagerForFreeze();
                        if (team != null)
                        {
                            team.OnUserLeave(user);

                            user.Team = Team.None;

                            if (user.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                                user.GetClient().GetHabbo().Effects().ApplyEffect(0);
                        }
                    }

                    RemoveRoomUser(user);

                    if (user.CurrentItemEffect != ItemEffectType.None)
                    {
                        if (session.GetHabbo().Effects() != null)
                            session.GetHabbo().Effects().CurrentEffect = -1;
                    }

                    if (user.IsTrading)
                    {
                        if (_room.GetTrading().TryGetTrade(user.TradeId, out Trade trade))
                            trade.EndTrade(user.TradeId);
                    }

                    //Session.GetHabbo().CurrentRoomId = 0;

                    if (session.GetHabbo().GetMessenger() != null)
                        session.GetHabbo().GetMessenger().OnStatusChanged(true);

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE user_roomvisits SET exit_timestamp = '" + PlusEnvironment.GetUnixTimestamp() + "' WHERE room_id = '" + _room.RoomId + "' AND user_id = '" + session.GetHabbo().Id + "' ORDER BY exit_timestamp DESC LIMIT 1");
                        dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '" + _room.UsersNow + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
                    }

                    user.Dispose();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        private void OnRemove(RoomUser user)
        {
            try
            {
                GameClient session = user.GetClient();
                if (session == null)
                    return;

                List<RoomUser> bots = new();

                try
                {
                    foreach (RoomUser roomUser in GetUserList().ToList())
                    {
                        if (roomUser == null)
                            continue;

                        if (roomUser.IsBot && !roomUser.IsPet)
                        {
                            if (!bots.Contains(roomUser))
                                bots.Add(roomUser);
                        }
                    }
                }
                catch
                {
                }

                List<RoomUser> petsToRemove = new();
                foreach (RoomUser bot in bots.ToList())
                {
                    if (bot == null || bot.BotAI == null)
                        continue;

                    bot.BotAI.OnUserLeaveRoom(session);

                    if (bot.IsPet && bot.PetData.OwnerId == user.UserId && !_room.CheckRights(session, true))
                    {
                        if (!petsToRemove.Contains(bot))
                            petsToRemove.Add(bot);
                    }
                }

                foreach (RoomUser toRemove in petsToRemove.ToList())
                {
                    if (toRemove == null)
                        continue;

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetInventoryComponent() == null)
                        continue;

                    if (user.GetClient().GetHabbo().GetInventoryComponent().TryAddPet(toRemove.PetData))
                    {
                        toRemove.PetData.RoomId = 0;
                        toRemove.PetData.PlacedInRoom = false;

                        RemoveBot(toRemove.VirtualId, false);
                    }
                }

                _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            }
            catch (Exception e)
            {
                ExceptionLogger.LogCriticalException(e);
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            if (user.SetStep)
                _room.GetGameMap().GameMap[user.SetX, user.SetY] = user.SqState;
            else
                _room.GetGameMap().GameMap[user.X, user.Y] = user.SqState;

            _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            _room.SendPacket(new UserRemoveComposer(user.VirtualId));

            if (_users.TryRemove(user.InternalRoomId, out RoomUser toRemove))
            {
                //uhmm, could put the below stuff in but idk.
            }

            user.InternalRoomId = -1;
            OnRemove(user);
        }

        public bool TryGetPet(int petId, out RoomUser pet)
        {
            return _pets.TryGetValue(petId, out pet);
        }

        public bool TryGetBot(int botId, out RoomUser bot)
        {
            return _bots.TryGetValue(botId, out bot);
        }

        public RoomUser GetBotByName(string name)
        {
            bool foundBot = _bots.Any(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == name.ToLower());
            if (foundBot)
            {
                int id = _bots.FirstOrDefault(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == name.ToLower()).Value.BotData.Id;

                return _bots[id];
            }

            return null;
        }

        public void UpdateUserCount(int count)
        {
            UserCount = count;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '" + count + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
            }
        }

        public RoomUser GetRoomUserByVirtualId(int virtualId)
        {
            if (!_users.TryGetValue(virtualId, out RoomUser user))
                return null;
            return user;
        }

        public RoomUser GetRoomUserByHabbo(int id)
        {
            return GetUserList().FirstOrDefault(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Id == id);
        }

        public List<RoomUser> GetRoomUsers()
        {
            return GetUserList().Where(x => !x.IsBot).ToList();
        }

        public List<RoomUser> GetRoomUserByRank(int minRank)
        {
            var returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank >= minRank)
                    returnList.Add(user);
            }

            return returnList;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            return GetUserList().FirstOrDefault(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username.Equals(pName, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdatePets()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (Pet pet in GetPets().ToList())
                {
                    if (pet == null)
                        continue;

                    if (pet.DbState == PetDatabaseUpdateState.NeedsInsert)
                    {
                        dbClient.SetQuery("INSERT INTO `bots` (`id`,`user_id`,`room_id`,`name`,`x`,`y`,`z`) VALUES ('" + pet.PetId + "','" + pet.OwnerId + "','" + pet.RoomId + "',@name,'0','0','0')");
                        dbClient.AddParameter("name", pet.Name);
                        dbClient.RunQuery();

                        dbClient.SetQuery("INSERT INTO `bots_petdata` (`type`,`race`,`color`,`experience`,`energy`,`createstamp`,`nutrition`,`respect`) VALUES ('" + pet.Type + "',@race,@color,'0','100','" + pet.CreationStamp + "','0','0')");
                        dbClient.AddParameter(pet.PetId + "race", pet.Race);
                        dbClient.AddParameter(pet.PetId + "color", pet.Color);
                        dbClient.RunQuery();
                    }
                    else if (pet.DbState == PetDatabaseUpdateState.NeedsUpdate)
                    {
                        //Surely this can be *99 better? // TODO
                        RoomUser user = GetRoomUserByVirtualId(pet.VirtualId);

                        dbClient.RunQuery("UPDATE `bots` SET room_id = " + pet.RoomId + ", x = " + (user != null ? user.X : 0) + ", Y = " + (user != null ? user.Y : 0) + ", Z = " + (user != null ? user.Z : 0) + " WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                        dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + pet.Experience + "', `energy` = '" + pet.Energy + "', `nutrition` = '" + pet.Nutrition + "', `respect` = '" + pet.Respect + "' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                    }

                    pet.DbState = PetDatabaseUpdateState.Updated;
                }
            }
        }

        private void UpdateBots()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (RoomUser user in GetRoomUsers().ToList())
                {
                    if (user == null || !user.IsBot)
                        continue;

                    if (user.IsBot)
                    {
                        dbClient.SetQuery("UPDATE bots SET x=@x, y=@y, z=@z, name=@name, look=@look, rotation=@rotation WHERE id=@id LIMIT 1;");
                        dbClient.AddParameter("name", user.BotData.Name);
                        dbClient.AddParameter("look", user.BotData.Look);
                        dbClient.AddParameter("rotation", user.BotData.Rot);
                        dbClient.AddParameter("x", user.X);
                        dbClient.AddParameter("y", user.Y);
                        dbClient.AddParameter("z", user.Z);
                        dbClient.AddParameter("id", user.BotData.BotId);
                        dbClient.RunQuery();
                    }
                }
            }
        }

        public List<Pet> GetPets()
        {
            List<Pet> pets = new();
            foreach (RoomUser user in _pets.Values.ToList())
            {
                if (user == null || !user.IsPet)
                    continue;

                pets.Add(user.PetData);
            }

            return pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> users = new();
            ICollection<RoomUser> roomUsers = GetUserList();

            if (roomUsers == null)
                return;

            foreach (RoomUser user in roomUsers.ToList())
            {
                if (user == null || !user.UpdateNeeded || users.Contains(user))
                    continue;

                user.UpdateNeeded = false;
                users.Add(user);
            }

            if (users.Count > 0)
                _room.SendPacket(new UserUpdateComposer(users));
        }

        public void UpdateUserStatusses()
        {
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                UpdateUserStatus(user, false);
            }
        }

        private bool IsValid(RoomUser user)
        {
            if (user == null)
                return false;
            if (user.IsBot)
                return true;
            if (user.GetClient() == null)
                return false;
            if (user.GetClient().GetHabbo() == null)
                return false;
            if (user.GetClient().GetHabbo().CurrentRoomId != _room.RoomId)
                return false;
            return true;
        }


        public void OnCycle()
        {
            int userCounter = 0;

            try
            {
                List<RoomUser> ToRemove = new List<RoomUser>();

                foreach (RoomUser User in GetUserList().ToList())
                {

                    if (User == null)
                        continue;
                    if (!IsValid(User))
                    {
                        if (User.GetClient() != null)
                            RemoveUserFromRoom(User.GetClient(), false, false);
                        else
                            RemoveRoomUser(User);
                    }
                    if (!User.IsBot && !User.IsPet)
                        // User.GetClient().GetRolePlay().RPTimer.OnCycle();
                        User.GetClient().GetRolePlay().GetTimer().OnCycle();
                    if (User.cine_tile > 0)
                    {
                        User.cine_timer--;
                        if (User.cine_timer < 1)
                        {
                            Item Item = _room.GetRoomItemHandler().GetItem(User.cine_tile);
                            if (Item != null)
                            {
                                int Distance = Math.Abs(User.X - Item.GetX) + Math.Abs(User.Y - Item.GetY);
                                if (Distance == 0)
                                    User.cine_timer = 2;
                                else
                                {
                                    Item.ExtraData = "0";
                                    Item.UpdateState(false, true);
                                    User.cine_tile = 0;
                                }
                            }
                            else
                            {
                                User.cine_tile = 0;
                                User.cine_timer = 0;
                            }
                        }
                    }
                    if (User.jailgate > 0)
                    {
                        User.jailgate_timer--;
                        if (User.jailgate_timer < 1)
                        {
                            Item Item = _room.GetRoomItemHandler().GetItem(User.jailgate);
                            if (Item != null)
                            {
                                int Distance = Math.Abs(User.X - Item.GetX) + Math.Abs(User.Y - Item.GetY);
                                if (Distance == 0)
                                    User.jailgate_timer = 2;
                                else
                                {
                                    Item.ExtraData = "0";
                                    Item.UpdateState(false, true);
                                    User.jailgate = 0;
                                }
                            }
                            else
                            {
                                User.jailgate = 0;
                                User.jailgate_timer = 0;
                            }
                            if (!User.IsBot && !User.IsPet)
                            {
                                User.GetClient().GetRolePlay().ExCon = false;
                                User.GetClient().GetRolePlay().RPCache(30);
                            }
                        }
                    }
                    bool updated = false;
                    User.IdleTime++;
                    User.HandleSpamTicks();
                    if (!User.IsBot && !User.IsAsleep)
                    {
                        if (User.IdleTime >= 300 || User.GetClient().GetRolePlay().isSleeping && User.IdleTime >= 15)
                        {
                            if (User.GetClient().GetRolePlay().GameType != "" && User.GetRoom().Id == 23)
                                User.GetClient().GetRolePlay().LeaveGame(true, true);
                            User.IsAsleep = true;
                            if (User.GetClient().GetRolePlay().JobManager.Working)
                                User.GetClient().GetRolePlay().Stopwork(false);
                           // else _room.SendPacket(new SleepComposer(User, true));
                        }
                    }

                    if (User.CarryItemId > 0)
                    {
                        User.CarryTimer--;
                        if (User.HabboId > 0)
                        {
                            User.GetClient().GetRolePlay().itemtimer--;
                            if (User.CarryTimer <= 0)
                                User.GetClient().GetRolePlay().item = 0;
                        }
                        if (User.CarryTimer <= 0)
                            User.CarryItem(0);
                    }

                    if (_room.GotFreeze())
                        _room.GetFreeze().CycleUser(User);

                    bool InvalidStep = false;

                    if (User.IsRolling)
                    {
                        if (User.RollerDelay <= 0)
                        {
                            UpdateUserStatus(User, false);
                            User.IsRolling = false;
                        }
                        else
                            User.RollerDelay--;
                    }

                    if (User.SetStep)
                    {
                        if (_room.GetGameMap().IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(User.SetX, User.SetY), (User.GoalX == User.SetX && User.GoalY == User.SetY), User.AllowOverride))
                        {
                            if (!User.RidingHorse)
                                _room.GetGameMap().UpdateUserMovement(new Point(User.Coordinate.X, User.Coordinate.Y), new Point(User.SetX, User.SetY), User);

                            List<Item> items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in items.ToList())
                            {
                                Item.UserWalksOffFurni(User);
                            }

                            if (!User.IsBot)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }
                            else if (User.IsBot && !User.RidingHorse)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }

                            if (!User.IsBot && User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseId);
                                if (Horse != null)
                                {
                                    Horse.X = User.SetX;
                                    Horse.Y = User.SetY;
                                }
                            }

                            List<Item> Items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in Items.ToList())
                            {
                                Item.UserWalksOnFurni(User);
                            }
                            UpdateUserStatus(User, true);
                        }
                        else
                            InvalidStep = true;
                        User.SetStep = false;
                    }

                    if (User.PathRecalcNeeded)
                    {
                        if (User.Path.Count > 1)
                            User.Path.Clear();
                        User.Path = PathFinder.FindPath(User, this._room.GetGameMap().DiagonalEnabled, this._room.GetGameMap(), new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));
                        if (User.Path.Count > 1)
                        {
                            User.PathStep = 1;
                            User.IsWalking = true;
                            User.PathRecalcNeeded = false;
                        }
                        else
                        {
                            User.PathRecalcNeeded = false;
                            if (User.Path.Count > 1)
                                User.Path.Clear();
                        }
                    }

                    if (User.IsWalking && !User.Freezed)
                    {
                        if (InvalidStep || (User.PathStep >= User.Path.Count) || (User.GoalX == User.X && User.GoalY == User.Y)) //No path found, or reached goal (:
                                                                                                                                 // if (!ArrowPath && (InvalidStep || (User.PathStep >= User.Path.Count) || (User.GoalX == User.X && User.GoalY == User.Y))) //No path found, or reached goal (:
                        {
                            User.IsWalking = false;
                            User.RemoveStatus("mv");

                            if (User.Statusses.ContainsKey("sign"))
                                User.RemoveStatus("sign");

                            if (User.IsBot && User.BotData.TargetUser > 0)
                            {
                                if (User.CarryItemId > 0)
                                {
                                    RoomUser Target = _room.GetRoomUserManager().GetRoomUserByHabbo(User.BotData.TargetUser);

                                    if (Target != null && Gamemap.TilesTouching(User.X, User.Y, Target.X, Target.Y))
                                    {
                                        User.SetRot(Rotation.Calculate(User.X, User.Y, Target.X, Target.Y), false);
                                        Target.SetRot(Rotation.Calculate(Target.X, Target.Y, User.X, User.Y), false);
                                        Target.CarryItem(User.CarryItemId);
                                    }
                                }

                                User.CarryItem(0);
                                User.BotData.TargetUser = 0;
                            }

                            if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                            {
                                RoomUser mascotaVinculada = GetRoomUserByVirtualId(User.HorseId);
                                if (mascotaVinculada != null)
                                {
                                    mascotaVinculada.IsWalking = false;
                                    mascotaVinculada.RemoveStatus("mv");
                                    mascotaVinculada.UpdateNeeded = true;
                                }
                            }
                        }
                        else
                        {
                            Vector2D NextStep = User.Path[(User.Path.Count - User.PathStep) - 1];
                            User.PathStep++;
                            if ((User.FastWalking || User.SuperFastWalking) && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                            }
                            #region cine tile
                            if ((!User.IsBot && (User.GetClient().GetRolePlay().JobManager.Working || User.GetClient().GetHabbo().Rank > 4))
                                || (User.IsBot))
                                foreach (Item Item in _room.GetRoomItemHandler().GetFurniObjects(NextStep.X, NextStep.Y).ToList())
                                {
                                    if (Item.BaseItem == 89929)
                                    {
                                        Item.ExtraData = "1";
                                        Item.UpdateState(false, true);


                                        if (User.cine_tile > 0)
                                        {
                                            Item item = _room.GetRoomItemHandler().GetItem(User.cine_tile);
                                            if (Item != null)
                                            {
                                                item.ExtraData = "0";
                                                item.UpdateState(false, true);
                                                User.cine_tile = 0;
                                                User.cine_timer = 0;
                                            }
                                        }
                                        User.cine_tile = Item.Id;
                                        User.cine_timer = 2;
                                    }
                                }
                            #endregion
                            #region prison gate
                            if ((!User.IsBot && (User.GetClient().GetRolePlay().JobManager.Working || User.GetClient().GetHabbo().Rank > 4 || User.GetClient().GetRolePlay().ExCon))
                                || (User.IsBot))
                                foreach (Item Item in _room.GetRoomItemHandler().GetFurniObjects(NextStep.X, NextStep.Y).ToList())
                                {
                                    if (Item.BaseItem == 39912)
                                    {
                                        Item.ExtraData = "1";
                                        Item.UpdateState(false, true);
                                        if (User.jailgate > 0)
                                        {
                                            Item item = _room.GetRoomItemHandler().GetItem(User.jailgate);
                                            if (Item != null)
                                            {
                                                item.ExtraData = "0";
                                                item.UpdateState(false, true);
                                                User.jailgate = 0;
                                                User.jailgate_timer = 0;
                                            }
                                        }
                                        User.jailgate = Item.Id;
                                        User.jailgate_timer = 2;
                                    }
                                }
                            #endregion

                            int nextX = NextStep.X;
                            int nextY = NextStep.Y;
                            User.RemoveStatus("mv");
                            if (_room.GetGameMap().IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(nextX, nextY), (User.GoalX == nextX && User.GoalY == nextY), User.AllowOverride))
                            {
                                double nextZ = _room.GetGameMap().SqAbsoluteHeight(nextX, nextY);
                                if (!User.IsBot)
                                {
                                    if (User.IsSitting)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.IsSitting = false;
                                        User.UpdateNeeded = true;
                                    }
                                    else if (User.IsLying)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.IsLying = false;
                                        User.UpdateNeeded = true;
                                    }
                                }
                                if (!User.IsBot)
                                {
                                    User.Statusses.Remove("lay");
                                    User.Statusses.Remove("sit");
                                }

                                if (!User.IsBot && !User.IsPet && User.GetClient() != null)
                                {
                                    if (User.GetClient().GetHabbo().IsTeleporting)
                                    {
                                        User.GetClient().GetHabbo().IsTeleporting = false;
                                        User.GetClient().GetHabbo().TeleportId = 0;
                                    }
                                    else if (User.GetClient().GetHabbo().IsHopping)
                                    {
                                        User.GetClient().GetHabbo().IsHopping = false;
                                        User.GetClient().GetHabbo().HopperId = 0;
                                    }
                                }

                                if (!User.IsBot && User.RidingHorse && User.IsPet == false)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseId);
                                    if (Horse != null)
                                        Horse.SetStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                    User.SetStatus("mv", +nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));

                                    User.UpdateNeeded = true;
                                    Horse.UpdateNeeded = true;
                                }
                                else
                                    User.SetStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                int newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.MoonwalkEnabled);

                                User.RotBody = newRot;
                                User.RotHead = newRot;

                                User.SetStep = true;
                                User.SetX = nextX;
                                User.SetY = nextY;
                                User.SetZ = nextZ;
                                UpdateUserEffect(User, User.SetX, User.SetY);

                                updated = true;

                                #region Escort Movement
                                if (!User.IsBot && !User.IsPet && User.GetClient().GetRolePlay().Escorting > 0)
                                {
                                    var user = GetRoomUserByHabbo(User.GetClient().GetRolePlay().Escorting);
                                    if (user != null)
                                    {
                                        var roomUser = User;
                                        if (User.AllowOverride != true)
                                            User.AllowOverride = true;
                                        if (User.GetClient().GetRolePlay().JobManager.Job == 7 && User.GetClient().GetRolePlay().JobManager.Working)
                                        {
                                            if (newRot == 0)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 1)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 2)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 3)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 4)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 5)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 6)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 7)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            if (User.RotBody != user.RotBody)
                                                user.GetClient().GetRolePlay().SetRot(User.RotBody);
                                        }
                                        else
                                        {
                                            user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY);
                                            if (newRot == 0)
                                                user.MoveTo(roomUser.SetX, roomUser.SetY - 1);
                                            else if (newRot == 1)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY - 1);
                                            else if (newRot == 2)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY);
                                            else if (newRot == 3)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY + 1);
                                            else if (newRot == 4)
                                                user.MoveTo(roomUser.SetX, roomUser.SetY + 1);
                                            else if (newRot == 5)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY + 1);
                                            else if (newRot == 6)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY);
                                            else if (newRot == 7)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY - 1);
                                            if (User.RotBody != user.RotBody)
                                                user.GetClient().GetRolePlay().SetRot(User.RotBody);
                                        }
                                    }
                                }
                                if (User.IsBot && User.BotData.Escorting > 0)
                                {
                                    var user = GetRoomUserByHabbo(User.BotData.Escorting);
                                    if (user != null)
                                    {
                                        var roomUser = User;
                                        if (User.AllowOverride != true)
                                            User.AllowOverride = true;
                                        if (User.BotData.Job == 1 || User.BotData.Job == 4 || User.BotData.Job == 8)
                                        {
                                            user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY);
                                            user.SetStatus("mv", "");
                                            if (newRot == 0)
                                                user.MoveTo(roomUser.SetX, roomUser.SetY - 1);
                                            else if (newRot == 1)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY - 1);
                                            else if (newRot == 2)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY);
                                            else if (newRot == 3)
                                                user.MoveTo(roomUser.SetX + 1, roomUser.SetY + 1);
                                            else if (newRot == 4)
                                                user.MoveTo(roomUser.SetX, roomUser.SetY + 1);
                                            else if (newRot == 5)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY + 1);
                                            else if (newRot == 6)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY);
                                            else if (newRot == 7)
                                                user.MoveTo(roomUser.SetX - 1, roomUser.SetY - 1);
                                            if (User.RotBody != user.RotBody)
                                                user.GetClient().GetRolePlay().SetRot(User.RotBody);
                                        }
                                        else if (User.BotData.Job == 2)
                                        {
                                            if (newRot == 0)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 1)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 2)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 3)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX - 1, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 4)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 5)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY - 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 6)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            else if (newRot == 7)
                                            {
                                                user.GetClient().GetRolePlay().Warp(roomUser.SetX + 1, roomUser.SetY + 1);
                                                user.MoveTo(roomUser.SetX, roomUser.SetY);
                                            }
                                            if (User.RotBody != user.RotBody)
                                                user.GetClient().GetRolePlay().SetRot(User.RotBody);
                                        }
                                    }
                                }
                                #endregion

                                if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseId);
                                    if (Horse != null)
                                    {
                                        Horse.RotBody = newRot;
                                        Horse.RotHead = newRot;

                                        Horse.SetStep = true;
                                        Horse.SetX = nextX;
                                        Horse.SetY = nextY;
                                        Horse.SetZ = nextZ;
                                    }
                                }
                                _room.GetGameMap().GameMap[User.X, User.Y] = User.SqState; // REstore the old one
                                User.SqState = _room.GetGameMap().GameMap[User.SetX, User.SetY]; //Backup the new one
                                if (_room.RoomBlockingEnabled == 0)
                                {
                                    RoomUser Users = _room.GetRoomUserManager().GetUserForSquare(nextX, nextY);
                                    if (Users != null)
                                        _room.GetGameMap().GameMap[nextX, nextY] = 0;
                                }
                                else
                                    _room.GetGameMap().GameMap[nextX, nextY] = 1;
                            }

                        }
                        if (!User.RidingHorse)
                            User.UpdateNeeded = true;
                    }
                    else
                    {
                        if (User.Statusses.ContainsKey("mv"))
                        {
                            User.RemoveStatus("mv");
                            User.UpdateNeeded = true;

                            if (User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseId);
                                if (Horse != null)
                                {
                                    Horse.RemoveStatus("mv");
                                    Horse.UpdateNeeded = true;
                                }
                            }
                        }
                    }

                    if (User.RidingHorse)
                        User.ApplyEffect(77);

                    if (User.IsBot && User.BotAI != null)
                        User.BotAI.OnTimerTick();
                    else
                        userCounter++;

                    if (!updated)
                        UpdateUserEffect(User, User.X, User.Y);
                }

                foreach (RoomUser toRemove in ToRemove.ToList())
                {
                    GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(toRemove.HabboId);
                    if (client != null)
                        RemoveUserFromRoom(client, true, false);
                    else
                        RemoveRoomUser(toRemove);
                }

                if (UserCount != userCounter)
                    UpdateUserCount(userCounter);
            }
            catch (Exception e)
            {
                int rId = 0;
                if (_room != null)
                    rId = _room.Id;

                //Logging.LogCriticalException("Affected Room - ID: " + rId + " - " + e.ToString());
            }
        }

        public void UpdateUserStatus(RoomUser user, bool cycleGameItems)
        {
            if (user == null)
                return;

            try
            {
                bool isBot = user.IsBot;
                if (isBot)
                    cycleGameItems = false;

                if (PlusEnvironment.GetUnixTimestamp() > PlusEnvironment.GetUnixTimestamp() + user.SignTime)
                {
                    if (user.Statusses.ContainsKey("sign"))
                    {
                        user.Statusses.Remove("sign");
                        user.UpdateNeeded = true;
                    }
                }

                if ((user.Statusses.ContainsKey("lay") && !user.IsLying) || (user.Statusses.ContainsKey("sit") && !user.IsSitting))
                {
                    if (user.Statusses.ContainsKey("lay"))
                        user.Statusses.Remove("lay");
                    if (user.Statusses.ContainsKey("sit"))
                        user.Statusses.Remove("sit");
                    user.UpdateNeeded = true;
                }
                else if (user.IsLying || user.IsSitting)
                    return;

                double newZ;
                List<Item> itemsOnSquare = _room.GetGameMap().GetAllRoomItemForSquare(user.X, user.Y);
                if (itemsOnSquare != null || itemsOnSquare.Count != 0)
                {
                    if (user.RidingHorse && user.IsPet == false)
                        newZ = _room.GetGameMap().SqAbsoluteHeight(user.X, user.Y, itemsOnSquare.ToList()) + 1;
                    else
                        newZ = _room.GetGameMap().SqAbsoluteHeight(user.X, user.Y, itemsOnSquare.ToList());
                }
                else
                {
                    newZ = 1;
                }

                if (newZ != user.Z)
                {
                    user.Z = newZ;
                    user.UpdateNeeded = true;
                }

                DynamicRoomModel model = _room.GetGameMap().Model;
                if (model.SqState[user.X, user.Y] == SquareState.Seat)
                {
                    if (!user.Statusses.ContainsKey("sit"))
                        user.Statusses.Add("sit", "1.0");
                    user.Z = model.SqFloorHeight[user.X, user.Y];
                    user.RotHead = model.SqSeatRot[user.X, user.Y];
                    user.RotBody = model.SqSeatRot[user.X, user.Y];

                    user.UpdateNeeded = true;
                }


                if (itemsOnSquare.Count == 0)
                    user.LastItem = null;


                foreach (Item item in itemsOnSquare.ToList())
                {
                    if (item == null)
                        continue;

                    if (item.GetBaseItem().IsSeat)
                    {
                        if (!user.Statusses.ContainsKey("sit"))
                        {
                            if (!user.Statusses.ContainsKey("sit"))
                                user.Statusses.Add("sit", TextHandling.GetString(item.GetBaseItem().Height));
                        }

                        user.Z = item.GetZ;
                        user.RotHead = item.Rotation;
                        user.RotBody = item.Rotation;
                        user.UpdateNeeded = true;
                    }

                    switch (item.GetBaseItem().InteractionType)
                    {
                        #region Beds & Tents

                        case InteractionType.Bed:
                        case InteractionType.TentSmall:
                            {
                                if (!user.Statusses.ContainsKey("lay"))
                                    user.Statusses.Add("lay", TextHandling.GetString(item.GetBaseItem().Height) + " null");

                                user.Z = item.GetZ;
                                user.RotHead = item.Rotation;
                                user.RotBody = item.Rotation;

                                user.UpdateNeeded = true;
                                break;
                            }

                        #endregion

                        #region Banzai Gates

                        case InteractionType.BanzaiGateGreen:
                        case InteractionType.BanzaiGateBlue:
                        case InteractionType.BanzaiGateRed:
                        case InteractionType.BanzaiGateYellow:
                            {
                                if (cycleGameItems)
                                {
                                    int effectId = Convert.ToInt32(item.Team + 32);
                                    TeamManager t = user.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForBanzai();

                                    if (user.Team == Team.None)
                                    {
                                        if (t.CanEnterOnTeam(item.Team))
                                        {
                                            if (user.Team != Team.None)
                                                t.OnUserLeave(user);
                                            user.Team = item.Team;

                                            t.AddUser(user);

                                            if (user.GetClient().GetHabbo().Effects().CurrentEffect != effectId)
                                                user.GetClient().GetHabbo().Effects().ApplyEffect(effectId);
                                        }
                                    }
                                    else if (user.Team != Team.None && user.Team != item.Team)
                                    {
                                        t.OnUserLeave(user);
                                        user.Team = Team.None;
                                        user.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(user);
                                        if (user.GetClient().GetHabbo().Effects().CurrentEffect == effectId)
                                            user.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        user.Team = Team.None;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);                                
                                }

                                break;
                            }

                        #endregion

                        #region Freeze Gates

                        case InteractionType.FreezeYellowGate:
                        case InteractionType.FreezeRedGate:
                        case InteractionType.FreezeGreenGate:
                        case InteractionType.FreezeBlueGate:
                            {
                                if (cycleGameItems)
                                {
                                    int effectId = Convert.ToInt32(item.Team + 39);
                                    TeamManager t = user.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForFreeze();

                                    if (user.Team == Team.None)
                                    {
                                        if (t.CanEnterOnTeam(item.Team))
                                        {
                                            if (user.Team != Team.None)
                                                t.OnUserLeave(user);
                                            user.Team = item.Team;
                                            t.AddUser(user);

                                            if (user.GetClient().GetHabbo().Effects().CurrentEffect != effectId)
                                                user.GetClient().GetHabbo().Effects().ApplyEffect(effectId);
                                        }
                                    }
                                    else if (user.Team != Team.None && user.Team != item.Team)
                                    {
                                        t.OnUserLeave(user);
                                        user.Team = Team.None;
                                        user.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(user);
                                        if (user.GetClient().GetHabbo().Effects().CurrentEffect == effectId)
                                            user.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        user.Team = Team.None;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);                                
                                }

                                break;
                            }

                        #endregion

                        #region Banzai Teles

                        case InteractionType.BanzaiTele:
                            {
                                if (user.Statusses.ContainsKey("mv"))
                                    _room.GetGameItemHandler().OnTeleportRoomUserEnter(user, item);
                                break;
                            }

                        #endregion

                        #region Football Gate

                        #endregion

                        #region Effects

                        case InteractionType.Effect:
                            {
                                if (user == null)
                                    return;

                                if (!user.IsBot)
                                {
                                    if (item == null || item.GetBaseItem() == null || user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().Effects() == null)
                                        return;

                                    if (item.GetBaseItem().EffectId == 0 && user.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                                        return;

                                    user.GetClient().GetHabbo().Effects().ApplyEffect(item.GetBaseItem().EffectId);
                                    item.ExtraData = "1";
                                    item.UpdateState(false, true);
                                    item.RequestUpdate(2, true);
                                }

                                break;
                            }

                        #endregion

                        #region Arrows

                        case InteractionType.Arrow:
                            {
                                if (user.GoalX == item.GetX && user.GoalY == item.GetY)
                                {
                                    if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                                        continue;
                                    Room Room;
                                    var This = user.GetClient().GetRolePlay();
                                    if (This.Dead || user.Stunned > 0 || user.IsAsleep || This.EscortID > 0 || This.BotEscort > 0 || This.Cuffed)
                                        return;
                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(user.GetClient().GetHabbo().CurrentRoomId, out Room room))
                                        return;

                                    if (!ItemTeleporterFinder.IsTeleLinked(item.Id, room))
                                        user.UnlockWalking();
                                    else
                                    {
                                        int linkedTele = ItemTeleporterFinder.GetLinkedTele(item.Id, room);
                                        int teleRoomId = ItemTeleporterFinder.GetTeleRoomId(linkedTele, room);

                                        if (teleRoomId == room.RoomId)
                                        {
                                            Item targetItem = room.GetRoomItemHandler().GetItem(linkedTele);
                                            if (targetItem == null)
                                            {
                                                if (user.GetClient() != null)
                                                    user.GetClient().SendWhisper("Hey, that arrow is poorly!");
                                                return;
                                            }

                                            room.GetGameMap().TeleportToItem(user, targetItem);
                                        }
                                        else if (teleRoomId != room.RoomId)
                                        {
                                            if (user != null && !user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null)
                                            {
                                                user.GetClient().GetHabbo().IsTeleporting = true;
                                                user.GetClient().GetHabbo().TeleportingRoomId = teleRoomId;
                                                user.GetClient().GetHabbo().TeleportId = linkedTele;
                                                if (item.BaseItem == 9999)
                                                {
                                                    if (item.Rotation == 4)
                                                        This.Rotate = 4;
                                                    else if (item.Rotation == 0)
                                                        This.Rotate = 10;
                                                    else if (item.Rotation == 6)
                                                        This.Rotate = 6;
                                                    else if (item.Rotation == 2)
                                                        This.Rotate = 2;
                                                }
                                                else
                                                {
                                                    if (item.Rotation == 4)
                                                        This.Rotate = 10;
                                                    else if (item.Rotation == 0)
                                                        This.Rotate = 4;
                                                    else if (item.Rotation == 6)
                                                        This.Rotate = 2;
                                                    else if (item.Rotation == 2)
                                                        This.Rotate = 6;

                                                }
                                                user.GetClient().GetHabbo().PrepareRoom(teleRoomId, "");
                                            }
                                        }
                                        else if (_room.GetRoomItemHandler().GetItem(linkedTele) != null)
                                        {
                                            user.SetPos(item.GetX, item.GetY, item.GetZ);
                                            user.SetRot(item.Rotation, false);
                                        }
                                        else
                                            user.UnlockWalking();
                                    }
                                }

                                break;
                            }

                            #endregion
                    }
                }

                if (user.IsSitting && user.TeleportEnabled)
                {
                    user.Z -= 0.35;
                    user.UpdateNeeded = true;
                }

                if (cycleGameItems)
                {
                    if (_room.GotSoccer())
                        _room.GetSoccer().OnUserWalk(user);

                    if (_room.GotBanzai())
                        _room.GetBanzai().OnUserWalk(user);

                    if (_room.GotFreeze())
                        _room.GetFreeze().OnUserWalk(user);
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        private void UpdateUserEffect(RoomUser user, int x, int y)
        {
            if (user == null || user.IsBot || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            try
            {
                byte newCurrentUserItemEffect = _room.GetGameMap().EffectMap[x, y];
                if (newCurrentUserItemEffect > 0)
                {
                    if (user.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                        user.CurrentItemEffect = ItemEffectType.None;

                    ItemEffectType type = ByteToItemEffectEnum.Parse(newCurrentUserItemEffect);
                    if (type != user.CurrentItemEffect)
                    {
                        switch (type)
                        {
                            case ItemEffectType.IceSkates:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(user.GetClient().GetHabbo().Gender == "M" ? 38 : 39);
                                    user.CurrentItemEffect = ItemEffectType.IceSkates;
                                    break;
                                }

                            case ItemEffectType.NormalSkates:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(user.GetClient().GetHabbo().Gender == "M" ? 55 : 56);
                                    user.CurrentItemEffect = type;
                                    break;
                                }
                            case ItemEffectType.Swim:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(29);
                                    user.CurrentItemEffect = type;
                                    break;
                                }
                            case ItemEffectType.SwimLow:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(30);
                                    user.CurrentItemEffect = type;
                                    break;
                                }
                            case ItemEffectType.SwimHalloween:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(37);
                                    user.CurrentItemEffect = type;
                                    break;
                                }

                            case ItemEffectType.None:
                                {
                                    user.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                                    user.CurrentItemEffect = type;
                                    break;
                                }
                        }
                    }
                }
                else if (user.CurrentItemEffect != ItemEffectType.None && newCurrentUserItemEffect == 0)
                {
                    user.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                    user.CurrentItemEffect = ItemEffectType.None;
                }
            }
            catch
            {
            }
        }

        public int PetCount { get; private set; }

        public ICollection<RoomUser> GetUserList()
        {
            return _users.Values;
        }

        public void Dispose()
        {
            UpdatePets();
            UpdateBots();

            _room.UsersNow = 0;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `id` = '" + _room.Id + "' LIMIT 1");
            }

            _users.Clear();
            _pets.Clear();
            _bots.Clear();

            UserCount = 0;
            PetCount = 0;

            _users = null;
            _pets = null;
            _bots = null;
            _room = null;
        }
    }
}