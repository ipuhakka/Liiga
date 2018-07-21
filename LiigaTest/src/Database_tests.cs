using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Liiga;

namespace LiigaTest
{
    [TestFixture]
    public class Database_tests
    {
        public Database db = new Database();

        [OneTimeSetUp]
        public void setUpTests()
        {
            /* setUp class sets Database variables and sets the path to the correct directory. */
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Liiga"));
            db.CreateDatabase(@"db\liigaTest.db");
            db.setConnectionString("Data Source=db\\liigaTest.db; Version=3;");
            db.set_db_schema("db\\liiga_schema_dump.sql");
            db.set_db_testdata("db\\liiga_testdata_dump.sql");
            db.CreateTables();
            db.FillTables();
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            /* Deletes the created test database. */
            db.ClearTables();
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
            result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09", true, "18-19");
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void test_CreateMatch_existing()
        {
            /* Creates a row to matches table witch already exists. Should return -1*/
            db.ClearTables();
            int result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09", true, "18-19");
            Assert.AreEqual(1, result);
            result = db.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09", true, "18-19");
            Assert.AreEqual(-1, result);
            db.ClearTables();
            db.FillTables();
        }

        [Test]
        public void test_CreateMatchRow_Wrong_ConnectionString()
        {
            /* by setting the connection string to a non existing one*/
            Database database = new Database();
            database.setConnectionString(@"Data Source=C:\Users\iirop\Documents\Visual Studio 2015\Projects\Liiga\Liiga\db\liigaTest2.db;Version=3");
            Assert.Throws<System.Data.SQLite.SQLiteException>(() => database.CreateMatchRow("Kärpät", "Tappara", 3, 1, false, "2018-03-09", true, "18-19"));

            if (System.IO.File.Exists(@"C:\Users\iirop\Documents\Visual Studio 2015\Projects\Liiga\Liiga\db\liigaTest2.db"))
                File.Delete(@"C:\Users\iirop\Documents\Visual Studio 2015\Projects\Liiga\Liiga\db\liigaTest2.db");
        }

        [Test]
        public void test_SelectNLastFromTeam()
        {
            MatchQuery mq = new MatchQuery();

            mq.addSubQuery(db.SelectNLastFromTeam(2, "Kärpät"));
            List<Match> matches = db.QueryMatches(mq.getQueryString());

            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual("2017-03-26", matches[0].date);
            Assert.AreEqual("2017-03-25", matches[1].date);
        }

        [Test]
        public void test_SelectNLastMatches_return_null()
        {
            /*if n is at most 0, should return null.*/
            string s = db.SelectNLastFromTeam(0, "TPS");
            Assert.IsNull(s);
        }

        [Test]
        public void test_SelectBetweenTeams()
        {
            /* Test with test data , kärpät, tps and tappara. Function should
             * returns all rows which contain only teams from parameter list (=6). */
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("TPS");

            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectBetweenTeams(teams));
            List<Match> rows = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(6, rows.Count);

            teams.Clear();
            teams.Add("TPS");
            teams.Add("TPS");
            teams.Add("TPS");

            mq.clearSubQueries();
            mq.addSubQuery(db.SelectBetweenTeams(teams));
            rows = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(0, rows.Count);
        }

        [Test]
        public void test_SelectBetweenTeams_return_null()
        {
            List<string> teams = new List<string>();
            Assert.IsNull(db.SelectBetweenTeams(teams));
        }

        [Test]
        public void test_SelectFromTeams()
        {
            /* Test with test data , kärpät, tps and tappara. Function should
             * returns all rows which contain teams from parameter list  (=6). 
               Add SaiPa to list, should return 12 matches.*/
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            teams.Add("Tappara");
            teams.Add("TPS");

            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectFromTeams(teams));
            List<Match> rows = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(12, rows.Count);
        }

        [Test]
        public void test_SelectFromTeams_return_null()
        {
            /* Test with test data containing names not in the database.        
               Clear tables at the end. Second SelectMatchesFromTeams 
               gets no teams as parameters and it should return null. */
            List<string> teams = new List<string>();

            teams.Clear();
            Assert.IsNull(db.SelectFromTeams(teams));
        }

