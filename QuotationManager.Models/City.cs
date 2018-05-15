using System;
using System.ComponentModel.DataAnnotations;
using QuotationManager.Models.Base;

namespace QuotationManager.Models
{
    public class City : NamedObject
    {
        [Required]
        [Range(1, 10)]
        public int SignificanceLevel { get; set; }
    }
}
