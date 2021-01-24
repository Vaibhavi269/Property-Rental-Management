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
    //[Authorize(Roles = "Manager")]
    public class AppartmentController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Appartment
        [Authorize(Roles = "Manager,Tenant")]
        public ActionResult Index()
        {
            var appartments = db.Appartments.Include(a => a.Building).Include(a => a.Person).Include(a => a.Person1).Include(a => a.Person2);
            var value = Roles.GetRolesForUser(User.Identity.Name);
            if (value.Length > 0)
            {
                ViewBag.role = value[0];
            }
            return View(appartments.ToList());
        }

        [Authorize(Roles = "Manager")]
        // GET: Appartment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appartment appartment = db.Appartments.Find(id);
            if (appartment == null)
            {
                return HttpNotFound();
            }
            return View(appartment);
        }

        // GET: Appartment/Create
        [Authorize(Roles = "Manager")]
        public ActionResult Create(int? id)
        {
            if (id != null)
                ViewBag.Build_id = new SelectList(db.Buildings, "Build_id", "Description", id);
            else
                ViewBag.Build_id = new SelectList(db.Buildings, "Build_id", "Description");
            //ViewBag.Tenant_id = new SelectList(db.Persons, "P_id", "FirstName");
            //ViewBag.Manager_id = new SelectList(db.Persons, "P_id", "FirstName");
            ViewBag.Owner_id = new SelectList(db.Persons.Where(x => x.Role == Enumeration.Role.Owner.ToString()), "P_id", "FirstName");
            return View();
        }

        // POST: Appartment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Apt_id,Build_id,Tenant_id,Manager_id,Owner_id,Description")] Appartment appartment)
        {
            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(User.Identity.Name.Split('|')[2]);
                appartment.Manager_id = userId;
                db.Appartments.Add(appartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Build_id = new SelectList(db.Buildings, "Build_id", "Description", appartment.Build_id);
            ViewBag.Tenant_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Tenant_id);
            ViewBag.Manager_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Manager_id);
            ViewBag.Owner_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Owner_id);
            return View(appartment);
        }

        // GET: Appartment/Edit/5
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appartment appartment = db.Appartments.Find(id);
            if (appartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Build_id = new SelectList(db.Buildings, "Build_id", "Description", appartment.Build_id);
            ViewBag.Tenant_id = new SelectList(db.Persons.Where(x => x.Role == Enumeration.Role.Tenant.ToString()), "P_id", "FirstName", appartment.Tenant_id);
            ViewBag.Manager_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Manager_id);
            ViewBag.Owner_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Owner_id);
            return View(appartment);
        }

        // POST: Appartment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "Apt_id,Build_id,Manager_id,Tenant_id,Owner_id,Description")] Appartment appartment)
        {
            if (ModelState.IsValid)
            {                
                db.Entry(appartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Build_id = new SelectList(db.Buildings, "Build_id", "Description", appartment.Build_id);
            ViewBag.Tenant_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Tenant_id);
            ViewBag.Manager_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Manager_id);
            ViewBag.Owner_id = new SelectList(db.Persons, "P_id", "FirstName", appartment.Owner_id);
            return View(appartment);
        }

        // GET: Appartment/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appartment appartment = db.Appartments.Find(id);
            if (appartment == null)
            {
                return HttpNotFound();
            }
            return View(appartment);
        }

        // POST: Appartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Appartment appartment = db.Appartments.Find(id);
            db.Appartments.Remove(appartment);
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
