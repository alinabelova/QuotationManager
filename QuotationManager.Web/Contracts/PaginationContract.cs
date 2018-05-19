using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuotationManager.Web.Contracts
{
    public class PaginationContract
    {
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public string SortBy { get; set; }
        public bool Descending { get; set; }
        public int Page { get; set; }
        public int RowsPerPage { get; set; }
    }
}
