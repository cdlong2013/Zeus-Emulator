using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MySql.Data.MySqlClient.Memcached;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Bcpg;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.Chat.Commands.Administrator;
using Plus.HabboHotel.Rooms.Chat.Commands.Events;
using Plus.HabboHotel.Rooms.Chat.Commands.Moderator;
using Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using Plus.HabboHotel.Rooms.Chat.Commands.User;
using Plus.HabboHotel.Rooms.Chat.Commands.User.Fun;
using Plus.HabboHotel.Rooms.PathFinding;
using Plus.RolePlay;

namespace Plus.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
      
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private readonly string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string prefix)
        {
            _prefix = prefix;
            _commands = new Dictionary<string, IChatCommand>();

            RegisterVip();
            RegisterUser();
            RegisterEvents();
            RegisterModerator();
            RegisterAdministrator();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="session">Session calling this method.</param>
        /// <param name="message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient session, string message)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().CurrentRoom == null)
                return false;

            if (!message.StartsWith(_prefix))
                return false;


            message = message.Substring(1);
            string[] split = message.Split(' ');


           

            switch (split[0].ToLower())
            {

                #region Banking Commands

                #region :balance <user>
                case "balance":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 3 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            var Target = User.GetClient().GetRolePlay();
                            if (Target == This)
                                return true;
                            if (Target.Dead || Target.Jailed > 0 || Target.JailedSec > 0 || Target.Cuffed || Target.roomUser.Stunned > 0 || This.subCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int dis = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                            if (dis >= 3)
                            {
                                session.SendWhisper("This user is too far away!");
                                return true;
                            }
                            if (Target.Storage.openaccount == 0)
                            {
                                session.SendWhisper("This user does not have an account!");
                                return true;
                            }
                            var money = string.Format("{0:n0}", Target.Storage.bankamount);
                            if (Target.Storage.bankamount == 1)
                                This.roomUser.OnChat(0, "@x You have a balance of " + money + " dollar", false);
                            else This.roomUser.OnChat(0, "@x You have a balance of " + money + " dollars", false);
                            This.subCooldown = 5;
                        }
                        return true;
                    }
                #endregion

                #region :deposit <amount>
                case "deposit":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 3 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            var Target = User.GetClient().GetRolePlay();
                            if (Target == This)
                                return true;
                            if (Target.Dead || Target.Jailed > 0 || Target.JailedSec > 0 || Target.Cuffed || Target.roomUser.Stunned > 0 || This.subCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int dis = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                            if (dis >= 3)
                            {
                                session.SendWhisper("This user is too far away!");
                                return true;
                            }
                            if (Target.Storage.openaccount == 0)
                            {
                                session.SendWhisper("This user does not have an account!");
                                return true;
                            }
                            int amount = Convert.ToInt32(Convert.ToInt32(split[2]));
                            if (Target.habbo.Credits < amount)
                            {
                                session.SendWhisper("This user does not have enough money!");
                                return true;
                            }
                            This.XPSet(2);
                            var money = string.Format("{0:n0}", amount);
                            if (amount == 1)
                                This.Say("deposits " + money + " dollar into " + Target.habbo.Username + "'s account", false);
                            else This.Say("deposits " + money + " dollars into " + Target.habbo.Username + "'s account", false);
                            Target.Storage.bankamount += amount;
                            Target.UpdateCredits(amount, false);
                            Target.RPCache(22);
                            This.subCooldown = 5;
                            This.JobManager.Task2++;
                            This.RPCache(28);
                        }
                        return true;
                    }
                #endregion

                #region :withdrawl <user> <amount>
                case "withdraw":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 3 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            var Target = User.GetClient().GetRolePlay();
                            if (Target == This)
                                return true;
                            if (Target.Dead || Target.Jailed > 0 || Target.JailedSec > 0 || Target.Cuffed || Target.roomUser.Stunned > 0 || This.subCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int dis = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                            if (dis >= 3)
                            {
                                session.SendWhisper("This user is too far away!");
                                return true;
                            }
                            if (Target.Storage.openaccount == 0)
                            {
                                session.SendWhisper("This user does not have an account!");
                                return true;
                            }
                            int amount = Convert.ToInt32(Convert.ToInt32(split[2]));
                            if (Target.Storage.bankamount < amount)
                            {
                                session.SendWhisper("This user does not have enough money in their account!");
                                return true;
                            }
                            This.XPSet(2);
                            var money = string.Format("{0:n0}", amount);
                            if (amount == 1)
                                This.Say("withdraws " + money + " dollar from " + Target.habbo.Username + "'s account and hands it to them", false);
                            else This.Say("withdraws " + money + " dollars from " + Target.habbo.Username + "'s account and hands it to them", false);
                            Target.Storage.bankamount -= amount;
                            Target.UpdateCredits(amount, true);
                            This.subCooldown = 5;
                            Target.RPCache(22);
                            This.JobManager.Task3++;
                            This.RPCache(27);
                        }
                        return true;
                    }
                #endregion

                #region :setaccount <user>
                case "setaccount":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 3 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            var Target = User.GetClient().GetRolePlay();
                            if (Target.Dead || Target.Jailed > 0 || Target.JailedSec > 0 || Target.Cuffed || Target.roomUser.Stunned > 0 || This.subCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int dis = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                            if (dis >= 3)
                            {
                                session.SendWhisper("This user is too far away, try getting closer!");
                                return true;
                            }
                            if (Target.Storage.openaccount > 0)
                            {
                                session.SendWhisper("This user's account is already set!");
                                return true;
                            }
                            This.xpdelay = 2;
                            This.xpsave = 2;
                            Target.Storage.openaccount = 1;
                            This.Say("opens a new account for " + Target.habbo.Username + "", false);
                            This.JobManager.Task1++;
                            This.RPCache(26);
                            Target.RPCache(21);
                            This.subCooldown = 5;
                            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                DB.RunQuery("UPDATE bank SET active = '" + Target.Storage.openaccount + "' WHERE id = '" + Target.habbo.Id + "'");
                            Target.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"account\","
                 + "\"name1\":\"" + Target.habbo.Username + "\", \"color1\":\"" + Target.Color + "\"}");
                        }
                        return true;
                    }
                #endregion

                #endregion

                #region Police Commands

                #region :stun <user>
                case "stun":
                    {
                        var This = session.GetRolePlay();
                        This.Stun(Convert.ToString(split[1]));
                        return true;
                    }
                #endregion

                #region cuff <user>
                case "cuff":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 1 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            if (This.lockTarget > 0)
                            {
                                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                                if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                    User = Target.GetHabbo().roomUser;
                            }
                            if (User == null)
                            {
                                var bot = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                                User = bot;
                                if (User == null)
                                {
                                    foreach (RoomUser pet in session.GetHabbo().CurrentRoom.GetRoomUserManager()._pets.Values.ToList())
                                        User = pet;
                                    if (User == null || User.BotData.Name != Convert.ToString(split[1]))
                                    {
                                        session.SendWhisper("User not found!");
                                        return true;
                                    }
                                    if (User.IsPet)
                                    {
                                        session.SendWhisper("You cannot cuff an animal.");
                                        return true;
                                    }
                                }
                            }
                            if (User == This.roomUser)
                            {
                                session.SendWhisper("You cannot cuff yourself!");
                                return true;
                            }
                            if ((!User.IsBot && User.GetClient().GetRolePlay().Cuffed) || (User.IsBot && User.BotData.Cuffed))
                            {
                                session.SendWhisper("This user is already cuffed!");
                                return true;
                            }
                            if (User.Stunned == 0)
                            {
                                session.SendWhisper("You must stun this user before you can cuff them!");
                                return true;
                            }
                            if ((!User.IsBot && (User.GetClient().GetRolePlay().Health < 1 || User.GetClient().GetRolePlay().Dead)) || (User.IsBot && User.BotData.Health < 1))
                            {
                                session.SendWhisper("That action cannot be performed on someone who is unconscious!");
                                return true;
                            }
                            int dis = Math.Abs(User.X - This.roomUser.X) + Math.Abs(User.Y - This.roomUser.Y);
                            if (dis <= 1 || This.CheckDiag(User.X, User.Y, This.roomUser.X, This.roomUser.Y))
                            {
                                if (User.IsBot)
                                    User.BotData.Cuffed = true;
                                else
                                {
                                    User.GetClient().GetRolePlay().Cuff();
                                    User.GetClient().GetRolePlay().SetRot(This.roomUser.RotBody, false);
                                    if (User.GetClient().GetRolePlay().Escorting > 0)
                                        User.GetClient().GetRolePlay().EndEscort();
                                }
                                User.ApplyEffect(590);
                                string name = "";
                                if (User.IsBot)
                                    name = User.BotData.Name;
                                else name = User.GetClient().GetHabbo().Username;
                                This.Say("removes cuffs from belt and places them on " + name + "'s wrist", true, 4);
                            }
                        }

                        return true;
                    }
                #endregion

                #region :release <user>
                case "release":
                    {
                        var This = session.GetRolePlay();
                        var User = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(split[1]);
                        var Target = User.GetRolePlay();
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        if (((This.habbo.Rank > 4) || (This.JobManager.Job == 1 && This.JobManager.Working)) && (Target.Jailed > 0 || Target.JailedSec > 0))
                        {
                            This.Say("releases " + Target.habbo.Username + " from jail", false);
                            Target.Timer(0, 1, "jail");
                        }
                    }
                    return true;
                #endregion

                #region :escort <user>
                case "escort":
                    {
                        var This = session.GetRolePlay();
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (This.lockTarget > 0)
                        {
                            var target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                            if (target != null && target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                User = target.GetHabbo().roomUser;
                        }
                        if (User == null)
                        {
                            var bot = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                            if (bot != null)
                                session.SendWhisper("Bots cannot be escorted.");
                            else session.SendWhisper("User not found!");
                            return true;
                        }
                        if (User == This.roomUser)
                            return true;
                        if (This.Cooldown > 0)
                        {
                            This.Responds();
                            return true;
                        }
                        var Target = User.GetClient().GetRolePlay();
                        if (Target.EscortID == This.habbo.Id)
                        {
                            if (This.JobManager.Working && This.JobManager.Job == 7)
                                return true;
                            This.Say("stops escorting " + User.GetClient().GetHabbo().Username + "", true, 4);
                            User.ClearMovement(true);
                            This.EndEscort();
                            return true;
                        }
                        int dis = Math.Abs(User.X - This.roomUser.X) + Math.Abs(User.Y - This.roomUser.Y);
                        if (dis <= 1 || This.CheckDiag(User.X, User.Y, This.roomUser.X, This.roomUser.Y))
                        {
                            if (User.GetClient().GetRolePlay().EscortID > 0)
                            {
                                session.SendWhisper("This user is no longer being escorted.");
                                return true;
                            }
                            if (Target.Health < 1 && This.JobManager.Job == 7)
                            {
                                This.roomUser.ApplyEffect(20);
                                User.ApplyEffect(20);
                                Target.Health = 1;
                                User.Stunned = 0;
                            }
                            else if (!Target.Cuffed && !This.onduty)
                            {
                                session.SendWhisper("This user is not logged in.");
                                return true;
                            }
                            Target.EscortID = This.habbo.Id;
                            This.Escorting = User.HabboId;
                            if (This.JobManager.Job == 7 && This.JobManager.Working)
                                This.Say("begans transfering " + User.GetClient().GetHabbo().Username + " to the nearest hospital", false);
                            else
                            {
                                This.Say("begins to escort " + User.GetClient().GetHabbo().Username + "", false);
                                This.roomUser.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, User.X, User.Y), false);
                            }
                            This.subCooldown = 5;
                            User.GetClient().GetRolePlay().RoomForward(This.Room.Id);
                            if (!Target.Cuffed)
                                Target.Cuff();
                        }
                        else session.SendWhisper("Você está muito longe para realizar esta ação!");
                        return true;
                    }
                case "stopescort":
                    {
                        var This = session.GetRolePlay();
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (This.lockTarget > 0)
                        {
                            var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                            if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                User = Target.GetHabbo().roomUser;
                        }
                        if (User == This.roomUser || User == null)
                            return true;
                        if (User.GetClient().GetRolePlay().EscortID == This.habbo.Id)
                        {
                            if (This.JobManager.Working && This.JobManager.Job == 7)
                                return true;
                            This.Say("stops escorting " + User.GetClient().GetHabbo().Username + "", true, 4);
                            User.ClearMovement(true);
                            This.EndEscort();
                            return true;
                        }
                        return true;
                    }
                #endregion

                #region :arrest <user> <time>
                case "arrest":
                    {
                        var This = session.GetRolePlay();
                        if ((This.JobManager.Job == 1 && This.JobManager.Working) || This.onduty)
                        {
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            if (This.lockTarget > 0)
                            {
                                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                                if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                    User = Target.GetHabbo().roomUser;
                            }
                            if (User == null)
                            {
                                var bot = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                                if (bot != null)
                                    User = bot;
                                else
                                {
                                    if (User.BotData.Name != Convert.ToString(split[1]))
                                    {
                                        session.SendWhisper("User not found!");
                                        return true;
                                    }
                                }
                            }
                            if (User == This.roomUser)
                            {
                                session.SendWhisper("You cannot perform this action on yourself!");
                                return true;
                            }
                            if ((!User.IsBot && User.GetClient().GetRolePlay().Health < 1) || (User.IsBot && User.BotData.Health < 1))
                            {
                                session.SendWhisper("This action cannot be performed on someone who is unconscious!");
                                return true;
                            }
                            if ((!User.IsBot && !User.GetClient().GetRolePlay().Cuffed) || (User.IsBot && !User.BotData.Cuffed))
                            {
                                session.SendWhisper("This user is not logged in.");
                                return true;
                            }
                            if (!User.IsBot && (This.Room.RoomId == 10 || User.GetClient().GetRolePlay().AutoLogout > 0))
                            { }
                            else
                            {
                                session.SendWhisper("You must be at the police station to perform this action.");
                                return true;
                            }
                            int dis = Math.Abs(User.X - This.roomUser.X) + Math.Abs(User.Y - This.roomUser.Y);
                            if (dis <= 1 || This.CheckDiag(User.X, User.Y, This.roomUser.X, This.roomUser.Y))
                            {
                                This.roomUser.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, User.X, User.Y), false);
                                int time = Convert.ToInt32(split[2]);
                                if (time < 1 || time > 60)
                                {
                                    session.SendWhisper("That is an invalid time.");
                                    return true;
                                }
                                This.xpsave += 25;
                                This.xpdelay = 2;
                                string name = "";
                                if (User.IsBot)
                                    name = User.BotData.Name;
                                else name = User.GetClient().GetHabbo().Username;
                                if (time == 1)
                                    This.Say("arrests " + name + " for " + time + " minutes", false);
                                else This.Say("arrests " + name + " for " + time + " minutes", false);
                                This.JobManager.Task1++;
                                This.RPCache(26);
                                if (!User.IsBot)
                                {
                                    if (User.GetClient().GetRolePlay().EscortID > 0)
                                    {
                                        This.Escorting = 0;
                                        User.GetClient().GetRolePlay().EscortID = 0;
                                    }
                                    User.Jail(time, This.habbo.Username);
                                }
                                else User.BotData.Arrested = true;

                            }
                        }
                        return true;
                    }
                #endregion

                #region :charge <user> <charge>
                case "charge":
                    {
                        var This =session.GetRolePlay();
                        if (This.JobManager.Job == 1 && This.JobManager.Working)
                        {
                            if (This.dubCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            var User = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(split[1]);
                            if (This.lockTarget > 0)
                            {
                                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                                if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                    User = Target;
                            }
                            if (User == null)
                            {
                                session.SendWhisper("User not found!");
                                return true;
                            }
                            if (User == This.GetClient())
                            {
                                session.SendWhisper("This action can not be performed on yourself!");
                                return true;
                            }
                            string charge = CommandManager.MergeParams(split, 2);
                            if (charge == "null" || string.IsNullOrEmpty(charge))
                            {
                                session.SendWhisper("Invalid!");
                                return true;
                            }
                            if (charge.Length > 15)
                            {
                                session.SendWhisper("Too many characters!");
                                return true;
                            }
                            if (charge.Length <= 2)
                            {
                                session.SendWhisper("not enough characters!");
                                return true;
                            }
                            var WantedList = This.habbo.GetClientManager().GetWL(User.GetHabbo().Username);
                            if (WantedList != null)
                            {
                                if (!This.habbo.GetClientManager().AddReason(charge, WantedList))
                                {
                                    session.SendWhisper("You can only add a maximum of 3 charges!");
                                    return true;
                                }
                            }
                            else This.habbo.GetClientManager().AddWL(User.GetHabbo().Username, User.GetHabbo().Look, charge, User.GetRolePlay().Color);
                            This.Say("charges " + User.GetHabbo().Username + " with " + charge + "", false);
                            User.SendWhisper("You have been charged with " + charge + "!");
                            This.dubCooldown = 5;
                            This.JobManager.Task3++;
                            This.RPCache(28);
                            This.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"charge\","
       + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetHabbo().Username + "\","
   + "\"color1\":\"" + This.Color + "\", \"color2\":\"" + User.GetRolePlay().Color + "\", \"charge\":\"" + charge + "\"}");
                        }
                        else if (This.JobManager.Job == 1)
                            session.SendWhisper("You need to be working in order to perform this action!");
                        return true;
                    }
                #endregion


                #endregion

                #region Corporation Commands

                #region :startwork

                case "startwork":
                    {
                        session.GetRolePlay().Startwork();
                        return true;
                    }

                #endregion

                #region :stopwork
                case "stopwork":
                    {
                        session.GetRolePlay().Stopwork(false);
                        return true;
                    }

                #endregion

                #region :promote <user>

                case "promote":
                    {
                        var This = session.GetRolePlay();
                        if (This.dubCooldown > 0)
                        {
                            This.Responds();
                            return true;
                        }
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        var Target = User.GetClient().GetRolePlay();
                        if (This.roomUser == User)
                            return true;
                        if ((This.JobManager.Working && This.JobManager.Boss > 0 && This.JobManager.Job == Target.JobManager.Job) || This.onduty)
                        {
                            if (Target.JobManager.JobRank + 1 >= This.JobManager.JobRank)
                            {
                                session.SendWhisper("You cannot promote this user to this rank.");
                                return true;
                            }
                            if (Target.JobManager.Working)
                                Target.Stopwork(false);
                            Target.JobManager.JobRank += 1;
                            Target.JobManager.SetJob();
                            This.Say("promotes " + User.GetUsername() + " to " + Target.JobManager.JobMotto + "", true, 4);
                            This.dubCooldown = 5;
                            This.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"promote\","
                          + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetUsername() + "\","
                          + "\"color1\":\"" + This.Color + "\", \"color2\":\"" + Target.Color + "\", \"job\":\"" + Target.JobManager.JobMotto + "\"}");
                        }
                        return true;
                    }

                #endregion

                #region :demote <user>

                case "demote":
                    {
                        var This = session.GetRolePlay();
                        if (This.dubCooldown > 0)
                        {
                            This.Responds();
                            return true;
                        }
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        var Target = User.GetClient().GetRolePlay();
                        if (This.roomUser == User)
                            return true;
                        if ((This.JobManager.Working && This.JobManager.Boss > 0 && This.JobManager.Job == Target.JobManager.Job) || This.onduty)
                        {
                            if (Target.JobManager.JobRank - 1 <= 0)
                            {
                                session.SendWhisper("You cannot demote this user any lower!");
                                return true;
                            }
                            if (Target.JobManager.Working)
                                Target.Stopwork(false);
                            Target.JobManager.JobRank--;
                            Target.JobManager.SetJob();
                            This.Say("demotes " + User.GetUsername() + " to " + Target.JobManager.JobMotto + "", false);
                            This.dubCooldown = 5;
                            This.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"demoted\","
                      + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetUsername() + "\","
                      + "\"color1\":\"" + This.Color + "\", \"color2\":\"" + Target.Color + "\", \"job\":\"" + Target.JobManager.JobMotto + "\"}");
                        }
                        return true;
                    }

                #endregion

                #region :hire <user> <job>

                case "hire":
                    {
                        var This = session.GetRolePlay();
                        if (This.dubCooldown > 0)
                        {
                            This.Responds();
                            return true;
                        }
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        var Target = User.GetClient().GetRolePlay();
                        int job = 0;
                        int rank = 0;
                        if (This.habbo.Rank >= 6 && !This.JobManager.Working)
                        {
                            job = Convert.ToInt32(split[2]);
                            rank = Convert.ToInt32(split[3]);
                        }
                        else if (This.JobManager.Working && This.JobManager.Boss > 0)
                        {
                            job = This.JobManager.Job;
                            rank = 1;
                        }
                        else return true;
                        if (Target.JobManager.Job > 0)
                        {
                            session.SendWhisper("This user already has a job.");
                            return true;
                        }
                        Target.JobManager.Job = job;
                        Target.JobManager.JobRank = rank;
                        if (Target.JobManager.SetJob(true, true) == "null")
                            session.SendWhisper("Este trabalho não existe!");
                        else
                        {
                            This.Say("hires " + Target.habbo.Username + " as" + Target.JobManager.JobMotto + "", false);
                            This.dubCooldown = 5;
                            This.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"hired\","
                                   + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetUsername() + "\","
                                   + "\"color1\":\"" + This.Color + "\", \"color2\":\"" + Target.Color + "\", \"job\":\"" + Target.JobManager.JobMotto + "\"}");
                        }
                        return true;
                    }

                #endregion

                #region :fire <user>

                case "fire":
                    {
                        var This = session.GetRolePlay();
                        if (This.dubCooldown > 0)
                        {
                            This.Responds();
                            return true;
                        }
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User == null)
                        {
                            session.SendWhisper("User not found.");
                            return true;
                        }
                        var Target = User.GetClient().GetRolePlay();
                        if (This.roomUser == User)
                            return true;
                        if ((This.JobManager.Working && This.JobManager.Boss > 0 && This.JobManager.Job == Target.JobManager.Job) || This.onduty)
                        {
                            if (Target.JobManager.JobRank - 1 <= 0)
                            {
                                session.SendWhisper("This user cannot be fired, they do not have a job.");
                                return true;
                            }
                            if (Target.JobManager.Working)
                                Target.Stopwork(false);
                            Target.JobManager.JobRank--;
                            Target.JobManager.SetJob();
                            This.Say("fires " + User.GetUsername() + " from their job as " + Target.JobManager.JobMotto + "", true, 4);
                            This.dubCooldown = 5;
                            This.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"demoted\","
                      + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetUsername() + "\","
                      + "\"color1\":\"" + This.Color + "\", \"color2\":\"" + Target.Color + "\", \"job\":\"" + Target.JobManager.JobMotto + "\"}");
                        }
                        return true;
                    }

                #endregion

                #region :heal <user>
                case "heal":
                    {
                        var This = session.GetRolePlay();
                        var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(split[1]);
                        var User = Target.GetHabbo().roomUser;
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        if (This.Cooldown > 0)
                            This.Responds();
                        else
                        {
                            if (This.habbo.Rank > 4 && This.roomUser.CarryItemId != 1014 && This.roomUser.CarryItemId != 1013)
                            {
                                if (User.GetClient().GetRolePlay().Health >= User.GetClient().GetRolePlay().MaxHealth + User.GetClient().GetRolePlay().bonushp)
                                {
                                    This.GetClient().SendWhisper("This users health is already full!");
                                    return true;
                                }
                                User.GetClient().GetRolePlay().Heal();
                                This.Say("uses their god-like powers to heal " + User.GetClient().GetHabbo().Username + "", false);
                                This.Cooldown = 5;
                                User.GetClient().GetRolePlay().Refresh();
                            }
                            else if (This.roomUser.CarryItemId == 1014 || This.roomUser.CarryItemId == 1013)
                            {
                                if (This.Room.Id != Target.GetHabbo().CurrentRoomId)
                                    return true;
                                int Distance = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                                if (Distance <= 1)
                                {
                                    if (This.roomUser != User)
                                        This.roomUser.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, User.X, User.Y), false);
                                    if (User.GetClient().GetRolePlay().Health >= User.GetClient().GetRolePlay().MaxHealth + User.GetClient().GetRolePlay().bonushp)
                                    {
                                        This.GetClient().SendWhisper("This user is already at full health!");
                                        return true;
                                    }
                                    if (This.roomUser.CarryItemId == 1013)
                                    {
                                        This.Say("applies bandages to " + User.GetUsername() + "", false);
                                        This.roomUser.CarryItem(0);
                                        if (User.GetClient().GetRolePlay().Health + 25 > User.GetClient().GetRolePlay().MaxHealth)
                                            User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().MaxHealth);
                                        else User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().Health + 25);
                                        if (User.CurrentEffect == 11)
                                            User.ApplyEffect(0);
                                        var slot = This.Inventory.Currslot1;
                                        This.WebHandler.Handle("equip", "false", "e1");
                                        This.Inventory.Additem(slot, true, false);
                                    }
                                    else if (This.roomUser.CarryItemId == 1014)
                                    {
                                        This.Say("injects " + User.GetUsername() + " with a syringe, healing them", false);
                                        This.JobManager.Task1++;
                                        This.RPCache(26);
                                        This.item = 0;
                                        This.itemtimer = 0;
                                        User.GetClient().GetRolePlay().JustHealed = 10;
                                        User.ApplyEffect(583);
                                        This.roomUser.CarryItem(0);
                                    }
                                    if (This.roomUser != User)
                                        This.XPSet(5);

                                }
                            }
                            else if (This.roomUser.CarryItemId != 1014 && This.JobManager.Job == 2 && This.JobManager.Working)
                                session.SendWhisper("You need a syringe from the cabinet to perform this action!");
                        }
                    }
                    return true;
                #endregion

                #endregion

                #region Basic RP Commands

                #region :push <user>

                case "push":
                    {
                        var This = session.GetRolePlay();
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (This.lockTarget > 0)
                        {
                            var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                            if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                User = Target.GetHabbo().roomUser;
                        }
                        if (User == null)
                        {
                            var bot = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                            User = bot;
                            if (User == null)
                            {
                                foreach (RoomUser pet in session.GetHabbo().CurrentRoom.GetRoomUserManager()._pets.Values.ToList())
                                    User = pet;
                                if (User == null && User.BotData.Name != Convert.ToString(split[1]))
                                {
                                    session.SendWhisper("That user could not be found.");
                                    return true;
                                }
                            }
                        }
                        if (This.subCooldown > 0 || This.roomUser.Stunned > 0 || This.Cuffed || This.Dead
                           || User.IsAsleep || User.TaxiDest > 0 || (User.HabboId > 0 && User.GetClient().GetRolePlay().Cuffed) ||
                           (User.HabboId == 0 && User.BotData.Cuffed) || (User.HabboId > 0 && User.GetClient().GetRolePlay().Dead) || User.Stunned > 0)
                            This.Responds();
                        else

                            if (!((Math.Abs(User.X - This.roomUser.X) >= 2) || (Math.Abs(User.Y - This.roomUser.Y) >= 2)))
                        {
                            if (This.roomUser.RotBody == 4)
                            {
                                User.MoveTo(User.X, User.Y + 1);
                            }

                            if (This.roomUser.RotBody == 0)
                            {
                                User.MoveTo(User.X, User.Y - 1);
                            }

                            if (This.roomUser.RotBody == 6)
                            {
                                User.MoveTo(User.X - 1, User.Y);
                            }

                            if (This.roomUser.RotBody == 2)
                            {
                                User.MoveTo(User.X + 1, User.Y);
                            }

                            if (This.roomUser.RotBody == 3)
                            {
                                User.MoveTo(User.X + 1, User.Y);
                                User.MoveTo(User.X, User.Y + 1);
                            }

                            if (This.roomUser.RotBody == 1)
                            {
                                User.MoveTo(User.X + 1, User.Y);
                                User.MoveTo(User.X, User.Y - 1);
                            }

                            if (This.roomUser.RotBody == 7)
                            {
                                User.MoveTo(User.X - 1, User.Y);
                                User.MoveTo(User.X, User.Y - 1);
                            }

                            if (This.roomUser.RotBody == 5)
                            {
                                User.MoveTo(User.X - 1, User.Y);
                                User.MoveTo(User.X, User.Y + 1);
                            }
                            string name = "";
                            if (User.HabboId > 0)
                                name = User.GetClient().GetHabbo().Username;
                            else name = User.BotData.Name;
                            This.Say("pushes " + name + "", true, 4);
                            //session.SendPacket(new ChatComposer(, " pushes " + name + "", 0, 4));

                        }
                    }
                    return true;

                #endregion

                #region :pull <user>

                case "pull":
                    {
                        var This = session.GetRolePlay();
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (This.lockTarget > 0)
                        {
                            var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                            if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                User = Target.GetHabbo().roomUser;
                        }
                        if (User == null)
                        {
                            var bot = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                            User = bot;
                            if (User == null)
                            {
                                foreach (RoomUser pet in session.GetHabbo().CurrentRoom.GetRoomUserManager()._pets.Values.ToList())
                                    User = pet;
                                if (User == null && User.BotData.Name != Convert.ToString(split[1]))
                                {
                                    session.SendWhisper("Target not found!");
                                    return true;
                                }
                            }
                        }
                        if (This.subCooldown > 0 || This.roomUser.Stunned > 0 || This.Cuffed || This.Dead
                            || User.IsAsleep || User.TaxiDest > 0 || (User.HabboId > 0 && User.GetClient().GetRolePlay().Cuffed) ||
                            (User.HabboId == 0 && User.BotData.Cuffed) || User.Stunned > 0)
                            This.Responds();
                        else
                        {
                            string PushDirection = "down";
                            if (User.GetRoom().Id == This.Room.Id && (Math.Abs(This.roomUser.X - User.X) < 3 && Math.Abs(This.roomUser.Y - User.Y) < 3))
                            {
                                string name = "";
                                if (User.HabboId > 0)
                                    name = User.GetClient().GetHabbo().Username;
                                else name = User.BotData.Name;
                                This.Say("pulls " + name + " to them", true, 4);

                                if (This.roomUser.RotBody == 0)
                                    PushDirection = "up";
                                if (This.roomUser.RotBody == 2)
                                    PushDirection = "right";
                                if (This.roomUser.RotBody == 4)
                                    PushDirection = "down";
                                if (This.roomUser.RotBody == 6)
                                    PushDirection = "left";

                                if (PushDirection == "up")
                                    User.MoveTo(This.roomUser.X, This.roomUser.Y - 1);

                                if (PushDirection == "right")
                                    User.MoveTo(This.roomUser.X + 1, This.roomUser.Y);

                                if (PushDirection == "down")
                                    User.MoveTo(This.roomUser.X, This.roomUser.Y + 1);

                                if (PushDirection == "left")
                                    User.MoveTo(This.roomUser.X - 1, This.roomUser.Y);
                                This.subCooldown = 5;
                            }
                        }
                    }
                    return true;

                #endregion

                #region :taxi <roomid>
                case "taxi":
                    {
                        int roomid = Convert.ToInt32(split[1]);
                        if (roomid > 0)
                            session.GetRolePlay().Taxi(roomid);
                        else
                            session.SendWhisper("Please provide a valid RoomID!");
                        return true;
                    }

                #endregion

                #region :911 <message>
                case "911":
                    {
                        string callmessage = CommandManager.MergeParams(split, 1);
                        if (String.IsNullOrEmpty(callmessage))
                        {
                            session.SendWhisper("You need to include a message in your call.");
                            return true;
                        }
                        if (message.Length <= 3)
                        {
                            session.SendWhisper("You need to be more detailed on your call!");
                            return true;
                        }
                        session.GetRolePlay().CallPolice(callmessage);
                        return true;
                    }
                #endregion

                #region :hit <user>
                case "hit":

                    {
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User == null)
                        {
                            User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetBotByName(Convert.ToString(split[1]));
                            if (User == null)
                            {
                                foreach (RoomUser pet in session.GetHabbo().CurrentRoom.GetRoomUserManager()._pets.Values.ToList())
                                    User = pet;
                                if (User == null || User.BotData.Name != Convert.ToString(split[1]) || String.IsNullOrEmpty(Convert.ToString(split[1])))
                                {
                                    session.SendWhisper("User not found!");
                                    return true;
                                }
                            }
                        }
                        if (User.IsPet)
                            session.GetRolePlay().HitBot(User.BotData.Id, true);
                        else if (User.IsBot)
                            session.GetRolePlay().HitBot(User.BotData.Id, false);
                        else session.GetRolePlay().Hit(User, false);
                        return true;
                    }
                #endregion

                #region :giveitem <user>

                case "giveitem":
                    {
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (User != session.GetRolePlay().roomUser)
                            session.GetRolePlay().GiveItem(User);
                        else if (session.GetRolePlay().onduty)
                            session.GetRolePlay().GiveItem(User);
                    }
                    return true;

                #endregion

                #region :pay
                case "pay":
                    {
                        var This = session.GetRolePlay();
                        var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        if (This.roomUser == User)
                            return true;
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        int dis = Math.Abs(This.roomUser.X - User.X) + Math.Abs(This.roomUser.Y - User.Y);
                        if (dis < 2 || This.CheckDiag(User.X, User.Y, This.roomUser.X, This.roomUser.Y))
                        {
                            if (This.Level < 2)
                            {
                                session.SendWhisper("You need to be level 2 before performing this action!");
                                return true;
                            }
                            if (This.dubCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int amount = Convert.ToInt32(split[2]);
                            if (session.GetHabbo().Credits < 1 || amount > session.GetHabbo().Credits || amount < 1)
                            {
                                session.SendWhisper("You have insuffiecent funds to perform this action!");
                                return true;
                            }
                            var money = string.Format("{0:n0}", amount);
                            if (amount == 1)
                                This.Say("Deu " + money + " reais para " + User.GetUsername() + "", false);
                            else This.Say("Pagou " + money + " reais para " + User.GetUsername() + "", false);
                            This.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, User.X, User.Y), false);
                            This.UpdateCredits(amount, false);
                            User.GetClient().GetRolePlay().UpdateCredits(amount, true);
                            This.dubCooldown = 10;
                            return true;
                        }
                        else
                        {
                            session.SendWhisper("You are too far away to perform this action!");
                            return true;
                        }
                    }
                #endregion

                #endregion

                #region Staff Commands 

                #region :summonbot <id>
                case "summonbot":
                case "sb":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank > 5)
                        {
                            if (This.dubCooldown > 0)
                            {
                                This.Responds();
                                return true;
                            }
                            int id = Convert.ToInt32(split[1]);
                            if (id > 3 || id < 0)
                            {
                                session.SendWhisper("You can only pick an id from 0 to 3!");
                                return true;
                            }
                            if (id == 1)
                            {
                                This.Room.PlacePolice(This.Room.Id, 8);
                                This.Say("requests police assistance", true, 23);
                            }
                            else if (id == 2)
                            {
                                This.Room.PlaceMedic(This.Room.Id, This.roomUser.X, This.roomUser.Y);
                                This.Say("requests medical assistance", true, 23);
                            }
                            else if (id == 3)
                            {
                                This.Room.PlaceServant(This.Room.Id, This.roomUser.X, This.roomUser.Y);
                                This.Say("requests a bartender", true, 23);
                            }
                            This.dubCooldown = 5;
                            
                            // else if
                        }

                    }
                    return true;
                #endregion

                #region :kickbots

                case "kickbots":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank > 5)
                        {
                            foreach (RoomUser Bot in This.Room.GetRoomUserManager()._bots.Values.ToList())
                                Bot.bypass = true;
                            session.SendWhisper("bots successfully kicked!");
                            This.Say("summons a bolt of lightning, killing all bots in the area", false, 23);


                        }
                        return true;
                    }

                #endregion

                #region :senduser <user> <roomid>
                case "senduser":
                case "send":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank >= 5)
                        {
                            int roomid = Convert.ToInt32(split[2]);
                            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(roomid);
                            if (Room != null)
                            {
                                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(split[1]);
                                var User = TargetClient.GetHabbo().roomUser;
                                User.ApplyEffect(0);
                                User.GetClient().GetRolePlay().RoomForward(roomid, 2);
                                This.Say("sends " + User.GetUsername() + " to " + Room.Name + " [" + roomid + "]", true, 23);
                            }
                            else
                                session.SendWhisper("This room does not exist!");
                        }

                        return true;
                    }
                #endregion

                #region :at <roomid>
                case "at":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank >= 5)
                        {
                            int roomid = Convert.ToInt32(split[1]);
                            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(roomid);
                            if (Room != null)
                            {
                                This.roomUser.ApplyEffect(0);
                                This.RoomForward(roomid);
                                This.Say("teleports to another location, disappearing in thin air", true, 23);
                            }
                            else
                                session.SendWhisper("This room does not exist!");
                        }
                        else return false;

                        return true;
                    }
                #endregion

                #region :effect <effectid>
                case "effect":
                case "enable":
                    {
                        var This = session.GetHabbo();
                        if (This.Rank > 4)
                        {
                            int id = Convert.ToInt32(split[1]);
                            This.roomUser.ApplyEffect(id);
                        }
                    }
                    return true;
                #endregion

                #region :roomheal 
                case "roomheal":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank >= 5)
                        {
                            foreach (RoomUser User in This.Room.GetRoomUserManager().GetRoomUsers())
                                User.GetClient().GetRolePlay().Heal();
                            This.Say("uses their god-like powers to heal the room", true, 23);
                        }
                        return true;
                    }

                #endregion

                #region :onduty

                case "onduty":
                case "offduty":
                    {
                        var This = session.GetRolePlay();
                        if (This.habbo.Rank >= 4)
                        {
                            if (This.stungun == "null")
                            {
                                This.SendWeb("{\"name\":\"selectstun\"}");
                                session.SendWhisper("You must select a stun-gun before performing this action!");
                                return true;
                            }
                            if (This.onduty)
                            {
                                This.onduty = false;
                                if (This.roomUser.CurrentEffect == 102)
                                    This.roomUser.ApplyEffect(0);
                                This.GetClient().SendWhisper("You are now off duty!");
                                //This.SendWeb("{\"name\":\"copbadge\", \"copbadge\":\"false\"}");
                                if (This.Inventory.Equip2.Contains("kevlar"))
                                    This.WebHandler.Handle("equip", "", "e2");
                                if (This.Inventory.Equip1 != "null")
                                    This.WebHandler.Handle("equip", "", "e1");
                                This.Inventory.Additem(This.stungun, true);
                                This.habbo.GetClientManager().STAFFactive--;
                            }
                            else if (!This.onduty)
                            {
                                This.onduty = true;
                                This.roomUser.ApplyEffect(102);
                                This.GetClient().SendWhisper("You are now on duty!");
                                if (!This.Inventory.IsInventoryFull(This.stungun))
                                {
                                    if (This.Inventory.Equip1 != "null" || This.Inventory.Equip2 != "null")
                                    {
                                        if (This.Inventory.Equip2.Contains("kevlar"))
                                            This.WebHandler.Handle("equip", "", "e2");
                                        if (This.Inventory.Equip1 != "null")
                                            This.WebHandler.Handle("equip", "", "e1");
                                    }
                                    //This.SendWeb("{\"name\":\"copbadge\", \"copbadge\":\"true\"}");
                                    if (This.Trade.Trading)
                                        This.client.SendWhisper("You need to stop trading to recieve your stun-gun");
                                    else This.Inventory.Additem(This.stungun);
                                    This.habbo.GetClientManager().STAFFactive++;
                                }
                            }
                        }
                    }

                    return true;
                #endregion

                #region :discharge <user>

                case "discharge":
                    {
                        var This = session.GetRolePlay();
                        if (This.JobManager.Job == 7 && This.Escorting > 0 && This.JobManager.Working)
                        {
                            if (This.Room.Id != 2)
                            {
                                session.SendWhisper("You need to be at a hospital in order to perform this action!");
                                return true;
                            }
                            var User = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                            if (This.lockTarget > 0)
                            {
                                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(This.lockTarget);
                                if (Target != null && Target.GetHabbo().CurrentRoom.Id == This.Room.Id)
                                    User = Target.GetHabbo().roomUser;
                            }
                            User.ClearMovement(true);
                            User.GetClient().GetRolePlay().Cuffed = false;
                            This.EndEscort();
                            This.roomUser.ApplyEffect(0);
                            User.ApplyEffect(0);
                            User.GetClient().GetRolePlay().Faint();
                            User.GetClient().GetRolePlay().DeadSetup(true, true);
                            This.Say("discharges " + User.GetUsername() + "", false);
                            This.UpdateCredits(2, true);
                            session.SendWhisper("You have been tipped 2 dollars for your services");
                            This.xpsave += 5;
                            This.xpdelay = 2;
                            This.JobManager.Task1++;
                            This.RPCache(26);
                            User.GetClient().GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"discharge\","
                               + "\"name1\":\"" + This.habbo.Username + "\", \"name2\":\"" + User.GetUsername() + "\"}");
                        }
                        return true;
                    }

                #endregion

                #region :rm <message>
                case "rm":
                    {
                        var This = session.GetHabbo();
                        if (This.Rank > 5)
                        {
                            string msg = CommandManager.MergeParams(split, 1);
                            This.roomUser.OnChat(34, "" + msg + "", true);
                        }
                        return true;
                    }
                #endregion

                #region kill <user>
                case "kill":
                    {
                        var This = session.GetRolePlay();
                        var User = This.Room.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(split[1]));
                        var Target = User.GetClient().GetRolePlay();
                        if (User == null)
                        {
                            session.SendWhisper("User not found!");
                            return true;
                        }
                        if (Target.Dead || Target.Health < 1)
                        {
                            session.SendWhisper("This user is already dead!");
                            return true;
                        }
                        if (This.dubCooldown > 0)
                            This.Responds();
                        else
                            if (session.GetHabbo().Rank > 6)
                        {
                            This.Say("summons a bolt of lighting, instantly killing " + Target.habbo.Username + "", true,23);
                            User.ApplyEffect(12);
                            Target.boltkill = 3;
                            This.dubCooldown = 15;
                        }
                    }
                    return true;
                #endregion

                #endregion

                #region VIP Commands

                #endregion




                // 27 - Gray with cloud
                // 28 - Blue Parrot
                // 29 - Blue Underwater
                // 30 - Bot
                case "test":
                    {
                        var This = session.GetRolePlay();

                        This.SendWeb("{\"name\":\"911\"}");
                
                        This.Say("This is a voice bubble!", true, Convert.ToInt32(split[1]));
                        
                        return true;
                    }

         

            }




            if (message == _prefix + "commands")
            {
                StringBuilder list = new();
                list.Append("This is the list of commands you have available:\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(cmdList.Value.PermissionRequired))
                    {
                        if (!session.GetHabbo().GetPermissions().HasCommand(cmdList.Value.PermissionRequired))
                            continue;
                    }

                    list.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }

                session.SendPacket(new MotdNotificationComposer(list.ToString()));
                return true;
            }

         
           



            if (split.Length == 0)
                return false;

            if (_commands.TryGetValue(split[0].ToLower(), out IChatCommand cmd))
            {
                if (session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    LogCommand(session.GetHabbo().Id, message, session.GetHabbo().MachineId);

                if (!string.IsNullOrEmpty(cmd.PermissionRequired))
                {
                    if (!session.GetHabbo().GetPermissions().HasCommand(cmd.PermissionRequired))
                        return false;
                }

                session.GetHabbo().ChatCommand = cmd;
                session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, session.GetHabbo(), this);

                cmd.Execute(session, session.GetHabbo().CurrentRoom, split);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Registers the VIP set of commands.
        /// </summary>
        private void RegisterVip()
        {
            Register("spull", new SuperPullCommand());
        }

        /// <summary>
        /// Registers the Events set of commands.
        /// </summary>
        private void RegisterEvents()
        {
            Register("eha", new EventAlertCommand());
            Register("eventalert", new EventAlertCommand());
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            Register("about", new InfoCommand());
            Register("pickall", new PickAllCommand());
            Register("ejectall", new EjectAllCommand());
            Register("lay", new LayCommand());
            Register("sit", new SitCommand());
            Register("stand", new StandCommand());
            Register("mutepets", new MutePetsCommand());
            Register("mutebots", new MuteBotsCommand());

            Register("mimic", new MimicCommand());
            Register("dance", new DanceCommand());
            Register("push", new PushCommand());
            Register("pull", new PullCommand());
            Register("enable", new EnableCommand());
            Register("follow", new FollowCommand());
            Register("faceless", new FacelessCommand());
            Register("moonwalk", new MoonwalkCommand());

            Register("unload", new UnloadCommand());
            Register("regenmaps", new RegenMaps());
            Register("emptyitems", new EmptyItemsCommand());
            Register("setmax", new SetMaxCommand());
            Register("setspeed", new SetSpeedCommand());
            Register("disablediagonal", new DisableDiagonalCommand());
            Register("flagme", new FlagMeCommand());

            Register("stats", new StatsCommand());
            Register("kickpets", new KickPetsCommand());
            Register("kickbots", new KickBotsCommand());

            Register("room", new RoomCommand());
            Register("dnd", new DndCommand());
            Register("disablegifts", new DisableGiftsCommand());
            Register("convertcredits", new ConvertCreditsCommand());
            Register("disablewhispers", new DisableWhispersCommand());
            Register("disablemimic", new DisableMimicCommand());

            Register("pet", new PetCommand());
            Register("spush", new SuperPushCommand());
            Register("superpush", new SuperPushCommand());
        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
            Register("ban", new BanCommand());
            Register("mip", new MipCommand());
            Register("ipban", new IpBanCommand());

            Register("ui", new UserInfoCommand());
            Register("userinfo", new UserInfoCommand());
            Register("sa", new StaffAlertCommand());
            Register("roomunmute", new RoomUnmuteCommand());
            Register("roommute", new RoomMuteCommand());
            Register("roombadge", new RoomBadgeCommand());
            Register("roomalert", new RoomAlertCommand());
            Register("roomkick", new RoomKickCommand());
            Register("mute", new MuteCommand());
            Register("smute", new MuteCommand());
            Register("unmute", new UnmuteCommand());
            Register("massbadge", new MassBadgeCommand());
            Register("kick", new KickCommand());
            Register("skick", new KickCommand());
            Register("ha", new HotelAlertCommand());
            Register("hotelalert", new HotelAlertCommand());
            Register("hal", new HalCommand());
            Register("give", new GiveCommand());
            Register("givebadge", new GiveBadgeCommand());
            Register("dc", new DisconnectCommand());
            Register("kill", new DisconnectCommand());
            Register("Disconnect", new DisconnectCommand());
            Register("alert", new AlertCommand());
            Register("tradeban", new TradeBanCommand());

            Register("teleport", new TeleportCommand());
            Register("summon", new SummonCommand());
            Register("override", new OverrideCommand());
            Register("massenable", new MassEnableCommand());
            Register("massdance", new MassDanceCommand());
            Register("freeze", new FreezeCommand());
            Register("unfreeze", new UnFreezeCommand());
            Register("fastwalk", new FastwalkCommand());
            Register("superfastwalk", new SuperFastwalkCommand());
            Register("coords", new CoordsCommand());
            Register("alleyesonme", new AllEyesOnMeCommand());
            Register("allaroundme", new AllAroundMeCommand());
            Register("forcesit", new ForceSitCommand());

            Register("ignorewhispers", new IgnoreWhispersCommand());
            Register("forced_effects", new DisableForcedFxCommand());

            Register("makesay", new MakeSayCommand());
            Register("flaguser", new FlagUserCommand());
        }

        /// <summary>
        /// Registers the administrator set of commands.
        /// </summary>
        private void RegisterAdministrator()
        {
            Register("bubble", new BubbleCommand());
            Register("update", new UpdateCommand());
            Register("deletegroup", new DeleteGroupCommand());
            Register("carry", new CarryCommand());
            Register("goto", new GotoCommand());
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="commandText">Text to type for this command.</param>
        /// <param name="command">The command to execute.</param>
        public void Register(string commandText, IChatCommand command)
        {
            _commands.Add(commandText, command);
        }

        public static string MergeParams(string[] @params, int start)
        {
            var merged = new StringBuilder();
            for (int i = start; i < @params.Length; i++)
            {
                if (i > start)
                    merged.Append(" ");
                merged.Append(@params[i]);
            }

            return merged.ToString();
        }

        public void LogCommand(int userId, string data, string machineId)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", userId);
                dbClient.AddParameter("Data", data);
                dbClient.AddParameter("MachineId", machineId);
                dbClient.AddParameter("Timestamp", PlusEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public bool TryGetCommand(string command, out IChatCommand chatCommand)
        {
            return _commands.TryGetValue(command, out chatCommand);
        }
    }
}