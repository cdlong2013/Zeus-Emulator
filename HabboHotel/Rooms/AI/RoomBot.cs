using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.HabboHotel.Rooms.AI.Types;

namespace Plus.HabboHotel.Rooms.AI
{
    public class RoomBot
    {
        public int Id;
        public int BotId;
        public int VirtualId;

        public BotAIType AiType;

        public int DanceId;
        public string Gender;

        public string Look;
        public string Motto;
        public string Name;
        public int RoomId;
        public int Rot;
        public int Escorting = 0;
        public int Master;


        public string WalkingMode;

        public int X;
        public int Y;
        public double Z;
        public int MaxX;
        public int MaxY;
        public int MinX;
        public int MinY;

        public int Health;
        public bool Stunned;
        public bool Cuffed;
        public bool Arrested;
        public int Job;
        public int ownerID;
        public int lockedon;

        public int OwnerId;

        public bool AutomaticChat;
        public int SpeakingInterval;
        public bool MixSentences;

        public RoomUser RoomUser;
        public List<RandomSpeech> RandomSpeech;

        public bool ForcedMovement { get; set; }
        public int ForcedUserTargetMovement { get; set; }
        public Point TargetCoordinate { get; set; }

        public int TargetUser { get; set; }

        public RoomBot(int BotId, int RoomId, string AiType, string WalkingMode, string Name, string Motto, string Look, int X, int Y, double Z, int Rot,
           int minX, int minY, int maxX, int maxY, ref List<RandomSpeech> Speeches, string Gender, int Dance, int ownerID,
           bool AutomaticChat, int SpeakingInterval, bool MixSentences, int ChatBubble, int Job, int Health)
        {
            this.Id = BotId;
            this.BotId = BotId;
            this.RoomId = RoomId;

            this.Name = Name;
            this.Motto = Motto;
            this.Look = Look;
            this.Gender = Gender.ToUpper();

            this.AiType = BotUtility.GetAIFromString(AiType);
            this.WalkingMode = WalkingMode;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Rot = Rot;
            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;

            this.VirtualId = -1;
            this.RoomUser = null;
            this.DanceId = Dance;

            this.LoadRandomSpeech(Speeches);
            //this.LoadResponses(Responses);

            this.ownerID = ownerID;

            this.AutomaticChat = AutomaticChat;
            this.SpeakingInterval = SpeakingInterval;
            this.MixSentences = MixSentences;

            this.ChatBubble = ChatBubble;
            this.ForcedMovement = false;
            this.TargetCoordinate = new Point();
            this.TargetUser = 0;
            this.Job = Job;
            this.Health = Health;
        }

        public bool IsPet => AiType == BotAIType.Pet;

        public void HealthChange(int i)
        {
            Health = i;
            if (lockedon > 0)
            {
                foreach (var User in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                {
                    if (User == null)
                        return;
                    if (User.GetRolePlay().lockID == Id)
                    {
                        User.GetRolePlay().SendWeb("{\"name\":\"targethealth\", \"health\":\"" + Health + "\", \"maxhealth\":\"100\"}");
                        if (Health < 1)
                        {
                            User.GetRolePlay().SendWeb("{\"name\":\"lock\", \"lock\":\"true\"}");
                            User.GetRolePlay().lockBot = 0;
                        }
                    }
                }
            }
        }

        #region Speech Related

        public void LoadRandomSpeech(List<RandomSpeech> speeches)
        {
            RandomSpeech = new List<RandomSpeech>();
            foreach (RandomSpeech speech in speeches)
            {
                if (speech.BotId == BotId)
                    RandomSpeech.Add(speech);
            }
        }

        public RandomSpeech GetRandomSpeech()
        {
            var rand = new Random();

            if (RandomSpeech.Count < 1)
                return new RandomSpeech("", 0);
            return RandomSpeech[rand.Next(0, RandomSpeech.Count - 1)];
        }

        #endregion

        #region AI Related

        public BotAI GenerateBotAI(int virtualId)
        {
            switch (AiType)
            {
                case BotAIType.Pet:
                    return new PetBot(virtualId);
                case BotAIType.Generic:
                    return new GenericBot(virtualId);
                case BotAIType.Bartender:
                    return new BartenderBot(virtualId);
                default:
                    return new GenericBot(virtualId);
            }
        }

        #endregion

        public int ChatBubble { get; set; }
    }
}