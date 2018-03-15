using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

namespace Liiga
{
    [TestFixture]
    public class Database_Match_Test
    {
        public Database db = new Database();

        [OneTimeSetUp]
        public void setUpTests()
        {
            /* setUp class sets Database variables and sets the path to the correct directory. */
            Directory.SetCurrentDirectory(@"C:\Users\iirop\Documents\Visual Studio 2015\Projects\Liiga\Liiga");
            db.CreateDatabase(@"db\liigaTest.db");
            db.setConnectionString("Data Source=db\\liigaTest.db; Version=3;");
            db.set_db_schema("db\\liiga_schema_dump.sql");
            db.set_db_testdata("db\\liiga_testdata_dump.sql");
            db.CreateTables();
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            /* Deletes the created test database. */
            db.DeleteDatabase(@"db\liigaTest.db");
        }

        [Test]
        public void test_CreateDatabase()
        {
            /* create a database instance. Deletes it once tasks are done.*/
            string path = @"db\liigaTest1.db";
            Database database = new Database();
            Assert.False(File.Exists(path));
            database.CreateDatabase(path);
            Assert.True(File.Exists(path));
            database.DeleteDatabase(path);
        }

        [Test]
        public void test_DeleteDatabase()
        {
            /* Tries to delete non existing database, creates a database instance and deletes it. */
            string path = @"db\liigaTest1.db";
            Assert.False(File.Exists(path)); //first confirm that file does not exist to begin with
            Database database = new Database();
            int result = database.DeleteDatabase(path);
            Assert.AreEqual(-1, result);

            database.CreateDatabase(path);
            result = database.DeleteDatabase(path);
            Assert.AreEqual(1, result);

            Assert.False(File.Exists(path));
        }

        [Test]
        public void test_CreateTables()
        {
            /* Creates a new database instance, creates a database and tables without setting 
                schema attribute and connectionString. This should produce DatabaseError. Then 
                sets the needed variables and creates table. This should return 1.
                
                Deletes the database after completing the tasks.
             */

            Database database = new Database();
            database.CreateDatabase(@"db\liigaTest1.db");
            Assert.Throws<DatabaseError>(() => database.CreateTables()); 

            database.setConnectionString("Data Source=db\\liigaTest1.db; Version=3;");
            database.set_db_schema("db\\liiga_schema_dump.sql");
            var test = database.CreateTables(); 

            Assert.AreEqual(1, test);
            database.DeleteDatabase(@"db\liigaTest1.db");
        }

        [Test]
        public void test_CreateMatch()
        {
            /*need to set the paths correctly before executing the actual tests.
             1. Test creating a correct match.
             2. Test creating a match with incorrect date format.
             3. Test creating an existing match.*/                  
            int result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09", true, "18-19");
            Assert.That(result, Is.EqualTo(1));
            Assert.Throws<DateConversionError>(() => db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "201465-44-34", true, "18-19"));
            result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09",true, "18-19");
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void test_CreateMatch_existing()
        {
            /* Creates a row to matches table witch already exists. Should return -1*/
            db.ClearTables();
            int result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09",true, "18-19");
            Assert.AreEqual(1, result);
            result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09",true, "18-19");
            Assert.AreEqual(-1, result);
            db.ClearTables();
            db.FillTables();
        }

        [Test]
        public void test_SelectBetweenTeams()
        {
            /* Test with test data , kärpät, tps and tappara. Function should
             * returns all rows which contain only teams from parameter list (=3). 
               Add JYP to list, should return 4 matches.
               
             Test also with a list consisting of one teams name several times.*/
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("TPS");

            db.FillTables();
            List<Match> rows = db.SelectBetweenTeams(teams);
            Assert.AreEqual(6, rows.Count);

            teams.Add("JYP");
            rows = db.SelectBetweenTeams(teams);
            Assert.AreEqual(8, rows.Count);

            teams.Clear();
            teams.Add("TPS");
            teams.Add("TPS");
            teams.Add("TPS");
            rows = db.SelectBetweenTeams(teams);
            Assert.AreEqual(0, rows.Count);
        }

        [Test]
        public void test_SelectFromTeams()
        {
            /* Test with test data , kärpät, tps and tappara. Function should
             * returns all rows which contain teams from parameter list  (=6). 
               Add SaiPa to list, should return 7 matches.
                
            
            Test also with a list of one teams name several times.               
               Clear tables at the end. */
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("TPS");

            db.FillTables();
            List<Match> rows = db.SelectFromTeams(teams);
            Assert.AreEqual(12, rows.Count);

            teams.Add("SaiPa");
            rows = db.SelectFromTeams(teams);
            Assert.AreEqual(14, rows.Count);

            teams.Clear();
            teams.Add("TPS");
            teams.Add("TPS");
            teams.Add("TPS");
            rows = db.SelectFromTeams(teams);
            Assert.AreEqual(6, rows.Count);

            db.ClearTables();
        }

