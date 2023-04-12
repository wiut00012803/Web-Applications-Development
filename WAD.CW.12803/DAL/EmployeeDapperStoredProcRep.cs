using Dapper;
using Microsoft.Data.SqlClient;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.DAL
{
    public class EmployeeDapperStoredProcRep : IEmployeeRep
    {
        private readonly string _connStr;

        public EmployeeDapperStoredProcRep(string connStr)
        {
            _connStr = connStr;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IList<EmployeeModel> Filter(
            string Name, 
            string LName, 
            DateTime? birthDate, 
            out int totalRows, int page = 1, 
            int pageSize = 10, 
            string sortColumn = "EmployeeId", 
            bool sortDesc = false)
        {
            using var conn = new SqlConnection(_connStr);
            var employees = conn.Query<EmployeeModel>(
                "dbsdEmployeeFilterWithSorting",
                commandType: System.Data.CommandType.StoredProcedure,
                param: new
                {
                    FN = Name,
                    LN = LName,
                    DOB = birthDate,
                    SortColumn = sortColumn,
                    SortDesc = sortDesc,
                    Page = page,
                    PageSize = pageSize
                });

            totalRows = employees.FirstOrDefault()?.TotalRowsCount ?? 0;

            return employees.AsList();
        }

        /*
        create proc DatabaseGetAllEmployees
        as
        begin
            select EmployeeId, Name, LName, BirthDate from Employee
        end
        go
        exec DatabaseGetAllEmployees
         */
        public IList<EmployeeModel> GetAll()
        {
            using var conn = new SqlConnection(_connStr);
            return conn
                .Query<EmployeeModel>("DatabaseGetAllEmployees", 
                                 commandType: System.Data.CommandType.StoredProcedure)
                .AsList();
        }

        public EmployeeModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<EmployeeModel> ImportXml(string xml)
        {
            throw new NotImplementedException();
        }

        /*
         create proc DatabaseInsertEmployee(
          @Name nvarchar(20), 
          @LName nvarchar(20), 
          @BirthDate datetime,
          @EmployeeId int OUT,
          @Errors varchar(1000) OUT
        ) as
        begin
          begin try
	          insert into Employee(Name, LName, BirthDate)
	          values(@Name, @LName, @BirthDate)
	          select @EmployeeId = SCOPE_IDENTITY()
          end try
          begin catch
            select @Errors = ERROR_MESSAGE()
            return (1)
          end catch
          return (0)
        end
        go
        declare @eid int
        declare @err varchar(1000)
        declare @retcode int
        exec @retcode = DatabaseInsertEmployee @Name = 'aaa', @LName= 'bbb', @BirthDate = '2012-12-16'
		        ,@EmployeeId = @eid OUT, @Errors = @err OUT
        print @eid
        print @err
        print @retcode
         */
        public int Insert(EmployeeModel emp)
        {
            using var conn = new SqlConnection(_connStr);
            var p = new DynamicParameters();
            p.Add("Name", emp.Name);
            p.Add("LName", emp.LName);
            p.Add("BirthDate", emp.BirthDate);
            p.Add("EmployeeId", emp.EmployeeId,
                direction: System.Data.ParameterDirection.Output, 
                dbType: System.Data.DbType.Int32);
            p.Add("Errors",
                direction: System.Data.ParameterDirection.Output,
                dbType: System.Data.DbType.String, size: 1000);
            p.Add("RetVal", direction: System.Data.ParameterDirection.ReturnValue, dbType: System.Data.DbType.Int32);

            conn.Execute(
                "DatabaseInsertEmployee",
                commandType: System.Data.CommandType.StoredProcedure,
                param: p
                );

            int retVal = p.Get<int>("RetVal");
            string errors = p.Get<string>("Errors");

            return p.Get<int>("EmployeeId");
        }

        public IList<EmployeeModel> Insert(IEnumerable<EmployeeModel> employees)
        {
            throw new NotImplementedException();
        }

        public void Update(EmployeeModel emp)
        {
            throw new NotImplementedException();
        }
    }
}
