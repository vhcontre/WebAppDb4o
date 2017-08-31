using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppDb4o.Data;
using WebAppDb4o.Models;

namespace WebAppDb4o.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonRepository db = new PersonRepository();
        // GET: Person
        public ActionResult Index()
        {
            return View(db.All());
        }

        public ActionResult Details(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Person model)
        {
            model.RowGuid = Guid.NewGuid().ToString();
            db.Add(model);
            return Redirect("Index");
        } 
        #endregion
        
        #region Edit
        public ActionResult Edit(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }
        [HttpPost]
        public ActionResult Edit(Person model)
        {
            db.Edit(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }
        [HttpPost]
        public ActionResult Delete(Person model)
        {
            db.Delete(model);
            return RedirectToAction("Index");
        } 
        #endregion
    }
}