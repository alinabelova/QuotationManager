using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuotationManager.Models.Enums
{
    public enum RefinancingTarget
    {
        [Display(Name = "Ипотека")]
        Mortgage = 0,

        [Display(Name = "Потребительский кредит")]
        ConsumerLoan = 1,

        [Display(Name = "Автокредит")]
        CarLoan = 2,

    }
}
