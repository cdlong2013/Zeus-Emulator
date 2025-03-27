using Fleck;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.PathFinding;
using System.Text.RegularExpressions;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using System.Collections.Concurrent;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.Users.Messenger;
using Plus.RolePlay.Gang;
using Plus.RolePlay.Timer;
using Plus.RolePlay.Police;
using Plus.RolePlay.Inventory;
using Plus.RolePlay.WebHandle;
using Plus.RolePlay.Trade;
using Plus.RolePlay.Storage;
using Plus.RolePlay.Job;
using MySqlX.XDevAPI;
using static System.Collections.Specialized.BitVector32;
using System.Reflection.Metadata.Ecma335;
using WebSocket;

namespace Plus.RolePlay
{
    public class RPData
    {
        public GameClient client;

        public RolePlayTimer RPTimer;

        public PoliceCall PoliceCalls;

        public WebManager _ws;

        public Equipment Inventory;

        public WebHandler WebHandler;

        public ItemTrade Trade;

        public ItemStorage Storage;

        public JobManager JobManager;

        #region Human
        private int _Health;
        private int _Energy;
        private int _Hunger;
        private bool _Dead;
        private int _Jailed;
        private int _Strength;
        private int _XP;
        private int _Level;
        private int _XPdue;
        private int _MaxHealth;
        public int MaxStrength = 0;
        public int MaxEnergy;
        #endregion

        #region Statistics
        private int _Kills;
        private int _Deaths;
        private int _Punches;
        private int _Arrests;
        public int SoloWin;
        public int SoloLost;
        #endregion

        #region Cooldowns/Timers
        public int Cooldown;
        public int subCooldown;
        public int Cooldown2;
        public int Cooldown3;
        public int TurfTime;
        public int TurfTimer;
        public int TempEffect;
        public int JustHealed;
        public int dubCooldown;
        public int takingtoolong;
        public int atmCD;
        #endregion

        #region A.I Related
        private int _HasPet;
        public int ParamedicBotDelay;
        public int CallPoliceDelay;
        public int SendToMafia;
        public int BackUpPolice;
        public int lockBot = 0;
        public int SpawnPet;
        #endregion

        #region Game
        public string GameType = "";
        public bool BeatGame;
        public string Team = "";
        public bool Spinning;
        public int SpinTimer;
        public int SpinSlot;
        public int spin1_pic1;
        public int spin1_pic2;
        public int spin1_pic3;
        public int spin2_pic1;
        public int spin2_pic2;
        public int spin2_pic3;
        public int spin3_pic1;
        public int spin3_pic2;
        public int spin3_pic3;
        #endregion

        #region Gang
        private int _Gang;
        public bool GangBoss;
        public int GangInvite;
        public int kills2;
        public int punches2;
        public int arrests2;
        public int jailbreak;
        public int captures;
        public int GangRank;
        public string RankName;
        public string GangDate;
        #endregion

        #region Basic
        public int GP;
        public int GPTimer;
        public int GPWarning;
        public int lockTarget;
        public bool onduty;
        public bool checkid;
        public bool isSleeping;
        public int bleeding;
        public int EscortID = 0;
        public int Escorting = 0;
        public int Delay;
        public int Delay2;
        public bool Lowhealth2;
        public bool Lowhealth;
        public int AutoLogout = 0;
        public bool Assault;
        private int _RoomX;
        private int _RoomY;
        private int _Rotate;
        public bool Loggedin = true;
        public bool Cuffed;
        public int callroomid;
        public int callroomid2;
        public bool ShadowClone;
        public int Pet;
        public int bonushp;
        public int boltkill;
        public int BotEscort = 0;
        public int hitcount;
        public int sleeptimer;
        public string _Color;
        public bool PetPlaced;
        public int item;
        public int itemtimer;
        public int lockID = 0;
        public int lockedon;
        public int InteractID;
        public int InteractingItem;
        public int InteractingTimer;
        public int InteractingCD;
        public string onlinetime;
        public string AcceptOffer = "null";
        public int AcceptTimer;
        public int OfferTimer;
        public int TradeTimer;
        public int OfferAmount;
        public int achievetimer;
        public int ItemCD;
        public int InteractATM;
        public int UpTime;
        public int PlantSeed;
        public int PlantSeedRoom;
        public int Seed;
        public int SeedTimer;
        public int WeedTimer;
        public int WeedTime;
        public int JailedSec;
        public bool trashwep;
        public bool personalbot;
        public int TradeType;
        public int TradeTarget;
        public bool WLopen;
        public int WLCurPage = 1;
        public bool actionpoint;
        public string bio;
        public string prevColor;
        public string Color1;
        public string Color2;
        public int xpdelay;
        public int xpsave;
        public int fishing;
        public int fishingCD;
        public bool Wanted;
        public bool Skateboard;
        public bool DisplayRoomInfo;
        public string SoulMate;
        public int smID;
        public string smColor;
        public bool ExCon;
        public int FreeMoney = 600;
        public string stungun;
        #endregion

        #region Dragons
        public bool bdrag;
        public bool wdrag;
        public bool jdrag;
        public bool pdrag;
        public bool sdrag;
        #endregion

        #region Previous
        public int prevHealth;
        public int prevMaxHealth;
        public int prevEnergy;
        public int prevHunger;
        public bool prevDead;
        public int prevKills;
        public int prevDeaths;
        public int prevPunches;
        public int prevArrests;
        public int prevStrength;
        public int prevXPdue;
        public int prevJob;
        public int prevJobRank;
        public int prevLevel;
        public string prevMotto;
        public int prevGang;
        public string prevName = "";
        public string previousLook;
        public int prevShift;
        #endregion

        #region Enable
        public int enable_trade;
        public int enable_sound;
        public int enable_marcos;
        #endregion

        #region Marco
        public bool Marco;
        public string marco1_1 = "null";
        public string marco1_2 = "null";
        public string marco2_1 = "null";
        public string marco2_2 = "null";
        public string marco3_1 = "null";
        public string marco3_2 = "null";
        public string marco4_1 = "null";
        public string marco4_2 = "null";
        public string marco5_1 = "null";
        public string marco5_2 = "null";
        #endregion

        #region Set Value
        public int Health
        {
            get { return this._Health; }
            set { this._Health = value; }
        }
        public int Energy
        {
            get { return this._Energy; }
            set { this._Energy = value; }
        }
        public int Hunger
        {
            get { return this._Hunger; }
            set { this._Hunger = value; }
        }
        public bool Dead
        {
            get { return this._Dead; }
            set { this._Dead = value; }
        }
        public int Jailed
        {
            get { return this._Jailed; }
            set { this._Jailed = value; }
        }
        public int Strength
        {
            get { return this._Strength; }
            set { this._Strength = value; }
        }
        public int XP
        {
            get { return this._XP; }
            set { this._XP = value; }
        }
        public int Level
        {
            get { return this._Level; }
            set { this._Level = value; }
        }
        public int XPdue
        {
            get { return this._XPdue; }
            set { this._XPdue = value; }
        }
        public int Gang
        {
            get { return this._Gang; }
            set { this._Gang = value; }
        }
        public int MaxHealth
        {
            get { return this._MaxHealth; }
            set { this._MaxHealth = value; }
        }
        public int Kills
        {
            get { return this._Kills; }
            set { this._Kills = value; }
        }
        public int Deaths
        {
            get { return this._Deaths; }
            set { this._Deaths = value; }
        }
        public int Punches
        {
            get { return this._Punches; }
            set { this._Punches = value; }
        }
        public int Arrests
        {
            get { return this._Arrests; }
            set { this._Arrests = value; }

        }
        public int RoomX
        {
            get { return this._RoomX; }
            set { this._RoomX = value; }

        }
        public int RoomY
        {
            get { return this._RoomY; }
            set { this._RoomY = value; }

        }
        public int HasPet
        {
            get { return this._HasPet; }
            set { this._HasPet = value; }

        }
        public int Rotate
        {
            get { return this._Rotate; }
            set { this._Rotate = value; }

        }
        public string Color
        {
            get { return this._Color; }
            set { this._Color = value; }

        }
        #endregion

        public RolePlayTimer GetTimer()
        {
            RPTimer = new RolePlayTimer(this);
            return RPTimer;
        }

        public IWebSocketConnection ws { get; set; }

        public GangManager GangManager;

        public GameClient GetClient()
        {
            return this.client;
        }

        public Habbo habbo
        {
            get
            {
                return this.GetClient().GetHabbo();
            }
        }
        public WebManager WebSocket()
        {
            return _ws;
        }
        public RoomUser roomUser
        {
            get
            {
                if (Room == null)
                {
                    Console.WriteLine("Room is Null!!!");
                    return null;
                }
                if (Room.GetRoomUserManager() == null)
                {
                    Console.WriteLine("GetRoomUserManager() is null!!");
                    return null;
                }
                var User = Room.GetRoomUserManager().GetRoomUserByHabbo(client.GetHabbo().Id);
                if (User == null)
                {
                    Console.WriteLine("GetRoomUserByHabbo() returned null!!");
                    
                }
                return User;
                //return Room.GetRoomUserManager().GetRoomUserByHabbo(client.GetHabbo().Id);
            }
        }

        public Room Room
        {
            get
            {
                if (this.client.GetHabbo().CurrentRoomId <= 0)
                    return null;

                Room _room = null;
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(habbo.CurrentRoomId, out _room))
                    return _room;

                return null;
            }
        }

        public void SendWeb(string data)
        {
           
            if (AutoLogout > 0)
                return;
            if (ws == null)
                return;
            if (data == null)
                return;
            if (!data.Contains("{"))
                return;
            ws.Send(data);
            Console.WriteLine("SendWeb is initialized!");
        }

        public RPData(DataRow stat, GameClient _client)
        {
            if (stat == null)
            {
                throw new ArgumentNullException(nameof(stat), "Error: Datarow 'stat' is null");
            }
            this.client = _client;
            this._Health = (int)stat["health"];
            this._MaxHealth = (int)stat["maxhealth"];
            this._Energy = (int)stat["energy"];
            this._Dead = Convert.ToBoolean(stat["dead"]);
            this._Hunger = (int)stat["hunger"];
            this._Strength = (int)stat["strength"];
            this._XP = (int)stat["xp"];
            this._Jailed = (int)stat["jailed"];
            this._Level = (int)stat["level"];
            this._XPdue = (int)stat["xpdue"];
            this._Gang = (int)stat["gang"];
            this._Kills = (int)stat["kills"];
            this._Punches = (int)stat["hits"];
            this._RoomX = (int)stat["roomx"];
            this._RoomY = (int)stat["roomy"];
            this._Arrests = (int)stat["arrests"];
            this._Deaths = (int)stat["deaths"];
            this._HasPet = (int)stat["haspet"];
            this.GP = (int)stat["gp"];
            this._Rotate = (int)stat["rotation"];
            this._Color = (string)stat["color"];
            this.kills2 = (int)stat["gangkills"];
            this.punches2 = (int)stat["ganghits"];
            this.arrests2 = (int)stat["gangarrests"];
            this.jailbreak = (int)stat["gangjb"];
            this.captures = (int)stat["gangcap"];
            this.GangRank = (int)stat["gangrank"];
            this.WeedTimer = (int)stat["wtimer"];
            this.JailedSec = (int)stat["jailsec"];
            this.SoloWin = (int)stat["solowin"];
            this.SoloLost = (int)stat["sololost"];
            this.enable_marcos = (int)stat["enable_marcos"];
            this.enable_sound = (int)stat["enable_sound"];
            this.enable_trade = (int)stat["enable_trade"];
            this.bio = (string)stat["bio"];
            this.GangDate = (string)stat["gangdate"];
            this.Color1 = (string)stat["color1"];
            this.Color2 = (string)stat["color2"];
            this.smID = (int)stat["soulmate"];
            this.ExCon = Convert.ToBoolean((int)stat["excon"]);
            this.stungun = (string)stat["stungun"];
            this.MaxEnergy = Energy;
            this.Inventory = new Equipment(this);
            this.WebHandler = new WebHandler(this);
            this.Trade = new ItemTrade(this);
            this.Storage = new ItemStorage(this);
            this.JobManager = new JobManager(this);
            if (this.Health < 1 && !Dead)
            {
                this.Dead = true;
                HealthChange(1);
            }
            if (smID > 0)
                Marriage("data");
            Storage.SetBank();
            SetMarco();
            if (Gang > 0)
                SetGang(false);
            if (HasPet > 0)
            {
                using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    DB.RunQuery("UPDATE bots SET room_id = '0' WHERE id = '" + HasPet + "'");
            }
            if (_Color == "rainbow" && habbo.Rank == 1)
                WebHandler.Handle("namecolor", "#000000", "0");
            DragonType();
        }

