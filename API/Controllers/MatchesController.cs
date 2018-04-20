using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;

namespace API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MatchesController : ApiController
    {
        // GET: api/matches?parameter1=value1&param2=value2..
        public HttpResponseMessage Get(bool between, [FromUri]List<string> seasons, [FromUri]List<string> teams, int? goal_difference = null, bool? GD_is_at_least = null, bool? playoff = null, bool? played_at_home = null, bool? match_end_in_overtime = null)
        {
            MatchesModels model = new MatchesModels();

            try
            {
                string json = JsonConvert.SerializeObject(model.getmatches(between, seasons, teams, goal_difference, GD_is_at_least, playoff, played_at_home, match_end_in_overtime));
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(json, System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
                return response;
            }
            catch (APIError e) {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(JsonConvert.SerializeObject(e.errormessage));
                return response;
            }
        }

    }
}

