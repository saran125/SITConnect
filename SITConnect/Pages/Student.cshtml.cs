using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SITConnect.Models;
using SITConnect.Services;

namespace SITConnect.Pages
{
    public class StudentModel : PageModel
    {
        private readonly ILogger<StudentModel> _logger;
        private UserService _svc;
        public StudentModel(ILogger<StudentModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;

        }
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string decrypt { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") == "Student")

            {
                user = _svc.GetUserByEmail(HttpContext.Session.GetString("Email"));
                decrypt = _svc.Decrypt(user.Key);
                HttpContext.Session.GetString("Email");
                return Page();
            }
            else
            {
                return Redirect("/403");
            }
        }
    }
}
