﻿using Microsoft.AspNetCore.Mvc;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSITTeam1.Controllers
{
    public class CCompanyBasicController : Controller
    {
        private readonly helloContext hello;

        public CCompanyBasicController(helloContext _hello)
        {
            hello = _hello;
        }
        public IActionResult Index(string taxid)
        {
            ViewBag.account = CDictionary.account;
            ViewBag.type = CDictionary.memtype;
            ViewBag.name = CDictionary.username;
            var comm = (from p in hello.TPhotos where p.FAccount == taxid
                       join t in hello.TCompanyBasics on taxid equals t.CompanyTaxid into pt
                       from combin in pt.DefaultIfEmpty()
                       select new CCompanyBasicWithPhotoViewModelcs()
                       {
                           photo = p,
                           combasic = combin,
                  }).ToList();
            return View(comm);
        }
    }
}
