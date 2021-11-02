using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using BulkyBookPractice.Models.ViewModels;
using BulkyBookPractice.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBookPractice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            // Check if user is logged in when they access the home page
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                // If they are logged in, add the count of objects in their cart to the session

                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value)
                                                    .ToList()
                                                    .Count();

                HttpContext.Session.SetInt32(SD.CartSesh, count);
            }

            return View(productsList);
        }

        // When the user selects the details button of a product
        public IActionResult Details(int id)
        {
            // Load the product
            var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");

            // Create a cart object with the product in it
            ShoppingCart cartObj = new()
            {
                Product = product,
                ProductId = product.Id
            };

            // Send the cart object to the details view
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        // Add to Cart
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;

            if (ModelState.IsValid)
            {                
                // Get the ApplicationUser Id
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                // Check if the user already has a shopping cart object in the database
                ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ApplicationUserId == CartObject.ApplicationUserId
                                                                                     && u.ProductId == CartObject.ProductId,
                                                                                     includeProperties: "Product");

                if (cartFromDB == null)
                {
                    // If the user does not already have a cart, create a new one with the current CartObject
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }
                else
                {
                    // If the user already has a cart, update the quantity
                    cartFromDB.Count += CartObject.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDB);
                }
                _unitOfWork.Save();

                // Get the count of objects in a users cart 
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == CartObject.ApplicationUserId)
                                                    .ToList()
                                                    .Count();

                // Add the count to the session as either an int or an object
                HttpContext.Session.SetInt32(SD.CartSesh, count);
                //HttpContext.Session.SetObject(SD.CartSesh, CartObject);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDB = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == CartObject.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new()
                {
                    Product = productFromDB,
                    ProductId = productFromDB.Id
                };
                return View(cartObj);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
