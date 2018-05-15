using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QuotationManager.Models.Base;

namespace QuotationManager.Models
{
    public class Contribution : NamedObject
    {
        [Required]
        [Range(0, 10)]
        public int BaseAmount { get; set; }

        [Required]
        public int CityId { get; set; }

        public virtual City City { get; set; }
    }
}
