using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SITConnect.Models;
using SITConnect.Repository;
using SITConnect.Services;

namespace SITConnect.Pages
{
    public class TwoFactorAuthenticationControllerModel : PageModel
    {
        private readonly ILogger<TwoFactorAuthenticationControllerModel> _logger;
        private UserService _svc;
        private AuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TwoFactorAuthenticationControllerModel(ILogger<TwoFactorAuthenticationControllerModel> logger, UserService service, IHttpContextAccessor httpContextAccessor, AuditRepository repository)
        {
            _logger = logger;
            _svc = service;
            _auditRepository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string SetupCode { get; set; }
        [BindProperty]
        public string BarcodeImageUrl { get; set; }
        [BindProperty]
        public string Code { get; set; }
        [BindProperty]
        public string MyMessage { get; set; }
        [BindProperty]
        public string MyId { get; set; }
        [BindProperty]
        public string getid { get; set; }
        public IActionResult OnGet(string Id)
        { if (Id == null)
            {
                return Redirect("/forbidden");
            }
            else
            {
                MyId = Id;
                User currentuser = _svc.GetUserById(Id);
                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                var setupInfo = twoFactor.GenerateSetupCode("SITConnect", currentuser.Email, TwoFactorKey(currentuser), false, 3);
                SetupCode = setupInfo.ManualEntryKey;
                BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                return Page();
            }
        }

        private void AuditTwoFactor(string Id)
        {
            var objaudit = new AuditModel();
            objaudit.ActionName = "TwoFactorAuthenticator";
            objaudit.Action = "Enabled TwoFactorAuthenticator";
            objaudit.Time = DateTime.Now.ToString();
            objaudit.LoginStatus = "N";
            objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.UserId = Id;
            objaudit.PageAccessed = "TwoFactorAuthenticationController";
            objaudit.SessionId = HttpContext.Session.Id;
            _auditRepository.InsertAuditLogs(objaudit);
        }
        private static string TwoFactorKey(User user)
        {
            return $"myverysecretkey+{user.Email}";
        }
        public IActionResult OnPost()
        {
            User user = _svc.GetUserById(getid);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), Code);
            if (isValid)
            {
                if (_svc.Enable_twofactor(user.Email))
                {
                    AuditTwoFactor(getid);
                    return Redirect("/login");
                }
                else
                {
                    var setupInfo = twoFactor.GenerateSetupCode("SITConnect", user.Email, TwoFactorKey(user), false, 3);
                    SetupCode = setupInfo.ManualEntryKey;
                    BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    MyMessage = "Invalid Code";
                    MyId = user.Id;
                    return Page();
                }
            }

            else
            {
                var setupInfo = twoFactor.GenerateSetupCode("SITConnect", user.Email, TwoFactorKey(user), false, 3);
                SetupCode = setupInfo.ManualEntryKey;
                BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                MyMessage = "Invalid Code";
                MyId = user.Id;
                return Page();
            }
        }
    }
}
