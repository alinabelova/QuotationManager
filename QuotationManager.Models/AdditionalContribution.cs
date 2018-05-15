using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QuotationManager.Models.Base;

namespace QuotationManager.Models
{
    public class AdditionalContribution : EntityBase
    {
        [Required]
        public int QuotaId { get; set; }

        public virtual Quota Quota { get; set; }

        [Required]
        public int ContributionId { get; set; }

        public virtual Contribution Contribution { get; set; }

        public decimal Amount { get; set; }
    }
}
