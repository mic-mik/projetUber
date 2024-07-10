using Projet_GoFast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_GoFast.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        private GoFastDb db = new GoFastDb();

        public ActionResult Index()
        {
            db.Adresse.Count();
            return View();
        }
    }
}