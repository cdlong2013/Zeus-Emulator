﻿namespace Plus.Communication.Packets.Outgoing.Users
{
    internal class UpdateUsernameComposer : MessageComposer
    {
        public string Username { get; }

        public UpdateUsernameComposer(string username)
            : base(ServerPacketHeader.UpdateUsernameMessageComposer)
        {
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
            packet.WriteString(Username);
            packet.WriteInteger(0);
        }
    }
}