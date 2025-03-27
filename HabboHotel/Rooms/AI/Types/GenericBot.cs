using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Rooms.AI.Responses;
using Plus.HabboHotel.Rooms.PathFinding;
using System.Data;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;


namespace Plus.HabboHotel.Rooms.AI.Types
{
    public class GenericBot : BotAI
    {
        private int VirtualId;
        private int ActionTimer = 0;
        private int SpeechTimer = 0;
        private static readonly Random Random = new Random();
        private int chasingid = 0;
        private bool WentLooking;
        private bool SayItAgain;
        private bool Busy;
        private bool OnEnter = true;
        private int KickTimer = 50;
        private int KickDelay = 25;
        private bool SaidThis;
        private int TakingtooLong;
        private int followX;
        private int followY;
        private int itemId;
        private bool HasFoundExit;
        private int BackUp;
        private bool HasFoundTile;
        private int subTimer = 25;
        private int Timer;
        private int Master;
        private bool Gone;
        private int Delay;
        private bool Lowhealth;
        private int cooldown;
        private int callpolice;
        private int affect;
        private int saycooldown;
        private string Team;
        private int Strength;
        private bool check;
        private bool bypass;
        private int replyCD;
        private int SpawnRoom;

        public GenericBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {

        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {

        }

        public override void OnUserEnterRoom(RoomUser User)
        {

        }

        public override void OnUserLeaveRoom(GameClient Client)
        {

        }

        public override void OnUserSay(RoomUser User, string Message)
        {

        }

        public override void OnUserShout(RoomUser User, string Message)
        {

        }

        internal void Reset()
        {
            if (GetBotData() == null)
                return;
            followX = 0;
            followY = 0;
            WentLooking = false;
            chasingid = 0;
            Busy = false;
            HasFoundExit = false;
            SaidThis = false;
            Timer = 0;
            subTimer = 0;
            if (GetBotData().Job != 15)
                GetRoomUser().ApplyEffect(0);
            GetRoomUser().CarryItem(0);
            if (GetBotData().Job == 1)
                this.KickTimer = 50;
            Console.WriteLine("Kicktimer engaged!");
            if (GetBotData().Job == 4 && GetRoomUser().RoomId != SpawnRoom)
                this.KickTimer = 35;
            if (GetBotData().Job == 15)
            {
                KickTimer = 0;
                SaidThis = true;
                KickDelay = 0;
            }
            if (GetBotData().Job == 2 || GetBotData().Job == 3)
                this.KickTimer = 5;

        }

        internal void Leave()
        {
            if (GetBotData() == null)
                return;
            if (GetBotData().Job == 1 || GetBotData().Job == 4)
                GetRoom().BotsAllowed -= 1;
            GetRoom().GetGameMap().RemoveUserFromMap(GetRoomUser(), new System.Drawing.Point(GetRoomUser().X, GetRoomUser().Y));
            GetRoom().GetRoomUserManager().RemoveBot(GetRoomUser().VirtualId, false);
        }

        public void Faint()
        {
            RoomUser roomUser = GetRoomUser();
            if (roomUser.IsWalking)
                roomUser.ClearMovement(true);
            if ((roomUser.RotBody % 2) == 0)
            {
                if (this != null)
                {
                    try
                    {
                        roomUser.Statusses.Add("lay", "1.0 null");
                        roomUser.Z -= 0.35;
                        roomUser.UpdateNeeded = true;
                    }
                    catch { }
                }
                else
                {
                    roomUser.RotBody--;
                    roomUser.Statusses.Add("lay", "1.0 null");
                    roomUser.Z -= 0.35;
                    roomUser.UpdateNeeded = true;
                }
            }
            else
            {
                roomUser.RotBody = 2;
                roomUser.RotHead = 2;
                roomUser.Statusses.Add("lay", "1.0 null");
                roomUser.Z -= 0.35;
                roomUser.UpdateNeeded = true;
            }
        }

        public void Say(string message)
        {
            string prevName = GetBotData().Name;
            GetBotData().Name = "*" + GetBotData().Name;
            message = message + "*";
            GetRoomUser().GetRoom().SendPacket(new UsersComposer(GetRoomUser()));
            GetRoomUser().Chat("" + message + "", 31);
            GetBotData().Name = prevName;
            GetRoomUser().GetRoom().SendPacket(new UsersComposer(GetRoomUser()));
        }

        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;
            if (GetBotData().Job == 4 && SpawnRoom == 0)
            {
                if (GetBotData().Name == "Jackson")
                    SpawnRoom = 2;
                else SpawnRoom = 5;
            }
            if (cooldown > 0)
                cooldown--;
            if (saycooldown > 0)
                saycooldown--;
            if (callpolice > 0)
                callpolice--;
            if (replyCD > 0)
                replyCD--;
            if (GetRoomUser().bypass)
            {
                SaidThis = true;
                KickDelay = 0;
                itemId = 0;
                Busy = false;
                bypass = true;
                GetRoomUser().bypass = false;
            }
            if (affect > 0)
            {
                affect--;
                if (affect == 0)
                    GetRoomUser().ApplyEffect(0);
            }
            if (!check && (GetBotData().Job == 6 || GetBotData().Job == 9))
            {
                if (GetBotData().Job == 9)
                    Strength = 9;
                if (Team == null)
                {
                    if (GetRoom().Id == 182)
                        Team = "Green";
                    else if (GetRoom().Id == 174)
                        Team = "Blue";
                }
                if (GetBotData().Name == "God-Father")
                {
                    GetBotData().Health = 250;
                    GetRoomUser().maxhealth = 250;
                    Strength = 11;

                }
                else if (GetBotData().Name.Contains("Thug"))
                {
                    GetBotData().Health = 150;
                    GetRoomUser().maxhealth = 150;
                    Strength = 8;
                }
                check = true;
            }
            if (GetRoomUser().reset)
            {
                GetRoomUser().reset = false;
                Reset();
            }
            if (OnEnter)
            {
                OnEnter = false;
                if (GetRoom().BotsAllowed > 0)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        if (User.Assault > 0)
                            User.Assault = 0;
                    }
                }
                if (GetBotData().Job == 2 || GetBotData().Job == 3)
                {
                    if (GetRoom().Taxi == 0)
                        GetRoomUser().ApplyEffect(20);
                }
                if (GetBotData().Job == 15)
                    GetRoomUser().ApplyEffect(19);
            }

