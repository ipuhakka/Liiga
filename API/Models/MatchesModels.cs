using System.Collections.Generic;
using System.IO;
using Liiga;
using System;

namespace API
{
    public class MatchesModels
    {

        /// <summary>
        /// Performs queries based on the parameters, returns a list of matches that fit the query.
        /// </summary>
        /// <param name="between">When true, only matches between parameter teams are searched for.
        /// When teams has no items it is ignored.</param>
        /// <param name="gd_is_at_least">When true, only matches which have a goal difference of at least goal_difference are searched for, when false matches which have a goal difference of at most 
        /// goal_difference are searched for. When null goal difference is not a search parameter.</param>
        /// <param name="goal_difference">Integer indicating what is the limit of goal_difference between home- 
        /// and awayteam that is used in the search.</param>
        /// <param name="match_end_in_overtime">true = Only matches that ended in overtime
        /// are returned. false = only matches that ended in regular time are returned. When set to null match ending
        /// is not a search parameter.</param>
        /// <param name="played_at_home">When true, only home matches are returned from parameter teams. When false, only away matches are returned from
        /// parameter teams. When null, both home and away matches are returned.</param>
        /// <param name="playoff">When true, only playoff mathches are returned. When false, only 
        /// regular season matches are returned. When null, all match types are returned.</param>
        /// <param name="seasons">A list of seasons where the matches are searched for. When no seasons are given, 
        /// matches are searched from all the seasons.</param>
        /// <param name="teams">List of team names whose matches are searched for. When no teams are given
        /// all teams are considered in the search.</param>
        /// <returns>A list of matches that  where in all of the queries.</returns>
        public List<Match> getmatches(bool between, List<string> seasons, List<string> teams, int? goal_difference = null, bool? gd_is_at_least = null, bool? playoff = null, bool? played_at_home = null, bool? match_end_in_overtime = null)
        {
            /*
             1.Get the path to database from file filePath.txt
             2.Set connectionString
             3.Set seasons and teams null if they have no items.
             4.Create the joined list which will include all separate query results.
             5. Add basequery to join
             6. Add miniqueries to join.
             7.Return db.Join(join)*/
            string path = File.ReadAllText(@"Application Files\filePath.txt");
            string pathCombined = Path.Combine(path, "db\\liiga.db");
            if (File.Exists(pathCombined))
            {
                Database db = new Database();
                db.setConnectionString(String.Format("Data Source = {0}; Version = 3;", pathCombined));

                if (seasons.Count == 0)
                    seasons = null;

                if (teams.Count == 0)
                    teams = null;

                List<List<Match>> join = new List<List<Match>>();

                if (teams == null && seasons == null) //basequery
                {
                    join.Add(db.SelectAllMatches());
                }
                else if (teams == null && seasons != null)
                {
                    join.Add(db.SelectFromSeasons(seasons));
                }
                else if (teams != null && seasons == null && !between)
                {
                    join.Add(db.SelectFromTeams(teams));
                }
                else if (teams != null && seasons == null && between)
                {
                    join.Add(db.SelectBetweenTeams(teams));
                }
                else if (teams != null && seasons != null && between)
                {
                    join.Add(db.SelectBetweenTeamsFromSeasons(teams, seasons));
                }
                else if (teams != null && seasons != null && !between)
                {
                    join.Add(db.SelectFromTeams(teams));
                    join.Add(db.SelectFromSeasons(seasons));
                } //basequery ends 

                if (gd_is_at_least != null && goal_difference != null)
                {
                    join.Add(db.SelectWhereGD((int)goal_difference, (bool)gd_is_at_least));
                }

                if (playoff != null)
                {
                    join.Add(db.SelectWherePlayoff((bool)playoff));
                }

                if (played_at_home != null)
                {
                    if ((bool)played_at_home)
                    {
                        join.Add(db.SelectWhereHometeam(teams));
                    }
                    if ((bool)!played_at_home)
                    {
                        join.Add(db.SelectWhereAwayteam(teams));
                    }
                }
                if (match_end_in_overtime != null)
                {
                    join.Add(db.SelectWhereOvertime((bool)match_end_in_overtime));
                }
                return db.Join(join);
            }
            else
                throw new APIError("File " + path + " not found");
        }

    }
}
