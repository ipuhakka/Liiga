using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liiga
{
    /// <summary>
    /// Class connects multiple queries together.
    /// </summary>
    public class MatchQuery
    {
        private List<string> sub_queries;

        public MatchQuery()
        {
            sub_queries = new List<string>();
        }

        /// <summary>
        /// Clears sub_queries so that one MatchQuery object can handle multiple different queries.
        /// </summary>
        public void clearSubQueries()
        {
            sub_queries.Clear();
        }

        /// <summary>
        /// Adds a new sub query to sub_queries list.
        /// </summary>
        /// <param name="sub"></param>
        public void addSubQuery(string sub)
        {
            if (sub != null)
                sub_queries.Add(sub);
        }

        /// <summary>
        /// Creates and returns a querystring formed from all sub_queries.
        /// </summary>
        /// <returns></returns>
        public string getQueryString()
        {
            string query = "SELECT * FROM matches WHERE ";

            for (int i = 0; i < sub_queries.Count; i++)
            {
                query = query + sub_queries[i];

                if (i < sub_queries.Count - 1)
                    query = query + "AND ";
            }

            return query + ";";
        }
    }
}
