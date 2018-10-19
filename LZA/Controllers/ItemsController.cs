using LZA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            rep.Update(item);
        }
    }
}
