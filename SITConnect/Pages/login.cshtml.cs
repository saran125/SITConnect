using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SITConnect.Models;
using SITConnect.Repository;
using SITConnect.Services;

namespace SITConnect.Pages
{
    public class loginModel : PageModel
    {
        private readonly ILogger<loginModel> _logger;
        private UserService _svc;
        private AuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public loginModel(ILogger<loginModel> logger, UserService service,IHttpContextAccessor httpContextAccessor, AuditRepository repository)
        {
            _logger = logger;
            _svc = service;
            _auditRepository = repository;
            _httpContextAccessor = httpContextAccessor;

        }

        [BindProperty]
        public string MyMessage { get; set; }
        [BindProperty]
        public User theuser { get; set; }
        private void Audit(string Id)
        {
            var objaudit = new AuditModel();
            objaudit.ActionName = "Login";
            objaudit.Action = "Unable to login. Account is locked";
            objaudit.Time = DateTime.Now.ToString();
            objaudit.LoginStatus = "N";
            objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.UserId = Id;
            objaudit.PageAccessed = "Login";
            objaudit.SessionId = HttpContext.Session.Id;
            _auditRepository.InsertAuditLogs(objaudit);
        }
        public void OnGet()
        {
        }
        [ValidateAntiForgeryToken]
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (_svc.Lockout(theuser.Email))
                {
              
                        if (_svc.Login(theuser))
                        {
                        User login_user = _svc.GetUserByEmail(theuser.Email);
                       
                            if (_svc.Twofactor(theuser.Email))
                            {
                 
                            HttpContext.Session.SetString("Id", login_user.Id);
                            return Redirect("/TwoFactor?Id=" + login_user.Id);
                            }
                            else
                            {
                                return Redirect("/TwoFactorAuthenticationController?Id=" + login_user.Id);
                            } 
                    }
                        else
                        {
                            MyMessage = "Invalid Email or Password";
                            return Page();
                        }
                    
                }
                else
                {
                    User user = _svc.GetUserByEmail(theuser.Email);
                    Audit(user.Id);
                    MyMessage = "Account is locked!";
                    return Page();
                }

            }
            return Page();
        }
    }
}
