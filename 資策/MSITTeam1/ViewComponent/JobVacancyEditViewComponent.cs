﻿using Microsoft.AspNetCore.Mvc;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSITTeam1.ViewComponent
{
    [Microsoft.AspNetCore.Mvc.ViewComponent]
    public class JobVacancyEditViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly helloContext hello;
        public JobVacancyEditViewComponent(helloContext _hello)
        {
            hello = _hello;
        }
        public IViewComponentResult Invoke(int id = 0)
        {
            if (id != 0) 
            {
                TNewJobVacancy job = hello.TNewJobVacancies.FirstOrDefault(p => p.Fid == id);
                if(job != null)
                {
                    CJobVacancyViewModel c = new CJobVacancyViewModel() { job = job };
                    ViewBag.fid = id;
                    return View(c);
                }
            }
            return View();
        }
    }
}
