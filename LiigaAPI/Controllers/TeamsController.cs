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
    public class TeamsController : ApiController
    {
        // GET: api/Teams
        public HttpResponseMessage Get()
        {
            TeamsModels obj = new TeamsModels();

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(obj.getTeams(), System.Text.Encoding.GetEncoding("iso-8859-1"), "application/json");
            return response;
        }

        // GET: api/Teams/5
       /* public string Get(int id)
        {
            return "value";
        }

        // POST: api/Teams
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Teams/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Teams/5
        public void Delete(int id)
        {
        } */
    }
}
