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
    public class ProductProcessController : Controller
    {
        private AppContext db = new AppContext();

        // GET: ProductProcess
        public async Task<ActionResult> Index()
        {
            return View(new ProductProcessViewModel()
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description")
            });
        }

        // GET: ProductProcess/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductProcess productProcess = await db.ProductProcesses.FindAsync(id);
            if (productProcess == null)
            {
                return HttpNotFound();
            }
            return View(productProcess);
        }

        // GET: ProductProcess/Create
        public ActionResult Create()
        {
            ViewBag.ProcessKey = new SelectList(db.Process, "Key", "Code");
            ViewBag.ProductKey = new SelectList(db.Products, "Key", "Code");
            return View();
        }

        // POST: ProductProcess/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Key,ProductKey,ProcessKey,Order,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] ProductProcess productProcess)
        {
            if (ModelState.IsValid)
            {
                db.ProductProcesses.Add(productProcess);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProcessKey = new SelectList(db.Process, "Key", "Code", productProcess.ProcessKey);
            ViewBag.ProductKey = new SelectList(db.Products, "Key", "Code", productProcess.ProductKey);
            return View(productProcess);
        }

        // GET: ProductProcess/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductProcess productProcess = await db.ProductProcesses.FindAsync(id);
            if (productProcess == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProcessKey = new SelectList(db.Process, "Key", "Code", productProcess.ProcessKey);
            ViewBag.ProductKey = new SelectList(db.Products, "Key", "Code", productProcess.ProductKey);
            return View(productProcess);
        }

        // POST: ProductProcess/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key,ProductKey,ProcessKey,Order,UserCreated,UserModified,CreatedAt,UpdatedAt,TimeStamp,ActiveFlag")] ProductProcess productProcess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productProcess).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProcessKey = new SelectList(db.Process, "Key", "Code", productProcess.ProcessKey);
            ViewBag.ProductKey = new SelectList(db.Products, "Key", "Code", productProcess.ProductKey);
            return View(productProcess);
        }

        // GET: ProductProcess/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductProcess productProcess = await db.ProductProcesses.FindAsync(id);
            if (productProcess == null)
            {
                return HttpNotFound();
            }
            return View(productProcess);
        }

        // POST: ProductProcess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ProductProcess productProcess = await db.ProductProcesses.FindAsync(id);
            db.ProductProcesses.Remove(productProcess);
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
