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
    public class DiesController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Dies
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dies/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Die die = await db.Dies.FindAsync(id);
            if (die == null)
            {
                return HttpNotFound();
            }
            return View(die);
        }

        // GET: Dies/Create
        [Roles(Roles.Create, Roles.CreateEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dies/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "MaxSink,MaxCount,Tolerance,Code,Description")] DieViewModel die)
        {
            if (ModelState.IsValid)
            {
                var Division = await db.Divisions.FindAsync(User.GetDivision());

                if (Division == null)
                {
                    return HttpNotFound();
                }

                db.Dies.Add(new Die
                {
                    MaxCount = die.MaxCount,
                    Tolerance = die.Tolerance,
                    Code = die.Code,
                    Description = die.Description,
                    MaxSink = die.MaxSink,
                    Division = Division
                });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(die);
        }

        // GET: Dies/Edit/5
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Die die = await db.Dies.FindAsync(id);
            if (die == null)
            {
                return HttpNotFound();
            }
            return View(new DieEditViewModel(die));
        }

        // POST: Dies/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,MaxSink,MaxCount,Tolerance,Code,Description")] DieEditViewModel die)
        {
            if (ModelState.IsValid)
            {
                var Die = await db.Dies.Include(x => x.Division).Where(x => x.Key == die.Key).FirstOrDefaultAsync();

                Die.Code = die.Code;
                Die.Description = die.Description;
                Die.MaxCount = die.MaxCount;
                Die.Tolerance = die.Tolerance;
                Die.MaxSink = die.MaxSink;

                db.Entry(Die).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(die);
        }

        // GET: Dies/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Die die = await db.Dies.FindAsync(id);
            if (die == null)
            {
                return HttpNotFound();
            }
            return View(die);
        }

        // POST: Dies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Die die = await db.Dies.FindAsync(id);
            db.Dies.Remove(die);
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
