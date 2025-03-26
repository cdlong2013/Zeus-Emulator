using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.Rooms;
using System.Text.RegularExpressions;

namespace Plus.RolePlay.Trade
{
    public class ItemTrade
    {
        #region Trade Values
        internal bool Trading;
        internal int TradeTimer;
        internal bool TradeReady;
        internal int TradeMoney;
        internal string Slot1 = "null";
        internal string Slot2 = "null";
        internal string Slot3 = "null";
        internal string Slot4 = "null";
        internal string Slot5 = "null";
        internal string Slot6 = "null";
        internal int Quantity1;
        internal int Quantity2;
        internal int Quantity3;
        internal int Quantity4;
        internal int Quantity5;
        internal int Quantity6;
        internal int HP1;
        internal int HP2;
        internal int HP3;
        internal int HP4;
        internal int HP5;
        internal int HP6;
        #endregion

        RPData RP;

        public ItemTrade(RPData RP)
        {
            this.RP = RP;
        }

        public void StartTrade(RoomUser TargetUser)
        {
            if (TargetUser == RP.roomUser)
                return;
            if (RP.EscortID > 0 || RP.Escorting > 0 || TargetUser.GetClient().GetRolePlay().Escorting > 0 || TargetUser.GetClient().GetRolePlay().EscortID > 0)
            {
                RP.client.SendWhisper("This aciton can not be performed at this time!");
                return;
            }
            if (RP.habbo.TradingLockExpiry > 0)
            {
                if (RP.habbo.TradingLockExpiry > PlusEnvironment.GetUnixTimestamp())
                {
                    RP.client.SendNotification("You're currently banned from trading.");
                    return;
                }
                else
                {
                    RP.client.GetHabbo().TradingLockExpiry = 0;
                    RP.client.SendNotification("Your trading ban has now expired.");

                    using (var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + RP.client.GetHabbo().Id + "' LIMIT 1");
                }
            }

            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                return;
            if (!RP.habbo.GetPermissions().HasRight("room_trade_override"))
            {
                if (RP.Room.TradeSettings == 1 && RP.Room.OwnerId != RP.habbo.Id)
                {
                    RP.client.SendWhisper("Only the owner if this room can trade!");
                    return;
                }
                else if (RP.Room.TradeSettings == 0 && RP.Room.OwnerId != RP.habbo.Id)
                {
                    RP.client.SendWhisper("trading is disabled in this room!");
                    return;
                }
            }
            if (TargetUser.GetClient().GetHabbo().TradingLockExpiry > 0)
            {
                RP.client.SendNotification("Oops, it appears this user is currently banned from trading!");
                return;
            }
            if (RP.enable_trade == 0)
            {
                RP.client.SendWhisper("You need to enable trading before performing this action!");
                return;
            }
            if (TargetUser.GetClient().GetRolePlay().enable_trade == 0)
            {
                RP.client.SendWhisper("This user has disabled trading!");
                return;
            }
            if (RP.TradeTarget == TargetUser.HabboId)
            {
                RP.client.SendWhisper("You have already sent a trade request to this user!");
                return;
            }
            if (TargetUser.GetClient().GetRolePlay().Trade.Trading || TargetUser.GetClient().GetRolePlay().TradeTarget > 0 || TargetUser.IsTrading)
            {
                RP.client.SendWhisper("This action can not be performed while user is trading!");
                return;
            }
            if (Trading)
            {
                RP.client.SendWhisper("This action can not be performed while trading!");
                return;
            }
            RP.TradeTarget = TargetUser.HabboId;
            RP.SendWeb("{\"name\":\"trade\"}");
        }      

