using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QuotationManager.Models.Base;
using QuotationManager.Models.Enums;

namespace QuotationManager.Models
{
    public class Quota : EntityBase
    {
        [Required]
        public int CityId { get; set; }

        public virtual City City { get; set; }

        [Required]
        public RefinancingTarget RefinancingTarget { get; set; }

        [Range(0.0, 5000000.0)]
        public decimal RefinancingAmount { get; set; }

        public List<AdditionalContribution> AdditionalContributions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //TODO
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        [StringLength(1024)]
        public string Comment { get; set; }

        //TODO
        [Required]
        [Range(0.0, 100.0)]
        public int BaseAmount { get; set; }

        [Required]
        public string ClientId { get; set; }

        public virtual ApplicationUser Client { get; set; }

    }
}
