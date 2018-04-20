using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;


namespace API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TeamsController : ApiController
    {
        // GET: api/Teams
        public HttpResponseMessage Get()
        {
            TeamsModels obj = new TeamsModels();
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(obj.getTeams(), System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
                return response;
            }
            catch (APIError e)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(JsonConvert.SerializeObject(e.errormessage));
                return response;
            }
        }
    }
}