        public bool IsTradeValid(ItemTrade Trade)
        {
            string slot1 = RP.Inventory.Item1;
            string slot2 = RP.Inventory.Item2;
            string slot3 = RP.Inventory.Item3;
            string slot4 = RP.Inventory.Item4;
            string slot5 = RP.Inventory.Item5;
            string slot6 = RP.Inventory.Item6;
            string slot7 = RP.Inventory.Item7;
            string slot8 = RP.Inventory.Item8;
            string slot9 = RP.Inventory.Item9;
            string slot10 = RP.Inventory.Item10;
            string slot11 = RP.Inventory.Item11;
            string slot12 = RP.Inventory.Item12;
            string tslot1 = Trade.Slot1;
            string tslot2 = Trade.Slot2;
            string tslot3 = Trade.Slot3;
            string tslot4 = Trade.Slot4;
            string tslot5 = Trade.Slot5;
            string tslot6 = Trade.Slot6;
            int check1 = 0;
            int check2 = 0;
            int check3 = 0;
            int check4 = 0;
            int check5 = 0;
            int check6 = 0;
            if (Trade.Slot1 == "null") // dont need to add
                check1 = 1;
            if (Trade.Slot2 == "null")
                check2 = 1;
            if (Trade.Slot3 == "null")
                check3 = 1;
            if (Trade.Slot4 == "null")
                check4 = 1;
            if (Trade.Slot5 == "null")
                check5 = 1;
            if (Trade.Slot6 == "null")
                check6 = 1;
            if (IsTradeValid2(Trade.Slot1)) // add to occupied slot
                check1 = 1;
            if (IsTradeValid2(Trade.Slot2))
                check2 = 1;
            if (IsTradeValid2(Trade.Slot3))
                check3 = 1;
            if (IsTradeValid2(Trade.Slot4))
                check4 = 1;
            if (IsTradeValid2(Trade.Slot5))
                check5 = 1;
            if (IsTradeValid2(Trade.Slot6))
                check6 = 1;
            if (check1 == 0) // add to empty slot
            {
                if (slot1 == "null")
                {
                    check1 = 1;
                    slot1 = tslot1;
                }
                else if (slot2 == "null")
                {
                    check1 = 1;
                    slot2 = tslot1;
                }
                else if (slot3 == "null")
                {
                    check1 = 1;
                    slot3 = tslot1;
                }
                else if (slot4 == "null")
                {
                    check1 = 1;
                    slot4 = tslot1;
                }
                else if (slot5 == "null")
                {
                    check1 = 1;
                    slot5 = tslot1;
                }
                else if (slot6 == "null")
                {
                    check1 = 1;
                    slot6 = tslot1;
                }
                else if (slot7 == "null")
                {
                    check1 = 1;
                    slot7 = tslot1;
                }
                else if (slot8 == "null")
                {
                    check1 = 1;
                    slot8 = tslot1;
                }
                else if (slot9 == "null")
                {
                    check1 = 1;
                    slot9 = tslot1;
                }
                else if (slot10 == "null")
                {
                    check1 = 1;
                    slot10 = tslot1;
                }
                else if (slot11 == "null")
                {
                    check1 = 1;
                    slot11 = tslot1;
                }
                else if (slot12 == "null")
                {
                    check1 = 1;
                    slot12 = tslot1;
                }
            }
            if (check2 == 0)
            {
                if (slot1 == "null")
                {
                    check2 = 1;
                    slot1 = tslot2;
                }
                else if (slot2 == "null")
                {
                    check2 = 1;
                    slot2 = tslot2;
                }
                else if (slot3 == "null")
                {
                    check2 = 1;
                    slot3 = tslot2;
                }
                else if (slot4 == "null")
                {
                    check2 = 1;
                    slot4 = tslot2;
                }
                else if (slot5 == "null")
                {
                    check2 = 1;
                    slot5 = tslot2;
                }
                else if (slot6 == "null")
                {
                    check2 = 1;
                    slot6 = tslot2;
                }
                else if (slot7 == "null")
                {
                    check2 = 1;
                    slot7 = tslot2;
                }
                else if (slot8 == "null")
                {
                    check2 = 1;
                    slot8 = tslot2;
                }
                else if (slot9 == "null")
                {
                    check2 = 1;
                    slot9 = tslot2;
                }
                else if (slot10 == "null")
                {
                    check2 = 1;
                    slot10 = tslot2;
                }
                else if (slot11 == "null")
                {
                    check2 = 1;
                    slot11 = tslot2;
                }
                else if (slot12 == "null")
                {
                    check2 = 1;
                    slot12 = tslot2;
                }
            }
            if (check3 == 0)
            {
                if (slot1 == "null")
                {
                    check3 = 1;
                    slot1 = tslot3;
                }
                else if (slot2 == "null")
                {
                    check3 = 1;
                    slot2 = tslot3;
                }
                else if (slot3 == "null")
                {
                    check3 = 1;
                    slot3 = tslot3;
                }
                else if (slot4 == "null")
                {
                    check3 = 1;
                    slot4 = tslot3;
                }
                else if (slot5 == "null")
                {
                    check3 = 1;
                    slot5 = tslot3;
                }
                else if (slot6 == "null")
                {
                    check3 = 1;
                    slot6 = tslot3;
                }
                else if (slot7 == "null")
                {
                    check3 = 1;
                    slot7 = tslot3;
                }
                else if (slot8 == "null")
                {
                    check3 = 1;
                    slot8 = tslot3;
                }
                else if (slot9 == "null")
                {
                    check3 = 1;
                    slot9 = tslot3;
                }
                else if (slot10 == "null")
                {
                    check3 = 1;
                    slot10 = tslot3;
                }
                else if (slot11 == "null")
                {
                    check3 = 1;
                    slot11 = tslot3;
                }
                else if (slot12 == "null")
                {
                    check3 = 1;
                    slot12 = tslot3;
                }

            }
            if (check4 == 0)
            {
                if (slot1 == "null")
                {
                    check4 = 1;
                    slot1 = tslot4;
                }
                else if (slot2 == "null")
                {
                    check4 = 1;
                    slot2 = tslot4;
                }
                else if (slot3 == "null")
                {
                    check4 = 1;
                    slot3 = tslot4;
                }
                else if (slot4 == "null")
                {
                    check4 = 1;
                    slot4 = tslot4;
                }
                else if (slot5 == "null")
                {
                    check4 = 1;
                    slot5 = tslot4;
                }
                else if (slot6 == "null")
                {
                    check4 = 1;
                    slot6 = tslot4;
                }
                else if (slot7 == "null")
                {
                    check4 = 1;
                    slot7 = tslot4;
                }
                else if (slot8 == "null")
                {
                    check4 = 1;
                    slot8 = tslot4;
                }
                else if (slot9 == "null")
                {
                    check4 = 1;
                    slot9 = tslot4;
                }
                else if (slot10 == "null")
                {
                    check4 = 1;
                    slot10 = tslot4;
                }
                else if (slot11 == "null")
                {
                    check4 = 1;
                    slot11 = tslot4;
                }
                else if (slot12 == "null")
                {
                    check4 = 1;
                    slot12 = tslot4;
                }
            }
            if (check5 == 0)
            {
                if (slot1 == "null")
                {
                    check5 = 1;
                    slot1 = tslot5;
                }
                else if (slot2 == "null")
                {
                    check5 = 1;
                    slot2 = tslot5;
                }
                else if (slot3 == "null")
                {
                    check5 = 1;
                    slot3 = tslot5;
                }
                else if (slot4 == "null")
                {
                    check5 = 1;
                    slot4 = tslot5;
                }
                else if (slot5 == "null")
                {
                    check5 = 1;
                    slot5 = tslot5;
                }
                else if (slot6 == "null")
                {
                    check5 = 1;
                    slot6 = tslot5;
                }
                else if (slot7 == "null")
                {
                    check5 = 1;
                    slot7 = tslot5;
                }
                else if (slot8 == "null")
                {
                    check5 = 1;
                    slot8 = tslot5;
                }
                else if (slot9 == "null")
                {
                    check5 = 1;
                    slot9 = tslot5;
                }
                else if (slot10 == "null")
                {
                    check5 = 1;
                    slot10 = tslot5;
                }
                else if (slot11 == "null")
                {
                    check5 = 1;
                    slot11 = tslot5;
                }
                else if (slot12 == "null")
                {
                    check5 = 1;
                    slot12 = tslot5;
                }
            }
            if (check6 == 0)
            {
                if (slot1 == "null")
                {
                    check6 = 1;
                    slot1 = tslot6;
                }
                else if (slot2 == "null")
                {
                    check6 = 1;
                    slot2 = tslot6;
                }
                else if (slot3 == "null")
                {
                    check6 = 1;
                    slot3 = tslot6;
                }
                else if (slot4 == "null")
                {
                    check6 = 1;
                    slot4 = tslot6;
                }
                else if (slot5 == "null")
                {
                    check6 = 1;
                    slot5 = tslot6;
                }
                else if (slot6 == "null")
                {
                    check6 = 1;
                    slot6 = tslot6;
                }
                else if (slot7 == "null")
                {
                    check6 = 1;
                    slot7 = tslot6;
                }
                else if (slot8 == "null")
                {
                    check6 = 1;
                    slot8 = tslot6;
                }
                else if (slot9 == "null")
                {
                    check6 = 1;
                    slot9 = tslot6;
                }
                else if (slot10 == "null")
                {
                    check6 = 1;
                    slot10 = tslot6;
                }
                else if (slot11 == "null")
                {
                    check6 = 1;
                    slot11 = tslot6;
                }
                else if (slot12 == "null")
                {
                    check6 = 1;
                    slot12 = tslot6;
                }
            }
            if (check1 == 1 && check2 == 1 && check3 == 1 &&
                check4 == 1 && check5 == 1 && check6 == 1)
                return true;
            else return false;
        }