        [Test]
        public void test_SelectFromSeasons()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectFromSeasons(new List<string>() { "17-18" }));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(9, matches.Count);
        }

        [Test]
        public void test_SelectFromSeasons_return_null()
        {
            /*empty list parameters should return null.*/
            Assert.IsNull(db.SelectFromSeasons(new List<string>()));
        }

        [Test]
        public void test_SelectBetweenTeamsFromSeasons()
        {
            MatchQuery mq = new MatchQuery();
            List<string> teams = new List<string>() { "Kärpät", "Tappara" };
            List<string> seasons = new List<string>() { "16-17" };
            mq.addSubQuery(db.SelectBetweenTeamsFromSeasons(teams, seasons));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(2, matches.Count);

            seasons.RemoveAt(0);
            mq.clearSubQueries();
            mq.addSubQuery(db.SelectBetweenTeamsFromSeasons(teams, seasons));
            matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(4, matches.Count);
        }

        [Test]
        public void test_SelectBetweenTeamsFromSeasons_return_null()
        {
            /*only when both teams and seasons parameters are 0 length, should function
             return null.*/
            List<string> teams = new List<string>() { "Kärpät" };
            List<string> seasons = new List<string>() { "16-17" };

            Assert.IsNotNull(db.SelectBetweenTeamsFromSeasons(teams, seasons));
            seasons.RemoveAt(0);
            Assert.IsNotNull(db.SelectBetweenTeamsFromSeasons(teams, seasons));
            teams.RemoveAt(0);
            Assert.IsNull(db.SelectBetweenTeamsFromSeasons(teams, seasons));
        }

        [Test]
        public void test_SelectBeforeOrAfterDate_throw_DateConversionerror()
        {
            /*Date needs to be convertable into format yyyy-MM-dd.*/
            List<string> teams = new List<string>();
            teams.Add("Kärpät");
            Assert.Throws<DateConversionError>(() => db.SelectBeforeOrAfterDate(teams, "03031998", true));
            Assert.Throws<DateConversionError>(() => db.SelectBeforeOrAfterDate(teams, "19992-03-03", true));
            Assert.DoesNotThrow(() => db.SelectBeforeOrAfterDate("2007-12-30", true));
        }

        [Test]
        public void test_SelectBeforeOrAfterDate_return_null()
        {
            Assert.IsNull(db.SelectBeforeOrAfterDate(new List<string>(), "03-03-1998", true));
        }

        [Test]
        public void test_SelectBeforeOrAfterDate_teamOverload()
        {
            /* 1. Test with incorrect date format, should throw DateConversionError
               2. Test with a list of Kärpät, Kärpät after date '2017/03/25'. 
                    Should return 2 matches.
               3. Test with list of Kärpät, Kärpät, TPS after date '2017-03-25. 
                    Should return 3 matches.'*/
            List<string> teams = new List<string>() { "Kärpät", "Kärpät" };
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectBeforeOrAfterDate(teams, "2017/03/25", true));
            List<Match> rows = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(2, rows.Count);

            teams.Add("TPS");
            mq.clearSubQueries();
            mq.addSubQuery(db.SelectBeforeOrAfterDate(teams, "2017-03-25", true));
            rows = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(3, rows.Count);
        }

        [Test]
        public void test_SelectBeforeOrAfterDate()
        {
            /* 
              1. Select * before the date "2017-03-25". Should return 13 matches.
              2. Select * after date "2016-03-26". Should return 14 matches.
              3. Join should return 9 matches.
              4. Union should return 18 matches.
              5. & 6:Test with date "20933-13-13". Should throw DateConversionError.
              */
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectBeforeOrAfterDate("2017-03-25", false));
            List<Match> before = db.QueryMatches(mq.getQueryString());

            mq.addSubQuery(db.SelectBeforeOrAfterDate("2016-03-26", true));
            List<Match> join = db.QueryMatches(mq.getQueryString());

            Assert.AreEqual(13, before.Count);
            Assert.AreEqual(9, join.Count);
        }

        [Test]
        public void test_SelectWhereOvertime_true()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereOvertime(true));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(8, matches.Count);
        }

        [Test]
        public void test_SelectWhereOvertime_false()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereOvertime(false));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(10, matches.Count);
        }

        [Test]
        public void test_SelectWhereGD_isAtLeast_true()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereGD(3, true));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(4, matches.Count);
        }

        [Test]
        public void test_SelectWhereGD_isAtLeast_false()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereGD(2, false));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(14, matches.Count);
        }

        [Test]
        public void test_SelectWhereGD_GD_0()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereGD(0, false));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(0, matches.Count);
            mq.clearSubQueries();
            mq.addSubQuery(db.SelectWhereGD(0, true));
            matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(18, matches.Count);
        }

        [Test]
        public void test_SelectWhereGD_GD_negative()
        {
            /*negative goal difference is changed to 0, so test results should be equal 
             * to test_SelectWhereGD_GD_0().*/
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereGD(-1, false));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(0, matches.Count);
            mq.clearSubQueries();
            mq.addSubQuery(db.SelectWhereGD(-2, true));
            matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(18, matches.Count);
        }

        [Test]
        public void test_SelectWhereHometeam()
        {
            MatchQuery mq = new MatchQuery();
            List<string> teams = new List<string>() { "Kärpät", "asdf" };
            mq.addSubQuery(db.SelectWhereHometeam(teams));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(2, matches.Count);
        }

        [Test]
        public void test_SelectWhereHometeam_return_null()
        {
            Assert.IsNull(db.SelectWhereHometeam(new List<string>()));
        }

        [Test]
        public void test_SelectWhereAwayteam()
        {
            MatchQuery mq = new MatchQuery();
            List<string> teams = new List<string>() { "Kärpät", "asdf" };
            mq.addSubQuery(db.SelectWhereAwayteam(teams));
            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(4, matches.Count);
        }

        [Test]
        public void test_SelectWhereAwayteam_return_null()
        {
            Assert.IsNull(db.SelectWhereAwayteam(new List<string>()));
        }

        [Test]
        public void test_SelectWherePlayoff_true()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWherePlayoff(true));
            List<Match> playoff = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(5, playoff.Count);
        }

        [Test]
        public void test_SelectWherePlayoff_false()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWherePlayoff(false));
            List<Match> regularSeason = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(13, regularSeason.Count);
        }

        [Test]
        public void test_GetSeasons()
        {
            Assert.AreEqual(2, db.GetSeasons().Count);
        }

        [Test]
        public void test_GetTeamNames()
        {
            Assert.AreEqual(10, db.GetTeamnames().Count);
        }

        [Test]
        public void test_SelectAllMatches()
        {
            MatchQuery mq = new MatchQuery();
            Assert.DoesNotThrow(() => db.QueryMatches(mq.getQueryString()));
        }

        [Test]
        public void test_QueryMatches_GD_atLeast2_overtime()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(db.SelectWhereGD(2, true));
            mq.addSubQuery(db.SelectWhereOvertime(true));
            System.Console.WriteLine(mq.getQueryString());

            List<Match> matches = db.QueryMatches(mq.getQueryString());
            Assert.AreEqual(0, matches.Count);
        }
    }
}