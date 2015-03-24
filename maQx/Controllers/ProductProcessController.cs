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
    public class ProductProcessController : Controller
    {
        // GET: ProductProcess
        public async Task<ActionResult> Index()
        {
            return View(new ProductProcessViewModel()
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description")
            });
        }
    }
}
