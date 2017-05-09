using FinderJobs.Manager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using AspNet.Identity.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinderJobs.Manager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HabilidadesAdminController : Controller
    {
        public async Task<ActionResult> Index()
        {	        
            return View();
        }        
    }
}
