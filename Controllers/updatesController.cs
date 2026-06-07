using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdoptPets.Models;
using System.IO;
using AdoptPets.ViewModels;

namespace AdoptPets.Controllers
{
    public class updatesController : Controller
    {
        private adoptpetsEntities db = new adoptpetsEntities();

        public ActionResult MyAdoptions()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            var adoptions = db.Adoptions
                .Include(a => a.Animal)
                .Include(a => a.Animal.images)
                .Include(a => a.User)   // previous owner
                .Include(a => a.User1)  // new owner
                .Where(a => a.id_new_owner == userId)
                .ToList();

            return View(adoptions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        public ActionResult Create(int adoptionId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            Adoption adoption = db.Adoptions.Find(adoptionId);

            if (adoption == null)
                return HttpNotFound();

            if (adoption.id_new_owner != userId)
                return new HttpStatusCodeResult(403);

            return View(new CreateUpdateViewModel { adoptionId = adoptionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUpdateViewModel model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            Adoption adoption = db.Adoptions.Find(model.adoptionId);

            if (adoption == null)
                return HttpNotFound();

            if (adoption.id_new_owner != userId)
                return new HttpStatusCodeResult(403);

            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (model.image != null && model.image.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Updates/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.image.FileName);
                    string fullPath = Path.Combine(folderPath, fileName);

                    model.image.SaveAs(fullPath);

                    imagePath = "/Uploads/Updates/" + fileName;
                }

                update newUpdate = new update();

                newUpdate.id_adoption = model.adoptionId;
                newUpdate.description = model.description;
                newUpdate.image_path = imagePath;
                newUpdate.dateTook = DateTime.Now;
                newUpdate.created_at = DateTime.Now;

                db.updates.Add(newUpdate);
                db.SaveChanges();

                return RedirectToAction("MyAdoptions");
            }

            return View(model);
        }
    }
}