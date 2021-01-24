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
    public class MessageController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Message
        public ActionResult Index()
        {
            var userId = Convert.ToInt32(User.Identity.Name.Split('|')[2]);
            var messages = db.Messages.Where(x => x.From_person == userId || x.To_person == userId).OrderBy(x=>x.Created_at);
            return View(messages.ToList());
        }

        // GET: Message/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Message/Create
        public ActionResult Create(int id)
        {
            ViewBag.Aptormsg_id = id;
            return View();
        }

        // POST: Message/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string msg_content, int aptormsg_id)
        {
            if (ModelState.IsValid)
            {                
                int to = 0;
                int frm = Convert.ToInt32(User.Identity.Name.Split('|')[2]);
                var role = Roles.GetRolesForUser(User.Identity.Name).FirstOrDefault();                 
                if (role != Enumeration.Role.Manager.ToString())
                {
                    //when user is tenant then get managerid
                    to = db.Appartments.FirstOrDefault(x => x.Apt_id == aptormsg_id).Manager_id;
                }
                else
                {
                    //when user is manager then get tenant id from msg table
                    to = db.Messages.FirstOrDefault(x => x.M_Id == aptormsg_id).From_person;
                }
                    
                Message message = new Message
                {
                    From_person = frm,
                    To_person = to,
                    Msg_content = msg_content
                };
                message.Created_at = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Apt_id = aptormsg_id;
            Message messageReturn = new Message { Msg_content = msg_content };
            //ViewBag.From_person = new SelectList(db.Persons, "P_id", "FirstName", message.From_person);
            //ViewBag.To_person = new SelectList(db.Persons, "P_id", "FirstName", message.To_person);
            return View(messageReturn);
        }

        // GET: Message/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            ViewBag.From_person = new SelectList(db.Persons, "P_id", "FirstName", message.From_person);
            ViewBag.To_person = new SelectList(db.Persons, "P_id", "FirstName", message.To_person);
            return View(message);
        }

        // POST: Message/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "M_Id,From_person,To_person,Msg_content")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.From_person = new SelectList(db.Persons, "P_id", "FirstName", message.From_person);
            ViewBag.To_person = new SelectList(db.Persons, "P_id", "FirstName", message.To_person);
            return View(message);
        }

        // GET: Message/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Message/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
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
