using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersControl.Data;
using UsersControl.Models;

namespace UsersControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        //private ApplicationDbContext db;
        //public UsersController(ApplicationDbContext context)
        //{
        //    db = context;
        //}
        public IActionResult Index([FromServices] ApplicationDbContext db)
        {
            var users = db.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ToggleAdmin(string id,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByIdAsync(id);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id,
            [FromServices] ApplicationDbContext db)
        {
            if (id != null)
            {
                var user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser editedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await db.Users.FirstOrDefaultAsync(c => c.Id == editedUser.Id);
            user.Email = editedUser.Email;
            user.Surname = editedUser.Surname;
            user.FirstName = editedUser.FirstName;
            user.LastName = editedUser.LastName;
            db.Users.Update(user);
            await db.SaveChangesAsync();

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var urlEncode = HttpUtility.UrlEncode(token);
            var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/Identity/Account/ResetPassword?userId={user.Id}&code={urlEncode}";

            var emailSender = new EmailSender();
            await emailSender.SendEmailAsync("orechpavel@yandex.ru", "Смена пароля",
                      "Для ввода пароля перейдите по ссылке: <a href=\"" + callbackUrl + "\">СБРОС</a>");
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string id,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (id != null)
            {
                var user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
    }
}