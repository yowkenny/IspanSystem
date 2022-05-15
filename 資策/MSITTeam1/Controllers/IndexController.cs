﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSITTeam1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSITTeam1.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Name = CDictionary.username;
            ViewBag.account = CDictionary.account;
            if (CDictionary.username != null)
            {
                ViewBag.Name = CDictionary.username.Trim();
            }
           
            
            ViewBag.Type = CDictionary.memtype;
            return View();
        }
    }
}
