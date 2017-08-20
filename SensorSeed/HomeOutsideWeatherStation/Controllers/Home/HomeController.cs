using System;
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
        public ActionResult Index(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomeIndexModel model = new HomeIndexModel(endDateNotNull);            
            return View(model);
        }

        public JsonResult TemperatureFeelsLikeDewPointChartData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomeTemperatureFeelsLikeDewPointChartDataModel model = new HomeTemperatureFeelsLikeDewPointChartDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HumidityPressureChartData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomeHumidityPressureDataModel model = new HomeHumidityPressureDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrecipitationChartData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomePrecipitationChartDataModel model = new HomePrecipitationChartDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WindChartData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomeWindChartDataModel model = new HomeWindChartDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PowerChartData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomePowerChartDataModel model = new HomePowerChartDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TenDaySunriseSunsetData(DateTime? endDate)
        {
            DateTime endDateNotNull = DateTime.Now;
            if (endDate.HasValue)
            {
                endDateNotNull = endDate.Value;
            }
            endDateNotNull = endDateNotNull.ToUniversalTime();
            HomeTenDaySunriseSunsetDataModel model = new HomeTenDaySunriseSunsetDataModel(endDateNotNull);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}