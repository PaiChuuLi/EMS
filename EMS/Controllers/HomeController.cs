using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.Models;
using EVEStandard;
using EVEStandard.Enumerations;
using EMS.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using EVEStandard.Models.SSO;
using EVEStandard.Models.API;
using EVEStandard.Models;

namespace EMS.Controllers
{
    public class ESIModelDTO<T>
    {
        public T Model { get; set; }
        public bool NotModified { get; set; }
        public string ETag { get; set; }
        public string Language { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public int MaxPages { get; set; }
        public int RemainingErrors { get; set; }
    }

    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        
        public scopesservice emsservice;
        public EVEStandardAPI emsAPI;
        public EVEStandard.Models.SSO.Authorization model;

        public HomeController(IHostingEnvironment _env)
        {
            _hostingEnvironment = _env;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Login()
        {            
            emsservice = new scopesservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\emsscopes.ini"));
            emsservice.getScopes();
            //string callbackUri = "http://eve-ems.azurewebsites.net/Home/Callback";
            string callbackUri = "http://localhost:55697/Home/Callback";
            string clientID = "f8be736a20244e12840b919908ce55b7";
            string secretkey = "ejRR281zI3gcJz3qV6I3eIyfdWU7oy2hFSjI3QWw";
            TimeSpan timeOut = new TimeSpan(0, 5, 0);
            emsAPI = new EVEStandardAPI("EMS", DataSource.Tranquility, timeOut, callbackUri,clientID,secretkey);
            model = emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes, "ems");
            return new RedirectResult(emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes, "ems").SignInURI);            
        }

        public async Task<IActionResult> Callback()
        {           
            emsservice = new scopesservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\emsscopes.ini"));
            emsservice.getScopes();
            //string callbackUri = "http://eve-ems.azurewebsites.net/Home/Callback";
            string callbackUri = "http://localhost:55697/Home/Callback";
            string clientID = "f8be736a20244e12840b919908ce55b7";
            string secretkey = "ejRR281zI3gcJz3qV6I3eIyfdWU7oy2hFSjI3QWw";
            TimeSpan timeOut = new TimeSpan(0, 5, 0);
            emsAPI = new EVEStandardAPI("EMS", DataSource.Tranquility, timeOut, callbackUri, clientID, secretkey);
            model = emsAPI.SSO.AuthorizeToEVEURI(emsservice.scopes, "ems");
            model.AuthorizationCode= Request.Query["code"];
            model.ReturnedState = Request.Query["state"];
            model.ExpectedState= "ems";
            AccessTokenDetails token=await emsAPI.SSO.VerifyAuthorizationAsync(model);
            ViewData["access_token"] = token.AccessToken;
            ViewData["expires_in"] = token.ExpiresIn;
            ViewData["refresh_token"] = token.RefreshToken;
            ViewData["expires"] = token.Expires;
            CharacterDetails character = await emsAPI.SSO.GetCharacterDetailsAsync(token.AccessToken);
            ViewData["CharacterID"] = character.CharacterID;
            ViewData["CharacterName"] = character.CharacterName;
            ViewData["ExpiresOn"] = character.ExpiresOn;
            ViewData["Scopes"] = character.Scopes;
            ViewData["TokenType"] = character.TokenType;
            ViewData["CharacterOwnerHash"] = character.CharacterOwnerHash;

            AuthDTO authDTOems = new AuthDTO();
            authDTOems.AccessToken = token;
            authDTOems.Character = character;
            List<CharacterMining> mininglist = new List<CharacterMining>();
            long pages = 0;
            try
            {
                (mininglist, pages) = await emsAPI.Industry.CharacterMiningLedgerV1Async(authDTOems, 1);
            }
            catch(Exception e)
            {
                ViewData["emessage"]= e.Message.ToString(); ;
                ViewData["error"]= e.ToString(); ;
                ViewData["data"]= e.Data.ToString();
                return View();
            }
            ViewData["mininglist"] = mininglist;
            //string image = "https://imageserver.eveonline.com/Character/" + character.CharacterID + "_512.jpg";
            //Uri imguri = new Uri(image);
            //ViewData["image"] = imguri;
            return View();
        }
    }
}