        #region Basic RolePlay Features
        private static readonly Random Random = new Random();
        public static int Distance(Vector2D Pos1, Vector2D Pos2)
        {
            return Math.Abs(Pos1.X - Pos2.X) + Math.Abs(Pos1.Y - Pos2.Y);
        }
        public static bool AreUsersInRange(RoomUser User1, RoomUser User2, int range)
        {
            int DistanceA = Math.Abs(User1.X - User2.X) + Math.Abs(User1.Y - User2.Y);
            int DistanceB = Math.Abs(User1.SetX - User2.SetX) + Math.Abs(User1.SetY - User2.SetY);
            return DistanceA > range && DistanceB > range ? false : true;
        }
        public void Hit(RoomUser User, bool slap)
        {
            if (lockTarget > 0 && User == null)
            {
                var user = Room.GetRoomUserManager().GetRoomUserByHabbo(lockTarget);
                if (user != null)
                    User = user;
                else
                {
                    client.SendWhisper("User not found!");
                    return;
                }
            }
            var Target = User.GetClient().GetRolePlay();
            #region Warnings
            if (Cuffed || Cooldown > 0 || Energy < 1 || Health < 1 || Dead || Jailed > 0 || JailedSec > 0 || Target.Dead || Target.Jailed > 0 ||
               Target.JailedSec > 0 || roomUser.Statusses.ContainsKey("lay") || GameType != "" && Team == Target.Team || roomUser.Stunned > 0)
                Responds();
            else if (Skateboard)
                GetClient().SendWhisper("You cannot skate at this location.");
            else if (Room.Fight > 0)
                GetClient().SendWhisper("You cannot fight at this location.");
            else if (Target == this)
                GetClient().SendWhisper("You cannot perform this action on yourself.");
            else if (Target.onduty)
                GetClient().SendWhisper("Você não pode bater neste staff enquanto ele estiver de plantão!");
            else if (Target.roomUser.IsAsleep)
                GetClient().SendWhisper("You cannot perform this action on someone who is AFK.");
            else if (Target.Health < 1)
                GetClient().SendWhisper("You cannot perform this action while unconcious.");
            else if (Target.JobManager.Job == 7 && Target.JobManager.Working)
                GetClient().SendWhisper("Esta ação não pode ser executada em membros do SUS!");
            else if (JobManager.Job == 7 && JobManager.Working)
                GetClient().SendWhisper("Esta ação não pode ser executada durante o trabalho!");
            #endregion
            else
            {
                if (AreUsersInRange(roomUser, Target.roomUser, 1))
                {
                    if (GP > 0)
                    {
                        GPWarning++;
                        if (GPWarning < 3)
                            GetClient().SendWhisper("You are protected by God Zeus, if you decide to continue you will lose your protection! Warning: " + GPWarning + "/2");
                        else
                        {
                            GPTimer = 100;
                            GP = 1;
                        }
                    }
                    else if (Target.GP > 0)
                        GetClient().SendWhisper("That action cannot be performed on this user because they are passive.");
                    else
                    {
                        #region damage System
                        var xp = 0;
                        var str = Strength + MaxStrength;
                        var Damage = 0;
                        var randNum = 0;
                        var DamageType = "";
                        var Swing = "lands a";
                        var wepdmg = 0;
                        if (bdrag)
                            randNum = PlusEnvironment.GetRandomNumber(1, 14);
                        else
                            randNum = PlusEnvironment.GetRandomNumber(1, 13);
                        if (randNum >= 1 && randNum <= 7)
                        {
                            Damage = str + 1;
                            DamageType = "blow";
                        }
                        else if (randNum >= 8 && randNum <= 10)
                        {
                            Damage = str + 3;
                            DamageType = "good blow";
                        }
                        else if (randNum == 11 || randNum == 12)
                        {
                            Damage = str * 2 + 2;
                            DamageType = "great blow";
                        }
                        else if (randNum == 13)
                        {
                            Damage = str * 3 + 3;
                            DamageType = "critical blow";
                        }
                        else if (randNum == 14)
                        {
                            Damage = str * 3 + 3 + 12;
                            DamageType = "dragon blow";
                        }
                        if (slap)
                        {
                            Damage = 1;
                            Swing = "slaps";
                        }
                        else if (Inventory.Equip1 != "null")
                        {
                            DamageType = Inventory.Equip1;
                            if (Inventory.Equip1 == "bat")
                            {
                                Damage += 2;
                                wepdmg = 5;
                            }
                            else if (Inventory.Equip1 == "sword")
                            {
                                Damage += 3;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "axe")
                            {
                                Damage += 3;
                                wepdmg = 3;
                            }
                            else if (Inventory.Equip1 == "knife")
                            {
                                Damage += 2;
                                wepdmg = 3;
                            }
                            else if (Inventory.Equip1 == "battle_axe" || Inventory.Equip1 == "gold_battleaxe")
                            {
                                DamageType = "battle axe";
                                Damage += 4;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "chain_stick" || Inventory.Equip1 == "gold_chainstick")
                            {
                                DamageType = "chain stick";
                                Damage += 2;
                                wepdmg = 4;
                            }
                            else if (Inventory.Equip1 == "crowbar" || Inventory.Equip1 == "gold_crowbar")
                            {
                                DamageType = "crowbar";
                                Damage += 2;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "iron_bat" || Inventory.Equip1 == "gold_bat")
                            {
                                if (Inventory.Equip1 == "gold_bat")
                                    DamageType = "golden bat";
                                else DamageType = "iron bat";
                                Damage += 3;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "lightsaber" || Inventory.Equip1 == "gold_lightsaber")
                            {
                                DamageType = "lightsaber";
                                Damage += 10;
                                wepdmg = 0;
                            }
                            else if (Inventory.Equip1 == "long_sword" || Inventory.Equip1 == "golden_longsword")
                            {
                                DamageType = "long sword";
                                Damage += 5;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "metal_pipe" || Inventory.Equip1 == "gold_pipe")
                            {
                                if (Inventory.Equip1 == "gold_pipe")
                                    DamageType = "golden pipe";
                                else DamageType = "metal pipe";
                                Damage += 3;
                                wepdmg = 10;
                            }
                            else if (Inventory.Equip1 == "power_axe" || Inventory.Equip1 == "gold_poweraxe")
                            {
                                DamageType = "power axe";
                                Damage += 4;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "spike_ball" || Inventory.Equip1 == "gold_spikeball")
                            {
                                DamageType = "spike ball";
                                Damage += 3;
                                wepdmg = 3;
                            }
                            Swing = "swings their";
                        }
                        xp = Damage;
                        #endregion
                        Target.HealthChange(Target.Health - Damage, Damage);
                        if (Swing == "balança seus" && Inventory.Equip1 != "lightsaber")
                            UpdateEnergy(30, 2);
                        else UpdateEnergy(20, 2);
                        if (Target.Health > 0)
                        {
                            string at = "on";
                            if (Swing == "swings their")
                                at = "at";
                            if (Swing == "slaps")
                                Say("" + Swing + " " + Target.habbo.Username + ", causing" + Damage + " damage", true);
                            else Say("" + Swing + " " + DamageType + " " + at + " " + Target.habbo.Username + ", causing " + Damage + " damage", true);
                            if (lockTarget != Target.habbo.Id)
                            {
                                lockTarget = Target.habbo.Id;
                                if (lockID > 0)
                                    SendWeb("{\"name\":\"lock\", \"lock\":\"false\"}");
                                if (lockBot > 0)
                                    lockBot = 0;
                                client.SendWhisper("Alvo bloqueado em: " + Target.habbo.Username + "");
                            }
                        }
                        if (Target.Health < 1)
                            xp *= 2;
                        Target.DmgRespond();
                        roomUser.Assault = Target.habbo.Id;
                        PunchChange();
                        Cooldown = 3;
                        if (Swing != "balança seus")
                        {
                            if (Target.Health < 1)
                            {
                                xpsave += xp;
                                xpdelay = 3;
                            }
                            else XPSet(xp);
                        }
                        else
                            Inventory.ItemHealth("true", wepdmg);
                        if (Target.Health < 1)
                        {
                            if (GameType == "Brawl" && Target.GameType == GameType)
                            {
                                Say("swings at " + Target.habbo.Username + ", knocking them out of the arena", true, 4);
                                Room.Brawl--;
                                if (Room.Brawl < 2)
                                {
                                    HealthChange(MaxHealth + bonushp);
                                    GameType = "";
                                    habbo.Motto = prevMotto;
                                    RoomForward(21, 5);
                                }
                            }
                            else if (GameType == "Mafia" && Target.GameType == GameType)
                                Say("swings at " + Target.habbo.Username + ", knocking them out & sending them to the graveyard", true, 4);
                            else
                            {
                                Say("swings at " + Target.habbo.Username + ", knocking them out", true);
                                KillChange();
                                roomUser.Assault = 9999;
                                GetClient().log("" + habbo.Username + " killed " + Target.habbo.Username + "");
                                habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"ko\","
                                    + "\"name1\":\"" + habbo.Username + "\", \"name2\":\"" + Target.habbo.Username + "\"}");
                            }
                        }

                    }
                }
                else if (Room.Fight == 0)
                {
                    Say("swings at " + Target.habbo.Username + ", but misses", true, 4);
                    Cooldown = 3;
                    UpdateEnergy(15, 1);
                    if (lockTarget != Target.habbo.Id)
                    {
                        lockTarget = Target.habbo.Id;
                        if (lockID > 0)
                            SendWeb("{\"name\":\"lock\", \"lock\":\"false\"}");
                        if (lockBot > 0)
                            lockBot = 0;
                        client.SendWhisper("Target locked on: " + Target.habbo.Username + "");
                    }
                }
            }
            if (roomUser.IsAsleep)
                roomUser.UnIdle();
            if (User != roomUser && !Cuffed && roomUser.Stunned == 0)
                SetRot(Rotation.Calculate(roomUser.X, roomUser.Y, User.X, User.Y), false);
        }
        public void HitBot(int id, bool ispet)
        {
            if (roomUser.IsAsleep)
                roomUser.UnIdle();
            RoomUser Bot = null;
            if (!ispet)
            {
                if (!Room.GetRoomUserManager().TryGetBot(id, out Bot))
                    return;
            }
            else if (ispet && Room.GetRoomUserManager().TryGetPet(id, out Bot))
            { }

            if (Bot != null && Bot.BotData.Health > 0)
            {
                if (ispet && Bot.BotData.Id == HasPet)
                    GetClient().SendWhisper("You cannot attack your own pet!");
                else if (Cooldown > 0 || Dead || Jailed > 0 || JailedSec > 0 || roomUser.Stunned > 0 || Energy < 1 || Cuffed || Health < 1)
                    Responds();
                else if (Room.Fight > 0)
                    GetClient().SendWhisper("You cannot fight at this location!");
                else if (GP > 0)
                {
                    GPWarning += 1;
                    if (GPWarning <= 2)
                        GetClient().SendWhisper("You are protected by God Zeus, if you decide to continue you will lose your protection! Warning: " + GPWarning + "/2");
                    else
                    {
                        GPTimer = 100;
                        GP = 1;
                    }
                }
                else
                {
                    if (AreUsersInRange(roomUser, Bot, 1))
                    {
                        #region damage System
                        var xp = 0;
                        var str = Strength + MaxStrength;
                        var Damage = 0;
                        var randNum = 0;
                        var DamageType = "";
                        var Swing = "lands a";
                        var wepdmg = 0;
                        if (bdrag)
                            randNum = PlusEnvironment.GetRandomNumber(1, 14);
                        else
                            randNum = PlusEnvironment.GetRandomNumber(1, 13);
                        if (randNum >= 1 && randNum <= 7)
                        {
                            Damage = str + 1;
                            DamageType = "blow";
                        }
                        else if (randNum >= 8 && randNum <= 10)
                        {
                            Damage = str + 3;
                            DamageType = "good blow";
                        }
                        else if (randNum == 11 || randNum == 12)
                        {
                            Damage = str * 2 + 2;
                            DamageType = "great blow";
                        }
                        else if (randNum == 13)
                        {
                            Damage = str * 3 + 3;
                            DamageType = "critical blow";
                        }
                        else if (randNum == 14)
                        {
                            Damage = str * 3 + 3 + 12;
                            DamageType = "dragon blow";
                        }
                        if (Inventory.Equip1 != "null")
                        {
                            DamageType = Inventory.Equip1;
                            if (Inventory.Equip1 == "bat")
                            {
                                Damage += 2;
                                wepdmg = 5;
                            }
                            else if (Inventory.Equip1 == "sword")
                            {
                                Damage += 3;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "axe")
                            {
                                Damage += 3;
                                wepdmg = 3;
                            }
                            else if (Inventory.Equip1 == "knife")
                            {
                                Damage += 2;
                                wepdmg = 3;
                            }
                            else if (Inventory.Equip1 == "battle_axe" || Inventory.Equip1 == "gold_battleaxe")
                            {
                                DamageType = "battle axe";
                                Damage += 4;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "chain_stick" || Inventory.Equip1 == "gold_chainstick")
                            {
                                DamageType = "chain stick";
                                Damage += 2;
                                wepdmg = 4;
                            }
                            else if (Inventory.Equip1 == "crowbar" || Inventory.Equip1 == "gold_crowbar")
                            {
                                DamageType = "crowbar";
                                Damage += 2;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "iron_bat" || Inventory.Equip1 == "gold_bat")
                            {
                                if (Inventory.Equip1 == "gold_bat")
                                    DamageType = "golden bat";
                                else DamageType = "iron bat";
                                Damage += 3;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "lightsaber" || Inventory.Equip1 == "gold_lightsaber")
                            {
                                DamageType = "lightsaber";
                                Damage += 10;
                                wepdmg = 0;
                            }
                            else if (Inventory.Equip1 == "long_sword" || Inventory.Equip1 == "golden_longsword")
                            {
                                DamageType = "long sword";
                                Damage += 5;
                                wepdmg = 2;
                            }
                            else if (Inventory.Equip1 == "metal_pipe" || Inventory.Equip1 == "gold_pipe")
                            {
                                if (Inventory.Equip1 == "gold_pipe")
                                    DamageType = "golden pipe";
                                else DamageType = "metal pipe";
                                Damage += 3;
                                wepdmg = 10;
                            }
                            else if (Inventory.Equip1 == "power_axe" || Inventory.Equip1 == "gold_poweraxe")
                            {
                                DamageType = "power axe";
                                Damage += 4;
                                wepdmg = 1;
                            }
                            else if (Inventory.Equip1 == "spike_ball" || Inventory.Equip1 == "gold_spikeball")
                            {
                                DamageType = "spike ball";
                                Damage += 3;
                                wepdmg = 3;
                            }
                            Swing = "swings their";
                        }
                        xp = Damage;
                        #endregion
                        Bot.BotData.HealthChange(Bot.BotData.Health - Damage);
                        if (Bot.BotData.Health > 0)
                        {
                            string at = "on";
                            if (Swing == "swings their")
                            {
                                at = "at";
                                Inventory.ItemHealth("true", wepdmg);
                            }
                            Say("" + Swing + " " + DamageType + " " + at + " " + Bot.BotData.Name + ", causing " + Damage + " damage", true);
                            if (lockBot != Bot.BotData.Id)
                            {
                                lockBot = Bot.BotData.Id;
                                if (lockID > 0)
                                    SendWeb("{\"name\":\"lock\", \"lock\":\"false\"}");
                                if (lockTarget > 0)
                                    lockTarget = 0;
                                client.SendWhisper("Target locked on: " + Bot.BotData.Name + "");
                            }
                        }
                        if (Bot.BotData.Health < 1 && DamageType.Contains("blow"))
                            xp *= 2;
                        if (Swing == "swings their" && Inventory.Equip1 != "lightsaber")
                            UpdateEnergy(30, 2);
                        else UpdateEnergy(20, 2);
                        Cooldown = 5;
                        if (roomUser.Assault == 0)
                            roomUser.Assault = Bot.BotData.Id;
                        PunchChange();
                        if (Bot.BotData.Health < 1)
                        {
                            Bot.BotData.Health = 0;
                            Say("balança em " + Bot.BotData.Name + ", nocauteando-os", true);
                            KillChange();
                            roomUser.Assault = 9999;
                            if (Bot.BotData.Name == "Thug1" || Bot.BotData.Name == "Thug2")
                                xp += 25;
                            else if (Bot.BotData.Name == "God-Father")
                            {
                                xp += 50;
                                BeatGame = true;
                            }
                        }
                        if (DamageType.Contains("blow"))
                            XPSet(xp);

                    }
                    else if (Room.Fight == 0)
                    {
                        if (Cooldown > 0 || Energy < 1 || Health < 1 || roomUser.Stunned > 0 || Cuffed)
                            Responds();
                        else
                        {
                            Say("swings at " + Bot.BotData.Name + ", but misses", true);
                            UpdateEnergy(15, 1);
                            Cooldown = 3;
                            if (lockBot != Bot.BotData.Id)
                            {
                                lockBot = Bot.BotData.Id;
                                if (lockID > 0)
                                    SendWeb("{\"name\":\"lock\", \"lock\":\"false\"}");
                                if (lockTarget > 0)
                                    lockTarget = 0;
                                client.SendWhisper("Target locked on: " + Bot.BotData.Name + "");
                            }
                        }
                    }
                }
            }
            else GetClient().SendWhisper("" + Bot.BotData.Name + " is unconscious!");
            if (!Cuffed)
                SetRot(Rotation.Calculate(roomUser.X, roomUser.Y, Bot.X, Bot.Y), false);
        }
        public void Stun(string name)
        {
            //if (!Inventory.Equip1.Contains("stun"))
            //{
            //    GetClient().SendWhisper("You must equip a taser before performing this action.");
            //    return;
            //}

            if (subCooldown > 0 || Energy < 1)
            {
                Responds();
                return;
            }
            var User = Room.GetRoomUserManager().GetRoomUserByHabbo(name);
            if (lockTarget > 0)
            {
                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(lockTarget);
                if (Target != null && Target.GetHabbo().CurrentRoom.Id == Room.Id)
                    User = Target.GetHabbo().roomUser;
            }

            if (User == null)
            {
                var bot = Room.GetRoomUserManager().GetBotByName(Convert.ToString(name));
                User = bot;
                if (User == null)
                {
                    //foreach (RoomUser pet in Room.GetRoomUserManager()._pets.Values.ToList())
                    //    User = pet;
                    if (User == null || User.BotData.Name != name)
                    {
                        GetClient().SendWhisper("User not found!");
                        return;
                    }
                }
            }
            if (User == roomUser)
            {
                GetClient().SendWhisper("You cannot perform this action on yourself.");
                return;
            }
            if (User.Stunned > 0 || (User.HabboId > 0 && User.GetClient().GetRolePlay().Cuffed)
                || (User.HabboId == 0 && User.BotData.Cuffed))
            {
                GetClient().SendWhisper("This user is already stunned.");
                return;
            }
            if ((User.HabboId > 0 && User.GetClient().GetRolePlay().Health < 1)
                || (User.HabboId == 0 && User.BotData.Health < 1))
            {
                GetClient().SendWhisper("You cannot perform this action while unconscious!");
                return;
            }
            if (User.HabboId == 0 && (User.BotData.Job == 1 || User.BotData.Job == 4 || User.BotData.Job == 8 || User.BotData.Job == 2))
            {
                GetClient().SendWhisper("You cannot stun a bot.");
                return;
            }
            roomUser.SetRot(Rotation.Calculate(roomUser.X, roomUser.Y, User.X, User.Y), false);
            int dis = Math.Abs(User.X - roomUser.X) + Math.Abs(User.Y - roomUser.Y);
            if (name == "")
            {
                if (User.IsBot)
                    name = User.BotData.Name;
                else if (User.IsPet) name = User.PetData.Name;
                else name = User.GetUsername();
            }
            if (dis <= 2)
            {
                if (User.HabboId > 0)
                    User.Stunned = 7;
                else User.BotData.Stunned = true;
                if (User.IsPet)
                    Say("fires their stungun at " + name + "", true, 4);
                else Say("fires their stungun at " + name + "", true, 4);
                UpdateEnergy(25, 2);
            }
            else if (dis > 2 && dis < 7)
            {
                Say("fires their stungun at " + name + ", but misses", true, 37);
                UpdateEnergy(5, 1);
            }
            else if (dis > 6)
                GetClient().SendWhisper("This user is too far away to perform this action.");
            subCooldown = 5;
        }
        public static string Figure(string _Figure)
        {
            string _Uni;
            string FigurePartHair = _Figure;
            string GetHairPart;
            string FigurePartBeard = _Figure;
            string GetBeardPart;

            if (_Figure.Contains("hr"))
            {
                GetHairPart = Regex.Split(_Figure, "hr")[1];
                FigurePartHair = GetHairPart.Split('.')[0];
            }
            if (_Figure.Contains("fa"))
            {
                GetBeardPart = Regex.Split(_Figure, "fa")[1];
                FigurePartBeard = GetBeardPart.Split('.')[0];
            }
            string FigurePartBody = _Figure;
            string GetBodyPart;

            GetBodyPart = Regex.Split(_Figure, "hd")[1];
            FigurePartBody = GetBodyPart.Split('.')[0];

            if (_Figure.Contains("fa") && _Figure.Contains("hr"))
                _Uni = Convert.ToString("hr" + FigurePartHair + "." + "hd" + FigurePartBody + "." + "fa" + FigurePartBeard + ".");
            else if (_Figure.Contains("fa"))
                _Uni = Convert.ToString("hd" + FigurePartBody + "." + "fa" + FigurePartBeard + ".");
            else if (_Figure.Contains("hr"))
                _Uni = Convert.ToString("hr" + FigurePartHair + "." + "hd" + FigurePartBody + ".");
            else _Uni = Convert.ToString("hd" + FigurePartBody + ".");

            return _Uni;
        }
        public void XPSystem()
        {
            if (Strength < 1 && XP > 0)
            {
                XPdue += 75;
                Strength += 1;
                Say("levels up to " + Strength + " strength", true, 31);
                roomUser.ApplyEffect(59);
                TempEffect = 10;
                HealthChange(MaxHealth);
            }
            else if (XP >= XPdue)
            {
                XPdue *= 2;
                Level += 1;
                if (Strength < 11)
                {
                    Strength += 1;
                    MaxHealth += 2;
                    Say("levels up their strength to level " + Strength + "", false);
                }
                GetClient().SendWhisper("You have increased your strength to level " + Level + "!");
                roomUser.ApplyEffect(59);
                TempEffect = 10;
                SendWeb("{\"name\":\"level\", \"level\":\"" + Level + "\"}");
                HealthChange(MaxHealth);
                if (lockedon > 0)
                {
                    foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                    {
                        if (User == null)
                            return;
                        if (User.GetRolePlay().lockID == habbo.Id)
                            User.GetRolePlay().SendWeb("{\"name\":\"targetlevel\", \"level\":\"" + Level + "\"}");
                    }
                }
            }
            int _xpdue = XPdue;
            int _xp = XP;
            if (XPdue > 75)
            {
                _xpdue = XPdue / 2;
                _xp = XP - _xpdue;
            }
            SendWeb("{\"name\":\"xp\", \"xp\":\"" + _xp + "\", \"xpdue\":\"" + _xpdue + "\", \"xp1\":\"" + XP + "\", \"xp2\":\"" + XPdue + "\"}");
            if (lockedon > 0)
            {
                foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (User == null)
                        return;
                    if (User.GetRolePlay().lockID == habbo.Id)
                        User.GetRolePlay().SendWeb("{\"name\":\"targetxp\", \"xp\":\"" + _xp + "\", \"xpdue\":\"" + _xpdue + "\","
                                                   + " \"xp1\":\"" + XP + "\", \"xp2\":\"" + XPdue + "\"}");
                }
            }
        }
        public void RPCache(int CacheThis)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                int userID = habbo.Id;
                if (CacheThis == 1)
                    DB.RunQuery("UPDATE stats SET health = '" + Health + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 3)
                    DB.RunQuery("UPDATE stats SET energy = '" + Energy + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 2)
                    DB.RunQuery("UPDATE stats SET hunger = '" + Hunger + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 4)
                    DB.RunQuery("UPDATE stats SET xp = '" + XP + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 5)
                    DB.RunQuery("UPDATE stats SET xpdue = '" + XPdue + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 6)
                    DB.RunQuery("UPDATE stats SET hits = '" + Punches + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 7)
                    DB.RunQuery("UPDATE stats SET strength = '" + Strength + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 8)
                    DB.RunQuery("UPDATE stats SET arrests = '" + Arrests + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 9)
                    DB.RunQuery("UPDATE stats SET deaths = '" + Deaths + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 10)
                    DB.RunQuery("UPDATE stats SET jailed = '" + Jailed + "', jailsec = '" + JailedSec + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 11)
                    DB.RunQuery("UPDATE stats SET dead = '" + Convert.ToInt32(Dead) + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 12)
                    DB.RunQuery("UPDATE stats SET bio = '" + bio + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 13)
                    DB.RunQuery("UPDATE stats SET job = '" + JobManager.Job + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 14)
                    DB.RunQuery("UPDATE stats SET kills = '" + Kills + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 15)
                    DB.RunQuery("UPDATE stats SET jobrank = '" + JobManager.JobRank + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 16)
                    DB.RunQuery("UPDATE stats SET level = '" + Level + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 17)
                    DB.RunQuery("UPDATE stats SET gang = '" + Gang + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 18)
                    DB.RunQuery("UPDATE stats SET maxhealth = '" + MaxHealth + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 19)
                    DB.RunQuery("UPDATE stats SET gp = '" + GP + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 20)
                    DB.RunQuery("UPDATE stats SET shifts = '" + JobManager.Shifts + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 21)
                    DB.RunQuery("UPDATE bank SET active = '" + Storage.openaccount + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 22)
                    DB.RunQuery("UPDATE bank SET amount = '" + Storage.bankamount + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 23)
                    DB.RunQuery("UPDATE stats SET wtimer = '" + WeedTimer + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 24)
                    DB.RunQuery("UPDATE stats SET jobmin = '" + JobManager.Jobmin + "', jobsec = '" + JobManager.Jobsec + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 25)
                    DB.RunQuery("UPDATE stats SET jobdate = '" + JobManager.JobDate + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 26)
                    DB.RunQuery("UPDATE stats SET task1 = '" + JobManager.Task1 + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 27)
                    DB.RunQuery("UPDATE stats SET task2 = '" + JobManager.Task2 + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 28)
                    DB.RunQuery("UPDATE stats SET task3 = '" + JobManager.Task3 + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 29)
                    DB.RunQuery("UPDATE users SET credits = '" + habbo.Credits + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 30)
                    DB.RunQuery("UPDATE stats SET excon = '" + Convert.ToInt32(ExCon) + "' WHERE id = '" + userID + "'");
                else if (CacheThis == 31)
                    DB.RunQuery("UPDATE stats SET stungun = '" + stungun + "' WHERE id = '" + userID + "'");
            }
        }
        public void SetHomeRoom()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
             //   dbClient.RunQuery("UPDATE users SET home_room = '" + roomUser.RoomId + "' WHERE id = '" + habbo.Id + "'");
               // dbClient.RunQuery("UPDATE stats SET roomx = '" + roomUser.X + "', roomy = '" + roomUser.Y + "', rotation = '" + roomUser.RotBody + "' WHERE id = '" + habbo.Id + "' LIMIT 1");
            }
        }
        public void Responds()
        {
            if (Dead || Health < 1)
                GetClient().SendWhisper("That action cannot be performed while you are dead!");
            else if (Jailed > 0 || JailedSec > 0)
                GetClient().SendWhisper("That action cannot be performed while in jail!");
            else if (roomUser.Stunned > 0)
                GetClient().SendWhisper("That action cannot be performed while stunned!");
            else if (Cuffed)
                GetClient().SendWhisper("That action cannot be performed while cuffed!");
            else if (Cooldown > 0)
                GetClient().SendWhisper("This ability is on cooldown [" + Cooldown + "/3]");
            else if (Cooldown2 > 0)
                GetClient().SendWhisper("This ability is on cooldown (" + Cooldown2 + ")");
            else if (subCooldown > 0)
                GetClient().SendWhisper("This ability is on cooldown (" + subCooldown + ")");
            else if (dubCooldown > 0)
                GetClient().SendWhisper("This ability is on cooldown (" + dubCooldown + ")");
            else if (Cooldown3 > 0)
                GetClient().SendWhisper("This ability is on cooldown (" + Cooldown3 + ")");
            else if (atmCD > 0)
                GetClient().SendWhisper("This ability is on cooldown (" + atmCD + ")");
            else if (MaxEnergy < 1)
                GetClient().SendWhisper("You have no energy left to perform this action!");
            else if (Energy < 1)
                GetClient().SendWhisper("That action cannot be performed with low energy!");
            else if (GP > 0)
                GetClient().SendWhisper("That action cannot be peformed while in passive mode!");
            else if (Escorting > 0)
                GetClient().SendWhisper("That action cannot be performed while escorting!");
            else if (JobManager.Working)
                GetClient().SendWhisper("That action cannot be performed while working!");
            else
                GetClient().SendWhisper("That action cannot be performed at this time!");
        }
        public void CallPolice(string message)
        {
            if (CallPoliceDelay > 0)
            {
                //GetClient().SendWhisper("Você já chamou a polícia!");
                return;
            }
            if (Jailed > 0 || JailedSec > 0 || Dead || Cooldown2 > 0 || roomUser.Stunned > 0 || fishing > 0
                || GameType != "" || Room.Name.Contains("turf") || Room.Name.Contains("Turf") || Cuffed || EscortID > 0)
            {
                Responds();
                return;
            }
            if (Room.BotsAllowed > 1)
            {
                GetClient().SendWhisper("There is already a police bot at your location.");
                return;
            }           
            if (habbo.GetClientManager().NYPDactive > 0)
            {
                habbo.GetClientManager().PoliceCalls++;
                habbo.GetClientManager().AddPC(habbo.Username, Room.Name, Room.Id, message, habbo.Look, "false", habbo.GetClientManager().PoliceCalls, Color);
            }
            else
            {
                CallPoliceDelay = Random.Next(30, 80);
                callroomid = Room.Id;
            }
            if (habbo.GetClientManager().STAFFactive > 0 && habbo.GetClientManager().NYPDactive == 0)
            {
                habbo.GetClientManager().PoliceCalls++;
                habbo.GetClientManager().AddPC(habbo.Username, Room.Name, Room.Id, message, habbo.Look, "false", habbo.GetClientManager().PoliceCalls, Color);
            }
            if (roomUser.IsAsleep)
                roomUser.UnIdle();
            Say("calls the police for help", true, 4);
            Cooldown2 = 60;
        }
        public void RoomForward(int roomid, int delay = 0)
        {
            if (actionpoint)
            {
                actionpoint = false;
                Storage.Curstorage = 0;
                SendWeb("{\"name\":\"ap\"}");
            }
            if (Trade.Trading)
                WebHandler.Handle("cancel_trade", "", "");
            else if (TradeTarget > 0)
            {
                var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(TradeTarget);
                if (TradeTimer > 0)
                {
                    TradeTimer = 0;
                    user.GetRolePlay().TradeTarget = 0;
                    user.GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"cancel\"}");
                }
                TradeTarget = 0;
                if (Trade.Trading) // cancel trade
                    Trade.Trading = false;
            }
            if (roomUser.TeleportEnabled)
                roomUser.TeleportEnabled = false;
            if (delay > 0)
            {
                roomUser.delayroom = roomid;
                roomUser.delay = delay;
                return;
            }
            if (JobManager.Working && JobManager.Job == 7)
                JobManager.Task2++;
            if (ExCon)
            {
                ExCon = false;
                RPCache(30);
            }
                habbo.PrepareRoom(roomid, "");
        }
        public void Arrest(bool alert)
        {
            prevMotto = habbo.Motto;
            previousLook = habbo.Look;
            roomUser.ApplyEffect(0);
            habbo.Motto = "Inmate [#" + PlusEnvironment.GetRandomNumber(1, 9999) + "]";
            string uniform = "";
            if (habbo.Gender == "f")
                uniform = Figure(habbo.Look) + "ch-635-94.lg-695-94.sh-300-62";
            else
                uniform = Figure(habbo.Look) + "ch-3030-94.lg-695-94.sh-300-62";
            Look(uniform);
            Assault = false;
            roomUser.Assault = 0;
            Cuffed = false;
            ArrestChange();
            if (Room.Id != 9 && !Loggedin)
                RoomForward(20,0);
            else
            {
                Refresh();
                roomUser.ApplyEffect(0);
                Warp(Room.GetGameMap().Model.DoorX, Room.GetGameMap().Model.DoorY);
                roomUser.Stunned = 0;
            }
            if (alert)
            {
                if (Jailed == 1)
                    GetClient().SendWhisper("You were arrested for " + Jailed + "minutes!");
                   //GetClient().SendNotifi("You were arrested for " + Jailed + " minutos!");
           
                else GetClient().SendWhisper("You were arrested for " + Jailed + "minutes!");
            }
            var WL = habbo.GetClientManager().GetWL(habbo.Username, 0);
            if (WL != null)
                habbo.GetClientManager().RemoveWL(WL.ID);
            GetClient().log("" + habbo.Username + " was arrested for " + Jailed + " minutes");
            Timer(Jailed, JailedSec, "jail");
        }
        public void XPSet(int Amount)
        {
            if (habbo.Rank > 1)
                Amount *= 2;
            XP += Amount;
            RPCache(4);
            XPSystem();
            // todo if (Gang > 0)
            //{
            //    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
            //    {
            //        if (_client.GetRolePlay().Gang == Gang)
            //            _client.GetRolePlay().GangManager.UpdateGang(Amount / 2);
            //    }
            //    GangManager.SaveGang();
            //}
            client.SendPacket(new HabboActivityPointNotificationComposer(0, 0));
            SendWeb("{\"name\":\"sidealert\", \"evnt\":\"xp\", \"xpalert\":\"" + Amount + "\"}");
        }
        public void Heal()
        {
            HealthChange(MaxHealth + bonushp);
            HungerChange(100);
            MaxEnergy = 100;
            Dead = false;
            roomUser.Stunned = 0;
            roomUser.Faint = 0;
            roomUser.ApplyEffect(583);
            TempEffect = 15;
        }
        public void PetFollow(bool pickup, int PetId, Room room)
        {
            if (!pickup)
            {
                if (Jailed == 0 && JailedSec == 0 && !Dead)
                    Room.FollowPet(HasPet, roomUser.X, roomUser.Y, GetClient());
            }
            else
            {
                RoomUser Pet = null;
                if (!room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                    return;

                if (Pet.RidingHorse)
                {
                    RoomUser UserRiding = room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseId);
                    if (UserRiding != null)
                    {
                        UserRiding.RidingHorse = false;
                        UserRiding.ApplyEffect(-1);
                        UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                    }
                    else
                        Pet.RidingHorse = false;
                }


                if (Pet.PetData != null)
                {
                    Pet.PetData.RoomId = 0;
                    Pet.PetData.PlacedInRoom = false;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        dbClient.RunQuery("UPDATE `bots` SET `room_id` = '0', `x` = '0', `Y` = '0', `Z` = '0' WHERE `id` = '" + Pet.PetData.PetId + "' LIMIT 1");
                    habbo.GetInventoryComponent().TryAddPet(Pet.PetData);
                    GetClient().SendPacket(new PetInventoryComposer(habbo.GetInventoryComponent().GetPets()));
                    room.GetGameMap().RemoveUserFromMap(Pet, new System.Drawing.Point(Pet.X, Pet.Y));
                    room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);
                    PetPlaced = false;

                }
            }
        }
        public void DmgRespond()
        {
            if (Health <= 25 && Health > 0 && bleeding == 0)
            {
                Say("begins to bleed", true);
                bleeding++;
            }
            else if (Health <= 15 && Health > 0 && bleeding > 0)
            {
                Say("barely manages to keep upright", true);
                bleeding++;
            }
        }
        public void DragonType()
        {
            List<Item> Items = ItemLoader.GetItemsForUser(habbo.Id);
            foreach (Item Item in Items.ToList())
            {
                if (Item.BaseItem == 407)
                    bdrag = true;

            }
        }
        public void DeadSetup(bool SentHosp, bool alert)
        {
            if (SentHosp == false)
            {
                if (habbo.GetClientManager().Paramedics > 0)
                    habbo.GetClientManager().GlobalParamedic(habbo.Username, Room.Name, Room.Id);
                else ParamedicBotDelay = Random.Next(20, 30);
            }
            else if (SentHosp == true)
            {
                if (Inventory.Equip2 != "null")
                    WebHandler.Handle("equip", "", "e2");
                if (Inventory.Equip1 != "null")
                    WebHandler.Handle("equip", "", "e1");
                if (BotEscort > 0)
                    BotEscort = 0;
                HealthChange(1);
                roomUser.Stunned = 0;
                roomUser.ApplyEffect(0);
                if (Hunger <= 10)
                    HungerChange(25);
                MaxEnergy = 100;
                takingtoolong = 0;
                Dead = true;
                prevMotto = habbo.Motto;
                habbo.Motto = "[Dead] " + habbo.Motto;
                RoomForward(2);
                if (alert)
                {
                    int charge = 4;
                    if (habbo.Rank > 1)
                        charge = 2;
                    UpdateCredits(charge, false);
                    GetClient().SendNotification("Você desmaiou no chão e foi transportado para o hospital mais próximo!\r\rvocê foi cobrado " + charge + " moedas para os serviços do SUS!");
                }
            }
        }
        public void Taxi(int RoomId)
        {
            if (Jailed > 0 || JailedSec > 0 || Dead || roomUser.Stunned > 0 || dubCooldown > 0 || GameType != "" || Escorting > 0 || EscortID > 0)
                Responds();
            else
            {
                if (roomUser.TaxiDest > 0)
                    return;
                Room _Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                
                

                if (_Room == null)
                {
                    GetClient().SendWhisper("That location does not exist.");
                    return;
                }
                if (Room.Id == RoomId)
                {
                    GetClient().SendWhisper("You are already at this location.");
                    return;
                }
                if (_Room.Taxi > 0 || Room.Taxi > 0)
                {
                    if (Room.Taxi > 0)
                        GetClient().SendWhisper("Transporte de " + Room.Name + " foi desabilitado!");
                    else GetClient().SendWhisper("Transporte para " + _Room.Name + " foi desabilitado!");
                    return;
                }
                dubCooldown = 7;
                if (JobManager.Job == 7 && JobManager.Working)
                {
                    Say("calls for a taxi to " + _Room.Name + " [" + _Room.RoomId + "]", false);
                    roomUser.ApplyEffect(599);
                }
                else if (JobManager.Job == 1 && JobManager.Working)
                {
                    Say("calls for a taxi to " + _Room.Name + " [" + _Room.RoomId + "]", false);
                    roomUser.ApplyEffect(597);
                }
                else
                {
                    Say("calls for a taxi to " + _Room.Name + " [" + _Room.RoomId + "]", false);
                    roomUser.ApplyEffect(596);
                }
                roomUser.TaxiDest = RoomId;
                if ((JobManager.Job == 7 && JobManager.Working))
                    roomUser.TaxiTime = Random.Next(7, 15);
                else if (habbo.Rank > 1 || (JobManager.Job == 1 && JobManager.Working))
                    roomUser.TaxiTime = Random.Next(15, 30);
                else if (habbo.Rank == 1)
                    roomUser.TaxiTime = Random.Next(30, 60);
                if (roomUser.IsWalking)
                    roomUser.ClearMovement(true);

            }
        }
        public void BedSetup()
        {
            Item bed1 = null;
            Item bed2 = null;
            Item bed3 = null;
            Item bed4 = null;
            Room _Room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(2, out _Room))
                return;
            foreach (Item item in _Room.GetRoomItemHandler().GetFloor.ToList())
                if (item.BaseItem == 10279)
                    bed1 = item;
            foreach (Item item2 in _Room.GetRoomItemHandler().GetFloor.ToList())
                if (item2.BaseItem == 10279 && bed1 != item2)
                    bed2 = item2;
            foreach (Item item3 in _Room.GetRoomItemHandler().GetFloor.ToList())
                if (item3.BaseItem == 10279 && bed1 != item3 && bed2 != item3)
                    bed3 = item3;
            foreach (Item item4 in _Room.GetRoomItemHandler().GetFloor.ToList())
                if (item4.BaseItem == 10279 && bed1 != item4 && bed2 != item4 && bed3 != item4)
                    bed3 = item4;

            int Bed = Random.Next(1, 4);
            if (Bed == 1 && bed1 != null)
            {
                RoomX = bed1.GetX;
                RoomY = bed1.GetY;
                Rotate = bed1.Rotation;
            }
            else if (Bed == 2 && bed2 != null)
            {
                RoomX = bed2.GetX;
                RoomY = bed2.GetY;
                Rotate = bed2.Rotation;
            }
            else if (Bed == 3 && bed3 != null)
            {
                RoomX = bed3.GetX;
                RoomY = bed3.GetY;
                Rotate = bed3.Rotation;
            }
            else if (Bed == 4 && bed4 != null)
            {
                RoomX = bed4.GetX;
                RoomY = bed4.GetY;
                Rotate = bed4.Rotation;
            }
            else
            {
                RoomX = _Room.GetGameMap().Model.DoorX;
                RoomY = _Room.GetGameMap().Model.DoorY;
                Rotate = _Room.GetGameMap().Model.DoorOrientation;
            }
        }
        public void SetMarco()
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `marco` WHERE id = '" + habbo.Id + "' LIMIT 1");
                DataRow dataRow = DB.GetRow();
                marco1_1 = (string)dataRow["key1_1"];
                marco1_2 = (string)dataRow["key1_2"];
                marco2_1 = (string)dataRow["key2_1"];
                marco2_2 = (string)dataRow["key2_2"];
                marco3_1 = (string)dataRow["key3_1"];
                marco3_2 = (string)dataRow["key3_2"];
                marco4_1 = (string)dataRow["key4_1"];
                marco4_2 = (string)dataRow["key4_2"];
                marco5_1 = (string)dataRow["key5_1"];
                marco5_2 = (string)dataRow["key5_2"];
            }
        }
        public void SetGang(bool update)
        {
            this.GangManager = new GangManager(Gang);
            if (GangManager.OwnerId == habbo.Id)
            {
                GangBoss = true;
                GangManager.Id1 = habbo.Id;
            }
            else
            {
                if (GangManager.Id1 == habbo.Id || GangManager.Id2 == habbo.Id ||
                    GangManager.Id3 == habbo.Id || GangManager.Id4 == habbo.Id ||
                    GangManager.Id5 == habbo.Id || GangManager.Id6 == habbo.Id ||
                    GangManager.Id7 == habbo.Id || GangManager.Id8 == habbo.Id ||
                    GangManager.Id9 == habbo.Id || GangManager.Id10 == habbo.Id)
                { }
                else
                {
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                    {
                        if (_client.GetRolePlay().Gang == Gang)
                        {
                            if (GangManager.Id2 == 0)
                                _client.GetRolePlay().GangManager.Id2 = habbo.Id;
                            else if (GangManager.Id3 == 0)
                                _client.GetRolePlay().GangManager.Id3 = habbo.Id;
                            else if (GangManager.Id4 == 0)
                                _client.GetRolePlay().GangManager.Id4 = habbo.Id;
                            else if (GangManager.Id5 == 0)
                                _client.GetRolePlay().GangManager.Id5 = habbo.Id;
                            else if (GangManager.Id6 == 0)
                                _client.GetRolePlay().GangManager.Id6 = habbo.Id;
                            else if (GangManager.Id7 == 0)
                                _client.GetRolePlay().GangManager.Id7 = habbo.Id;
                            else if (GangManager.Id8 == 0)
                                _client.GetRolePlay().GangManager.Id8 = habbo.Id;
                            else if (GangManager.Id9 == 0)
                                _client.GetRolePlay().GangManager.Id9 = habbo.Id;
                            else if (GangManager.Id10 == 0)
                                _client.GetRolePlay().GangManager.Id10 = habbo.Id;
                        }
                    }
                }
            }
            if (GangRank == 0)
            {
                if (GangBoss)
                    GangRank = 1;
                else
                {
                    if (GangManager.Rank5 != "null")
                        GangRank = 5;
                    else if (GangManager.Rank4 != "null")
                        GangRank = 4;
                    else if (GangManager.Rank3 != "null")
                        GangRank = 3;
                    else if (GangManager.Rank2 != "null")
                        GangRank = 2;
                    else GangRank = 1;
                }
                SaveGangStats();
            }
            GangManager.SaveGang();
            habbo.Motto = "[Gang] " + GangManager.Name;
            if (GangRank == 1)
                RankName = GangManager.Rank1;
            else if (GangRank == 2)
                RankName = GangManager.Rank2;
            else if (GangRank == 3)
                RankName = GangManager.Rank3;
            else if (GangRank == 4)
                RankName = GangManager.Rank4;
            else if (GangRank == 4)
                RankName = GangManager.Rank4;
            else if (GangRank == 5)
                RankName = GangManager.Rank5;
            if (update)
                Refresh();

            if (GangManager.Turf1 > 0)
            {
                bonushp += 5;
                MaxStrength += 1;
            }
            if (GangManager.Turf2 > 0)
            {
                bonushp += 5;
                MaxStrength += 1;
            }
            if (GangManager.Turf3 > 0)
            {
                bonushp += 5;
                MaxStrength += 1;
            }
            if (GangManager.Turf4 > 0)
            {
                bonushp += 5;
                MaxStrength += 1;
            }

        }
        public void SetGangRank()
        {
            if (GangRank == 1)
                RankName = GangManager.Rank1;
            else if (GangRank == 2)
                RankName = GangManager.Rank2;
            else if (GangRank == 3)
                RankName = GangManager.Rank3;
            else if (GangRank == 4)
                RankName = GangManager.Rank4;
            else if (GangRank == 4)
                RankName = GangManager.Rank4;
            else if (GangRank == 5)
                RankName = GangManager.Rank5;
        }
        public void GangDelete()
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.RunQuery("DELETE FROM `gang` WHERE id = '" + Gang + "' AND owner = '" + habbo.Id + "' LIMIT 1");
                DB.RunQuery("UPDATE `stats` SET gang = '0' WHERE gang = '" + Gang + "'");
                if (habbo.GetClientManager().SouthTurf == GangManager.Name)
                    habbo.GetClientManager().SouthTurf = null;
                if (habbo.GetClientManager().EastTurf == GangManager.Name)
                    habbo.GetClientManager().EastTurf = null;
                if (habbo.GetClientManager().NorthTurf == GangManager.Name)
                    habbo.GetClientManager().NorthTurf = null;
                if (habbo.GetClientManager().WestTurf == GangManager.Name)
                    habbo.GetClientManager().WestTurf = null;
                //   habbo.GetClientManager().GlobalGang(Gang, null, null, GangManager.Name);
            }
        }
        public void SaveGangStats()
        {
            if (Gang > 0)
            {
                using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    DB.RunQuery("UPDATE stats SET gangkills = '" + kills2 + "', ganghits = '" + punches2 + "',"
                + "gangarrests = '" + arrests2 + "', gangjb = '" + jailbreak + "',"
                + "gangcap = '" + captures + "', gangrank = '" + GangRank + "' WHERE id = '" + habbo.Id + "'");
            }
        }
        public void SaveMarco()
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                DB.RunQuery("UPDATE marco SET key1_1 = '" + marco1_1 + "', key1_2 = '" + marco1_2 + "',"
                    + "key2_1 = '" + marco2_1 + "', key2_2 = '" + marco2_2 + "',"
                     + "key3_1 = '" + marco3_1 + "', key3_2 = '" + marco3_2 + "',"
                     + "key4_1 = '" + marco4_1 + "', key4_2 = '" + marco4_2 + "',"
                    + "key5_1 = '" + marco5_1 + "', key5_2 = '" + marco5_2 + "' WHERE id = '" + habbo.Id + "'");
        }
        public void CreateGang(string name)
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT name FROM `gang` LIMIT 1");
                string gangname = DB.GetString();
                if (gangname == name)
                    GetClient().SendWhisper("This name already exists!");
                else
                {
                    DB.RunQuery("INSERT INTO `gang` (name, owner) VALUES ('" + name + "', '" + habbo.Id + "')");
                    GetClient().SendWhisper("Gang: " + name + " successfully created!");
                    DB.SetQuery("SELECT id FROM `gang` WHERE owner = '" + habbo.Id + "' LIMIT 1");
                    Gang = DB.GetInteger();
                    SetGang(true);
                    UpdateCredits(75, false);
                    WebHandler.Handle("gang", "", "");
                    SendWeb("{\"name\":\"gangalert\"}");
                }
            }
        }
        public void Startwork()
        {
            if (JobManager.Sendhome > 0)
            {
                int senthome = JobManager.Sendhome / 60;
                GetClient().SendWhisper("You are sent home, you still have " + senthome + " minutes remaining!");
                return;
            }
            if (Jailed > 0 || JailedSec > 0 || Dead || roomUser.Stunned > 0 || subCooldown > 0 || GameType != "" || Escorting > 0)
                Responds();
            else if (JobManager.Job == 0)
                GetClient().SendWhisper("You do not have a job!");
            else if (JobManager.Working == true)
                GetClient().SendWhisper("You are already working!");
            else
            {
                if (JobManager.JobRoom == Room.Id || JobManager.Job == 1 && habbo.Rank > 4)
                {
                    if (JobManager.Job == 1 && stungun == "null")
                    {
                        SendWeb("{\"name\":\"selectstun\"}");
                        client.SendWhisper("You must select your stungun before you can begin your duty!");
                        return;
                    }
                    if (Inventory.IsInventoryFull("stun") && JobManager.Job == 1)
                    {
                        client.SendWhisper("You need to clear a slot in your inventory to equip your stungun!");
                        return;
                    }
                    if (Inventory.Equip1 != "null" || Inventory.Equip2 != "null")
                    {
                        if (Inventory.Equip2.Contains("kevlar"))
                            WebHandler.Handle("equip", "", "e2");
                        if (Inventory.Equip1 != "null")
                            WebHandler.Handle("equip", "", "e1");
                    }
                    subCooldown = 5;
                    prevMotto = habbo.Motto;
                    previousLook = habbo.Look;
                    habbo.Motto = "[Working] " + JobManager.JobMotto;
                    JobManager.Working = true;
                    Look(Figure(habbo.Look) + JobManager.JobLook);
                    Say("starts working", true, 4);
                    Refresh();
                    #region worker logs
                    if (JobManager.Job == 1)
                        habbo.GetClientManager().NYPDactive++;
                    if (JobManager.Job == 2)
                        habbo.GetClientManager().HospWorkers++;
                    if (JobManager.Job == 7)
                        habbo.GetClientManager().Paramedics++;
                    #endregion
                    //if (JobManager.Job == 1)
                    //{
                    //    SendWeb("{\"name\":\"copbadge\", \"copbadge\":\"true\"}");
                    //    if (Trade.Trading)
                    //        client.SendWhisper("Você precisa parar de negociar para receber sua arma de choque");
                    //    else Inventory.Additem(stungun);
                    //}
                    if (JobManager.Jobmin <= 0 && JobManager.Jobsec <= 0)
                        JobManager.Jobmin = 10;
                    Timer(JobManager.Jobmin, JobManager.Jobsec, "job");
                }
                else
                    GetClient().SendWhisper("You must be at your work place to begin working!");
            }
        }
        public void Stopwork(bool fired)
        {
            bool sleep = false;
            if (!fired && (Jailed > 0 || JailedSec > 0 || Dead))
            {
                Responds();
                return;
            }
            if (JobManager.Job == 0)
            {
                GetClient().SendWhisper("You do not have a job!");
                return;
            }
            if (!JobManager.Working && !fired)
            {
                GetClient().SendWhisper("You are not working!");
                return;
            }

            if (JobManager.Working)
            {
                JobManager.Working = false;
                habbo.Motto = prevMotto;
                if (!fired && AutoLogout == 0)
                {
                    if (roomUser.IsAsleep)
                    {
                        Say("stops working", true,4);
                        sleep = true;
                    }
                    else Say("stops working", true, 4);
                }
                #region worker logs
                if (JobManager.Job == 1)
                    habbo.GetClientManager().NYPDactive--;
                if (JobManager.Job == 2)
                    habbo.GetClientManager().HospWorkers--;
                if (JobManager.Job == 7)
                    habbo.GetClientManager().Paramedics--;
                #endregion
                SendWeb("{\"name\":\"copbadge\", \"copbadge\":\"false\"}");
                if (Inventory.Equip2.Contains("kevlar"))
                    WebHandler.Handle("equip", "", "e2");
                if (Inventory.Equip1 != "null")
                    WebHandler.Handle("equip", "", "e1");
                Inventory.Additem(stungun, true);
                Look(previousLook);
                Refresh();
            }
            if (fired)
            {
                JobManager.LeaveJob();
                RPCache(31);
            }
            if (EscortID > 0 || Escorting > 0)
                EndEscort();
            Timer(JobManager.Jobmin, JobManager.Jobsec, "stopwork");
            if (sleep)
            {
               // habbo.CurrentRoom.SendPacket(new SleepComposer(roomUser, true));
        
                roomUser.IsAsleep = true;
            }
            if (roomUser.CurrentEffect > 0)
                roomUser.ApplyEffect(0);

        }
        public void UpdateCredits(int amount, bool add)
        {
            if (add)
                habbo.Credits += amount;
            else habbo.Credits -= amount;
            GetClient().SendPacket(new CreditBalanceComposer(habbo.Credits));
            var money = string.Format("{0:n0}", habbo.Credits);
            SendWeb("{\"name\":\"updatemoney\", \"money\":\"" + money + "\"}");
            RPCache(29);

        }
        public void Refresh()
        {
            if (habbo.Motto.Contains("Dead") || habbo.Motto.Contains("Inmate"))
                habbo.Motto = prevMotto;
            Room.SendPacket(new UserChangeComposer(roomUser, false));
            GetClient().SendPacket(new UserChangeComposer(roomUser, true));
        }
        public void Faint()
        {
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
        public void Say(string message, bool shout, int color = 0)
        {
           // shout = true;
            if (AutoLogout > 0)
                return;
            if (roomUser.IsAsleep)
                roomUser.UnIdle();
           
            message = message + "*";
            prevName = habbo.Username;
            habbo.Username = "*<font color='" + GetClient().GetRolePlay().Color + "'>" + habbo.Username + "</font>";
            Room.SendPacket(new UserNameChangeComposer(Room.Id, roomUser.VirtualId, habbo.Username));
            roomUser.OnChat(color, message, true);
            habbo.Username = prevName;
            Room.SendPacket(new UserNameChangeComposer(Room.Id, roomUser.VirtualId, habbo.Username));

        }
        public void GameSet(string Game)
        {
            if (Inventory.Equip2.Contains("kevlar"))
                WebHandler.Handle("equip", "", "e2");
            if (Inventory.Equip1 != "null")
                WebHandler.Handle("equip", "", "e1");

            if (Game == "Brawl")
            {
                if (GameType != Game)
                {
                    GameType = Game;
                    prevMotto = habbo.Motto;
                    habbo.Motto = "[Brawl] " + habbo.Motto;
                    Room.Brawl += 1;
                    Refresh();
                    roomUser.ApplyEffect(4);
                    TempEffect = 10;
                }
            }
            else
                if (Game == "Mafia")
            {
                if (GameType != Game)
                {
                    GameType = Game;
                    prevMotto = habbo.Motto;
                    previousLook = habbo.Look;
                    if (Team == "Blue")
                    {
                        string uniform = "";
                        habbo.Motto = "[Mafia] Blue Team ";
                        if (habbo.Gender == "f")
                            uniform = Figure(habbo.Look) + "sh-290-82.lg-275-82.ch-635-82.ha-1026-62";
                        else
                            uniform = Figure(habbo.Look) + "sh-290-82.lg-275-82.ch-215-82.ha-1026-62";
                        Look(uniform);
                        prevColor = Color;
                        Color = "#17bbf6";
                    }
                    else if (Team == "Green")
                    {
                        string uniform = "";
                        habbo.Motto = "[Mafia] Green Team ";
                        if (habbo.Gender == "f")
                            uniform = Figure(habbo.Look) + "sh-290-84.lg-275-84.ch-635-84.ha-1026-62";
                        else
                            uniform = Figure(habbo.Look) + "sh-290-84.lg-275-84.ch-215-84.ha-1026-62";
                        Look(uniform);
                        prevColor = Color;
                        Color = "#56e807";
                    }
                    roomUser.ApplyEffect(4);
                    TempEffect = 10;
                    Refresh();
                    roomUser.GoalX = 0;
                    roomUser.GoalY = 0;
                    roomUser.PathRecalcNeeded = true;
                    if (Team == "Green")
                        Warp(3, 9);
                    else if (Team == "Blue")
                        Warp(3, 21);
                    roomUser.GetClient().SendWhisper("You have joined the " + Team + " team! New to Mafia Wars? Type 'gamehelp'.");
                    if (Health <= 20)
                        HealthChange(25);
                    SetRot(2, false);

                }
            }
            else if (Game == "CW")
            {
                if (GameType != Game)
                {
                    GameType = Game;
                    prevMotto = habbo.Motto;
                    previousLook = habbo.Look;
                    if (Team == "Blue")
                    {

                    }
                    else if (Team == "Green")
                    {

                    }
                    else if (Team == "Pink")
                    {

                    }
                    else if (Team == "Yellow")
                    {

                    }
                }
            }
        }
        public void LeaveGame(bool updatelook, bool kick)
        {
            habbo.Motto = prevMotto;
            Look(previousLook);
            if (GameType == "Brawl")
                Room.Brawl--;
            if (GameType == "Mafia")
            {
                if (Room.Id == 23)
                {
                    TeamManager t = Room.GetTeamManagerForBanzai();
                    if (Team == "Blue")
                    {
                        Room.BlueTeam--;
                    }
                    else
                    {
                        Room.GreenTeam--;
                    }
                    t.OnUserLeave(roomUser);
                }
            }
            Team = "";
            GameType = "";
            Color = prevColor;
            if (updatelook)
                Refresh();
            if (kick)
                RoomForward(21);
        }
        public bool CheckName(string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z0-9-$ ]+$"))
                return false;
            else return true;

        }
        public void Leavegroup()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = '" + habbo.Id + "' LIMIT 1");
            habbo.GetStats().FavouriteGroupId = 0;
            Plus.HabboHotel.Groups.Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(99, out Group))
                return;
            Room.SendPacket(new HabboGroupBadgesComposer(Group));
            Room.SendPacket(new UpdateFavouriteGroupComposer(null, roomUser.VirtualId));
        }
        public bool CheckDiag(int GetX, int GetY, int roomUserX, int roomUserY)
        {
            if (roomUserX + 1 == GetX && roomUserY - 1 == GetY || roomUserX + 1 == GetX && roomUserY + 1 == GetY || roomUserX - 1 == GetX && roomUserY - 1 == GetY || roomUserX - 1 == GetX && roomUserY + 1 == GetY)
                return true;
            else return false;
        }
        public void Warp(int X, int Y)
        {
            Room.GetGameMap().UpdateUserMovement(new Point(roomUser.X, roomUser.Y), new Point(X, Y), roomUser);
            roomUser.X = X;
            roomUser.Y = Y;
            roomUser.UpdateNeeded = true;
            roomUser.ClearMovement(true);
        }
        public void UpdateEnergy(int value, int count = 0)
        {
            if (Energy - value < 0)
                EnergyChange(0);
            else EnergyChange(Energy - value);
            if (WeedTime > 0)
                roomUser.EnergyTime = 2;
            else roomUser.EnergyTime = 5;
            hitcount += count;
        }
        public void SetRot(int rot, bool headonly = false)
        {
            if (headonly == false)
                roomUser.RotBody = rot;
            roomUser.RotHead = rot;
            roomUser.UpdateNeeded = true;
        }
        public void SaveLook()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE `id` = '" + habbo.Id + "' LIMIT 1");
                dbClient.AddParameter("look", habbo.Look);
                dbClient.AddParameter("gender", habbo.Gender);
                dbClient.RunQuery();
            }
        }
        public void Cuff()
        {
            int stun = roomUser.Stunned;
            if (Inventory.Equip1 != "null" || Inventory.Equip2 != "null")
            {
                roomUser.Stunned = 0;
                if (Inventory.Equip2.Contains("kevlar"))
                    WebHandler.Handle("equip", "", "e2");
                if (Inventory.Equip1 != "null")
                    WebHandler.Handle("equip", "", "e1");
            }
            roomUser.Stunned = stun;
            Cuffed = true;
        }
        public void EndEscort()
        {
            if (EscortID > 0)
            {
                var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(EscortID);
                user.GetRolePlay().Escorting = 0;
                EscortID = 0;

            }
            else if (Escorting > 0)
            {
                var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Escorting);
                user.GetRolePlay().EscortID = 0;
                Escorting = 0;
            }
        }
        public void RemoveCloth(string RemovePart, bool savelook = false)
        {
            string FigurePart = Regex.Split(habbo.Look, RemovePart)[1];
            if (RemovePart == "cc")
            {
                FigurePart = RemovePart + FigurePart.Split('.')[0];
            }
            else FigurePart = RemovePart + FigurePart.Split('.')[0] + ".";
            Look(habbo.Look.Replace(FigurePart, ""));
            Refresh();
            if (savelook)
                SaveLook();
            if (RemovePart == "cc" && habbo.Look.Contains("cc"))
                RemoveCloth("cc", false);
        }
        public void GiveItem(RoomUser Target)
        {
            if (Target != null)
            {
                int dis = Math.Abs(Target.X - roomUser.X) + Math.Abs(Target.Y - roomUser.Y);
                if (dis <= 2 || CheckDiag(Target.X, Target.Y, roomUser.X, roomUser.Y))
                {
                    if (roomUser.CarryItemId > 0)
                    {
                        if (roomUser.CarryItemId == 22)
                            return;
                        if (((JobManager.Job == 6 || JobManager.Job == 2 || JobManager.Job == 5) && JobManager.Working) || onduty)
                        {
                            if (InteractID > 0 || Target.GetClient().GetRolePlay().InteractID > 0)
                                GetClient().SendWhisper("That action cannot be performed at this time!");
                            else
                            {
                                if ((JobManager.Job == 6 || onduty) && (roomUser.CarryItemId == 33 || roomUser.CarryItemId == 19 || roomUser.CarryItemId == 9 || roomUser.CarryItemId == 8))
                                {
                                    Say("offers " + Target.GetUsername() + " a bubble juice for $3", false);
                                    Target.GetClient().GetRolePlay().OfferAmount = 3;
                                    Target.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + habbo.Username + "</b> is offering you a <b>Bubble Juice</b> for <b>3</b> dollars!\"}");
                                }
                                else if ((JobManager.Job == 5 || onduty) && roomUser.CarryItemId == 52)
                                {
                                    Say("offers " + Target.GetUsername() + " some cheetos for $15", false);
                                    Target.GetClient().GetRolePlay().OfferAmount = 15;
                                    Target.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + habbo.Username + "</b> esta oferecendo a você um <b>Cheetos Apimentado</b> por <b>7</b> reais!\"}");
                                }
                                else if ((JobManager.Job == 2 || onduty) && roomUser.CarryItemId == 1013)
                                {
                                    Say("offers " + Target.GetUsername() + "a medkit for $15", false);
                                    Target.GetClient().GetRolePlay().OfferAmount = 15;
                                    Target.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + habbo.Username + "</b> esta oferecendo a você um <b>kit médico </b> por <b>15</b> reais!\"}");
                                }
                                else
                                {
                                    Target.CarryItem(roomUser.CarryItemId);
                                    Target.DanceId = 0;
                                    Say("hands " + Target.GetUsername() + " their item", false);
                                    if ((roomUser.CarryItemId == 52 || roomUser.CarryItemId == 3 ||
                                      roomUser.CarryItemId == 1008 || roomUser.CarryItemId == 1013) && Inventory.Currslot1 != "null")
                                    {
                                        string currslot = Inventory.Currslot1;
                                        if (Inventory.IsSlotEmpty(currslot))
                                        {
                                            WebHandler.Handle("equip", "", "e1");
                                            Inventory.Additem(currslot, true);
                                        }
                                        else
                                        {
                                            Inventory.Additem(currslot, true);
                                            WebHandler.Handle("equip", "", "e1");
                                        }
                                    }
                                    else roomUser.CarryItem(0);
                                    return;
                                }
                                InteractID = Target.HabboId;
                                OfferTimer = 20;
                                Target.GetClient().GetRolePlay().InteractID = habbo.Id;
                                Target.GetClient().GetRolePlay().AcceptOffer = "drink";
                            }
                        }
                        else
                        {
                            Target.CarryItem(roomUser.CarryItemId);
                            Target.DanceId = 0;
                            Say("hands " + Target.GetUsername() + " their item", false);
                            if ((roomUser.CarryItemId == 52 || roomUser.CarryItemId == 3 ||
                              roomUser.CarryItemId == 1008 || roomUser.CarryItemId == 1013) && Inventory.Currslot1 != "null")
                            {
                                string currslot = Inventory.Currslot1;
                                if (Inventory.IsSlotEmpty(currslot))
                                {
                                    WebHandler.Handle("equip", "", "e1");
                                    Inventory.Additem(currslot, true);
                                }
                                else
                                {
                                    Inventory.Additem(currslot, true);
                                    WebHandler.Handle("equip", "", "e1");
                                }
                            }
                            else roomUser.CarryItem(0);
                            return;
                        }
                    }
                }
                else client.SendWhisper("You're too far away to perform this action!");
            }
        }
        public void Ticket(RoomUser Target, int amount)
        {
            if (Target != null)
            {
                int dis = Math.Abs(Target.X - roomUser.X) + Math.Abs(Target.Y - roomUser.Y);
                if (dis <= 1 || CheckDiag(Target.X, Target.Y, roomUser.X, roomUser.Y))
                {
                    if ((JobManager.Job == 1 && JobManager.Working) || onduty)
                    {
                        if (!Target.GetClient().GetRolePlay().Cuffed)
                        {
                            client.SendWhisper("This user is not cuffed!");
                            return;
                        }
                        if (InteractID > 0 || Target.GetClient().GetRolePlay().InteractID > 0)
                            GetClient().SendWhisper("That action cannot be performed at this time!");
                        else
                        {
                            if (amount > 100)
                            {
                                client.SendWhisper("You cannot exceed a ticket amount of $100!");
                                return;
                            }
                            if (amount < 8)
                            {
                                client.SendWhisper("You must offer a mimimum of a $8 ticket!");
                                return;
                            }
                            InteractID = Target.HabboId;
                            OfferTimer = 5;
                            Target.GetClient().GetRolePlay().InteractID = habbo.Id;
                            Target.GetClient().GetRolePlay().OfferAmount = amount;
                            Target.GetClient().GetRolePlay().AcceptOffer = "ticket";
                            Target.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + habbo.Username + "</b> has issued you a <b>ticket</b> of <b>" + amount + "</b> dollars!\"}");
                            Say("issues " + Target.GetUsername() + " a ticket of " + amount + " dollars", false);

                        }
                    }
                }
                else client.SendWhisper("You're too far away to perform this action!");
            }
        }
        public void DeathChange()
        {
            Deaths++;
            string badge_desc = "";
            string badge_name = "";
            if (Deaths >= 1000)
            {
                badge_desc = "Grim Reaper X";
                badge_name = "D10";
                habbo.GetBadgeComponent().RemoveBadge("D9", client);
            }
            else if (Deaths >= 750 && Deaths < 1000)
            {
                badge_desc = "Grim Reaper IX";
                badge_name = "D9";
                habbo.GetBadgeComponent().RemoveBadge("D8", client);
            }
            else if (Deaths >= 600 && Deaths < 750)
            {
                badge_desc = "Grim Reaper VIII";
                badge_name = "D8";
                habbo.GetBadgeComponent().RemoveBadge("D7", client);
            }
            else if (Deaths >= 500 && Deaths < 600)
            {
                badge_desc = "Grim Reaper VII";
                badge_name = "D7";
                habbo.GetBadgeComponent().RemoveBadge("D6", client);
            }
            else if (Deaths >= 350 && Deaths < 500)
            {
                badge_desc = "Grim Reaper VI";
                badge_name = "D6";
                habbo.GetBadgeComponent().RemoveBadge("D5", client);
            }
            else if (Deaths >= 250 && Deaths < 350)
            {
                badge_desc = "Grim Reaper V";
                badge_name = "D5";
                habbo.GetBadgeComponent().RemoveBadge("D4", client);
            }
            else if (Deaths >= 100 && Deaths < 250)
            {
                badge_desc = "Grim Reaper IV";
                badge_name = "D4";
                habbo.GetBadgeComponent().RemoveBadge("D3", client);
            }
            else if (Deaths >= 75 && Deaths < 100)
            {
                badge_desc = "Grim Reaper III";
                badge_name = "D3";
                habbo.GetBadgeComponent().RemoveBadge("D2", client);
            }
            else if (Deaths >= 25 && Deaths < 75)
            {
                badge_desc = "Grim Reaper II";
                badge_name = "D2";
                habbo.GetBadgeComponent().RemoveBadge("D1", client);
            }
            else if (Deaths >= 10 && Deaths < 25)
            {
                badge_desc = "Grim Reaper I";
                badge_name = "D1";
            }
            if (badge_desc != "" && !habbo.GetBadgeComponent().HasBadge(badge_name))
            {
                SendWeb("{\"name\":\"achievement\", \"badge_name\":\"" + badge_name + "\", \"badge_desc\":\"You unlocked the Achievement " + badge_desc + "\", \"badge_type\":\"deaths\"}");
                habbo.GetBadgeComponent().GiveBadge(badge_name, true, client);
                achievetimer = 15;
                client.SendPacket(new HabboActivityPointNotificationComposer(0, 0));
            }
        }
        public void ArrestChange()
        {
            Arrests++;
            if (Gang > 0)
            {
                arrests2++;
                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (_client.GetRolePlay().Gang == Gang)
                        _client.GetRolePlay().GangManager.Arrests++;
                    GangManager.SaveGang();
                }
            }
            string badge_desc = "";
            string badge_name = "";
            if (Arrests >= 1000)
            {
                badge_desc = "Jail House X";
                badge_name = "A10";
                habbo.GetBadgeComponent().RemoveBadge("A9", client);
            }
            else if (Arrests >= 750 && Arrests < 1000)
            {
                badge_desc = "Jail House IX";
                badge_name = "A9";
                habbo.GetBadgeComponent().RemoveBadge("A8", client);
            }
            else if (Arrests >= 600 && Arrests < 750)
            {
                badge_desc = "Jail House VIII";
                badge_name = "A8";
                habbo.GetBadgeComponent().RemoveBadge("A7", client);
            }
            else if (Arrests >= 500 && Arrests < 600)
            {
                badge_desc = "Jail House VII";
                badge_name = "A7";
                habbo.GetBadgeComponent().RemoveBadge("A6", client);
            }
            else if (Arrests >= 350 && Arrests < 500)
            {
                badge_desc = "Jail House VI";
                badge_name = "A6";
                habbo.GetBadgeComponent().RemoveBadge("A5", client);
            }
            else if (Arrests >= 250 && Arrests < 350)
            {
                badge_desc = "Jail House V";
                badge_name = "A5";
                habbo.GetBadgeComponent().RemoveBadge("A4", client);
            }
            else if (Arrests >= 100 && Arrests < 250)
            {
                badge_desc = "Jail House IV";
                badge_name = "A4";
                habbo.GetBadgeComponent().RemoveBadge("A3", client);
            }
            else if (Arrests >= 75 && Arrests < 100)
            {
                badge_desc = "Jail House III";
                badge_name = "A3";
                habbo.GetBadgeComponent().RemoveBadge("A2", client);
            }
            else if (Arrests >= 25 && Arrests < 75)
            {
                badge_desc = "Jail House II";
                badge_name = "A2";
                habbo.GetBadgeComponent().RemoveBadge("A1", client);
            }
            else if (Arrests >= 5 && Arrests < 25)
            {
                badge_desc = "Jail House I";
                badge_name = "A1";
            }
            if (badge_desc != "" && !habbo.GetBadgeComponent().HasBadge(badge_name))
            {
                SendWeb("{\"name\":\"achievement\", \"badge_name\":\"" + badge_name + "\", \"badge_desc\":\"You unlocked the Achievement " + badge_desc + "\",  \"badge_type\":\"arrests\"}");
                habbo.GetBadgeComponent().GiveBadge(badge_name, true, client);
                achievetimer = 15;
                client.SendPacket(new HabboActivityPointNotificationComposer(0, 0));
            }
        }
        public void KillChange()
        {
            Kills++;
            if (Gang > 0)
            {
                kills2++;
                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (_client.GetRolePlay().Gang == Gang)
                        _client.GetRolePlay().GangManager.Kills++;
                    GangManager.SaveGang();
                }
            }
            string badge_desc = "";
            string badge_name = "";
            if (Kills >= 1000)
            {
                badge_desc = "K.O King X";
                badge_name = "K10";
                habbo.GetBadgeComponent().RemoveBadge("K9", client);
            }
            else if (Kills >= 750 && Kills < 1000)
            {
                badge_desc = "K.O King IX";
                badge_name = "K9";
                habbo.GetBadgeComponent().RemoveBadge("K8", client);
            }
            else if (Kills >= 600 && Kills < 750)
            {
                badge_desc = "K.O King VIII";
                badge_name = "K8";
                habbo.GetBadgeComponent().RemoveBadge("K7", client);
            }
            else if (Kills >= 500 && Kills < 600)
            {
                badge_desc = "K.O King VII";
                badge_name = "K7";
                habbo.GetBadgeComponent().RemoveBadge("K6", client);
            }
            else if (Kills >= 350 && Kills < 500)
            {
                badge_desc = "K.O King VI";
                badge_name = "K6";
                habbo.GetBadgeComponent().RemoveBadge("K5", client);
            }
            else if (Kills >= 250 && Kills < 350)
            {
                badge_desc = "K.O King V";
                badge_name = "K5";
                habbo.GetBadgeComponent().RemoveBadge("K4", client);
            }
            else if (Kills >= 100 && Kills < 250)
            {
                badge_desc = "K.O King IV";
                badge_name = "K4";
                habbo.GetBadgeComponent().RemoveBadge("K3", client);
            }
            else if (Kills >= 75 && Kills < 100)
            {
                badge_desc = "K.O King III";
                badge_name = "K3";
                habbo.GetBadgeComponent().RemoveBadge("K2", client);
            }
            else if (Kills >= 25 && Kills < 75)
            {
                badge_desc = "K.O King II";
                badge_name = "K2";
                habbo.GetBadgeComponent().RemoveBadge("K1", client);
            }
            else if (Kills >= 5 && Kills < 25)
            {
                badge_desc = "K.O King I";
                badge_name = "K1";
            }
            if (badge_desc != "" && !habbo.GetBadgeComponent().HasBadge(badge_name))
            {
                SendWeb("{\"name\":\"achievement\", \"badge_name\":\"" + badge_name + "\", \"badge_desc\":\"You unlocked the Achievement " + badge_desc + "\", \"badge_type\":\"kills\"}");
                habbo.GetBadgeComponent().GiveBadge(badge_name, true, client);
                achievetimer = 15;
                client.SendPacket(new HabboActivityPointNotificationComposer(0, 0));
            }
        }
        public void PunchChange()
        {
            Punches++;
            if (Gang > 0)
            {
                punches2++;
                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (_client.GetRolePlay().Gang == Gang)
                        _client.GetRolePlay().GangManager.Hits++;
                    GangManager.SaveGang();
                }
            }
            string badge_desc = "";
            string badge_name = "";
            if (Punches >= 10000)
            {
                badge_desc = "Street Fighter X";
                badge_name = "P10";
                habbo.GetBadgeComponent().RemoveBadge("P9", client);
            }
            else if (Punches >= 8000 && Punches < 10000)
            {
                badge_desc = "Street Fighter IX";
                badge_name = "P9";
                habbo.GetBadgeComponent().RemoveBadge("P8", client);
            }
            else if (Punches >= 5000 && Punches < 8000)
            {
                badge_desc = "Street Fighter VIII";
                badge_name = "P8";
                habbo.GetBadgeComponent().RemoveBadge("P7", client);
            }
            else if (Punches >= 2500 && Punches < 5000)
            {
                badge_desc = "Street Fighter VII";
                badge_name = "P7";
                habbo.GetBadgeComponent().RemoveBadge("P6", client);
            }
            else if (Punches >= 1000 && Punches < 2500)
            {
                badge_desc = "Street Fighter VI";
                badge_name = "P6";
                habbo.GetBadgeComponent().RemoveBadge("P5", client);
            }
            else if (Punches >= 750 && Punches < 1000)
            {
                badge_desc = "Street Fighter V";
                badge_name = "P5";
                habbo.GetBadgeComponent().RemoveBadge("P4", client);
            }
            else if (Punches >= 500 && Punches < 750)
            {
                badge_desc = "Street Fighter IV";
                badge_name = "P4";
                habbo.GetBadgeComponent().RemoveBadge("P3", client);
            }
            else if (Punches >= 250 && Punches < 500)
            {
                badge_desc = "Street Fighter III";
                badge_name = "P3";
                habbo.GetBadgeComponent().RemoveBadge("P2", client);
            }
            else if (Punches >= 100 && Punches < 250)
            {
                badge_desc = "Street Fighter II";
                badge_name = "P2";
                habbo.GetBadgeComponent().RemoveBadge("P1", client);
            }
            else if (Punches >= 50 && Punches < 100)
            {
                badge_desc = "Street Fighter I";
                badge_name = "P1";
            }
            if (badge_desc != "" && !habbo.GetBadgeComponent().HasBadge(badge_name))
            {
                SendWeb("{\"name\":\"achievement\", \"badge_name\":\"" + badge_name + "\", \"badge_desc\":\"You unlocked the Achievement " + badge_desc + "\", \"badge_type\":\"hits\"}");
                habbo.GetBadgeComponent().GiveBadge(badge_name, true, client);
                achievetimer = 15;
                client.SendPacket(new HabboActivityPointNotificationComposer(0, 0));
            }
        }
        public void LeaveGang(bool show = false)
        {
            if (show)
            {
                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (_client.GetRolePlay().Gang == Gang)
                        _client.GetRolePlay().GangManager.Clear(habbo.Id);
                }
            }

            Gang = 0;
            kills2 = 0;
            punches2 = 0;
            arrests2 = 0;
            jailbreak = 0;
            captures = 0;
            GangRank = 0;
            if (JobManager.Working || Jailed > 0 || JailedSec > 0 || Dead || GameType != "")
                prevMotto = "Civil";
            else
            {
                habbo.Motto = "Civil";
                Refresh();
            }
            SendWeb("{\"name\":\"ganghide\"}");
            GangBoss = false;
            SaveGangStats();
        }
        public void Timer(int minutes, int seconds, string Event)
        {
            SendWeb("{\"name\":\"timer\", \"time\":\"" + minutes + "\", \"seconds\":\"" + seconds + "\", \"timerevent\":\"" + Event + "\"}");
        }
        public string JobTask(int id)
        {
            if (JobManager.Job == 1)
            {
                if (id == 1)
                    return "Detained <b>" + JobManager.Task1 + "</b> Criminals";
                else if (id == 2)
                    return "Fined <b>" + JobManager.Task2 + "</b> Civilians";
                else if (id == 3)
                    return "Charged <b>" + JobManager.Task3 + "</b> Criminals";
            }
            else if (JobManager.Job == 2)
            {
                if (id == 1)
                    return "Healed <b>" + JobManager.Task1 + "</b> Civilians";
                else if (id == 2)
                    return "Sold <b>" + JobManager.Task2 + "</b> Medic kits";
                else if (id == 3)
                    return "Performed <b>" + JobManager.Task3 + "</b> Surgery"; // code surgery for hosp workers
            }
            else if (JobManager.Job == 3)
            {
                if (id == 1)
                    return "Set up <b>" + JobManager.Task1 + "</b> Accounts";
                else if (id == 2)
                    return "Initiated <b>" + JobManager.Task2 + "</b> Deposits";
                else if (id == 3)
                    return "Initiated <b>" + JobManager.Task3 + "</b> Withdraws";
            }
            else if (JobManager.Job == 5)
            {
                if (id == 1)
                    return "Served " + JobManager.Task1 + " Customers";//
                else if (id == 2)
                    return "Sold " + JobManager.Task2 + " Foods";//
                else if (id == 3)
                    return "Work 2 consecutive hours";
            }
            else if (JobManager.Job == 6)
            {
                if (id == 1)
                    return "Served " + JobManager.Task1 + " Customers";//
                else if (id == 2)
                    return "Sold " + JobManager.Task2 + " Foods";//
                else if (id == 3)
                    return "Work 2 consecutive hours";
            }
            else if (JobManager.Job == 7)
            {
                if (id == 1)
                    return "Discharged 25 Civilians";
                else if (id == 2)
                    return "Traveled 50 rooms";
                else if (id == 3)
                    return "Worked 2 consecutive hours";
            }
            else if (JobManager.Job == 8)
            {
                if (id == 1)
                    return "Served " + JobManager.Task1 + " Customers";//
                else if (id == 2)
                    return "Sold " + JobManager.Task2 + " foods";//
                else if (id == 3)
                    return "Work 2 consecutive hours";
            }
            return "";
        }
        public string datecheck(int month)
        {
            if (month == 1)
                return "Jan";
            else if (month == 2)
                return "Feb";
            else if (month == 3)
                return "Mar";
            else if (month == 4)
                return "Apr";
            else if (month == 5)
                return "May";
            else if (month == 6)
                return "Jun";
            else if (month == 7)
                return "Jul";
            else if (month == 8)
                return "Aug";
            else if (month == 9)
                return "Sep";
            else if (month == 10)
                return "Oct";
            else if (month == 11)
                return "Nov";
            else if (month == 12)
                return "Dec";
            else return "";
        }
        public void GetTimer(int time)
        {
            string time2 = "";
            time /= 60 / 60;
            if (time / 60 <= 0)
            {
                if (time == 1)
                    time2 = " segundo atrás";
                else time2 = " segundos atrás";
            }
            else
            {
                time /= 60;
                if (time / 60 <= 0)
                {
                    if (time == 1)
                        time2 = " minuto atrás";
                    else time2 = " minutos atrás";
                }
                else
                {
                    time /= 60;
                    if (time / 60 <= 0)
                    {
                        if (time == 1)
                            time2 = " hora atrás";
                        else time2 = " horas atrás";
                    }
                    else
                    {
                        time /= 60;
                        if (time / 60 <= 0)
                        {
                            if (time == 1)
                                time2 = " dia atrás ";
                            else time2 = " dias atrás ";
                        }
                        else
                        {
                            time /= 60;
                            if (time / 60 <= 0)
                            {
                                if (time == 1)
                                    time2 = " mês atrás";
                                else time2 = " meses atrás";
                            }
                            else
                            {
                                time /= 60;
                                if (time / 60 <= 0)
                                {
                                    if (time == 1)
                                        time2 = " ano atrás";
                                    else time2 = " anos atrás";
                                }
                            }
                        }
                    }
                }
            }
            onlinetime = Convert.ToString(time) + time2;
        }
        public void HealthChange(int curhp, int dmg = 0)
        {
            if (Inventory.Equip2.Contains("kevlar"))
            {
                if (dmg > 0)
                {
                    var hp = Inventory.GetHP(Inventory.Currslot2);                   
                    Inventory.ItemHealth("false", dmg);
                    if (hp < dmg)
                        dmg = dmg - hp;
                    else return;
                }
            }
            if (curhp < 1)
            {
                if (Inventory.Equip2.Contains("kevlar"))
                    WebHandler.Handle("equip", "", "e2");
                if (Inventory.Equip1 != "null")
                    WebHandler.Handle("equip", "", "e1");
                Health = 0;
            }
            else Health = curhp;
            SendWeb("{\"name\":\"health\", \"health\":\"" + Health + "\", \"maxhealth\":\"" + (MaxHealth + bonushp) + "\"}");
            if (lockedon > 0)
            {
                foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (User.GetRolePlay().lockID == habbo.Id)
                    {
                        User.GetRolePlay().SendWeb("{\"name\":\"targethealth\", \"health\":\"" + Health + "\", \"maxhealth\":\"" + (MaxHealth + bonushp) + "\"}");
                        if (Health < 1 && User.GetRolePlay().lockTarget == habbo.Id)
                        {
                            User.GetRolePlay().SendWeb("{\"name\":\"lock\", \"lock\":\"true\"}");
                            User.GetRolePlay().lockTarget = 0;
                        }
                    }
                }
            }
        }
        public void EnergyChange(int i)
        {
            Energy = i;
            SendWeb("{\"name\":\"energy\", \"energy\":\"" + Energy + "\"}");
            if (lockedon > 0)
            {
                foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (User.GetRolePlay().lockID == habbo.Id)
                        User.GetRolePlay().SendWeb("{\"name\":\"targetenergy\", \"energy\":\"" + Energy + "\"}");
                }
            }
        }
        public void HungerChange(int i)
        {
            Hunger = i;
            SendWeb("{\"name\":\"hunger\", \"hunger\":\"" + Hunger + "\"}");
            if (lockedon > 0)
            {
                foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (User.GetRolePlay().lockID == habbo.Id)
                        User.GetRolePlay().SendWeb("{\"name\":\"targethunger\", \"hunger\":\"" + Hunger + "\"}");
                }
            }
        }
        public void Look(string look)
        {
            habbo.Look = look;
            SendWeb("{\"name\":\"look\", \"look\":\"" + look + "\"}");
            if (lockedon > 0)
            {
                foreach (GameClient User in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (User.GetRolePlay().lockID == habbo.Id && User != client)
                        User.GetRolePlay().SendWeb("{\"name\":\"targetlook\", \"look\":\"" + look + "\"}");
                }
            }
        }
        public void LoadStats(bool web = false)
        {
            if (ws == null)
                client.Disconnect();
           Console.WriteLine("Websocket is null!");
            int _xpdue = XPdue;
            int _xp = XP;
            if (XPdue > 75)
            {
                _xpdue = XPdue / 2;
                _xp = XP - _xpdue;
            }
            var money = string.Format("{0:n0}", habbo.Credits);
            SendWeb("{\"name\":\"stats\", \"health\":\"" + Health + "\", \"maxhealth\":\"" + (MaxHealth + bonushp) + "\","
                + "\"energy\":\"" + Energy + "\", \"look\":\"" + habbo.Look + "\", \"xp\":\"" + _xp + "\","
                + "\"xpdue\":\"" + _xpdue + "\", \"level\":\"" + Level + "\", \"wslot1\":\"" + Inventory.Item1 + "\","
                + "\"wslot2\":\"" + Inventory.Item2 + "\", \"wslot3\":\"" + Inventory.Item3 + "\", \"wslot4\":\"" + Inventory.Item4 + "\","
                + "\"wslot5\":\"" + Inventory.Item5 + "\", \"wslot6\":\"" + Inventory.Item6 + "\", \"wslot7\":\"" + Inventory.Item7 + "\","
                + "\"wslot9\":\"" + Inventory.Item9 + "\", \"wslot10\":\"" + Inventory.Item10 + "\", \"wslot11\":\"" + Inventory.Item11 + "\", \"wslot12\":\"" + Inventory.Item12 + "\","
                + "\"wslot8\":\"" + Inventory.Item8 + "\", \"whp1\":\"" + (Inventory.ItemHP("w1") - Inventory.HP1) + "\", \"whp2\":\"" + (Inventory.ItemHP("w2") - Inventory.HP2) + "\","
                + "\"whp3\":\"" + (Inventory.ItemHP("w3") - Inventory.HP3) + "\", \"whp4\":\"" + (Inventory.ItemHP("w4") - Inventory.HP4) + "\", \"whp5\":\"" + (Inventory.ItemHP("w5") - Inventory.HP5) + "\","
                + "\"whp6\":\"" + (Inventory.ItemHP("w6") - Inventory.HP6) + "\", \"whp7\":\"" + (Inventory.ItemHP("w7") - Inventory.HP7) + "\", \"whp8\":\"" + (Inventory.ItemHP("w8") - Inventory.HP8) + "\","
                + "\"whp9\":\"" + (Inventory.ItemHP("w9") - Inventory.HP9) + "\", \"whp10\":\"" + (Inventory.ItemHP("w10") - Inventory.HP10) + "\", \"whp11\":\"" + (Inventory.ItemHP("w11") - Inventory.HP11) + "\", \"whp12\":\"" + (Inventory.ItemHP("w12") - Inventory.HP12) + "\","
                + "\"maxhp1\":\"" + Inventory.ItemHP("w1") + "\", \"maxhp2\":\"" + Inventory.ItemHP("w2") + "\", \"maxhp3\":\"" + Inventory.ItemHP("w3") + "\","
                + "\"maxhp4\":\"" + Inventory.ItemHP("w4") + "\", \"maxhp5\":\"" + Inventory.ItemHP("w5") + "\", \"maxhp6\":\"" + Inventory.ItemHP("w6") + "\","
                + "\"maxhp7\":\"" + Inventory.ItemHP("w7") + "\", \"maxhp8\":\"" + Inventory.ItemHP("w8") + "\", \"maxhp9\":\"" + Inventory.ItemHP("w9") + "\","
                + "\"maxhp10\":\"" + Inventory.ItemHP("w10") + "\", \"maxhp11\":\"" + Inventory.ItemHP("w11") + "\", \"maxhp12\":\"" + Inventory.ItemHP("w12") + "\","
                + "\"quantity1\":\"" + Inventory.Quantity1 + "\", \"quantity2\":\"" + Inventory.Quantity2 + "\", \"quantity3\":\"" + Inventory.Quantity3 + "\","
                + "\"quantity4\":\"" + Inventory.Quantity4 + "\", \"quantity5\":\"" + Inventory.Quantity5 + "\", \"quantity6\":\"" + Inventory.Quantity6 + "\","
                + "\"quantity9\":\"" + Inventory.Quantity9 + "\", \"quantity10\":\"" + Inventory.Quantity10 + "\", \"quantity11\":\"" + Inventory.Quantity11 + "\", \"quantity12\":\"" + Inventory.Quantity12 + "\","
                + "\"quantity7\":\"" + Inventory.Quantity7 + "\", \"quantity8\":\"" + Inventory.Quantity8 + "\", \"money\":\"" + money + "\", \"enable1\":\"" + enable_trade + "\","
                + "\"enable2\":\"" + enable_sound + "\", \"enable3\":\"" + enable_marcos + "\", \"color1\":\"" + Color1 + "\", \"color2\":\"" + Color2 + "\","
                + "\"hunger\":\"" + Hunger + "\", \"xp1\":\"" + XP + "\", \"xp2\":\"" + XPdue + "\"}");
            if (web)
                return;
            if (habbo.FriendCount > 0)
            {
                foreach (MessengerBuddy buddy in habbo.GetMessenger().GetFriends())
                {
                    if (buddy.IsOnline && buddy.Client != client)
                    buddy.Client.GetRolePlay().SendWeb("{\"name\":\"sidealert\", \"evnt\":\"online\", \"name1\":\"" + habbo.Username + "\", \"color\":\"" + Color + "\"}");
                }
            }
            if (Jailed > 0 || JailedSec > 0)
            {
                Timer(Jailed, JailedSec, "jail");
                prevMotto = habbo.Motto;
                previousLook = habbo.Look;
                habbo.Motto = "Inmate [#" + PlusEnvironment.GetRandomNumber(1, 9999) + "]";
                string uniform = "";
                if (habbo.Gender == "f")
                    uniform = Figure(habbo.Look) + "ch-635-94.lg-695-94.sh-300-62";
                else
                    uniform = Figure(habbo.Look) + "ch-3030-94.lg-695-94.sh-300-62";
                Look(uniform);
            }
            if (RPTimer == null)
            RPTimer = new RolePlayTimer(this);
        }
        public void RoomJobInfo()
        {
            string jobbadge = "null";
            if (Room.Id == 5)
                jobbadge = "nypd.png";
            else if (Room.Id == 2)
                jobbadge = "hosp.gif";
            else if (Room.Id == 3)
                jobbadge = "bank.png";
            else if (Room.Id == 10)
                jobbadge = "rest.gif";
            else if (Room.Id == 268 || Room.Id == 7)
                jobbadge = "bbj.gif";
            else jobbadge = "none";
            SendWeb("{\"name\":\"roomjobinfo\", \"job\":\"" + jobbadge + "\", \"roomid\":\"ID: " + Room.Id + "\"}");
        }
        public void Marriage(string action)
        {
            if (action == "marry")
            {
                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(smID).GetRolePlay();
                SoulMate = Target.habbo.Username;
                smColor = Target.GetClient().GetRolePlay().Color;
                habbo.GetBadgeComponent().GiveBadge("WD0", true, client);
                client.SendWhisper("You have recieved a wedding badge!");
                Marriage("save");
            }
            else if (action == "data")
            {
                using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    Habbo targetData = PlusEnvironment.GetHabboById(smID);
                    if (targetData != null)
                        SoulMate = targetData.Username;
                    DB.SetQuery("SELECT color FROM stats WHERE id = '" + smID + "' LIMIT 1");
                    smColor = DB.GetString();
                }
            }
            else if (action == "divorce")
            {
                var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(smID);
                if (user != null)
                {
                    user.SendWhisper(habbo.Username + " divorcioi de você!");
                    user.GetRolePlay().SoulMate = "";
                    user.GetRolePlay().smID = 0;
                    user.GetRolePlay().Marriage("save");
                }
                else
                {
                    using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        DB.RunQuery("UPDATE stats SET soulmate = '0' WHERE id = '" + smID + "'");
                }
                using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    DB.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + smID + " LIMIT 1");
                    DB.AddParameter("badge", "WD0");
                    DB.RunQuery();
                }
                SoulMate = "";
                smID = 0;
                habbo.GetBadgeComponent().RemoveBadge("WD0", client);
                Marriage("save");
            }
            else if (action == "save")
            {
                using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    DB.RunQuery("UPDATE stats SET soulmate = '" + smID + "' WHERE id = '" + habbo.Id + "'");
            }
        }
        #endregion
    }
}