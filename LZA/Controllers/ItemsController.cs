using LZA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using LZA.Hubs;
namespace LZA.Controllers
{
    public class ItemsController : ApiController
    {
        [HttpPost]
        [Route("items")]
        public void Items(Item item)
        {
            var rep = new GenericRepository<Item>(new ConnectionFactory());
            item.id = 1;

            var vision = JsonConvert.DeserializeObject<VisionResponse>(item.json);

            vision.predictions = vision.predictions.Where(x => x.probability*100 > Settings.MinValue).ToArray();

            item.json = JsonConvert.SerializeObject(vision);

            rep.Update(item);

            var connection = new HubConnection("http://localhost:54588/");

            var myhub = connection.CreateHubProxy("ChatHub");
            connection.Start().Wait();
            
            myhub.Invoke<string>("Send", "LZA", item.json);
            
        }
    }
}
