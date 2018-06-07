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
    //public class ESIModelDTO<T>
    //{
    //    public T Model { get; set; }
    //    public bool NotModified { get; set; }
    //    public string ETag { get; set; }
    //    public string Language { get; set; }
    //    public DateTimeOffset? Expires { get; set; }
    //    public DateTimeOffset? LastModified { get; set; }
    //    public int MaxPages { get; set; }
    //    public int RemainingErrors { get; set; }
    //}


    public class HomeController : Controller
    {
        
        
        public scopesservice Emsservice;
        public EVEStandardAPI EmsApi;
        public EVEStandard.Models.SSO.Authorization Model;
        public iniservice EmsIniservice;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment _env)
        {
            this._hostingEnvironment = _env;
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
            Emsservice = new scopesservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\emsscopes.ems"));
            Emsservice.GetScopes();
            EmsIniservice=new iniservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\cfg.xml"));
            EmsApi = new EVEStandardAPI("EMS", DataSource.Tranquility, new TimeSpan(0, 5, 0), EmsIniservice.CallbackUrl,EmsIniservice.ClientId,EmsIniservice.SecretKey);
            Model = EmsApi.SSO.AuthorizeToEVEURI(Emsservice.scopes, "ems");

            return new RedirectResult(EmsApi.SSO.AuthorizeToEVEURI(Emsservice.scopes, "ems").SignInURI);            
        }

        public async Task<IActionResult> Callback()
        {
            Emsservice = new scopesservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\emsscopes.ems"));
            Emsservice.GetScopes();
            EmsIniservice = new iniservice(Path.Combine(_hostingEnvironment.WebRootPath, @"res\cfg.xml"));
            EmsApi = new EVEStandardAPI("EMS", DataSource.Tranquility, new TimeSpan(0, 5, 0), EmsIniservice.CallbackUrl, EmsIniservice.ClientId, EmsIniservice.SecretKey);
            Model = EmsApi.SSO.AuthorizeToEVEURI(Emsservice.scopes, "ems");

            Model.AuthorizationCode= Request.Query["code"];
            Model.ReturnedState = Request.Query["state"];
            //Model.ExpectedState= "ems";
            AccessTokenDetails token=await EmsApi.SSO.VerifyAuthorizationAsync(Model);
            ViewData["access_token"] = token.AccessToken;
            ViewData["expires_in"] = token.ExpiresIn;
            ViewData["refresh_token"] = token.RefreshToken;
            ViewData["expires"] = token.Expires;
            CharacterDetails character = await EmsApi.SSO.GetCharacterDetailsAsync(token.AccessToken);
            ViewData["CharacterID"] = character.CharacterID;
            ViewData["CharacterName"] = character.CharacterName;
            ViewData["ExpiresOn"] = character.ExpiresOn;
            ViewData["Scopes"] = character.Scopes;
            ViewData["TokenType"] = character.TokenType;
            ViewData["CharacterOwnerHash"] = character.CharacterOwnerHash;

            AuthDTO authDTOems = new AuthDTO
            {
                AccessToken = token,
                Character = character
            };
            List<CharacterMining> mininglist = new List<CharacterMining>();
            long pages = 0;
            try
            {
                (mininglist, pages) = await EmsApi.Industry.CharacterMiningLedgerV1Async(authDTOems, 1);
            }
            catch(Exception e)
            {
                ViewData["emessage"]= e.Message.ToString(); ;
                ViewData["error"]= e.ToString(); ;
                ViewData["data"]= e.Data.ToString();
                return View();
            }
            mininglist.ToArray();
            string image = "https://imageserver.eveonline.com/Character/" + character.CharacterID + "_512.jpg";
            Uri imguri = new Uri(image);
            ViewData["image"] = imguri;
            return View(mininglist);
        }
    }
}

