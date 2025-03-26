using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.RolePlay;

namespace Plus.RolePlay.Police
{
    public class PoliceCall
    {
        public int ID;
        public string Name;
        public string RoomName;
        public int RoomID;
        public string Message;
        public string Look;
        public string IsBot;
        public string Time = "0 seconds ago";
        public string Color;
        int TimerCount1;
        int TimerCount2;
        int TimerCount3;
        int CycleCount;

        public PoliceCall(string Name, string RoomName, int RoomID, string Message, string Look, string IsBot, int ID, string Color)
        {
            this.Name = Name;
            this.RoomName = RoomName;
            this.RoomID = RoomID;
            this.Message = Message;
            this.Look = Look;
            this.IsBot = IsBot;
            this.ID = ID;
            this.Color = Color;
        }
        public void OnCycle()
        {
            try
            {
                CycleCount++;
                if (CycleCount >= 37)
                {
                    CycleCount = 0;
                    TimerCount1++;
                    if (TimerCount1 > 60)
                    {
                        TimerCount1 = 0;
                        TimerCount2++;
                        if (TimerCount2 > 60)
                        {
                            TimerCount2 = 0;
                            TimerCount3++;
                        }
                    }
                    if (TimerCount3 > 0)
                    {
                        if (TimerCount3 == 1)
                            Time = TimerCount3 + " hour ago";
                        else Time = TimerCount3 + " hours ago";
                        foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().JobManager.Curpage == ID && ((client.GetRolePlay().JobManager.Job == 1 && client.GetRolePlay().JobManager.Working) || client.GetRolePlay().onduty)) client.GetRolePlay().SendWeb("{\"name\":\"911timer\", \"time\":\"" + Time + "\"}");
                    }
                    else if (TimerCount2 > 0)
                    {
                        if (TimerCount2 == 1)
                            Time = TimerCount2 + " minute ago";
                        else Time = TimerCount2 + " minutes ago";
                        foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().JobManager.Curpage == ID && ((client.GetRolePlay().JobManager.Job == 1 && client.GetRolePlay().JobManager.Working) || client.GetRolePlay().onduty))
                                client.GetRolePlay().SendWeb("{\"name\":\"911timer\", \"time\":\"" + Time + "\"}");
                    }
                    else
                    {
                        if (TimerCount1 == 1)
                            Time = TimerCount1 + " second ago";
                        else Time = TimerCount1 + " seconds ago";
                        foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().JobManager.Curpage == ID && ((client.GetRolePlay().JobManager.Job == 1 && client.GetRolePlay().JobManager.Working) || client.GetRolePlay().onduty)) client.GetRolePlay().SendWeb("{\"name\":\"911timer\", \"time\":\"" + Time + "\"}");
                    }
                }
            }
            catch { }
        }
    }
}