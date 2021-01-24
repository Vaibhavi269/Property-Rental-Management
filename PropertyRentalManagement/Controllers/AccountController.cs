using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagement.Models;
using PropertyRentalManagement.helpers;
using System.Web.Security;
using System.Security.Claims;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Owner")]
    public class AccountController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Account        
        public ActionResult Index()
        {
            var persons = db.Persons.Include(p => p.User).Where(x => x.Role != Enumeration.Role.Owner.ToString());
            return View(persons.ToList());
        }

        // GET: Account/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: Account/Create
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "FirstName,LastName,Phone,Email,User")] Person person)
        {
            if (ModelState.IsValid)
            {
                //check email exist
                if (!db.Persons.Any(x => x.Email == person.Email))
                {
                    //get role from auth if available 
                    var value = Roles.GetRolesForUser(User.Identity.Name);
                    if (value.Length > 0 && value[0] == Enumeration.Role.Owner.ToString())
                    {
                        //if owner then owner can create manager account
                        person.Role = Enumeration.Role.Manager.ToString();
                    }
                    else
                    {
                        //add teant role for online user 
                        person.Role = Enumeration.Role.Tenant.ToString();
                    }
                    db.Persons.Add(person);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(person);
        }

        // GET: Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = Roles.GetRolesForUser(User.Identity.Name).FirstOrDefault();
                if (role == Enumeration.Role.Owner.ToString())
                {
                    //owner view 
                    return RedirectToAction("Index");
                }
                else if (role == Enumeration.Role.Manager.ToString())
                {
                    //manager view 
                    return RedirectToAction("Index", "Building");
                }
                else
                {
                    //Tenant view
                    return RedirectToAction("Index", "Appartment");
                }
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login([Bind(Include = "Email,User")] Person person)
        {
            if (!string.IsNullOrWhiteSpace(person.Email) && !string.IsNullOrWhiteSpace(person.User.Password))
            {
                var result = db.Persons.Where(x => x.Email == person.Email
                                && x.User.Password == person.User.Password).FirstOrDefault();
                if (result != null)
                {
                    FormsAuthentication.SetAuthCookie(result.Email + '|' + result.FirstName + '|' + result.P_id, false);
                    if (result.Role == Enumeration.Role.Owner.ToString())
                    {
                        //owner view 
                        return RedirectToAction("Index");
                    }
                    else if (result.Role == Enumeration.Role.Manager.ToString())
                    {
                        //manager view 
                        return RedirectToAction("Index", "Building");
                    }
                    else
                    {
                        //Tenant view
                        return RedirectToAction("Index", "Appartment");
                    }
                }
            }

            return View(person);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: Account/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.P_id = new SelectList(db.Users, "Id", "Password", person.P_id);
            ViewBag.P_id = new SelectList(db.Users, "Id", "Password", person.P_id);
            return View(person);
        }

        // POST: Account/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "P_id,FirstName,LastName,Phone,Email,Role")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.P_id = new SelectList(db.Users, "Id", "Password", person.P_id);
            ViewBag.P_id = new SelectList(db.Users, "Id", "Password", person.P_id);
            return View(person);
        }

        // GET: Account/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.Persons.Find(id);
            db.Users.Remove(person.User);
            db.Persons.Remove(person);
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
