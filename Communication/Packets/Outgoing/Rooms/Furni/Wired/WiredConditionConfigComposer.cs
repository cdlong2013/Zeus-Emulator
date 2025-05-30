﻿using System;
using System.Linq;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredConditionConfigComposer : MessageComposer
    {
        public IWiredItem Box { get; }

        public WiredConditionConfigComposer(IWiredItem box)
            : base(ServerPacketHeader.WiredConditionConfigMessageComposer)
        {
            Box = box;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(5);

            packet.WriteInteger(Box.SetItems.Count);
            foreach (Item item in Box.SetItems.Values.ToList())
            {
                packet.WriteInteger(item.Id);
            }

            packet.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(Box.Item.Id);
            packet.WriteString(Box.StringData);

            if (Box.Type == WiredBoxType.ConditionMatchStateAndPosition || Box.Type == WiredBoxType.ConditionDontMatchStateAndPosition)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0;0";

                packet.WriteInteger(3); //Loop
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.ConditionUserCountInRoom || Box.Type == WiredBoxType.ConditionUserCountDoesntInRoom)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                packet.WriteInteger(2); //Loop
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 1);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 50);
            }

            if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni)
                packet.WriteInteger(1);

            if (Box.Type != WiredBoxType.ConditionUserCountInRoom && Box.Type != WiredBoxType.ConditionUserCountDoesntInRoom && Box.Type != WiredBoxType.ConditionFurniHasNoFurni)
                packet.WriteInteger(0);
            else if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0";
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 50);
            }

            packet.WriteInteger(0);
            packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
        }
    }
}