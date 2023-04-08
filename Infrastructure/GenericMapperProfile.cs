using AutoMapper;
using Shop.DAL.Entities;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Infrastructure
{
    public class GenericMapperProfile: Profile
    {
        public GenericMapperProfile()
        {
            CreateMap<Employee, EmployeeModel>().ReverseMap();
            CreateMap<EmployeeFilterStoredProcResult, EmployeeModel>();
        }
    }
}
