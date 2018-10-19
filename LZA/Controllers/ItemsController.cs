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

            vision.predictions = vision.predictions.Where(x => x.probability > Settings.MinValue).ToArray();

            item.json = JsonConvert.SerializeObject(vision);

            rep.Update(item);
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.All.send("LZA", item);
        }
    }
}
