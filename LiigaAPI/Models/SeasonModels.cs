using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Liiga;
using Newtonsoft.Json;
using System.Reflection;

namespace LiigaAPI.Models
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
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/Files/filePath.txt");
            Directory.SetCurrentDirectory(File.ReadAllText(path));
            Database db = new Database();
            db.setConnectionString("Data Source = db\\liiga.db; Version = 3;");

            Console.WriteLine(Directory.GetCurrentDirectory());

            List<string> seasons = db.GetSeasons();

            return JsonConvert.SerializeObject(seasons);
        }

    }
}