using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.DAL
{
    public interface IEmployeeRep
    {
        IList<EmployeeModel> GetAll();
        int Insert(EmployeeModel emp);
        IList<EmployeeModel> Insert(IEnumerable<EmployeeModel> employees);

        IList<EmployeeModel> ImportXml(string xml);

        void Update(EmployeeModel emp);
        void Delete(int id);
        EmployeeModel GetById(int id);

        IList<EmployeeModel> Filter(
            string Name, string LName, DateTime? birthDate,
            out int totalRows, 
            int page = 1, int pageSize = 10,
            string sortColumn = "EmployeeId", bool sortDesc = false);

    }
}
