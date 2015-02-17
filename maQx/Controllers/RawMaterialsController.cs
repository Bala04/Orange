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
    [Authorize(Roles = Roles.AppUser)]
    [AccessAuthorize]
    public class RawMaterialsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: RawMaterials
        public ActionResult Index()
        {
            return View();
        }

        // GET: RawMaterials/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterial rawMaterial = await db.RawMaterials.FindAsync(id);
            if (rawMaterial == null)
            {
                return HttpNotFound();
            }
            return View(rawMaterial);
        }

        // GET: RawMaterials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RawMaterials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Key,Unit,Measurement,Code,Description,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] RawMaterial rawMaterial)
        {
            if (ModelState.IsValid)
            {
                db.RawMaterials.Add(rawMaterial);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(rawMaterial);
        }

        // GET: RawMaterials/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterial rawMaterial = await db.RawMaterials.FindAsync(id);
            if (rawMaterial == null)
            {
                return HttpNotFound();
            }
            return View(rawMaterial);
        }

        // POST: RawMaterials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key,Unit,Measurement,Code,Description,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] RawMaterial rawMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rawMaterial).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(rawMaterial);
        }

        // GET: RawMaterials/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterial rawMaterial = await db.RawMaterials.FindAsync(id);
            if (rawMaterial == null)
            {
                return HttpNotFound();
            }
            return View(rawMaterial);
        }

        // POST: RawMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            RawMaterial rawMaterial = await db.RawMaterials.FindAsync(id);
            db.RawMaterials.Remove(rawMaterial);
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
