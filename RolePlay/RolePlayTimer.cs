using System;
using System.Linq;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.RolePlay;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.HabboHotel.Rooms.AI;
using Org.BouncyCastle.Bcpg;

namespace Plus.RolePlay.Timer
{
    public class RolePlayTimer
    {
        public RPData RP;

        int CycleCount;

        public RolePlayTimer(RPData RP)
        {

            this.RP = RP;
        }
      
        public void OnCycle()
        {
            try
            {
                CycleCount = 2;
                CycleCount--;
                RP.Cooldown--;
                if (RP.Cooldown2 > 0)
                    RP.Cooldown2--;
                if (RP.AggressionCD > 0)
                    RP.AggressionCD--;
                //RP.client.SendWhisper("" + RP.AggressionCD + "");
                
                if (RP.Cooldown3 > 0)
                    RP.Cooldown3--;
                if (RP.subCooldown > 0)
                    RP.subCooldown--;
                if (RP.roomUser.CarrotTimer > 0)
                    RP.roomUser.CarrotTimer--;
                




                if (RP.dubCooldown > 0)
                RP.dubCooldown--;
                if (RP.InteractingCD > 0)
                    RP.InteractingCD--;
                if (RP.ItemCD > 0)
                    RP.ItemCD--;
                if (RP.atmCD > 0)
                    RP.atmCD--;
                if (RP.WeedTimer > 0)
                    RP.WeedTimer--;
                if (RP.fishingCD > 0)
                    RP.fishingCD--;
                if (RP.FreeMoney > 0)
                {
                    RP.FreeMoney--;
                    if (RP.FreeMoney == 0)
                    {
                        RP.FreeMoney = 600;
                        RP.UpdateCredits(4, true);
                    }
                }
                
                if (RP.WeedTime > 0)
                {
                    RP.WeedTime--;
                    if (RP.WeedTime == 0)
                    {
                        RP.client.SendWhisper("Your temporary stamina recovery boost has worn off!");
                        RP.MaxStrength--;
                    }
                }
                if (RP.TempEffect > 0)
                {
                    RP.TempEffect--;
                    if (RP.TempEffect == 0)
                        RP.roomUser.ApplyEffect(0);
                }
                if (RP.CallPoliceDelay > 0)
                {
                    RP.CallPoliceDelay--;
                    if (RP.CallPoliceDelay == 0)
                        RP.Room.PlacePolice(RP.callroomid);
                }
                if (RP.AutoLogout > 0)
                {
                    RP.AutoLogout--;
                    if (RP.AutoLogout == 0)
                    {
                        if (RP.lockedon > 0)
                        {
                            foreach (GameClient User in RP.habbo.GetClientManager()._clients.Values.ToList())
                            {
                                if (User.GetRolePlay().lockID == RP.habbo.Id)
                                {
                                    User.GetRolePlay().lockID = 0;
                                    if (User.GetRolePlay().lockTarget == RP.habbo.Id)
                                    {
                                        User.GetRolePlay().SendWeb("{\"name\":\"lock\", \"lock\":\"true\"}");
                                        User.GetRolePlay().lockTarget = 0;
                                    }
                                }
                            }
                        }
                        RP.client.Dispose(false);
                    }
                }
                if (RP.achievetimer > 0)
                {
                    RP.achievetimer--;
                    if (RP.achievetimer == 0)
                        RP.SendWeb("{\"name\":\"achievetimer\"}");

                }
                if (RP.xpdelay > 0)
                {
                    RP.xpdelay--;
                    if (RP.xpdelay == 0)
                    {
                        RP.XPSet(RP.xpsave);
                        RP.xpsave = 0;
                    }
                }
                if (RP.roomUser.delay > 0)
                {
                    RP.roomUser.delay--;
                    if (RP.roomUser.delay == 0)
                        RP.RoomForward(RP.roomUser.delayroom);
                }
                if (RP.JobManager.Sendhome > 0)
                    RP.JobManager.Sendhome--;
                if (RP.TurfTime > 0)
                {
                    RP.TurfTimer++;
                    if (RP.TurfTimer == 60)
                    {
                        RP.TurfTimer = 0;
                        RP.TurfTime--;
                        if (RP.TurfTime == 0)
                        {
                            RP.Say("successfully claims " + RP.Room.Name + "", false);
                            if (RP.Room.Name.Contains("North"))
                            {
                                RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "North Turf", RP.habbo.GetClientManager().NorthTurf);
                                RP.habbo.GetClientManager().SetTurfs(1, RP.Gang, RP.GangManager.Name);
                            }
                            else if (RP.Room.Name.Contains("East"))
                            {
                                RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "East Turf", RP.habbo.GetClientManager().EastTurf);
                                RP.habbo.GetClientManager().SetTurfs(2, RP.Gang, RP.GangManager.Name);
                            }
                            else if (RP.Room.Name.Contains("West"))
                            {
                                RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "West Turf", RP.habbo.GetClientManager().WestTurf);
                                RP.habbo.GetClientManager().SetTurfs(3, RP.Gang, RP.GangManager.Name);
                            }
                            else if (RP.Room.Name.Contains("South"))
                            {
                                RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "South Turf", RP.habbo.GetClientManager().SouthTurf);
                                RP.habbo.GetClientManager().SetTurfs(4, RP.Gang, RP.GangManager.Name);
                            }
                            RP.XPSet(100);
                            RP.captures++;
                        }
                        else
                        {
                            if (RP.TurfTime == 1)
                                RP.Say("" + RP.TurfTime + " minute left to claim this turf", false);
                            else RP.Say("" + RP.TurfTime + " minutes left to claim this turf", false);
                        }
                    }
                    if (RP.roomUser.IsWalking || RP.Health < 1)
                    {
                        RP.TurfTime = 0;
                        RP.TurfTimer = 0;
                        RP.Say("stops claiming this turf", false);
                    }

                }
                if (RP.EscortID > 0)
                {
                    var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.EscortID);
                    if (user.GetRolePlay().Room.Id != RP.Room.Id && (user.GetHabbo().roomUser.X > 0 || user.GetHabbo().roomUser.Y > 0))
                        RP.RoomForward(user.GetRolePlay().Room.Id);
                }

                #region Stats Update
                if (RP.habbo.HomeRoom != RP.Room.Id)
                {
                    RP.habbo.HomeRoom = RP.Room.Id;
                    RP.SetHomeRoom();
                }
                if (RP.prevShift != RP.JobManager.Shifts)
                {
                    RP.prevShift = RP.JobManager.Shifts;
                    RP.RPCache(20);
                }
                if (RP.prevHealth != RP.Health)
                {
                    RP.prevHealth = RP.Health;
                    RP.RPCache(1);
                }
                if (RP.prevHunger != RP.Hunger)
                {
                    RP.prevHunger = RP.Hunger;
                    RP.RPCache(2);
                }
                if (RP.prevEnergy != RP.Energy)
                {
                    RP.prevEnergy = RP.Energy;
                    RP.RPCache(3);
                }
                if (RP.prevXPdue != RP.XPdue)
                {
                    RP.prevXPdue = RP.XPdue;
                    RP.RPCache(5);
                }
                if (RP.prevPunches != RP.Punches)
                {
                    RP.prevPunches = RP.Punches;
                    RP.RPCache(6);
                }
                if (RP.prevStrength != RP.Strength)
                {
                    RP.prevStrength = RP.Strength;
                    RP.RPCache(7);
                }
                if (RP.prevArrests != RP.Arrests)
                {
                    RP.prevArrests = RP.Arrests;
                    RP.RPCache(8);
                }
                if (RP.prevDeaths != RP.Deaths)
                {
                    RP.prevDeaths = RP.Deaths;
                    RP.RPCache(9);
                }
                if (RP.prevDead != RP.Dead)
                {
                    RP.prevDead = RP.Dead;
                    RP.RPCache(11);
                }
                if (RP.prevJob != RP.JobManager.Job)
                {
                    RP.prevJob = RP.JobManager.Job;
                    RP.RPCache(13);
                }
                if (RP.prevKills != RP.Kills)
                {
                    RP.prevKills = RP.Kills;
                    RP.RPCache(14);
                }
                if (RP.prevJobRank != RP.JobManager.JobRank)
                {
                    RP.prevJobRank = RP.JobManager.JobRank;
                    RP.RPCache(15);
                }
                if (RP.prevLevel != RP.Level)
                {
                    RP.prevLevel = RP.Level;
                    RP.RPCache(16);
                }
                if (RP.prevGang != RP.Gang)
                {
                    RP.prevGang = RP.Gang;
                    RP.RPCache(17);
                }
                if (RP.prevMaxHealth != RP.MaxHealth)
                {
                    RP.prevMaxHealth = RP.MaxHealth;
                    RP.RPCache(18);
                }
                if (RP.habbo.Duckets != RP.XP)
                    RP.habbo.Duckets = RP.XP;
                #endregion

                #region Jail
                if (RP.roomUser.Stunned > 0 && !RP.isSleeping)
                {
                    RP.roomUser.Stunned--;
                    if (RP.roomUser.IsWalking)
                    {
                        RP.roomUser.IsWalking = false;
                        RP.roomUser.RemoveStatus("mv");
                    }
                    if (!RP.Dead && RP.Health > 0 && RP.Jailed == 0 && RP.JailedSec == 0 && !RP.Cuffed && RP.AutoLogout == 0)
                        RP.roomUser.ApplyEffect(53);
                    if (RP.roomUser.Stunned == 0 && !RP.Cuffed)
                        RP.roomUser.ApplyEffect(0);
                }
                if (RP.CallPoliceDelay > 0)
                {
                    RP.CallPoliceDelay--;
                    if (RP.CallPoliceDelay == 0)
                        RP.Room.PlacePolice(RP.callroomid);
                }
                if (RP.BackUpPolice > 0)
                {
                    RP.BackUpPolice--;
                    if (RP.BackUpPolice == 0)
                        RP.Room.PlacePolice(RP.callroomid2);
                }
                if (RP.Assault == true && RP.roomUser.Assault == 0)
                    RP.roomUser.Assault = 1;
                #endregion

                #region Taxi
                if (RP.roomUser.TaxiTime > 0)
                {
                    RP.roomUser.TaxiTime--;
                    if (RP.roomUser.TaxiTime == 0 && !RP.Dead && RP.Jailed == 0 && RP.JailedSec == 0)
                    {

                        if (RP.JobManager.Working && (RP.JobManager.Job == 1 || RP.JobManager.Job == 7))
                        {
                            RP.roomUser.ApplyEffect(0);
                            RP.GetClient().SendPacket(new CreditBalanceComposer(RP.habbo.Credits));
                            RP.RoomForward(RP.roomUser.TaxiDest);
                        }
                        else RP.Room.PlaceTaxi(RP.habbo.Id);
                    }
                    if (RP.roomUser.IsWalking || RP.roomUser.Stunned > 0 || RP.Cuffed || RP.Health <= 0)
                    {
                        RP.roomUser.TaxiTime = 0;
                        RP.roomUser.TaxiDest = 0;
                        RP.roomUser.ApplyEffect(0);
                    }
                }
                if (RP.roomUser.SendRoom > 0)
                {
                    RP.RoomForward(RP.roomUser.SendRoom);
                    RP.roomUser.SendRoom = 0;
                }
                #endregion

                #region Health Management
                if (RP.Health < 1 && RP.roomUser.Stunned == 0 && !RP.Dead)
                {
                    /*if (RP.Inventory.Equip1 != "null")
                    RP.WebHandler.Handle("equip", "", "e1");
                    if (RP.Inventory.Equip2 != "null")
                        RP.WebHandler.Handle("equip", "", "e2");*/
                    if (RP.GameType != "")
                    {
                        if (RP.GameType == "Brawl")
                        {
                            RP.HealthChange(RP.MaxHealth + RP.bonushp);
                            RP.RoomForward(21);
                            RP.GameType = "";
                            RP.habbo.Motto = RP.prevMotto;
                        }
                        else if (RP.GameType == "Mafia")
                        {
                            RP.HealthChange(RP.MaxHealth + RP.bonushp);
                            RP.RoomForward(170);
                            if (RP.Team == "Blue")
                            {
                                RP.roomUser.SetPos(7, 4, RP.roomUser.Z);
                                RP.Rotate = 6;
                            }
                            else if (RP.Team == "Green")
                            {
                                RP.roomUser.SetPos(8, 10, RP.roomUser.Z);
                                RP.Rotate = 2;
                            }
                            // RP.GetClient().SendNotifi("You have been knocked out and sent to the graveyard!");
                        }
                        else if (RP.GameType == "CW")
                        {

                        }
                    }
                    else
                    {
                        RP.EnergyChange(0);
                        if (RP.JobManager.Working)
                            RP.Stopwork(false);
                        if (RP.Cuffed)
                        {
                            RP.Cuffed = false;
                            if (RP.roomUser.CurrentEffect == 590)
                                RP.roomUser.ApplyEffect(0);
                        }
                        if (RP.EscortID > 0 || RP.Escorting > 0)
                            RP.EndEscort();
                        RP.DeathChange();
                        RP.roomUser.Stunned = 9001; //OVER 9 THOUSAND!!!!!!!
                        RP.roomUser.Faint = 6;
                        RP.GetClient().SendWhisper("Você desmaiou e os paramédicos estão a caminho!");
                        RP.DeadSetup(false, false);
                    }
                }
                if (RP.Health < 1)
                {
                    RP.takingtoolong++;
                    if (RP.takingtoolong >= 120)
                    {
                        if (RP.Hunger < 10)
                            RP.HungerChange(RP.Hunger + 5);
                        RP.roomUser.Stunned = 0;
                        RP.roomUser.ApplyEffect(0);
                        RP.DeadSetup(true, true);
                        RP.takingtoolong = 0;

                    }
                }
                if (RP.ParamedicBotDelay > 0 && RP.Health < 1)
                {
                    RP.ParamedicBotDelay--;
                    if (RP.ParamedicBotDelay == 0)
                        RP.Room.PlaceParamedic();
                }
                if (RP.Health <= 25)
                {
                    if (!RP.Dead && RP.roomUser.Stunned == 0 && RP.JustHealed == 0 && RP.AutoLogout == 0 &&
                        !RP.Cuffed && RP.roomUser.CurrentEffect != 592 && RP.roomUser.TaxiDest == 0 && !RP.roomUser.IsLying && RP.EscortID == 0)
                    {
                        RP.Delay++;
                        if (RP.Delay == 2 && RP.Lowhealth)
                        {
                            RP.roomUser.ApplyEffect(11);
                            RP.Delay = 0;
                            RP.Lowhealth = false;
                        }
                        else
                            if (RP.Delay == 2 && !RP.Lowhealth)
                        {
                            RP.roomUser.ApplyEffect(0);
                            RP.Delay = 0;
                            RP.Lowhealth = true;
                        }
                    }
                    RP.Delay2++;
                    if (RP.Delay2 == 1 && RP.Lowhealth2)
                    {
                        RP.Delay2 = 0;
                        RP.Lowhealth2 = false;
                        RP.SendWeb("{\"name\":\"bleed\", \"bleed\":\"true\"}");
                    }
                    else
                        if (RP.Delay2 == 1 && !RP.Lowhealth2)
                    {
                        RP.Delay2 = 0;
                        RP.Lowhealth2 = true;
                        RP.SendWeb("{\"name\":\"bleed\", \"bleed\":\"false\"}");
                    }

                    if (!RP.Dead && RP.Jailed == 0 && RP.JailedSec == 0 && RP.GameType == "" && RP.Health > 0 && RP.EscortID == 0)
                    {
                        RP.roomUser.Losehealth++;
                        if (RP.roomUser.Losehealth >= 100)
                        {
                            if (RP.Health - 2 < 0)
                                RP.HealthChange(0, 0);
                            else RP.HealthChange(RP.Health - 2, 2);
                            RP.roomUser.Losehealth = 0;
                            if (RP.Health > 0)
                                RP.GetClient().SendWhisper("Você perdeu 2 hp de sangue!");
                        }
                    }
                }
                if (RP.bleeding > 0 && RP.Health > 25)
                    RP.bleeding = 0;

                if (RP.Dead)
                {
                    RP.roomUser.DeadTimer++;
                    if (RP.roomUser.DeadTimer >= 2)
                    {
                        if (RP.habbo.Rank > 3)
                            RP.HealthChange(RP.Health + 2);
                        else RP.HealthChange(RP.Health + 1);
                        RP.roomUser.DeadTimer = 0;
                        if (RP.Health >= RP.MaxHealth + RP.bonushp)
                        {
                            if (RP.Assault)
                            {
                                RP.Assault = false;
                                RP.roomUser.Assault = 0;
                            }
                            RP.HealthChange(RP.MaxHealth + RP.bonushp);
                            RP.Dead = false;
                            RP.Say("recupera a consciência", false);
                            //RP.GetClient().SendNotifi("Agora você está livre para sair!");
                            RP.roomUser.Stunned = 0;
                            RP.Refresh();
                        }
                    }
                }
                if (RP.JustHealed > 0 && RP.Health > 0)
                {
                    if (RP.Health >= RP.MaxHealth + RP.bonushp)
                    {
                        RP.JustHealed = 0;
                        RP.roomUser.healtimer = 0;
                        RP.roomUser.ApplyEffect(0);
                    }
                    else
                    {
                        RP.roomUser.healtimer++;
                        if (RP.roomUser.healtimer >= 4)
                        {

                            RP.JustHealed--;
                            RP.roomUser.healtimer = 0;
                            if (RP.JustHealed == 0)
                                RP.roomUser.ApplyEffect(0);
                            if (RP.Health + 10 >= RP.MaxHealth + RP.bonushp)
                                RP.HealthChange(RP.MaxHealth + RP.bonushp);
                            else RP.HealthChange(RP.Health + 5);
                        }
                    }

                }
                if (RP.roomUser.Faint > 0 && RP.Health < 1)
                {
                    RP.Faint();
                    RP.roomUser.Faint--;
                }
                if (RP.boltkill > 0)
                {
                    RP.roomUser.CanWalk = false;
                    RP.boltkill--;
                    if (RP.roomUser.CurrentEffect != 12)
                        RP.roomUser.ApplyEffect(12);
                    else RP.roomUser.ApplyEffect(0);
                    if (RP.boltkill == 0)
                    {
                        if (RP.roomUser.CurrentEffect == 12)
                            RP.roomUser.ApplyEffect(0);
                        RP.HealthChange(RP.Health - 9999);
                        RP.roomUser.CanWalk = true;
                    }
                }
                #endregion

                #region Hunger/Energy
                RP.roomUser.HungerTime++;
                if (RP.roomUser.HungerTime >= 400)
                {
                    if (RP.Hunger < 1 && RP.Health > 26)
                    {
                        RP.HealthChange(RP.Health - 1);
                        RP.client.SendWhisper("Você perdeu 1 hp de fome!");
                        RP.SendWeb("{\"name\":\"hungeralert\", \"meal\":\"" + PlusEnvironment.GetRandomNumber(1, 5) + "\"}");
                    }
                    else if (RP.Hunger > 0)
                        RP.HungerChange(RP.Hunger - 1);
                    RP.roomUser.HungerTime = 0;
                }
                if (RP.JobManager.Working)
                    RP.roomUser.EnergyTimer++;
                if (RP.roomUser.EnergyTimer >= 600 && RP.MaxEnergy > 0)
                {
                    RP.MaxEnergy -= 2;
                    if (RP.Energy > RP.MaxEnergy)
                        RP.Energy = RP.MaxEnergy;
                    RP.roomUser.EnergyTimer = 0;
                }
                if (RP.Energy < RP.MaxEnergy && RP.Health > 0)
                {
                    if (RP.roomUser.EnergyTime > 0)
                        RP.roomUser.EnergyTime--;
                    if (RP.roomUser.EnergyTime <= 0)
                    {
                        if (RP.Energy + 5 >= RP.MaxEnergy)
                            RP.EnergyChange(RP.MaxEnergy);
                        else
                            RP.EnergyChange(RP.Energy + 5);
                    }
                }
                if (RP.hitcount >= 10)
                {
                    RP.hitcount = 0;
                    RP.MaxEnergy -= 12;
                }
                if (RP.isSleeping && RP.MaxEnergy < 100)
                {
                    RP.sleeptimer++;
                    if (RP.sleeptimer > 100)
                    {
                        if (RP.MaxEnergy + 5 >= 100)
                            RP.MaxEnergy = 100;
                        else RP.MaxEnergy += 5;
                        if (RP.Energy > RP.MaxEnergy)
                            RP.EnergyChange(RP.MaxEnergy);
                    }
                }
                #endregion

                #region Bots/Pets
                if (RP.HasPet > 0 && !RP.roomUser.petspawned)
                {
                    if (RP.SpawnPet == 0)
                    {
                        if (RP.roomUser.PetDelay == 0)
                            RP.roomUser.PetDelay = 3;

                        if (RP.roomUser.PetDelay > 0)
                        {
                            RP.roomUser.PetDelay--;
                            if (RP.roomUser.PetDelay == 0)
                            {
                                RP.roomUser.petspawned = true;
                                RP.PetFollow(false, 0, null);
                            }
                        }
                    }
                    if (RP.SpawnPet > 0)
                        RP.SpawnPet--;
                }
                if (RP.personalbot && !RP.roomUser.spawnbot && !RP.Cuffed)
                {
                    RP.roomUser.spawnbot = true;
                    RP.Room.PersonalBot(RP.Room.Id, RP.roomUser.X, RP.roomUser.Y);

                }
                #endregion

                #region Working
                if (RP.JobManager.Working)
                {
                    if (RP.JobManager.Job != 1 && RP.JobManager.Job != 7 && RP.Room.Id != RP.JobManager.JobRoom)
                    {
                        if (RP.JobManager.Job == 3 && RP.Room.Name.Contains("Bank"))
                        { }
                        else RP.Stopwork(false);
                    }
                }
                #endregion

                #region Games
                if (RP.GameType == "Mafia")
                {
                    if (RP.BeatGame && !RP.habbo.GetClientManager().GameOver)
                    {
                        RP.habbo.GetClientManager().GameOver = true;
                        RP.habbo.GetClientManager().WiningTeam = RP.Team;
                        RP.BeatGame = false;
                        // habbo.GetClientManager().CheckGame();
                    }
                    if (RP.habbo.GetClientManager().GameOver && RP.Room.Id != 23)
                    {
                        if (RP.Team == RP.habbo.GetClientManager().WiningTeam)
                            RP.XPSet(150);
                        //RP.GetClient().SendNotifi("Game Over!\r\r" + RP.habbo.GetClientManager().WiningTeam + " Team Won!");
                        RP.LeaveGame(true, true);
                        RP.HealthChange(RP.MaxHealth + RP.bonushp);

                    }

                    if (RP.Room.Id == 23)
                    {
                        RP.roomUser.GameDelay--;
                        if (RP.Room.GreenTeam >= 1 && RP.Room.BlueTeam >= 1) // team full
                            RP.Room.GameFull = true;// send players to game
                        if (RP.Room.GameFull && !RP.habbo.GetClientManager().GameInSession) // game starts
                        {
                            if (RP.SendToMafia == 0)
                                RP.SendToMafia = PlusEnvironment.GetRandomNumber(2, 5);
                        }
                        else
                            if (RP.roomUser.GameDelay == 0) // team is not equal
                        {
                            RP.roomUser.GameDelay = 50;
                            if (RP.habbo.GetClientManager().GameInSession) // game already in session
                                RP.GetClient().SendWhisper("There is currently a game in-session, please wait for the game to end.");
                            else RP.GetClient().SendWhisper("Team must be equal, 5 players per team. Blue : " + RP.Room.BlueTeam + " Green: " + RP.Room.GreenTeam + " ");
                        }
                    }

                    if (RP.SendToMafia > 0)
                    {
                        RP.SendToMafia--;
                        if (RP.SendToMafia == 0)
                        {
                            if (RP.Room.Id == 23)
                                RP.RoomForward(170);
                        }
                    }
                }
                #endregion

                #region Item Duration
                if (RP.DisplayRoomInfo)
                {
                    if (RP.habbo.InRoom)
                    {
                        if (RP.Skateboard && RP.Room.Taxi == 1)
                        {
                            if (RP.Inventory.Equip1 != "null")
                                RP.WebHandler.Handle("equip", "", "e1");
                        }
                        RP.RoomJobInfo();
                        RP.DisplayRoomInfo = false;
                    }
                }
                if (RP.Skateboard)
                {
                    if (!RP.roomUser.SuperFastWalking)
                        RP.roomUser.SuperFastWalking = true;
                }
                if (RP.actionpoint && RP.roomUser.IsWalking)
                {
                    RP.actionpoint = false;
                    if (RP.Storage.Curstorage != 0)
                        RP.Storage.StorageCache(0, true);
                    RP.Storage.Curstorage = 0;
                    RP.SendWeb("{\"name\":\"ap\"}");
                }
                if (RP.Trade.TradeReady && RP.Trade.TradeTimer > 0)
                {

                    RP.Trade.TradeTimer--;
                    if (RP.Trade.TradeTimer == 0)
                    {
                        #region Trade
                        int money = RP.Trade.TradeMoney;
                        string t1 = RP.Trade.Slot1;
                        string t2 = RP.Trade.Slot2;
                        string t3 = RP.Trade.Slot3;
                        string t4 = RP.Trade.Slot4;
                        string t5 = RP.Trade.Slot5;
                        string t6 = RP.Trade.Slot6;
                        int q1 = RP.Trade.Quantity1;
                        int q2 = RP.Trade.Quantity2;
                        int q3 = RP.Trade.Quantity3;
                        int q4 = RP.Trade.Quantity4;
                        int q5 = RP.Trade.Quantity5;
                        int q6 = RP.Trade.Quantity6;
                        int h1 = RP.Trade.HP1;
                        int h2 = RP.Trade.HP2;
                        int h3 = RP.Trade.HP3;
                        int h4 = RP.Trade.HP4;
                        int h5 = RP.Trade.HP5;
                        int h6 = RP.Trade.HP6;
                        var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                        RP.Trade.TradeMoney = User.GetClient().GetRolePlay().Trade.TradeMoney;
                        RP.Trade.Slot1 = User.GetClient().GetRolePlay().Trade.Slot1;
                        RP.Trade.Slot2 = User.GetClient().GetRolePlay().Trade.Slot2;
                        RP.Trade.Slot3 = User.GetClient().GetRolePlay().Trade.Slot3;
                        RP.Trade.Slot4 = User.GetClient().GetRolePlay().Trade.Slot4;
                        RP.Trade.Slot5 = User.GetClient().GetRolePlay().Trade.Slot5;
                        RP.Trade.Slot6 = User.GetClient().GetRolePlay().Trade.Slot6;
                        RP.Trade.Quantity1 = User.GetClient().GetRolePlay().Trade.Quantity1;
                        RP.Trade.Quantity2 = User.GetClient().GetRolePlay().Trade.Quantity2;
                        RP.Trade.Quantity3 = User.GetClient().GetRolePlay().Trade.Quantity3;
                        RP.Trade.Quantity4 = User.GetClient().GetRolePlay().Trade.Quantity4;
                        RP.Trade.Quantity5 = User.GetClient().GetRolePlay().Trade.Quantity5;
                        RP.Trade.Quantity6 = User.GetClient().GetRolePlay().Trade.Quantity6;
                        RP.Trade.HP1 = User.GetClient().GetRolePlay().Trade.HP1;
                        RP.Trade.HP2 = User.GetClient().GetRolePlay().Trade.HP2;
                        RP.Trade.HP3 = User.GetClient().GetRolePlay().Trade.HP3;
                        RP.Trade.HP4 = User.GetClient().GetRolePlay().Trade.HP4;
                        RP.Trade.HP5 = User.GetClient().GetRolePlay().Trade.HP5;
                        RP.Trade.HP6 = User.GetClient().GetRolePlay().Trade.HP6;
                        User.GetClient().GetRolePlay().Trade.Slot1 = t1;
                        User.GetClient().GetRolePlay().Trade.Slot2 = t2;
                        User.GetClient().GetRolePlay().Trade.Slot3 = t3;
                        User.GetClient().GetRolePlay().Trade.Slot4 = t4;
                        User.GetClient().GetRolePlay().Trade.Slot5 = t5;
                        User.GetClient().GetRolePlay().Trade.Slot6 = t6;
                        User.GetClient().GetRolePlay().Trade.Quantity1 = q1;
                        User.GetClient().GetRolePlay().Trade.Quantity2 = q2;
                        User.GetClient().GetRolePlay().Trade.Quantity3 = q3;
                        User.GetClient().GetRolePlay().Trade.Quantity4 = q4;
                        User.GetClient().GetRolePlay().Trade.Quantity5 = q5;
                        User.GetClient().GetRolePlay().Trade.Quantity6 = q6;
                        User.GetClient().GetRolePlay().Trade.HP1 = h1;
                        User.GetClient().GetRolePlay().Trade.HP2 = h2;
                        User.GetClient().GetRolePlay().Trade.HP3 = h3;
                        User.GetClient().GetRolePlay().Trade.HP4 = h4;
                        User.GetClient().GetRolePlay().Trade.HP5 = h5;
                        User.GetClient().GetRolePlay().Trade.HP6 = h6;
                        User.GetClient().GetRolePlay().Trade.TradeMoney = money;
                        RP.Trade.StopTrade();
                        User.GetClient().GetRolePlay().Trade.StopTrade();
                        #endregion
                    }
                }
                if (RP.roomUser.CarryItemId == 33 || RP.roomUser.CarryItemId == 19 || RP.roomUser.CarryItemId == 9 || RP.roomUser.CarryItemId == 8 || RP.roomUser.CarryItemId == 85)
                {
                    if ((RP.JobManager.Job == 6 && RP.JobManager.Working) || RP.onduty)
                    { }
                    else
                    {
                        RP.roomUser.EatItem++;
                        if (RP.roomUser.EatItem == 10)
                        {
                            int i = 5;
                            if (RP.roomUser.CarryItemId == 9 || RP.roomUser.CarryItemId == 8 || RP.roomUser.CarryItemId == 85)
                                i = 2;
                            if (RP.MaxEnergy + i >= 100)
                            {
                                RP.MaxEnergy = 100;
                                RP.roomUser.CarryItem(0);
                                RP.item = 0;
                            }
                            else
                                RP.MaxEnergy += i;
                            RP.roomUser.EatItem = 0;
                        }
                    }
                }
                else if (RP.roomUser.CarryItemId == 52)
                {
                    if ((RP.JobManager.Job == 6 && RP.JobManager.Working) || RP.onduty)
                    { }
                    else
                    {
                        RP.roomUser.EatItem++;
                        if (RP.roomUser.EatItem == 10)
                        {
                            if (RP.Hunger + 5 >= 100)
                                RP.HungerChange(100);
                            else
                                RP.HungerChange(RP.Hunger + 5);
                            RP.roomUser.CarryItem(0);
                            RP.item = 0;
                            if (RP.Health + 5 >= RP.MaxHealth)
                                RP.HealthChange(RP.MaxHealth);
                            else RP.HealthChange(RP.Health + 5);
                            RP.roomUser.EatItem = 0;
                            if (RP.Inventory.Currslot1 != "null" && RP.Inventory.Equip1 == "snack")
                            {
                                string currslot = RP.Inventory.Currslot1;
                                if (RP.Inventory.IsSlotEmpty(currslot))
                                {
                                    RP.WebHandler.Handle("equip", "", "e1");
                                    RP.Inventory.Additem(currslot, true);
                                }
                                else
                                {
                                    RP.Inventory.Additem(currslot, true);
                                    RP.WebHandler.Handle("equip", "", "e1");
                                }
                            }
                        }
                    }
                }
                else if (RP.roomUser.CarryItemId == 3)
                {
                    if ((RP.JobManager.Job == 6 && RP.JobManager.Working) || RP.onduty)
                    { }
                    else
                    {
                        RP.roomUser.EatItem++;
                        if (RP.roomUser.EatItem == 10)
                        {
                            if (RP.Hunger + 3 >= 100)
                                RP.HungerChange(100);
                            else
                                RP.HungerChange(RP.Hunger + 3);
                            RP.roomUser.CarryItem(0);
                            RP.item = 0;
                            RP.roomUser.EatItem = 0;
                            if (RP.Inventory.Currslot1 != "null" && RP.Inventory.Equip1 == "carrot")
                            {
                                string currslot = RP.Inventory.Currslot1;
                                if (RP.Inventory.IsSlotEmpty(currslot))
                                {
                                    RP.WebHandler.Handle("equip", "", "e1");
                                    RP.Inventory.Additem(currslot, true);
                                }
                                else
                                {
                                    RP.Inventory.Additem(currslot, true);
                                    RP.WebHandler.Handle("equip", "", "e1");
                                }
                            }
                        }
                    }
                }
                if (RP.Inventory.Equip1 == "weed")
                {
                    RP.roomUser.EatItem++;
                    if (RP.roomUser.EatItem == 10)
                    {
                        RP.roomUser.CarryItem(0);
                        RP.item = 0;
                        RP.roomUser.EatItem = 0;
                        if (RP.Hunger - 5 < 0)
                            RP.HungerChange(0);
                        else RP.HungerChange(RP.Hunger - 5);
                        if (RP.WeedTimer == 0)
                        {
                            RP.MaxStrength++;
                            RP.client.SendWhisper("You've gained 50% stamina recovery for a duration of 10 minutes!");
                            RP.WeedTimer = 600;
                            RP.WeedTime = 1800;
                        }
                        else RP.client.SendWhisper("To gain any benefits from this item, you must wait at least " + RP.WeedTimer / 60 + " minutes!");
                        if (RP.Inventory.Currslot1 != "null")
                        {
                            string currslot = RP.Inventory.Currslot1;
                            if (RP.Inventory.IsSlotEmpty(currslot))
                            {
                                RP.WebHandler.Handle("equip", "", "e1");
                                RP.Inventory.Additem(currslot, true);
                            }
                            else
                            {
                                RP.Inventory.Additem(currslot, true);
                                RP.WebHandler.Handle("equip", "", "e1");
                            }
                        }

                    }
                }





               

                if (RP.roomUser.CarrotID > 0)
                {
                    if (RP.roomUser.IsWalking)
                    {
                        foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                            if (item.Id == RP.roomUser.CarrotID)
                                item.Collect = false;
                        RP.roomUser.CarrotID = 0;
                        RP.roomUser.ApplyEffect(0);
                    }
                    RP.roomUser.CarrotTimer--;
                   
                    //RP.UpdateEnergy(1, 1);
                    RP.UpdateEnergy(1, 0);

                    if (RP.roomUser.CarrotTimer <= 0 && RP.roomUser.CarrotID > 0)
                    {
                        foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (item.Id == RP.roomUser.CarrotID)

                                RP.Room.GetRoomItemHandler().RemoveRoomItem(item, RP.habbo.Id);
                            RP.roomUser.CarrotID = 0;
                            RP.roomUser.CarrotTimer = 30;
                            RP.roomUser.ApplyEffect(0);
                            RP.XPSet(5);
                            RP.Say("harvest one (1) carrot", true, 4);
                            RP.Inventory.Additem("carrot");
                            item.UpdateState();
                            item.ExtraData = "1";
                        }

                    }
                    else
                        if (RP.roomUser.IsWalking && RP.roomUser.CarrotID > 0)
                    {
                        RP.roomUser.CarrotID = 0;
                        RP.CarrotTimer = 80;
                        RP.roomUser.ApplyEffect(0);

                        RP.Say("stops harvesting the carrot", true, 4);
                    }
                }
                
               



                // Rock Farming


                if (RP.roomUser.RockID > 0)
                {
                    if (RP.roomUser.IsWalking)
                    {
                        foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                            if (item.Id == RP.roomUser.RockID)
                                item.Collect = false;
                        RP.roomUser.RockID = 0;
                        RP.roomUser.ApplyEffect(0);
                    }
                    RP.roomUser.RockTimer++;
                    RP.UpdateEnergy(1, 1);
                    if (RP.roomUser.RockTimer >= 12)
                    {
                        foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                            if (item.Id == RP.roomUser.RockID)
                                RP.Room.GetRoomItemHandler().RemoveRoomItem(item, RP.habbo.Id);
                        RP.roomUser.RockID = 0;
                        RP.roomUser.RockTimer = 0;
                        RP.roomUser.ApplyEffect(0);
                        RP.XPSet(5);
                        RP.Say("completely shatters the stone", false);
                        if (RP.Jailed > 0 || RP.JailedSec > 0)
                        {
                            RP.Jailed -= PlusEnvironment.GetRandomNumber(1, 2);
                            if (RP.Jailed < 1)
                                RP.Jailed = 0;
                            RP.Timer(RP.Jailed, RP.JailedSec, "jail");
                        }
                        RP.Room.rocks--;
                    }
                }

                if (RP.InteractingItem > 0)
                {
                    RP.InteractingTimer++;
                    if (RP.InteractingTimer >= 15)
                    {
                        if (RP.InteractingItem == 20013)
                        {
                            if (PlusEnvironment.GetRandomNumber(1, 100) >= 85)
                            {
                                int money = PlusEnvironment.GetRandomNumber(1, 10);
                                RP.UpdateCredits(money, true);
                                if (money == 1)
                                    RP.Say("finds " + money + " dollar", true, 4);
                                else RP.Say("finds " + money + " dollars", true, 4);
                            }
                            else if (PlusEnvironment.GetRandomNumber(1, 100) >= 95)
                            {
                                string item = "";
                                int _item = PlusEnvironment.GetRandomNumber(1, 10);
                                if (_item == 1)
                                    item = "bat";
                                else if (_item == 2)
                                    item = "sword";
                                else if (_item == 3)
                                    item = "axe";
                                else if (_item == 4)
                                    item = "kevlar";
                                else if (_item == 5)
                                    item = "crowbar";
                                else if (_item == 6)
                                    item = "metal_pipe";
                                else if (_item == 7)
                                    item = "kevlar2";
                                else if (_item == 8)
                                    item = "kevlar3";
                                else if (_item == 9)
                                    item = "iron_bat";
                                else if (_item == 10)
                                    item = "chain_stick";
                                if (item.Contains("kevlar"))
                                    RP.Inventory.Additem(RP.AcceptOffer, false, true, 1, false, PlusEnvironment.GetRandomNumber(20, 100));
                                else RP.Inventory.Additem(item);
                                RP.Say("discovers a special item", false);
                            }
                            else
                                RP.Say("finds nothing", true, 4);
                            RP.roomUser.ApplyEffect(0);
                            RP.InteractingCD = 300;
                        }
                        RP.InteractingItem = 0;
                        RP.InteractingTimer = 0;

                    }
                    if (RP.roomUser.IsWalking && RP.InteractingItem > 0)
                    {
                        RP.InteractingItem = 0;
                        RP.InteractingTimer = 0;
                        RP.roomUser.ApplyEffect(0);
                        RP.Say("stops their search", false);
                        
                    }
                }

             


                // Farming Carrots



                // End Carrot

                if (RP.OfferTimer > 0)
                {
                    RP.OfferTimer--;
                    if ((RP.OfferTimer == 0 && RP.InteractID > 0) || RP.AutoLogout > 0)
                    {
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.InteractID);
                        user.GetRolePlay().InteractID = 0;
                        RP.InteractID = 0;
                        user.GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"cancel\"}");
                        RP.client.SendWhisper(user.GetHabbo().Username + " did not accept your offer within the time frame!");
                        RP.AcceptOffer = "null";
                    }
                }
                if (RP.TradeTimer > 0)
                {
                    RP.TradeTimer--;
                    if (RP.TradeTimer == 0 && RP.TradeTarget > 0)
                    {
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.TradeTarget);
                        user.GetRolePlay().TradeTarget = 0;
                        RP.TradeTarget = 0;
                        user.GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"cancel\"}");
                        RP.client.SendWhisper(user.GetHabbo().Username + " did not accept your request within the time frame!");
                        RP.AcceptOffer = "null";

                    }
                }
                if (RP.InteractATM > 0 && RP.roomUser.IsWalking)
                {
                    foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                    {
                        if (item.Id == RP.InteractATM)
                        {
                            item.ExtraData = "0";
                            item.UpdateState();
                        }
                    }

                }
                if (RP.Seed > 0)
                {
                    RP.SeedTimer++;
                    if (RP.SeedTimer >= 60)
                    {
                        RP.SeedTimer = 0;
                        Room room = null;
                        if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RP.PlantSeedRoom, out room))
                            return;
                        int id = PlusEnvironment.GetRandomNumber(6000, 800000);
                        Item seed = new Item(id + 1, RP.PlantSeedRoom, RP.Seed, "0", room.GetGameMap().Model.DoorX, room.GetGameMap().Model.DoorY, 0.0, 0, room.OwnerId, 0, 0, 0, string.Empty);
                        double Z = 0.0;
                        if (RP.Seed == 4552)
                            Z = 0.2;
                        foreach (Item item in room.GetRoomItemHandler().GetFloor.ToList())
                            if (item.Id == RP.PlantSeed)
                                if (room.GetRoomItemHandler().PlaceItem(seed, item.GetX, item.GetY, 0, true, false, true, false, Z))
                                    RP.client.SendWhisper("Your seed has been planted! double click it to add it to your inventory");
                                else RP.client.SendWhisper("Your seed did not plant! try not to stand on the dirt nest!");
                        RP.Seed = 0;
                        RP.PlantSeed = 0;
                        RP.PlantSeedRoom = 0;
                    }
                }
                if (RP.Spinning)
                {
                    RP.SpinTimer++;
                    if (RP.SpinTimer == 4)
                    {
                        int spin1 = 0;
                        int spin2 = 0;
                        int spin3 = 0;
                        int win = 0;
                        RP.SpinSlot++;
                        RP.SpinTimer = 0;
                        if (RP.SpinSlot == 1)
                        {
                            spin1 = 1;
                            Random r = new Random();
                            int[] priceArray1 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex1 = r.Next(priceArray1.Length);
                            int[] priceArray2 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex2 = r.Next(priceArray2.Length);
                            int[] priceArray3 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex3 = r.Next(priceArray3.Length);
                            RP.spin1_pic1 = priceArray1[randomIndex1];
                            RP.spin1_pic2 = priceArray2[randomIndex2];
                            RP.spin1_pic3 = priceArray3[randomIndex3];
                        }
                        else if (RP.SpinSlot == 2)
                        {
                            spin2 = 1;
                            Random r = new Random();
                            int[] priceArray1 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex1 = r.Next(priceArray1.Length);
                            int[] priceArray2 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex2 = r.Next(priceArray2.Length);
                            int[] priceArray3 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex3 = r.Next(priceArray3.Length);
                            RP.spin2_pic1 = priceArray1[randomIndex1];
                            RP.spin2_pic2 = priceArray2[randomIndex2];
                            RP.spin2_pic3 = priceArray3[randomIndex3];
                        }
                        else if (RP.SpinSlot == 3)
                        {
                            spin3 = 1;
                            Random r = new Random();
                            int[] priceArray1 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex1 = r.Next(priceArray1.Length);
                            int[] priceArray2 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex2 = r.Next(priceArray2.Length);
                            int[] priceArray3 = new int[] { 1, 5, 10, 20, 50, 100 };
                            int randomIndex3 = r.Next(priceArray3.Length);
                            RP.spin3_pic1 = priceArray1[randomIndex1];
                            RP.spin3_pic2 = priceArray2[randomIndex2];
                            RP.spin3_pic3 = priceArray3[randomIndex3];
                            int prize = 0;
                            RP.Spinning = false;
                            RP.SpinSlot = 0;
                            if (RP.spin3_pic1 == RP.spin2_pic1 && RP.spin3_pic1 == RP.spin1_pic1)
                            {
                                win = 1;
                                if (RP.spin1_pic1 == 1)
                                    prize += 10;
                                else if (RP.spin1_pic1 == 5)
                                    prize += 20;
                                else if (RP.spin1_pic1 == 10)
                                    prize += 50;
                                else if (RP.spin1_pic1 == 20)
                                    prize += 100;
                                else if (RP.spin1_pic1 == 50)
                                    prize += 250;
                                else if (RP.spin1_pic1 == 100)
                                    prize += 500;
                            }
                            if (RP.spin3_pic2 == RP.spin2_pic2 && RP.spin3_pic2 == RP.spin1_pic2)
                            {
                                win = 1;
                                if (RP.spin1_pic2 == 1)
                                    prize += 10;
                                else if (RP.spin1_pic2 == 5)
                                    prize += 20;
                                else if (RP.spin1_pic2 == 10)
                                    prize += 50;
                                else if (RP.spin1_pic2 == 20)
                                    prize += 100;
                                else if (RP.spin1_pic2 == 50)
                                    prize += 250;
                                else if (RP.spin1_pic2 == 100)
                                    prize += 500;
                            }
                            if (RP.spin3_pic3 == RP.spin2_pic3 && RP.spin3_pic3 == RP.spin1_pic3)
                            {
                                win = 1;
                                if (RP.spin1_pic3 == 1)
                                    prize += 10;
                                else if (RP.spin1_pic3 == 5)
                                    prize += 20;
                                else if (RP.spin1_pic3 == 10)
                                    prize += 50;
                                else if (RP.spin1_pic3 == 20)
                                    prize += 100;
                                else if (RP.spin1_pic3 == 50)
                                    prize += 250;
                                else if (RP.spin1_pic3 == 100)
                                    prize += 500;
                            }
                            if (prize > 0)
                            {
                                RP.Say("wins " + prize + " dollars", false);
                                RP.UpdateCredits(prize, true);
                            }
                            else RP.Say("wins nothing", false);
                        }
                        RP.SendWeb("{\"name\":\"slotspin\", \"spin1\":\"" + spin1 + "\","
                            + "\"spin1_pic1\":\"" + RP.spin1_pic1 + "\","
                    + "\"spin1_pic2\":\"" + RP.spin1_pic2 + "\", \"spin1_pic3\":\"" + RP.spin1_pic3 + "\","
                  + "\"spin2\":\"" + spin2 + "\", \"spin2_pic1\":\"" + RP.spin2_pic1 + "\","
                    + "\"spin2_pic2\":\"" + RP.spin2_pic2 + "\", \"spin2_pic3\":\"" + RP.spin2_pic3 + "\","
                    + "\"spin3\":\"" + spin3 + "\", \"spin3_pic1\":\"" + RP.spin3_pic1 + "\","
                    + "\"spin3_pic2\":\"" + RP.spin3_pic2 + "\", \"spin3_pic3\":\"" + RP.spin3_pic3 + "\", \"win\":\"" + win + "\"}");
                    }
                }
                #endregion





                if (CycleCount <= 0)
                {
                    CycleCount = 2;
                    Console.WriteLine("Yay!");
                    CycleCount = 0;
               
                    #region Timers
                    
                    if (RP.Cooldown > 0)
                        RP.Cooldown--;
                    if (RP.Cooldown2 > 0)
                        RP.Cooldown2--;
                    if (RP.Cooldown3 > 0)
                        RP.Cooldown3--;
                    if (RP.subCooldown > 0)
                        RP.subCooldown--;
                    if (RP.dubCooldown > 0)
                        Console.WriteLine("DubCoolDOwn is working!");
                    RP.dubCooldown--;
                    if (RP.InteractingCD > 0)
                        RP.InteractingCD--;
                    if (RP.ItemCD > 0)
                        RP.ItemCD--;
                    if (RP.atmCD > 0)
                        RP.atmCD--;
                    if (RP.WeedTimer > 0)
                        RP.WeedTimer--;
                    if (RP.fishingCD > 0)
                        RP.fishingCD--;
                    if (RP.FreeMoney > 0)
                    {
                        RP.FreeMoney--;
                        if (RP.FreeMoney == 0)
                        {
                            RP.FreeMoney = 600;
                            RP.UpdateCredits(4, true);
                        }
                    }
                    if (RP.WeedTime > 0)
                    {
                        RP.WeedTime--;
                        if (RP.WeedTime == 0)
                        {
                            RP.client.SendWhisper("Your temporary stamina recovery boost has worn off!");
                            RP.MaxStrength--;
                        }
                    }
                    if (RP.fishing > 0)
                    {
                        RP.fishing--;
                        if (RP.habbo.Effects().CurrentEffect != 593)
                            RP.roomUser.ApplyEffect(593);
                        if (RP.fishing == 0)
                        {
                            if (RP.Inventory.IsInventoryFull("fish"))
                                RP.client.SendWhisper("Your inventory is currently full!");
                            else
                            {
                                var curslot = RP.Inventory.Currslot1;
                                RP.Inventory.ItemHealth("true", 2);
                                if (RP.Inventory.Equip1 != "null")
                                    RP.WebHandler.Handle("equip", "", "e1");
                                int fishcount = PlusEnvironment.GetRandomNumber(1, 3);
                                RP.Inventory.Additem("fish", false, true, fishcount);
                                RP.Say("reels in their fishing rod, discovering " + fishcount + " fish(es)", false);
                                RP.ItemCD = 5;
                                RP.WebHandler.Handle("equip", "", curslot);
                            }                        
                        }
                        if (RP.Cuffed || RP.Jailed > 0 || RP.JailedSec > 0
                              || RP.Health < 1 || RP.roomUser.IsWalking || RP.roomUser.Stunned > 0
                              || RP.Inventory.Equip1 != "fishing_rod" || RP.fishing == 0)
                        {
                            RP.fishing = 0;
                            RP.fishingCD = 15;
                            RP.roomUser.ApplyEffect(0);
                        }
                    }
                    if (RP.AcceptTimer > 0)
                    {
                        RP.AcceptTimer--;
                        if (RP.AcceptTimer == 0)
                            RP.AcceptOffer = "null";
                    }
                    if (RP.GP > 0)
                    {
                        RP.GPTimer++;
                        if (RP.GPTimer >= 60)
                        {
                            RP.GPTimer = 0;
                            RP.GP--;
                            if (RP.GP <= 0) { }
                                //RP.GetClient().SendNotifi("You are no longer under God Protection, good luck!");
                            else RP.GetClient().SendWhisper("You have " + RP.GP + " minute(s) left until you lose your god protection!");
                            RP.RPCache(19);
                        }
                    }
                    if (RP.TempEffect > 0)
                    {
                        RP.TempEffect--;
                        if (RP.TempEffect == 0)
                            RP.roomUser.ApplyEffect(0);
                        }
                    if (RP.CallPoliceDelay > 0)
                    {
                        RP.CallPoliceDelay--;
                        if (RP.CallPoliceDelay == 0)
                            RP.Room.PlacePolice(RP.callroomid);
                    }
                    if (RP.AutoLogout > 0)
                    {
                        RP.AutoLogout--;
                        if (RP.AutoLogout == 0)
                        {
                            if (RP.lockedon > 0)
                            {
                                foreach (GameClient User in RP.habbo.GetClientManager()._clients.Values.ToList())
                                {
                                    if (User.GetRolePlay().lockID == RP.habbo.Id)
                                    {
                                        User.GetRolePlay().lockID = 0;
                                        if (User.GetRolePlay().lockTarget == RP.habbo.Id)
                                        {
                                            User.GetRolePlay().SendWeb("{\"name\":\"lock\", \"lock\":\"true\"}");
                                            User.GetRolePlay().lockTarget = 0;
                                        }
                                    }
                                }
                            }
                            RP.client.Dispose(false);
                        }
                    }
                    if (RP.achievetimer > 0)
                    {
                        RP.achievetimer--;
                        if (RP.achievetimer == 0)
                            RP.SendWeb("{\"name\":\"achievetimer\"}");

                    }
                    if (RP.xpdelay > 0)
                    {
                        RP.xpdelay--;
                        if (RP.xpdelay == 0)
                        {
                            RP.XPSet(RP.xpsave);
                            RP.xpsave = 0;
                        }
                    }
                    if (RP.roomUser.delay > 0)
                    {
                        RP.roomUser.delay--;
                        if (RP.roomUser.delay == 0)
                            RP.RoomForward(RP.roomUser.delayroom);
                    }
                    if (RP.JobManager.Sendhome > 0)
                        RP.JobManager.Sendhome--;
                    if (RP.TurfTime > 0)
                    {
                        RP.TurfTimer++;
                        if (RP.TurfTimer == 60)
                        {
                            RP.TurfTimer = 0;
                            RP.TurfTime--;
                            if (RP.TurfTime == 0)
                            {
                                RP.Say("successfully claims " + RP.Room.Name + "", false);
                                if (RP.Room.Name.Contains("North"))
                                {
                                    RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "North Turf", RP.habbo.GetClientManager().NorthTurf);
                                    RP.habbo.GetClientManager().SetTurfs(1, RP.Gang, RP.GangManager.Name);
                                }
                                else if (RP.Room.Name.Contains("East"))
                                {
                                    RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "East Turf", RP.habbo.GetClientManager().EastTurf);
                                    RP.habbo.GetClientManager().SetTurfs(2, RP.Gang, RP.GangManager.Name);
                                }
                                else if (RP.Room.Name.Contains("West"))
                                {
                                    RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "West Turf", RP.habbo.GetClientManager().WestTurf);
                                    RP.habbo.GetClientManager().SetTurfs(3, RP.Gang, RP.GangManager.Name);
                                }
                                else if (RP.Room.Name.Contains("South"))
                                {
                                    RP.habbo.GetClientManager().GlobalGang(0, "Your turf has been claimed by " + RP.GangManager.Name + "", "South Turf", RP.habbo.GetClientManager().SouthTurf);
                                    RP.habbo.GetClientManager().SetTurfs(4, RP.Gang, RP.GangManager.Name);
                                }
                                RP.XPSet(100);
                                RP.captures++;
                            }
                            else
                            {
                                if (RP.TurfTime == 1)
                                    RP.Say("" + RP.TurfTime + " minute left to claim this turf", false);
                                else RP.Say("" + RP.TurfTime + " minutes left to claim this turf", false);
                            }
                        }
                        if (RP.roomUser.IsWalking || RP.Health < 1)
                        {
                            RP.TurfTime = 0;
                            RP.TurfTimer = 0;
                            RP.Say("stops claiming this turf", false);
                        }

                    }
                    if (RP.EscortID > 0)
                    {
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.EscortID);
                        if (user.GetRolePlay().Room.Id != RP.Room.Id && (user.GetHabbo().roomUser.X > 0 || user.GetHabbo().roomUser.Y > 0))
                            RP.RoomForward(user.GetRolePlay().Room.Id);
                    }
                    #endregion

                }
            }
            catch { }
        }
    }
}
