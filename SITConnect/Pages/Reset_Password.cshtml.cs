using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class Reset_PasswordModel : PageModel
    {
        private readonly ILogger<Reset_PasswordModel> _logger;
        private UserService _svc;
        private AuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Reset_PasswordModel(ILogger<Reset_PasswordModel> logger, UserService service, IHttpContextAccessor httpContextAccessor, AuditRepository repository)
        {
            _logger = logger;
            _svc = service;
            _auditRepository = repository;
            _httpContextAccessor = httpContextAccessor;

        }

        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public string Myid { get; set; }
        
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
           ErrorMessage = "Password Need to be at least 12 characters, combination of lower case, upper case, numbers & special characters")]
        [BindProperty]
        public string Currentpwd { get; set; }
        private void Audit()
        {
            var objaudit = new AuditModel();
            objaudit.ActionName = "Password";
            objaudit.Action = "Successfully changed password";
            objaudit.Role = HttpContext.Session.GetString("Role");
            objaudit.Time = DateTime.Now.ToString();
            objaudit.LoginStatus = "Y";
            objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.UserId = HttpContext.Session.GetString("Id"); 
            objaudit.PageAccessed = "Reset Password";
            objaudit.SessionId = HttpContext.Session.GetString("AuthToken");
            _auditRepository.InsertAuditLogs(objaudit);
        }
        public IActionResult OnGet()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("AuthToken")) && Request.Cookies["AuthToken"] != null)
                {
                    user = _svc.GetUserByEmail(HttpContext.Session.GetString("Email"));
                Myid = user.Id;
                HttpContext.Session.GetString("Email");
                    return Page();
               
                }
            else
                {
                    return Redirect("/forbidden");
                }
            
            
        }
        public IActionResult OnPost()
        {

            User theuser = _svc.GetUserByEmail(HttpContext.Session.GetString("Email"));
            if (_svc.pwdcheck(HttpContext.Session.GetString("Id"), Currentpwd))
            {
                if (_svc.Min_password(theuser.Id))
                {

                    if (_svc.reset_password(user))
                    {
                        Audit();
                        return Redirect("/logout");
                    }
                    else
                    {
                        Message = "Cannot use previous Password!";
                        return Page();
                    }
                }
                else
                {
                    Message = "Cannot Change Password Now, Please wait at least few minutes!";
                    return Page();
                }
            }
            else
            {
                Myid = user.Id;
                Message = "Please enter valid Password!";
                return Page();
            }
        }
    }
}
