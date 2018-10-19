using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LZA.Models
{
    public static class Settings
    {
        private static int minValue = 0;
        public static int MinValue
        {
            get
            {
                if (minValue == 0)
                {
                    minValue = Int32.Parse(ConfigurationManager.AppSettings["MinProbability"].ToString());
                }
                return minValue;
            }
        } 
    }
}