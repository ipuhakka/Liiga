using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using LiigaAPI.Models;
using Newtonsoft.Json;

namespace LiigaAPI.Controllers
{
    public class MatchesController : ApiController
    {
        // GET: api/matches?parameter1=value1&param2=value2..
        public HttpResponseMessage Get(bool between, [FromUri]List<string> seasons, [FromUri]List<string> teams, int? goal_difference = null, bool? GD_is_at_least = null, bool? playoff = null, bool? played_at_home = null, bool? match_end_in_overtime = null)
        {
            MatchesModels model = new MatchesModels();
            string json = JsonConvert.SerializeObject(model.getmatches(between, seasons, teams, goal_difference, GD_is_at_least, playoff, played_at_home, match_end_in_overtime));
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(json, System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
            return response;
        }

        // GET: api/Matches/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Matches
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Matches/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Matches/5
        public void Delete(int id)
        {
        }
    }
}
