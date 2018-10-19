using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LZA.Models
{
    [Table("lza_itemcache")]
    public class Item
    {
        [Key]
        public int id { get; set; }
        public string json { get; set; }
    }
}