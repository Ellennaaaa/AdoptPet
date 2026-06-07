using System;
using System.Linq;
using System.Web.Mvc;
using AdoptPets.Models;
using System.Data.Entity;

namespace AdoptPets.Controllers
{
    public class requestsController : Controller
    {
        private adoptpetsEntities db = new adoptpetsEntities();

        public ActionResult Create(int announcementId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = Request.RawUrl });
            }

            int userId = (int)Session["UserId"];

            announcement ann = db.announcements.Find(announcementId);


            if (ann == null)
            {
                return HttpNotFound();
            }

            if (ann.id_user == userId)
            {
                TempData["Error"] = "Ne možete poslati zahtjev za svoju životinju.";
                return RedirectToAction("Details", "announcements", new { id = announcementId });
            }

            bool alreadyRequested = db.requests.Any(r =>
                r.id_ann == announcementId &&
                r.id_user == userId);

            if (alreadyRequested)
            {
                TempData["Error"] = "Već ste poslali zahtjev za ovu životinju.";
                return RedirectToAction("Details", "announcements", new { id = announcementId });
            }

            request req = new request();

            req.id_ann = announcementId;
            req.id_user = userId;
            req.id_status = 1;
            req.dateReq = DateTime.Now;
            req.created_at = DateTime.Now;
            req.updated_at = null;

            db.requests.Add(req);
            db.SaveChanges();

            TempData["Success"] = "Zahtjev za usvajanje je poslat.";

            return RedirectToAction("Details", "announcements", new { id = announcementId });
        }

        public ActionResult ReceivedForAnnouncement(int announcementId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = (int)Session["UserId"];

            var receivedRequests = db.requests
                .Include(r => r.User)
                .Include(r => r.User.Residence)
                .Include(r => r.User.Location)
                .Include(r => r.User.Location.City)
                .Include(r => r.User.users_jobs.Select(uj => uj.Job))
                .Include(r => r.status)
                .Include(r => r.announcement)
                .Where(r => r.id_ann == announcementId && r.announcement.id_user == userId)
                .ToList();

            ViewBag.AnnouncementId = announcementId;

            return View(receivedRequests);
        }

        public ActionResult Accept(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = (int)Session["UserId"];

            request req = db.requests
                .Include(r => r.announcement)
                .FirstOrDefault(r => r.id == id);

            if (req == null)
            {
                return HttpNotFound();
            }

            if (req.announcement.id_user != userId)
            {
                return new HttpStatusCodeResult(403);
            }

            req.id_status = 2; // Accepted

            db.SaveChanges();

            return RedirectToAction(
                "ReceivedForAnnouncement",
                new { announcementId = req.id_ann });
        }

        public ActionResult Mine()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = (int)Session["UserId"];

            var myRequests = db.requests
                .Include(r => r.announcement)
                .Include(r => r.announcement.Animal)
                .Include(r => r.announcement.Animal.images)
                .Include(r => r.status)
                .Where(r => r.id_user == userId)
                .ToList();

            return View(myRequests);
        }
    }
}