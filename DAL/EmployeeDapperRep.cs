using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.DAL
{
    public class EmployeeDapperRep : IEmployeeRep
    {
        private const string SQL_GET_ALL = @"select EmployeeId, Name, LName, BirthDate
                                    from Employee";
        private const string SQL_INSERT = @"insert into Employee(Name, LName, BirthDate)
                                values(@Name, @LName, @BirthDate)
                                select SCOPE_IDENTITY()";
        private const string SQL_GET_BY_ID = @"select EmployeeId, Name, LName, BirthDate
                                from Employee
                                where EmployeeId = @EmployeeId";
        private const string SQL_UPDATE = @"update Employee set
                                              Name = @Name, 
                                              LName  = @LName, 
                                              BirthDate  = @BirthDate
                                            where EmployeeId = @EmployeeId";
        private const string SQL_DELETE = @"delete from Employee where EmployeeId = @EmployeeId";
        private const string SQL_FILTER = @"select Employeeid, Name, LName, BirthDate,
	                                            count(*) over() TotalRowsCount
                                            from Employee
                                            where Name like coalesce(@Name, '') + '%'
	                                            and LName like coalesce(@LName, '') + '%'
	                                            and BirthDate >= coalesce(@BirthDate, '1900-01-01')
                                            order by {0}
                                            offset @OffsetRows rows
                                            fetch next @PageSize rows only";

        private readonly string _connString;

        public EmployeeDapperRep(string connString)
        {
            _connString = connString;
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(_connString);
            conn.Execute(SQL_DELETE, new { EmployeeId = id });
        }

        public IList<EmployeeModel> Filter(
            string Name, string LName, DateTime? birthDate,
            out int totalRows, int page = 1, int pageSize = 10, string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            if (page <= 0)
                page = 1;

            var sort = "EmployeeId";
            if ("EmployeeId".Equals(sortColumn))
                sort = "EmployeeId";
            else if ("Name".Equals(sortColumn))
                    sort = "Name";

            if (sortDesc)
                sort += " DESC ";

            string sql = string.Format(SQL_FILTER, sort);

            using var conn = new SqlConnection(_connString);
            var employees = conn.Query<EmployeeModel>(
                sql,
                new
                {
                    Name = Name,
                    LName = LName,
                    BirthDate = birthDate,
                    OffsetRows = (page - 1)* pageSize,
                    PageSize = pageSize
                }).AsList();

            totalRows = employees.FirstOrDefault()?.TotalRowsCount ?? 0;

            return employees;
        }

        public IList<EmployeeModel> GetAll()
        {
            using var conn = new SqlConnection(_connString);
            return conn.Query<EmployeeModel>(SQL_GET_ALL).AsList();
        }

        public EmployeeModel GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return conn.QueryFirst<EmployeeModel>(SQL_GET_BY_ID, new { Id = id});
        }

        public IList<EmployeeModel> ImportXml(string xml)
        {
            using var conn = new SqlConnection(_connString);
            return conn.Query<EmployeeModel>("dbsdImportEmployeesFromXml",
                commandType: System.Data.CommandType.StoredProcedure,
                param: new { xml = xml }).AsList();
        }

        public int Insert(EmployeeModel emp)
        {
            using var conn = new SqlConnection(_connString);
            return conn.ExecuteScalar<int>(SQL_INSERT, emp);
        }

        public IList<EmployeeModel> Insert(IEnumerable<EmployeeModel> employees)
        {
            SqlMetaData[] recordDefinition =
            {
                new SqlMetaData("Name", System.Data.SqlDbType.NVarChar, 20),
                new SqlMetaData("LName", System.Data.SqlDbType.NVarChar, 20),
                new SqlMetaData("BirthDate", System.Data.SqlDbType.DateTime)
            };
            var records = new List<SqlDataRecord>();
            foreach(var emp in employees)
            {
                var r = new SqlDataRecord(recordDefinition);
                r.SetString(0, emp.Name);
                r.SetString(1, emp.LName);
                if (emp.BirthDate.HasValue)
                    r.SetDateTime(2, emp.BirthDate.Value);
                else
                    r.SetDBNull(2);

                records.Add(r);
            }

            using var conn = new SqlConnection(_connString);
            return conn.Query<EmployeeModel>(
                "dbsdBulkInsertEmployees",
                commandType: System.Data.CommandType.StoredProcedure,
                param: new { EmpTable = records.AsTableValuedParameter() }
                ).AsList();
        }

        public void Update(EmployeeModel emp)
        {
            using var conn = new SqlConnection(_connString);
            conn.Execute(SQL_UPDATE, emp);
        }
    }
}
