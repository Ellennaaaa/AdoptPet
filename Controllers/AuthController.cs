using System;
using System.Linq;
using System.Web.Mvc;
using AdoptPets.Models;
using AdoptPets.ViewModels;

namespace AdoptPets.Controllers
{
    public class AuthController : Controller
    {
        private adoptpetsEntities db = new adoptpetsEntities();

        public ActionResult Register()
        {
            ViewBag.id_residence = new SelectList(db.Residences, "id", "name");
            ViewBag.id_city = new SelectList(db.Cities, "id", "name");
            ViewBag.id_job = new SelectList(db.Jobs, "id", "name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool exists = db.Users.Any(u =>
                    u.email == model.email ||
                    u.username == model.username);

                if (exists)
                {
                    ModelState.AddModelError("", "Email or username already exists.");
                }
                else
                {
                    Location location = new Location();

                    location.id_city = model.id_city;
                    location.address = model.address;
                    location.latitude = null;
                    location.longitude = null;
                    location.created_at = DateTime.Now;

                    db.Locations.Add(location);
                    db.SaveChanges();

                    User user = new User();

                    user.name = model.name;
                    user.surname = model.surname;
                    user.email = model.email;
                    user.username = model.username;
                    user.password = BCrypt.Net.BCrypt.HashPassword(model.password);
                    user.phoneNumber = model.phoneNumber;
                    user.familyMembers = model.familyMembers;
                    user.dateOfBirth = model.dateOfBirth;

                    user.id_role = 2; // regular user
                    user.id_residence = model.id_residence;
                    user.id_location = location.id;

                    user.created_at = DateTime.Now;
                    user.updated_at = null;

                    db.Users.Add(user);
                    db.SaveChanges();

                    users_jobs userJob = new users_jobs();

                    userJob.id_user = user.id;
                    userJob.id_job = model.id_job;
                    userJob.salary = model.salary;
                    userJob.created_at = DateTime.Now;

                    db.users_jobs.Add(userJob);
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }
            }

            ViewBag.id_residence = new SelectList(db.Residences, "id", "name", model.id_residence);
            ViewBag.id_city = new SelectList(db.Cities, "id", "name", model.id_city);
            ViewBag.id_job = new SelectList(db.Jobs, "id", "name", model.id_job);

            return View(model);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.FirstOrDefault(u =>
                    u.username == model.usernameOrEmail ||
                    u.email == model.usernameOrEmail);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.password, user.password))
                {
                    Session["UserId"] = user.id;
                    Session["Username"] = user.username;
                    Session["RoleId"] = user.id_role;

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username/email or password.");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}