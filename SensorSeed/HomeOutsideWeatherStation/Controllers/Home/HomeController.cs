using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeOutsideWeatherStation.Controllers.Home
{
    using Models.Home;
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeIndexModel model = new HomeIndexModel();            
            return View(model);
        }

    }
}