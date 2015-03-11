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
    public class ProductRawMaterialsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: ProductRawMaterials
        public async Task<ActionResult> Index()
        {
            return View(new ProductRawMaterialViewModel()
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description", "All", DefaultValue: "0"),
                RawMaterials = (await Shared.GetRawMaterials(User.GetDivision())).ToSelectList("Description", "All", DefaultValue: "0")
            });
        }

        // GET: ProductRawMaterials/Create
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(new ProductRawMaterialCreateViewModel
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description", SelectedField: Id),
                RawMaterials = (await Shared.GetRawMaterials(User.GetDivision())).ToSelectList("Description", "- Raw Materials -")
            });
        }

        // GET: ProductRawMaterials/Edit/[Id]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ProductRawMaterial = await db.ProductRawMaterials.Include(x => x.Product).Include(x => x.RawMaterial).Where(x => x.Key == Id).FirstOrDefaultAsync();

            if (ProductRawMaterial == null)
            {
                return HttpNotFound();
            }

            return View(new ProductRawMaterialEditViewModel(ProductRawMaterial));
        }

        // GET: Process/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> Delete(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ProductRawMaterial = await db.ProductRawMaterials.Include(x => x.Product).Include(x => x.RawMaterial).Where(x => x.Key == Id).FirstOrDefaultAsync();

            if (ProductRawMaterial == null)
            {
                return HttpNotFound();
            }

            return View(ProductRawMaterial);
        }

        // POST: ProductRawMaterials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "Product,RawMaterial,InputQuantity,Unit")] ProductRawMaterialCreateViewModel productRawMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Product = await db.Products.FindAsync(productRawMaterial.Product);

                    if (Product == null)
                    {
                        return HttpNotFound();
                    }

                    var RawMaterial = await db.RawMaterials.FindAsync(productRawMaterial.RawMaterial);

                    if (RawMaterial == null)
                    {
                        return HttpNotFound();
                    }

                    var InputQuantity = Convert.ToDouble(productRawMaterial.InputQuantity);

                    db.ProductRawMaterials.Add(new ProductRawMaterial
                    {
                        Product = Product,
                        RawMaterial = RawMaterial,
                        Quantity = UnitOfMeasure.Convert(InputQuantity, productRawMaterial.Unit, Units.Kgs),
                        InputQuantity = InputQuantity,
                        SelectedUnit = productRawMaterial.Unit,
                    });

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            productRawMaterial.Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description", SelectedField: productRawMaterial.Product);
            productRawMaterial.RawMaterials = (await Shared.GetRawMaterials(User.GetDivision())).ToSelectList("Description", SelectedField: productRawMaterial.RawMaterial);

            return View(productRawMaterial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,Product,RawMaterial,InputQuantity,Unit")] ProductRawMaterialEditViewModel productRawMaterial)
        {
            var ProductRawMaterial = await db.ProductRawMaterials.Include(x => x.Product).Include(x => x.RawMaterial).Where(x => x.Key == productRawMaterial.Key).FirstOrDefaultAsync();

            if (ProductRawMaterial == null)
            {
                return HttpNotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var InputQuantity = Convert.ToDouble(productRawMaterial.InputQuantity);

                    ProductRawMaterial.Quantity = UnitOfMeasure.Convert(InputQuantity, productRawMaterial.Unit, Units.Kgs);
                    ProductRawMaterial.InputQuantity = InputQuantity;
                    ProductRawMaterial.SelectedUnit = productRawMaterial.Unit;

                    db.Entry(ProductRawMaterial).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            productRawMaterial.ProductItem = ProductRawMaterial.Product;
            productRawMaterial.RawMaterialItem = ProductRawMaterial.RawMaterial;

            return View(productRawMaterial);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var ProductRawMaterial = await db.ProductRawMaterials.FindAsync(id);
            db.ProductRawMaterials.Remove(ProductRawMaterial);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void SetInfo()
        {
            ViewBag.Info = TempData["Info"] as ClientInfo;
        }
    }


}