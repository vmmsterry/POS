using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Utils
{
    public class CountriesInfo
    {

        public Dictionary<string, string[]> countries = new Dictionary<string, string[]>();
        
        public CountriesInfo()
        {
            //United States
            RegionInfo regionInfo = new RegionInfo("US");
            countries.Add("en-US", new string[] { "US", regionInfo.CurrencySymbol });
            //Mexico
            regionInfo = new RegionInfo("MX");
            countries.Add("es-MX", new string[] { "MX", regionInfo.CurrencySymbol });
        }
    }
}