        public bool IsTradeValid2(string Slot)
        {
            if (Slot == "null" || !RP.Inventory.CheckItem(Slot))
                return false;
            if (Slot == RP.Inventory.Item1 || Slot == RP.Inventory.Item2 ||
                Slot == RP.Inventory.Item3 || Slot == RP.Inventory.Item4 ||
                Slot == RP.Inventory.Item5 || Slot == RP.Inventory.Item6 ||
                Slot == RP.Inventory.Item7 || Slot == RP.Inventory.Item8 ||
                Slot == RP.Inventory.Item9 || Slot == RP.Inventory.Item10 ||
                Slot == RP.Inventory.Item11 || Slot == RP.Inventory.Item12)
                return true;
            return false;
        }

        public bool IsTradeEmpty(ItemTrade Trade)
        {
            if (Trade.Slot1 == "null" && Trade.Slot2 == "null" &&
                Trade.Slot3 == "null" && Trade.Slot4 == "null" &&
                Trade.Slot5 == "null" && Trade.Slot6 == "null" &&
                Slot1 == "null" && Slot2 == "null" &&
                Slot3 == "null" && Slot4 == "null" &&
                Slot5 == "null" && Slot6 == "null" &&
                TradeMoney == 0 && Trade.TradeMoney == 0)
                return true;
            else return false;
        }

