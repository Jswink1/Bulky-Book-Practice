using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using BulkyBookPractice.Models.ViewModels;
using BulkyBookPractice.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<Category> categoryList = await _unitOfWork.Category.GetAllAsync();

            // Initialize View Model
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            // If we are inserting a new product 
            if (id == null)
            {
                // Return the view with an empty Product Model
                return View(productVM);
            }

            // if we are updating a product
            else
            {
                // Load the product record from the DB
                productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

                // Check if the object is found
                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                // Get the web root path
                string webRootpath = _hostEnvironment.WebRootPath;
                // Retrieve the files that have been uploaded
                var files = HttpContext.Request.Form.Files;

                // If an image file was uploaded
                if (files.Count > 0)
                {
                    // Name the file with a Guid
                    string fileName = Guid.NewGuid().ToString();
                    // Navigate to the images path
                    var uploads = Path.Combine(webRootpath, @"images\products");
                    // Get the extension of the uploaded file
                    var extension = Path.GetExtension(files[0].FileName);

                    // If we are editing
                    if (productVM.Product.ImageUrl != null)
                    {
                        // Remove the old image
                        var imagePath = Path.Combine(webRootpath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Upload the new image
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                // Else, if the user did not upload a new image file
                else
                {
                    // If we are editing, an image file for the product should already exist in the DB
                    if (productVM.Product.Id != 0)
                    {
                        // Retrieve the image stored in the DB
                        Product productFromDB = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = productFromDB.ImageUrl;
                    }
                    // If we are inserting, the user needs to upload an image, so throw an error
                    //else
                    //{
                    //    TempData["Error"] = "You must upload an Image File";
                    //    return View(productVM);
                    //}
                }

                // If we are inserting
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }

                // If we are updating
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // If the user submits a ViewModel with invalid inputs
            // Initialize a new ViewModel and send that so that the application does not crash, and the correct input validation error messsages appear
            else
            {
                IEnumerable<Category> categoryList = await _unitOfWork.Category.GetAllAsync();

                productVM.CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Retrieve the list of Products
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            // Send the list to the javascript function to be displayed as a DataTable
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // Load Product from DB
            var product = _unitOfWork.Product.Get(id);
            if (product == null)
            {
                // Send error status and message to javascript function so that Sweetalert and Toastr can display notification
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Remove the image
            string webRootpath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootpath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Remove and Save
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            // Send success status and message to javascript function so that Sweetalert and Toastr can display notification
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
