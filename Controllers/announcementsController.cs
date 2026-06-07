using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Mvc;
using AdoptPets.Models;
using AdoptPets.ViewModels;

namespace AdoptPets.Controllers
{
    public class announcementsController : Controller
    {
        private adoptpetsEntities db = new adoptpetsEntities();

        // GET: announcements
        public ActionResult Index()
        {
            var announcements = db.announcements
                .Include(a => a.Animal)
                .Include(a => a.User)
                .Where(a => a.status == true);

            return View(announcements.ToList());
        }

        public ActionResult Mine()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = (int)Session["UserId"];

            var myAnnouncements = db.announcements
                .Include(a => a.Animal)
                .Include(a => a.Animal.images)
                .Include(a => a.Animal.Species)
                .Where(a => a.id_user == userId)
                .ToList();

            return View(myAnnouncements);
        }

        // GET: announcements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            announcement announcement = db.announcements
                .Include(a => a.Animal)
                .Include(a => a.Animal.images)
                .Include(a => a.Animal.Species)
                .Include(a => a.Animal.Gender)
                .Include(a => a.Animal.animal_conditions.Select(ac => ac.Condition))
                .Include(a => a.Animal.animal_vaccines.Select(av => av.Vaccine))
                .Include(a => a.User)
                .Include(a => a.User.Location)
                .Include(a => a.User.Location.City)
                .FirstOrDefault(a => a.id == id);

            if (announcement == null)
                return HttpNotFound();

            return View(announcement);
        }

        // GET: announcements/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            PrepareCreateViewBags();

            return View(new CreateAnnouncementViewModel());
        }

        // POST: announcements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAnnouncementViewModel model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserId"];

                Animal animal = new Animal
                {
                    name = model.animalName,
                    id_species = model.id_species,
                    id_gender = model.id_gender,
                    dateOfBirth = model.animalDateOfBirth.Value,
                    id_user = userId,
                    created_at = DateTime.Now
                };

                db.Animals.Add(animal);
                db.SaveChanges();

                announcement announcement = new announcement
                {
                    id_user = userId,
                    id_animal = animal.id,
                    dateAnn = DateTime.Now,
                    status = true,
                    description = model.description,
                    created_at = DateTime.Now,
                    updated_at = null
                };

                db.announcements.Add(announcement);
                db.SaveChanges();

                if (model.selectedConditions != null)
                {
                    foreach (int conditionId in model.selectedConditions)
                    {
                        db.animal_conditions.Add(new animal_conditions
                        {
                            id_animal = animal.id,
                            id_condition = conditionId,
                            created_at = DateTime.Now
                        });
                    }
                }

                if (model.selectedVaccines != null)
                {
                    foreach (int vaccineId in model.selectedVaccines)
                    {
                        db.animal_vaccines.Add(new animal_vaccines
                        {
                            id_animal = animal.id,
                            id_vaccine = vaccineId,
                            created_at = DateTime.Now
                        });
                    }
                }

                db.SaveChanges();

                if (model.images != null)
                {
                    foreach (var file in model.images)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                            string folderPath = Server.MapPath("~/Uploads/Animals/");
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            string path = Path.Combine(folderPath, fileName);

                            file.SaveAs(path);

                            image img = new image();

                            img.id_animal = animal.id;
                            img.title = animal.name;
                            img.image_path = "/Uploads/Animals/" + fileName;
                            img.tookOn = DateTime.Now;
                            img.created_at = DateTime.Now;

                            db.images.Add(img);
                        }
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            PrepareCreateViewBags();

            return View(model);
        }

        private void PrepareCreateViewBags()
        {
            ViewBag.id_species = new SelectList(db.Species, "id", "name");
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name");
            ViewBag.Conditions = db.Conditions.ToList();
            ViewBag.Vaccines = db.Vaccines.ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}