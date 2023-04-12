using Microsoft.EntityFrameworkCore;
using System;

namespace Shop.DAL.Entities
{
    [Keyless]
    public class EmployeeFilterStoredProcResult
    {
        public string LName { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public int TotalRowsCount { get; set; }
        public string ManagerName { get; set; }
        public string ManagerLName { get; set; }
    }
}
