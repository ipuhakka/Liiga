using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace API
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = new HttpSelfHostConfiguration("http://localhost:3000");

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on localhost:3000, press Enter to quit.");
                Console.ReadLine();
            }

        }
    }
}
