using System;
using System.Collections.Generic;
using System.Text;

namespace QuotationManager.Models.Config
{
    public class MainConfig
    {
        public string ConnectionString { get; set; }
        public string AppVersion { get; set; }
        public bool LogApiRequest { get; set; }
    }
}
