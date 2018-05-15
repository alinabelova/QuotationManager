using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuotationManager.Models.Base
{
    public abstract class NamedObject : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
