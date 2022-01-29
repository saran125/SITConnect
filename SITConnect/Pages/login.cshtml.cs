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
    public class loginModel : PageModel
    {
        private readonly ILogger<loginModel> _logger;
        private UserService _svc;
        public loginModel(ILogger<loginModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;
        }
        [BindProperty]
        public string MyMessage { get; set; }
        [BindProperty]
        public User thuser { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (_svc.Lockout(thuser.Email))
                {
                    if (_svc.Login(thuser))
                    {
                        if (_svc.Twofactor(thuser.Email))
                        {
                            return Redirect("/TwoFactor?Email=" + thuser.Email);
                        }
                        else
                        {
                            return Redirect("/TwoFactorAuthenticationController?Email=" + thuser.Email);
                        }

                        User currentuser = _svc.Theuser(thuser);
                        HttpContext.Session.SetString("Email", currentuser.Email);
                        HttpContext.Session.SetString("Fname", currentuser.Fname);
                        HttpContext.Session.SetString("Lname", currentuser.Lname);
                        HttpContext.Session.SetString("Role", currentuser.Role);
                        return RedirectToPage("/Index");

                    }
                    else
                    {
                        MyMessage = "Invalid Email or Password";
                        return Page();
                    }
                }
                else
                {
                    MyMessage = "Account is locked!";
                    return Page();
                }

            }
            return Page();
        }
    }
}
