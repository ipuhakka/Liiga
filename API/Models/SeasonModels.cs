using System.Collections.Generic;
using System.IO;
using Liiga;
using Newtonsoft.Json;
using System;

namespace API
{
    public class SeasonModels
    {
        /// <summary>
        /// gets all the seasons that have matches in the database, turns it into JSON and returns 
        /// the result string.
        /// </summary>
        /// <returns>Serialized json-object containing all seasons in the database.</returns>
        public string getSeasons()
        {
            /*set directory to correct folder, set connectionString and getSeasons.*/
            string path = File.ReadAllText(@"Application Files\filePath.txt");
            string pathCombined = Path.Combine(path, "db\\liiga.db");
            if (File.Exists(pathCombined))
            {
                Database db = new Database();
                db.setConnectionString(String.Format("Data Source = {0}; Version = 3;", pathCombined));

                List<string> seasons = db.GetSeasons();

                return JsonConvert.SerializeObject(seasons);
            }
            else
                throw new APIError("File " + path + " not found");
        }

    }
}