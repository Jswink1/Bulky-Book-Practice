using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using BulkyBookPractice.Models.ViewModels;
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
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productPage = 1)
        {
            // Retrieve all the CategoryModels
            CategoryVM categoryVM = new()
            {
                Categories = await _unitOfWork.Category.GetAllAsync()
            };
            var count = categoryVM.Categories.Count();

            // Amount of items to display per page
            int itemsPerPage = 3;

            // Filter the list of Categorys to display the correct amount according to our pagination
            categoryVM.Categories = categoryVM.Categories.OrderBy(p => p.Name)
                                                         .Skip((productPage - 1) * itemsPerPage)
                                                         .Take(itemsPerPage)
                                                         .ToList();

            categoryVM.Pagination = new()
            {
                CurrentPage = productPage,
                ItemsPerPage = itemsPerPage,
                TotalItems = count,
                UrlParam = "/Admin/Category/Index?productPage=:"
            };

            return View(categoryVM);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new();

            // If a new Category is being inserted
            if (id == null)
            {
                // Send an empty Category Model to the view
                return View(category);
            }

            // If we are updating a Category
            else
            {
                // Load the Category from the DB
                category = await _unitOfWork.Category.GetAsync(id.GetValueOrDefault());

                if (category == null)
                {
                    return NotFound();
                }

                // Send the Category to the view
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                // If we are inserting
                if (category.Id == 0)
                {
                    await _unitOfWork.Category.AddAsync(category);
                }

                // If we are updating
                else
                {
                    _unitOfWork.Category.Update(category);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            // Load Category from DB
            var category = await _unitOfWork.Category.GetAsync(id);
            if (category == null)
            {
                // Set Temporary Data to display error message on screen
                TempData["Error"] = "Error deleting Category";

                
                return Json(new { });
            }

            // Remove and Save
            await _unitOfWork.Category.RemoveAsync(category);
            _unitOfWork.Save();

            // Set Temporary Data to display success message on screen
            TempData["Success"] = "Category successfully deleted";

            
            return Json(new { });
        }
    }
}
