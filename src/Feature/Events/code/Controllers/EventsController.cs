﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Feature.Events.Controllers
{
    using System.Globalization;
    using Sitecore.Shell.Feeds.FeedTypes;

    public class EventsController : Controller
    {
        public ActionResult List()
        {
            return this.View();
        }

        public ActionResult Calendar()
        {
            return this.View();
        }

        public ActionResult Detail()
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult GetEventsListJson()
        {
            var events = new List<object>
            {
                new {title = "EventsControllerevent1",startsAtTxt=System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)},
                new {title = "EventsControllerevent2",startsAtTxt=System.DateTime.UtcNow.AddDays(2).ToString(CultureInfo.InvariantCulture)},
                new {title = "EventsControllerevent3",startsAtTxt=System.DateTime.UtcNow.AddDays(4).ToString(CultureInfo.InvariantCulture)}
            };
            return Json(events.ToArray(), JsonRequestBehavior.AllowGet);
        }

    }
}