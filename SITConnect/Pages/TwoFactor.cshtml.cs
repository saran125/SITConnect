using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SITConnect.Models;
using SITConnect.Services;

namespace SITConnect.Pages
{
    public class TwoFactorModel : PageModel
    {
        private readonly ILogger<TwoFactorModel> _logger;
        private UserService _svc;
        public TwoFactorModel(ILogger<TwoFactorModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;

        }
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public string Code { get; set; }
        [BindProperty]
        public string Myemail { get; set; }
        [BindProperty]
        public string getemail { get; set; }
        private static string TwoFactorKey(User user)
        {
            return $"myverysecretkey+{user.Email}";
        }
        public void OnGet(string Email)
        {
            Myemail = Email;
        }
        public User theuser { get; set; }
        public IActionResult OnPost()
        {
            User currentuser = _svc.GetUserByEmail(getemail);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(currentuser), Code);
            if (isValid)
            {
      
                HttpContext.Session.SetString("Email", currentuser.Email);
                HttpContext.Session.SetString("Fname", currentuser.Fname);
                HttpContext.Session.SetString("Lname", currentuser.Lname);
                HttpContext.Session.SetString("Role", currentuser.Role);
                if(currentuser.Role == "Student")
                { return RedirectToPage("/Student"); }
                else if (currentuser.Role == "Staff")
                {
                    { return RedirectToPage("/Staff"); }
                }
            }
            return Page();
        }
    }
}
