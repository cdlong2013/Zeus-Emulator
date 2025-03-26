using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Plus.RolePlay.Inventory
{
    public class Equipment
    {
        #region Inventory Values
        public string Currslot1 = "null";
        public string Currslot2 = "null";
        public string Equip1 = "null";
        public string Equip2 = "null";
        public string Item1;
        public string Item2;
        public string Item3;
        public string Item4;
        public string Item5;
        public string Item6;
        public string Item7;
        public string Item8;
        public string Item9;
        public string Item10;
        public string Item11;
        public string Item12;
        public int Quantity1;
        public int Quantity2;
        public int Quantity3;
        public int Quantity4;
        public int Quantity5;
        public int Quantity6;
        public int Quantity7;
        public int Quantity8;
        public int Quantity9;
        public int Quantity10;
        public int Quantity11;
        public int Quantity12;
        public int HP1;
        public int HP2;
        public int HP3;
        public int HP4;
        public int HP5;
        public int HP6;
        public int HP7;
        public int HP8;
        public int HP9;
        public int HP10;
        public int HP11;
        public int HP12;

        public string showitem1 = "null";
        public string showitem2 = "null";
        #endregion

        RPData RP;

        public Equipment(RPData RP)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.RP = RP;
                DB.SetQuery("SELECT * FROM `inventory` WHERE id = '" + RP.habbo.Id + "' LIMIT 1");
                foreach (DataRow dataRow in DB.GetTable().Rows)
                {
                    this.Item1 = (string)dataRow["wslot1"];
                    this.Item2 = (string)dataRow["wslot2"];
                    this.Item3 = (string)dataRow["wslot3"];
                    this.Item4 = (string)dataRow["wslot4"];
                    this.Item5 = (string)dataRow["wslot5"];
                    this.Item6 = (string)dataRow["wslot6"];
                    this.Item7 = (string)dataRow["wslot7"];
                    this.Item8 = (string)dataRow["wslot8"];
                    this.Item9 = (string)dataRow["wslot9"];
                    this.Item10 = (string)dataRow["wslot10"];
                    this.Item11 = (string)dataRow["wslot11"];
                    this.Item12 = (string)dataRow["wslot12"];
                    this.HP1 = (int)dataRow["whp1"];
                    this.HP2 = (int)dataRow["whp2"];
                    this.HP3 = (int)dataRow["whp3"];
                    this.HP4 = (int)dataRow["whp4"];
                    this.HP5 = (int)dataRow["whp5"];
                    this.HP6 = (int)dataRow["whp6"];
                    this.HP7 = (int)dataRow["whp7"];
                    this.HP8 = (int)dataRow["whp8"];
                    this.HP9 = (int)dataRow["whp9"];
                    this.HP10 = (int)dataRow["whp10"];
                    this.HP11 = (int)dataRow["whp11"];
                    this.HP12 = (int)dataRow["whp12"];
                    this.Quantity1 = (int)dataRow["quantity1"];
                    this.Quantity2 = (int)dataRow["quantity2"];
                    this.Quantity3 = (int)dataRow["quantity3"];
                    this.Quantity4 = (int)dataRow["quantity4"];
                    this.Quantity5 = (int)dataRow["quantity5"];
                    this.Quantity6 = (int)dataRow["quantity6"];
                    this.Quantity7 = (int)dataRow["quantity7"];
                    this.Quantity8 = (int)dataRow["quantity8"];
                    this.Quantity9 = (int)dataRow["quantity9"];
                    this.Quantity10 = (int)dataRow["quantity10"];
                    this.Quantity11 = (int)dataRow["quantity11"];
                    this.Quantity12 = (int)dataRow["quantity12"];
                    Validation();
                }
            }
        }

        public void Validation()
        {
            if (String.IsNullOrEmpty(Item1))
            {
                Item1 = "null";
                ItemCache(1);
            }
            if (String.IsNullOrEmpty(Item2))
            {
                Item2 = "null";
                ItemCache(2);
            }
            if (String.IsNullOrEmpty(Item3))
            {
                Item3 = "null";
                ItemCache(3);
            }
            if (String.IsNullOrEmpty(Item4))
            {
                Item4 = "null";
                ItemCache(4);
            }
            if (String.IsNullOrEmpty(Item5))
            {
                Item5 = "null";
                ItemCache(5);
            }
            if (String.IsNullOrEmpty(Item6))
            {
                Item6 = "null";
                ItemCache(6);
            }
            if (String.IsNullOrEmpty(Item7))
            {
                Item7 = "null";
                ItemCache(7);
            }
            if (String.IsNullOrEmpty(Item8))
            {
                Item8 = "null";
                ItemCache(8);
            }
            if (String.IsNullOrEmpty(Item9))
            {
                Item9 = "null";
                ItemCache(9);
            }
            if (String.IsNullOrEmpty(Item10))
            {
                Item10 = "null";
                ItemCache(10);
            }
            if (String.IsNullOrEmpty(Item11))
            {
                Item11 = "null";
                ItemCache(1);
            }
            if (String.IsNullOrEmpty(Item12))
            {
                Item12 = "null";
                ItemCache(12);
            }
                
        }

        public bool IsInventoryFull(string Item)
        {
            if (Item12 != "null")
            {
                if (Item == Item1 && CheckItem(Item) && Quantity1 < 50)
                    return false;
                else if (Item == Item2 && CheckItem(Item) && Quantity2 < 50)
                    return false;
                else if (Item == Item3 && CheckItem(Item) && Quantity3 < 50)
                    return false;
                else if (Item == Item4 && CheckItem(Item) && Quantity4 < 50)
                    return false;
                else if (Item == Item5 && CheckItem(Item) && Quantity5 < 50)
                    return false;
                else if (Item == Item6 && CheckItem(Item) && Quantity6 < 50)
                    return false;
                else if (Item == Item7 && CheckItem(Item) && Quantity7 < 50)
                    return false;
                else if (Item == Item8 && CheckItem(Item) && Quantity8 < 50)
                    return false;
                else if (Item == Item9 && CheckItem(Item) && Quantity9 < 50)
                    return false;
                else if (Item == Item10 && CheckItem(Item) && Quantity10 < 50)
                    return false;
                else if (Item == Item11 && CheckItem(Item) && Quantity11 < 50)
                    return false;
                else if (Item == Item12 && CheckItem(Item) && Quantity12 < 50)
                    return false;
                else if (Item1 == "null")
                    return false;
                else if (Item2 == "null")
                    return false;
                else if (Item3 == "null")
                    return false;
                else if (Item4 == "null")
                    return false;
                else if (Item5 == "null")
                    return false;
                else if (Item6 == "null")
                    return false;
                else if (Item7 == "null")
                    return false;
                else if (Item8 == "null")
                    return false;
                else if (Item9 == "null")
                    return false;
                else if (Item10 == "null")
                    return false;
                else if (Item11 == "null")
                    return false;
                else if (Item12 == "null")
                    return false;
                return true;
            }
            return false;
        }

        public bool CheckItem(string item)
        {
            if (item != "bat" && !item.Contains("stun") &&
                item != "axe" && item != "sword" &&
                !item.Contains("kevlar") && item != "battle_axe" &&
                item != "chain_stick" && item != "crowbar" &&
                item != "fishing_rod" && item != "iron_bat" &&
                item != "lightsaber" && item != "long_sword" &&
                item != "metal_pipe" && item != "power_axe" &&
                item != "spike_ball" && item != "gold_bat" && item != "gold_battleaxe" &&
                item != "gold_chainstick" && item != "gold_crowbar" &&
                item != "gold_lightsaber" && item != "gold_pipe" &&
                item != "gold_poweraxe" && item != "gold_spikeball" && 
                item != "gold_longsword" && item != "skateboard" && item != "knife")
                return true;
            else return false;
        }

        public int ItemHP(string item)
        {
            if (item == "w1")
            {
                if (Item1 == "kevlar2")
                    return 150;
                else if (Item1 == "kevlar3")
                    return 200;
                else if (Item1 == "kevlar4")
                    return 350;
            }
            else if (item == "w2")
            {
                if (Item2 == "kevlar2")
                    return 150;
                else if (Item2 == "kevlar3")
                    return 200;
                else if (Item2 == "kevlar4")
                    return 350;
            }
            else if (item == "w3")
            {
                if (Item3 == "kevlar2")
                    return 150;
                else if (Item3 == "kevlar3")
                    return 200;
                else if (Item3 == "kevlar4")
                    return 350;
            }
            else if (item == "w4")
            {
                if (Item4 == "kevlar2")
                    return 150;
                else if (Item4 == "kevlar3")
                    return 200;
                else if (Item4 == "kevlar4")
                    return 350;
            }
            else if (item == "w5")
            {
                if (Item5 == "kevlar2")
                    return 150;
                else if (Item5 == "kevlar3")
                    return 200;
                else if (Item5 == "kevlar4")
                    return 350;
            }
            else if (item == "w6")
            {
                if (Item6 == "kevlar2")
                    return 150;
                else if (Item6 == "kevlar3")
                    return 200;
                else if (Item6 == "kevlar4")
                    return 350;
            }
            else if (item == "w7")
            {
                if (Item7 == "kevlar2")
                    return 150;
                else if (Item7 == "kevlar3")
                    return 200;
                else if (Item7 == "kevlar4")
                    return 350;
            }
            else if (item == "w8")
            {
                if (Item8 == "kevlar2")
                    return 150;
                else if (Item8 == "kevlar3")
                    return 200;
                else if (Item8 == "kevlar4")
                    return 350;
            }
            else if (item == "w9")
            {
                if (Item9 == "kevlar2")
                    return 150;
                else if (Item9 == "kevlar3")
                    return 200;
                else if (Item9 == "kevlar4")
                    return 350;
            }
            else if (item == "w10")
            {
                if (Item10 == "kevlar2")
                    return 150;
                else if (Item10 == "kevlar3")
                    return 200;
                else if (Item10 == "kevlar4")
                    return 350;
            }
            else if (item == "w11")
            {
                if (Item11 == "kevlar2")
                    return 150;
                else if (Item11 == "kevlar3")
                    return 200;
                else if (Item11 == "kevlar4")
                    return 350;
            }
            else if (item == "w12")
            {
                if (Item12 == "kevlar2")
                    return 150;
                else if (Item12 == "kevlar3")
                    return 200;
                else if (Item12 == "kevlar4")
                    return 350;
            }
            return 100;
        }

        public bool IsSlotEmpty(string item)
        {
            if (GetQuantity(item) <= 1)
                return true;
            else return false;
        }

        public void ItemHealth(string equip, int dmg)
        {
            int hp = 0;
            int id = 0;
            int itemcircle = 0;
            string slot = "";
            string wep = "";
            
            if (equip == "false")
            {
                wep = Currslot2;
                UpdateHP(Currslot2, GetHP(Currslot2) - dmg);
                hp = GetHP(Currslot2);
                id = GetItemID(Currslot2);
                if (hp < 1)
                {
                    if (Equip2.Contains("kevlar"))
                        RP.Say("armor shatters", false);
                    slot = Currslot2;
                    RP.WebHandler.Handle("equip", "false", "e2");
                    Additem(slot, true, false);
                }
            }
            else
            {
                wep = Currslot1;
                UpdateHP(Currslot1, GetHP(Currslot1) - dmg);
                hp = GetHP(Currslot1);
                id = GetItemID(Currslot1);
                if (hp < 1)
                {
                    RP.client.SendWhisper("your item has broken!");
                    slot = Currslot1;
                    RP.WebHandler.Handle("equip", "false", "e1");
                    Additem(slot, true, false);
                }
            }
            ItemCache(id);
            if (showitem1 == Equip1 && equip == "true")
                itemcircle = 1;
            else if (showitem1 == Equip2 && equip == "false")
                itemcircle = 1;
            else if (showitem2 == Equip1 && equip == "true")
                itemcircle = 2;
            else if (showitem2 == Equip2 && equip == "false")
                itemcircle = 2;
            RP.SendWeb("{\"name\":\"itemhp\", \"hp\":\"" + (ItemHP(wep) - hp) + "\", \"hp2\":\"" + hp + "\", \"maxhp\":\"" + (ItemHP(wep)) + "\", \"slot\":\"" + equip + "\", \"circle\":\"" + itemcircle + "\"}");
        }

        public void ItemCache(int CacheThis, bool save = false)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if ((CacheThis == 1 || save) && !Item1.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot1 = '" + Item1 + "', whp1 = '" + HP1 + "', quantity1 = '" + Quantity1 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 2 || save) && !Item2.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot2 = '" + Item2 + "', whp2 = '" + HP2 + "', quantity2 = '" + Quantity2 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 3 || save) && !Item3.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot3 = '" + Item3 + "', whp3 = '" + HP3 + "', quantity3 = '" + Quantity3 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 4 || save) && !Item4.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot4 = '" + Item4 + "', whp4 = '" + HP4 + "', quantity4 = '" + Quantity4 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 5 || save) && !Item5.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot5 = '" + Item5 + "', whp5 = '" + HP5 + "', quantity5 = '" + Quantity5 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 6 || save) && !Item6.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot6 = '" + Item6 + "', whp6 = '" + HP6 + "', quantity6 = '" + Quantity6 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 7 || save) && !Item7.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot7 = '" + Item7 + "', whp7 = '" + HP7 + "', quantity7 = '" + Quantity7 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 8 || save) && !Item8.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot8 = '" + Item8 + "', whp8 = '" + HP8 + "', quantity8 = '" + Quantity8 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 9 || save) && !Item9.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot9 = '" + Item9 + "', whp9 = '" + HP9 + "', quantity9 = '" + Quantity9 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 10 || save) && !Item10.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot10 = '" + Item10 + "', whp10 = '" + HP10 + "', quantity10 = '" + Quantity10 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 11 || save) && !Item11.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot11 = '" + Item11 + "', whp11 = '" + HP11 + "', quantity11 = '" + Quantity11 + "' WHERE id = '" + RP.habbo.Id + "'");
                if ((CacheThis == 12 || save) && !Item12.Contains("stun"))
                    DB.RunQuery("UPDATE inventory SET wslot12 = '" + Item12 + "', whp12 = '" + HP12 + "', quantity12 = '" + Quantity12 + "' WHERE id = '" + RP.habbo.Id + "'");
            }
        }

        public void ItemEffect(string Item, bool bypass = false)
        {          
            if (Item.Contains("stun"))
                RP.roomUser.ApplyEffect(592);
            else if (Item == "knife")
                RP.roomUser.ApplyEffect(672);
            else if (Item == "fish")
                RP.roomUser.CarryItem(34);
            else if (Item == "pear")
                RP.roomUser.CarryItem(36);
            else if (Item == "tomato")
                RP.roomUser.CarryItem(98);
            else if (Item == "orange")
                RP.roomUser.CarryItem(38);
            else if (Item == "pineapple")
                RP.roomUser.CarryItem(39);
            else if (Item == "yellow_pepper")
                RP.roomUser.CarryItem(45);
            else if (Item == "green_pepper")
                RP.roomUser.CarryItem(46);
            else if (Item == "red_pepper")
                RP.roomUser.CarryItem(47);
            else if (Item == "chicken")
                RP.roomUser.CarryItem(70);
            else if (Item == "bread")
                RP.roomUser.CarryItem(71);
            else if (Item == "apple")
                RP.roomUser.CarryItem(83);
            else if (Item == "cake")
                RP.roomUser.CarryItem(96);
            else if (Item == "bat")
                RP.roomUser.ApplyEffect(591);
            else if (Item == "sword")
                RP.roomUser.ApplyEffect(162);
            else if (Item == "axe")
                RP.roomUser.ApplyEffect(117);
            else if (Item == "snack")
                RP.roomUser.CarryItem(52);
            else if (Item == "medic")
                RP.roomUser.CarryItem(1013);
            else if (Item == "carrot")
                RP.roomUser.CarryItem(3);
            else if (Item == "flower")
                RP.roomUser.CarryItem(1008);
            else if (Item == "seed" || Item == "weed")
                RP.roomUser.CarryItem(22);
            else if (Item == "lightsaber")
                RP.roomUser.ApplyEffect(196);
            else if (Item == "skateboard")
            {
                RP.roomUser.ApplyEffect(72);
                RP.Skateboard = true;
            }
            else if (Item == "fishing_rod")
            { }
            else if (Item.Contains("kevlar"))
            {
                if (!RP.habbo.Look.Contains("cc-3420"))
                {
                    string look = "";
                    if (Item == "kevlar")
                        look = RP.habbo.Look + ".cc-3420-82";
                    else if (Item == "kevlar2")
                        look = RP.habbo.Look + ".cc-3420-63";
                    else if (Item == "kevlar3")
                        look = RP.habbo.Look + ".cc-3420-64";
                    else look = RP.habbo.Look + ".cc-3420-66";
                    RP.Look(look);
                    RP.Refresh();
                }
            }
            else RP.roomUser.CarryItem(22);
            if (RP.ItemCD > 0)
                return;
            if (!bypass)
            {
                if (Item == "skateboard")
                {
                    if (RP.Cooldown > 0)
                    {
                        RP.Responds();
                        return;
                    }
                    RP.Say("hops on their skateboard", false);
                }
                else if (Item == "apple")
                    RP.Say("takes out a fresh apple from their inventory", false);
                else if (Item == "apple")
                    RP.Say("takes out a slice of cheese from their inventory", false);
                else if (Item == "banana")
                    RP.Say("takes out a fresh banana from their inventory", false);
                else if (Item == "beatroot")
                    RP.Say("takes out a beatroot from their inventory", false);
                else if (Item == "blackberry")
                    RP.Say("takes out a black berry from their inventory", false);
                else if (Item == "cake")
                    RP.Say("takes out a slice of cake from their inventory", false);
                else if (Item == "chicken")
                    RP.Say("takes out a cooked chicken from their inventory", false);
                else if (Item == "cookbook")
                    RP.Say("takes out a cooking book from their inventory", false);
                else if (Item == "egg")
                    RP.Say("takes out an egg from their inventory", false);
                else if (Item == "fish")
                    RP.Say("takes out a raw fish from their inventory", false);
                else if (Item == "grapes")
                    RP.Say("takes out a sum of white grapes from their inventory", false);
                else if (Item == "lemon")
                    RP.Say("takes out a fresh lemon from their inventory", false);
                else if (Item == "mushroom")
                    RP.Say("takes out a fresh mushroom from their inventory", false);
                else if (Item == "orange")
                    RP.Say("takes out an orange from their inventory", false);
                else if (Item == "pear")
                    RP.Say("takes out a pear fruit from their inventory", false);
                else if (Item == "pineapple")
                    RP.Say("takes out a pineapple from their inventory", false);
                else if (Item == "purple_grapes")
                    RP.Say("takes out a sum of purple grapes from their inventory", false);
                else if (Item == "raw_chicken")
                    RP.Say("takes out a raw chicken from their inventory", false);
                else if (Item == "raw_stake")
                    RP.Say("takes out a raw stake from their inventory", false);
                else if (Item == "stake")
                    RP.Say("takes out a cooked stake from their inventory", false);
                else if (Item == "strawberry")
                    RP.Say("takes out a strawberry from their inventory", false);
                else if (Item == "tomato")
                    RP.Say("takes out a fresh tomato from their inventory", false);
                else if (Item == "watermelon")
                    RP.Say("takes out a slice of watermelon from their inventory", false);
                else if (Item == "weed")
                    RP.Say("takes out a stash of weed from their inventory", false);
                else if (Item == "medic")
                    RP.Say("takes out a medkit from their inventory", false);
                else if (Item == "snack")
                    RP.Say("takes out a bag of chips from their inventory", false);
                else if (Item == "akorn")
                    RP.Say("takes out an akorn from their inventory", false);
                else if (Item == "bread")
                    RP.Say("takes out a loaf of bread from their inventory", false);
                else if (Item == "cooked_egg")
                    RP.Say("takes out a cooked egg from their inventory", false);
                else if (Item == "cooked_fish")
                    RP.Say("takes out a cooked fish from their inventory", false);
                else if (Item == "cookie")
                    RP.Say("takes out a cookie from their inventory", false);
                else if (Item == "green_pepper")
                    RP.Say("takes out a green pepper from their inventory", false);
                else if (Item == "yellow_pepper")
                    RP.Say("takes out a yellow pepper from their inventory", false);
                else if (Item == "red_pepper")
                    RP.Say("takes out a red pepper from their inventory", false);
                else if (Item == "meat")
                    RP.Say("takes out a piece of meat from their inventory", false);
                else if (Item == "raw_meat")
                    RP.Say("takes out a piece of raw meat from their inventory", false);
                else if (Item == "green_pepper")
                    RP.Say("takes out a green pepper from their inventory", false);
                else if (Item == "seed")
                    RP.Say("takes out a plant " + Item + " from their inventory", false);
                else if (Item == "carrot" || Item == "flower")
                    RP.Say("takes out a " + Item + " from their inventory", false);
                else if (Item == "chocolate")
                    RP.Say("takes out a " + Item + " bar from their inventory", false);
                else if (Item == "cheese")
                    RP.Say("takes out a slice of " + Item + " from their inventory", false);
                else if (Item.Contains("kevlar"))
                    RP.Say("puts on their kevlar vest", false);
                else if (Item.Contains("stun"))
                    RP.Say("pulls out their stun-gun", false);
                else if (Item == "battle_axe")
                    RP.Say("pulls out their battle axe", false);
                else if (Item == "chain_stick")
                    RP.Say("pulls out their chain stick", false);
                else if (Item == "fishing_rod")
                    RP.Say("pulls out their fishing rod", false);
                else if (Item == "iron_bat")
                    RP.Say("pulls out their iron bat", false);
                else if (Item == "gold_bat")
                    RP.Say("pulls out their golden bat", false);
                else if (Item == "gold_battleaxe")
                    RP.Say("pulls out their golden battle axe", false);
                else if (Item == "gold_chainstick")
                    RP.Say("pulls out their golden chain stick", false);
                else if (Item == "gold_crowbar")
                    RP.Say("pulls out their golden crowbar", false);
                else if (Item == "gold_lightsaber")
                    RP.Say("pulls out their golden lightsaber", false);
                else if (Item == "gold_longsword")
                    RP.Say("pulls out their golden sword", false);
                else if (Item == "gold_pipe")
                    RP.Say("pulls out their golden pipe", false);
                else if (Item == "gold_poweraxe")
                    RP.Say("pulls out their golden power axe", false);
                else if (Item == "gold_spikeball")
                    RP.Say("pulls out their golden spike ball", false);
                else if (Item == "lightsaber")
                    RP.Say("pulls out their light saber", false);
                else if (Item == "long_sword")
                    RP.Say("pulls out their long sword", false);
                else if (Item == "metal_pipe")
                    RP.Say("pulls out their metal pipe", false);
                else if (Item == "power_axe")
                    RP.Say("pulls out their power axe", false);
                else if (Item == "spike_ball")
                    RP.Say("pulls out their spike ball", false);
                else RP.Say("pulls out their " + Item + "", false);
                RP.ItemCD = 5;
            }
        }
     
        public void Additem(string Item, bool remove = false, bool web = true, int Quantity = 1, bool bypass = false, int hp = 100)
        {
            int id = 0;
            if (!remove)
            {
                if (Item == Item1 && CheckItem(Item) && Quantity1 < 50)
                {
                    Quantity1 += Quantity;
                    id = 1;
                    if (Quantity1 > 50)
                        Quantity1 = 50;
                }
                else if (Item == Item2 && CheckItem(Item) && Quantity2 < 50)
                {
                    Quantity2 += Quantity;
                    id = 2;
                    if (Quantity2 > 50)
                        Quantity2 = 50;
                }
                else if (Item == Item3 && CheckItem(Item) && Quantity3 < 50)
                {
                    Quantity3 += Quantity;
                    id = 3;
                    if (Quantity3 > 50)
                        Quantity3 = 50;
                }
                else if (Item == Item4 && CheckItem(Item) && Quantity4 < 50)
                {
                    Quantity4 += Quantity;
                    id = 4;
                    if (Quantity4 > 50)
                        Quantity4 = 50;
                }
                else if (Item == Item5 && CheckItem(Item) && Quantity5 < 50)
                {
                    Quantity5 += Quantity;
                    id = 5;
                    if (Quantity5 > 50)
                        Quantity5 = 50;
                }
                else if (Item == Item6 && CheckItem(Item) && Quantity6 < 50)
                {
                    Quantity6 += Quantity;
                    id = 6;
                    if (Quantity6 > 50)
                        Quantity6 = 50;
                }
                else if (Item == Item7 && CheckItem(Item) && Quantity7 < 50)
                {
                    Quantity7 += Quantity;
                    id = 7;
                    if (Quantity7 > 50)
                        Quantity7 = 50;
                }
                else if (Item == Item8 && CheckItem(Item) && Quantity8 < 50)
                {
                    Quantity8 += Quantity;
                    id = 8;
                    if (Quantity8 > 50)
                        Quantity8 = 50;
                }
                else if (Item == Item9 && CheckItem(Item) && Quantity9 < 50)
                {
                    Quantity9 += Quantity;
                    id = 9;
                    if (Quantity9 > 50)
                        Quantity9 = 50;
                }
                else if (Item == Item10 && CheckItem(Item) && Quantity10 < 50)
                {
                    Quantity10 += Quantity;
                    id = 10;
                    if (Quantity10 > 50)
                        Quantity10 = 50;
                }
                else if (Item == Item11 && CheckItem(Item) && Quantity11 < 50)
                {
                    Quantity11 += Quantity;
                    id = 11;
                    if (Quantity11 > 50)
                        Quantity11 = 50;
                }
                else if (Item == Item12 && CheckItem(Item) && Quantity12 < 50)
                {
                    Quantity12 += Quantity;
                    id = 12;
                    if (Quantity12 > 50)
                        Quantity12 = 50;
                }
                else if (Item1 == "null")
                {
                    Item1 = Item;
                    HP1 = hp;
                    id = 1;
                    Quantity1 += Quantity;
                }
                else if (Item2 == "null")
                {
                    Item2 = Item;
                    HP2 = hp;
                    id = 2;
                    Quantity2 += Quantity;
                }
                else if (Item3 == "null")
                {
                    Item3 = Item;
                    HP3 = hp;
                    id = 3;
                    Quantity3 += Quantity;
                }
                else if (Item4 == "null")
                {
                    Item4 = Item;
                    HP4 = hp;
                    id = 4;
                    Quantity4 += Quantity;
                }
                else if (Item5 == "null")
                {
                    Item5 = Item;
                    HP5 = hp;
                    id = 5;
                    Quantity5 += Quantity;
                }
                else if (Item6 == "null")
                {
                    Item6 = Item;
                    HP6 = hp;
                    id = 6;
                    Quantity6 += Quantity;
                }
                else if (Item7 == "null")
                {
                    Item7 = Item;
                    HP7 = hp;
                    id = 7;
                    Quantity7 += Quantity;
                }
                else if (Item8 == "null")
                {
                    Item8 = Item;
                    HP8 = hp;
                    id = 8;
                    Quantity8 += Quantity;
                }
                else if (Item9 == "null")
                {
                    Item9 = Item;
                    HP9 = hp;
                    id = 9;
                    Quantity9 += Quantity;
                }
                else if (Item10 == "null")
                {
                    Item10 = Item;
                    HP10 = hp;
                    id = 10;
                    Quantity10 += Quantity;
                }
                else if (Item11 == "null")
                {
                    Item11 = Item;
                    HP11 = hp;
                    id = 11;
                    Quantity11 += Quantity;
                }
                else if (Item12 == "null")
                {
                    Item12 = Item;
                    HP12 = hp;
                    id = 12;
                    Quantity12 += Quantity;
                }
                else
                {
                    RP.client.SendWhisper("Your inventory is currently full!");
                    return;
                }
            }
            else
            {
                if (CheckItem(GetItem(Item)) && GetQuantity(Item) > 1)
                {
                    UpdateQuantity(Item, GetQuantity(Item) - 1);
                    id = GetItemID(Item); 
                }
                else if (!Item.Contains("stun"))
                {
                    id = GetItemID(Item);
                    UpdateItem(Item, "null");
                    UpdateQuantity(Item, 0);
                }
                else if (Item.Contains("stun"))
                {
                    if (Item1 == Item)
                        Item1 = "null";
                    else if (Item2 == Item)
                        Item2 = "null";
                    else if (Item3 == Item)
                        Item3 = "null";
                    else if (Item4 == Item)
                        Item4 = "null";
                    else if (Item5 == Item)
                        Item5 = "null";
                    else if (Item6 == Item)
                        Item6 = "null";
                    else if (Item7 == Item)
                        Item7 = "null";
                    else if (Item8 == Item)
                        Item8 = "null";
                    else if (Item9 == Item)
                        Item9 = "null";
                    else if (Item10 == Item)
                        Item10 = "null";
                    else if (Item11 == Item)
                        Item11 = "null";
                    else if (Item12 == Item)
                        Item12 = "null";
                }
            }
            if (web)
                RP.LoadStats(true);
            if (!bypass && !Item.Contains("stun"))
                ItemCache(id);
        }

        public string DefineItem(string item)
        {
            if (String.IsNullOrEmpty(item))
                return "";
            string i = item;
            if (item.Contains("10") || item.Contains("11") || item.Contains("12"))
                item = Convert.ToString(Regex.Match(item, @"(.{2})\s*$"));
            else item = Convert.ToString(Regex.Match(item, @"(.{1})\s*$"));
            if (i.Contains("eslot"))
                item = "e" + item;
            else item = "w" + item;
            return item;
        }

        public string GetItem(string item)
        {
            if (item == "w1")
                return Item1;
            else if (item == "w2")
                return Item2;
            else if (item == "w3")
                return Item3;
            else if (item == "w4")
                return Item4;
            else if (item == "w5")
                return Item5;
            else if (item == "w6")
                return Item6;
            else if (item == "w7")
                return Item7;
            else if (item == "w8")
                return Item8;
            else if (item == "w9")
                return Item9;
            else if (item == "w10")
                return Item10;
            else if (item == "w11")
                return Item11;
            else if (item == "w12")
                return Item12;
            else return "null";
        }

        public int GetHP(string item)
        {
            if (item == "w1")
                return HP1;
            else if (item == "w2")
                return HP2;
            else if (item == "w3")
                return HP3;
            else if (item == "w4")
                return HP4;
            else if (item == "w5")
                return HP5;
            else if (item == "w6")
                return HP6;
            else if (item == "w7")
                return HP7;
            else if (item == "w8")
                return HP8;
            else if (item == "w9")
                return HP9;
            else if (item == "w10")
                return HP10;
            else if (item == "w11")
                return HP11;
            else if (item == "w12")
                return HP12;
            else return 0;
        }

        public int GetQuantity(string item)
        {
            if (item == "w1")
                return Quantity1;
            else if (item == "w2")
                return Quantity2;
            else if (item == "w3")
                return Quantity3;
            else if (item == "w4")
                return Quantity4;
            else if (item == "w5")
                return Quantity5;
            else if (item == "w6")
                return Quantity6;
            else if (item == "w7")
                return Quantity7;
            else if (item == "w8")
                return Quantity8;
            else if (item == "w9")
                return Quantity9;
            else if (item == "w10")
                return Quantity10;
            else if (item == "w11")
                return Quantity11;
            else if (item == "w12")
                return Quantity12;
            else return 0;
        }

        public int GetItemID(string item)
        {
            if (item == "w1")
                return 1;
            else if (item == "w2")
                return 2;
            else if (item == "w3")
                return 3;
            else if (item == "w4")
                return 4;
            else if (item == "w5")
                return 5;
            else if (item == "w6")
                return 6;
            else if (item == "w7")
                return 7;
            else if (item == "w8")
                return 8;
            else if (item == "w9")
                return 9;
            else if (item == "w10")
                return 10;
            else if (item == "w11")
                return 11;
            else if (item == "w12")
                return 12;
            else return 0;
        }

        public void UpdateItem(string id, string data)
        {
            if (id == "w1")
                Item1 = data;
            else if (id == "w2")
                Item2 = data;
            else if (id == "w3")
                Item3 = data;
            else if (id == "w4")
                Item4 = data;
            else if (id == "w5")
                Item5 = data;
            else if (id == "w6")
                Item6 = data;
            else if (id == "w7")
                Item7 = data;
            else if (id == "w8")
                Item8 = data;
            else if (id == "w9")
                Item9 = data;
            else if (id == "w10")
                Item10 = data;
            else if (id == "w11")
                Item11 = data;
            else if (id == "w12")
                Item12 = data; 
        }

        public void UpdateHP(string id, int data)
        {
            if (id == "w1")
                HP1 = data;
            else if (id == "w2")
                HP2 = data;
            else if (id == "w3")
                HP3 = data;
            else if (id == "w4")
                HP4 = data;
            else if (id == "w5")
                HP5 = data;
            else if (id == "w6")
                HP6 = data;
            else if (id == "w7")
                HP7 = data;
            else if (id == "w8")
                HP8 = data;
            else if (id == "w9")
                HP9 = data;
            else if (id == "w10")
                HP10 = data;
            else if (id == "w11")
                HP11 = data;
            else if (id == "w12")
                HP12 = data;
        }

        public void UpdateQuantity(string id, int data)
        {
            if (id == "w1")
                Quantity1 = data;
            else if (id == "w2")
                Quantity2 = data;
            else if (id == "w3")
                Quantity3 = data;
            else if (id == "w4")
                Quantity4 = data;
            else if (id == "w5")
                Quantity5 = data;
            else if (id == "w6")
                Quantity6 = data;
            else if (id == "w7")
                Quantity7 = data;
            else if (id == "w8")
                Quantity8 = data;
            else if (id == "w9")
                Quantity9 = data;
            else if (id == "w10")
                Quantity10 = data;
            else if (id == "w11")
                Quantity11 = data;
            else if (id == "w12")
                Quantity12 = data;
        }

    }
}
