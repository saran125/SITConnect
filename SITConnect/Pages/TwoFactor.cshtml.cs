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
        public string Myid { get; set; }
        [BindProperty]
        public string Mymessage { get; set; }
        [BindProperty]
        public string getid { get; set; }
        private static string TwoFactorKey(User user)
        {
            return $"myverysecretkey+{user.Email}";
        }
        public IActionResult OnGet()

        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return Redirect("/403");
            }
            else
            Myid = HttpContext.Session.GetString("Id");
            return Page();
        }
        public User theuser { get; set; }
        public IActionResult OnPost()
        {
            User currentuser = _svc.GetUserById(getid);
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(currentuser), Code);
            if (isValid)
            {
                string guid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("AuthToken", guid);
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddMinutes(10);
                Response.Cookies.Append("AuthToken", guid, option);
                HttpContext.Session.SetString("Email", currentuser.Email);
                
                if (_svc.Max_password(currentuser.Email))
                {
                    HttpContext.Session.SetString("Role", currentuser.Role);
                    if (currentuser.Role == "Student")
                    { return RedirectToPage("/Student"); }
                    else if (currentuser.Role == "Staff")
                    {
                        { return RedirectToPage("/Staff"); }
                    }
                }
                else
                {
                    
                    return Redirect("/Reset_Password?Id=" + currentuser.Id);
                }

                }
            else
            {
                var setupInfo = twoFactor.GenerateSetupCode("SITConnect", user.Email, TwoFactorKey(user), false, 3);
                Mymessage = "Invalid Code";
                Myid = user.Id;
                return Page();
            }
            return Page();
        }
    }
}
