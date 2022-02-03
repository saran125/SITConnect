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
        public User theuser { get; set; }
        
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
                    MyMessage = "Account is locked!";
                    return Page();
                }

            }
            return Page();
        }
    }
}
