using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liiga
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();
            db.setConnectionString("Data Source=db\\liiga.db; Version=3;");
            db.set_db_schema(@"db\liiga_schema_dump.sql");
            //db.set_db_testdata(@"db\liiga_testdata_dump.sql");
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("JYP");
            teams.Add("TPS");

            List<string> seasons = new List<string>();
            seasons.Add("17-18");

            List<Match> results = db.SelectBetweenTeamsFromSeasons(teams, seasons);

            foreach(Match m in results)
            {
                string ot = "";

                if (m.overtime)
                    ot = "Overtime";

                Console.WriteLine(String.Format("{0} - {1} {2} - {3} {4}", m.hometeam, m.awayteam, m.homescore, m.awayscore, ot));


            }

            Console.WriteLine("OK");
            Console.ReadLine();
        }
    }
}
