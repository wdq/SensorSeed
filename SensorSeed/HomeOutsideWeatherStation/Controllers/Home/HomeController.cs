﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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

        public JsonResult TemperatureFeelsLikeDewPointChartData()
        {
            HomeTemperatureFeelsLikeDewPointChartDataModel model = new HomeTemperatureFeelsLikeDewPointChartDataModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HumidityPressureChartData()
        {
            HomeHumidityPressureChartDataModel model = new HomeHumidityPressureChartDataModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrecipitationChartData()
        {
            return View();
        }

        public ActionResult WindChartData()
        {
            return View();
        }

        public JsonResult PowerChartData()
        {
            HomePowerChartDataModel model = new HomePowerChartDataModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}