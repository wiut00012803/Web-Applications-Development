using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class EmployeeFilterViewModel
    {
        public IList<EmployeeModel> Employees;
        public int TotalRows { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public string SortColumn { get; set; } = "EmployeeId";
        public bool SortDesc { get; set; } = false;

        public string Name { get; set; }
        public string LName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
