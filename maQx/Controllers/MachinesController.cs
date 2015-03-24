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
    public class MachinesController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Machines
        public ActionResult Index()
        {
            return View();
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
        [Roles(Roles.Create, Roles.CreateEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Machines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "Key,MachineType,MinLoad,MaxLoad,Code,Description")] MachineViewModel machine)
        {
            if (ModelState.IsValid)
            {
                var Division = await db.Divisions.FindAsync(User.GetDivision());

                if (Division == null)
                {
                    return HttpNotFound();
                }

                db.Machines.Add(new Machine
                {
                    Code = machine.Code,
                    Description = machine.Description,
                    MachineType = machine.MachineType,
                    MinLoad = machine.MinLoad,
                    MaxLoad = machine.MaxLoad,
                    Division = Division
                });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(machine);
        }

        // GET: Machines/Edit/5
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
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
            return View(new MachineEditViewModel(machine));
        }

        // POST: Machines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,MachineType,MinLoad,MaxLoad,Code,Description")] MachineEditViewModel machine)
        {
            if (ModelState.IsValid)
            {
                var Machine = await db.Machines.Include(x => x.Division).Where(x => x.Key == machine.Key).FirstOrDefaultAsync();

                Machine.Code = machine.Code;
                Machine.Description = machine.Description;
                Machine.MachineType = machine.MachineType;
                Machine.MinLoad = machine.MinLoad;
                Machine.MaxLoad = machine.MaxLoad;

                db.Entry(Machine).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(machine);
        }

        // GET: Machines/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
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
        [Roles(Roles.Delete, Roles.EditDelete)]
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
