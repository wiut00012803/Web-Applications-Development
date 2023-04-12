using System;

namespace Shop.Models
{
    public class EmployeeModel
    {
        public int? EmployeeId { get; set; }
        public string Name { get; set; }
        public string LName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int TotalRowsCount { get; set; } = 1;
    }
}