            if (GetRoomUser().Stunned == 0 && !GetBotData().Cuffed && GetBotData().Health > 0)
            {
                #region hello?
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                {
                    if (User.LastMessage.Contains("" + GetBotData().Name + "") && replyCD == 0 && !Busy)
                    {
                        if (PlusEnvironment.GetRandomNumber(1, 4) > 2)
                            GetRoomUser().Chat("Hello!", 29);
                        else GetRoomUser().Chat("Can I help you? " + User.GetUsername() + "", 31);
                        if (GetRoomUser().IsWalking)
                            GetRoomUser().ClearMovement(true);
                        GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                        GetRoom().SendPacket(new ActionComposer(GetRoomUser().VirtualId, 1));
                        User.LastMessage = "";
                        replyCD = 200;
                    }
                    else if (User.LastMessage.Contains("" + GetBotData().Name + ""))
                        User.LastMessage = "";
                }
                #endregion

                #region Call For Help
                if (GetBotData().Job != 1 && GetBotData().Job != 4
                    && GetBotData().Job != 6 && GetBotData().Job != 10
                    && GetBotData().Job != 3 && GetBotData().Job != 9
                    && GetBotData().Job != 12 && !GetRoom().Name.Contains("turf"))
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User.Assault > 0 && callpolice == 0 && GetRoom().BotsAllowed == 0)
                        {
                            if (User.GetClient().GetHabbo().GetClientManager().NYPDactive == 0)
                            {
                                Say("dials the police for help");
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                User.GetClient().GetRolePlay().CallPoliceDelay = Random.Next(20, 60);
                                User.GetClient().GetRolePlay().callroomid = GetRoom().Id;
                            }
                            else if (User.GetClient().GetHabbo().GetClientManager().NYPDactive > 0)
                            {
                                Say("dials the police for help");
                                User.GetClient().GetHabbo().GetClientManager().PoliceCalls++;
                                User.GetClient().GetHabbo().GetClientManager().AddPC("[BOT] " + GetBotData().Name, GetRoom().Name, GetRoomUser().RoomId, "" + User.GetUsername() + " is attacking!", GetBotData().Look, "true", User.GetClient().GetHabbo().GetClientManager().PoliceCalls);
                            }
                            callpolice = 600;
                        }
                    }
                }
                #endregion

                #region Police Bots
                if (GetBotData().Job == 1 || GetBotData().Job == 4 || GetBotData().Job == 8)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        var This = User.GetClient().GetRolePlay();
                        if (This.Wanted && User.Assault == 0)
                        {
                            var WL = This.habbo.GetClientManager().GetWL(This.habbo.Username, 0);
                            if (WL != null)
                            {
                                if (WL.Reason1.Contains("assault") || WL.Reason1.Contains("Assault")
                                    || WL.Reason2.Contains("assault") || WL.Reason2.Contains("Assault")
                                    || WL.Reason3.Contains("assault") || WL.Reason3.Contains("Assault"))
                                    User.Assault = 1;
                                else if (WL.Reason1.Contains("murder") || WL.Reason1.Contains("Murder")
                                    || WL.Reason2.Contains("murder") || WL.Reason2.Contains("Murder")
                                    || WL.Reason3.Contains("murder") || WL.Reason3.Contains("Murder"))
                                    User.Assault = 9999;
                            }
                        }
                        if (chasingid == User.HabboId && This.BotEscort > 0 && This.BotEscort != GetBotData().Id && GetBotData().Escorting == 0)
                            Reset();
                        else if (chasingid == User.HabboId && Busy && (This.Health < 1 || User.Assault == 0 || This.EscortID > 0 || GetBotData().Escorting > 0))
                            Reset();
                        else if (User.Assault > 0 && This.Health > 0 && !This.Dead && This.Jailed <= 0 && This.JailedSec <= 0)
                        {
                            if (!Busy && chasingid == 0 && This.EscortID == 0 && GetBotData().Escorting == 0 && (This.BotEscort == 0 || This.BotEscort == GetBotData().Id))
                            {
                                if (!SayItAgain && !This.Cuffed)
                                {
                                    GetRoomUser().Chat("NYPD, FREEZE!", 31);
                                    SayItAgain = true;
                                }
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                This.Assault = true;
                                chasingid = User.HabboId;
                                this.Busy = true;
                                GetRoomUser().ApplyEffect(592);
                            }
                            if (User.HabboId != chasingid && BackUp == 0 && !User.calledon && GetRoom().BotsAllowed < 2 && This.BotEscort == 0 && This.EscortID == 0)
                            {
                                GetRoomUser().Chat("We got multiple suspects down here, backup requested!", 31);
                                if (This.habbo.GetClientManager().NYPDactive > 0)
                                {
                                    User.GetClient().GetHabbo().GetClientManager().PoliceCalls++;
                                    This.habbo.GetClientManager().AddPC(GetBotData().Name, GetRoom().Name, GetRoomUser().RoomId, "requesting backup!", GetBotData().Look, "true", User.GetClient().GetHabbo().GetClientManager().PoliceCalls);
                                }
                                else
                                {
                                    This.callroomid = GetRoom().RoomId;
                                    This.BackUpPolice = Random.Next(10, 30);
                                    This.callroomid2 = This.Room.Id;
                                }
                                BackUp = 300;
                                User.calledon = true;
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                #region Follow
                                this.followX = User.GoalX;
                                this.followY = User.GoalY;
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 2)
                                    HasFoundTile = true;
                                if (!HasFoundTile)
                                {
                                    if (User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    else if (!User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                }
                                HasFoundTile = false;
                                #endregion
                                subTimer--;

                                if (subTimer == 0 && Distance > 1)
                                {
                                    GetRoomUser().Chat("Stop where you are, " + User.GetUsername() + "!", 31);
                                    subTimer = 60;
                                }
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                if (Timer > 0 && Distance < 2)
                                    this.Timer--;
                                if (Distance <= 2 && User.Stunned == 0 && !This.Cuffed)
                                {
                                    Say("fires their stun-gun at " + User.GetUsername() + "");
                                    User.Stunned = 7;
                                    this.Timer = 5;
                                }
                                if (Distance <= 1 && User.Stunned > 0 && Timer == 0 && !This.Cuffed)
                                {
                                    Say("wraps their handcuffs around " + User.GetUsername() + "'s wrists");
                                    if (User.GetClient().GetRolePlay().Escorting > 0)
                                        User.GetClient().GetRolePlay().EndEscort();
                                    User.GetClient().GetRolePlay().SetRot(GetRoomUser().RotBody, false);
                                    This.Cuff();
                                    this.Timer = 5;
                                    User.ApplyEffect(590);
                                }
                                if (This.Cuffed && Distance < 2 && This.Room.Id == 5 && Timer == 0)
                                {
                                    GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                    if (User.Assault == 9999)
                                    {
                                        Say("arrests " + User.GetUsername() + " for Murder [10 minutes]");
                                        User.Jail(7, GetBotData().Name);
                                    }
                                    else if (User.Assault > 0)
                                    {
                                        Say("arrests " + User.GetUsername() + " for Assault [5 Minutes]");
                                        User.Jail(5, GetBotData().Name);
                                    }
                                    User.Stunned = 0;
                                    User.GetClient().GetRolePlay().BotEscort = 0;
                                    GetBotData().Escorting = 0;
                                    Reset();
                                }
                                else if (This.Cuffed && Distance < 2 && GetBotData().Escorting == 0 && Timer == 0)
                                {
                                    Say("begins to escort " + User.GetUsername() + "");
                                    This.BotEscort = GetBotData().Id;
                                    GetBotData().Escorting = User.HabboId;
                                    This.RoomForward(This.Room.Id);
                                    GetRoomUser().bypass = true;

                                }
                            }
                        }
                    }
                }
                #endregion

                #region Security Bots
                if (GetBotData().Job == 12)
                {

                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        var This = User.GetClient().GetRolePlay();
                        if (This.Wanted && User.Assault == 0)
                        {
                            var WL = This.habbo.GetClientManager().GetWL(This.habbo.Username, 0);
                            if (WL != null)
                            {
                                if (WL.Reason1.Contains("assault") || WL.Reason1.Contains("Assault")
                                    || WL.Reason2.Contains("assault") || WL.Reason2.Contains("Assault")
                                    || WL.Reason3.Contains("assault") || WL.Reason3.Contains("Assault"))
                                    User.Assault = 1;
                                else if (WL.Reason1.Contains("murder") || WL.Reason1.Contains("Murder")
                                    || WL.Reason2.Contains("murder") || WL.Reason2.Contains("Murder")
                                    || WL.Reason3.Contains("murder") || WL.Reason3.Contains("Murder"))
                                    User.Assault = 9999;
                            }
                        }
                        if (chasingid == User.HabboId && This.BotEscort > 0)
                            Reset();
                        else if (chasingid == User.HabboId && Busy && (This.Health < 1 || User.Assault == 0 || This.EscortID > 0 || This.BotEscort > 0 || This.Cuffed))
                            Reset();
                        else if (User.Assault > 0 && This.Health > 0 && !This.Dead && This.Jailed <= 0 && This.JailedSec <= 0 && !This.Cuffed)
                        {
                            if (!Busy && chasingid == 0)
                            {
                                if (!SayItAgain && !This.Cuffed)
                                {
                                    GetRoomUser().Chat("Stop where you are, " + User.GetUsername() + "!", 31);
                                    SayItAgain = true;
                                }
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                This.Assault = true;
                                chasingid = User.HabboId;
                                this.Busy = true;
                                GetRoomUser().ApplyEffect(592);
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                #region Follow
                                this.followX = User.GoalX;
                                this.followY = User.GoalY;
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 2)
                                    HasFoundTile = true;
                                if (!HasFoundTile)
                                {
                                    if (User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    else if (!User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                }
                                HasFoundTile = false;
                                #endregion
                                subTimer--;

                                if (subTimer == 0 && Distance > 1)
                                {
                                    GetRoomUser().Chat("Stop resisting, " + User.GetUsername() + "!", 31);
                                    subTimer = 60;
                                }
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                if (Timer > 0 && Distance < 2)
                                    this.Timer--;
                                if (Distance < 2 && User.Stunned == 0 && !This.Cuffed)
                                {
                                    Say("fires their stun-gun at " + User.GetUsername() + "");
                                    User.Stunned = 15;
                                    this.Timer = 5;
                                }
                                if (Distance <= 1 && User.Stunned > 0 && Timer == 0 && !This.Cuffed)
                                {
                                    Say("wraps their handcuffs around " + User.GetUsername() + "'s wrists");
                                    if (User.GetClient().GetRolePlay().Escorting > 0)
                                        User.GetClient().GetRolePlay().EndEscort();
                                    User.GetClient().GetRolePlay().SetRot(GetRoomUser().RotBody, false);
                                    This.Cuff();
                                    User.ApplyEffect(590);
                                    if (This.habbo.GetClientManager().NYPDactive > 0)
                                    {
                                        User.GetClient().GetHabbo().GetClientManager().PoliceCalls++;
                                        This.habbo.GetClientManager().AddPC(GetBotData().Name, GetRoom().Name, GetRoomUser().RoomId, "We have a suspect in custody!", GetBotData().Look, "true", User.GetClient().GetHabbo().GetClientManager().PoliceCalls);
                                    }
                                    else
                                    {
                                        This.callroomid = GetRoom().RoomId;
                                        This.BackUpPolice = Random.Next(10, 30);
                                        This.callroomid2 = This.Room.Id;
                                    }
                                    Say("calls for backup!");
                                    Reset();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Mafia Bots
                if (GetBotData().Job == 6)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        if (chasingid == User.HabboId && User.GetClient().GetHabbo().IsTeleporting)
                            Reset();
                        if (User.GetClient().GetRolePlay().GameType == "Mafia" && User.GetClient().GetRolePlay().Team != this.Team)
                        {
                            if (!Busy && chasingid == 0)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                chasingid = User.HabboId;
                                this.Busy = true;
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                #region Follow
                                this.followX = User.X;
                                this.followY = User.Y;
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 2)
                                    HasFoundTile = true;
                                if (Timer > 0)
                                    Timer--;
                                if (!HasFoundTile && Timer == 0)
                                {
                                    if (User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    else if (!User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    Timer = 6;
                                }
                                HasFoundTile = false;
                                #endregion
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                if (cooldown == 0 && Distance < 2)
                                {
                                    #region damage System
                                    int Damage = 0;
                                    int Randoms = Random.Next(1, 13);
                                    if (Randoms > 0 && Randoms < 7)
                                        Damage = Strength + 1;
                                    if (Randoms > 6 && Randoms < 10)
                                        Damage = Strength + 3;
                                    if (Randoms > 9 && Randoms < 12)
                                        Damage = Strength + Strength + 2;
                                    if (Randoms > 11 && Randoms < 14)
                                        Damage = Strength * 3 + 3;
                                    #endregion
                                    cooldown = 7;
                                    User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().Health - Damage, Damage);
                                    User.GetClient().GetRolePlay().DmgRespond();
                                    if (User.GetClient().GetRolePlay().Health > 0)
                                        Say("swings at " + User.GetUsername() + ", causing " + Damage + " damage");
                                    else if (User.GetClient().GetRolePlay().Health < 1)
                                    {
                                        Say("swings at " + User.GetUsername() + ", knocking them out");
                                        Reset();
                                        User.Assault = 0;
                                        User.GetClient().GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"ko\","
                                  + "\"name1\":\"" + GetBotData().Name + "\", \"name2\":\"" + User.GetUsername() + "\"}");
                                    }
                                }
                                else if (cooldown == 0 && Distance > 1 && Distance < 4)
                                {
                                    Say("swings at " + User.GetUsername() + ", but misses");
                                    cooldown = 7;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Gang Bots
                if (GetBotData().Job == 9)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        if (chasingid == User.HabboId && User.GetClient().GetHabbo().IsTeleporting || User.GetClient().GetRolePlay().AutoLogout > 0)
                            Reset();
                        if (User.GetClient().GetRolePlay().TurfTime > 0 || User.GetClient().GetRolePlay().roomUser.Assault == GetBotData().Id || chasingid == User.GetClient().GetRolePlay().habbo.Id)
                        {
                            if (!Busy && chasingid == 0)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                chasingid = User.HabboId;
                                this.Busy = true;
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                if (User.GetClient().GetRolePlay().Health < 1)
                                    Reset();
                                #region Follow
                                this.followX = User.X;
                                this.followY = User.Y;
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 2)
                                    HasFoundTile = true;
                                if (Timer > 0)
                                    Timer--;
                                if (!HasFoundTile && Timer == 0)
                                {
                                    if (User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    else if (!User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    Timer = 6;
                                }
                                HasFoundTile = false;
                                #endregion
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                if (cooldown == 0 && Distance < 2)
                                {
                                    #region damage System
                                    int Damage = 0;
                                    int Randoms = Random.Next(1, 13);
                                    if (Randoms > 0 && Randoms < 7)
                                        Damage = Strength + 1;
                                    if (Randoms > 6 && Randoms < 10)
                                        Damage = Strength + 3;
                                    if (Randoms > 9 && Randoms < 12)
                                        Damage = Strength + Strength + 2;
                                    if (Randoms > 11 && Randoms < 14)
                                        Damage = Strength * 3 + 3;
                                    #endregion
                                    cooldown = 7;

                                    User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().Health - Damage, Damage);
                                    User.GetClient().GetRolePlay().DmgRespond();
                                    if (User.GetClient().GetRolePlay().Health > 0)
                                        Say("swings at " + User.GetUsername() + ", causing " + Damage + " damage");
                                    else if (User.GetClient().GetRolePlay().Health < 1)
                                    {
                                        Say("swings at " + User.GetUsername() + ", knocking them out");
                                        User.Assault = 0;
                                    }
                                }
                                else if (cooldown == 0 && Distance > 1 && Distance < 4)
                                {
                                    Say("swings at " + User.GetUsername() + ", but misses");
                                    cooldown = 7;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Personal Bots
                if (GetBotData().Health > 0 && ((Master > 0 && GetBotData().Job == 10) || (GetBotData().Job == 20)))
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (chasingid == User.HabboId && Busy && User.Assault != Master)
                            Reset();
                        else if (chasingid == User.HabboId && Busy && (User.GetClient().GetRolePlay().Health < 1
                            || User.Assault == 0 || User.GetClient().GetRolePlay().EscortID > 0 || GetBotData().Escorting > 0))
                            Reset();
                        else if (User.Assault == Master && User.HabboId != Master && User.GetClient().GetRolePlay().Health > 0)
                        {
                            if (!Busy && chasingid == 0)
                            {
                                if (GetBotData().Job != 20)
                                    GetRoomUser().Chat("oh, no you don't!", 31);
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                chasingid = User.HabboId;
                                this.Busy = true;
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                #region Follow
                                this.followX = User.X;
                                this.followY = User.Y;
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 2)
                                    HasFoundTile = true;
                                if (Timer > 0)
                                    Timer--;
                                if (!HasFoundTile && Timer == 0)
                                {
                                    if (User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    else if (!User.IsWalking)
                                    {
                                        if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X - 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y - 1);
                                        else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                            GetRoomUser().MoveTo(User.X + 1, User.Y);
                                        else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                            GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    }
                                    Timer = 6;
                                }
                                HasFoundTile = false;
                                #endregion
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                if (cooldown == 0 && Distance < 2)
                                {
                                    #region damage System
                                    int Damage = 0;
                                    int Randoms = Random.Next(1, 13);
                                    int Strength = 17;
                                    if (Randoms > 0 && Randoms < 7)
                                        Damage = Strength + 1;
                                    if (Randoms > 6 && Randoms < 10)
                                        Damage = Strength + 3;
                                    if (Randoms > 9 && Randoms < 12)
                                        Damage = Strength + Strength + 2;
                                    if (Randoms > 11 && Randoms < 14)
                                        Damage = Strength * 3 + 3;
                                    #endregion
                                    cooldown = 7;

                                    User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().Health - Damage, Damage);
                                    User.GetClient().GetRolePlay().DmgRespond();
                                    if (User.GetClient().GetRolePlay().Health > 0)
                                        Say("swings at " + User.GetUsername() + ", causing " + Damage + " damage");
                                    else if (User.GetClient().GetRolePlay().Health < 1)
                                    {
                                        Say("swings at " + User.GetUsername() + ", knocking them out");
                                        Reset();
                                        User.Assault = 0;
                                        User.GetClient().GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"ko\","
                                    + "\"name1\":\"" + GetBotData().Name + "\", \"name2\":\"" + User.GetUsername() + "\"}");
                                    }
                                }
                                else if (cooldown == 0 && Distance > 1 && Distance < 5)
                                {
                                    Say("swings at " + User.GetUsername() + ", but misses");
                                    cooldown = 7;
                                }
                            }
                        }
                    }
                }
                if (GetBotData().Job == 10 && Master == 0 && GetBotData().Health > 0)
                {

                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null) return;
                        if (User.GetUsername() == "Clif" && Master == 0) // personal slut
                            Master = User.GetClient().GetRolePlay().habbo.Id;
                    }
                }
                if (Master > 0 && !Busy && GetBotData().Health > 0)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null)
                            return;
                        if (User.HabboId == Master)
                        {
                            if (User.GetClient().GetRolePlay().Dead || (User.GetClient().GetRolePlay().Jailed > 0 && User.GetClient().GetRolePlay().JailedSec > 0) || User.GetClient().GetRolePlay().Health < 1 || User.GetClient().GetRolePlay().AutoLogout > 0)
                                Leave();
                            int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                            if (Distance < 2)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                return;
                            }
                            GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                            #region Bot Movement
                            if (User.IsWalking)
                            {
                                if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                    GetRoomUser().MoveTo(User.X - 1, User.Y);
                                else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                    GetRoomUser().MoveTo(User.X, User.Y - 1);
                                else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                    GetRoomUser().MoveTo(User.X + 1, User.Y);
                                else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                    GetRoomUser().MoveTo(User.X, User.Y + 1);
                            }
                            else if (!User.IsWalking)
                            {
                                if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                    GetRoomUser().MoveTo(User.X - 1, User.Y);
                                else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                    GetRoomUser().MoveTo(User.X, User.Y - 1);
                                else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                    GetRoomUser().MoveTo(User.X + 1, User.Y);
                                else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                    GetRoomUser().MoveTo(User.X, User.Y + 1);
                            }
                            #endregion
                            if ((User.GetClient().GetRolePlay().Jailed > 0 && User.GetClient().GetRolePlay().JailedSec > 0)
                                || User.GetClient().GetRolePlay().Dead || User.GetClient().GetRolePlay().AutoLogout > 0 || User.GetClient().GetRolePlay().Cuffed)
                                Leave();

                        }
                    }
                }
                #endregion

                #region Medic Bots
                if (GetBotData().Job == 5)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null)
                            return;
                        if (chasingid == User.HabboId && User.GetClient().GetRolePlay().Health >= 75)
                            Reset();
                        if (User.LastMessage.Contains("medical aid") || User.LastMessage.Contains("heal") || User.HabboId == chasingid)
                        {
                            if (User.GetClient().GetRolePlay().Health <= 74 && !User.GetClient().GetRolePlay().Dead && User.GetClient().GetRolePlay().JustHealed == 0 && User.GetClient().GetHabbo().GetClientManager().HospWorkers == 0 && User.Assault == 0)
                            {
                                if (!Busy && chasingid == 0)
                                {
                                    GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), true);
                                    this.Busy = true;
                                    chasingid = User.HabboId;
                                    GetRoomUser().CarryItem(1014);
                                }

                                if (Busy && chasingid == User.HabboId && !User.GetClient().GetRolePlay().Dead && User.GetClient().GetRolePlay().AutoLogout == 0)
                                {
                                    #region Follow
                                    int dis = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                    if (dis < 2)
                                        HasFoundTile = true;
                                    if (!HasFoundTile)
                                    {
                                        if (User.IsWalking)
                                        {
                                            if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                                GetRoomUser().MoveTo(User.X - 1, User.Y);
                                            else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                                GetRoomUser().MoveTo(User.X, User.Y - 1);
                                            else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                                GetRoomUser().MoveTo(User.X + 1, User.Y);
                                            else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                                GetRoomUser().MoveTo(User.X, User.Y + 1);
                                        }
                                        else if (!User.IsWalking)
                                        {
                                            if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                                GetRoomUser().MoveTo(User.X - 1, User.Y);
                                            else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                                GetRoomUser().MoveTo(User.X, User.Y - 1);
                                            else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                                GetRoomUser().MoveTo(User.X + 1, User.Y);
                                            else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                                GetRoomUser().MoveTo(User.X, User.Y + 1);
                                        }
                                    }
                                    HasFoundTile = false;
                                    #endregion

                                    subTimer++;
                                    if (subTimer > 55 && User.GoalX != followX && User.GoalY > followY && User.IsWalking)
                                    {
                                        GetRoomUser().Chat("Hold still " + User.GetUsername() + ", I'm trying to help you!", 31);
                                        subTimer = 0;
                                    }
                                    int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                    if (Distance < 2)
                                    {
                                        GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                        if (Timer == 4)
                                        {
                                            Say("Injects " + User.GetUsername() + " with a syringe needle, healing their wound");
                                            User.GetClient().GetRolePlay().JustHealed = 10;
                                            User.ApplyEffect(583);
                                            User.GetClient().GetRolePlay().HealthChange(User.GetClient().GetRolePlay().Health + 5);
                                            User.LastMessage = "";
                                            Reset();
                                            GetRoomUser().CarryItem(0);
                                        }
                                        if (Busy)
                                            this.Timer++;
                                    }
                                }
                            }
                            else if (User.Assault > 0)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Sorry, but you're currently black listed!", 0, 0));
                                User.LastMessage = "";
                                cooldown = 35;
                                chasingid = 0;
                                return;
                            }
                            else if (User.GetClient().GetRolePlay().Health >= 75)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Sorry, I can only apply medical aid to those in an emergency!", 0, 0));
                                User.LastMessage = "";
                                chasingid = 0;
                                return;
                            }
                        }
                    }
                }
                #endregion

                #region Paramedic Bots
                if (this.GetBotData().Job == 2)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {

                        if (User == null) return;
                        var This = User.GetClient().GetRolePlay();
                        if (chasingid == User.HabboId && This.BotEscort > 0 && This.BotEscort != GetBotData().Id && GetBotData().Escorting == 0)
                            Reset();
                        else if (chasingid == User.HabboId && Busy && (This.Health > 0 || This.EscortID > 0 || GetBotData().Escorting > 0))
                            Reset();
                        else if (This.Health < 1)
                        {
                            if (!Busy && chasingid == 0 && This.EscortID == 0 && GetBotData().Escorting == 0)
                            {
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), true);
                                this.Busy = true;
                                this.chasingid = User.HabboId;
                            }
                            if (Busy && chasingid == User.HabboId)
                            {
                                if (Timer > 0)
                                    this.Timer++;

                                #region Bot Moving
                                if (GetRoomUser().X == User.X && GetRoomUser().Y == User.Y + 1 || GetRoomUser().X == User.X && GetRoomUser().Y == User.Y - 1 || GetRoomUser().X == User.X + 1 && GetRoomUser().Y == User.Y || GetRoomUser().X == User.X - 1 && GetRoomUser().Y == User.Y)
                                    HasFoundTile = true;
                                if (!HasFoundTile)
                                {
                                    if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                        base.GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                        base.GetRoomUser().MoveTo(User.X, User.Y - 1);
                                    else if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                        base.GetRoomUser().MoveTo(User.X - 1, User.Y);
                                    else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                        base.GetRoomUser().MoveTo(User.X + 1, User.Y);
                                }
                                HasFoundTile = false;
                                #endregion
                                int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                if (Distance < 3)
                                    GetRoomUser().ApplyEffect(0);
                                if (User.Stunned > 0 && Distance <= 1)
                                {
                                    Timer++;
                                    GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                    if (GetRoom().Id == 2)
                                    {
                                        User.Stunned = 0;
                                        This.DeadSetup(true, true);
                                        Timer = 0;
                                        User.GetClient().GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"discharge\","
                                   + "\"name1\":\"" + GetBotData().Name + "\", \"name2\":\"" + User.GetUsername() + "\"}");
                                        Reset();
                                    }
                                    else if (Timer > 5)
                                    {
                                        Say("begins to transfer " + This.habbo.Username + " to the nearest hospital");
                                        GetRoomUser().ApplyEffect(20);
                                        User.Statusses.Add("sit", "1.0");
                                        User.ApplyEffect(20);
                                        This.BotEscort = GetBotData().Id;
                                        GetBotData().Escorting = User.HabboId;
                                        GetRoomUser().bypass = true;

                                    }
                                }
                            }
                        }

                    }
                }
                #endregion

                #region Taxi Bots
                if (this.GetBotData().Job == 15)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {

                        if (chasingid == User.HabboId && User.GetClient().GetRolePlay().AutoLogout > 0 || User.GetClient().GetHabbo().IsTeleporting)
                            Reset();
                        if (!Busy && chasingid == 0 && User.GetClient().GetRolePlay().habbo.Id == GetBotData().Id)
                        {
                            this.Busy = true;
                            this.chasingid = User.HabboId;
                        }
                        if (Busy && chasingid == User.HabboId)
                        {
                            var This = User.GetClient().GetRolePlay();
                            if (Timer > 0)
                                this.Timer++;

                            #region Follow
                            this.followX = User.GoalX;
                            this.followY = User.GoalY;
                            int dis = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                            if (dis < 2)
                                HasFoundTile = true;
                            if (!HasFoundTile)
                            {
                                if (User.IsWalking)
                                {
                                    if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User) && User.GoalX > User.X)
                                        GetRoomUser().MoveTo(User.X - 1, User.Y);
                                    else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User) && User.GoalY > User.Y)
                                        GetRoomUser().MoveTo(User.X, User.Y - 1);
                                    else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User) && User.GoalX < User.X)
                                        GetRoomUser().MoveTo(User.X + 1, User.Y);
                                    else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User) && User.GoalY < User.Y)
                                        GetRoomUser().MoveTo(User.X, User.Y + 1);
                                }
                                else if (!User.IsWalking)
                                {
                                    if (GetRoom().GetGameMap().Path(User.X - 1, User.Y, User))
                                        GetRoomUser().MoveTo(User.X - 1, User.Y);
                                    else if (GetRoom().GetGameMap().Path(User.X, User.Y - 1, User))
                                        GetRoomUser().MoveTo(User.X, User.Y - 1);
                                    else if (GetRoom().GetGameMap().Path(User.X + 1, User.Y, User))
                                        GetRoomUser().MoveTo(User.X + 1, User.Y);
                                    else if (GetRoom().GetGameMap().Path(User.X, User.Y + 1, User))
                                        GetRoomUser().MoveTo(User.X, User.Y + 1);
                                    else User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Someone is blocking my path, come closer to me!", 0, 0));
                                }
                            }
                            HasFoundTile = false;
                            #endregion
                            subTimer++;
                            if (subTimer > 50 && User.IsWalking)
                            {
                                GetRoomUser().Chat("" + User.GetUsername() + ", stay still you fucker!", 31);
                                subTimer = 0;
                            }
                            if (dis < 2)
                            {
                                // GetRoomUser().Chat("2", true, 0);
                                if (User.GetClient().GetHabbo().Rank > 1)
                                    User.GetClient().GetRolePlay().UpdateCredits(2, false);
                                else User.GetClient().GetRolePlay().UpdateCredits(4, false);
                                User.Stunned = 0;
                                User.SetPos(GetRoomUser().X, GetRoomUser().Y, 0);
                                User.ApplyEffect(0);
                                User.GetClient().GetRolePlay().RoomForward(User.TaxiDest);
                                Room _Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(User.TaxiDest);
                                Say("Transports " + User.GetUsername() + " to " + _Room.Name + " [" + _Room.Id + "] ");
                                Reset();


                            }
                        }
                    }
                }
                #endregion

                #region Request Bots
                if (GetBotData().Job == 7 || GetBotData().Job == 11)
                {
                    foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                    {
                        if (User == null)
                            return;

                        #region Diner Bot
                        if (GetBotData().Job == 7)
                        {
                            if (User.CarryItemId != 3)
                            {
                                if (User.LastMessage.Contains("CARROT") || User.LastMessage.Contains("carrot") || chasingid > 0) // Diner
                                {
                                    if (User.Assault == 0)
                                    {
                                        int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                        if (Distance <= 10 && !Busy)
                                        {
                                            GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                            if (!Busy)
                                                Busy = true;
                                        }
                                        else if (cooldown == 0 && !Busy)
                                        {
                                            GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                            GetRoomUser().Chat("come closer to me, " + User.GetUsername() + "", 31);
                                            User.LastMessage = "";
                                            cooldown = 15;
                                        }
                                        if (Busy && Timer == 0)
                                        {
                                            foreach (Item item in GetRoom().GetRoomItemHandler().GetFloor.ToList())
                                            {
                                                if (item.Id == 46301)
                                                {

                                                    base.GetRoomUser().MoveTo(item.GetX + 1, item.GetY);
                                                    int FurniDistance = Math.Abs(item.GetX - GetRoomUser().X) + Math.Abs(item.GetY - GetRoomUser().Y);
                                                    if (FurniDistance == 1)
                                                    {
                                                        GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, item.GetX, item.GetY), false);
                                                        item.ExtraData = "1";
                                                        item.UpdateState(false, true);
                                                        GetRoomUser().CarryItem(3);
                                                        Timer = 6;

                                                    }
                                                }

                                            }
                                        }
                                    }
                                    else if (saycooldown <= 0)
                                    {
                                        User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Sorry, but you're currently black listed!", 0, 0));
                                        User.LastMessage = "";
                                        saycooldown = 35;
                                    }

                                    if (Timer > 0)
                                    {
                                        Timer--;
                                        if (Timer == 3)
                                        {
                                            foreach (Item item in GetRoom().GetRoomItemHandler().GetFloor.ToList())
                                            {
                                                if (item.Id == 46301)
                                                {
                                                    int FurniDistance = Math.Abs(item.GetX - GetRoomUser().X) + Math.Abs(item.GetY - GetRoomUser().Y);
                                                    if (FurniDistance == 1 && item.ExtraData == "1")
                                                    {
                                                        item.ExtraData = "0";
                                                        item.UpdateState(false, true);
                                                    }
                                                }
                                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                            }
                                        }
                                    }
                                    if (Timer == 1)
                                    {
                                        GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                        GetRoomUser().Chat("there you go :)", 31);
                                        User.CarryItem(3);
                                        User.LastMessage = "";
                                        GetRoomUser().CarryItem(0);
                                        Busy = false;
                                        Timer = 0;

                                    }

                                }
                            }
                        }
                        #endregion

                        #region Farmer Bot
                        if (GetBotData().Job == 11)
                        {
                            if ((User.LastMessage.Contains("BUY") || User.LastMessage.Contains("buy") || User.LastMessage.Contains("Buy")) && (User.LastMessage.Contains("SEED")
                                || User.LastMessage.Contains("seed") || User.LastMessage.Contains("Seed") || User.LastMessage.Contains("SEEDS")
                                || User.LastMessage.Contains("seeds") || User.LastMessage.Contains("Seeds")))
                            {
                                User.LastMessage = "";
                                if (User.Assault == 0)
                                {
                                    if (User.GetClient().GetRolePlay().AcceptOffer != "null")
                                        return;
                                    int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                    GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                    if (Distance <= 5 && !Busy)
                                    {
                                        Say("offers " + User.GetUsername() + " a pack of seeds for 25 dollars");
                                        User.GetClient().GetRolePlay().AcceptOffer = "seed";
                                        User.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + GetBotData().Name + "</b> is offering you a pack of <b>Plant Seeds</b> (3) for <b>25</b> dollars!\"}");
                                        User.GetClient().GetRolePlay().OfferAmount = 25;
                                    }
                                    else if (cooldown == 0 && !Busy)
                                    {
                                        GetRoomUser().Chat("come closer to me, " + User.GetUsername() + "", 31);
                                        cooldown = 15;
                                    }
                                }
                                else if (saycooldown == 0)
                                {
                                    User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Sorry, but you're currently black listed!", 0, 0));
                                    saycooldown = 35;
                                }
                            }
                            else if ((User.LastMessage.Contains("SELL") || User.LastMessage.Contains("sell") || User.LastMessage.Contains("Sell")) &&
                                (User.GetClient().GetRolePlay().Inventory.Equip1 == "flower" || User.GetClient().GetRolePlay().Inventory.Equip1 == "carrot" ||
                                User.GetClient().GetRolePlay().Inventory.Equip1 == "weed"))
                            {
                                User.LastMessage = "";
                                if (User.Assault == 0)
                                {
                                    int Distance = Math.Abs(User.X - GetRoomUser().X) + Math.Abs(User.Y - GetRoomUser().Y);
                                    GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y), false);
                                    if (Distance <= 5 && !Busy)
                                    {
                                        if (User.GetClient().GetRolePlay().Inventory.Currslot1 != "null")
                                        {
                                            string currslot = User.GetClient().GetRolePlay().Inventory.Currslot1;
                                            if (User.GetClient().GetRolePlay().Inventory.IsSlotEmpty(currslot))
                                            {
                                                User.GetClient().GetRolePlay().WebHandler.Handle("equip", "", "e1");
                                                User.GetClient().GetRolePlay().Inventory.Additem(currslot, true);
                                            }
                                            else
                                            {
                                                User.GetClient().GetRolePlay().Inventory.Additem(currslot, true);
                                                User.GetClient().GetRolePlay().WebHandler.Handle("equip", "", "e1");
                                            }
                                            User.GetClient().GetRolePlay().UpdateCredits(2, true);
                                            Say("hands " + User.GetUsername() + " 2 dollars for their item");
                                        }
                                    }
                                    else if (cooldown == 0 && !Busy)
                                    {
                                        GetRoomUser().Chat("come closer to me, " + User.GetUsername() + "", 31);
                                        cooldown = 15;
                                    }
                                }
                                else if (saycooldown == 0)
                                {
                                    User.GetClient().SendPacket(new WhisperComposer(GetRoomUser().VirtualId, "Sorry, but you're currently black listed!", 0, 0));
                                    saycooldown = 35;
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region Bot Exit
                if (!Busy)
                {
                    if (GetBotData().Job > 0 && GetBotData().Job < 4 || GetBotData().Job == 4 && GetRoomUser().RoomId != SpawnRoom || GetBotData().Job == 15 || bypass)
                    {
                        if (KickTimer <= 0 && !SaidThis)
                        {
                            if (GetBotData().Job == 1 || GetBotData().Job == 4)
                                GetRoomUser().Chat("Everything seems to be under control here, I should get going..", 31);
                            if (GetBotData().Job == 2)
                                GetRoomUser().Chat("My job seems to be done here, I should get going..", 31);
                            SaidThis = true;
                        }
                        if (KickDelay <= 0 && itemId == 0)
                        {
                            foreach (Item item in GetRoom().GetRoomItemHandler().GetFloor.ToList())
                            {
                                if (item.BaseItem == 9999 || item.BaseItem == 7887)
                                {
                                    this.followX = item.GetX;
                                    this.followY = item.GetY;
                                    this.itemId = item.Id;
                                    HasFoundExit = true;

                                }
                                else if (item.IsTrans && !HasFoundExit)
                                {
                                    this.followX = item.GetX;
                                    this.followY = item.GetY;
                                    this.itemId = item.Id;
                                    HasFoundExit = true;
                                }
                            }
                            if (!HasFoundExit)
                                Leave();
                        }
                        if (HasFoundExit)
                        {
                            GetRoomUser().MoveTo(followX, followY);
                            var user = GetRoomUser().GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().Escorting);
                            if (GetBotData().Escorting > 0 && user != null)
                            {
                                var This = user.GetClient().GetRolePlay();
                                if (This.Health < 1 && GetBotData().Job != 2)
                                {
                                    This.BotEscort = 0;
                                    GetBotData().Escorting = 0;
                                    Reset();
                                    return;
                                }
                            }
                            else if (GetBotData().Escorting > 0 && user == null)
                            {
                                GetBotData().Escorting = 0;
                                Reset();
                                return;
                            }
                            if (GetRoomUser().X == followX && GetRoomUser().Y == followY)
                            {
                                foreach (Item item in GetRoom().GetRoomItemHandler().GetFloor.ToList())
                                {
                                    if (item.IsTrans && itemId == item.Id)
                                    {
                                        if (GetBotData().Escorting > 0)
                                        {
                                            var This = user.GetClient().GetRolePlay();
                                            if (This.Health < 1 && GetBotData().Job != 2)
                                            {
                                                This.BotEscort = 0;
                                                GetBotData().Escorting = 0;
                                                Reset();
                                                return;
                                            }
                                            if (GetBotData().Job != 2 && This.Room.Id != 10)
                                                This.RoomForward(20);
                                            else if (GetBotData().Job == 2 && This.Room.Id != 2)
                                            {
                                                user.GetClient().GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"discharge\","
                              + "\"name1\":\"" + GetBotData().Name + "\", \"name2\":\"" + user.GetUsername() + "\"}");
                                                Reset();
                                                This.DeadSetup(true, true);
                                            }
                                            if (This.habbo.InRoom)
                                            {
                                                This.Room.BotsAllowed += 1;
                                                if (GetBotData().Job == 8)
                                                    GetBotData().Job = 1;
                                                var Rnd = new List<RandomSpeech>();
                                                This.Room.GetRoomUserManager().DeployBot(new RoomBot(GetBotData().Id, user.RoomId, "generic", "freeroam", GetBotData().Name, GetBotData().Motto, GetBotData().Look, This.Room.GetGameMap().Model.DoorX, This.Room.GetGameMap().Model.DoorY, 0.0, This.Room.GetGameMap().Model.DoorOrientation, 0, 0, 0, 0, ref Rnd, GetBotData().Gender, 0, 1, false, 0, false, 0, GetBotData().Job, GetBotData().Health), null);
                                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, item.GetX, item.GetY), false);
                                            }
                                        }
                                        else if (GetBotData().Job == 4)
                                        {
                                            Room Rooms = PlusEnvironment.GetGame().GetRoomManager().GetRoom(SpawnRoom);
                                            foreach (Item item2 in Rooms.GetRoomItemHandler().GetFurniObjects(Rooms.GetGameMap().Model.DoorX, Rooms.GetGameMap().Model.DoorY).ToList())
                                            {
                                                if (item2 != null && item2.BaseItem != 7908)
                                                {
                                                    item2.ExtraData = "2";
                                                    item2.UpdateState(false, true);
                                                }
                                            }
                                            Rooms.BotsAllowed += 1;
                                            var Rnd = new List<RandomSpeech>();
                                            Rooms.GetRoomUserManager().DeployBot(new RoomBot(GetBotData().Id, Rooms.Id, "generic", "freeroam", GetBotData().Name, GetBotData().Motto, GetBotData().Look, Rooms.GetGameMap().Model.DoorX, Rooms.GetGameMap().Model.DoorY, 0.0, Rooms.GetGameMap().Model.DoorOrientation, 0, 0, 0, 0, ref Rnd, GetBotData().Gender, 0, 1, false, 0, false, 0, GetBotData().Job, GetBotData().Health), null);
                                        }
                                        GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, item.GetX, item.GetY), false);
                                        if (item.BaseItem != 7908)
                                        {
                                            item.ExtraData = "2";
                                            item.UpdateState(false, true);
                                        }
                                        Leave();
                                    }
                                }
                            }
                        }
                    }
                    if (SaidThis)
                        TakingtooLong++;
                    if (KickTimer > 0)
                        KickTimer--;
                    if (SaidThis)
                        KickDelay--;
                    if (KickTimer > 5 && GetBotData().Job == 2)
                        KickTimer = 5;
                    if (KickDelay > 5 && GetBotData().Job == 2)
                        KickDelay = 5;

                }
                if (TakingtooLong > 60)
                    Leave();
                #endregion

                #region Chase User
                if (WentLooking && !Gone)
                {
                    GetRoomUser().MoveTo(followX, followY);
                    this.TakingtooLong++;
                    if (GetRoomUser().X == followX && GetRoomUser().Y == followY)
                    {
                        foreach (Item item in GetRoom().GetRoomItemHandler().GetFurniObjects(followX, followY).ToList())
                        {
                            if (item != null && item.IsTrans)
                            {
                                GetRoomUser().MoveTo(item.GetX, item.GetY);
                                int LinkedTele = ItemTeleporterFinder.GetLinkedTele(item.Id, GetRoom());
                                int TeleRoomId = ItemTeleporterFinder.GetTeleRoomId(LinkedTele, GetRoom());
                                Room Rooms = PlusEnvironment.GetGame().GetRoomManager().GetRoom(TeleRoomId);

                                foreach (Item item2 in Rooms.GetRoomItemHandler().GetFloor.ToList())
                                {
                                    if (item2 != null && item2.Id == LinkedTele)
                                    {
                                        int RoomId = TeleRoomId;
                                        int X = item2.GetX;
                                        int Y = item2.GetY;
                                        int rot = 0;
                                        if (item.Rotation == 4)
                                            rot = 4;
                                        else if (item.Rotation == 0)
                                            rot = 10;
                                        else if (item.Rotation == 6)
                                            rot = 6;
                                        else if (item.Rotation == 2)
                                            rot = 2;


                                        if (Rooms.RoomData == null)
                                            Reset();
                                        var Rnd = new List<RandomSpeech>();
                                        Rooms.BotsAllowed += 1;
                                        if (!Gone)
                                        {
                                            if (item2.BaseItem != 7908)
                                            {
                                                item2.ExtraData = "2";
                                                item2.UpdateState(false, true);
                                            }
                                            Rooms.GetRoomUserManager().DeployBot(new RoomBot(GetBotData().Id, RoomId, "generic", "freeroam", GetBotData().Name, GetBotData().Motto, GetBotData().Look, X, Y, 0.0, rot, 0, 0, 0, 0, ref Rnd, GetBotData().Gender, 0, 1, false, 0, false, 0, GetBotData().Job, GetBotData().Health), null);
                                        }
                                    }
                                    else
                                        Reset();
                                }
                                Gone = true;
                                WentLooking = false;
                                GetRoomUser().SetRot(Rotation.Calculate(GetRoomUser().X, GetRoomUser().Y, item.GetX, item.GetY), false);
                                if (item.BaseItem != 7908)
                                {
                                    item.ExtraData = "2";
                                    item.UpdateState(false, true);
                                }
                                Leave();
                            }
                            else
                                Reset();
                        }
                    }
                }
                if (GetBotData().Job == 1 || GetBotData().Job == 4 || GetBotData().Job == 8 || GetBotData().Job == 12)
                {
                    if (Busy && chasingid > 0 && !WentLooking && !Gone)
                    {
                        GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(chasingid);
                        if (Target == null)
                            Reset();
                        else if (Target.GetRolePlay().Room.Id != GetRoom().RoomId && Target.GetRolePlay().Jailed == 0 && Target.GetRolePlay().JailedSec == 0)
                        {
                            if (GetBotData().Job == 12)
                                Reset();
                            else
                            {
                                GetRoomUser().Chat("Suspect is getting away, backup requested!", 31);
                                WentLooking = true;
                                Timer = 0;
                            }
                            var WL = Target.GetHabbo().GetClientManager().GetWL(Target.GetHabbo().Username);
                            if (Target.GetRolePlay().Wanted && WL != null)
                            {
                                if (WL.Reason1.Contains("assault") || WL.Reason1.Contains("Assault")
                                    || WL.Reason2.Contains("assault") || WL.Reason2.Contains("Assault")
                                    || WL.Reason3.Contains("assault") || WL.Reason3.Contains("Assault")
                                    || WL.Reason1.Contains("murder") || WL.Reason1.Contains("Murder")
                                    || WL.Reason2.Contains("murder") || WL.Reason2.Contains("Murder")
                                    || WL.Reason3.Contains("murder") || WL.Reason3.Contains("Murder"))
                                    return;
                            }
                            if (WL != null)
                            {
                                if (Target.GetRolePlay().habbo.GetClientManager().AddReason("assault", WL))
                                { }
                            }
                            else Target.GetHabbo().GetClientManager().AddWL(Target.GetHabbo().Username, Target.GetHabbo().Look, "assault", Target.GetRolePlay().Color);
                            if (WL == null)
                                WL = Target.GetHabbo().GetClientManager().GetWL(Target.GetHabbo().Username);
                            if (Target.GetRolePlay().habbo.GetClientManager().AddReason("evade", WL))
                            { }
                            if (Target.GetHabbo().InRoom)
                                Target.SendWhisper("You have been charged with assault and evading!");
                            Target.GetHabbo().GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"charge\","
    + "\"name1\":\"" + GetBotData().Name + "\", \"name2\":\"" + Target.GetHabbo().Username + "\","
+ "\"color1\":\"#FFFFFF\", \"color2\":\"" + Target.GetRolePlay().Color + "\", \"charge\":\"assault\"}");
                        }
                    }
                }
                else if (GetBotData().Job == 10 && Master > 0)
                {
                    GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Master);
                    if (Target.GetRolePlay().Room.Id != GetRoom().RoomId || Target == null)
                        Leave();
                }
                #endregion

            }

            #region Health Management
            if (GetBotData().Health < 0)
                GetBotData().Health = 0;
            if (GetBotData().Health < 1 && GetRoomUser().Stunned == 0)
            {
                GetRoomUser().Stunned = 60;
                Faint();
                if (GetBotData().Escorting > 0)
                {
                    RoomUser User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().Escorting);
                    if (User != null)
                    {
                        User.GetClient().GetRolePlay().BotEscort = 0;
                        GetBotData().Escorting = 0;
                    }
                }
            }
            if (GetBotData().Health < 26 && GetRoomUser().Stunned == 0)
            {
                Delay++;
                if (Delay == 2 && Lowhealth)
                {
                    GetRoomUser().ApplyEffect(11);
                    Delay = 0;
                    Lowhealth = false;
                }
                else
                    if (Delay == 2 && !Lowhealth)
                {
                    GetRoomUser().ApplyEffect(0);
                    Delay = 0;
                    Lowhealth = true;
                }
                if (GetBotData().Job == 5 || GetBotData().Job == 3)
                {
                    GetBotData().HealthChange(GetBotData().Health + 50);
                    Say("Injects " + GetBotData().Name + " with a syringe needle, healing their wound");
                    GetRoomUser().ApplyEffect(583);
                    affect = 12;
                }
            }
            if (GetBotData().Stunned && GetRoomUser().Stunned == 0)
            {
                GetRoomUser().Stunned = 15;
                GetRoomUser().ClearMovement(true);
                GetRoomUser().ApplyEffect(53);
            }
            if (GetRoomUser().Stunned > 0 && !GetBotData().Cuffed)
            {
                GetRoomUser().Stunned--;
                if (GetRoomUser().Stunned == 0)
                {
                    if (GetBotData().Health < 1)
                        Leave();
                    else
                    {
                        GetBotData().Stunned = false;
                        GetRoomUser().ApplyEffect(0);
                    }
                }
            }
            if (GetBotData().Arrested)
                Leave();
            #endregion

            if (SpeechTimer <= 0 && !HasFoundExit && !Busy && Master == 0)
            {
                if (GetBotData().RandomSpeech.Count > 1)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

                    string String = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Speech.Message);
                    // if (String.Contains("<img src") || String.Contains("<font ") || String.Contains("</font>") || String.Contains("</a>") || String.Contains("<i>"))
                    //   String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String,GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;

            if (ActionTimer <= 0 && !HasFoundExit && !Busy && Master == 0 && GetRoomUser().Stunned == 0 && !GetBotData().Cuffed)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":
                        break;

                    case "freeroam":
                        nextCoord = GetRoom().GetGameMap().GetRandomWalkableSquare();
                        GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        break;

                    case "specified_range":

                        break;
                }
                if (GetBotData().Job == 5)
                    ActionTimer = Random.Next(10, 35);
                else if (GetBotData().Job == 6 || GetBotData().Job == 9)
                    ActionTimer = Random.Next(20, 55);
                else
                    ActionTimer = Random.Next(5, 15);
            }
            else
                ActionTimer--;
        }
    }
}