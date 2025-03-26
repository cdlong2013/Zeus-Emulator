using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.RolePlay.WantedList
{
    public class WantedList
    {
        public int ID;
        public string Name;
        public string Color;
        public string Look;
        public string Reason1 = "";
        public string Reason2 = "";
        public string Reason3 = "";
        public int ReasonCount1;
        public int ReasonCount2;
        public int ReasonCount3;
        public string Time = "0 seconds ago";
        int TimerCount1;
        int TimerCount2;
        int TimerCount3;
        int CycleCount;

        public WantedList(string Name, string Look, int ID, string Color, string Reason1)
        {
            this.Name = Name;
            this.Look = Look;
            this.ID = ID;
            this.Color = Color;
            this.Reason1 = Reason1;
            this.ReasonCount1++;
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
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen && client.GetRolePlay().WLCurPage == ID)
                                client.GetRolePlay().SendWeb("{\"name\":\"wl_timer\", \"time\":\"" + Time + "\"}");
                    }
                    else if (TimerCount2 > 0)
                    {
                        if (TimerCount2 == 1)
                            Time = TimerCount2 + " minute ago";
                        else Time = TimerCount2 + " minutes ago";
                        foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen && client.GetRolePlay().WLCurPage == ID)
                                client.GetRolePlay().SendWeb("{\"name\":\"wl_timer\", \"time\":\"" + Time + "\"}");
                    }
                    else
                    {
                        if (TimerCount1 == 1)
                            Time = TimerCount1 + " second ago";
                        else Time = TimerCount1 + " seconds ago";
                        foreach (var client in PlusEnvironment.GetGame().GetClientManager()._clients.Values.ToList())
                            if (client != null && client.GetRolePlay() != null && client.GetRolePlay().WLopen && client.GetRolePlay().WLCurPage == ID)
                                client.GetRolePlay().SendWeb("{\"name\":\"wl_timer\", \"time\":\"" + Time + "\"}");
                    }
                }
            }
            catch { }
        }
        
    }
}
