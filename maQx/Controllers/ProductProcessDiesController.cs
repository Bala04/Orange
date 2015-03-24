using System.Threading.Tasks;
using System.Web.Mvc;
using maQx.Models;
using maQx.Utilities;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.AppUser)]
    [AccessAuthorize]
    public class ProductProcessDiesController : Controller
    {
        // GET: ProductProcessDies
        public async Task<ActionResult> Index()
        {
            return View(new ProductProcessViewModel()
            {
                Products = (await Shared.GetProducts(User.GetDivision())).ToSelectList("Description")
            });
        }
    }
}