        [Test]
        public void test_SelectMatchesFromTeams0()
        {
            /* Test with test data containing names not in the database.        
               Clear tables at the end. Second SelectMatchesFromTeams 
               gets no teams as parameters and it should return null. */
            List<string> teams = new List<string>();
            teams.Add("Kkäpät");
            teams.Add("Ta");
            teams.Add("TS");

            db.FillTables();
            List<Match> rows = db.SelectFromTeams(teams);
            Assert.AreEqual(0, rows.Count);

            teams.Clear();
            rows = db.SelectFromTeams(teams);
            Assert.IsNull(rows, "rows is not null");
            db.ClearTables();
        }

        [Test]
        public void test_SelectNLastFromTeam()
        {
            /* Query teams last n games.  
             * 
             * n <= 0 parameter should return null.
             * Fill tables before starting
             * and clear afterwards.  */

            db.FillTables();
            List<Match> matches = db.SelectNLastFromTeam(2, "Kärpät");

            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual("2017-03-26", matches[0].date);
            Assert.AreEqual("2017-03-25", matches[1].date);

            matches = db.SelectNLastFromTeam(0, "TPS");
            Assert.IsNull(matches);

            db.ClearTables();

        }

        [Test]
        public void test_SelectAfterDate()
        {
            /* 1. Test with incorrect date format, should throw DateConversionError
               2. Test with a list of Kärpät, Kärpät after date '2017/03/25'. 
                    Should return 2 matches.
               3. Test with list of Kärpät, Kärpät, TPS after date '2017-03-25. 
                    Should return 3 matches.'*/
            db.FillTables();
            List<string> teams = new List<string>();

            teams.Add("Kärpät");
            Assert.Throws<DateConversionError>(() => db.SelectBeforeOrAfterDate(teams, "wrong format", true));

            teams.Add("Kärpät");
            List<Match> rows = db.SelectBeforeOrAfterDate(teams, "2017/03/25", true);
            Assert.AreEqual(2, rows.Count);

            teams.Add("TPS");
            rows = db.SelectBeforeOrAfterDate(teams, "2017/03/25", true);
            Assert.AreEqual(3, rows.Count);
            db.ClearTables();
        }

        [Test]
        public void test_SelectBeforeOrAfterDate_normal()
        {
            /* 
              1. Select * before the date "2017-03-25". Should return 13 matches.
              2. Select * after date "2016-03-26". Should return 14 matches.
              3. Join should return 9 matches.
              4. Union should return 18 matches.
              5. & 6:Test with date "20933-13-13". Should throw DateConversionError.
              */
            db.FillTables();
            List<Match> before = db.SelectBeforeOrAfterDate("2017-03-25", false);
            List<Match> after = db.SelectBeforeOrAfterDate("2016-03-26", true);

            List<List<Match>> results = new List<List<Match>>();
            results.Add(before);
            results.Add(after);

            List<Match> join = db.Join(results);
            List<Match> union = db.Union(results);

            Assert.AreEqual(13, before.Count);
            Assert.AreEqual(14, after.Count);
            Assert.AreEqual(9, join.Count);
            Assert.AreEqual(18, union.Count);
            Assert.Throws<DateConversionError>(() => db.SelectBeforeOrAfterDate("20933-13-13", true));
            Assert.Throws<DateConversionError>(() => db.SelectBeforeOrAfterDate("20933-13-13", false));
            db.ClearTables();
        }

        [Test]
        public void test_SelectBeforeOrAfterDate_teamsOverload()
        {
            /* 1. Select matches before "2017-03-25"  from Kärpät. Should return 5 matches
               2. Select matches after date "2016-03-26" and Kärpät. Should return 4.
               3. Join results should return 4.
               4. Union results should return 5.*/
            db.FillTables();  
            List<string> teams = new List<string>();
            teams.Add("Kärpät");

            List<Match> before = db.SelectBeforeOrAfterDate(teams, "2017-03-25", false);
            Assert.AreEqual(5, before.Count);

            List<Match> after = db.SelectBeforeOrAfterDate(teams, "2016-03-26", true);
            Assert.AreEqual(4, after.Count);

            List<List<Match>> group = new List<List<Match>>();
            group.Add(before);
            group.Add(after);

            List<Match> join = db.Join(group);
            List<Match> union = db.Union(group);

            Assert.AreEqual(3, join.Count);
            Assert.AreEqual(6, union.Count);
            db.ClearTables();
        }

