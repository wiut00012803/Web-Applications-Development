using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shop.DAL.Entities;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.DAL
{
    public class EmployeeEfStoredProcRep : IEmployeeRep
    {
        private readonly DatabaseDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeEfStoredProcRep(DatabaseDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IList<EmployeeModel> Filter(string Name, string LName, DateTime? birthDate,
            out int totalRows, int page = 1, int pageSize = 10, 
            string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            var pFn = new SqlParameter("@FN", Name ?? (object)DBNull.Value);
            var pLn = new SqlParameter("@LN", LName ?? (object)DBNull.Value);
            var pBirthDate = new SqlParameter("@DOB", birthDate ?? (object)DBNull.Value);
            var pSortColumn = new SqlParameter("@SortColumn", sortColumn);
            var pSortDesc = new SqlParameter("@SortDesc", sortDesc);
            var pPage = new SqlParameter("@Page", page);
            var pPageSize = new SqlParameter("@PageSize", pageSize);

            totalRows = 0;

            return _dbContext.EmployeeFilterStoredProcResults.FromSqlRaw(
                @"exec dbsdEmployeeFilterWithSorting 
                            @FN, 
                            @LN,
                            @DOB,
                            @SortColumn , 
                            @SortDesc ,								
                            @Page, 
                            @PageSize", 
                            pFn, pLn, pBirthDate, pSortColumn, pSortDesc, pPage, pPageSize
                )
                .ToList()
                .AsQueryable()
                .ProjectTo<EmployeeModel>(_mapper.ConfigurationProvider).ToList();
        }

        public IList<EmployeeModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public EmployeeModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<EmployeeModel> ImportXml(string xml)
        {
            throw new NotImplementedException();
        }

        public int Insert(EmployeeModel emp)
        {
            throw new NotImplementedException();
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
