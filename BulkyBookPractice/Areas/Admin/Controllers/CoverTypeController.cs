using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using BulkyBookPractice.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
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
            CoverType coverType = new();

            // If a new CoverType is being inserted
            if (id == null)
            {
                // Send an empty CoverType Model to the view
                return View(coverType);
            }

            // If we are updating a CoverType
            else
            {
                // Create Stored Procedure parameters
                DynamicParameters parameters = new();
                parameters.Add("@Id", id);

                // Load the CoverType from the DB using Stored Procedures
                coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameters);

                if (coverType == null)
                {
                    return NotFound();
                }

                // Send the CoverType to the view
                return View(coverType);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                // Create Stored Procedure parameter for inserting
                DynamicParameters parameters = new();
                parameters.Add("@Name", coverType.Name);

                // If we are inserting
                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameters);
                }

                // If we are updating
                else
                {
                    // Add parameter for updating
                    parameters.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameters);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Retrieve the list of CoverTypes through Stored Procedure
            var coverTypes = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);

            // Send the list to the javascript function to be displayed as a DataTable
            return Json(new { data = coverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // Create Stored Procedure parameters
            DynamicParameters parameters = new();
            parameters.Add("@Id", id);

            // Load CoverType from DB using Stored Procedures
            var coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameters);
            if (coverType == null)
            {
                // Send error status and message to javascript function so that Sweetalert and Toastr can display notification
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Remove using Stored Procedure and Save
            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameters);
            _unitOfWork.Save();

            // Send success status and message to javascript function so that Sweetalert and Toastr can display notification
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
