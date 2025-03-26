using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.GameClients;
using System.Text.RegularExpressions;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.RolePlay;

namespace Plus.RolePlay.WebHandle
{
    public class WebHandler
    {
        RPData RP;

        internal string CurItemDrag;
        internal bool DeletingItem;
        internal string ItemType;

        public WebHandler(RPData RP)
        {
            this.RP = RP;
        }

        public void Handle(string Event, string Data, string ExtraData)
        {
            if (RP.AutoLogout > 0)
                return;
            if (Event == "select_stun")
                SelectStun(Event, Data, ExtraData);
            else if (Event == "acceptitem")
                AcceptOffer(Event, Data, ExtraData);
            else if (Event == "iteminfo")
                ItemInfo(Event, Data, ExtraData);
            else if (Event == "equip")
                Equip(Event, Data, ExtraData);
            else if (Event == "itemdrag" || Event == "itemposition")
                ItemDrag(Event, Data, ExtraData);
            else if (Event == "bio" || Event == "statsrequest")
                Profile(Event, Data, ExtraData);
            else if (Event == "lock" || Event == "closetarget")
                LockTarget(Event, Data, ExtraData);
            else if (Event == "selectmeal")
                SelectMeal(Event, Data, ExtraData);
            else if (Event.Contains("911"))
                PoliceCall(Event, Data, ExtraData);
            else if (Event.Contains("color"))
                Color(Event, Data, ExtraData);
            else if (Event.Contains("atm"))
                ATM(Event, Data, ExtraData);
            else if (Event == "spin")
                SlotMachine(Event, Data, ExtraData);
            else if (Event.Contains("seed"))
                Farm(Event, Data, ExtraData);
            else if (Event.Contains("wl"))
                WantedList(Event, Data, ExtraData);
            else if (Event.Contains("timer"))
                Timer(Event, Data, ExtraData);
            else if (Event == "trash")
                TrashWep(Event, Data, ExtraData);
            else if (Event == "add_money" || Event.Contains("trade"))
                Trade(Event, Data, ExtraData);
            else if (Event == "enable")
                Enable(Event, Data, ExtraData);
            else if (Event == "vault")
                Vault(Event, Data, ExtraData);
            else if (Event.Contains("storage"))
                Storage(Event, Data, ExtraData);
            else if (Event.Contains("deleteitem"))
                ItemDelete(Event, Data, ExtraData);
        }

