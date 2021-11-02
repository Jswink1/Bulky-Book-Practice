using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using BulkyBookPractice.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            // If a new Company is being inserted
            if (id == null)
            {
                // Send an empty Company Model to the view
                return View(company);
            }

            // If we are updating a Company
            else
            {
                // Load the Company object from the DB
                company = _unitOfWork.Company.Get(id.GetValueOrDefault());

                if (company == null)
                {
                    return NotFound();
                }

                // Send the Company to the view
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                // If we are inserting
                if (company.Id == 0)
                {
                   _unitOfWork.Company.Add(company);
                }

                // If we are updating
                else
                {
                    _unitOfWork.Company.Update(company);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }        

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Retrieve the list of Companies through Stored Procedure
            var companies = _unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // Load Company from DB
            var company = _unitOfWork.Company.Get(id);
            if (company == null)
            {
                // Send error status and message to javascript function so that Sweetalert and Toastr can display notification
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Remove and Save
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();

            // Send success status and message to javascript function so that Sweetalert and Toastr can display notification
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