        public void InitTrade(string slot)
        {
            if (slot == "w1" && RP.Inventory.Quantity1 > 0)
                Trade(RP.Inventory.Item1, RP.Inventory.HP1, slot);
            else if (slot == "w2" && RP.Inventory.Quantity2 > 0)
                Trade(RP.Inventory.Item2, RP.Inventory.HP2, slot);
            else if (slot == "w3" && RP.Inventory.Quantity3 > 0)
                Trade(RP.Inventory.Item3, RP.Inventory.HP3, slot);
            else if (slot == "w4" && RP.Inventory.Quantity4 > 0)
                Trade(RP.Inventory.Item4, RP.Inventory.HP4, slot);
            else if (slot == "w5" && RP.Inventory.Quantity5 > 0)
                Trade(RP.Inventory.Item5, RP.Inventory.HP5, slot);
            else if (slot == "w6" && RP.Inventory.Quantity6 > 0)
                Trade(RP.Inventory.Item6, RP.Inventory.HP6, slot);
            else if (slot == "w7" && RP.Inventory.Quantity7 > 0)
                Trade(RP.Inventory.Item7, RP.Inventory.HP7, slot);
            else if (slot == "w8" && RP.Inventory.Quantity8 > 0)
                Trade(RP.Inventory.Item8, RP.Inventory.HP8, slot);
            else if (slot == "w9" && RP.Inventory.Quantity9 > 0)
                Trade(RP.Inventory.Item9, RP.Inventory.HP9, slot);
            else if (slot == "w10" && RP.Inventory.Quantity10 > 0)
                Trade(RP.Inventory.Item10, RP.Inventory.HP10, slot);
            else if (slot == "w11" && RP.Inventory.Quantity11 > 0)
                Trade(RP.Inventory.Item11, RP.Inventory.HP11, slot);
            else if (slot == "w12" && RP.Inventory.Quantity12 > 0)
                Trade(RP.Inventory.Item12, RP.Inventory.HP12, slot);
        }

