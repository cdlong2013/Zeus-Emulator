﻿namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserRightsComposer : MessageComposer
    {
        public int Rank { get; }

        public UserRightsComposer(int rank)
            : base(ServerPacketHeader.UserRightsMessageComposer)
        {
            Rank = rank;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(2); //Club level
            packet.WriteInteger(Rank);
            packet.WriteBoolean(false); //Is an ambassador
        }
    }
}