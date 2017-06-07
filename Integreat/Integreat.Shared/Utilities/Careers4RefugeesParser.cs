﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Integreat.Shared.Models.C4R;
using Xamarin.Forms;

namespace Integreat.Shared.Utilities
{
    public  class Careers4RefugeesParser
    {
        //Constructor to initialize Parser
        public Careers4RefugeesParser() { }


        public Task<List<CareerOffer>> GetCareerList()
        {
            var offers = XmlWebParser.ParseXmlFromAddressAsync<List<CareerOffer>>("http://www.careers4refugees.de/jobsearch/exports/integreat_regensburg", "anzeigen");
            return offers;
        }
    }
}

