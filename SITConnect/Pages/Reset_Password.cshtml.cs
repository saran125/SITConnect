using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SITConnect.Models;
using SITConnect.Services;

namespace SITConnect.Pages
{
    public class Reset_PasswordModel : PageModel
    {
        private readonly ILogger<Reset_PasswordModel> _logger;
        private UserService _svc;
        public Reset_PasswordModel(ILogger<Reset_PasswordModel> logger, UserService service)
        {
            _logger = logger;
            _svc = service;

        }
        [BindProperty]
        public User user { get; set; }
        public void OnGet()
        {
        }

    }
}
