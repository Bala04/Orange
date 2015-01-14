using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using maQx.Models;
using maQx.Utilities;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.SysAdmin)]
    public class DepartmentsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Departments
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("Context/Departments")]
        public async Task<JsonResult> ContextList()
        {
            return Json(await db.Departments.ToListAsync(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextList(string ID)
        {
            return Json(await db.Departments.FindAsync(ID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("Context/Departments")]
        public async Task<JsonResult> ContextCreate([Bind(Include = "Name")] Department Department)
        {
            try
            {
                Department.ID = Guid.NewGuid().ToString();               
                Department.Access = Roles.SysAdmin;
                db.Departments.Add(Department);

                await db.SaveChangesAsync();

                return Json(Department, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextUpdate([Bind(Include = "ID, Name")] Department Department)
        {
            try
            {
                var Dep = await db.Departments.FindAsync(Department.ID);
                Dep.Name = Department.Name;

                if (Dep == null)
                {
                    return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                }

                db.Entry(Dep).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(Dep, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextDelete(string ID)
        {
            try
            {
                var Dep = await db.Departments.FindAsync(ID);

                if (Dep == null)
                {
                    return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                }

                db.Departments.Remove(Dep);
                await db.SaveChangesAsync();

                return Json(Dep, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Departments/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Access")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Access")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
            await db.SaveChangesAsync();
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
