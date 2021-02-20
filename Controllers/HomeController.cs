using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApplication11.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _log;

        public HomeController(ILogger<HomeController> log)
        {
            _log = log;
            _log.LogDebug("Ctor");
        }

        public ActionResult Index()
        {
            _log.LogInformation("{Action}", nameof(Index));
            return View();
        }

        public ActionResult About()
        {
            _log.LogInformation("{Action}", nameof(About));
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            _log.LogInformation("{Action} - {ViewBag}", nameof(Contact), JsonConvert.SerializeObject((object)ViewBag));

            return View();
        }
    }
}