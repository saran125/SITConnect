using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SITConnect.Models;

using SITConnect.Services;

namespace SITConnect.Pages
{
    public class registerModel : PageModel
    {
        private readonly ILogger<registerModel> _logger;
        private UserService _svc;
        public registerModel(ILogger<registerModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;
       
        }

        [BindProperty]
        public User theuser { get; set; }
        [BindProperty]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
           ErrorMessage = "Password Need to be at least 12 characters, combination of lower case, upper case, numbers & special characters")]
        public string Cfmpwd { get; set; }
        [BindProperty]
        public string MyMessage { get; set; }
        
        public void OnGet()
        {
        }

        [ValidateReCaptcha]
        public IActionResult OnPost()
        {

            if (ModelState.IsValid)
            {
               
                if (Cfmpwd == theuser.Password)
                {

                    if (_svc.Register(theuser))
                    {
                        return Redirect("/TwoFactorAuthenticationController?Id=" + theuser.Id);
                    }
                    else
                    {
                        MyMessage = "User already exist!";
                        return Page();
                    }
                }
                else
                {
                    MyMessage = "The Password is not similar!";
                    return Page();
                }
            }
            return Page();
        }
    }
}
