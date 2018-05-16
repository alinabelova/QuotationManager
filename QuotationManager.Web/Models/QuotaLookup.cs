using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuotationManager.Models;

namespace QuotationManager.Web.Models
{
    public class QuotaLookup
    {
        public List<City> Cities { get; set; }

        public Dictionary<string, int> RefinancingTargets { get; set; }

        public List<Contribution> Contributions { get; set; }
    }
}
