using BulkyBookPractice.DataAccess.Data;
using BulkyBookPractice.Models;
using BulkyBookPractice.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Retrieve the list of Users, Roles, and the user role assignments
            var users = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var roles = _db.Roles.ToList();
            var userRoles = _db.UserRoles.ToList();

            foreach (var user in users)
            {
                // Get the users RoleId from the UserRoles List
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                // Assign the Role Name to the User object
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                // Initialize Company name of User to prevent null exception
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // User is currently locked, so unlock them
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                // User is currently unlocked, so lock them
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Success" });
        }

        #endregion
    }
}
