using Microsoft.Data.SqlClient;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Shop.DAL
{
    public class EmployeeRep : IEmployeeRep
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

        private readonly string _connString;

        public EmployeeRep(string connString)
        {
            _connString = connString;
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = SQL_DELETE;
            cmd.Parameters.AddWithValue("@EmployeeId", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public IList<EmployeeModel> Filter(string Name, string LName, DateTime? birthDate, out int totalRows, int page = 1, int pageSize = 10, string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            throw new NotImplementedException();
        }

        public IList<EmployeeModel> GetAll()
        {
            var employees = new List<EmployeeModel>();

            using (var conn = new SqlConnection(_connString))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = SQL_GET_ALL;

                conn.Open();
                using var rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    var emp = MapReaderToEmployee(rdr);
                    employees.Add(emp);
                }
            }
            
            return employees;
        }

        public EmployeeModel GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = SQL_GET_BY_ID;
            cmd.Parameters.AddWithValue("@EmployeeId", id);

            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if(rdr.Read())
            {
                return MapReaderToEmployee(rdr);
            }

            return null;
        }

        public IList<EmployeeModel> ImportXml(string xml)
        {
            throw new NotImplementedException();
        }

        public int Insert(EmployeeModel emp)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = SQL_INSERT;

            var pName = cmd.CreateParameter();
            pName.ParameterName = "@Name";
            pName.Value = emp.Name;
            pName.DbType = DbType.String;
            pName.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(pName);

            cmd.Parameters.AddWithValue("@LName", emp.LName);
            cmd.Parameters.AddWithValue("@BirthDate", emp.BirthDate ?? (object)DBNull.Value);

            conn.Open();
            var id = (decimal)cmd.ExecuteScalar();

            emp.EmployeeId = (int)id;

            return (int)id;
        }

        public IList<EmployeeModel> Insert(IEnumerable<EmployeeModel> employees)
        {
            throw new NotImplementedException();
        }

        public void Update(EmployeeModel emp)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = SQL_UPDATE;

            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@LName", emp.LName);
            cmd.Parameters.AddWithValue("@BirthDate", emp.BirthDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private EmployeeModel MapReaderToEmployee(DbDataReader rdr)
        {
            var emp = new EmployeeModel();
            emp.EmployeeId = rdr.GetInt32(rdr.GetOrdinal("EmployeeId"));
            emp.Name = rdr.GetString("Name");
            emp.LName = rdr.GetString("LName");
            emp.BirthDate = rdr.IsDBNull("BirthDate")
                ? (DateTime?)null
                : rdr.GetDateTime("BirthDate");

            return emp;
        }
    }
}
