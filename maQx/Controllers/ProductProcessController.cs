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
