using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Liiga
{
    public class Database
    {
        private string db_schema;
        private string db_testdata;
        private string connectionString;

        public void set_db_schema(string path) { db_schema = path; }
        public void set_db_testdata(string path) { db_testdata = path; }
        public void setConnectionString(string con) { connectionString = con; }

        public List<Match> QueryMatches(string query)
        {
            /*
                Implements a query to the database. Open connection, perform query, 
                create a list of Match objects and close connection.

                param string query: search query which is performed.

                return: List<Match> result: contains Match objects created from rows in the answer
                to query.
             */

            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Match> result = CreateMatchList(reader);

            con.Close();

            return result;
        }

        public void Query(string query)
        {
            /* makes queries which are not expected to return anything.
             
             param string query: query to be made to the database.*/
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.ExecuteNonQuery();
            con.Close();

        }

        /// <summary>Deletes a file given as the parameter.</summary>
        /// <param name="path">path to file to be deleted</param>
        /// <returns>-1 if file wasn't deleted, 1 on success.</returns>
        public int DeleteDatabase(string path)
        {
            /* Deletes a file. As SQLiteConnection.Close() might not close the connection properly
             * we need to wait for garbagecollector to close database connections before deleting
             * the file.*/
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(path))
                File.Delete(path);
            else
                return -1;

            return 1;
        }

        ///<summary>Creates a file with parameter name.</summary>
        ///<param name="dbName">Name for the file to be created.</param>
        public void CreateDatabase(string dbName)
        {
            SQLiteConnection.CreateFile(dbName);
        }

        /// <summary>Deleted all rows from matches tables with current connectionString.</summary>
        public void ClearTables()
        {
            string sql = "DELETE FROM matches;";

            Query(sql);
        }

        /// <summary> Creates schemas for tables from script with name db_schema</summary>
        /// <exception cref="DatabaseError">Thrown when 
        /// no db_schema or connectionString is set.</exception>
        /// <returns>1 on success.</returns>
        public int CreateTables()
        {

            if (db_schema == null || connectionString == null)
                throw new DatabaseError();

            string sql = File.ReadAllText(db_schema);

            Query(sql);
            Console.WriteLine("Created tables");

            return 1;
        }

        /// <summary>
        /// Fills tables with test data.
        /// </summary>
        /// <param name="db_testdata">File which is used to populate the database.</param>
        public void FillTables()
        {
            /* fills tables with test data.*/
            string sql = File.ReadAllText(db_testdata);

            Query(sql);
        }

        /// <summary>Select n last matches from a team.</summary>
        /// <param name="n">Number of last matches to get.</param>
        /// <param name="team">Name of the team to be searched for.</param>
        /// <returns>string query to get n last matches from a team, null if n is less than or equal to 0.</returns>
        public string SelectNLastFromTeam(int n, string team)
        {
            if (n <= 0)
                return null;

            string end = String.Format("ORDER BY played_date DESC LIMIT {0}", n);
            string conditions = String.Format("hometeam='{0}' OR awayteam='{0}' ", team);

            return conditions + end;
        }

        /// <summary>Selects all found matches between parameter teams</summary>
        /// <param name="teams">A list of team names that are looked for.</param>
        /// <returns>string query for selecting matches between teams, if no teams provided, returns null.</returns>
        public string SelectBetweenTeams(List<string> teams)
        {
            /*creates a query for for finding all matches that are played between teams in parameter list.
             Connects to database and makes the query. */

            if (teams.Count == 0)
                return null;

            string homequery = "(";
            string awayquery = "(";

            for (int i = 0; i < teams.Count; i++)
            {
                homequery = homequery + String.Format("hometeam='{0}'", teams[i]);
                awayquery = awayquery + String.Format("awayteam='{0}'", teams[i]);

                if (i < teams.Count - 1)
                {
                    homequery = homequery + " or ";
                    awayquery = awayquery + " or ";
                }
            }
            homequery = homequery + ")";
            awayquery = awayquery + ") ";
            return homequery + " and " + awayquery;
        }

        /// <summary>Select all matches from specified teams.</summary>
        /// <param name="teams">Names of the teams whose matches are searched for.</param>
        /// <returns>string query to select matches from teams.</returns>
        public string SelectFromTeams(List<string> teams)
        {
            /*creates a query for for finding all matches with the teams in parameter list.
             Connects to database and makes the query. */
            if (teams.Count == 0)
                return null;

            string homequery = "(";
            string awayquery = "(";

            for (int i = 0; i < teams.Count; i++)
            {
                homequery = homequery + String.Format("hometeam='{0}'", teams[i]);
                awayquery = awayquery + String.Format("awayteam='{0}'", teams[i]);

                if (i < teams.Count - 1)
                {
                    homequery = homequery + " or ";
                    awayquery = awayquery + " or ";
                }
            }
            homequery = homequery + ")";
            awayquery = awayquery + ") ";
            return homequery + " or " + awayquery;
        }

        /// <summary>Selects all matches from a certain season</summary>
        /// <param name="seasons">List of seasons where matches are searched for.</param>
        /// <returns>string query for selecting matches in selected seasons. If no seasons
        /// are provided, returns null.</returns>
        /// 
        public string SelectFromSeasons(List<string> seasons)
        {
            if (seasons.Count <= 0)
                return null;
            string query = "";

            for (int i = 0; i < seasons.Count; i++)
            {
                query = query + String.Format("season='{0}' ", seasons[i]);

                if (i < seasons.Count - 1)
                    query = query + "OR ";
            }

            return query;
        }

        /// <summary>Selects matches between parameter teams from selected seasons.</summary>
        /// <param name="seasons">A list of all seasons where matches are searched for
        /// (format YY-YY).</param>
        /// <param name="teams">A list of team names whose games are searched for.</param>
        /// <returns>Query string to select matches between teams from selected seasons. Returns null
        /// if both parameters have length less than or equal to 0.</returns>
        public string SelectBetweenTeamsFromSeasons(List<string> teams, List<string> seasons)
        {
            /* create a query selecting all matches between parameter teams
             * from the parameter seasons. */

            if (teams.Count <= 0 && seasons.Count <= 0)
                return null;
            string query = "";

            string homequery = "(";
            string awayquery = "(";

            for (int i = 0; i < teams.Count; i++)
            {
                homequery = homequery + String.Format("hometeam='{0}'", teams[i]);
                awayquery = awayquery + String.Format("awayteam='{0}'", teams[i]);

                if (i < teams.Count - 1)
                {
                    homequery = homequery + " or ";
                    awayquery = awayquery + " or ";
                }
            }

            string seasonQuery = " and (";
            for (int j = 0; j < seasons.Count; j++)
            {
                seasonQuery = seasonQuery + String.Format("season='{0}' ", seasons[j]);

                if (j < seasons.Count - 1)
                    seasonQuery = seasonQuery + "or ";
            }

            homequery = homequery + ")";
            awayquery = awayquery + ")";

            if (teams.Count > 0)
                query = query + homequery + " and " + awayquery; 
            if (seasons.Count > 0)
                query = query + seasonQuery + ") ";
            return query;
        }

        /// <summary>Select all matches before or after (and on the same) day.</summary>
        /// <param name="date"> The first/last date included in the search.</param>
        /// <param name="after">Boolean which indicates whether or not function looks for matches
        /// after or before the date.</param>
        /// <returns>string query to get matches before or after a specific date.</returns> 
        /// <exception cref="DateConversionError">Thrown when the date parameter has a
        /// format which cannot be converted into "yyyy-MM-dd".</exception>
        public string SelectBeforeOrAfterDate(string date, bool after)
        {
            /* Select all matches before or after (and on) a date. 
              * forms a query to select all matches before or after  (and the same) date. Date parameter is parsed into
             * correct format 'YYYY-MM-DD'. parsedDate variable is used to check if date is in approved format.
             */
            DateTime parsedDate;
            string query = "";

            if (DateTime.TryParse(date, out parsedDate))
            {
                date = parsedDate.ToString("yyyy-MM-dd");
            }
            else
            {
                throw new DateConversionError();
            }

            if (after)
                query = query + String.Format("played_date >='{0}' ", date);
            else
                query = query + String.Format("played_date <='{0}' ", date);

            return query;

        }

        /// <summary>Select matches before or after (and on the same) day from
        /// selected teams.</summary>
        /// <param name="after">Boolean indicating if search is done to matches on and after the day, 
        /// or on and before the date.</param>
        /// <param name="date">Date which is set as the limit of the search. Must be 
        /// convertable to format "yyyy-MM-dd".</param>
        /// <param name="teams">A list of team names whose matches are searched for.</param>
        /// <returns>query string to select matches before or after date from selected teams.</returns>
        /// <exception cref="DateConversionError">Thrown when date parameter cannot be converted into
        /// format "yyyy-MM-dd".</exception>
        public string SelectBeforeOrAfterDate(List<string> teams, string date, bool after)
        {
            /* Overload to select matches from specific teams.
             * forms a query to select all matches before or after  (and the same) date from all the teams given as a parameter. Date parameter is parsed into
             * correct format 'YYYY-MM-DD'. parsedDate variable is used to check if date is in approved format. */

            if (teams.Count <= 0)
                return null;

            DateTime parsedDate;
            string query = "(";

            if (DateTime.TryParse(date, out parsedDate))
            {
                date = parsedDate.ToString("yyyy-MM-dd");
            }
            else
            {
                throw new DateConversionError();
            }

            for (int i = 0; i < teams.Count; i++)
            {
                query = query + String.Format("hometeam='{0}' OR awayteam='{0}' ", teams[i]);

                if (i < (teams.Count - 1))
                    query = query + "OR ";

            }
            if (after)
                query = query + String.Format(") AND played_date >='{0}'", date);
            else
                query = query + String.Format(") AND played_date <='{0}'", date);

            return query;
        }

        /// <summary>Selects matches depending on wheter they went into overtime.</summary>
        /// <param name="ot">Boolean indicating whether search is for matches that went into
        /// overtime or didn't go to overtime.</param>
        /// <returns>query string for selecting matches where overtime is parameter ot.</returns>
        public string SelectWhereOvertime(bool ot)
        {
            return String.Format("overtime={0} ", Convert.ToInt32(ot));
        }

        /// <summary>Returns a list of matches where goal difference is equal to and
        /// greater or less than gd.</summary>
        /// <param name="gd">Specified goal difference.</param>
        /// <param name="isAtLeast"> Boolean indicating whether to look for
        /// games with a smaller or larger difference.</param>
        /// <returns>string query for selecting matches with selected goal difference, null if no teams are given.</returns>
        public string SelectWhereGD(int gd, bool isAtLeast)
        {
            string query = "";

            if (gd < 0)
                gd = 0;

            if (isAtLeast)
                query = String.Format("homescore >= (awayscore + {0}) OR awayscore >= (homescore + {0}) ", gd);
            else
                query = String.Format("homescore <= (awayscore + {0}) AND awayscore <= (homescore + {0}) ", gd);

            return query;
        }

        /// <summary>Selects matches from database where hometeam is one of the parameter teams.</summary>
        /// <param name="teams">List of team names that are searched for.</param>
        /// <returns>string query for selecting home matches from specific teams.</returns>
        /// 
        public string SelectWhereHometeam(List<string> teams)
        {
            if (teams.Count == 0)
                return null;

            string query = "";

            for (int i = 0; i < teams.Count; i++)
            {
                query = query + String.Format("hometeam='{0}' ", teams[i]);
                if (i < (teams.Count - 1))
                    query = query + "OR ";
            }

            return query;
        }

        /// <summary>Selects matches from database where awayteam is one of the parameter teams.</summary>
        /// <param name="teams">List of team names that are searched for.</param>
        /// <returns>query string to select away matches from selected teams, null if no teams are given.</returns>
        /// 
        public string SelectWhereAwayteam(List<string> teams)
        {
            if (teams.Count == 0)
                return null;

            string query = "";

            for (int i = 0; i < teams.Count; i++)
            {
                query = query + String.Format("awayteam='{0}' ", teams[i]);
                if (i < (teams.Count - 1))
                    query = query + "OR ";
            }

            return query;
        }

        /// <summary> Select a list of playoff or regular season matches.</summary>
        /// <param name="playoff">True indicates that function searches playoff matches, false
        /// that search searches regular season games.</param>
        /// <returns>query string for selecting matches with specific playoff parameter.</returns>
        /// 
        public string SelectWherePlayoff(bool playoff)
        {
            return String.Format("playoff={0} ", Convert.ToInt32(playoff));
        }

        /// <summary>Creates a list of match objects from query return data.</summary>
        /// <param name="reader">An SQLiteDataReader which holds data returned by a query. </param>
        /// <returns>A list of created Match-objects.</returns>
        public List<Match> CreateMatchList(SQLiteDataReader reader)
        {
            /* 
             Creates a Match list from query return data. Date needs to be parsed into correct format
             using Convert.ToDateTime.
             */

            List<Match> matches = new List<Match>();
            while (reader.Read())
            {
                Match obj = new Match(reader["hometeam"].ToString(), reader["awayteam"].ToString(), reader["season"].ToString(),
                    Convert.ToDateTime(reader["played_date"]).ToString("yyyy-MM-dd"), Convert.ToInt32(reader["homescore"].ToString()), Convert.ToInt32(reader["awayscore"].ToString()),
                    Convert.ToInt32(reader["overtime"].ToString()), Convert.ToInt32(reader["playoff"].ToString()));

                matches.Add(obj);
            }

            return matches;
        }

        /// <summary>
        /// Selects all different seasons from the database.
        /// </summary>
        /// <returns>A list of strings containing all seasons from which database
        /// has matches.</returns>
        public List<string> GetSeasons()
        {
            return QueryString("SELECT DISTINCT season FROM matches;");
        }

        /// <summary>
        /// Returns all teamnames that are in the database.
        /// </summary>
        /// <returns>A list of strings containing all names of teams in database.</returns>
        public List<string> GetTeamnames()
        {
            return QueryString("SELECT hometeam FROM matches WHERE hometeam IS NOT NULL UNION SELECT awayteam FROM matches WHERE awayteam IS NOT NULL; ");
        }

        /// <summary>
        /// Returns list of strings based on the query. Can be used  to get distinct teams and seasons.
        /// </summary>
        /// <returns>A list of strings containing all different strings in database based on 
        /// the search.</returns>
        public List<string> QueryString(string query)
        {
            List<string> results = new List<string>();

            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }

            con.Close();

            return results;
        }


        /// <summary>Creates a new row to matches table.</summary>
        /// <param name="home">Hometeam name.</param>
        /// <param name="away">Awayteam name.</param>
        /// <param name="homescore">Goals scored by the hometeam.</param>
        /// <param name="awayscore">Goals scored by the awayteam.</param>
        /// <param name="ot">Boolean indicating if the match went into overtime.</param>
        /// <param name="playoff">False = not a playoff match, true = is a playoff match</param>
        /// <param name="date">Date of the match. Needs to be convertable into format
        /// "yyyy-MM-dd".</param>
        /// <param name="season">string description of which season the match was played.</param>
        /// <returns>1 on successful creation, -1 if match already exists in the database.</returns>
        /// <exception cref="DateConversionError">Thrown when date cannot be converted.
        /// into format "yyyy-MM-dd".</exception>
        /// <exception cref="SQLiteException">Thrown when executing command throws an error.</exception>
        public int CreateMatchRow(string home, string away, int homescore, int awayscore, bool ot, string date, bool playoff, string season)
        {
            /* creates a new row to matches table. First checks if the match already exists in the database. */
            string select = String.Format("SELECT count(1) FROM matches WHERE hometeam='{0}' AND awayteam='{1}' AND played_date='{2}';"
                , home, away, date);
            int exists = 1;
            DateTime parsedDate;

            if (DateTime.TryParse(date, out parsedDate))
            {
                date = parsedDate.ToString("yyyy-MM-dd");
            }
            else
            {
                throw new DateConversionError();
            }

            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(select, con);
            try
            {
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    exists = Convert.ToInt32(reader.GetInt32(0));
                }
                if (exists == 1)
                {
                    con.Close();
                    return -1;
                }
                else
                {
                    string insert = String.Format("INSERT INTO matches VALUES('{0}', '{1}', {2}, {3}, {4}, '{5}', {6}, '{7}');",
                        home, away, homescore, awayscore, Convert.ToInt32(ot), date, Convert.ToInt32(playoff), season);

                    Query(insert);
                    return 1;
                }
            }
            catch (SQLiteException e)
            {
                con.Close();
                throw e;
            }

        }
    }
}

