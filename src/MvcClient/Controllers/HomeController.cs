using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json.Linq;

namespace MvcClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
