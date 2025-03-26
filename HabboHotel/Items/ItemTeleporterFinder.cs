using System;
using System.Data;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items
{
    public static class ItemTeleporterFinder
    {
        public static int GetLinkedTele(int TeleId, Room pRoom)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `tele_two_id` FROM `room_items_tele_links` WHERE `tele_one_id` = '" + TeleId + "' LIMIT 1");
                DataRow Row = dbClient.GetRow();

                if (Row == null)
                {
                    return 0;
                }

                return Convert.ToInt32(Row[0]);
            }
        }

        public static int GetTeleRoomId(int teleId, Room pRoom)
        {
            if (pRoom.GetRoomItemHandler().GetItem(teleId) != null)
                return pRoom.RoomId;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `room_id` FROM `items` WHERE `id` = " + teleId + " LIMIT 1");
                DataRow row = dbClient.GetRow();

                if (row == null)
                {
                    return 0;
                }

                return Convert.ToInt32(row[0]);
            }
        }

        public static bool IsTeleLinked(int TeleId, Room pRoom)
        {
            int LinkId = GetLinkedTele(TeleId, pRoom);

            if (LinkId == 0)
            {
                return false;
            }


            Item item = pRoom.GetRoomItemHandler().GetItem(LinkId);
            if (item != null && item.GetBaseItem().InteractionType == InteractionType.Teleport)
                return true;

            int RoomId = GetTeleRoomId(LinkId, pRoom);

            if (RoomId == 0)
            {
                return false;
            }

            return true;
        }
    }
}