using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.Models;
using EVEStandard;
using EVEStandard.Enumerations;
using EMS.Services;

namespace EMS.Controllers
{
    public class HomeController : Controller
    {
        public scopesservice emsservice;
        public EVEStandardAPI emsAPI;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Login()
        {
            emsservice = new scopesservice();
            emsservice.getScopes();
            string callbackUri = "http://eve-ems.azurewebsites.net/Home/Callback";
            string clientID = "f8be736a20244e12840b919908ce55b7";
            string secretkey = "ejRR281zI3gcJz3qV6I3eIyfdWU7oy2hFSjI3QWw";
            TimeSpan timeOut = new TimeSpan(0, 5, 0);
            emsAPI = new EVEStandardAPI("EMS", DataSource.Tranquility, timeOut, callbackUri,clientID,secretkey);
            emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes,"ems");
            string test = emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes, "ems").SignInURI;
            return new RedirectResult(test);
        }

        [HttpGet]
        public async Task<ActionResult> Callback()
        {
            await emsAPI.SSO.VerifyAuthorizationAsync(emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes, "ems"));
            return Redirect("/Home/Index");
        }
    }
}

