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

        /*
      private readonly AuditRepository _auditRepository;
      private readonly IHttpContextAccessor _httpContextAccessor;

      public logoutModel( IHttpContextAccessor httpContextAccessor, AuditRepository auditRepository)
      {

          _httpContextAccessor = httpContextAccessor;
          _auditRepository = auditRepository;

      }
      private void AuditLogout()
      {
          var objaudit = new AuditModel();
          objaudit.role = HttpContext.Session.GetString("Role");
          objaudit.ControllerName = "Portal";
          objaudit.ActionName = "Logout";
          objaudit.Area = "";
          objaudit.Id = Guid.NewGuid().ToString();
          objaudit.LoggedOutAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
          if (_httpContextAccessor.HttpContext != null)
              objaudit.IpAddress = Convert.ToString(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
          objaudit.UserId = HttpContext.Session.GetString("Id");
          objaudit.PageAccessed = "";
          objaudit.UrlReferrer = "";
          objaudit.SessionId = HttpContext.Session.Id;

          _auditRepository.InsertAuditLogs(objaudit);
      }
      */
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Index");
        }
    }
}
