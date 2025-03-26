using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.RolePlay.Storage
{
    public class ItemStorage
    {       
        #region Storage Values
        public int Curstorage;
        public int Storage1;
        public int Storage2;
        public int Storage3;
        public int Storage4;
        public int Storage5;
        public string Slot1;
        public string Slot2;
        public string Slot3;
        public string Slot4;
        public string Slot5;
        public string Slot6;
        public string Slot7;
        public string Slot8;
        public string Slot9;
        public string Slot10;
        public string Slot11;
        public string Slot12;
        public int Quan1;
        public int Quan2;
        public int Quan3;
        public int Quan4;
        public int Quan5;
        public int Quan6;
        public int Quan7;
        public int Quan8;
        public int Quan9;
        public int Quan10;
        public int Quan11;
        public int Quan12;
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
        public int bankamount;
        public int openaccount;
        #endregion

        RPData RP;

        public ItemStorage(RPData RP)
        {
            this.RP = RP;
        }

        public void SetBank()
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `bank` WHERE id = '" + RP.habbo.Id + "' LIMIT 1");
                var dataRow = DB.GetRow();
                bankamount = (int)dataRow["amount"];
                openaccount = (int)dataRow["active"];
                Storage1 = (int)dataRow["storage1"];
                Storage2 = (int)dataRow["storage2"];
                Storage3 = (int)dataRow["storage3"];
                Storage4 = (int)dataRow["storage4"];
                Storage5 = (int)dataRow["storage5"];
                if (Storage1 == 0)
                {
                    DB.RunQuery("INSERT INTO `storage` (owner) VALUES ('" + RP.habbo.Id + "')");
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "'");
                    Storage1 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage1 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage1 = '" + Storage1 + "' WHERE id = '" + RP.habbo.Id + "'");
                }
            }
        }

        public int ItemHP(string item)
        {
             if (item == "w1")
             {
                 if (Slot1 == "kevlar2")
                     return 150;
                 else if (Slot1 == "kevlar3")
                     return 200;
                 else if (Slot1 == "kevlar4")
                     return 350;
             }
             else if (item == "w2")
             {
                 if (Slot2 == "kevlar2")
                     return 150;
                 else if (Slot2 == "kevlar3")
                     return 200;
                 else if (Slot2 == "kevlar4")
                     return 350;
             }
             else if (item == "w3")
             {
                 if (Slot3 == "kevlar2")
                     return 150;
                 else if (Slot3 == "kevlar3")
                     return 200;
                 else if (Slot3 == "kevlar4")
                     return 350;
             }
             else if (item == "w4")
             {
                 if (Slot4 == "kevlar2")
                     return 150;
                 else if (Slot4 == "kevlar3")
                     return 200;
                 else if (Slot4 == "kevlar4")
                     return 350;
             }
             else if (item == "w5")
             {
                 if (Slot5 == "kevlar2")
                     return 150;
                 else if (Slot5 == "kevlar3")
                     return 200;
                 else if (Slot5 == "kevlar4")
                     return 350;
             }
             else if (item == "w6")
             {
                 if (Slot6 == "kevlar2")
                     return 150;
                 else if (Slot6 == "kevlar3")
                     return 200;
                 else if (Slot6 == "kevlar4")
                     return 350;
             }
             else if (item == "w7")
             {
                 if (Slot7 == "kevlar2")
                     return 150;
                 else if (Slot7 == "kevlar3")
                     return 200;
                 else if (Slot7 == "kevlar4")
                     return 350;
             }
             else if (item == "w8")
             {
                 if (Slot8 == "kevlar2")
                     return 150;
                 else if (Slot8 == "kevlar3")
                     return 200;
                 else if (Slot8 == "kevlar4")
                     return 350;
             }
             else if (item == "w9")
             {
                 if (Slot9 == "kevlar2")
                     return 150;
                 else if (Slot9 == "kevlar3")
                     return 200;
                 else if (Slot9 == "kevlar4")
                     return 350;
             }
             else if (item == "w10")
             {
                 if (Slot10 == "kevlar2")
                     return 150;
                 else if (Slot10 == "kevlar3")
                     return 200;
                 else if (Slot10 == "kevlar4")
                     return 350;
             }
             else if (item == "w11")
             {
                 if (Slot11 == "kevlar2")
                     return 150;
                 else if (Slot11 == "kevlar3")
                     return 200;
                 else if (Slot11 == "kevlar4")
                     return 350;
             }
             else if (item == "w12")
             {
                 if (Slot12 == "kevlar2")
                     return 150;
                 else if (Slot12 == "kevlar3")
                     return 200;
                 else if (Slot12 == "kevlar4")
                     return 350;
             } 
            return 100;
        }

        public void StorageCache(int CacheThis, bool bypass = false)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (CacheThis == 1 || bypass)
                    DB.RunQuery("UPDATE storage SET slot1 = '" + Slot1 + "', hp1 = '" + HP1 + "', quantity1 = '" + Quan1 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 2 || bypass)
                    DB.RunQuery("UPDATE storage SET slot2 = '" + Slot2 + "', hp2 = '" + HP2 + "', quantity2 = '" + Quan2 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 3 || bypass)
                    DB.RunQuery("UPDATE storage SET slot3 = '" + Slot3 + "', hp3 = '" + HP3 + "', quantity3 = '" + Quan3 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 4 || bypass)
                    DB.RunQuery("UPDATE storage SET slot4 = '" + Slot4 + "', hp4 = '" + HP4 + "', quantity4 = '" + Quan4 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 5 || bypass)
                    DB.RunQuery("UPDATE storage SET slot5 = '" + Slot5 + "', hp5 = '" + HP5 + "', quantity5 = '" + Quan5 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 6 || bypass)
                    DB.RunQuery("UPDATE storage SET slot6 = '" + Slot6 + "', hp6 = '" + HP6 + "', quantity6 = '" + Quan6 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 7 || bypass)
                    DB.RunQuery("UPDATE storage SET slot7 = '" + Slot7 + "', hp7 = '" + HP7 + "', quantity7 = '" + Quan7 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 8 || bypass)
                    DB.RunQuery("UPDATE storage SET slot8 = '" + Slot8 + "', hp8 = '" + HP8 + "', quantity8 = '" + Quan8 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 9 || bypass)
                    DB.RunQuery("UPDATE storage SET slot9 = '" + Slot9 + "', hp9 = '" + HP9 + "', quantity9 = '" + Quan9 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 10 || bypass)
                    DB.RunQuery("UPDATE storage SET slot10 = '" + Slot10 + "', hp10 = '" + HP10 + "', quantity10 = '" + Quan10 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 11 || bypass)
                    DB.RunQuery("UPDATE storage SET slot11 = '" + Slot11 + "', hp11 = '" + HP11 + "', quantity11 = '" + Quan11 + "' WHERE id = '" + Curstorage + "'");
                if (CacheThis == 12 || bypass)
                    DB.RunQuery("UPDATE storage SET slot12 = '" + Slot12 + "', hp12 = '" + HP12 + "', quantity12 = '" + Quan12 + "' WHERE id = '" + Curstorage + "'");
            }
        }

        public void SetStorage()
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.RunQuery("INSERT INTO `storage` (owner) VALUES ('" + RP.habbo.Id + "')");
                if (Storage1 == 0)
                {
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "'");
                    Storage1 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage1 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage1 = '" + Storage1 + "' WHERE id = '" + RP.habbo.Id + "'");
                    return;
                }
                else if (Storage2 == 0)
                {
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "' AND taken = '0'");
                    Storage2 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage2 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage2 = '" + Storage2 + "' WHERE id = '" + RP.habbo.Id + "'");
                    return;
                }
                else if (Storage3 == 0)
                {
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "' AND taken = '0'");
                    Storage3 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage3 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage3 = '" + Storage3 + "' WHERE id = '" + RP.habbo.Id + "'");
                    return;
                }
                else if (Storage4 == 0)
                {
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "' AND taken = '0'");
                    Storage4 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage4 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage4 = '" + Storage4 + "' WHERE id = '" + RP.habbo.Id + "'");
                    return;
                }
                else if (Storage5 == 0)
                {
                    DB.SetQuery("SELECT id FROM storage WHERE owner = '" + RP.habbo.Id + "' AND taken = '0'");
                    Storage5 = DB.GetInteger();
                    DB.RunQuery("UPDATE `storage` SET taken = '1' WHERE id = '" + Storage5 + "'");
                    DB.RunQuery("UPDATE `bank` SET storage5 = '" + Storage5 + "' WHERE id = '" + RP.habbo.Id + "'");
                    return;
                }

            }
        }

        public void Storage(int id)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `storage` WHERE id = '" + id + "' LIMIT 1");
                var dataRow = DB.GetRow();
                Slot1 = (string)dataRow["slot1"];
                Slot2 = (string)dataRow["slot2"];
                Slot3 = (string)dataRow["slot3"];
                Slot4 = (string)dataRow["slot4"];
                Slot5 = (string)dataRow["slot5"];
                Slot6 = (string)dataRow["slot6"];
                Slot7 = (string)dataRow["slot7"];
                Slot8 = (string)dataRow["slot8"];
                Slot9 = (string)dataRow["slot9"];
                Slot10 = (string)dataRow["slot10"];
                Slot11 = (string)dataRow["slot11"];
                Slot12 = (string)dataRow["slot12"];
                HP1 = (int)dataRow["hp1"];
                HP2 = (int)dataRow["hp2"];
                HP3 = (int)dataRow["hp3"];
                HP4 = (int)dataRow["hp4"];
                HP5 = (int)dataRow["hp5"];
                HP6 = (int)dataRow["hp6"];
                HP7 = (int)dataRow["hp7"];
                HP8 = (int)dataRow["hp8"];
                HP9 = (int)dataRow["hp9"];
                HP10 = (int)dataRow["hp10"];
                HP11 = (int)dataRow["hp11"];
                HP12 = (int)dataRow["hp12"];
                Quan1 = (int)dataRow["quantity1"];
                Quan2 = (int)dataRow["quantity2"];
                Quan3 = (int)dataRow["quantity3"];
                Quan4 = (int)dataRow["quantity4"];
                Quan5 = (int)dataRow["quantity5"];
                Quan6 = (int)dataRow["quantity6"];
                Quan7 = (int)dataRow["quantity7"];
                Quan8 = (int)dataRow["quantity8"];
                Quan9 = (int)dataRow["quantity9"];
                Quan10 = (int)dataRow["quantity10"];
                Quan11 = (int)dataRow["quantity11"];
                Quan12 = (int)dataRow["quantity12"];
                Curstorage = id;
            }
        }

        public string GetItem(string item)
        {
            if (item == "w1")
                return Slot1;
            else if (item == "w2")
                return Slot2;
            else if (item == "w3")
                return Slot3;
            else if (item == "w4")
                return Slot4;
            else if (item == "w5")
                return Slot5;
            else if (item == "w6")
                return Slot6;
            else if (item == "w7")
                return Slot7;
            else if (item == "w8")
                return Slot8;
            else if (item == "w9")
                return Slot9;
            else if (item == "w10")
                return Slot10;
            else if (item == "w11")
                return Slot11;
            else if (item == "w12")
                return Slot12;
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
                return Quan1;
            else if (item == "w2")
                return Quan2;
            else if (item == "w3")
                return Quan3;
            else if (item == "w4")
                return Quan4;
            else if (item == "w5")
                return Quan5;
            else if (item == "w6")
                return Quan6;
            else if (item == "w7")
                return Quan7;
            else if (item == "w8")
                return Quan8;
            else if (item == "w9")
                return Quan9;
            else if (item == "w10")
                return Quan10;
            else if (item == "w11")
                return Quan11;
            else if (item == "w12")
                return Quan12;
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
                Slot1 = data;
            else if (id == "w2")
                Slot2 = data;
            else if (id == "w3")
                Slot3 = data;
            else if (id == "w4")
                Slot4 = data;
            else if (id == "w5")
                Slot5 = data;
            else if (id == "w6")
                Slot6 = data;
            else if (id == "w7")
                Slot7 = data;
            else if (id == "w8")
                Slot8 = data;
            else if (id == "w9")
                Slot9 = data;
            else if (id == "w10")
                Slot10 = data;
            else if (id == "w11")
                Slot11 = data;
            else if (id == "w12")
                Slot12 = data;
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
                Quan1 = data;
            else if (id == "w2")
                Quan2 = data;
            else if (id == "w3")
                Quan3 = data;
            else if (id == "w4")
                Quan4 = data;
            else if (id == "w5")
                Quan5 = data;
            else if (id == "w6")
                Quan6 = data;
            else if (id == "w7")
                Quan7 = data;
            else if (id == "w8")
                Quan8 = data;
            else if (id == "w9")
                Quan9 = data;
            else if (id == "w10")
                Quan10 = data;
            else if (id == "w11")
                Quan11 = data;
            else if (id == "w12")
                Quan12 = data;
        }

        public void AddStorage(string item, int hp, string slot = "", bool remove = false)
        {
            int id = 0;
            if (!remove)
            {
                if (item.Contains("stun"))
                {
                    RP.client.SendWhisper("You can not add this item into your storage!");
                    return;
                }
                if (item == Slot1 && RP.Inventory.CheckItem(item) && Quan1 < 50)
                {
                    Quan1++;
                    id = 1;
                }
                else if (item == Slot2 && RP.Inventory.CheckItem(item) && Quan2 < 50)
                {
                    Quan2++;
                    id = 2;
                }
                else if (item == Slot3 && RP.Inventory.CheckItem(item) && Quan3 < 50)
                {
                    Quan3++;
                    id = 3;
                }
                else if (item == Slot4 && RP.Inventory.CheckItem(item) && Quan4 < 50)
                {
                    Quan4++;
                    id = 4;
                }
                else if (item == Slot5 && RP.Inventory.CheckItem(item) && Quan5 < 50)
                {
                    Quan5++;
                    id = 5;
                }
                else if (item == Slot6 && RP.Inventory.CheckItem(item) && Quan6 < 50)
                {
                    Quan6++;
                    id = 6;
                }
                else if (item == Slot7 && RP.Inventory.CheckItem(item) && Quan7 < 50)
                {
                    Quan7++;
                    id = 7;
                }
                else if (item == Slot8 && RP.Inventory.CheckItem(item) && Quan8 < 50)
                {
                    Quan8++;
                    id = 8;
                }
                else if (item == Slot9 && RP.Inventory.CheckItem(item) && Quan9 < 50)
                {
                    Quan9++;
                    id = 9;
                }
                else if (item == Slot10 && RP.Inventory.CheckItem(item) && Quan10 < 50)
                {
                    Quan10++;
                    id = 10;
                }
                else if (item == Slot11 && RP.Inventory.CheckItem(item) && Quan11 < 50)
                {
                    Quan11++;
                    id = 11;
                }
                else if (item == Slot12 && RP.Inventory.CheckItem(item) && Quan12 < 50)
                {
                    Quan12++;
                    id = 12;
                }
                else if (Slot1 == "null")
                {
                    Slot1 = item;
                    HP1 = hp;
                    id = 1;
                    Quan1 = 1;
                }
                else if (Slot2 == "null")
                {
                    Slot2 = item;
                    HP2 = hp;
                    id = 2;
                    Quan2 = 1;
                }
                else if (Slot3 == "null")
                {
                    Slot3 = item;
                    HP3 = hp;
                    id = 3;
                    Quan3 = 1;
                }
                else if (Slot4 == "null")
                {
                    Slot4 = item;
                    HP4 = hp;
                    id = 4;
                    Quan4 = 1;
                }
                else if (Slot5 == "null")
                {
                    Slot5 = item;
                    HP5 = hp;
                    id = 5;
                    Quan5 = 1;
                }
                else if (Slot6 == "null")
                {
                    Slot6 = item;
                    HP6 = hp;
                    id = 6;
                    Quan6 = 1;
                }
                else if (Slot7 == "null")
                {
                    Slot7 = item;
                    HP7 = hp;
                    id = 7;
                    Quan7 = 1;
                }
                else if (Slot8 == "null")
                {
                    Slot8 = item;
                    HP8 = hp;
                    id = 8;
                    Quan8 = 1;
                }
                else if (Slot9 == "null")
                {
                    Slot9 = item;
                    HP9 = hp;
                    id = 9;
                    Quan9 = 1;
                }
                else if (Slot10 == "null")
                {
                    Slot10 = item;
                    HP10 = hp;
                    id = 10;
                    Quan10 = 1;
                }
                else if (Slot11 == "null")
                {
                    Slot11 = item;
                    HP11 = hp;
                    id = 11;
                    Quan11 = 1;
                }
                else if (Slot12 == "null")
                {
                    Slot12 = item;
                    HP12 = hp;
                    id = 12;
                    Quan12 = 1;
                }
                else
                {
                    RP.GetClient().SendWhisper("Your storage box is currently full!");
                    return;
                }
                RP.Inventory.Additem(slot, true);
            }
            else
            {
                int Slothp = 100;
                if (item == "w1" && RP.Inventory.CheckItem(Slot1) && Quan1 > 1)
                {
                    Quan1--;
                    id = 1;
                }
                else if (item == "w2" && RP.Inventory.CheckItem(Slot2) && Quan2 > 1)
                {
                    Quan2--;
                    id = 2;
                }
                else if (item == "w3" && RP.Inventory.CheckItem(Slot3) && Quan3 > 1)
                {
                    Quan3--;
                    id = 3;
                }
                else if (item == "w4" && RP.Inventory.CheckItem(Slot4) && Quan4 > 1)
                {
                    Quan4--;
                    id = 4;
                }
                else if (item == "w5" && RP.Inventory.CheckItem(Slot5) && Quan5 > 1)
                {
                    Quan5--;
                    id = 5;
                }
                else if (item == "w6" && RP.Inventory.CheckItem(Slot6) && Quan6 > 1)
                {
                    Quan6--;
                    id = 6;
                }
                else if (item == "w7" && RP.Inventory.CheckItem(Slot7) && Quan7 > 1)
                {
                    Quan7--;
                    id = 7;
                }
                else if (item == "w8" && RP.Inventory.CheckItem(Slot8) && Quan8 > 1)
                {
                    Quan8--;
                    id = 8;
                }
                else if (item == "w9" && RP.Inventory.CheckItem(Slot9) && Quan9 > 1)
                {
                    Quan9--;
                    id = 9;
                }
                else if (item == "w10" && RP.Inventory.CheckItem(Slot10) && Quan10 > 1)
                {
                    Quan10--;
                    id = 10;
                }
                else if (item == "w11" && RP.Inventory.CheckItem(Slot11) && Quan11 > 1)
                {
                    Quan11--;
                    id = 11;
                }
                else if (item == "w12" && RP.Inventory.CheckItem(Slot12) && Quan12 > 1)
                {
                    Quan12--;
                    id = 12;
                }
                else if (item == "w1")
                {
                    id = 1;
                    Slot1 = "null";
                    Quan1 = 0;
                    Slothp = HP1;
                }
                else if (item == "w2")
                {
                    id = 2;
                    Slot2 = "null";
                    Quan2 = 0;
                    Slothp = HP2;
                }
                else if (item == "w3")
                {
                    id = 3;
                    Slot3 = "null";
                    Quan3 = 0;
                    Slothp = HP3;
                }
                else if (item == "w4")
                {
                    id = 4;
                    Slot4 = "null";
                    Quan4 = 0;
                    Slothp = HP4;
                }
                else if (item == "w5")
                {
                    id = 5;
                    Slot5 = "null";
                    Quan5 = 0;
                    Slothp = HP5;
                }
                else if (item == "w6")
                {
                    id = 6;
                    Slot6 = "null";
                    Quan6 = 0;
                    Slothp = HP6;
                }
                else if (item == "w7")
                {
                    id = 7;
                    Slot7 = "null";
                    Quan7 = 0;
                    Slothp = HP7;
                }
                else if (item == "w8")
                {
                    id = 8;
                    Slot8 = "null";
                    Quan8 = 0;
                    Slothp = HP8;
                }
                else if (item == "w9")
                {
                    id = 9;
                    Slot9 = "null";
                    Quan9 = 0;
                    Slothp = HP9;
                }
                else if (item == "w10")
                {
                    id = 10;
                    Slot10 = "null";
                    Quan10 = 0;
                    Slothp = HP10;
                }
                else if (item == "w11")
                {
                    id = 11;
                    Slot11 = "null";
                    Quan11 = 0;
                    Slothp = HP11;
                }
                else if (item == "w12")
                {
                    id = 12;
                    Slot12 = "null";
                    Quan12 = 0;
                    Slothp = HP12;
                }
                RP.Inventory.Additem(slot, false, true, 1, false, Slothp);
            }
            UpdateStorage(Curstorage);
            StorageCache(id);            
        }

        public void UpdateStorage(int ID, bool bypass = false)
        {
            RP.SendWeb("{\"name\":\"storage\", \"bslot1\":\"" + RP.Storage.Slot1 + "\","
                + "\"bslot2\":\"" + RP.Storage.Slot2 + "\", \"bslot3\":\"" + RP.Storage.Slot3 + "\", \"bslot4\":\"" + RP.Storage.Slot4 + "\","
                + "\"bslot5\":\"" + RP.Storage.Slot5 + "\", \"bslot6\":\"" + RP.Storage.Slot6 + "\", \"bslot7\":\"" + RP.Storage.Slot7 + "\","
                + "\"bslot8\":\"" + RP.Storage.Slot8 + "\", \"bslot9\":\"" + RP.Storage.Slot9 + "\", \"bslot10\":\"" + RP.Storage.Slot10 + "\", \"bslot11\":\"" + RP.Storage.Slot11 + "\","
                + "\"bslot12\":\"" + RP.Storage.Slot12 + "\", \"quantity1\":\"" + RP.Storage.Quan1 + "\", \"quantity2\":\"" + RP.Storage.Quan2 + "\", \"quantity3\":\"" + RP.Storage.Quan3 + "\","
                + "\"quantity4\":\"" + RP.Storage.Quan4 + "\", \"quantity5\":\"" + RP.Storage.Quan5 + "\", \"quantity6\":\"" + RP.Storage.Quan6 + "\", \"quantity8\":\"" + RP.Storage.Quan8 + "\","
                + "\"quantity9\":\"" + RP.Storage.Quan9 + "\", \"quantity10\":\"" + RP.Storage.Quan10 + "\", \"quantity11\":\"" + RP.Storage.Quan11 + "\", \"quantity12\":\"" + RP.Storage.Quan12 + "\","
                + "\"quantity7\":\"" + RP.Storage.Quan7 + "\", \"hp1\":\"" + (RP.Storage.ItemHP("w1") - RP.Storage.HP1) + "\", \"hp2\":\"" + (RP.Storage.ItemHP("w2") - RP.Storage.HP2) + "\","
                + "\"hp3\":\"" + (RP.Storage.ItemHP("w3") - RP.Storage.HP3) + "\", \"hp4\":\"" + (RP.Storage.ItemHP("w4") - RP.Storage.HP4) + "\", \"hp5\":\"" + (RP.Storage.ItemHP("w5") - RP.Storage.HP5) + "\","
                + "\"hp6\":\"" + (RP.Storage.ItemHP("w6") - RP.Storage.HP6) + "\", \"hp7\":\"" + (RP.Storage.ItemHP("w7") - RP.Storage.HP7) + "\", \"hp8\":\"" + (RP.Storage.ItemHP("w8") - RP.Storage.HP8) + "\","
                + "\"hp9\":\"" + (RP.Storage.ItemHP("w9") - RP.Storage.HP9) + "\", \"hp10\":\"" + (RP.Storage.ItemHP("w10") - RP.Storage.HP10) + "\", \"hp11\":\"" + (RP.Storage.ItemHP("w11") - RP.Storage.HP11) + "\","
                + "\"hp12\":\"" + (RP.Storage.ItemHP("w12") - RP.Storage.HP12) + "\", \"maxhp1\":\"" + RP.Storage.ItemHP("w1") + "\", \"maxhp2\":\"" + RP.Storage.ItemHP("w2") + "\","
                + "\"maxhp3\":\"" + RP.Storage.ItemHP("w3") + "\", \"maxhp4\":\"" + RP.Storage.ItemHP("w4") + "\", \"maxhp5\":\"" + RP.Storage.ItemHP("w5") + "\", \"maxhp6\":\"" + RP.Storage.ItemHP("w6") + "\","
                + "\"maxhp7\":\"" + RP.Storage.ItemHP("w7") + "\", \"maxhp8\":\"" + RP.Storage.ItemHP("w8") + "\", \"maxhp9\":\"" + RP.Storage.ItemHP("w9") + "\", \"maxhp10\":\"" + RP.Storage.ItemHP("w10") + "\","
                + "\"maxhp11\":\"" + RP.Storage.ItemHP("w11") + "\", \"maxhp12\":\"" + RP.Storage.ItemHP("w12") + "\", \"id\":\"" + ID + "\", \"bypass\":\"" + bypass + "\"}");
        }
    }
}
