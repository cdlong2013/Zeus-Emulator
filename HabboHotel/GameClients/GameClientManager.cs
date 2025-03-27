using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DotNetty.Transport.Channels;
using log4net;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Users.Messenger;
using Plus.RolePlay.Police;
using Plus.RolePlay.WantedList;

namespace Plus.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameClientManager));

        public List<PoliceCall> PoliceList = new List<PoliceCall>();
        public List<WantedList> WantedList = new List<WantedList>();
        public readonly ConcurrentDictionary<IChannelId, GameClient> _clients;
        private readonly ConcurrentDictionary<int, GameClient> _userIdRegister;
        private readonly ConcurrentDictionary<string, GameClient> _usernameRegister;

        private readonly Queue _timedOutConnections;

        private readonly Stopwatch _clientPingStopwatch;
        internal string NorthTurf;
        internal string SouthTurf;
        internal string WestTurf;
        internal string EastTurf;
        internal int NYPDactive;
        internal int STAFFactive;
        internal bool GameOver;
        internal bool GameInSession;
        internal string WiningTeam;
        public int HospWorkers;
        public int Paramedics;
        public bool weather;
        public bool pickup;
        public int weatherTimer;
        public int weatherTime;
        public int DayTime = 1;
        public int DayTimer;
        public int Hue = 132;
        public int Sat = 169;
        public int Light = 231;
        public int PoliceCalls;
        public int WLCount;
        
        public GameClientManager()
        {

             _clients = new ConcurrentDictionary<IChannelId, GameClient>();
            _userIdRegister = new ConcurrentDictionary<int, GameClient>();
            _usernameRegister = new ConcurrentDictionary<string, GameClient>();




            _timedOutConnections = new Queue();

            _clientPingStopwatch = new Stopwatch();
            _clientPingStopwatch.Start();
        }
      

        public void OnCycle()
        {
            TestClientConnections();
            HandleTimeouts();
        }

        public GameClient GetClientByUserId(int userId)
        {
            if (_userIdRegister.ContainsKey(userId))
                return _userIdRegister[userId];
            return null;
        }

        public GameClient GetClientByUsername(string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                return _usernameRegister[username.ToLower()];
            return null;
        }

        public bool TryGetClient(IChannelId clientId, out GameClient client)
        {
            return _clients.TryGetValue(clientId, out client);
        }

        public bool UpdateClientUsername(GameClient client, string oldUsername, string newUsername)
        {
            if (client == null || !_usernameRegister.ContainsKey(oldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(oldUsername.ToLower(), out client);
            _usernameRegister.TryAdd(newUsername.ToLower(), client);
            return true;
        }

        public string GetNameById(int id)
        {
            GameClient client = GetClientByUserId(id);

            if (client != null)
                return client.GetHabbo().Username;

            string username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", id);
                username = dbClient.GetString();
            }

            return username;
        }

        public IEnumerable<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            foreach (int id in users)
            {
                GameClient client = GetClientByUserId(id);
                if (client != null)
                    yield return client;
            }
        }

        public void GlobalWeb(string data)
        {
            if (data == null)
                return;
            foreach (GameClient client in this._clients.Values.ToList())
            {
                  if (client != null)
                    client.GetRolePlay().SendWeb(data);
            }
        }
        public void StaffAlert(MessageComposer message, int exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 2 || client.GetHabbo().Id == exclude)
                    continue;

                client.SendPacket(message);
            }
        }

        public void ModAlert(string message)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") &&
                    !client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try
                    {
                        client.SendWhisper(message, 5);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void DoAdvertisingReport(GameClient reporter, GameClient target)
        {
            if (reporter == null || target == null || reporter.GetHabbo() == null || target.GetHabbo() == null)
                return;

            StringBuilder builder = new();
            builder.Append("New report submitted!\r\r");
            builder.Append("Reporter: " + reporter.GetHabbo().Username + "\r");
            builder.Append("Reported User: " + target.GetHabbo().Username + "\r\r");
            builder.Append(target.GetHabbo().Username + "s last 10 messages:\r\r");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + target.GetHabbo().Id +
                                  "' ORDER BY `id` DESC LIMIT 10");
                int number = 11;
                var reader = dbClient.GetTable();
                foreach (DataRow row in reader.Rows)
                {
                    number -= 1;
                    builder.Append(number + ": " + row["message"] + "\r");
                }
            }

            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") && !client.GetHabbo().GetPermissions()
                    .HasRight("staff_ignore_advertisement_reports"))
                    client.SendPacket(new MotdNotificationComposer(builder.ToString()));
            }
        }


        public void SendPacket(MessageComposer packet, string fuse = "")
        {
            foreach (GameClient client in _clients.Values.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!client.GetHabbo().GetPermissions().HasRight(fuse))
                        continue;
                }

                client.SendPacket(packet);
            }
        }

        public void CreateAndStartClient(IChannelHandlerContext connection)
        {
            GameClient client = new(connection);
            if (_clients.TryAdd(connection.Channel.Id, client))
            {
                //Hmmmmm?
            }
            else
                connection.CloseAsync();
        }

        public void DisposeConnection(IChannelId clientId)
        {
            GameClient Client = null;
            if (!TryGetClient(clientId, out Client))
                return;
            if (Client != null && Client.GetRolePlay() != null)
                Client.Dispose(false);
            else
                Client.Dispose(false);

            _clients.TryRemove(clientId, out Client);
        }
 

        public WantedList GetWL(string Name = "", int ID = 0)
        {
            foreach (var Data in WantedList)
            {
                if (Data != null)
                {
                    if (Name != "" && Data.Name == Name && Data.ID > 0)
                        return Data;
                    else if (ID != 0 && Data.ID == ID)
                        return Data;
                }
            }
            return null;
        }

        public void RemoveWL(int ID)
        {
            var _Wanted = GetWL("", ID);
            if (_Wanted == null)
                return;
            WLCount--;
            foreach (var Data in WantedList)
            {
                if (Data != null)
                {
                    if (Data.ID > _Wanted.ID)
                        Data.ID--;
                }
            }
            _Wanted.ID = 0;
            foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen)
                {
                    if (WLCount == 0)
                    {
                        client.GetRolePlay().WLCurPage = 1;
                        client.GetRolePlay().SendWeb("{\"name\":\"wl\", \"closewl\":\"1\"}");
                        client.SendWhisper("There is currently no users on the wanted list!");
                    }
                    else if (client.GetRolePlay().WLCurPage == 1)
                        client.GetRolePlay().WebHandler.Handle("wl", "", "true");
                    else client.GetRolePlay().WebHandler.Handle("wl_back", "", "");
                }
            var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(_Wanted.Name);
            if (user != null)
                user.GetRolePlay().Wanted = false;
        }

        public void AddWL(string Name, string Look, string Reason, string Color)
        {
            WLCount++;
            if (Color == "#000000" || String.IsNullOrEmpty(Color))
                Color = "#FFFFFF";
            this.WantedList.Add(new WantedList(Name, Look, WLCount, Color, Reason));
            foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen)
                    client.GetRolePlay().SendWeb("{\"name\":\"wl_page\", \"pagestart\":\"" + client.GetRolePlay().WLCurPage + "\","
                    + "\"pageend\":\"" + WLCount + "\"}");
            var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Name);
            user.GetRolePlay().Wanted = true;
        }

        public bool AddReason(string reason, WantedList WL)
        {
            if (reason == WL.Reason1)
                WL.ReasonCount1++;
            else if (reason == WL.Reason2)
                WL.ReasonCount2++;
            else if (reason == WL.Reason3)
                WL.ReasonCount3++;
            else if (WL.Reason2 == "")
            {
                WL.Reason2 = reason;
                WL.ReasonCount2++;
            }
            else if (WL.Reason3 == "")
            {
                WL.Reason3 = reason;
                WL.ReasonCount3++;
            }
            else return false;
            foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen && client.GetRolePlay().WLCurPage == WL.ID)
                    client.GetRolePlay().WebHandler.Handle("wl", "", "true");
            return true;
        }

        public void AddPC(string Name, string RoomName, int RoomID, string Message, string Look, string IsBot, int ID, string Color = "#000000")
        {
            this.PoliceList.Add(new PoliceCall(Name, RoomName, RoomID, Message, Look, IsBot, ID, Color));
            foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                if (client != null && client.GetRolePlay() != null && ((client.GetRolePlay().JobManager.Job == 1 && client.GetRolePlay().JobManager.Working) || client.GetRolePlay().onduty))
                    client.GetRolePlay().SendWeb("{\"name\":\"copalert\", \"pagestart\":\"" + client.GetRolePlay().JobManager.Curpage + "\","
                    + "\"pageend\":\"" + PoliceCalls + "\"}");
        }

        public PoliceCall GetPoliceCall(int ID)
        {
            foreach (var Data in PoliceList)
            {
                if (Data != null && Data.ID == ID)
                    return Data;
            }
            return null;
        }

        public void RemovePC(int ID)
        {
            var Police = GetPoliceCall(ID);
            if (Police == null)
                return;
            PoliceCalls--;
            foreach (var Data in PoliceList)
            {
                if (Data != null)
                {
                    if (Data.ID > Police.ID)
                        Data.ID--;
                }
            }
            Police.ID = 0;

            foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                if (client != null && client.GetRolePlay() != null && ((client.GetRolePlay().JobManager.Job == 1 && client.GetRolePlay().JobManager.Working) || client.GetRolePlay().onduty))
                {
                    if (PoliceCalls == 0)
                    {
                        client.GetRolePlay().JobManager.Curpage = 1;
                        client.GetRolePlay().SendWeb("{\"name\":\"911\"}");
                        client.SendWhisper("There is currently no help requests!");
                    }
                    else if (client.GetRolePlay().JobManager.Curpage == 1)
                        client.GetRolePlay().WebHandler.Handle("911", "", "null");
                    else client.GetRolePlay().WebHandler.Handle("911back", "", "");
                }
        }


        public void LogClonesOut(int userId)
        {
            GameClient client = GetClientByUserId(userId);
            client?.Disconnect();
        }

        public void RegisterClient(GameClient client, int userId, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client;
            else
                _usernameRegister.TryAdd(username.ToLower(), client);

            if (_userIdRegister.ContainsKey(userId))
                _userIdRegister[userId] = client;
            else
                _userIdRegister.TryAdd(userId, client);
        }

        public void UnregisterClient(int userId, string username)
        {
            _userIdRegister.TryRemove(userId, out GameClient _);
            _usernameRegister.TryRemove(username.ToLower(), out GameClient _);
        }
        public void SetTurfs(int turf, int gangid, string name)
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (turf > 0)
                {
                    if (turf == 1)
                    {
                        DB.RunQuery("UPDATE gang SET turf1 = '0' WHERE turf1 = '1'");
                        DB.RunQuery("UPDATE gang SET turf1 = '1' WHERE id = '" + gangid + "'");
                        NorthTurf = name;
                        GlobalGang(gangid, "Your gang has successfully claimed North Turf!", "Gang Turf", null);
                    }
                    else if (turf == 2)
                    {
                        DB.RunQuery("UPDATE gang SET turf2 = '0' WHERE turf2 = '1'");
                        DB.RunQuery("UPDATE gang SET turf2 = '1' WHERE id = '" + gangid + "'");
                        EastTurf = name;
                        GlobalGang(gangid, "Your gang has successfully claimed East Turf!", "Gang Turf", null);
                    }
                    else if (turf == 3)
                    {
                        DB.RunQuery("UPDATE gang SET turf3 = '0' WHERE turf3 = '1'");
                        DB.RunQuery("UPDATE gang SET turf3 = '1' WHERE id = '" + gangid + "'");
                        WestTurf = name;
                        GlobalGang(gangid, "Your gang has successfully claimed West Turf!", "Gang Turf", null);
                    }
                    else if (turf == 4)
                    {
                        DB.RunQuery("UPDATE gang SET turf4 = '0' WHERE turf4 = '1'");
                        DB.RunQuery("UPDATE gang SET turf4 = '1' WHERE id = '" + gangid + "'");
                        SouthTurf = name;
                        GlobalGang(gangid, "Your gang has successfully claimed South Turf!", "Gang Turf", null);
                    }
                }
                else
                {

                    DB.SetQuery("SELECT name FROM gang WHERE turf4 = '1' LIMIT 1");
                    SouthTurf = DB.GetString();
                    DB.SetQuery("SELECT name FROM gang WHERE turf3 = '1' LIMIT 1");
                    WestTurf = DB.GetString();
                    DB.SetQuery("SELECT name FROM gang WHERE turf2 = '1' LIMIT 1");
                    EastTurf = DB.GetString();
                    DB.SetQuery("SELECT name FROM gang WHERE turf1 = '1' LIMIT 1");
                    NorthTurf = DB.GetString();

                }
            }
        }
        public void GlobalParamedic(string name, string roomname, int roomid)
        {
            foreach (GameClient client in this._clients.Values.ToList())
            {
                if (client != null)
                {
                    var This = client.GetRolePlay();
                    if (This.JobManager.Job == 7 && This.JobManager.Working)
                    {
                        This.SendWeb("{\"name\":\"medicalert\"}");
                        client.SendWhisper(name + " passed out at " + roomname + " [" + roomid + "]");
                    }
                }
            }
        }

        public void GlobalGang(int gangid, string message, string name, string gangname)
        {
            foreach (GameClient client in this._clients.Values.ToList())
                if ((client != null && client.GetRolePlay().Gang == gangid && gangid != 0) || client.GetRolePlay().GangManager.Name == gangname)
                    if (message != null)
                        client.SendWhisper("[Gang Alert] " + message + " [" + name + "]", 3);
        }

        public void CloseAll()
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client?.GetHabbo() != null)
                {
                    try
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery(client.GetHabbo().GetQueryString);
                        }

                        Console.Clear();
                        Log.Info("<<- SERVER SHUTDOWN ->> INVENTORY IS SAVING");
                    }
                    catch
                    {
                    }
                }
            }

            Log.Info("Done saving users inventory!");
            Log.Info("Closing server connections...");
            try
            {
                foreach (GameClient client in GetClients.ToList())
                {
                    if (client == null)
                        continue;

                    try
                    {
                        client.Dispose(false);
                    }
                    catch
                    {
                    }
                }

                Console.Clear();
                Log.Info("<<- SERVER SHUTDOWN ->> CLOSING CONNECTIONS");
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (_clients.Count > 0)
                _clients.Clear();

            Log.Info("Connections closed!");
        }

        private void TestClientConnections()
        {
            if (_clientPingStopwatch.ElapsedMilliseconds >= 30000)
            {
                _clientPingStopwatch.Restart();

                List<GameClient> toPing = new();

                foreach (GameClient client in _clients.Values.ToList())
                {
                    if (client.PingCount < 6)
                    {
                        client.PingCount++;

                        toPing.Add(client);
                    }
                    else
                    {
                        lock (_timedOutConnections.SyncRoot)
                        {
                            _timedOutConnections.Enqueue(client);
                        }
                    }
                }

                DateTime start = DateTime.Now;

                foreach (GameClient client in toPing.ToList())
                {
                    try
                    {
                        client.SendPacket(new PongComposer());
                    }
                    catch
                    {
                        lock (_timedOutConnections.SyncRoot)
                        {
                            _timedOutConnections.Enqueue(client);
                        }
                    }
                }
            }
        }

        private void HandleTimeouts()
        {
            if (_timedOutConnections.Count > 0)
            {
                lock (_timedOutConnections.SyncRoot)
                {
                    while (_timedOutConnections.Count > 0)
                    {
                        GameClient client = null;

                        if (_timedOutConnections.Count > 0)
                            client = (GameClient) _timedOutConnections.Dequeue();

                        client?.Disconnect();
                    }
                }
            }
        }

        public int Count => _clients.Count;

        public ICollection<GameClient> GetClients => _clients.Values;
    }
}