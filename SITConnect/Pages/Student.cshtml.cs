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
    public class StudentModel : PageModel
    {
        private readonly ILogger<StudentModel> _logger;
        private UserService _svc;
        /*private readonly AuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentModel(IHttpContextAccessor httpContextAccessor, AuditRepository auditRepository)
        {

            _httpContextAccessor = httpContextAccessor;
            _auditRepository = auditRepository;

        }
        */
        public StudentModel(ILogger<StudentModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;

        }
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string decrypt { get; set; }
        /*
        private void AuditLogin()
        {
            var objaudit = new AuditModel();
            objaudit.ControllerName = "Portal";
            objaudit.ActionName = "Login";
            objaudit.Id = Guid.NewGuid().ToString();
            objaudit.Area = "";
            if (_httpContextAccessor.HttpContext != null)
                objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.role = Convert.ToString(HttpContext.Session.GetInt32("Role"));
            objaudit.LoggedInAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            objaudit.UserId = Convert.ToString(HttpContext.Session.GetInt32("Id"));
            objaudit.PageAccessed = "";
            objaudit.UrlReferrer = "";
            objaudit.SessionId = HttpContext.Session.Id;
            _auditRepository.InsertAuditLogs(objaudit);
        }
        */
        public IActionResult OnGet()
        {
           
            if (HttpContext.Session.GetString("Role") == "Student")
            {
                user = _svc.GetUserByEmail(HttpContext.Session.GetString("Email"));
                decrypt = _svc.Decrypt(user.Card_number, user.Key, user.IV);
                HttpContext.Session.GetString("Email");
               // AuditLogin();
                return Page();
            }
            else
            {
                return Redirect("/forbidden");
            }
        }
    }
}
