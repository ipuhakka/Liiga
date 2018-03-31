using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Liiga;

namespace LiigaAPI.Models
{
    public class TeamsModels
    {

        public string getTeams()
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/Files/filePath.txt");
            Directory.SetCurrentDirectory(File.ReadAllText(path));
            Database db = new Database();
            db.setConnectionString("Data Source = db\\liiga.db; Version = 3;");

            return JsonConvert.SerializeObject(db.GetTeamnames());
        }

    }
}