        [Test]
        public void test_SelectBetweenTeamsFromSeasons()
        {
            /*
             *1. Test finding all matches between Kärpät and TPS in 17-18. Should return 1 matches
             * 2. Test finding same team matches from season 16-17 and 17-18. Should return 2
             * 3. Test adding the same season twice doesn't produce double numbers
             */

            List<string> teams = new List<string>();
            List<string> seasons = new List<string>();

            teams.Add("Kärpät");
            teams.Add("TPS");

            seasons.Add("17-18");

            List<Match> matches = db.SelectBetweenTeamsFromSeasons(teams, seasons);
            Assert.AreEqual(1, matches.Count);

            seasons.Add("16-17");
            matches = db.SelectBetweenTeamsFromSeasons(teams, seasons);
            Assert.AreEqual(2, matches.Count);

            seasons.Add("16-17");
            matches = db.SelectBetweenTeamsFromSeasons(teams, seasons);
            Assert.AreNotEqual(3, matches.Count);
            Assert.AreEqual(2, matches.Count);
        }

        [Test]
        public void test_Join()
        {
            /* 1. Create a query for selecting matches after date '2017-01-01' for team Kärpät.
                2. Create a query for selecting matches between Kärpät and TPS
                3. Join the results, should return 1 match.*/
            db.ClearTables();
            db.FillTables();
            List<string> teams = new List<string>();
            teams.Add("Kärpät");

            List<Match> afterDate = db.SelectBeforeOrAfterDate(teams, "2017-01-01", true);
            teams.Add("TPS");
            List<Match> matchesBetween = db.SelectBetweenTeams(teams);

            List<List<Match>> results = new List<List<Match>>();
            results.Add(afterDate);
            results.Add(matchesBetween);

            List<Match> join = db.Join(results);
            Assert.AreEqual(1, join.Count);
        }

        [Test]
        public void test_Join_large()
        {
            /*
              Select matches from season 16-17
              Select where hometeam tps
              Select where gd >= 2. Should return 1*/
            db.FillTables();
            List<string> seasons = new List<string>();
            seasons.Add("16-17");
            List<Match> season = db.SelectFromSeasons(seasons);
            List<string> teams = new List<string>();
            teams.Add("TPS");
            List<Match> home = db.SelectWhereHometeam(teams);
            List<Match> gd = db.SelectWhereGD(2, true);

            List<List<Match>> results = new List<List<Match>>();
            results.Add(season);
            results.Add(home);
            results.Add(gd);

            List<Match> join = db.Join(results);

            db.ClearTables();
        }

        [Test]
        public void test_UnionAndJoin()
        {
            /* 
             1. Query select matches from teams with Kärpät, Tappara, TPS.
             2. Query select matches from teams with Ässät, Lukko.
             3. Union should return 16 results.
             4. Join should return 2 results.*/
            db.ClearTables();
            db.FillTables();
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("TPS");

            List<Match> results1 = db.SelectFromTeams(teams);

            teams.Clear();
            teams.Add("Ässät");
            teams.Add("Lukko");
            List<Match> results2 = db.SelectFromTeams(teams);

            List<List<Match>> lists = new List<List<Match>>();
            lists.Add(results1);
            lists.Add(results2);

            List<Match> union = db.Union(lists);
            List<Match> join = db.Join(lists);

            Assert.AreEqual(14, union.Count);
            Assert.AreEqual(2, join.Count); 
        }

        [Test]
        public void test_UnionAndJoinWithOneAnd0List()
        {
            /* search all matches from teams Kärpät. Union and Join should return 6 results*/
            List<string> teams = new List<string>();
            teams.Add("Kärpät");

            List<Match> results = db.SelectFromTeams(teams);
            Assert.AreEqual(6, results.Count);

            List<List<Match>> matches = new List<List<Match>>();
            matches.Add(results);
            List<Match> join = db.Join(matches);

            List<Match> union = db.Union(matches);

            Assert.AreEqual(6, join.Count);
            Assert.AreEqual(6, union.Count);

            matches.Clear();
            union = db.Union(matches);
            join = db.Join(matches);

            Assert.IsNull(union);
            Assert.IsNull(join);
        }

