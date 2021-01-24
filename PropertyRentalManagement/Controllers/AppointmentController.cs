using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PropertyRentalManagement.helpers;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Appointment
        public ActionResult Index()
        {
            var appointments = new List<Appointment>();
            var role = Roles.GetRolesForUser(User.Identity.Name).FirstOrDefault();
            if (role == Enumeration.Role.Manager.ToString())
            {
                appointments = db.Appointments.Include(a => a.Person).Include(a => a.Appartment).ToList();
            }
            else
            {
                var email = User.Identity.Name.Split('|')[0];
                int p_id = db.Persons.FirstOrDefault(x => x.Email == email).P_id;
                appointments = db.Appointments.Include(a => a.Person).Include(a => a.Appartment).Where(x => x.potential_tenent_id == p_id).ToList();
            }
            return View(appointments);
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointment/Create
        public ActionResult Create(int id)
        {
            //ViewBag.potential_tenent_id = new SelectList(db.Persons, "P_id", "FirstName");
            //ViewBag.Apt_id = new SelectList(db.Appartments, "Apt_id", "Description");
            ViewBag.Apt_id = id;
            return View(db.Appointments.FirstOrDefault(x => x.Apt_id == id));
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Apt_id,Time")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var oldAppointment = db.Appointments.FirstOrDefault(x => x.Apt_id == appointment.Apt_id);
                if (oldAppointment != null)
                {
                    oldAppointment.Time = appointment.Time;
                    db.Entry(oldAppointment).State = EntityState.Modified;
                }
                else
                {
                    var email = User.Identity.Name.Split('|')[0];
                    appointment.potential_tenent_id = db.Persons.FirstOrDefault(x => x.Email == email).P_id;
                    db.Appointments.Add(appointment);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.potential_tenent_id = new SelectList(db.Persons, "P_id", "FirstName", appointment.potential_tenent_id);
            //ViewBag.Apt_id = new SelectList(db.Appartments, "Apt_id", "Description", appointment.Apt_id);
            return View(appointment);
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.potential_tenent_id = new SelectList(db.Persons, "P_id", "FirstName", appointment.potential_tenent_id);
            ViewBag.Apt_id = new SelectList(db.Appartments, "Apt_id", "Description", appointment.Apt_id);
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "A_id,Apt_id,potential_tenent_id,Time")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.potential_tenent_id = new SelectList(db.Persons, "P_id", "FirstName", appointment.potential_tenent_id);
            ViewBag.Apt_id = new SelectList(db.Appartments, "Apt_id", "Description", appointment.Apt_id);
            return View(appointment);
        }

        // GET: Appointment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
