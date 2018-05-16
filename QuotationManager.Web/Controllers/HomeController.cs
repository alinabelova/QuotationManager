﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuotationManager.DataAccess.Repository;
using QuotationManager.Models;
using QuotationManager.Web.Models;

namespace QuotationManager.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly GuestDataService _guestDataService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Quota> _quotaRepository;
        public HomeController(UserManager<ApplicationUser> userManager, IRepository<Quota> quotaRepository)
        {
            _userManager = userManager;
            _quotaRepository = quotaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