        [Test]
        public void SelectWhereOvertime()
        {
            /*
             1. Test finding matches where overtime true. Should return 8 matches.
             2. Join these with after date 2017-01-01. Should return 4 matches.
             3. Join part 1 matches with quert overtime = False. Should return 0.
             4. Union of these two should return all matches.*/
            db.FillTables();
            List<Match> otTrue = db.SelectWhereOvertime(true);
            Assert.AreEqual(8, otTrue.Count);

            List<Match> after = db.SelectBeforeOrAfterDate("2017-01-01", true);

            List<List<Match>> results = new List<List<Match>>();
            results.Add(otTrue);
            results.Add(after);

            List<Match> join = db.Join(results);
            Assert.AreEqual(4, join.Count);

            results.Clear();

            List<Match> otFalse = db.SelectWhereOvertime(false);
            results.Add(otTrue);
            results.Add(otFalse);
            join = db.Join(results);
            Assert.AreEqual(0, join.Count);

            List<Match> union = db.Union(results);
            Assert.AreEqual(18, union.Count);
            db.ClearTables();
        }

        [Test]
        public void SelectWhereGD()
        {
            /* 
             1. Select with gd 3, true. Should return 4 rows.
             2. Select with gd 2, false. Should return 14 rows.
             3. Select with gd 0, false. Should return 0 rows.*/
            db.FillTables();
            List<Match> rows = db.SelectWhereGD(3, true);
            Assert.AreEqual(4, rows.Count);

            rows = db.SelectWhereGD(2, false);
            Assert.AreEqual(14, rows.Count);

            rows = db.SelectWhereGD(0, false);
            Assert.AreEqual(0, rows.Count);

            db.ClearTables();
        }

        [Test]
        public void test_SelectMatchesWhereHometeam()
        {
            /*
             1. Test with teams Kärpät, Tappara. Should return 4 matches.
             2. Test with Pelicans, should return 0 matches.
             3. Test with empty list, should return null.*/
            db.FillTables();
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");

            List<Match> matches = db.SelectWhereHometeam(teams);
            Assert.AreEqual(4, matches.Count);

            teams.Clear();
            teams.Add("Pelicans");
            matches = db.SelectWhereHometeam(teams);
            Assert.AreEqual(0, matches.Count);
            Assert.IsNotNull(matches);

            teams.Clear();
            matches = db.SelectWhereHometeam(teams);
            Assert.IsNull(matches);

            db.ClearTables();
        }

        [Test]
        public void test_SelectMatchesWhereAwayteam()
        {
            /*
             1. Test with teams Kärpät, Tappara. Should return 8 matches.
             2. Test with Pelicans, should return 2 matches.
             3. Test with empty list, should return null.*/
            db.FillTables();
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");

            List<Match> matches = db.SelectWhereAwayteam(teams);
            Assert.AreEqual(8, matches.Count);

            teams.Clear();
            teams.Add("Pelicans");
            matches = db.SelectWhereAwayteam(teams);
            Assert.AreEqual(2, matches.Count);
            Assert.IsNotNull(matches);

            teams.Clear();
            matches = db.SelectWhereAwayteam(teams);
            Assert.IsNull(matches);

            db.ClearTables();
        }

        [Test]
        public void test_SelectWherePlayoff()
        {
            /*
             1. Select playoff = true. Should return 5*/
            db.FillTables();
            List<Match> matches = db.SelectWherePlayoff(true);
            Assert.AreEqual(5, matches.Count);
            db.ClearTables();
        }

        [Test]
        public void SelectWhereSeason()
        {
            /*Select where season 17-18, should return 9.
             Join with playoffs, should return 2.*/
            db.FillTables();
            List<string> seasons = new List<string>();
            seasons.Add("17-18");
            List<Match> season = db.SelectFromSeasons(seasons);
            List<Match> playoffs = db.SelectWherePlayoff(true);

            List<List<Match>> join = new List<List<Match>>();

            join.Add(season);
            join.Add(playoffs);

            List<Match> results = db.Join(join);

            Assert.AreEqual(9, season.Count);
            Assert.AreEqual(2, results.Count);
            db.ClearTables();
        }

        [Test]
        public void test_GetSeasons()
        {
            /* getSeasons returns all different season from which there are matches in the database.*/
            db.FillTables();

            List<string> results = db.GetSeasons();

            Assert.AreEqual(2, results.Count);

            foreach (string s in results)
                Console.WriteLine(s);

            db.ClearTables();
        }

        [Test]
        public void test_SelectAllMatches()
        {
            List<Match> results = db.SelectAllMatches();
            Assert.AreEqual(0, results.Count);

            db.FillTables();
            results = db.SelectAllMatches();
            Assert.AreEqual(18, results.Count);

            db.ClearTables();
        }

        [Test]
        public void test_GetTeamnames()
        {
            /* getTeamnames returns all teams whose matches are in database.
             Database needs to be cleared of any existing data before first call.*/
            db.ClearTables();
            List<string> res = db.GetTeamnames();
            Assert.AreEqual(0, res.Count);

            db.FillTables();
            res = db.GetTeamnames();
            Assert.AreEqual(10, res.Count);

            db.ClearTables();
        }
    }
}