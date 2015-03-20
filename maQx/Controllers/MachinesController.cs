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

namespace maQx.Controllers
{
    public class MachinesController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Machines
        public async Task<ActionResult> Index()
        {
            return View(await db.Machines.ToListAsync());
        }

        // GET: Machines/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = await db.Machines.FindAsync(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // GET: Machines/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Machines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Key,MachineType,MinLoad,MaxLoad,Code,Description,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                db.Machines.Add(machine);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(machine);
        }

        // GET: Machines/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = await db.Machines.FindAsync(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: Machines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key,MachineType,MinLoad,MaxLoad,Code,Description,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(machine).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(machine);
        }

        // GET: Machines/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = await db.Machines.FindAsync(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Machine machine = await db.Machines.FindAsync(id);
            db.Machines.Remove(machine);
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
