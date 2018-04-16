using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
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
            Console.WriteLine("get seasons");
            System.Diagnostics.Debug.WriteLine("get seasons");
            string json = seasons.getSeasons();
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(json, System.Text.Encoding.GetEncoding("iso-8859-1"),"application/json");
            return response;
        }

    }
}
