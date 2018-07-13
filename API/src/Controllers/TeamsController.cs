using System;
using System.Diagnostics;
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TeamsModels obj = new TeamsModels();
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(obj.getTeams(), System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
                sw.Stop();
                Console.WriteLine("Get teams, 200 OK, took " + sw.ElapsedMilliseconds + "ms to handle");
                return response;
            }
            catch (APIError e)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(JsonConvert.SerializeObject(e.errormessage));
                sw.Stop();
                Console.WriteLine("Get seasons, 500 Internal Server Error, took " + sw.ElapsedMilliseconds + "ms to handle");
                return response;
            }
        }
    }
}

