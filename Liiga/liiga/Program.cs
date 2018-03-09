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
            db.set_db_testdata(@"db\liiga_testdata_dump.sql");

            List<string> teams = new List<string>();
            teams.Add("Kärpät");

            List<Match> afterDate = db.SelectBeforeOrAfterDate(teams, "2017-01-01", true);
            teams.Add("TPS");
            List<Match> matchesBetween = db.SelectBetweenTeams(teams);

            List<List<Match>> results = new List<List<Match>>();
            results.Add(afterDate);
            results.Add(matchesBetween);

            List<Match> join = db.Join(results);
            Console.WriteLine("Join count: " + join.Count);
            Console.WriteLine("OK");
            Console.ReadLine();
        }
    }
}
