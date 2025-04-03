using Plus.Database.Interfaces;
using Plus.HabboHotel.Groups;
using Plus.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace Plus.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public RoomAccess Access { get; set; }
        public string Type { get; set; }
        public int UsersMax { get; set; }
        public int UsersNow { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public string Floor { get; set; }
        public string Landscape { get; set; }
        public int AllowPets { get; set; }
        public int AllowPetsEating { get; set; }
        public int RoomBlockingEnabled { get; set; }
        public int HideWall { get; set; }
        public int WallThickness { get; set; }
        public int FloorThickness { get; set; }
        public string Wallpaper { get; set; }
        public int WhoCanMute { get; set; }
        public int WhoCanBan { get; set; }
        public int WhoCanKick { get; set; }
        public int ChatMode { get; set; }
        public int ChatSize { get; set; }
        public int ChatSpeed { get; set; }
        public int ExtraFlood { get; set; }
        public int ChatDistance { get; set; }
        public int TradeSettings { get; set; }
        public bool PushEnabled { get; set; }
        public bool PullEnabled { get; set; }
        public bool SuperPushEnabled { get; set; }
        public bool SuperPullEnabled { get; set; }
        public bool EnablesEnabled { get; set; }
        public bool RespectNotificationsEnabled { get; set; }
        public bool PetMorphsAllowed { get; set; }
        public int SalePrice { get; set; }
        public bool ReverseRollers { get; set; }
        public bool LayEnabled { get; set; }
        public int Fight { get; set; }
        public int Taxi { get; set; }


        public List<string> Tags;

        private Group _group;


        public RoomModel Model { get; set; }

        public RoomData(int id,string caption, string modelName, string ownerName, int ownerId, string password, int score, string type, string access, int usersNow, int usersMax, int category, string description,
            string tags, string floor, string landscape, int allowPets, int allowPetsEating, int roomBlockingEnabled, int hideWall, int wallThickness, int floorThickness, string wallpaper, int muteSettings,
            int banSettings, int kickSettings, int chatMode, int chatSize, int chatSpeed, int extraFlood, int chatDistance, int tradeSettings, bool pushEnabled, bool pullEnabled, bool superPushEnabled,
            bool superPullEnabled, bool enablesEnabled, bool respectedNotificationsEnabled, bool petMorphsAllowed, int groupId, int salePrice, bool layEnabled, int fight, int taxi, RoomModel model)
        {
            Id = id;
            Name = caption;
            ModelName = modelName;
            OwnerName = ownerName;
            OwnerId = ownerId;
            Password = password;
            Score = score;
            Type = type;
            Access = RoomAccessUtility.ToRoomAccess(access);
            UsersNow = usersNow;
            UsersMax = usersMax;
            Category = category;
            Description = description;

            Tags = new List<string>();
            foreach (string tag in tags.Split(','))
            {
                Tags.Add(tag);
            }

            Floor = floor;
            Landscape = landscape;
            AllowPets = allowPets;
            AllowPetsEating = allowPetsEating;
            RoomBlockingEnabled = roomBlockingEnabled;
            HideWall = hideWall;
            WallThickness = wallThickness;
            FloorThickness = floorThickness;
            Wallpaper = wallpaper;
            WhoCanMute = muteSettings;
            WhoCanBan = banSettings;
            WhoCanKick = kickSettings;
            ChatMode = chatMode;
            ChatSize = chatSize;
            ChatSpeed = chatSpeed;
            ExtraFlood = extraFlood;
            ChatDistance = chatDistance;
            TradeSettings = tradeSettings;
            PushEnabled = pushEnabled;
            PullEnabled = pullEnabled;
            SuperPushEnabled = superPushEnabled;
            SuperPullEnabled = superPullEnabled;
            EnablesEnabled = enablesEnabled;
            RespectNotificationsEnabled = respectedNotificationsEnabled;
            PetMorphsAllowed = petMorphsAllowed;
            SalePrice = salePrice;
            ReverseRollers = false;
            LayEnabled = layEnabled;
            

            if (groupId > 0)
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out _group);

            LoadPromotions();

            Model = model;
        }

        public RoomPromotion Promotion { get; set; }

        public void Fill(DataRow Row)
        {
            Id = Convert.ToInt32(Row["id"]);
            Name = Convert.ToString(Row["caption"]);
            Description = Convert.ToString(Row["description"]);
            Type = Convert.ToString(Row["roomtype"]);
            OwnerId = Convert.ToInt32(Row["owner"]);

            OwnerName = "";
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @owner LIMIT 1");
                dbClient.AddParameter("owner", OwnerId);
                string result = dbClient.GetString();
                if (!String.IsNullOrEmpty(result))
                    OwnerName = result;
            }

            this.Access = RoomAccessUtility.ToRoomAccess(Row["state"].ToString().ToLower());

            Category = Convert.ToInt32(Row["category"]);
            if (!string.IsNullOrEmpty(Row["users_now"].ToString()))
                UsersNow = Convert.ToInt32(Row["users_now"]);
            else
                UsersNow = 0;
            UsersMax = Convert.ToInt32(Row["users_max"]);
            ModelName = Convert.ToString(Row["model_name"]);
            Score = Convert.ToInt32(Row["score"]);
            Tags = new List<string>();
            AllowPets = Convert.ToInt32(Row["allow_pets"].ToString());
            AllowPetsEating = Convert.ToInt32(Row["allow_pets_eat"].ToString());
            RoomBlockingEnabled = Convert.ToInt32(Row["room_blocking_disabled"].ToString());
            HideWall = Convert.ToInt32(Row["allow_hidewall"].ToString());
            Password = Convert.ToString(Row["password"]);
            Wallpaper = Convert.ToString(Row["wallpaper"]);
            Floor = Convert.ToString(Row["floor"]);
            Landscape = Convert.ToString(Row["landscape"]);
            FloorThickness = Convert.ToInt32(Row["floorthick"]);
            WallThickness = Convert.ToInt32(Row["wallthick"]);
            WhoCanMute = Convert.ToInt32(Row["mute_settings"]);
            WhoCanKick = Convert.ToInt32(Row["kick_settings"]);
            WhoCanBan = Convert.ToInt32(Row["ban_settings"]);
            ChatMode = Convert.ToInt32(Row["chat_mode"]);
            ChatSpeed = Convert.ToInt32(Row["chat_speed"]);
            ChatSize = Convert.ToInt32(Row["chat_size"]);
            TradeSettings = Convert.ToInt32(Row["trade_settings"]);
            Fight = Convert.ToInt32(Row["fight"]);
            Taxi = Convert.ToInt32(Row["taxi"]);
            Group G = null;
            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(Row["group_id"]), out G))
                Group = G;
            else
                Group = null;

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                Tags.Add(Tag);
            }

            Model = PlusEnvironment.GetGame().GetRoomManager().GetModel(ModelName);

            this.PushEnabled = PlusEnvironment.EnumToBool(Row["push_enabled"].ToString());
            this.PullEnabled = PlusEnvironment.EnumToBool(Row["pull_enabled"].ToString());
            this.SuperPushEnabled = PlusEnvironment.EnumToBool(Row["spush_enabled"].ToString());
            this.SuperPullEnabled = PlusEnvironment.EnumToBool(Row["spull_enabled"].ToString());
            this.EnablesEnabled = PlusEnvironment.EnumToBool(Row["enables_enabled"].ToString());
            this.RespectNotificationsEnabled = PlusEnvironment.EnumToBool(Row["respect_notifications_enabled"].ToString());
            this.PetMorphsAllowed = PlusEnvironment.EnumToBool(Row["pet_morphs_allowed"].ToString());
        }

        public Group Group
        {
            get => _group;
            set => _group = value;
        }

        public RoomData(RoomData data)
        {
            Id = data.Id;
            Name = data.Name;
            ModelName = data.ModelName;
            OwnerName = data.OwnerName;
            OwnerId = data.OwnerId;
            Password = data.Password;
            Score = data.Score;
            Type = data.Type;
            Access = data.Access;
            UsersNow = data.UsersNow;
            UsersMax = data.UsersMax;
            Category = data.Category;
            Description = data.Description;
            Tags = data.Tags;
            Floor = data.Floor;
            Landscape = data.Landscape;
            AllowPets = data.AllowPets;
            AllowPetsEating = data.AllowPetsEating;
            RoomBlockingEnabled = data.RoomBlockingEnabled;
            HideWall = data.HideWall;
            WallThickness = data.WallThickness;
            FloorThickness = data.FloorThickness;
            Wallpaper = data.Wallpaper;
            WhoCanMute = data.WhoCanMute;
            WhoCanBan = data.WhoCanBan;
            WhoCanKick = data.WhoCanKick;
            ChatMode = data.ChatMode;
            ChatSize = data.ChatSize;
            ChatSpeed = data.ChatSpeed;
            ExtraFlood = data.ExtraFlood;
            ChatDistance = data.ChatDistance;
            TradeSettings = data.TradeSettings;
            PushEnabled = data.PushEnabled;
            PullEnabled = data.PullEnabled;
            SuperPushEnabled = data.SuperPushEnabled;
            SuperPullEnabled = data.SuperPullEnabled;
            RespectNotificationsEnabled = data.RespectNotificationsEnabled;
            PetMorphsAllowed = data.PetMorphsAllowed;
            Group = data.Group;
            SalePrice = data.SalePrice;
            EnablesEnabled = data.EnablesEnabled;
            ReverseRollers = data.ReverseRollers;
            LayEnabled = data.LayEnabled;

            Model = data.Model;
        }

        public void LoadPromotions()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                DataRow getPromotion = dbClient.GetRow();

                if (getPromotion != null)
                {
                    if (Convert.ToDouble(getPromotion["timestamp_expire"]) > UnixTimestamp.GetNow())
                        Promotion = new RoomPromotion(Convert.ToString(getPromotion["title"]), Convert.ToString(getPromotion["description"]), Convert.ToDouble(getPromotion["timestamp_start"]), Convert.ToDouble(getPromotion["timestamp_expire"]), Convert.ToInt32(getPromotion["category_id"]));
                }
            }
        }

        public bool HasActivePromotion => Promotion != null;

        public void EndPromotion()
        {
            if (!HasActivePromotion)
                return;

            Promotion = null;
        }
    }
}