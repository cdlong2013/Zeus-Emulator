using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.Rooms;

namespace Plus.RolePlay.Job
{
    public class JobManager
    {
        #region Job Values
        public int Job;
        public int JobRank;
        public string JobDate;
        public string JobLook;
        public string JobMotto;
        public int JobRoom;
        public int JobPay;
        public int Sendhome;
        public bool Working;
        public int Boss;
        public int Curpage = 1;
        public string Curcall = "";
        public int Shifts;
        public int Task1;
        public int Task2;
        public int Task3;
        public int Jobsec;
        public int Jobmin;
        #endregion

        RPData RP;

        public JobManager(RPData RP)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.RP = RP;
                DB.SetQuery("SELECT * FROM `stats` WHERE id = '" + RP.habbo.Id + "' LIMIT 1");
                foreach (DataRow stat in DB.GetTable().Rows)
                {
                    this.Job = (int)stat["job"];
                    this.JobRank = (int)stat["jobrank"];
                    this.Shifts = (int)stat["shifts"];
                    this.JobDate = (string)stat["jobdate"];
                    this.Jobmin = (int)stat["jobmin"];
                    this.Jobsec = (int)stat["jobsec"];
                    this.Task1 = (int)stat["task1"];
                    this.Task2 = (int)stat["task2"];
                    this.Task3 = (int)stat["task3"];
                    if (Job > 0)
                        SetJob();
                }
            }
        }

        public string SetJob(bool showbadge = false, bool date = false)
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `job_info` WHERE job = '" + Job + "' AND rank = '" + JobRank + "' AND gender = '" + RP.habbo.Gender + "' LIMIT 1");
                DataRow dataRow = DB.GetRow();
                if (dataRow == null)
                {
                    Job = 0;
                    JobRank = 0;
                    return "null";
                }
                JobLook = (string)dataRow["outfit"];
                JobPay = (int)dataRow["pay"];
                JobRoom = (int)dataRow["room"];
                JobMotto = (string)dataRow["motto"];
                Boss = (int)dataRow["boss"];
                if (showbadge)
                {
                    Plus.HabboHotel.Groups.Group Group = null;
                    if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Job, out Group))
                        return "";
                    RP.Room.SendPacket(new HabboGroupBadgesComposer(Group));
                    RP.Room.SendPacket(new UpdateFavouriteGroupComposer(null, RP.roomUser.VirtualId));
                  
                    RP.habbo.GetStats().FavouriteGroupId = Job;
                    DB.RunQuery("UPDATE `user_stats` SET `groupid` = " + Job + " WHERE `id` = '" + RP.habbo.Id + "' LIMIT 1");
                }
                if (!date)
                    return "";
                DateTime datevalue = (Convert.ToDateTime(DateTime.Now.ToString().ToString()));
                JobDate = RP.datecheck(Convert.ToInt32(datevalue.Month)) + "-" + datevalue.Day.ToString() + "-" + datevalue.Year.ToString();
                RP.RPCache(25);
                return "";
            }
        }

        public void LeaveJob()
        {
            Job = 0;
            JobPay = 0;
            JobRank = 0;
            JobRoom = 0;
            JobMotto = "";
            JobLook = "";
            Boss = 0;
            Jobmin = 0;
            Jobsec = 0;
            Shifts = 0;
            Task1 = 0;
            Task2 = 0;
            Task3 = 0;
            RP.RPCache(26);
            RP.RPCache(27);
            RP.RPCache(28);
            RP.RPCache(20);
            RP.Leavegroup();
            RP.stungun = "null";
        }
    }
}
