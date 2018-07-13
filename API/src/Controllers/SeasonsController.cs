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
    public class SeasonsController : ApiController
    {
        // GET: api/Seasons
        public HttpResponseMessage Get()
        {
            /* Get json representation of list of seasons, create response with OK status, 
             set content and return response.*/
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                SeasonModels seasons = new SeasonModels();
                string json = seasons.getSeasons();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new StringContent(json, System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
                sw.Stop();
                Console.WriteLine("Get seasons, 200 OK, took " + sw.ElapsedMilliseconds + "ms to handle");
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
