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

namespace SITConnect.Pages
{

    public class logoutModel : PageModel
    {

        
      private readonly AuditRepository _auditRepository;
      private readonly IHttpContextAccessor _httpContextAccessor;

      public logoutModel( IHttpContextAccessor httpContextAccessor, AuditRepository auditRepository)
      {

          _httpContextAccessor = httpContextAccessor;
          _auditRepository = auditRepository;

      }
        private void Audit()
        {
            var objaudit = new AuditModel();
            objaudit.ActionName = "Logout";
            objaudit.Action = "Successfully logout";
            objaudit.Role = HttpContext.Session.GetString("Role");
            objaudit.Time = DateTime.Now.ToString();
            objaudit.LoggedOutAt = DateTime.Now.ToString();
            objaudit.LoginStatus = "N";
            objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
            objaudit.UserId = HttpContext.Session.GetString("Id");
            objaudit.PageAccessed = "Logout";
            objaudit.SessionId = HttpContext.Session.GetString("AuthToken");
            _auditRepository.InsertAuditLogs(objaudit);
        }

        public IActionResult OnGet()
        {
            Audit();
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Index");
        }
    }
}