        public void Trade(string wep, int hp, string wslot = "", bool trade2 = false, string slot = "", int quantity = 0)
        {
            if (TradeReady && !trade2)
                return;
            if (wep.Contains("stun"))
            {
                RP.client.SendWhisper("You can not trade this item!");
                return;
            }
            if (!trade2)
            {
                #region trade 1
                if (Slot1 == "null" || (Slot1 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity1 < 50))
                {
                    Slot1 = wep;
                    HP1 = hp;
                    Quantity1 += 1;
                    quantity = Quantity1;
                    slot = "w1";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else if (Slot2 == "null" || (Slot2 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity2 < 50))
                {
                    Slot2 = wep;
                    HP2 = hp;
                    Quantity2 += 1;
                    quantity = Quantity2;
                    slot = "w2";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else if (Slot3 == "null" || (Slot3 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity3 < 50))
                {
                    Slot3 = wep;
                    HP3 = hp;
                    Quantity3 += 1;
                    quantity = Quantity3;
                    slot = "w3";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else if (Slot4 == "null" || (Slot4 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity4 < 50))
                {
                    Slot4 = wep;
                    HP4 = hp;
                    Quantity4 += 1;
                    quantity = Quantity4;
                    slot = "w4";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else if (Slot5 == "null" || (Slot5 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity5 < 50))
                {
                    Slot5 = wep;
                    HP5 = hp;
                    Quantity5 += 1;
                    quantity = Quantity5;
                    slot = "w5";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else if (Slot6 == "null" || (Slot6 == wep &&
                    RP.Inventory.CheckItem(wep) && Quantity6 < 50))
                {
                    Slot6 = wep;
                    HP6 = hp;
                    Quantity6 += 1;
                    quantity = Quantity6;
                    slot = "w6";
                    RP.Inventory.Additem(wslot, true, true, 1, true);
                }
                else return;
                #endregion
            }
            else if (trade2)
            {
                #region trade 2
                if (slot == "w1")
                    slot = "w7";
                else if (slot == "w2")
                    slot = "w8";
                else if (slot == "w3")
                    slot = "w9";
                else if (slot == "w4")
                    slot = "w10";
                else if (slot == "w5")
                    slot = "w11";
                else if (slot == "w6")
                    slot = "w12";
                #endregion
            }
            string num = Regex.Replace(slot, "w", "").Trim();
            int temphp = 100;
            if (wep == "kevlar2")
                temphp = 150;
            else if (wep == "kevlar3")
                temphp = 200;
            else if (wep == "kevlar4")
                temphp = 350;
            RP.SendWeb("{\"name\":\"trade_equip\", \"slot\":\"" + num + "\", \"wep\":\"" + wep + "\", \"hp\":\"" + (temphp - hp) + "\", \"maxhp\":\"" + temphp + "\", \"quantity\":\"" + quantity + "\"}");
            if (!trade2)
            {
                var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                User.GetClient().GetRolePlay().Trade.Trade(wep, hp, "", true, slot, quantity);
                if (User.GetClient().GetRolePlay().Trade.TradeReady)
                {
                    User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"3\"}");
                    RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"4\"}");
                    User.GetClient().GetRolePlay().Trade.TradeReady = false;
                }
            }
        }

        public void OpenTrade(RoomUser User)
        {
            RP.SendWeb("{\"name\":\"opentrade\", \"name2\":\"" + User.GetUsername() + "'s offer:\"}");
            User.GetClient().GetRolePlay().SendWeb("{\"name\":\"opentrade\", \"name2\":\"" + RP.habbo.Username + "'s offer:\"}");
        }

        public bool ReturnItem(string Item, int Quantity)
        {
           if (Item != "null" && RP.Inventory.CheckItem(Item))
            {
                if (Item == RP.Inventory.Item1)
                    RP.Inventory.Quantity1 += Quantity;
                else if (Item == RP.Inventory.Item2)
                    RP.Inventory.Quantity2 += Quantity;
                else if (Item == RP.Inventory.Item3)
                    RP.Inventory.Quantity3 += Quantity;
                else if (Item == RP.Inventory.Item4)
                    RP.Inventory.Quantity4 += Quantity;
                else if (Item == RP.Inventory.Item5)
                    RP.Inventory.Quantity5 += Quantity;
                else if (Item == RP.Inventory.Item6)
                    RP.Inventory.Quantity6 += Quantity;
                else if (Item == RP.Inventory.Item7)
                    RP.Inventory.Quantity7 += Quantity;
                else if (Item == RP.Inventory.Item8)
                    RP.Inventory.Quantity8 += Quantity;
                else if (Item == RP.Inventory.Item9)
                    RP.Inventory.Quantity9 += Quantity;
                else if (Item == RP.Inventory.Item10)
                    RP.Inventory.Quantity10 += Quantity;
                else if (Item == RP.Inventory.Item11)
                    RP.Inventory.Quantity11 += Quantity;
                else if (Item == RP.Inventory.Item12)
                    RP.Inventory.Quantity12 += Quantity;
                else return false;
                return true;
            }
            return false;
        }

        public void ReturnItem(string Item, int Quantity, int HP)
        {
            if (Item == "null")
                return;

            if (RP.Inventory.Item1 == "null")
            {
                RP.Inventory.Item1 = Item;
                RP.Inventory.Quantity1 = Quantity;
                RP.Inventory.HP1 = HP;
            }
            else if (RP.Inventory.Item2 == "null")
            {
                RP.Inventory.Item2 = Item;
                RP.Inventory.Quantity2 = Quantity;
                RP.Inventory.HP2 = HP;
            }
            else if (RP.Inventory.Item3 == "null")
            {
                RP.Inventory.Item3 = Item;
                RP.Inventory.Quantity3 = Quantity;
                RP.Inventory.HP3 = HP;
            }
            else if (RP.Inventory.Item4 == "null")
            {
                RP.Inventory.Item4 = Item;
                RP.Inventory.Quantity4 = Quantity;
                RP.Inventory.HP4 = HP;
            }
            else if (RP.Inventory.Item5 == "null")
            {
                RP.Inventory.Item5 = Item;
                RP.Inventory.Quantity5 = Quantity;
                RP.Inventory.HP5 = HP;
            }
            else if (RP.Inventory.Item6 == "null")
            {
                RP.Inventory.Item6 = Item;
                RP.Inventory.Quantity6 = Quantity;
                RP.Inventory.HP6 = HP;
            }
            else if (RP.Inventory.Item7 == "null")
            {
                RP.Inventory.Item7 = Item;
                RP.Inventory.Quantity7 = Quantity;
                RP.Inventory.HP7 = HP;
            }
            else if (RP.Inventory.Item8 == "null")
            {
                RP.Inventory.Item8 = Item;
                RP.Inventory.Quantity8 = Quantity;
                RP.Inventory.HP8 = HP;
            }
            else if (RP.Inventory.Item9 == "null")
            {
                RP.Inventory.Item9 = Item;
                RP.Inventory.Quantity9 = Quantity;
                RP.Inventory.HP9 = HP;
            }
            else if (RP.Inventory.Item10 == "null")
            {
                RP.Inventory.Item10 = Item;
                RP.Inventory.Quantity10 = Quantity;
                RP.Inventory.HP10 = HP;
            }
            else if (RP.Inventory.Item11 == "null")
            {
                RP.Inventory.Item11 = Item;
                RP.Inventory.Quantity11 = Quantity;
                RP.Inventory.HP1 = HP;
            }
            else if (RP.Inventory.Item12 == "null")
            {
                RP.Inventory.Item12 = Item;
                RP.Inventory.Quantity12 = Quantity;
                RP.Inventory.HP1 = HP;
            }
        }

        public void ClearSlot()
        {
            Slot1 = "null";
            Slot2 = "null";
            Slot3 = "null";
            Slot4 = "null";
            Slot5 = "null";
            Slot6 = "null";
            Quantity1 = 0;
            Quantity2 = 0;
            Quantity3 = 0;
            Quantity4 = 0;
            Quantity5 = 0;
            Quantity6 = 0;
        }

        public void StopTrade()
        {
            RP.SendWeb("{\"name\":\"cancel_trade\"}");
            if (!ReturnItem(Slot1, Quantity1))
                ReturnItem(Slot1, Quantity1, HP1);
            if (!ReturnItem(Slot2, Quantity2))
                ReturnItem(Slot2, Quantity2, HP2);
            if (!ReturnItem(Slot3, Quantity3))
                ReturnItem(Slot3, Quantity3, HP3);
            if (!ReturnItem(Slot4, Quantity4))
                ReturnItem(Slot4, Quantity4, HP4);
            if (!ReturnItem(Slot5, Quantity5))
                ReturnItem(Slot5, Quantity5, HP5);
            if (!ReturnItem(Slot6, Quantity6))
                ReturnItem(Slot6, Quantity6, HP6);
            if (TradeMoney > 0)
                RP.UpdateCredits(TradeMoney, true);
            TradeReady = false;
            Trading = false;
            TradeMoney = 0;
            ClearSlot();
            RP.TradeTarget = 0;
            RP.Inventory.ItemCache(0, true);
            RP.LoadStats(true);

        }
    }
}
