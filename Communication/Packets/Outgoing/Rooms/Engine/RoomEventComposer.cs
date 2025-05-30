﻿using System;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomEventComposer : MessageComposer
    {
        public RoomData Data { get; }
        public RoomPromotion Promotion { get; }

        public RoomEventComposer(RoomData data, RoomPromotion promotion)
            : base(ServerPacketHeader.RoomEventMessageComposer)
        {
            Data = data;
            Promotion = promotion;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Promotion == null ? -1 : Convert.ToInt32(Data.Id));
            packet.WriteInteger(Promotion == null ? -1 : Data.OwnerId);
            packet.WriteString(Promotion == null ? "" : Data.OwnerName);
            packet.WriteInteger(Promotion == null ? 0 : 1);
            packet.WriteInteger(0);
            packet.WriteString(Promotion == null ? "" : Promotion.Name);
            packet.WriteString(Promotion == null ? "" : Promotion.Description);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0); //Unknown, came in build RELEASE63-201411181343-400753188
        }
    }
}