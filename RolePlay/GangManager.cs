using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Plus.Database.Interfaces;

namespace Plus.RolePlay.Gang
{
    public class GangManager
    {
        
        #region Personal
        public int Id;
        public string Name;
        public int OwnerId;
        public string Color1;
        public string Color2;
        #endregion

        #region Gang Turfs
        public int Turf1;
        public int Turf2;
        public int Turf3;
        public int Turf4;
        #endregion

        #region Relationship
        public string Ally1;
        public string Ally2;
        public string Rival1;
        public string Rival2;
        #endregion

        #region Basic Stats
        public int Kills;
        public int Hits;
        public int Arrests;
        public int JB;
        public int Level;
        public int XP;
        public int XPdue;
        #endregion

        #region Rank Name
        public string Rank1;
        public string Rank2;
        public string Rank3;
        public string Rank4;
        public string Rank5;
        #endregion

        #region Members ID
        public int Id1;
        public int Id2;
        public int Id3;
        public int Id4;
        public int Id5;
        public int Id6;
        public int Id7;
        public int Id8;
        public int Id9;
        public int Id10;
        #endregion

        #region Members Row
        public int row1_1;
        public int row1_2;
        public int row1_3;
        public int row1_4;
        public int row1_5;

        public int row2_1;
        public int row2_2;
        public int row2_3;
        public int row2_4;
        public int row2_5;

        public int row3_1;
        public int row3_2;
        public int row3_3;
        public int row3_4;
        public int row3_5;

        public int row4_1;
        public int row4_2;
        public int row4_3;
        public int row4_4;
        public int row4_5;

        public int row5_1;
        public int row5_2;
        public int row5_3;
        public int row5_4;
        public int row5_5;
        #endregion


        public GangManager(int gangid)
        {
            this.Id = gangid;
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `gang` WHERE id = '" + this.Id + "' LIMIT 1");
                DataTable table = DB.GetTable();
                foreach (DataRow dataRow in table.Rows)
                {
                    Name = (string)dataRow["name"];
                    OwnerId = (int)dataRow["owner"];
                    Color1 = (string)dataRow["color1"];
                    Color2 = (string)dataRow["color2"];
                    Turf1 = (int)dataRow["turf1"];
                    Turf2 = (int)dataRow["turf2"];
                    Turf3 = (int)dataRow["turf3"];
                    Turf4 = (int)dataRow["turf4"];
                    Ally1 = (string)dataRow["ally1"];
                    Ally2 = (string)dataRow["ally2"];
                    Rival1 = (string)dataRow["rival1"];
                    Rival2 = (string)dataRow["rival2"];
                    Kills = (int)dataRow["kills"];
                    Hits = (int)dataRow["hits"];
                    Arrests = (int)dataRow["arrests"];
                    JB = (int)dataRow["jb"];
                    Level = (int)dataRow["level"];
                    XP = (int)dataRow["xp"];
                    XPdue = (int)dataRow["xpdue"];
                    Rank1 = (string)dataRow["rank1"];
                    Rank2 = (string)dataRow["rank2"];
                    Rank3 = (string)dataRow["rank3"];
                    Rank4 = (string)dataRow["rank4"];
                    Rank5 = (string)dataRow["rank5"];
                    Id1 = (int)dataRow["id1"];
                    Id2 = (int)dataRow["id2"];
                    Id3 = (int)dataRow["id3"];
                    Id4 = (int)dataRow["id4"];
                    Id5 = (int)dataRow["id5"];
                    Id6 = (int)dataRow["id6"];
                    Id7 = (int)dataRow["id7"];
                    Id8 = (int)dataRow["id8"];
                    Id9 = (int)dataRow["id9"];
                    Id10 = (int)dataRow["id10"];

                    if (XP >= XPdue)
                    {
                        Level++;
                        XPdue *= 2;
                    }
                }
            }
        }
        public void UpdateGang(int xp)
        {
            XP += xp;
            if (XP >= XPdue)
            {
                Level++;
                XPdue *= 2;
            }
        }
        public void SaveGang()
        {
            using (IQueryAdapter DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.RunQuery("UPDATE gang SET kills = '" + Kills + "', hits = '" + Hits + "', arrests = '" + Arrests + "', jb = '" + JB + "', xp = '" + XP + "', xpdue = '" + XPdue + "', level = '" + Level + "', color1 = '" + Color1 + "',"
           + "color2 = '" + Color2 + "', rank1 = '" + Rank1 + "', rank2 = '" + Rank2 + "', rank3 = '" + Rank3 + "', rank4 = '" + Rank4 + "', rank5 = '" + Rank5 + "', rival1 = '" + Rival1 + "', rival2 = '" + Rival2 + "', ally1 = '" + Ally1 + "',"
            + "ally2 = '" + Ally2 + "', id1 = '" + Id1 + "', id2 = '" + Id2 + "', id3 = '" + Id3 + "', id4 = '" + Id4 + "', id5 = '" + Id5 + "', id6 = '" + Id6 + "', id7 = '" + Id7 + "', id8 = '" + Id8 + "', id9 = '" + Id9 + "', id10 = '" + Id10 + "'  WHERE id = '" + Id + "'");
            }
        }
        public void Clear(int id, bool onlyrow = false)
        {
                if (row1_1 == id)
                    row1_1 = 0;
                if (row1_2 == id)
                    row1_2 = 0;
                if (row1_3 == id)
                    row1_3 = 0;
                if (row1_4 == id)
                    row1_4 = 0;
                if (row1_5 == id)
                    row1_5 = 0;

                if (row2_1 == id)
                    row2_1 = 0;
                if (row2_2 == id)
                    row2_2 = 0;
                if (row2_3 == id)
                    row2_3 = 0;
                if (row2_4 == id)
                    row2_4 = 0;
                if (row2_5 == id)
                    row2_5 = 0;

                if (row3_1 == id)
                    row3_1 = 0;
                if (row3_2 == id)
                    row3_2 = 0;
                if (row3_3 == id)
                    row3_3 = 0;
                if (row3_4 == id)
                    row3_4 = 0;
                if (row3_5 == id)
                    row3_5 = 0;

                if (row4_1 == id)
                    row4_1 = 0;
                if (row4_2 == id)
                    row4_2 = 0;
                if (row4_3 == id)
                    row4_3 = 0;
                if (row4_4 == id)
                    row4_4 = 0;
                if (row4_5 == id)
                    row4_5 = 0;

                if (row5_1 == id)
                    row5_1 = 0;
                if (row5_2 == id)
                    row5_2 = 0;
                if (row5_3 == id)
                    row5_3 = 0;
                if (row5_4 == id)
                    row5_4 = 0;
                if (row5_5 == id)
                    row5_5 = 0;
                if (!onlyrow)
                {
                    if (Id1 == id)
                        Id1 = 0;
                    if (Id2 == id)
                        Id2 = 0;
                    if (Id3 == id)
                        Id3 = 0;
                    if (Id4 == id)
                        Id4 = 0;
                    if (Id5 == id)
                        Id5 = 0;
                    if (Id6 == id)
                        Id6 = 0;
                    if (Id7 == id)
                        Id7 = 0;
                    if (Id8 == id)
                        Id8 = 0;
                    if (Id9 == id)
                        Id9 = 0;
                    if (Id10 == id)
                        Id10 = 0;

                    SaveGang();
                }
        }
    }
}
