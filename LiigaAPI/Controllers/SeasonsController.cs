using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Cors;
using System.Net.Http;
using System.Web.Http;
using LiigaAPI.Models;

namespace LiigaAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SeasonsController : ApiController
    {
        // GET: api/Seasons
        public HttpResponseMessage Get()
        {
            /* Get json representation of list of seasons, create response with OK status, 
             set content and return response.*/
            SeasonModels seasons = new SeasonModels();

            string json = seasons.getSeasons();
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.GetEncoding("iso-8859-1"),"application/json");
            return response;
        }

        // GET: api/Seasons/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Seasons
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Seasons/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Seasons/5
        public void Delete(int id)
        {
        }
    }
}
