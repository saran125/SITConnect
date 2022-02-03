using System;
using System.Collections.Generic;
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
    public class StaffModel : PageModel
    {

        private readonly ILogger<StaffModel> _logger;
        private UserService _svc;
        private readonly AuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StaffModel(ILogger<StaffModel> logger, UserService service, IHttpContextAccessor httpContextAccessor, AuditRepository auditRepository)
        {
            _logger = logger;
            _svc = service;
            _httpContextAccessor = httpContextAccessor;
            _auditRepository = auditRepository;

        }
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string decrypt { get; set; }
        private void AuditLogin()
        {
            var objaudit = new AuditModel();
            objaudit.ActionName = "Login";
            objaudit.Action = "Successfully login";
            objaudit.Role = HttpContext.Session.GetString("Role");
            objaudit.Time = DateTime.Now.ToString();
            objaudit.LoggedInAt = DateTime.Now.ToString();
            objaudit.LoginStatus = "Y";
            objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.UserId = HttpContext.Session.GetString("Id");
            objaudit.PageAccessed = "Staff homepage";
            objaudit.SessionId = HttpContext.Session.GetString("AuthToken");
            _auditRepository.InsertAuditLogs(objaudit);
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") == "Staff")
            {
                user = _svc.GetUserByEmail(HttpContext.Session.GetString("Email"));
                decrypt = _svc.Decrypt(user.Card_number, user.Key, user.IV);
                HttpContext.Session.GetString("Email");
                AuditLogin();
                return Page();
            }
            else
            {
                return Redirect("/forbidden");
            }
        }
}
}
