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
    [Authorize(Roles = Roles.AppUser)]
    [AccessAuthorize]
    public class ProcessController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Process
        public ActionResult Index()
        {
            return View();
        }

        // GET: Process/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Process process = await db.Process.FindAsync(id);
            if (process == null)
            {
                return HttpNotFound();
            }
            return View(process);
        }

        // GET: Process/Create
        [Roles(Roles.Create, Roles.CreateEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Process/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "ValidateRawMaterial,Code,Description")] ProcessViewModel process)
        {
            if (ModelState.IsValid)
            {
                var Division = await db.Divisions.FindAsync(User.GetDivision());

                if (Division == null)
                {
                    return HttpNotFound();
                }

                db.Process.Add(new Process
                {
                    Code = process.Code,
                    Description = process.Description,
                    ValidateRawMaterial = process.ValidateRawMaterial,
                    Division = Division
                });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(process);
        }

        // GET: Process/Edit/5
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Process process = await db.Process.FindAsync(id);
            if (process == null)
            {
                return HttpNotFound();
            }
            return View(new ProcessEditViewModel(process));
        }

        // POST: Process/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,ValidateRawMaterial,Code,Description")] ProcessEditViewModel process)
        {
            if (ModelState.IsValid)
            {
                var Process = await db.Process.Include(x => x.Division).Where(x => x.Key == process.Key).FirstOrDefaultAsync();

                Process.Code = process.Code;
                Process.Description = process.Description;
                Process.ValidateRawMaterial = process.ValidateRawMaterial;

                db.Entry(Process).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(process);
        }

        // GET: Process/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Process process = await db.Process.FindAsync(id);
            if (process == null)
            {
                return HttpNotFound();
            }
            return View(process);
        }

        // POST: Process/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Process process = await db.Process.FindAsync(id);
            db.Process.Remove(process);
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
