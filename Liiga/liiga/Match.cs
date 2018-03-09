using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liiga
{
    public class Match: IEquatable<Match>
    {
        public string hometeam, awayteam, season, date;
        public int homescore, awayscore;
        public bool overtime, playoff;

        public Match(string homet, string awayt, string season_par, string date_par, int homes, int aways, int ot_par, int playoff_par)
        {
            hometeam = homet;
            awayteam = awayt;
            homescore = homes;
            awayscore = aways;
            season = season_par;
            date = date_par;

            if (ot_par == 0)
                overtime = false;
            else
                overtime = true;

            if (playoff_par == 0)
                playoff = false;
            else
                playoff = true;

        }

        public bool Equals(Match other)
        {
            return this.hometeam == other.hometeam && this.awayteam == other.awayteam
                && this.homescore == other.homescore && this.awayscore == other.awayscore &&
                this.date == other.date && this.season == other.season && this.overtime ==
                other.overtime && this.playoff == other.playoff;
        }

    }
}
