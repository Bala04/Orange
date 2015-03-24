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
    public class ShiftsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Shifts
        public ActionResult Index()
        {
            return View();
        }

        // GET: Shifts/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = await db.Shifts.FindAsync(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // GET: Shifts/Create
        [Roles(Roles.Create, Roles.CreateEdit)]
        public ActionResult Create()
        {
            return View(new ShiftViewModel
            {
                StartTime = new DateTime(1970, 1, 1, 0, 0, 0).ToUnixTime(),
                EndTime = new DateTime(1970, 1, 1, 0, 0, 0).ToUnixTime(),
            });
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "Description,StartTime,EndTime")] ShiftViewModel shift)
        {
            if (ModelState.IsValid)
            {
                var Division = await db.Divisions.FindAsync(User.GetDivision());

                if (Division == null)
                {
                    return HttpNotFound();
                }

                var StartTime = shift.StartTime.FromUnixTime();
                var EndTime = shift.EndTime.FromUnixTime();

                db.Shifts.Add(new Shift
                {
                    Description = shift.Description,
                    StartTime = new DateTime(1970, 1, 1, StartTime.Hour, StartTime.Minute, 0),
                    EndTime = new DateTime(1970, 1, 1, EndTime.Hour, EndTime.Minute, 0),
                    Division = Division

                });

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(shift);
        }

        // GET: Shifts/Edit/5
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = await db.Shifts.FindAsync(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(new ShiftEditViewModel(shift));
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,Description,StartTime,EndTime")] ShiftEditViewModel shift)
        {
            if (ModelState.IsValid)
            {
                var Shift = await db.Shifts.Include(x => x.Division).Where(x => x.Key == shift.Key).FirstOrDefaultAsync();

                var StartTime = shift.StartTime.FromUnixTime();
                var EndTime = shift.EndTime.FromUnixTime();

                Shift.Description = shift.Description;
                Shift.StartTime = new DateTime(1970, 1, 1, StartTime.Hour, StartTime.Minute, 0);
                Shift.EndTime = new DateTime(1970, 1, 1, EndTime.Hour, EndTime.Minute, 0);

                db.Entry(Shift).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shift);
        }

        // GET: Shifts/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = await db.Shifts.FindAsync(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Shift shift = await db.Shifts.FindAsync(id);
            db.Shifts.Remove(shift);
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
