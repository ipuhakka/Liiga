using System.IO;
using Newtonsoft.Json;
using Liiga;
using System;

namespace API
{
    public class TeamsModels
    {

        public string getTeams()
        {
            string path = File.ReadAllText(@"Application Files\filePath.txt");
            string pathCombined = Path.Combine(path, "db\\liiga.db");
            if (File.Exists(pathCombined))
            {
                Database db = new Database();
                db.setConnectionString(String.Format("Data Source = {0}; Version = 3;", pathCombined));

                return JsonConvert.SerializeObject(db.GetTeamnames());
            }
            else
                throw new APIError("File " + path + " not found");
        }

    }
}