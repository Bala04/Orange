using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using maQx.Models;
using maQx.Utilities;

namespace maQx.Controllers
{
    public class ProductRawMaterialsController : Controller
    {
        // GET: ProductRawMaterials
        public async Task<ActionResult> Index()
        {
            return View(new ProductRawMaterialViewModel()
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description", "All", DefaultValue: "0"),
                RawMaterials = (await Shared.GetRawMaterials(User.GetDivision())).ToSelectList("Description", "All", DefaultValue: "0")
            });
        }
    }
}