using AutoMapper;
using Shop.DAL.Entities;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.DAL
{
    public class EmployeeEfRep : IEmployeeRep
    {
        private readonly DatabaseDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeEfRep(DatabaseDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Delete(int id)
        {
            _dbContext.Employees.Remove(new Employee() { EmployeeId = id });
            _dbContext.SaveChanges();
        }

        public IList<EmployeeModel> Filter(
            string Name, string LName, DateTime? birthDate, 
            out int totalRows, int page = 1, int pageSize = 10, 
            string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            var query = _dbContext.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(e => e.Name.StartsWith(Name));

            if (!string.IsNullOrWhiteSpace(LName))
                query = query.Where(e => e.Name.StartsWith(LName));

            if (birthDate.HasValue)
                query = query.Where(e => e.BirthDate >= birthDate);

            totalRows = query.Count();

            if (sortDesc)
                if ("Name".Equals(sortColumn, StringComparison.OrdinalIgnoreCase))
                    query = query.OrderByDescending(e => e.Name);
                else if ("LName".Equals(sortColumn, StringComparison.OrdinalIgnoreCase))
                    query = query.OrderByDescending(e => e.LName);
                else
                    query = query.OrderByDescending(e => e.EmployeeId);
            else
                if ("Name".Equals(sortColumn, StringComparison.OrdinalIgnoreCase))
                query = query.OrderBy(e => e.Name);
            else if ("LName".Equals(sortColumn, StringComparison.OrdinalIgnoreCase))
                query = query.OrderBy(e => e.LName);
            else
                query = query.OrderBy(e => e.EmployeeId);

            var entities = query.Skip((page - 1) * pageSize).Take(pageSize);

            return entities.Select(e => _mapper.Map<EmployeeModel>(e)).ToList();
        }

        public IList<EmployeeModel> GetAll()
        {
            var entities = _dbContext.Employees.ToList();
            return entities.Select(e => _mapper.Map<EmployeeModel>(e)).ToList();
        }

        public EmployeeModel GetById(int id)
        {
            var entity = _dbContext.Employees.Find(id);

            var emp = new EmployeeModel()
            {
                EmployeeId = entity.EmployeeId,
                Name = entity.Name,
                LName = entity.LName,
                BirthDate = entity.BirthDate
            };

            return emp;
        }

        public IList<EmployeeModel> ImportXml(string xml)
        {
            throw new NotImplementedException();
        }

        public int Insert(EmployeeModel emp)
        {
            var inserted = _dbContext.Employees.Add(_mapper.Map<Employee>(emp)).Entity;
            _dbContext.SaveChanges();

            return inserted.EmployeeId;
        }

        public IList<EmployeeModel> Insert(IEnumerable<EmployeeModel> employees)
        {
            throw new NotImplementedException();
        }

        public void Update(EmployeeModel emp)
        {
            _dbContext.Employees.Update(_mapper.Map<Employee>(emp));
            _dbContext.SaveChanges();
        }
    }
}