        public void AcceptOffer(string Event, string Data, string ExtraData)
        {
            if (Event == "acceptitem")
            {
                var user2 = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.InteractID);
                if (user2 == null && RP.InteractID > 0)
                {
                    RP.client.SendWhisper("This user is offline!");
                    RP.InteractID = 0;
                    return;
                }
                else if (RP.TradeTarget > 0)
                {
                    var user3 = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                    if (user3 == null)
                    {
                        RP.client.SendWhisper("This user is offline!");
                        RP.TradeTarget = 0;
                        return;
                    }
                }
                RP.OfferTimer = 0;
                if (RP.Trade.Trading)
                {
                    if (RP.AcceptOffer != "seed")
                    {
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.InteractID).GetRolePlay();
                        user.InteractID = 0;
                        RP.InteractID = 0;
                        RP.Say("declines " + user.habbo.Username + "'s offer", false);
                    }
                    RP.AcceptOffer = "null";
                }
                if (RP.AcceptOffer == "null")
                    return;
                if (Data == "1")
                {
                    if ((RP.Inventory.Equip1 != "null" || RP.Inventory.Equip2 != "null") && RP.AcceptOffer != "ticket"
                        && RP.AcceptOffer != "storage" && RP.AcceptOffer != "marry")
                    {
                        if (RP.Inventory.Equip2.Contains("kevlar") && RP.AcceptOffer != "drink")
                            Handle("equip", "", "e2");
                        if (RP.Inventory.Equip1 != "null")
                            Handle("equip", "", "e1");
                    }

                    RPData user = null;
                    if (RP.AcceptOffer != "seed" && RP.AcceptOffer != "trade")
                        user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.InteractID).GetRolePlay();
                    if (RP.AcceptOffer == "marry")
                    {
                        RP.Say("accepts " + user.habbo.Username + "'s proposal", false);
                        RP.smID = user.habbo.Id;
                        user.smID = RP.habbo.Id;
                        RP.Marriage("marry");
                        user.Marriage("marry");
                        user.InteractID = 0;
                        RP.InteractID = 0;
                        return;
                    }
                    if (RP.AcceptOffer == "trade")
                    {
                        var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget).GetClient().GetRolePlay();
                        RP.AcceptOffer = "null";
                        if (User == null)
                        {
                            RP.client.SendWhisper("User not found");
                            return;
                        }
                        if (User.GetClient().GetRolePlay().Trade.Trading)
                        {
                            RP.client.SendWhisper("This user is already trading!");
                            return;
                        }
                        if (RP.TradeType == 1)
                        {
                           // RP.Room.TryStartTrade(RP.roomUser, User.roomUser);
                            RP.TradeTarget = 0;
                            User.GetClient().GetRolePlay().TradeTarget = 0;
                        }
                        else if (RP.TradeType == 2)
                        {
                            if (User.Inventory.Equip2.Contains("kevlar"))
                                User.WebHandler.Handle("equip", "", "e2");
                            if (User.Inventory.Equip1 != "null")
                                User.WebHandler.Handle("equip", "", "e1");
                            RP.Trade.OpenTrade(User.roomUser);
                            User.Trade.Trading = true;
                            RP.Trade.Trading = true;
                        }
                        User.GetClient().GetRolePlay().TradeTimer = 0;
                        RP.TradeTimer = 0;
                        return;
                    }
                    if (RP.Inventory.IsInventoryFull(RP.AcceptOffer) && RP.AcceptOffer != "drink" && RP.AcceptOffer != "ticket")
                    {
                        RP.GetClient().SendWhisper("Your inventory is currently full!");
                        if (user != null)
                        {
                            user.InteractID = 0;
                            RP.InteractID = 0;
                            RP.Say("declines " + user.habbo.Username + "'s offer", false);
                        }
                        RP.AcceptOffer = "null";
                        return;
                    }
                    if (RP.habbo.Credits < RP.OfferAmount)
                    {
                        RP.client.SendWhisper("You do not have enough moeny to purchase this item!");
                        if (RP.AcceptOffer != "seed")
                        {
                            RP.InteractID = 0;
                            user.InteractID = 0;
                            user.client.SendWhisper(RP.habbo.Username + " could not afford your offer!");
                        }

                    }
                    else
                    {
                        if (RP.AcceptOffer == "storage")
                        {
                            RP.Say("purchases a new bank storage", false);
                            RP.Storage.SetStorage();
                            user.InteractID = 0;
                            RP.InteractID = 0;
                            RP.AcceptOffer = "null";
                        }
                        else if (RP.AcceptOffer == "seed")
                        {
                            RP.Inventory.Additem(RP.AcceptOffer, false, true, 3);
                            RP.Say("purchases a pack of plant " + RP.AcceptOffer + "", false);
                            RP.UpdateCredits(RP.OfferAmount, false);
                            RP.AcceptOffer = "null";
                            return;
                        }
                        else if (RP.AcceptOffer == "drink")
                        {
                            RP.Say("accepts " + user.habbo.Username + "'s offer", false);
                            if (user.roomUser.CarryItemId == 52)
                            {
                                RP.Inventory.Additem("snack");
                                if (user.Inventory.Currslot1 != "null" && user.Inventory.Equip1 == "snack")
                                {
                                    string currslot = user.Inventory.Currslot1;
                                    if (user.Inventory.IsSlotEmpty(currslot))
                                    {
                                        user.WebHandler.Handle("equip", "", "e1");
                                        user.Inventory.Additem(currslot, true);
                                    }
                                    else
                                    {
                                        user.Inventory.Additem(currslot, true);
                                        user.WebHandler.Handle("equip", "", "e1");
                                    }
                                }
                            }
                            else if (user.roomUser.CarryItemId == 1013)
                            {
                                RP.Inventory.Additem("medic");
                                if (user.Inventory.Currslot1 != "null" && user.Inventory.Equip1 == "medic")
                                {
                                    string currslot = user.Inventory.Currslot1;
                                    if (user.Inventory.IsSlotEmpty(currslot))
                                    {
                                        user.WebHandler.Handle("equip", "", "e1");
                                        user.Inventory.Additem(currslot, true);
                                    }
                                    else
                                    {
                                        user.Inventory.Additem(currslot, true);
                                        user.WebHandler.Handle("equip", "", "e1");
                                    }
                                }
                                user.JobManager.Task2++;
                                user.RPCache(27);
                            }
                            else
                            {
                                RP.roomUser.DanceId = 0;
                                RP.roomUser.CarryItem(user.roomUser.CarryItemId);
                            }
                            user.roomUser.CarryItem(0);
                        }
                        else if (RP.AcceptOffer == "ticket")
                        {
                            RP.Say("pays off their fine", false);
                            RP.roomUser.CanWalk = true;
                            RP.roomUser.ApplyEffect(0);
                            user.GetClient().GetRolePlay().JobManager.Task2++;
                            user.GetClient().GetRolePlay().RPCache(27);
                            if (RP.roomUser.Stunned > 0)
                                RP.roomUser.Stunned = 0;
                            RP.Cuffed = false;
                            RP.EndEscort();
                            var WL = RP.habbo.GetClientManager().GetWL(RP.habbo.Username, 0);
                            if (WL != null)
                            {
                                RP.Assault = false;
                                RP.habbo.GetClientManager().RemoveWL(WL.ID);
                            }
                            RP.roomUser.Assault = 0;
                            RP.habbo.GetClientManager().GlobalWeb("{\"name\":\"sidealert\", \"evnt\":\"ticket\","
                                + "\"name1\":\"" + user.habbo.Username + "\", \"name2\":\"" + RP.habbo.Username + "\","
                                + "\"color1\":\"" + user.Color + "\", \"color2\":\"" + RP.Color + "\", \"amount\":\"" + RP.OfferAmount + "\"}");
                        }
                        else if (!RP.Inventory.CheckItem(RP.AcceptOffer))
                        {
                            if (RP.AcceptOffer == "kevlar2")
                                RP.Inventory.Additem(RP.AcceptOffer, false, true, 1, false, 150);
                            else if (RP.AcceptOffer == "kevlar3")
                                RP.Inventory.Additem(RP.AcceptOffer, false, true, 1, false, 200);
                            else if (RP.AcceptOffer == "kevlar4")
                                RP.Inventory.Additem(RP.AcceptOffer, false, true, 1, false, 350);
                            else RP.Inventory.Additem(RP.AcceptOffer);
                            var item = RP.AcceptOffer;
                            if (item.Contains("_"))
                                item = Regex.Replace(item, "_", " ").Trim();
                            if (item.Contains("kevlar"))
                                RP.Say("purchases a new " + item + " vest", false);
                            else RP.Say("purchases a new " + item + "", false);
                        }
                        RP.UpdateCredits(RP.OfferAmount, false);
                        user.InteractID = 0;
                        RP.InteractID = 0;
                        user.UpdateCredits(2, true);
                        user.client.SendWhisper("You have been tipped 2 dollars for your services!");
                    }
                }
                else
                {
                    if (RP.AcceptOffer == "trade")
                    {
                        var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget).GetClient().GetRolePlay(); ;
                        User.client.SendWhisper("" + RP.habbo.Username + " declined your trade request!");
                        User.TradeTarget = 0;
                        RP.TradeTarget = 0;
                    }
                    else if (RP.AcceptOffer != "seed")
                    {
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.InteractID).GetRolePlay();
                        user.InteractID = 0;
                        RP.InteractID = 0;
                        RP.Say("declines " + user.habbo.Username + "'s offer", false);
                    }
                }
                RP.AcceptOffer = "null";
            }
        }

        public void Equip(string Event, string Data, string ExtraData)
        {
            if (Event == "equip" && RP.Room != null)
            {
                if (ExtraData.Length > 2)
                    ExtraData = RP.Inventory.DefineItem(ExtraData);
                if (RP.Trade.Trading && ExtraData != "e1" && ExtraData != "e2")
                {
                    RP.Trade.InitTrade(ExtraData);
                    return;
                }
                if (RP.Dead || RP.Jailed > 0 || RP.JailedSec > 0 || RP.GameType != "" || RP.Cuffed || RP.roomUser.Stunned > 0)
                {
                    RP.Responds();
                    return;
                }
                if (!RP.habbo.InRoom)
                    return;

                string showrest = "true";
                if (Data == "false")
                    showrest = "false";
                string Equip = "";
                string slot = "equip1";
                int hp = 0;
                string wep = "null";
                string Show = "";
                int quantity = 0;
                if (RP.trashwep)
                {
                    if (RP.Inventory.Currslot1 != "null" || RP.Inventory.Currslot2 != "null")
                    {
                        RP.client.SendWhisper("You must unequip your items before deleting!");
                        return;
                    }
                    RP.Inventory.Additem(ExtraData, true);
                }
                else if (RP.Storage.Curstorage > 0 && ExtraData != "e1" && ExtraData != "e2")
                {
                    if (RP.Inventory.Currslot1 != "null" || RP.Inventory.Currslot2 != "null")
                    {
                        RP.client.SendWhisper("You must unequip your items before storing them");
                        return;
                    }
                    RP.Storage.AddStorage(RP.Inventory.GetItem(ExtraData), RP.Inventory.GetHP(ExtraData), ExtraData);
                }
                else
                {
                    if (ExtraData == "e1")
                    {
                        wep = RP.Inventory.GetItem(RP.Inventory.Currslot1);
                        Show = RP.Inventory.Currslot1;
                        hp = RP.Inventory.GetHP(RP.Inventory.Currslot1);
                        quantity = RP.Inventory.GetQuantity(RP.Inventory.Currslot1);
                        RP.Inventory.Currslot1 = "null";
                        RP.Inventory.Equip1 = "null";
                    }
                    else if (ExtraData == "e2")
                    {
                        wep = RP.Inventory.GetItem(RP.Inventory.Currslot2);
                        Show = RP.Inventory.Currslot2;
                        hp = RP.Inventory.GetHP(RP.Inventory.Currslot2);
                        quantity = RP.Inventory.GetQuantity(RP.Inventory.Currslot2);
                        RP.Inventory.Currslot2 = "null";
                        RP.Inventory.Equip2 = "null";
                    }
                    else if (ExtraData.Contains("w"))
                    {
                        if (RP.Inventory.GetItem(ExtraData) == "skateboard" && RP.Room.Taxi == 1)
                        {
                            RP.client.SendWhisper("This action can only be performed outside!");
                            return;
                        }
                        if (RP.roomUser.Statusses.ContainsKey("lay") || RP.roomUser.Statusses.ContainsKey("sit"))
                        {
                            RP.client.SendWhisper("This action can not be performed while laying or sitting!");
                            return;
                        }
                        if (RP.Inventory.GetItem(ExtraData).Contains("kevlar") && RP.Inventory.Equip2 == "null")
                        {
                            RP.Inventory.Currslot2 = ExtraData;
                            RP.Inventory.Equip2 = RP.Inventory.GetItem(ExtraData);
                            slot = "equip2";
                        }
                        else if (RP.Inventory.Equip1 == "null" && !RP.Inventory.GetItem(ExtraData).Contains("kevlar"))
                        {
                            RP.Inventory.Currslot1 = ExtraData;
                            RP.Inventory.Equip1 = RP.Inventory.GetItem(ExtraData);
                        }
                        else return;
                        hp = RP.Inventory.GetHP(ExtraData);
                        Equip = RP.Inventory.GetItem(ExtraData);
                        if (RP.Inventory.CheckItem(Equip))
                            quantity = RP.Inventory.GetQuantity(ExtraData) - 1;
                    }
                }

                if (ExtraData == "e1" || ExtraData == "e2")
                {
                    if (wep == "null" || wep == "" || string.IsNullOrEmpty(wep))
                        return;
                    string num = Regex.Replace(Show, "w", "").Trim();
                    RP.SendWeb("{\"name\":\"equip2\", \"equip\":\"" + wep + "\", \"show\":\"" + num + "\", \"hide\":\"" + ExtraData + "\", \"whp\":\"" + (RP.Inventory.ItemHP(Show) - hp) + "\", \"maxhp\":\"" + (RP.Inventory.ItemHP(Show)) + "\", \"showrest\":\"" + showrest + "\", \"quantity\":\"" + quantity + "\"}");
                    if (wep == "skateboard")
                    {
                        RP.Skateboard = false;
                        RP.roomUser.SuperFastWalking = false;
                    }
                    if (wep.Contains("kevlar"))
                        RP.RemoveCloth("cc");
                    else RP.roomUser.ApplyEffect(0);
                    if (RP.Inventory.CheckItem(wep))
                    {
                        RP.item = 0;
                        RP.roomUser.CarryItem(0);
                    }
                    if (!RP.Inventory.CheckItem(wep))
                    {
                        if (RP.Inventory.showitem1 == wep && RP.Inventory.showitem2 != "null")
                        {
                            string item = "";
                            int itemhp = 0;
                            string curslot = "";
                            if (ExtraData == "e1")
                            {
                                item = RP.Inventory.GetItem(RP.Inventory.Currslot2);
                                itemhp = RP.Inventory.GetHP(RP.Inventory.Currslot2);
                                curslot = RP.Inventory.Currslot2;
                            }
                            else if (ExtraData == "e2")
                            {
                                item = RP.Inventory.GetItem(RP.Inventory.Currslot1);
                                itemhp = RP.Inventory.GetHP(RP.Inventory.Currslot1);
                                curslot = RP.Inventory.Currslot1;
                            }
                            RP.SendWeb("{\"name\":\"item_circle\", \"info\":\"5\", \"item\":\"" + item + "\", \"hp\":\"" + itemhp + "\", \"maxhp\":\"" + (RP.Inventory.ItemHP(curslot)) + "\"}");
                            RP.Inventory.showitem1 = RP.Inventory.showitem2;
                            RP.Inventory.showitem2 = "null";
                        }
                        else if (RP.Inventory.showitem1 == wep)
                        {
                            RP.SendWeb("{\"name\":\"item_circle\", \"info\":\"3\"}");
                            RP.Inventory.showitem1 = "null";
                        }
                        else if (RP.Inventory.showitem2 == wep)
                        {
                            RP.SendWeb("{\"name\":\"item_circle\", \"info\":\"4\"}");
                            RP.Inventory.showitem2 = "null";
                        }
                    }
                }
                else
                {
                    if (Equip == "null" || Equip == "" || string.IsNullOrEmpty(Equip))
                        return;
                    string num = Regex.Replace(ExtraData, "w", "").Trim();
                    RP.SendWeb("{\"name\":\"equip\", \"equip\":\"" + Equip + "\", \"hide\":\"" + num + "\", \"whp\":\"" + (RP.Inventory.ItemHP(ExtraData) - hp) + "\", \"maxhp\":\"" + (RP.Inventory.ItemHP(ExtraData)) + "\", \"slot\":\"" + slot + "\", \"quantity\":\"" + quantity + "\"}");
                    RP.Inventory.ItemEffect(Equip);
                    if (!RP.Inventory.CheckItem(Equip))
                    {
                        if (RP.Inventory.showitem1 == "null")
                        {
                            RP.SendWeb("{\"name\":\"item_circle\", \"info\":\"1\", \"item\":\"" + Equip + "\", \"hp\":\"" + hp + "\", \"maxhp\":\"" + (RP.Inventory.ItemHP(ExtraData)) + "\"}");
                            RP.Inventory.showitem1 = Equip;
                        }
                        else if (RP.Inventory.showitem2 == "null")
                        {
                            RP.SendWeb("{\"name\":\"item_circle\", \"info\":\"2\", \"item\":\"" + Equip + "\", \"hp\":\"" + hp + "\", \"maxhp\":\"" + (RP.Inventory.ItemHP(ExtraData)) + "\"}");
                            RP.Inventory.showitem2 = Equip;
                        }
                    }
                }

            }
        }

        public void ItemInfo(string Event, string Data, string ExtraData)
        {
            if (Event == "iteminfo")
            {
                ExtraData = RP.Inventory.DefineItem(ExtraData);
                string item = "";
                int whp = 0;
                int maxhp = 100;
                #region Equip
                if (ExtraData == "e1")
                {
                    item = RP.Inventory.GetItem(RP.Inventory.Currslot1);
                    whp = RP.Inventory.GetHP(RP.Inventory.Currslot1);
                }
                else if (ExtraData == "e2")
                {
                    item = RP.Inventory.GetItem(RP.Inventory.Currslot2);
                    whp = RP.Inventory.GetHP(RP.Inventory.Currslot2);
                }
                else if (Data == "")
                {
                    item = RP.Inventory.GetItem(ExtraData);
                    whp = RP.Inventory.GetHP(ExtraData);
                }
                #endregion
                #region Trade
                else if (Data == "1")
                {
                    string i = ExtraData;
                    if (i.Contains("10") || i.Contains("11") || i.Contains("12"))
                        i = Convert.ToString(Regex.Match(i, @"(.{2})\s*$"));
                    else i = Convert.ToString(Regex.Match(i, @"(.{1})\s*$"));
                    if (Convert.ToInt32(i) < 7)
                    {
                        if (ExtraData == "w1")
                        {
                            item = RP.Trade.Slot1;
                            whp = RP.Trade.HP1;
                        }
                        else if (ExtraData == "w2")
                        {
                            item = RP.Trade.Slot2;
                            whp = RP.Trade.HP2;
                        }
                        else if (ExtraData == "w3")
                        {
                            item = RP.Trade.Slot3;
                            whp = RP.Trade.HP3;
                        }
                        else if (ExtraData == "w4")
                        {
                            item = RP.Trade.Slot4;
                            whp = RP.Trade.HP4;
                        }
                        else if (ExtraData == "w5")
                        {
                            item = RP.Trade.Slot5;
                            whp = RP.Trade.HP5;
                        }
                        else if (ExtraData == "w6")
                        {
                            item = RP.Trade.Slot6;
                            whp = RP.Trade.HP6;
                        }
                    }
                    else
                    {
                        var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                        if (ExtraData == "w7")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot1;
                            whp = User.GetClient().GetRolePlay().Trade.HP1;
                        }
                        else if (ExtraData == "w8")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot2;
                            whp = User.GetClient().GetRolePlay().Trade.HP2;
                        }
                        else if (ExtraData == "w9")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot3;
                            whp = User.GetClient().GetRolePlay().Trade.HP3;
                        }
                        else if (ExtraData == "w10")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot4;
                            whp = User.GetClient().GetRolePlay().Trade.HP4;
                        }
                        else if (ExtraData == "w11")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot5;
                            whp = User.GetClient().GetRolePlay().Trade.HP5;
                        }
                        else if (ExtraData == "w12")
                        {
                            item = User.GetClient().GetRolePlay().Trade.Slot6;
                            whp = User.GetClient().GetRolePlay().Trade.HP6;
                        }
                    }
                }
                #endregion
                string title = "";
                string itemuse = "";
                if (item == "kevlar2")
                    maxhp = 150;
                else if (item == "kevlar3")
                    maxhp = 200;
                else if (item == "kevlar4")
                    maxhp = 350;
                #region description
                else if (item == "knife")
                {
                    title = "Knife";
                    itemuse = "+2 damage";
                }
                if (item == "skateboard")
                {
                    title = "Skateboard";
                    itemuse = "Increases your speed by 50%";
                }
                else if (item == "bat" || item == "gold_bat")
                {
                    if (item == "gold_bat")
                        title = "Golden Bat";
                    else title = "Baseball Bat";
                    itemuse = "+2 damage";
                }
                else if (item == "sword")
                {
                    title = "Pirate Sword";
                    itemuse = "+3 damage";
                }
                else if (item == "axe")
                {
                    title = "Viking Axe";
                    itemuse = "+3 damage";
                }
                else if (item.Contains("kevlar"))
                {
                    title = "Kevlar Vest";
                    itemuse = "Health Shield";
                }
                else if (item.Contains("stun"))
                {
                    title = "Stun Gun";
                    itemuse = "Tazer Shots";
                }
                else if (item == "medic")
                {
                    title = "Medical Kit";
                    itemuse = "+25 HP";
                }
                else if (item == "snack")
                {
                    title = "Hot Cheetos";
                    itemuse = "+5 HP";
                }
                else if (item == "seed")
                    title = "Plant Seed";
                else if (item == "weed")
                {
                    title = "Cannabis";
                    itemuse = "50% Stamina recovery";
                }
                else if (item == "flower")
                    title = "Yellow Flower";
                else if (item == "candycane")
                {
                    title = "Candy Cane";
                    itemuse = "+2 Damage";
                }
                else if (item == "carrot")
                {
                    title = "Carrot";
                    itemuse = "+3 Hunger";
                }
                else if (item == "battle_axe" || item == "gold_battleaxe")
                {
                    title = "Battle Axe";
                    itemuse = "+4 damage";
                }
                else if (item == "chain_stick" || item == "gold_chainstick")
                {
                    title = "Chain Stick";
                    itemuse = "+2 damage";
                }
                else if (item == "crowbar" || item == "gold_crowbar")
                {
                    title = "Crowbar";
                    itemuse = "+2 damage";
                }
                else if (item == "iron_bat")
                {
                    title = "Iron Bat";
                    itemuse = "+3 damage";
                }
                else if (item == "lightsaber" || item == "gold_lightsaber")
                {
                    title = "Light Saber";
                    itemuse = "+10 damage";
                }
                else if (item == "long_sword" || item == "gold_longsword")
                {
                    title = "Long Sword";
                    itemuse = "+5 damage";
                }
                else if (item == "metal_pipe" || item == "gold_pipe")
                {
                    if (item == "gold_pipe")
                        title = "Golden Pipe";
                    else title = "Metal Pipe";
                    itemuse = "+3 damage";
                }
                else if (item == "power_axe" || item == "gold_poweraxe")
                {
                    title = "Power Axe";
                    itemuse = "+4 damage";
                }
                else if (item == "spike_ball" || item == "gold_spikeball")
                {
                    title = "Spike Ball";
                    itemuse = "+3 damage";
                }
                else if (item == "fishing_rod")
                    title = "Fishing Rod";
                else if (item == "akorn")
                {
                    title = "Akorn";
                    itemuse = "+1 Hunger";
                }
                else if (item == "apple")
                {
                    title = "Red Apple";
                    itemuse = "+5 Hunger";
                }
                else if (item == "banana")
                {
                    title = "Banana";
                    itemuse = "+5 Hunger";
                }
                else if (item == "beetroot")
                {
                    title = "Beetroot";
                    itemuse = "+3 Hunger";
                }
                else if (item == "blackberry")
                {
                    title = "Black Berry";
                    itemuse = "+5 Hunger";
                }
                else if (item == "bread")
                {
                    title = "Bread";
                    itemuse = "+15 Hunger";
                }
                else if (item == "cheese")
                {
                    title = "Cheese";
                    itemuse = "+10 Hunger";
                }
                else if (item == "cake")
                {
                    title = "Cake";
                    itemuse = "+20 Hunger";
                }
                else if (item == "chicken")
                {
                    title = "Chicken";
                    itemuse = "+25 Hunger";
                }
                else if (item == "raw_chicken")
                {
                    title = "Raw Chicken";
                    itemuse = "+3 Hunger";
                }
                else if (item == "raw_meat")
                {
                    title = "Raw Meat";
                    itemuse = "+7 Hunger";
                }
                else if (item == "cookbook")
                    title = "Cook Book";
                else if (item == "cooked_fish")
                {
                    title = "Cooked Fish";
                    itemuse = "+30 Hunger";
                }
                else if (item == "cookie")
                {
                    title = "Cookie";
                    itemuse = "+2 Hunger";
                }
                else if (item == "grapes")
                {
                    title = "Grapes";
                    itemuse = "+3 Hunger";
                }
                else if (item == "purple_grapes")
                {
                    title = "Purple Grapes";
                    itemuse = "+3 Hunger";
                }
                else if (item == "orange")
                {
                    title = "Orange";
                    itemuse = "+3 Hunger";
                }
                else if (item == "mushroom")
                {
                    title = "Mushroom";
                    itemuse = "+3 Hunger";
                }
                else if (item == "lemon")
                {
                    title = "Lemon";
                    itemuse = "+3 Hunger";
                }
                else if (item == "meat")
                {
                    title = "Meat";
                    itemuse = "+50 Hunger";
                }
                else if (item == "pear")
                {
                    title = "Pear";
                    itemuse = "+3 Hunger";
                }
                else if (item == "pineapple")
                {
                    title = "Pineapple";
                    itemuse = "+15 Hunger";
                }
                else if (item == "watermelon")
                {
                    title = "Watermelon";
                    itemuse = "+15 Hunger";
                }
                else if (item == "stake")
                {
                    title = "Stake";
                    itemuse = "+50 Hunger";
                }
                else if (item == "raw_stake")
                {
                    title = "Raw Stake";
                    itemuse = "+5 Hunger";
                }
                else if (item == "strawberry")
                {
                    title = "Strawberry";
                    itemuse = "+3 Hunger";
                }
                else if (item == "tomato")
                {
                    title = "Tomato";
                    itemuse = "+2 Hunger";
                }
                else if (item == "cooked_egg")
                {
                    title = "Cooked Egg";
                    itemuse = "+10 Hunger";
                }
                else if (item == "green_pepper")
                {
                    title = "Green Pepper";
                    itemuse = "+1 Hunger";
                }
                else if (item == "yellow_pepper")
                {
                    title = "Yellow Pepper";
                    itemuse = "+1 Hunger";
                }
                else if (item == "red_pepper")
                {
                    title = "Red Pepper";
                    itemuse = "+1 Hunger";
                }
                else if (item == "egg")
                {
                    title = "Egg";
                    itemuse = "+1 Hunger";
                }
                else if (item == "fish")
                {
                    title = "Raw Fish";
                    itemuse = "+3 Hunger";
                }
                else if (item == "chocolate")
                {
                    title = "Chocolate Bar";
                    itemuse = "+3 Hunger";
                }
                else if (item == "cherry")
                {
                    title = "Cherry";
                    itemuse = "+2 Hunger";
                }
                else if (item == "xpboost")
                {
                    title = "XP boost";
                    itemuse = "25% experience boost";
                }
                else if (item == "flashdrive")
                    title = "Flash Drive";
                else if (item == "cutdiamond")
                    title = "Cut Diamond";
                else if (item == "cutruby")
                    title = "Cut Ruby";
                else if (item == "cutsapphire")
                    title = "Cut Sapphire";
                else if (item == "diamond")
                    title = "Diamond";
                else if (item == "pearl")
                    title = "Pearl";
                else if (item == "ruby")
                    title = "Ruby";
                else if (item == "sapphire")
                    title = "Sapphire";
                else if (item == "stone")
                    title = "Stone";
                else if (item == "stonediamond")
                    title = "Stone Diamond";
                else if (item == "stoneruby")
                    title = "Stone Ruby";
                else if (item == "stonesapphire")
                    title = "Stone Sapphire";
                else if (item == "bronzebar")
                    title = "Bronze Bar";
                else if (item == "bronzebar")
                    title = "Bronze Bar";
                else if (item == "goldbar")
                    title = "Golden Bar";
                else if (item == "silverbar")
                    title = "Silver Bar";
                else if (item == "coppercoin")
                    title = "Copper Coin";
                else if (item == "silvercoin")
                    title = "Silver Coin";
                else if (item == "telescope")
                    title = "Telescope";
                else if (item == "treasuremap")
                    title = "Treasure Map";
                #endregion
                if (item == "null" || item == "" || string.IsNullOrEmpty(item))
                    return;
                RP.SendWeb("{\"name\":\"iteminfo\", \"item\":\"" + item + "\", \"itemhp\":\"" + whp + "/" + maxhp + "\", \"itemtitle\":\"" + title + "\", \"itemuse\":\"" + itemuse + "\"}");
            }
        }

        public void ItemDrag(string Event, string Data, string ExtraData)
        {
            if (RP.trashwep || RP.Trade.Trading || DeletingItem)
                return;
            if (Event == "itemdrag")
            {
                if (String.IsNullOrEmpty(Data))
                {
                    RP.SendWeb("{\"name\":\"itemdrag3\"}");
                    CurItemDrag = "";
                    return;
                }
                if (Data.Contains("bslot"))
                    ItemType = "storage";
                else ItemType = "slot";
                Data = RP.Inventory.DefineItem(Data);
                string item = "";
                if (ItemType == "storage")
                    item = RP.Storage.GetItem(Data);
                else item = RP.Inventory.GetItem(Data);
                if (String.IsNullOrEmpty(item) || item == "null"
                    || ((RP.Inventory.Currslot1 == Data || RP.Inventory.Currslot2 == Data) && RP.Inventory.GetQuantity(Data) <= 1 && ItemType != "storage"))
                {
                    RP.SendWeb("{\"name\":\"itemdrag3\"}");
                    CurItemDrag = "";
                    return;
                }
                string slot = Regex.Replace(Data, "w", "").Trim();
                RP.SendWeb("{\"name\":\"itemdrag\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                CurItemDrag = Data;

            }
            else if (Event == "itemposition")
            {
                if (String.IsNullOrEmpty(CurItemDrag))
                    return;
                var item = "";
                if (ItemType == "slot")
                    item = RP.Inventory.GetItem(CurItemDrag);
                else item = RP.Storage.GetItem(CurItemDrag);
                string slot = Regex.Replace(CurItemDrag, "w", "").Trim();
                if (Data.Contains("eslot"))
                {
                    if (RP.Storage.Curstorage > 0 || RP.Dead || RP.Jailed > 0 || RP.JailedSec > 0)
                    {
                        RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                        CurItemDrag = "";
                        return;
                    }
                    if ((!RP.Inventory.GetItem(CurItemDrag).Contains("kevlar") && RP.Inventory.Equip1 == "null") || (RP.Inventory.GetItem(CurItemDrag).Contains("kevlar") && RP.Inventory.Equip2 == "null"))
                    {
                        if (RP.Inventory.GetQuantity(CurItemDrag) > 1)
                            RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                        Handle("equip", "", "slot" + slot + "");
                    }
                    else RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                    CurItemDrag = "";
                    return;
                }
                if (Data == "trashicon")
                {
                    RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                    RP.SendWeb("{\"name\":\"deleteitem\"}");
                    DeletingItem = true;
                    return;
                }
                if (ItemType == "storage" && (!Data.Contains("slot") || (CurItemDrag == RP.Inventory.DefineItem(Data) && Data.Contains("bslot"))))
                {
                    if (Data.Contains("quantity") && !Data.Contains("tquantity"))
                    { }
                    else
                    {
                        if ((CurItemDrag == RP.Inventory.DefineItem(Data) && Data.Contains("bslot") && ItemType == "slot")
                            || (CurItemDrag == RP.Inventory.DefineItem(Data) && !Data.Contains("bslot") && ItemType == "bslot"))
                        { }
                        else
                        {
                            var Item = RP.Storage.GetItem(CurItemDrag);
                            if (Item != "null")
                                RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                            CurItemDrag = "";
                            return;
                        }
                    }
                }
                if ((RP.Inventory.Currslot1 == CurItemDrag
                    || RP.Inventory.Currslot1 == CurItemDrag) && ItemType == "slot"
                    && RP.Inventory.GetQuantity(CurItemDrag) <= 1)
                {
                    CurItemDrag = "";
                    return;
                }
                if (ItemType == "slot" && (!Data.Contains("slot") || CurItemDrag == RP.Inventory.DefineItem(Data)))
                {
                    if (Data.Contains("quantity") && !Data.Contains("tquantity"))
                    { }
                    else
                    {
                        if ((CurItemDrag == RP.Inventory.DefineItem(Data) && Data.Contains("bslot") && ItemType == "slot")
                            || (CurItemDrag == RP.Inventory.DefineItem(Data) && !Data.Contains("bslot") && ItemType == "bslot"))
                        { }
                        else
                        {
                            var Item = RP.Inventory.GetItem(CurItemDrag);
                            if (Item != "null")
                                RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                            CurItemDrag = "";
                            return;
                        }
                    }
                }
                if (RP.Inventory.Equip1 != "null" || RP.Inventory.Equip2 != "null")
                {
                    RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                    RP.client.SendWhisper("You must first unequip your item(s) before moving this item");
                    CurItemDrag = "";
                    return;
                }
                if (ItemType == "storage")
                {
                    var I1 = RP.Storage.GetItem(CurItemDrag);
                    var Q1 = RP.Storage.GetQuantity(CurItemDrag);
                    var H1 = RP.Storage.GetHP(CurItemDrag);
                    if (Data.Contains("bslot") || Data.Contains("bquantity"))
                    {
                        Data = RP.Inventory.DefineItem(Data);
                        var I2 = RP.Storage.GetItem(Data);
                        var Q2 = RP.Storage.GetQuantity(Data);
                        var H2 = RP.Storage.GetHP(Data);
                        if (I1 == I2 && Q2 + Q1 <= 50 && RP.Inventory.CheckItem(I1))
                        {
                            RP.Storage.UpdateQuantity(Data, Q2 += Q1);
                            RP.Storage.UpdateItem(CurItemDrag, "null");
                            RP.Storage.UpdateHP(CurItemDrag, 0);
                            RP.Storage.UpdateQuantity(CurItemDrag, 0);
                        }
                        else
                        {
                            RP.Storage.UpdateItem(CurItemDrag, I2);
                            RP.Storage.UpdateHP(CurItemDrag, H2);
                            if (I2 == "null")
                                RP.Storage.UpdateQuantity(CurItemDrag, 0);
                            else RP.Storage.UpdateQuantity(CurItemDrag, Q2);
                            RP.Storage.UpdateItem(Data, I1);
                            RP.Storage.UpdateHP(Data, H1);
                            RP.Storage.UpdateQuantity(Data, Q1);
                        }
                        CurItemDrag = "";
                        RP.Storage.UpdateStorage(RP.Storage.Curstorage);
                        return;
                    }
                    else if (!Data.Contains("bslot") && !Data.Contains("bquantity"))
                    {
                        Data = RP.Inventory.DefineItem(Data);
                        var I2 = RP.Inventory.GetItem(Data);
                        var Q2 = RP.Inventory.GetQuantity(Data);
                        var H2 = RP.Inventory.GetHP(Data);
                        if (I2.Contains("stun"))
                        {
                            RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                            RP.client.SendWhisper("You can not replace this item!");
                            CurItemDrag = "";
                            return;
                        }
                        if (I1 == I2 && Q2 + Q1 <= 50 && RP.Inventory.CheckItem(I1))
                        {
                            RP.Inventory.UpdateQuantity(Data, Q2 += Q1);
                            RP.Storage.UpdateItem(CurItemDrag, "null");
                            RP.Storage.UpdateHP(CurItemDrag, 0);
                            RP.Storage.UpdateQuantity(CurItemDrag, 0);
                        }
                        else
                        {
                            RP.Storage.UpdateItem(CurItemDrag, I2);
                            RP.Storage.UpdateHP(CurItemDrag, H2);
                            if (I2 == "null")
                                RP.Storage.UpdateQuantity(CurItemDrag, 0);
                            else RP.Storage.UpdateQuantity(CurItemDrag, Q2);
                            RP.Inventory.UpdateItem(Data, I1);
                            RP.Inventory.UpdateHP(Data, H1);
                            RP.Inventory.UpdateQuantity(Data, Q1);
                        }
                        CurItemDrag = "";
                        RP.LoadStats(true);
                        RP.Storage.UpdateStorage(RP.Storage.Curstorage);
                        return;
                    }
                }
                var item1 = RP.Inventory.GetItem(CurItemDrag);
                var quantity1 = RP.Inventory.GetQuantity(CurItemDrag);
                var hp1 = RP.Inventory.GetHP(CurItemDrag);
                if (Data.Contains("bslot") || Data.Contains("bquantity"))
                {
                    Data = RP.Inventory.DefineItem(Data);
                    var Item2 = RP.Storage.GetItem(Data);
                    var Quantity2 = RP.Storage.GetQuantity(Data);
                    var Hp2 = RP.Storage.GetHP(Data);
                    if (item1.Contains("stun"))
                    {
                        RP.SendWeb("{\"name\":\"itemdrag2\", \"item\":\"" + item + "\", \"slot\":\"" + slot + "\", \"itemtype\":\"" + ItemType + "\"}");
                        RP.client.SendWhisper("You can not add this item into your storage!");
                        CurItemDrag = "";
                        return;
                    }
                    if (item1 == Item2 && quantity1 + Quantity2 <= 50 && RP.Inventory.CheckItem(item1))
                    {
                        RP.Storage.UpdateQuantity(Data, Quantity2 += quantity1);
                        RP.Inventory.UpdateItem(CurItemDrag, "null");
                        RP.Inventory.UpdateHP(CurItemDrag, 0);
                        RP.Inventory.UpdateQuantity(CurItemDrag, 0);
                    }
                    else
                    {
                        RP.Inventory.UpdateItem(CurItemDrag, Item2);
                        RP.Inventory.UpdateHP(CurItemDrag, Hp2);
                        if (Item2 == "null")
                            RP.Inventory.UpdateQuantity(CurItemDrag, 0);
                        else RP.Inventory.UpdateQuantity(CurItemDrag, Quantity2);
                        RP.Storage.UpdateItem(Data, item1);
                        RP.Storage.UpdateHP(Data, hp1);
                        RP.Storage.UpdateQuantity(Data, quantity1);
                    }
                    RP.LoadStats(true);
                    RP.Storage.UpdateStorage(RP.Storage.Curstorage);
                    CurItemDrag = "";
                    return;
                }
                Data = RP.Inventory.DefineItem(Data);
                var item2 = RP.Inventory.GetItem(Data);
                var quantity2 = RP.Inventory.GetQuantity(Data);
                var hp2 = RP.Inventory.GetHP(Data);
                if (item1 == item2 && quantity1 + quantity2 <= 50 && RP.Inventory.CheckItem(item1))
                {
                    RP.Inventory.UpdateQuantity(Data, quantity2 += quantity1);
                    RP.Inventory.UpdateItem(CurItemDrag, "null");
                    RP.Inventory.UpdateHP(CurItemDrag, 0);
                    RP.Inventory.UpdateQuantity(CurItemDrag, 0);
                }
                else
                {
                    RP.Inventory.UpdateItem(CurItemDrag, item2);
                    RP.Inventory.UpdateHP(CurItemDrag, hp2);
                    if (item2 == "null")
                        RP.Inventory.UpdateQuantity(CurItemDrag, 0);
                    else RP.Inventory.UpdateQuantity(CurItemDrag, quantity2);
                    RP.Inventory.UpdateItem(Data, item1);
                    RP.Inventory.UpdateHP(Data, hp1);
                    RP.Inventory.UpdateQuantity(Data, quantity1);
                }
                CurItemDrag = "";
                RP.LoadStats(true);
            }
        }

        public void ItemDelete(string Event, string Data, string ExtraData)
        {
            if (Event == "deleteitem")
            {
                DeletingItem = false;
                if (ExtraData == "no")
                    CurItemDrag = "";
                else if (ExtraData == "yes")
                {
                    if (RP.Inventory.Equip1 != "null" || RP.Inventory.Equip2 != "null")
                    {
                        RP.client.SendWhisper("You must unequip your item(s) before performing this action!");
                        CurItemDrag = "";
                        return;
                    }
                    if (RP.Trade.Trading)
                    {
                        RP.client.SendWhisper("You must stop trading before performing this action!");
                        CurItemDrag = "";
                        return;
                    }
                    if (ItemType == "slot")
                    {
                        RP.Inventory.UpdateItem(CurItemDrag, "null");
                        RP.Inventory.UpdateHP(CurItemDrag, 0);
                        RP.Inventory.UpdateQuantity(CurItemDrag, 0);
                        RP.LoadStats(true);
                        RP.Inventory.ItemCache(RP.Inventory.GetItemID(CurItemDrag));
                    }
                    else
                    {
                        RP.Storage.UpdateItem(CurItemDrag, "null");
                        RP.Storage.UpdateHP(CurItemDrag, 0);
                        RP.Storage.UpdateQuantity(CurItemDrag, 0);
                        RP.Storage.UpdateStorage(RP.Storage.Curstorage);
                    }
                    CurItemDrag = "";
                }
            }
        }

        public void Profile(string Event, string Data, string ExtraData)
        {
            if (Event == "bio")
            {
                RP.client.SendWhisper(ExtraData);
                if (String.IsNullOrEmpty(ExtraData))
                {
                    RP.client.SendWhisper("The field is blank!");
                    return;
                }
                if (ExtraData.Length > 384)
                {
                    RP.client.SendWhisper("This bio is too long!");
                    return;
                }
                RP.bio = ExtraData;
                RP.RPCache(12);
                RP.SendWeb("{\"name\":\"bio\", \"bio\":\"" + RP.bio + "\"}");
            }
            else if (Event == "statsrequest")
            {
                if (ExtraData == "false")
                {
                    RPData Target = null;
                    if (Data == "true")
                        Target = RP.GetClient().GetRolePlay();
                    else if (Data == "false")
                    {
                        var user = PlusEnvironment.GetHabboById(RP.lockID);
                        if (user == null || !user.InRoom || user.GetClient().GetRolePlay().AutoLogout > 0)
                        {
                            RP.client.SendWhisper("This user is currently offline!");
                            return;
                        }
                        Target = user.GetClient().GetRolePlay();
                    }
                    else if (!String.IsNullOrEmpty(Data))
                    {
                        var user = RP.habbo.GetClientManager().GetClientByUsername(Data);
                        if (user == null || !user.GetHabbo().InRoom || user.GetRolePlay().AutoLogout > 0)
                        {
                            RP.client.SendWhisper("This user is currently offline!");
                            return;
                        }
                        Target = user.GetRolePlay();
                    }
                    else
                    {
                        if (RP.PoliceCalls.IsBot == "true")
                        {
                            RP.client.SendWhisper("This action can not be performed on a bot!");
                            return;
                        }
                        var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(RP.PoliceCalls.Name);
                        if (user == null || !user.GetHabbo().InRoom || user.GetRolePlay().AutoLogout > 0)
                            return;
                        Target = user.GetRolePlay();
                    }

                    int xp1 = Target.XP;
                    int xp2 = Target.XPdue;
                    string job = "";
                    string jobtitle = "Unemployed";
                    string color1 = "null";
                    string color2 = "null";
                    string gangname = "";
                    string gangrank = "null";
                    string gangdate = "null";
                    string petname = "No Pet";
                    int pettype = 0;
                    int petlevel = 0;
                    int petxp = 0;
                    int petxpdue = 0;
                    int petxp1 = 0;
                    int petxp2 = 0;
                    int petkill = 0;
                    int pethit = 0;
                    int petdeath = 0;
                    int petarrested = 0;
                    int ganghits = 0;
                    int gangkills = 0;
                    int turf = 0;
                    int gangarrest = 0;
                    if (xp2 > 75)
                    {
                        xp2 = Target.XPdue / 2;
                        xp1 = Target.XP - xp2;
                    }
                    if (xp2 == 0 && xp1 == 0)
                        xp2 = 1;
                    if (Target.JobManager.Job == 1)
                    {
                        job = "nypd.png";
                        jobtitle = "New York Police Department";
                    }
                    else if (Target.JobManager.Job == 2 || Target.JobManager.Job == 7)
                    {
                        job = "hosp.gif";
                        jobtitle = "Rose Medical Center";
                    }
                    else if (Target.JobManager.Job == 3)
                    {
                        job = "bank.png";
                        jobtitle = "Mellon Bank";
                    }
                    else if (Target.JobManager.Job == 4)
                    {
                        job = "security.gif";
                        jobtitle = "New York Security Department";
                    }
                    else if (Target.JobManager.Job == 5)
                    {
                        job = "rest.gif";
                        jobtitle = "Sun Rise Diner";
                    }
                    else if (Target.JobManager.Job == 6)
                    {
                        job = "bbj.gif";
                        jobtitle = "Ben's Bubble Juice";
                    }
                    else if (Target.JobManager.Job == 8)
                    {
                        job = "casino.gif";
                        jobtitle = "Queens Casino";
                    }
                    if (Target.Gang > 0)
                    {
                        color1 = Target.GangManager.Color1;
                        color2 = Target.GangManager.Color2;
                        gangname = Target.GangManager.Name;
                        gangrank = Target.RankName;
                        gangarrest = Target.arrests2;
                        gangkills = Target.kills2;
                        ganghits = Target.punches2;
                        gangdate = Target.GangDate;
                        turf = Target.captures;
                        string editdate = gangdate.Substring(0, 2);
                        if (editdate.Contains("-"))
                        {
                            editdate = gangdate.Substring(0, 1);
                            gangdate = RP.datecheck(Convert.ToInt32(editdate)) + gangdate.Substring(1);
                        }
                        else gangdate = RP.datecheck(Convert.ToInt32(editdate)) + gangdate.Substring(2);
                    }
                    if (Target.HasPet > 0)
                    {
                        RoomUser Pet = null;
                        if (Target.Room.GetRoomUserManager().TryGetPet(Target.HasPet, out Pet))
                        {
                            petname = Pet.PetData.Name;
                            petlevel = Pet.PetData.Lvl;
                            pettype = Pet.PetData.Type;
                            petxp = Pet.PetData.XP;
                            petxpdue = Pet.PetData.XPDue;
                            petkill = Pet.PetData.Kills;
                            pethit = Pet.PetData.Hits;
                            petarrested = Pet.PetData.Arrest;
                            petdeath = Pet.PetData.Death;
                            petxp1 = petxp;
                            petxp2 = petxpdue;
                            if (petxp2 > 25)
                            {
                                petxp2 = petxpdue / 2;
                                petxp1 = petxp - petxp2;
                            }
                            if (petxp2 == 0 && petxp1 == 0)
                                petxp2 = 1;
                        }
                    }
                    Target.GetTimer(Convert.ToInt32(PlusEnvironment.GetUnixTimestamp() - Target.habbo.LastOnline));
                    RP.SendWeb("{\"name\":\"fullstats\", \"smcolor\":\"" + Target.smColor + "\", \"soulmate\":\"" + Target.SoulMate + "\", \"username\":\"" + Target.habbo.Username + "\", \"look\":\"" + Target.habbo.Look + "\", \"level\":\"<b>·</b> Level: <b>" + Target.Level + "</b>\","
                    + "\"xp\":\"XP: " + Target.XP + "/" + Target.XPdue + "\", \"login\":\"" + Target.onlinetime + "\", \"bio\":\"" + Target.bio + "\", \"str\":\"<b>·</b> Strength: <b>" + (Target.Strength + Target.MaxStrength) + "</b>\","
                    + "\"arrest\":\"<b>·</b> Arrested: <b>" + Target.Arrests + "</b>\", \"deaths\":\"<b>·</b> Deaths: <b>" + Target.Deaths + "</b>\", \"punches\":\"<b>·</b> Punches: <b>" + Target.Punches + "</b>\", \"kills\":\"<b>·</b> Kills: <b>" + Target.Kills + "</b>\","
                    + "\"xp1\":\"" + xp1 + "\", \"xp2\":\"" + xp2 + "\", \"color\":\"" + Target.Color + "\", \"gender\":\"" + Target.habbo.Gender + "\", \"jobtitle\":\"" + jobtitle + "\","
                    + "\"jobbadge\":\"" + job + "\", \"jobrank\":\"Rank: " + Target.JobManager.JobMotto + "\", \"jobdate\":\"Hired: " + Target.JobManager.JobDate + "\", \"task1\":\"<b>·</b> Completed <b>" + Target.JobManager.Shifts + "</b> Shifts\", \"task2\":\"<b>·</b> " + Target.JobTask(1) + "\","
                    + "\"task3\":\"<b>·</b> " + Target.JobTask(2) + "\", \"task4\":\"<b>·</b> " + Target.JobTask(3) + "\", \"ganghit\":\"<b>·</b> Landed <b>" + ganghits + "</b> Punches\","
                    + "\"gangname\":\"" + gangname + "\", \"gangdate\":\"Joined: " + gangdate + "\", \"gangkill\":\"<b>·</b> Killed <b>" + gangkills + "</b> Civilians\","
                    + "\"gangrank\":\"Rank: " + gangrank + "\", \"gangarrest\":\"<b>·</b> Arrested <b>" + gangarrest + "</b> Times\", \"colo1\":\"" + color1 + "\", \"color2\":\"" + color2 + "\", \"turf\":\"<b>·</b> Captured <b>" + turf + "</b> Turfs\","
                    + "\"online\":\"online\", \"solowins\":\"<b>·</b> Solo Wins: <b>" + Target.SoloWin + "</b>\", \"sololost\":\"<b>·</b> Solo Lost: <b>" + Target.SoloLost + "</b>\","
                    + "\"petname\":\"" + petname + "\", \"petlevel\":\"Level: " + petlevel + "\", \"petxp\":\"XP: " + petxp + "/" + petxpdue + "\", \"petxp1\":\"" + petxp1 + "\", \"petxp2\":\"" + petxp2 + "\","
                    + "\"petkill\":\"<b>·</b> Killed <b>" + petkill + "</b> People\", \"pethit\":\"<b>·</b> Landed <b>" + pethit + "</b> Attacks\", \"petarrest\":\"<b>·</b> Arrested <b>" + petarrested + "</b> Times\", \"petdeath\":\"<b>·</b> Died <b>" + petdeath + "</b> Times\", \"pet\":\"" + pettype + "\"}");
                }
            }
        }

        public void LockTarget(string Event, string Data, string ExtraData)
        {
            if (Event == "lock")
            {
                if (Data == "true")
                {
                    if (ExtraData == "true")
                    {
                        RoomUser bot = null;
                        if (!RP.Room.GetRoomUserManager().TryGetBot(RP.lockID, out bot))
                            return;
                        RP.lockBot = RP.lockID;
                        if (RP.lockTarget > 0)
                            RP.lockTarget = 0;
                        RP.GetClient().SendWhisper("Target locked on: " + bot.BotData.Name + "");
                    }
                    else
                    {
                        var client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.lockID);
                        if (client == null)
                            return;
                        RP.lockTarget = RP.lockID;
                        if (RP.lockBot > 0)
                            RP.lockBot = 0;
                        RP.GetClient().SendWhisper("Target locked on: " + client.GetRolePlay().habbo.Username + "");
                    }
                }
                else
                {
                    if (ExtraData == "true")
                        RP.lockBot = 0;
                    else
                        RP.lockTarget = 0;
                }
            }
            else if (Event == "closetarget")
            {
                if (ExtraData == "true")
                {
                    RoomUser bot = null;
                    if (RP.Room.GetRoomUserManager().TryGetBot(RP.lockID, out bot))
                        bot.BotData.lockedon--;
                    if (RP.lockBot == RP.lockID)
                        RP.lockBot = 0;

                }
                else
                {
                    var client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(RP.lockID);
                    if (client != null)
                        client.GetRolePlay().lockedon--;
                    if (RP.lockTarget == RP.lockID)
                        RP.lockTarget = 0;
                }
                RP.lockID = 0;
            }
        }

        public void SelectMeal(string Event, string Data, string ExtraData)
        {
            if (Event == "selectmeal")
            {
                if (ExtraData == "true")
                {
                    var item = RP.Room.GetRoomItemHandler().GetItem(RP.InteractID);
                    if (item != null)
                    {
                        int foodID = 0;
                        if (Data == "meal_1")
                            foodID = 1209;
                        else if (Data == "meal_2")
                            foodID = 1208;
                        else if (Data == "meal_3")
                            foodID = 1207;
                        else if (Data == "meal_4")
                            foodID = 1205;
                        else if (Data == "meal_5")
                            foodID = 1206;
                        int id = PlusEnvironment.GetRandomNumber(1000, 99999);
                        RP.UpdateEnergy(5);
                        var emptyplate = new Item(id + 1, RP.Room.Id, foodID, "", RP.Room.Model.DoorX, RP.Room.Model.DoorY, 0.0, 0, RP.Room.OwnerId, 0, 0, 0, string.Empty);
                        RP.Room.GetRoomItemHandler().RemoveRoomItem(item, RP.habbo.Id);
                        RP.roomUser.OnChat(0, "Dinner is served!", false);
                        if (RP.Room.GetRoomItemHandler().PlaceItem(emptyplate, item.GetX, item.GetY, 0, true, false, true))
                        { }
                    }
                }
                RP.InteractID = 0;
            }
        }

        public void PoliceCall(string Event, string Data, string ExtraData)
        {
            if (Event == "911")
            {
                RP.PoliceCalls = RP.habbo.GetClientManager().GetPoliceCall(RP.JobManager.Curpage);
                if (RP.habbo.GetClientManager().PoliceCalls == 0 || RP.PoliceCalls == null)
                {
                    RP.GetClient().SendWhisper("There is currently no help requests!");
                    return;
                }
                RP.SendWeb("{\"name\":\"911\", \"username\":\"" + RP.PoliceCalls.Name + "\","
                    + "\"roomname\":\"" + RP.PoliceCalls.RoomName + "\", \"roomid\":\"" + RP.PoliceCalls.RoomID + "\","
                    + "\"look\":\"" + RP.PoliceCalls.Look + "\", \"msg\":\"" + RP.PoliceCalls.Message + "\", \"pagestart\":\"" + RP.JobManager.Curpage + "\","
                    + "\"pageend\":\"" + RP.habbo.GetClientManager().PoliceCalls + "\", \"color\":\"" + RP.PoliceCalls.Color + "\", \"time\":\"" + RP.PoliceCalls.Time + "\", \"bypass\":\"" + ExtraData + "\"}");

            }
            else if (Event == "911next")
            {
                if (RP.PoliceCalls == null || RP.habbo.GetClientManager().PoliceCalls <= 1 || RP.JobManager.Curpage == RP.habbo.GetClientManager().PoliceCalls)
                    return;
                RP.JobManager.Curpage++;
                Handle("911", "", "null");
            }
            else if (Event == "911back")
            {
                if (RP.JobManager.Curpage > 1)
                {
                    RP.JobManager.Curpage--;
                    Handle("911", "", "null");
                }
            }
            else if (Event == "911room")
                RP.Taxi(Convert.ToInt32(RP.PoliceCalls.RoomID));
            else if (Event == "911clear")
            {
                if ((RP.JobManager.Working && RP.JobManager.Job == 1) || RP.onduty)
                    RP.habbo.GetClientManager().RemovePC(RP.PoliceCalls.ID);
            }
        }

        public void Color(string Event, string Data, string ExtraData)
        {
            if (Event == "opencolor")
                RP.SendWeb("{\"name\":\"colorbox\", \"color\":\"" + RP.Color + "\"}");
            else if (Event == "namecolor")
            {
                if (Data == "null")
                    RP.GetClient().SendWhisper("You did not select a color!");
                else
                {
                    if (RP.habbo.Credits < 25)
                    {
                        RP.GetClient().SendWhisper("You do not have enough money to purchase this color!");
                        return;
                    }
                    if (RP.Color == Data)
                    {
                        RP.GetClient().SendWhisper("You can not purchase the same color!");
                        return;
                    }
                    if (Data == "#dbdbdb" || Data == "#dbdada" || Data == "#dddddd")
                    {
                        RP.client.SendWhisper("You can not purchase this color!");
                        return;
                    }
                    RP.Color = Data;
                    RP.UpdateCredits(25, false);
                    using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        DB.RunQuery("UPDATE stats SET color = '" + RP.Color + "' WHERE id = '" + RP.habbo.Id + "'");

                }
            }
            else if (Event == "globalcolor")
            {
                if (RP.Color1 == Data && RP.Color2 == ExtraData)
                {
                    RP.client.SendWhisper("You have already performed this aciton!");
                    return;
                }
                RP.Color1 = Data;
                RP.Color2 = ExtraData;
                using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    DB.RunQuery("UPDATE stats SET color1 = '" + RP.Color1 + "', color2 = '" + RP.Color2 + "' WHERE id = '" + RP.habbo.Id + "'");
            }
        }

        public void ATM(string Event, string Data, string ExtraData)
        {
            if (Event == "atm_balance")
            {
                var balance = string.Format("{0:n0}", RP.Storage.bankamount);
                RP.SendWeb("{\"name\":\"atm_balance\", \"balance\":\"" + balance + "\"}");
            }
            else if (Event == "atm")
            {
                if (String.IsNullOrEmpty(ExtraData))
                {
                    RP.client.SendWhisper("Please type in a digit!");
                    return;
                }
                if (!ExtraData.All(char.IsDigit))
                {
                    RP.client.SendWhisper("You can only type in numbers!");
                    return;
                }
                if (ExtraData.Length > 9)
                {
                    if (Data == "true")
                        RP.client.SendWhisper("You are over the withdrawal limit!");
                    else RP.client.SendWhisper("You are over the deposit limit!");
                    return;
                }
                int amount = Convert.ToInt32(ExtraData);
                if (amount == 0)
                {
                    RP.client.SendWhisper("Invalid number!");
                    return;
                }
                if (RP.atmCD > 0)
                {
                    RP.Responds();
                    return;
                }
                double Tax = 0;
                if (Data == "withdraw")
                {
                    if (RP.Storage.bankamount < amount)
                    {
                        RP.client.SendWhisper("You do not have enough money in your account!");
                        return;
                    }
                    var money = string.Format("{0:n0}", amount);
                    RP.Storage.bankamount -= amount;
                    double tax = amount * 0.15;
                    Tax = tax;
                    if (RP.Storage.bankamount - Convert.ToInt32(tax) <= 0)
                    {
                        amount -= Convert.ToInt32(tax);
                        RP.GetClient().SendWhisper("A portion of your withdrawal has been taxed!");
                    }
                    else
                        RP.Storage.bankamount -= Convert.ToInt32(tax);
                    RP.UpdateCredits(amount, true);
                    if (amount == 1)
                        RP.Say("withdraws " + money + " dollar from their account", false);
                    else RP.Say("withdraws " + money + " dollars from their account", false);
                    var Item = RP.Room.GetRoomItemHandler().GetItem(RP.InteractATM);
                    Item.ExtraData = "1";
                    Item.UpdateState();
                }
                else if (Data == "deposit")
                {
                    if (RP.habbo.Credits < amount)
                    {
                        RP.client.SendWhisper("You do not have enough money to deposit!");
                        return;
                    }
                    var money = string.Format("{0:n0}", amount);
                    RP.Storage.bankamount += amount;
                    double tax = amount * 0.15;
                    Tax = tax;
                    if (RP.Storage.bankamount - Convert.ToInt32(tax) <= 0)
                        RP.Storage.bankamount = 0;
                    else
                        RP.Storage.bankamount -= Convert.ToInt32(tax);
                    RP.UpdateCredits(amount, false);
                    if (amount == 1)
                        RP.Say("deposits " + money + " dollar into their account", false);
                    else RP.Say("deposits " + money + " dollars into their account", false);
                }
                if (Convert.ToInt32(Tax) >= 1)
                    RP.client.SendWhisper("You have been taxed " + Convert.ToInt32(Tax) + " dollar(s) for this transaction!");
                RP.RPCache(22);
                RP.atmCD = 10;
            }
        }

        public void SlotMachine(string Event, string Data, string ExtraData)
        {
            if (Event == "spin")
            {
                if (RP.Spinning)
                    return;
                if (RP.habbo.Credits < 10)
                {
                    RP.client.SendWhisper("You do not have enough money to perform this action!");
                    return;
                }
                RP.UpdateCredits(5, false);
                RP.Spinning = true;
                RP.Say("inserts 5 dollars into the slot machine", false);
                RP.SendWeb("{\"name\":\"spin\", \"spin1\":\"true\", \"spin3\":\"true\", \"spin2\":\"true\"}");
            }
        }

        public void Gang(string Event, string Data, string ExtraData)
        {
            #region gang
            /*
            #region View Gang
            if (Event == "gang")
            {
                if (Gang == 0)
                    SendWeb("{\"name\":\"gang\", \"data\":\"false\"}");
                else
                {
                    int xp2 = 0;
                    int xp1 = 0;
                    if (GangManager.Level == 1)
                    {
                        xp2 = GangManager.XPdue;
                        xp1 = GangManager.XP;
                    }
                    else
                    {
                        xp2 = GangManager.XPdue / 2;
                        xp1 = GangManager.XP - xp2;
                    }
                    int turfs = 0;
                    if (habbo.GetClientManager().SouthTurf == GangManager.Name)
                        turfs += 1;
                    if (habbo.GetClientManager().WestTurf == GangManager.Name)
                        turfs += 1;
                    if (habbo.GetClientManager().EastTurf == GangManager.Name)
                        turfs += 1;
                    if (habbo.GetClientManager().NorthTurf == GangManager.Name)
                        turfs += 1;
                    string owner = "member";
                    int members = 0;
                    if (GangManager.Id10 > 0)
                        members++;
                    if (GangManager.Id9 > 0)
                        members++;
                    if (GangManager.Id8 > 0)
                        members++;
                    if (GangManager.Id7 > 0)
                        members++;
                    if (GangManager.Id6 > 0)
                        members++;
                    if (GangManager.Id5 > 0)
                        members++;
                    if (GangManager.Id4 > 0)
                        members++;
                    if (GangManager.Id3 > 0)
                        members++;
                    if (GangManager.Id2 > 0)
                        members++;
                    if (GangManager.Id1 > 0)
                        members++;
                    string mem = "member";
                    if (members > 1)
                        mem = "members";
                    if (GangBoss)
                        owner = "owner";
                    SendWeb("{\"name\":\"gang\", \"data\":\"" + owner + "\", \"level\":\"Level " + GangManager.Level + "\", \"xptext\":\"" + GangManager.XP + "/" + GangManager.XPdue + "\","
                    + "\"xp1\":\"" + xp1 + "\", \"xp2\":\"" + xp2 + "\", \"look\":\"" + habbo.Look + "\", \"mykills\":\"Kills: <b>" + kills2 + "</b>\", \"myhits\":\"Punches: <b>" + punches2 + "</b>\","
                    + "\"myarrests\":\"Arrests: <b>" + arrests2 + "</b>\", \"myrank\":\"Rank: <b>" + RankName + "</b>\", \"myjb\":\"Jail breaks: <b>" + jailbreak + "</b>\", \"mycaptures\":\"Captures: <b>" + captures + "</b>\","
                    + "\"gangname\":\"" + GangManager.Name + "\", \"gangkills\":\"Total Kills: <b>" + GangManager.Kills + "</b>\", \"gangarrests\":\"Total Arrests: <b>" + GangManager.Arrests + "</b>\","
                    + "\"ganghits\":\"Total Punches: <b>" + GangManager.Hits + "</b>\", \"gangjb\":\"Total Jail breaks: <b>" + GangManager.JB + "</b>\", \"gangturfs\":\"Turfs Captured: <b>" + turfs + "</b>\","
                    + "\"gangrank\":\"Top Ranks: <b>#" + PlusEnvironment.GetRandomNumber(1, 10) + "</b>\", \"color1\":\"" + GangManager.Color1 + "\", \"color2\":\"" + GangManager.Color2 + "\", \"members\":\"" + members + " " + mem + "\"}");
                }
            }
            #endregion

            #region Create Gang
            if (Event == "cgang")
            {
                if (roomUser.Stunned > 0 || Cuffed || GameType != "" || GP > 0)
                {
                    Responds();
                    return;
                }
                if (String.IsNullOrEmpty(ExtraData))
                    return;
                if (Gang > 0)
                {
                    client.SendWhisper("You are already in a gang!");
                    return;
                }
                if (habbo.Credits < 75)
                {
                    client.SendWhisper("You need 75 coins to create your own gang!");
                    return;
                }
                if (!CheckName(ExtraData))
                {
                    if (ExtraData == "null" || ExtraData == "Null")
                    {
                        client.SendWhisper("Invalid name!");
                        return;
                    }
                    if (ExtraData.Length > 20)
                        client.SendWhisper("This name is too long!");
                    else if (ExtraData.Length <= 2)
                        client.SendWhisper("This name is too short!");
                    else CreateGang(ExtraData);
                }
                else client.SendWhisper("Invalid name!");

            }
            #endregion

            #region Gang Invite
            if (Event == "ganginvite")
            {
                if (String.IsNullOrEmpty(ExtraData))
                    return;
                if (GangBoss || GangRank == 1)
                {
                    var User = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(ExtraData);
                    if (User == null)
                    {
                        client.SendWhisper("User not found!");
                        return;
                    }
                    if (User.GetRolePlay().roomUser == roomUser)
                    {
                        client.SendWhisper("This aciton can not be performed on yourself!");
                        return;
                    }
                    if (User.GetRolePlay().GP > 0)
                    {
                        client.SendWhisper("This user is currently under god protection and cannot be invited!");
                        return;
                    }
                    if (User.GetRolePlay().Gang == Gang)
                    {
                        client.SendWhisper("This user is already in your gang!");
                        return;
                    }
                    if (User.GetRolePlay().GangInvite == Gang)
                    {
                        client.SendWhisper("You have already sent this user an invitation!");
                        return;
                    }
                    if (GangManager.Id10 > 0)
                    {
                        client.SendWhisper("Your gang has already reached a maximum of 10 memebers!");
                        return;
                    }
                    if (GangManager.Rank1 == "null")
                    {
                        client.SendWhisper("You must first add a rank before inviting other users!");
                        return;
                    }
                    User.GetRolePlay().GangInvite = Gang;
                    User.SendWhisper("You recieved an invitation to join " + GangManager.Name + ", type ':gangaccept' to accept the following invitation!");
                    SendWeb("{\"name\":\"ganginvite\", \"invite\":\"true\"}");
                }
                else client.SendWhisper("You can not invite other users at your current rank!");

            }
            #endregion

            #region Basic
            if (Event == "leavegang")
            {
                if (GangBoss)
                {
                    client.SendWhisper("You have successfully deleted " + GangManager.Name + "!");
                    GangDelete();
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang)
                        {
                            _client.SendNotifi("" + GangManager.Name + " has been disbanded!");
                            _client.GetRolePlay().LeaveGang();
                        }
                }
                else
                {
                    client.SendWhisper("You successfully left " + GangManager.Name + "!");
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang && _client != client)
                            _client.SendWhisper(habbo.Username + " has left the gang!");

                    LeaveGang(true);
                }
            }
            if (Event == "gangcolor")
            {

                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                    if (_client.GetRolePlay().Gang == Gang)
                    {
                        _client.GetRolePlay().GangManager.Color1 = Data;
                        _client.GetRolePlay().GangManager.Color2 = ExtraData;
                        GangManager.SaveGang();
                    }
                GangManager.SaveGang();
            }
            #endregion

            #region Gang Rank
            if (Event == "gangrank")
            {
                if (String.IsNullOrEmpty(ExtraData))
                    return;
                if (!CheckName(ExtraData))
                {
                    if (GangBoss || GangRank == 1)
                    {
                        if (ExtraData.Length > 15)
                            client.SendWhisper("This name is too long!");
                        else if (ExtraData.Length <= 2)
                            client.SendWhisper("This name is too short!");
                        else
                        {
                            if (ExtraData == GangManager.Rank1 || ExtraData == GangManager.Rank2 || ExtraData == GangManager.Rank3 || ExtraData == GangManager.Rank4 || ExtraData == GangManager.Rank5)
                            {
                                if (ExtraData == "null" || ExtraData == "null")
                                    client.SendWhisper("Invalid name!");
                                else client.SendWhisper("This name has already been created!");
                                return;
                            }
                            if (GangManager.Rank1 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rank1 = ExtraData;
                            }
                            else if (GangManager.Rank2 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rank2 = ExtraData;
                            }
                            else if (GangManager.Rank3 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rank3 = ExtraData;
                            }
                            else if (GangManager.Rank4 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rank4 = ExtraData;
                            }
                            else if (GangManager.Rank5 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rank5 = ExtraData;
                            }
                            else
                            {
                                client.SendWhisper("You have already reached a maximum of 5 ranks!");
                                return;
                            }
                        }
                        GangManager.SaveGang();
                        SendWeb("{\"name\":\"ganginvite\", \"invite\":\"false\"}");
                    }

                    else client.SendWhisper("This action can not be performed at your current rank!");
                }
            }
            #endregion

            #region Gang Kick
            if (Event == "gangcancel")
            {
                if (GangBoss || GangRank == 1)
                {
                    #region find user
                    using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {

                        if (ExtraData == "row1_cancel2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_2);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row1_cancel3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_3);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row1_cancel4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_4);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row1_cancel5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_5);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row2_cancel1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_1);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row2_cancel2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_2);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row2_cancel3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_3);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row2_cancel4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_4);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row2_cancel5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_5);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row3_cancel1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_1);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row3_cancel2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_2);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row3_cancel3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_3);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row3_cancel4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_4);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row3_cancel5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_5);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row4_cancel1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_1);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row4_cancel2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_2);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row4_cancel3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_3);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row4_cancel4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_4);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row4_cancel5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_5);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row5_cancel1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_1);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row5_cancel2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_2);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row5_cancel3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_3);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row5_cancel4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_4);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                        else if (ExtraData == "row5_cancel5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_5);
                            if (Id.InRoom)
                            {
                                if (Id.GetClient().GetRolePlay().GangRank == 1 && !GangBoss)
                                    return;
                                else { Id.GetClient().GetRolePlay().LeaveGang(true); Id.GetClient().SendWhisper("You have been kicked from " + GangManager.Name + ""); }
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList()) if (_client.GetRolePlay().Gang == Gang) _client.GetRolePlay().GangManager.Clear(Id.Id); DB.RunQuery("UPDATE stats SET gang = '0', gangrank = '0' WHERE id = '" + Id.Id + "'");
                            }
                        }
                    }
                    #endregion
                    WebHandle("gangmembers", "", "");
                }
                else client.SendWhisper("This action can not be performed at your current rank!");

            }
            #endregion

            #region Gang Promote
            if (Event == "gangup")
            {
                if (GangBoss || GangRank == 1)
                {
                    using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        if (ExtraData == "row2_up1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_up2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_up3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_up4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_up5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_up1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_up2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_up3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_up4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_up5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_up1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_up2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_up3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_up4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_up5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row5_up1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row5_up2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row5_up3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row5_up4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row5_up5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row5_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank--;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank - '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else return;
                    }
                    WebHandle("gangmembers", "", "");
                }
                else client.SendWhisper("This action can not be performed at your current rank!");
            }
            #endregion

            #region Gang Demote
            if (Event == "gangdown")
            {
                if (GangBoss || GangRank == 1)
                {
                    using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {

                        if (ExtraData.Contains("row1"))
                        {
                            if (!GangBoss)
                                return;
                        }
                        if ((ExtraData.Contains("row1") && GangManager.Rank2 == "null")
                            || (ExtraData.Contains("row2") && GangManager.Rank3 == "null")
                            || (ExtraData.Contains("row3") && GangManager.Rank4 == "null")
                            || (ExtraData.Contains("row4") && GangManager.Rank5 == "null"))
                            return;
                        if (ExtraData == "row1_down1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row1_down2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row1_down3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row1_down4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row1_down5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row1_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_down1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_down2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_down3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_down4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row2_down5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row2_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_down1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_down2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_down3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_down4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row3_down5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row3_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_down1")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_1);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_down2")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_2);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_down3")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_3);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_down4")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_4);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }
                        else if (ExtraData == "row4_down5")
                        {
                            var Id = PlusEnvironment.GetHabboById(GangManager.row4_5);

                            if (Id.InRoom)
                            {
                                Id.GetClient().GetRolePlay().GangRank++;
                                Id.GetClient().GetRolePlay().SetGangRank();
                            }
                            else
                                DB.RunQuery("UPDATE stats SET gangrank = gangrank + '1' WHERE id = '" + Id.Id + "'");
                            foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                if (_client.GetRolePlay().Gang == Gang)
                                    _client.GetRolePlay().GangManager.Clear(Id.Id, true);
                        }

                        else return;
                    }
                    WebHandle("gangmembers", "", "");
                }
                else client.SendWhisper("This action can not be performed at your current rank!");
            }
            #endregion

            #region Gang Rival
            if (Event == "rival")
            {
                string owner = "";
                if (GangBoss || GangRank == 1)
                    owner = "owner";
                SendWeb("{\"name\":\"gangrival\", \"data\":\"" + owner + "\", \"ally1\":\"" + GangManager.Ally1 + "\", \"ally2\":\"" + GangManager.Ally2 + "\", \"rival1\":\"" + GangManager.Rival1 + "\", \"rival2\":\"" + GangManager.Rival2 + "\"}");
            }
            if (Event == "rivalcancel")
            {
                if (ExtraData == "ally2")
                {
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang)
                            _client.GetRolePlay().GangManager.Ally2 = "null";
                }
                else if (ExtraData == "ally1")
                {
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang)
                        {
                            if (GangManager.Ally2 != "null")
                            {
                                _client.GetRolePlay().GangManager.Ally1 = GangManager.Ally2;
                                _client.GetRolePlay().GangManager.Ally2 = "null";
                            }
                            else _client.GetRolePlay().GangManager.Ally1 = "null";
                        }
                }
                else if (ExtraData == "rival")
                {
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang)
                            _client.GetRolePlay().GangManager.Rival2 = "null";
                }
                else if (ExtraData == "rival1")
                {
                    foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                        if (_client.GetRolePlay().Gang == Gang)
                        {
                            if (GangManager.Rival2 != "null")
                            {
                                _client.GetRolePlay().GangManager.Rival1 = GangManager.Rival2;
                                _client.GetRolePlay().GangManager.Rival2 = "null";
                            }
                            else _client.GetRolePlay().GangManager.Rival1 = "null";
                        }
                }
                WebHandle("rival", "", "");
                GangManager.SaveGang();
            }
            if (Event == "gangrival")
            {
                if (String.IsNullOrEmpty(ExtraData))
                    return;
                if (ExtraData.Length > 20)
                    client.SendWhisper("This name is too long!");
                else if (ExtraData.Length <= 2)
                    client.SendWhisper("This name is too short!");
                else
                {
                    if (GangBoss || GangRank == 1)
                    {
                        if ((ExtraData == "null" || ExtraData == "Null") || ExtraData == GangManager.Name)
                        {
                            client.SendWhisper("Invalid name!");
                            return;
                        }
                        if (GangManager.Rival1 == ExtraData || GangManager.Rival2 == ExtraData || GangManager.Ally1 == ExtraData || GangManager.Ally2 == ExtraData)
                        {
                            client.SendWhisper("You can not input the same gang!");
                            return;
                        }
                        if (Data == "1")
                        {
                            if (GangManager.Ally1 != "null" && GangManager.Ally2 != "null")
                            {
                                client.SendWhisper("You can only add a maximum of two allies!");
                                return;
                            }
                            if (GangManager.Ally1 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Ally1 = ExtraData;
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Ally2 = ExtraData;
                            }
                        }
                        else if (Data == "2")
                        {
                            if (GangManager.Rival1 != "null" && GangManager.Rival2 != "null")
                            {
                                client.SendWhisper("You can only add a maximum of two enemies!");
                                return;
                            }
                            if (GangManager.Rival1 == "null")
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rival1 = ExtraData;
                            }
                            else
                            {
                                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                                    if (_client.GetRolePlay().Gang == Gang)
                                        _client.GetRolePlay().GangManager.Rival2 = ExtraData;
                            }
                        }
                        WebHandle("rival", "", "");
                        GangManager.SaveGang();
                    }
                    else client.SendWhisper("This action can not be performed at your current rank!");
                }
            }
            #endregion

            #region Gang Members
            if (Event == "gangmembers")
            {
                if (GangManager.Rank1 == "null")
                {
                    client.SendWhisper("You must first create a rank before viewing this window!");
                    return;
                }
                #region Find Position
                foreach (GameClient _client in habbo.GetClientManager()._clients.Values.ToList())
                {
                    if (_client.GetRolePlay().Gang == Gang)
                    {
                        using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            _client.GetRolePlay().GangManager.row1_1 = _client.GetRolePlay().GangManager.Id1;
                            if (_client.GetRolePlay().GangManager.Id2 != 0)
                            {
                                var Id = PlusEnvironment.GetHabboById(_client.GetRolePlay().GangManager.Id2);
                                int gangrank = 0;
                                if (Id.InRoom)
                                    gangrank = Id.GetClient().GetRolePlay().GangRank;
                                else
                                {
                                    DB.SetQuery("SELECT gangrank FROM `stats` WHERE id = '" + _client.GetRolePlay().GangManager.Id2 + "' LIMIT 1");
                                    gangrank = DB.getInteger();
                                }

                                if (gangrank == 1)
                                {
                                    if (_client.GetRolePlay().GangManager.row1_1 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row1_2 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row1_3 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row1_4 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row1_5 == _client.GetRolePlay().GangManager.Id2)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row1_1 == 0)
                                            _client.GetRolePlay().GangManager.row1_1 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row1_2 == 0)
                                            _client.GetRolePlay().GangManager.row1_2 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row1_3 == 0)
                                            _client.GetRolePlay().GangManager.row1_3 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row1_4 == 0)
                                            _client.GetRolePlay().GangManager.row1_4 = _client.GetRolePlay().GangManager.Id2;
                                        else _client.GetRolePlay().GangManager.row1_5 = _client.GetRolePlay().GangManager.Id2;
                                    }
                                }
                                else if (gangrank == 2)
                                {
                                    if (_client.GetRolePlay().GangManager.row2_1 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row2_2 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row2_3 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row2_4 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row2_5 == _client.GetRolePlay().GangManager.Id2)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row2_1 == 0)
                                            _client.GetRolePlay().GangManager.row2_1 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row2_2 == 0)
                                            _client.GetRolePlay().GangManager.row2_2 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row2_3 == 0)
                                            _client.GetRolePlay().GangManager.row2_3 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row2_4 == 0)
                                            _client.GetRolePlay().GangManager.row2_4 = _client.GetRolePlay().GangManager.Id2;
                                        else _client.GetRolePlay().GangManager.row2_5 = _client.GetRolePlay().GangManager.Id2;
                                    }
                                }
                                else if (gangrank == 3)
                                {
                                    if (_client.GetRolePlay().GangManager.row3_1 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row3_2 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row3_3 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row3_4 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row3_5 == _client.GetRolePlay().GangManager.Id2)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row3_1 == 0)
                                            _client.GetRolePlay().GangManager.row3_1 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row3_2 == 0)
                                            _client.GetRolePlay().GangManager.row3_2 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row3_3 == 0)
                                            _client.GetRolePlay().GangManager.row3_3 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row3_4 == 0)
                                            _client.GetRolePlay().GangManager.row3_4 = _client.GetRolePlay().GangManager.Id2;
                                        else _client.GetRolePlay().GangManager.row3_5 = _client.GetRolePlay().GangManager.Id2;
                                    }
                                }
                                else if (gangrank == 4)
                                {
                                    if (_client.GetRolePlay().GangManager.row4_1 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row4_2 == _client.GetRolePlay().GangManager.Id2
               || _client.GetRolePlay().GangManager.row4_3 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row4_4 == _client.GetRolePlay().GangManager.Id2
               || _client.GetRolePlay().GangManager.row4_5 == _client.GetRolePlay().GangManager.Id2)
                                    { }
                                    else
                                    {

                                        if (_client.GetRolePlay().GangManager.row4_1 == 0)
                                            _client.GetRolePlay().GangManager.row4_1 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row4_2 == 0)
                                            _client.GetRolePlay().GangManager.row4_2 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row4_3 == 0)
                                            _client.GetRolePlay().GangManager.row4_3 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row4_4 == 0)
                                            _client.GetRolePlay().GangManager.row4_4 = _client.GetRolePlay().GangManager.Id2;
                                        else _client.GetRolePlay().GangManager.row4_5 = _client.GetRolePlay().GangManager.Id2;
                                    }

                                }
                                else if (gangrank == 5)
                                {
                                    if (_client.GetRolePlay().GangManager.row5_1 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row5_2 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row5_3 == _client.GetRolePlay().GangManager.Id2 || _client.GetRolePlay().GangManager.row5_4 == _client.GetRolePlay().GangManager.Id2
        || _client.GetRolePlay().GangManager.row5_5 == _client.GetRolePlay().GangManager.Id2)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row5_1 == 0)
                                            _client.GetRolePlay().GangManager.row5_1 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row5_2 == 0)
                                            _client.GetRolePlay().GangManager.row5_2 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row5_3 == 0)
                                            _client.GetRolePlay().GangManager.row5_3 = _client.GetRolePlay().GangManager.Id2;
                                        else if (_client.GetRolePlay().GangManager.row5_4 == 0)
                                            _client.GetRolePlay().GangManager.row5_4 = _client.GetRolePlay().GangManager.Id2;
                                        else _client.GetRolePlay().GangManager.row5_5 = _client.GetRolePlay().GangManager.Id2;
                                    }
                                }
                            }
                            if (_client.GetRolePlay().GangManager.Id3 != 0)
                            {
                                var Id = PlusEnvironment.GetHabboById(_client.GetRolePlay().GangManager.Id3);
                                int gangrank = 0;
                                if (Id.InRoom)
                                    gangrank = Id.GetClient().GetRolePlay().GangRank;
                                else
                                {
                                    DB.SetQuery("SELECT gangrank FROM `stats` WHERE id = '" + _client.GetRolePlay().GangManager.Id3 + "' LIMIT 1");
                                    gangrank = DB.getInteger();
                                }

                                if (gangrank == 1)
                                {
                                    if (_client.GetRolePlay().GangManager.row1_1 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row1_2 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row1_3 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row1_4 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row1_5 == _client.GetRolePlay().GangManager.Id3)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row1_1 == 0)
                                            _client.GetRolePlay().GangManager.row1_1 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row1_2 == 0)
                                            _client.GetRolePlay().GangManager.row1_2 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row1_3 == 0)
                                            _client.GetRolePlay().GangManager.row1_3 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row1_4 == 0)
                                            _client.GetRolePlay().GangManager.row1_4 = _client.GetRolePlay().GangManager.Id3;
                                        else _client.GetRolePlay().GangManager.row1_5 = _client.GetRolePlay().GangManager.Id3;
                                    }
                                }
                                else if (gangrank == 2)
                                {
                                    if (_client.GetRolePlay().GangManager.row2_1 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row2_2 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row2_3 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row2_4 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row2_5 == _client.GetRolePlay().GangManager.Id3)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row2_1 == 0)
                                            _client.GetRolePlay().GangManager.row2_1 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row2_2 == 0)
                                            _client.GetRolePlay().GangManager.row2_2 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row2_3 == 0)
                                            _client.GetRolePlay().GangManager.row2_3 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row2_4 == 0)
                                            _client.GetRolePlay().GangManager.row2_4 = _client.GetRolePlay().GangManager.Id3;
                                        else _client.GetRolePlay().GangManager.row2_5 = _client.GetRolePlay().GangManager.Id3;
                                    }
                                }
                                else if (gangrank == 3)
                                {
                                    if (_client.GetRolePlay().GangManager.row3_1 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row3_2 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row3_3 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row3_4 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row3_5 == _client.GetRolePlay().GangManager.Id3)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row3_1 == 0)
                                            _client.GetRolePlay().GangManager.row3_1 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row3_2 == 0)
                                            _client.GetRolePlay().GangManager.row3_2 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row3_3 == 0)
                                            _client.GetRolePlay().GangManager.row3_3 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row3_4 == 0)
                                            _client.GetRolePlay().GangManager.row3_4 = _client.GetRolePlay().GangManager.Id3;
                                        else _client.GetRolePlay().GangManager.row3_5 = _client.GetRolePlay().GangManager.Id3;
                                    }
                                }
                                else if (gangrank == 4)
                                {
                                    if (_client.GetRolePlay().GangManager.row4_1 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row4_2 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row4_3 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row4_4 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row4_5 == _client.GetRolePlay().GangManager.Id3)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row4_1 == 0)
                                            _client.GetRolePlay().GangManager.row4_1 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row4_2 == 0)
                                            _client.GetRolePlay().GangManager.row4_2 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row4_3 == 0)
                                            _client.GetRolePlay().GangManager.row4_3 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row4_4 == 0)
                                            _client.GetRolePlay().GangManager.row4_4 = _client.GetRolePlay().GangManager.Id3;
                                        else _client.GetRolePlay().GangManager.row4_5 = _client.GetRolePlay().GangManager.Id3;
                                    }
                                }
                                else if (gangrank == 5)
                                {
                                    if (_client.GetRolePlay().GangManager.row5_1 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row5_2 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row5_3 == _client.GetRolePlay().GangManager.Id3 || _client.GetRolePlay().GangManager.row5_4 == _client.GetRolePlay().GangManager.Id3
        || _client.GetRolePlay().GangManager.row5_5 == _client.GetRolePlay().GangManager.Id3)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row5_1 == 0)
                                            _client.GetRolePlay().GangManager.row5_1 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row5_2 == 0)
                                            _client.GetRolePlay().GangManager.row5_2 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row5_3 == 0)
                                            _client.GetRolePlay().GangManager.row5_3 = _client.GetRolePlay().GangManager.Id3;
                                        else if (_client.GetRolePlay().GangManager.row5_4 == 0)
                                            _client.GetRolePlay().GangManager.row5_4 = _client.GetRolePlay().GangManager.Id3;
                                        else _client.GetRolePlay().GangManager.row5_5 = _client.GetRolePlay().GangManager.Id3;
                                    }
                                }
                            }
                            if (_client.GetRolePlay().GangManager.Id4 != 0)
                            {
                                var Id = PlusEnvironment.GetHabboById(_client.GetRolePlay().GangManager.Id4);
                                int gangrank = 0;
                                if (Id.InRoom)
                                    gangrank = Id.GetClient().GetRolePlay().GangRank;
                                else
                                {
                                    DB.SetQuery("SELECT gangrank FROM `stats` WHERE id = '" + _client.GetRolePlay().GangManager.Id4 + "' LIMIT 1");
                                    gangrank = DB.getInteger();
                                }

                                if (gangrank == 1)
                                {
                                    if (_client.GetRolePlay().GangManager.row1_1 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row1_2 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row1_3 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row1_4 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row1_5 == _client.GetRolePlay().GangManager.Id4)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row1_1 == 0)
                                            _client.GetRolePlay().GangManager.row1_1 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row1_2 == 0)
                                            _client.GetRolePlay().GangManager.row1_2 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row1_3 == 0)
                                            _client.GetRolePlay().GangManager.row1_3 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row1_4 == 0)
                                            _client.GetRolePlay().GangManager.row1_4 = _client.GetRolePlay().GangManager.Id4;
                                        else _client.GetRolePlay().GangManager.row1_5 = _client.GetRolePlay().GangManager.Id4;
                                    }
                                }
                                else if (gangrank == 2)
                                {
                                    if (_client.GetRolePlay().GangManager.row2_1 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row2_2 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row2_3 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row2_4 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row2_5 == _client.GetRolePlay().GangManager.Id4)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row2_1 == 0)
                                            _client.GetRolePlay().GangManager.row2_1 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row2_2 == 0)
                                            _client.GetRolePlay().GangManager.row2_2 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row2_3 == 0)
                                            _client.GetRolePlay().GangManager.row2_3 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row2_4 == 0)
                                            _client.GetRolePlay().GangManager.row2_4 = _client.GetRolePlay().GangManager.Id4;
                                        else _client.GetRolePlay().GangManager.row2_5 = _client.GetRolePlay().GangManager.Id4;
                                    }
                                }
                                else if (gangrank == 3)
                                {
                                    if (_client.GetRolePlay().GangManager.row3_1 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row3_2 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row3_3 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row3_4 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row3_5 == _client.GetRolePlay().GangManager.Id4)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row3_1 == 0)
                                            _client.GetRolePlay().GangManager.row3_1 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row3_2 == 0)
                                            _client.GetRolePlay().GangManager.row3_2 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row3_3 == 0)
                                            _client.GetRolePlay().GangManager.row3_3 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row3_4 == 0)
                                            _client.GetRolePlay().GangManager.row3_4 = _client.GetRolePlay().GangManager.Id4;
                                        else _client.GetRolePlay().GangManager.row3_5 = _client.GetRolePlay().GangManager.Id4;
                                    }
                                }
                                else if (gangrank == 4)
                                {
                                    if (_client.GetRolePlay().GangManager.row4_1 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row4_2 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row4_3 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row4_4 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row4_5 == _client.GetRolePlay().GangManager.Id4)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row4_1 == 0)
                                            _client.GetRolePlay().GangManager.row4_1 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row4_2 == 0)
                                            _client.GetRolePlay().GangManager.row4_2 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row4_3 == 0)
                                            _client.GetRolePlay().GangManager.row4_3 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row4_4 == 0)
                                            _client.GetRolePlay().GangManager.row4_4 = _client.GetRolePlay().GangManager.Id4;
                                        else _client.GetRolePlay().GangManager.row4_5 = _client.GetRolePlay().GangManager.Id4;
                                    }
                                }
                                else if (gangrank == 5)
                                {
                                    if (_client.GetRolePlay().GangManager.row5_1 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row5_2 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row5_3 == _client.GetRolePlay().GangManager.Id4 || _client.GetRolePlay().GangManager.row5_4 == _client.GetRolePlay().GangManager.Id4
        || _client.GetRolePlay().GangManager.row5_5 == _client.GetRolePlay().GangManager.Id4)
                                    { }
                                    else
                                    {
                                        if (_client.GetRolePlay().GangManager.row5_1 == 0)
                                            _client.GetRolePlay().GangManager.row5_1 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row5_2 == 0)
                                            _client.GetRolePlay().GangManager.row5_2 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row5_3 == 0)
                                            _client.GetRolePlay().GangManager.row5_3 = _client.GetRolePlay().GangManager.Id4;
                                        else if (_client.GetRolePlay().GangManager.row5_4 == 0)
                                            _client.GetRolePlay().GangManager.row5_4 = _client.GetRolePlay().GangManager.Id4;
                                        else _client.GetRolePlay().GangManager.row5_5 = _client.GetRolePlay().GangManager.Id4;
                                    }
                                }
                            }
                            if (_client.GetRolePlay().GangManager.Id5 != 0)
                            {
                                var Id = PlusEnvironment.GetHabboById(_client.GetRolePlay().GangManager.Id5);
                                int gangrank = 0;
                                if (Id.InRoom)
                                    gangrank = Id.GetClient().GetRolePlay().GangRank;
                                else
                                {
                                    DB.SetQuery("SELECT gangrank FROM `stats` WHERE id = '" + _client.GetRolePlay().GangManager.Id5 + "' LIMIT 1");
                                    gangrank = DB.getInteger();
                                }

                                if (gangrank == 1)
                                {
                                    if (_client.GetRolePlay().GangManager.row1_1 == 0)
                                        _client.GetRolePlay().GangManager.row1_1 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row1_2 == 0)
                                        _client.GetRolePlay().GangManager.row1_2 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row1_3 == 0)
                                        _client.GetRolePlay().GangManager.row1_3 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row1_4 == 0)
                                        _client.GetRolePlay().GangManager.row1_4 = _client.GetRolePlay().GangManager.Id5;
                                    else _client.GetRolePlay().GangManager.row1_5 = _client.GetRolePlay().GangManager.Id5;
                                }
                                else if (gangrank == 2)
                                {
                                    if (_client.GetRolePlay().GangManager.row2_1 == 0)
                                        _client.GetRolePlay().GangManager.row2_1 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row2_2 == 0)
                                        _client.GetRolePlay().GangManager.row2_2 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row2_3 == 0)
                                        _client.GetRolePlay().GangManager.row2_3 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row2_4 == 0)
                                        _client.GetRolePlay().GangManager.row2_4 = _client.GetRolePlay().GangManager.Id5;
                                    else _client.GetRolePlay().GangManager.row2_5 = _client.GetRolePlay().GangManager.Id5;
                                }
                                else if (gangrank == 3)
                                {
                                    if (_client.GetRolePlay().GangManager.row3_1 == 0)
                                        _client.GetRolePlay().GangManager.row3_1 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row3_2 == 0)
                                        _client.GetRolePlay().GangManager.row3_2 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row3_3 == 0)
                                        _client.GetRolePlay().GangManager.row3_3 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row3_4 == 0)
                                        _client.GetRolePlay().GangManager.row3_4 = _client.GetRolePlay().GangManager.Id5;
                                    else _client.GetRolePlay().GangManager.row3_5 = _client.GetRolePlay().GangManager.Id5;
                                }
                                else if (gangrank == 4)
                                {
                                    if (_client.GetRolePlay().GangManager.row4_1 == 0)
                                        _client.GetRolePlay().GangManager.row4_1 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row4_2 == 0)
                                        _client.GetRolePlay().GangManager.row4_2 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row4_3 == 0)
                                        _client.GetRolePlay().GangManager.row4_3 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row4_4 == 0)
                                        _client.GetRolePlay().GangManager.row4_4 = _client.GetRolePlay().GangManager.Id5;
                                    else _client.GetRolePlay().GangManager.row4_5 = _client.GetRolePlay().GangManager.Id5;
                                }
                                else if (gangrank == 5)
                                {
                                    if (_client.GetRolePlay().GangManager.row5_1 == 0)
                                        _client.GetRolePlay().GangManager.row5_1 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row5_2 == 0)
                                        _client.GetRolePlay().GangManager.row5_2 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row5_3 == 0)
                                        _client.GetRolePlay().GangManager.row5_3 = _client.GetRolePlay().GangManager.Id5;
                                    else if (_client.GetRolePlay().GangManager.row5_4 == 0)
                                        _client.GetRolePlay().GangManager.row5_4 = _client.GetRolePlay().GangManager.Id5;
                                    else _client.GetRolePlay().GangManager.row5_5 = _client.GetRolePlay().GangManager.Id5;
                                }
                            }

                        }
                    }
                }
                #endregion

                #region Find Look & Name
                string owner = "member";
                if (GangBoss || GangRank == 1)
                    owner = "owner";
                string row1_1 = "null";
                string row1_2 = "null";
                string row1_3 = "null";
                string row1_4 = "null";
                string row1_5 = "null";

                string row2_1 = "null";
                string row2_2 = "null";
                string row2_3 = "null";
                string row2_4 = "null";
                string row2_5 = "null";

                string row3_1 = "null";
                string row3_2 = "null";
                string row3_3 = "null";
                string row3_4 = "null";
                string row3_5 = "null";

                string row4_1 = "null";
                string row4_2 = "null";
                string row4_3 = "null";
                string row4_4 = "null";
                string row4_5 = "null";

                string row5_1 = "null";
                string row5_2 = "null";
                string row5_3 = "null";
                string row5_4 = "null";
                string row5_5 = "null";

                string name1_1 = "null";
                string name1_2 = "null";
                string name1_3 = "null";
                string name1_4 = "null";
                string name1_5 = "null";

                string name2_1 = "null";
                string name2_2 = "null";
                string name2_3 = "null";
                string name2_4 = "null";
                string name2_5 = "null";

                string name3_1 = "null";
                string name3_2 = "null";
                string name3_3 = "null";
                string name3_4 = "null";
                string name3_5 = "null";

                string name4_1 = "null";
                string name4_2 = "null";
                string name4_3 = "null";
                string name4_4 = "null";
                string name4_5 = "null";

                string name5_1 = "null";
                string name5_2 = "null";
                string name5_3 = "null";
                string name5_4 = "null";
                string name5_5 = "null";

                string online1_1 = "offline";
                string online1_2 = "offline";
                string online1_3 = "offline";
                string online1_4 = "offline";
                string online1_5 = "offline";

                string online2_1 = "offline";
                string online2_2 = "offline";
                string online2_3 = "offline";
                string online2_4 = "offline";
                string online2_5 = "offline";

                string online3_1 = "offline";
                string online3_2 = "offline";
                string online3_3 = "offline";
                string online3_4 = "offline";
                string online3_5 = "offline";

                string online4_1 = "offline";
                string online4_2 = "offline";
                string online4_3 = "offline";
                string online4_4 = "offline";
                string online4_5 = "offline";

                string online5_1 = "offline";
                string online5_2 = "offline";
                string online5_3 = "offline";
                string online5_4 = "offline";
                string online5_5 = "offline";
                PlusEnvironment.GetHabboById(GangManager.row1_1);
                var id1_1 = PlusEnvironment.GetHabboById(GangManager.row1_1);
                if (id1_1 != null)
                {
                    row1_1 = id1_1.Look;
                    name1_1 = id1_1.Username + " <b>(owner)</b>";
                    if (id1_1.InRoom)
                        online1_1 = "online";
                }
                var id1_2 = PlusEnvironment.GetHabboById(GangManager.row1_2);
                if (id1_2 != null)
                {
                    row1_2 = id1_2.Look;
                    name1_2 = id1_2.Username;
                    if (id1_2.InRoom)
                        online1_2 = "online";
                }
                var id1_3 = PlusEnvironment.GetHabboById(GangManager.row1_3);
                if (id1_3 != null)
                {
                    row1_3 = id1_3.Look;
                    name1_3 = id1_3.Username;
                    if (id1_3.InRoom)
                        online1_3 = "online";
                }
                var id1_4 = PlusEnvironment.GetHabboById(GangManager.row1_4);
                if (id1_4 != null)
                {
                    row1_4 = id1_4.Look;
                    name1_4 = id1_4.Username;
                    if (id1_4.InRoom)
                        online1_4 = "online";
                }
                var id1_5 = PlusEnvironment.GetHabboById(GangManager.row1_5);
                if (id1_5 != null)
                {
                    row1_5 = id1_5.Look;
                    name1_5 = id1_5.Username;
                    if (id1_5.InRoom)
                        online1_5 = "online";
                }

                var id2_1 = PlusEnvironment.GetHabboById(GangManager.row2_1);
                if (id2_1 != null)
                {
                    row2_1 = id2_1.Look;
                    name2_1 = id2_1.Username;
                    if (id2_1.InRoom)
                        online2_1 = "online";
                }
                var id2_2 = PlusEnvironment.GetHabboById(GangManager.row2_2);
                if (id2_2 != null)
                {
                    row2_2 = id2_2.Look;
                    name2_2 = id2_2.Username;
                    if (id2_2.InRoom)
                        online2_2 = "online";
                }
                var id2_3 = PlusEnvironment.GetHabboById(GangManager.row2_3);
                if (id2_3 != null)
                {
                    row2_3 = id2_3.Look;
                    name2_3 = id2_3.Username;
                    if (id2_3.InRoom)
                        online2_3 = "online";
                }
                var id2_4 = PlusEnvironment.GetHabboById(GangManager.row2_4);
                if (id2_4 != null)
                {
                    row2_4 = id2_4.Look;
                    name2_4 = id2_4.Username;
                    if (id2_4.InRoom)
                        online2_4 = "online";
                }
                var id2_5 = PlusEnvironment.GetHabboById(GangManager.row2_5);
                if (id2_5 != null)
                {
                    row2_5 = id2_5.Look;
                    name2_5 = id2_5.Username;
                    if (id2_5.InRoom)
                        online2_5 = "online";
                }

                var id3_1 = PlusEnvironment.GetHabboById(GangManager.row3_1);
                if (id3_1 != null)
                {
                    row3_1 = id3_1.Look;
                    name3_1 = id3_1.Username;
                    if (id3_1.InRoom)
                        online3_1 = "online";
                }
                var id3_2 = PlusEnvironment.GetHabboById(GangManager.row3_2);
                if (id3_2 != null)
                {
                    row3_2 = id3_2.Look;
                    name3_2 = id3_2.Username;
                    if (id3_2.InRoom)
                        online3_2 = "online";
                }
                var id3_3 = PlusEnvironment.GetHabboById(GangManager.row3_3);
                if (id3_3 != null)
                {
                    row3_3 = id3_3.Look;
                    name3_3 = id3_3.Username;
                    if (id3_3.InRoom)
                        online3_3 = "online";
                }
                var id3_4 = PlusEnvironment.GetHabboById(GangManager.row3_4);
                if (id3_4 != null)
                {
                    row3_4 = id3_4.Look;
                    name3_4 = id3_4.Username;
                    if (id3_4.InRoom)
                        online3_4 = "online";
                }
                var id3_5 = PlusEnvironment.GetHabboById(GangManager.row3_5);
                if (id3_5 != null)
                {
                    row3_5 = id3_5.Look;
                    name3_5 = id3_5.Username;
                    if (id3_5.InRoom)
                        online3_5 = "online";
                }

                var id4_1 = PlusEnvironment.GetHabboById(GangManager.row4_1);
                if (id4_1 != null)
                {
                    row4_1 = id4_1.Look;
                    name4_1 = id4_1.Username;
                    if (id4_1.InRoom)
                        online4_1 = "online";
                }
                var id4_2 = PlusEnvironment.GetHabboById(GangManager.row4_2);
                if (id4_2 != null)
                {
                    row4_2 = id4_2.Look;
                    name4_2 = id4_2.Username;
                    if (id4_2.InRoom)
                        online4_2 = "online";
                }
                var id4_3 = PlusEnvironment.GetHabboById(GangManager.row4_3);
                if (id4_3 != null)
                {
                    row4_3 = id4_3.Look;
                    name4_3 = id4_3.Username;
                    if (id4_3.InRoom)
                        online4_3 = "online";
                }
                var id4_4 = PlusEnvironment.GetHabboById(GangManager.row4_4);
                if (id4_4 != null)
                {
                    row4_4 = id4_4.Look;
                    name4_4 = id4_4.Username;
                    if (id4_4.InRoom)
                        online4_4 = "online";
                }
                var id4_5 = PlusEnvironment.GetHabboById(GangManager.row4_5);
                if (id4_5 != null)
                {
                    row4_5 = id4_5.Look;
                    name4_5 = id4_5.Username;
                    if (id4_5.InRoom)
                        online4_5 = "online";
                }

                var id5_1 = PlusEnvironment.GetHabboById(GangManager.row5_1);
                if (id5_1 != null)
                {
                    row5_1 = id5_1.Look;
                    name5_1 = id5_1.Username;
                    if (id5_1.InRoom)
                        online5_1 = "online";
                }
                var id5_2 = PlusEnvironment.GetHabboById(GangManager.row5_2);
                if (id5_2 != null)
                {
                    row5_2 = id5_2.Look;
                    name5_2 = id5_2.Username;
                    if (id5_2.InRoom)
                        online5_2 = "online";
                }
                var id5_3 = PlusEnvironment.GetHabboById(GangManager.row5_3);
                if (id5_3 != null)
                {
                    row5_3 = id5_3.Look;
                    name5_3 = id5_3.Username;
                    if (id5_3.InRoom)
                        online5_3 = "online";
                }
                var id5_4 = PlusEnvironment.GetHabboById(GangManager.row5_4);
                if (id5_4 != null)
                {
                    row5_4 = id5_4.Look;
                    name5_4 = id5_4.Username;
                    if (id5_4.InRoom)
                        online5_4 = "online";
                }
                var id5_5 = PlusEnvironment.GetHabboById(GangManager.row5_5);
                if (id5_5 != null)
                {
                    row5_5 = id5_5.Look;
                    name5_5 = id5_5.Username;
                    if (id5_5.InRoom)
                        online5_5 = "online";
                }
                #endregion
                SendWeb("{\"name\":\"gangmembers\", \"data\":\"" + owner + "\", \"row1_1\":\"" + row1_1 + "\", \"name1_1\":\"" + name1_1 + "\", \"row1_2\":\"" + row1_2 + "\", \"name1_2\":\"" + name1_2 + "\","
   + "\"row1_3\":\"" + row1_3 + "\", \"name1_3\":\"" + name1_3 + "\", \"row1_4\":\"" + row1_4 + "\", \"name1_4\":\"" + name1_4 + "\", \"row1_5\":\"" + row1_5 + "\", \"name1_5\":\"" + name1_5 + "\", \"row2_1\":\"" + row2_1 + "\", \"name2_1\":\"" + name2_1 + "\", \"row2_2\":\"" + row2_2 + "\", \"name2_2\":\"" + name2_2 + "\","
   + "\"row2_3\":\"" + row2_3 + "\", \"name2_3\":\"" + name2_3 + "\", \"row2_4\":\"" + row2_4 + "\", \"name2_4\":\"" + name2_4 + "\", \"row2_5\":\"" + row2_5 + "\", \"name2_5\":\"" + name2_5 + "\","
   + "\"row3_1\":\"" + row3_1 + "\", \"name3_1\":\"" + name3_1 + "\", \"row3_2\":\"" + row3_2 + "\", \"name3_2\":\"" + name3_2 + "\", \"row3_3\":\"" + row3_3 + "\", \"name3_3\":\"" + name3_3 + "\","
   + "\"row3_4\":\"" + row3_4 + "\", \"name3_4\":\"" + name3_4 + "\", \"row3_5\":\"" + row3_5 + "\", \"name3_5\":\"" + name3_5 + "\", \"row4_1\":\"" + row4_1 + "\", \"name4_1\":\"" + name4_1 + "\","
   + "\"row4_2\":\"" + row4_2 + "\", \"name4_2\":\"" + name4_2 + "\", \"row4_3\":\"" + row4_3 + "\", \"name4_3\":\"" + name4_3 + "\", \"row4_4\":\"" + row4_4 + "\", \"name4_4\":\"" + name4_4 + "\","
   + "\"row4_5\":\"" + row4_5 + "\", \"name4_5\":\"" + name4_5 + "\", \"row5_1\":\"" + row5_1 + "\", \"name5_1\":\"" + name5_1 + "\", \"row5_2\":\"" + row5_2 + "\", \"name5_2\":\"" + name5_2 + "\","
   + "\"row5_3\":\"" + row5_3 + "\", \"name5_3\":\"" + name5_3 + "\", \"row5_4\":\"" + row5_4 + "\", \"name5_4\":\"" + name5_4 + "\", \"row5_5\":\"" + row5_5 + "\", \"name5_5\":\"" + name5_5 + "\","
   + "\"online1_1\":\"" + online1_1 + "\", \"online1_2\":\"" + online1_2 + "\", \"online1_3\":\"" + online1_3 + "\", \"online1_4\":\"" + online1_4 + "\", \"online1_5\":\"" + online1_5 + "\","
   + "\"online2_1\":\"" + online2_1 + "\", \"online2_2\":\"" + online2_2 + "\", \"online2_3\":\"" + online2_3 + "\", \"online2_4\":\"" + online2_4 + "\", \"online2_5\":\"" + online2_5 + "\","
   + "\"online3_1\":\"" + online3_1 + "\", \"online3_2\":\"" + online3_2 + "\", \"online3_3\":\"" + online3_3 + "\", \"online3_4\":\"" + online3_4 + "\", \"online3_5\":\"" + online3_5 + "\","
   + "\"online4_1\":\"" + online4_1 + "\", \"online4_2\":\"" + online4_2 + "\", \"online4_3\":\"" + online4_3 + "\", \"online4_4\":\"" + online4_4 + "\", \"online4_5\":\"" + online4_5 + "\","
   + "\"online5_1\":\"" + online5_1 + "\", \"online5_2\":\"" + online5_2 + "\", \"online5_3\":\"" + online5_3 + "\", \"online5_4\":\"" + online5_4 + "\", \"online5_5\":\"" + online5_5 + "\","
   + "\"rank1\":\"" + GangManager.Rank1 + "\", \"rank2\":\"" + GangManager.Rank2 + "\", \"rank3\":\"" + GangManager.Rank3 + "\", \"rank4\":\"" + GangManager.Rank4 + "\", \"rank5\":\"" + GangManager.Rank5 + "\"}");
            }
            #endregion
            */
            #endregion
        }

        public void Farm(string Event, string Data, string ExtraData)
        {
            if (Event == "closeseed")
            {
                RP.PlantSeed = 0;
                RP.PlantSeedRoom = 0;
                RP.roomUser.CanWalk = true;
            }
            else if (Event == "seed")
            {
                RP.roomUser.CanWalk = true;
                if (RP.Inventory.Equip1 != "seed")
                {
                    RP.client.SendWhisper("You must first equip a plant seed before performing this action!");
                    RP.PlantSeed = 0;
                    RP.PlantSeedRoom = 0;
                    return;
                }
                foreach (var User in RP.Room.GetRoomUserManager().GetRoomUsers())
                    if (User.GetClient().GetRolePlay().PlantSeed == RP.PlantSeed && User.GetClient().GetRolePlay().Seed > 0)
                    {
                        RP.client.SendWhisper("This dirt nest is currently being used by another user!");
                        RP.PlantSeed = 0;
                        RP.PlantSeedRoom = 0;
                        return;
                    }
                if (ExtraData == "1")
                    RP.Seed = 20237;
                else if (ExtraData == "2")
                    RP.Seed = 4552;
                else RP.Seed = 10000052;
                RP.Say("plants a special seed underneath a dirt nest", false);
                string currslot = RP.Inventory.Currslot1;
                if (RP.Inventory.IsSlotEmpty(currslot))
                {
                    Handle("equip", "", "e1");
                    RP.Inventory.Additem(currslot, true);
                }
                else
                {
                    RP.Inventory.Additem(currslot, true);
                    Handle("equip", "", "e1");
                }
            }
        }

        public void Marco(string Event, string Data, string ExtraData)
        {
            #region marco
            /*
            if (Event == "marcoswitch")
            {
                if (ExtraData == "1")
                    Marco = true;
                else Marco = false;
            }
            if (Event == "marco")
                SendWeb("{\"name\":\"marco\", \"key1_1\":\"" + marco1_1 + "\", \"key1_2\":\"" + marco1_2 + "\","
                    + "\"key2_1\":\"" + marco2_1 + "\", \"key2_2\":\"" + marco2_2 + "\", \"key3_1\":\"" + marco3_1 + "\","
                    + "\"key3_2\":\"" + marco3_2 + "\", \"key4_1\":\"" + marco4_1 + "\", \"key4_2\":\"" + marco4_2 + "\","
        + "\"key5_1\":\"" + marco5_1 + "\", \"key5_2\":\"" + marco5_2 + "\"}");
            if (Event == "marcokey")
            {
                if (String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(ExtraData))
                    return;
                if (marco5_1 != "null")
                {
                    client.SendWhisper("You can only add up to 5 marco keys!");
                    return;
                }
                if ((ExtraData == "null" || ExtraData == "Null") || (Data == "null" || Data == "Null"))
                {
                    client.SendWhisper("Invalid marco!");
                    return;
                }
                if (ExtraData.Length > 15 || Data.Length > 15)
                {
                    client.SendWhisper("This marco is too long!");
                    return;
                }
                if ((Data == marco1_1 || Data == marco2_1 ||
                    Data == marco3_1 || Data == marco4_1 ||
                    Data == marco5_1) || (ExtraData == marco1_2 ||
                    ExtraData == marco2_2 || ExtraData == marco3_2 ||
                    ExtraData == marco4_2 || ExtraData == marco5_2))
                {
                    client.SendWhisper("You can not add duplicate marco keys!");
                    return;
                }
                if (marco1_1 == "null")
                {
                    marco1_1 = Data;
                    marco1_2 = ExtraData;
                }
                else if (marco2_1 == "null")
                {
                    marco2_1 = Data;
                    marco2_2 = ExtraData;
                }
                else if (marco3_1 == "null")
                {
                    marco3_1 = Data;
                    marco3_2 = ExtraData;
                }
                else if (marco4_1 == "null")
                {
                    marco4_1 = Data;
                    marco4_2 = ExtraData;
                }
                else if (marco5_1 == "null")
                {
                    marco5_1 = Data;
                    marco5_2 = ExtraData;
                }
                SaveMarco();
                WebHandle("marco", "", "");

            }
            if (Event == "marcodelete")
            {
                if (ExtraData == "1")
                {
                    marco1_1 = "null";
                    marco1_2 = "null";
                    if (marco2_1 != "null")
                    {
                        marco1_1 = marco2_1;
                        marco1_2 = marco2_2;
                        marco2_1 = marco3_1;
                        marco2_2 = marco3_2;
                        marco3_1 = marco4_1;
                        marco3_2 = marco4_2;
                        marco4_1 = marco5_1;
                        marco4_2 = marco5_2;
                        marco5_1 = "null";
                        marco5_2 = "null";

                    }
                }
                else if (ExtraData == "2")
                {
                    marco2_1 = "null";
                    marco2_2 = "null";
                    if (marco3_1 != "null")
                    {
                        marco2_1 = marco3_1;
                        marco2_2 = marco3_2;
                        marco3_1 = marco4_1;
                        marco3_2 = marco4_2;
                        marco4_1 = marco5_1;
                        marco4_2 = marco5_2;
                        marco5_1 = "null";
                        marco5_2 = "null";

                    }
                }
                else if (ExtraData == "3")
                {
                    marco3_1 = "null";
                    marco3_2 = "null";
                    if (marco4_1 != "null")
                    {
                        marco3_1 = marco4_1;
                        marco3_2 = marco4_2;
                        marco4_1 = marco5_1;
                        marco4_2 = marco5_2;
                        marco5_1 = "null";
                        marco5_2 = "null";

                    }
                }
                else if (ExtraData == "4")
                {
                    marco4_1 = "null";
                    marco4_2 = "null";

                    if (marco5_1 != "null")
                    {
                        marco4_1 = marco5_1;
                        marco4_2 = marco5_2;
                        marco5_1 = "null";
                        marco5_2 = "null";
                    }
                }
                else if (ExtraData == "5")
                {
                    marco5_1 = "null";
                    marco5_2 = "null";
                }
                SaveMarco();
                WebHandle("marco", "", "");
            }
            */
            #endregion
        }

        public void WantedList(string Event, string Data, string ExtraData)
        {
            if (Event == "wl_clear")
            {
                if (RP.JobManager.Job == 1 && !RP.JobManager.Working)
                    RP.client.SendWhisper("You must start working before performing this action!");
                else if (RP.JobManager.Job != 1)
                    RP.client.SendWhisper("You can not perform this action!");
                else
                    RP.habbo.GetClientManager().RemoveWL(RP.WLCurPage);
            }
            else if (Event == "wl")
            {
                if (Data == "false" && ExtraData != "true")
                {
                    RP.WLopen = false;
                    return;
                }
                var Wanted = RP.habbo.GetClientManager().GetWL("", RP.WLCurPage);
                if (RP.habbo.GetClientManager().WLCount == 0 || Wanted == null)
                {
                    RP.GetClient().SendWhisper("There is currently no users on the wanted list!");
                    return;
                }
                string online = "offline";
                int job = 0;
                if (RP.JobManager.Job == 1 && RP.JobManager.Working)
                    job = 1;
                if (RP.habbo.GetClientManager().GetClientByUsername(Wanted.Name) != null)
                    online = "online";
                RP.SendWeb("{\"name\":\"wl\", \"username\":\"" + Wanted.Name + "\","
                    + "\"reason1\":\"" + Wanted.Reason1 + "\", \"rcount1\":\" x" + Wanted.ReasonCount1 + "\","
                    + "\"reason2\":\"" + Wanted.Reason2 + "\", \"rcount2\":\" x" + Wanted.ReasonCount2 + "\","
                    + "\"reason3\":\"" + Wanted.Reason3 + "\", \"rcount3\":\" x" + Wanted.ReasonCount3 + "\","
                    + "\"look\":\"" + Wanted.Look + "\", \"online\":\"" + online + "\", \"pagestart\":\"" + RP.WLCurPage + "\", \"job\":\"" + job + "\","
                    + "\"pageend\":\"" + RP.habbo.GetClientManager().WLCount + "\", \"color\":\"" + Wanted.Color + "\", \"time\":\"" + Wanted.Time + "\", \"bypass\":\"" + ExtraData + "\"}");
                RP.WLopen = true;
            }
            else if (Event == "wl_next")
            {
                RP.WLCurPage++;
                var Wanted = RP.habbo.GetClientManager().GetWL("", RP.WLCurPage);
                if (Wanted == null)
                {
                    RP.WLCurPage--;
                    return;
                }
                Handle("wl", "", "true");
            }
            else if (Event == "wl_back")
            {
                if (RP.WLCurPage > 1)
                {
                    RP.WLCurPage--;
                    Handle("wl", "", "true");
                }
            }
            else if (Event == "wl_profile")
            {
                var Wanted = RP.habbo.GetClientManager().GetWL("", RP.WLCurPage);
                Handle("statsrequest", Wanted.Name, "false");
            }
        }

        public void Timer(string Event, string Data, string ExtraData)
        {
            if (Event == "jailtimer")
            {
                RP.Jailed = Convert.ToInt32(Data);
                RP.JailedSec = Convert.ToInt32(ExtraData);
                if (RP.Jailed <= 0 && RP.JailedSec <= 0)
                {
                    RP.GetClient().SendWhisper("You are now free to leave!");
                    RP.ExCon = true;
                    if (RP.previousLook != null)
                        RP.Look(RP.previousLook);
                    if (RP.prevMotto != null)
                        RP.habbo.Motto = RP.prevMotto;
                    RP.Refresh();
                    RP.RPCache(10);
                }
            }
            else if (Event == "jobtimer")
            {
                RP.JobManager.Jobmin = Convert.ToInt32(Data);
                RP.JobManager.Jobsec = Convert.ToInt32(ExtraData);
                if (RP.JobManager.Jobmin <= 0 && RP.JobManager.Jobsec <= 0)
                {
                    int jobpay = RP.JobManager.JobPay;
                    if (RP.habbo.Rank > 1)
                        jobpay += 5;
                    RP.UpdateCredits(jobpay, true);
                    RP.GetClient().SendWhisper("You have been paid " + jobpay + " dollars from working!");
                    RP.JobManager.Shifts++;
                    RP.XPSet(5);
                    if (RP.JobManager.Job == 2 || RP.JobManager.Job == 7 || RP.JobManager.Job == 5)
                    {
                        RP.JobManager.Task3++;
                        RP.RPCache(28);
                    }
                }
            }
        }

        public void TrashWep(string Event, string Data, string ExtraData)
        {
            if (Event == "trash")
            {
                if (ExtraData == "true")
                {
                    RP.trashwep = true;
                    if (RP.Trade.Trading)
                        RP.client.SendWhisper("you can not delete items while trading!");
                    else RP.client.SendWhisper("click on the item you want to dispose");
                }
                else
                    RP.trashwep = false;
            }
        }

        public void Trade(string Event, string Data, string ExtraData)
        {
            if (Event == "trade")
            {
                var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                if (User == null)
                {
                    RP.client.SendWhisper("User not found!");
                    return;
                }
                if (ExtraData == "trade_btn1" || ExtraData == "trade_text1")
                    User.GetClient().GetRolePlay().TradeType = 1;
                else if (ExtraData == "trade_btn2" || ExtraData == "trade_text2")
                    User.GetClient().GetRolePlay().TradeType = 2;
                User.GetClient().GetRolePlay().TradeTarget = RP.habbo.Id;
                RP.TradeTimer = 20;
                User.GetClient().GetRolePlay().AcceptOffer = "trade";
                User.GetClient().GetRolePlay().SendWeb("{\"name\":\"acceptitem\", \"info\":\"<b>" + RP.habbo.Username + "</b>"
                    + " has requested to trade with you!\"}");
                RP.client.SendWhisper("Trade request sent!");
            }
            else if (Event == "accept_trade")
            {
                var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                if (User == null)
                    return;
                if (!RP.Trade.TradeReady)
                {
                    if (RP.Trade.IsTradeEmpty(User.GetClient().GetRolePlay().Trade))
                    {
                        RP.client.SendWhisper("Both trade slots are empty!");
                        return;
                    }
                    if (User.GetClient().GetRolePlay().Trade.TradeReady)
                    {
                        if (!RP.Trade.IsTradeValid(User.GetClient().GetRolePlay().Trade))
                        {
                            RP.client.SendWhisper("You do not have enough space in your inventory!");
                            return;
                        }
                        if (!User.GetClient().GetRolePlay().Trade.IsTradeValid(RP.Trade))
                        {
                            User.GetClient().SendWhisper("You do not have enough space in your inventory!");
                            User.GetClient().GetRolePlay().WebHandler.Handle("accept_trade", "", "");
                        }
                        else
                        {
                            User.GetClient().GetRolePlay().TradeTimer = 0;
                            RP.Trade.TradeTimer = 3;
                        }
                    }
                    RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"1\"}");
                    User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"2\"}");
                    RP.Trade.TradeReady = true;
                }
                else
                {
                    RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"3\"}");
                    User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"4\"}");
                    RP.Trade.TradeReady = false;
                    RP.Trade.TradeTimer = 0;
                }
            }
            else if (Event == "add_money")
            {
                if (String.IsNullOrEmpty(ExtraData))
                {
                    RP.client.SendWhisper("Please type in a digit!");
                    return;
                }
                if (!ExtraData.All(char.IsDigit))
                {
                    RP.client.SendWhisper("You can only type in numbers!");
                    return;
                }
                if (ExtraData.Length > 6)
                {
                    RP.client.SendWhisper("You can not add this amount of money!");
                    return;
                }
                if (Convert.ToInt32(ExtraData) == 0)
                {
                    RP.client.SendWhisper("Invalid number!");
                    return;
                }
                if (RP.habbo.Credits < Convert.ToInt32(ExtraData))
                {
                    RP.client.SendWhisper("You do not have enough money!");
                    return;
                }
                if (RP.Trade.TradeMoney > 0)
                    RP.UpdateCredits(RP.Trade.TradeMoney, true);
                RP.Trade.TradeMoney = Convert.ToInt32(ExtraData);
                RP.UpdateCredits(Convert.ToInt32(ExtraData), false);
                var amount = string.Format("{0:n0}", Convert.ToInt32(ExtraData));
                RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"5\", \"amount\":\"" + amount + "\"}");
                var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"6\", \"amount\":\"" + amount + "\"}");
                if (User.GetClient().GetRolePlay().Trade.TradeReady)
                {
                    User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"3\"}");
                    RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"4\"}");
                    User.GetClient().GetRolePlay().Trade.TradeReady = false;
                }
            }
            else if (Event == "cancel_trade")
            {
                if (RP.Trade.Trading && !RP.roomUser.IsTrading)
                {
                    var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                    User.GetClient().GetRolePlay().Trade.StopTrade();
                    User.GetClient().SendWhisper(RP.habbo.Username + " canceled the trade!");
                    RP.client.SendWhisper("You canceled the trade!");
                    RP.Trade.TradeTimer = 0;
                    User.GetClient().GetRolePlay().Trade.TradeTimer = 0;
                    RP.Trade.StopTrade();
                }
                RP.trashwep = false;
            }
            else if (Event == "tradeclose")
                RP.TradeTarget = 0;
            else if (Event == "unequip_trade")
            {
                ExtraData = RP.Inventory.DefineItem(ExtraData);
                if (RP.Trade.TradeReady)
                    return;
                int quantity = 0;
                if (ExtraData == "w1")
                {
                    if (RP.Trade.Slot1 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot1, false, true, 1, true, RP.Trade.HP1);
                    RP.Trade.Quantity1--;
                    quantity = RP.Trade.Quantity1;
                    if (quantity < 1)
                        RP.Trade.Slot1 = "null";
                }
                else if (ExtraData == "w2")
                {
                    if (RP.Trade.Slot2 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot2, false, true, 1, true, RP.Trade.HP2);
                    RP.Trade.Quantity2--;
                    quantity = RP.Trade.Quantity2;
                    if (quantity < 1)
                        RP.Trade.Slot2 = "null";
                }
                else if (ExtraData == "w3")
                {
                    if (RP.Trade.Slot3 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot3, false, true, 1, true, RP.Trade.HP3);
                    RP.Trade.Quantity3--;
                    quantity = RP.Trade.Quantity3;
                    if (quantity < 1)
                        RP.Trade.Slot3 = "null";
                }
                else if (ExtraData == "w4")
                {
                    if (RP.Trade.Slot4 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot4, false, true, 1, true, RP.Trade.HP4);
                    RP.Trade.Quantity4--;
                    quantity = RP.Trade.Quantity4;
                    if (quantity < 1)
                        RP.Trade.Slot4 = "null";
                }
                else if (ExtraData == "w5")
                {
                    if (RP.Trade.Slot5 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot5, false, true, 1, true, RP.Trade.HP5);
                    RP.Trade.Quantity5--;
                    quantity = RP.Trade.Quantity5;
                    if (quantity < 1)
                        RP.Trade.Slot5 = "null";
                }
                else if (ExtraData == "w6")
                {
                    if (RP.Trade.Slot6 == "null")
                        return;
                    RP.Inventory.Additem(RP.Trade.Slot6, false, true, 1, true, RP.Trade.HP6);
                    RP.Trade.Quantity6--;
                    quantity = RP.Trade.Quantity6;
                    if (quantity < 1)
                        RP.Trade.Slot6 = "null";
                }
                else return;
                string num = Regex.Replace(ExtraData, "w", "").Trim();
                RP.SendWeb("{\"name\":\"trade_unequip\", \"slot\":\"" + num + "\", \"quantity\":\"" + quantity + "\"}");
                if (ExtraData == "w1")
                    ExtraData = "w7";
                else if (ExtraData == "w2")
                    ExtraData = "w8";
                else if (ExtraData == "w3")
                    ExtraData = "w9";
                else if (ExtraData == "w4")
                    ExtraData = "w10";
                else if (ExtraData == "w5")
                    ExtraData = "w11";
                else if (ExtraData == "w6")
                    ExtraData = "w12";
                string num2 = Regex.Replace(ExtraData, "w", "").Trim();
                var User = RP.Room.GetRoomUserManager().GetRoomUserByHabbo(RP.TradeTarget);
                User.GetClient().GetRolePlay().SendWeb("{\"name\":\"trade_unequip\", \"slot\":\"" + num2 + "\", \"quantity\":\"" + quantity + "\"}");
                if (User.GetClient().GetRolePlay().Trade.TradeReady)
                {
                    User.GetClient().GetRolePlay().SendWeb("{\"name\":\"accept_trade\", \"info\":\"3\"}");
                    RP.SendWeb("{\"name\":\"accept_trade\", \"info\":\"4\"}");
                    User.GetClient().GetRolePlay().Trade.TradeReady = false;
                }
            }
        }

        public void Enable(string Event, string Data, string ExtraData)
        {
            if (Event == "enable")
            {
                if (Data == "1")
                {
                    if (ExtraData == "1")
                        RP.enable_trade = 1;
                    else if (ExtraData == "2")
                        RP.enable_sound = 1;
                    else if (ExtraData == "3")
                        RP.enable_marcos = 1;
                }
                else
                {
                    if (ExtraData == "1")
                        RP.enable_trade = 0;
                    else if (ExtraData == "2")
                        RP.enable_sound = 0;
                    else if (ExtraData == "3")
                        RP.enable_marcos = 0;
                }
                using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    DB.RunQuery("UPDATE stats SET enable_trade = '" + RP.enable_trade + "', enable_sound = '" + RP.enable_sound + "',"
                + "enable_marcos = '" + RP.enable_marcos + "' WHERE id = '" + RP.habbo.Id + "'");

            }
        }

        public void Vault(string Event, string Data, string ExtraData)
        {
            if (Event == "vault")
            {
                if (ExtraData.Length > 6 || ExtraData.Contains(" ")
                    || String.IsNullOrEmpty(ExtraData) || !ExtraData.All(char.IsLetterOrDigit)
                    || ExtraData != RP.Room.VaultCode)
                    RP.SendWeb("{\"name\":\"vault\", \"info\":\"3\"}");
                else
                {
                    RP.SendWeb("{\"name\":\"vault\", \"info\":\"2\"}");
                    RP.client.SendWhisper("The vault doors will close within 5 seconds!");
                    foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                    {
                        if (item.BaseItem == 4722 && item.ExtraData == "0")
                        {
                            item.ExtraData = "1";
                            RP.Room.VaultDoor = 12;
                            item.UpdateState();
                        }
                    }
                }
            }
        }

        public void Storage(string Event, string Data, string ExtraData)
        {
            if (Event == "storage")
            {
                if (RP.Storage.Curstorage != 0)
                    RP.Storage.StorageCache(0, true);
                string num = Regex.Replace(ExtraData, "ss-", "").Trim();
                if (RP.Storage.Curstorage == Convert.ToInt32(num))
                    return;
                bool error = false;
                int id = 0;
                if (ExtraData == "ss-1")
                {
                    if (RP.Storage.Storage1 > 0)
                        RP.Storage.Storage(RP.Storage.Storage1);
                    else error = true;
                    id = 1;
                }
                else if (ExtraData == "ss-2")
                {
                    if (RP.Storage.Storage2 > 0)
                        RP.Storage.Storage(RP.Storage.Storage2);
                    else error = true;
                    id = 2;
                }
                else if (ExtraData == "ss-3")
                {
                    if (RP.Storage.Storage3 > 0)
                        RP.Storage.Storage(RP.Storage.Storage3);
                    else error = true;
                    id = 3;
                }
                else if (ExtraData == "ss-4")
                {
                    if (RP.Storage.Storage4 > 0)
                        RP.Storage.Storage(RP.Storage.Storage4);
                    else error = true;
                    id = 4;
                }
                else if (ExtraData == "ss-5")
                {
                    if (RP.Storage.Storage5 > 0)
                        RP.Storage.Storage(RP.Storage.Storage5);
                    else error = true;
                    id = 5;
                }
                if (error)
                {
                    RP.client.SendWhisper("You need to purchase a storage from a bank worker!");
                    RP.Storage.Curstorage = 0;
                }
                else RP.Storage.UpdateStorage(id, true);
            }
            else if (Event == "close_storage")
            {
                if (RP.Storage.Curstorage != 0)
                    RP.Storage.StorageCache(0, true);
                RP.Storage.Curstorage = 0;
            }
            else if (Event == "unequip_storage")
            {
                if (RP.Trade.Trading)
                {
                    RP.client.SendWhisper("This action can not be performed while trading!");
                    return;
                }
                ExtraData = RP.Inventory.DefineItem(ExtraData);
                if (RP.trashwep)
                {
                    RP.Storage.UpdateQuantity(ExtraData, RP.Storage.GetQuantity(ExtraData) - 1);
                    if (RP.Storage.GetQuantity(ExtraData) <= 0)
                    {
                        RP.Storage.UpdateItem(ExtraData, "null");
                        RP.Storage.UpdateHP(ExtraData, 0);
                    }
                    RP.Storage.UpdateStorage(RP.Storage.Curstorage);
                    return;
                }
                if (RP.Inventory.Currslot1 != "null" || RP.Inventory.Currslot2 != "null")
                {
                    RP.client.SendWhisper("You must unequip your items before performing this action!");
                    return;
                }
                string wep = "";
                if (ExtraData == "w1")
                    wep = RP.Storage.Slot1;
                else if (ExtraData == "w2")
                    wep = RP.Storage.Slot2;
                else if (ExtraData == "w3")
                    wep = RP.Storage.Slot3;
                else if (ExtraData == "w4")
                    wep = RP.Storage.Slot4;
                else if (ExtraData == "w5")
                    wep = RP.Storage.Slot5;
                else if (ExtraData == "w6")
                    wep = RP.Storage.Slot6;
                else if (ExtraData == "w7")
                    wep = RP.Storage.Slot7;
                else if (ExtraData == "w8")
                    wep = RP.Storage.Slot8;
                else if (ExtraData == "w9")
                    wep = RP.Storage.Slot9;
                else if (ExtraData == "w10")
                    wep = RP.Storage.Slot10;
                else if (ExtraData == "w11")
                    wep = RP.Storage.Slot11;
                else if (ExtraData == "w12")
                    wep = RP.Storage.Slot12;
                if (!RP.Inventory.IsInventoryFull(wep))
                    RP.Storage.AddStorage(ExtraData, 0, wep, true);
                else RP.client.SendWhisper("Your inventory is currently full!");
            }
        }

        public void SelectStun(string Event, string Data, string ExtraData)
        {
            var value = Regex.Match(ExtraData, @"(.{2})\s*$");
            string stun = "stun";
            if (Convert.ToString(value).Contains("10"))
                stun = stun + "10";
            else stun = stun + Regex.Match(Convert.ToString(value), @"(.{1})\s*$");
            RP.Say("obtains a new stun-gun", true);
            RP.stungun = stun;
            RP.RPCache(31);
            RP.Startwork();
            //RP.client.SendWhisper(Convert.ToString(stun));
        }
    }
}
