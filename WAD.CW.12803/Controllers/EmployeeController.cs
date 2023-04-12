using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DAL;
using Shop.Models;

namespace Shop.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployeeRep _Rep;

        public EmployeeController(IEmployeeRep Rep)
        {
            _Rep = Rep;
        }

        // GET: EmployeeController
        public ActionResult Index()
        {
            var employees = _Rep.GetAll();

            return View(employees);
        }

        public ActionResult Import()
        {
            return View(new List<EmployeeModel>());
        }

        [HttpPost]
        public ActionResult Import(IFormFile importFile)
        {
            var employees = new List<EmployeeModel>();

            //parse the json/xml etc file
            using var stream = importFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            employees = (List<EmployeeModel>)JsonSerializer.Deserialize(
                    reader.ReadToEnd(),
                    typeof(List<EmployeeModel>
                ));

            var inserted = _Rep.Insert(employees);
            return View(inserted);
        }

        public ActionResult ImportXml()
        {
            return View(new List<EmployeeModel>());
        }

        [HttpPost]
        public ActionResult ImportXml(IFormFile importFile)
        {
            using var stream = importFile.OpenReadStream();
            using var reader = new StreamReader(stream);

            var employees = _Rep.ImportXml(reader.ReadToEnd());

            return View(employees);
        }

        public ActionResult Filter(EmployeeFilterViewModel filterModel)
        {
            int totalRows;
            var employees = _Rep.Filter(
                filterModel.Name, filterModel.LName, filterModel.BirthDate,
                out totalRows, filterModel.Page, filterModel.PageSize,
                filterModel.SortColumn, filterModel.SortDesc
                );

            filterModel.Employees = employees;

            return View(filterModel);
        }


        // GET: EmployeeController/Details/5
        public ActionResult Details(int id)
        {
            var emp = _Rep.GetById(id);
            return View(emp);
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeModel emp)
        {
            try
            {
                int id = _Rep.Insert(emp);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(int id)
        {
            var emp = _Rep.GetById(id);
            return View(emp);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeModel emp)
        {
            try
            {
                emp.EmployeeId = id;
                _Rep.Update(emp);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: EmployeeController/Delete/5
        public ActionResult Delete(int id)
        {
            var emp = _Rep.GetById(id);
            return View(emp);
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _Rep.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }
    }
}
