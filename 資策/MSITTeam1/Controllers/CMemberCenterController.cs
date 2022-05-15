﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MSITTeam1.Controllers
{
    public class CMemberCenterController : Controller
    {
        private readonly helloContext hello;
        IWebHostEnvironment _enviroment;

        public CMemberCenterController(helloContext _hello, IWebHostEnvironment p )
        {
            hello = _hello;
            _enviroment = p;
        }
        public IActionResult Index()
        {
            ViewBag.Name = CDictionary.username;
            ViewBag.Type = CDictionary.memtype;
            ViewBag.account = CDictionary.account;
            return View();
        }


        //更新公司基本資料
        [HttpPost]
        public IActionResult CompanyInformationEdit(CCompanyBasicViewModel company)
        {
            TCompanyBasic c = hello.TCompanyBasics.FirstOrDefault(p => p.CompanyTaxid == company.CompanyTaxid);
            TCityContrast city = hello.TCityContrasts.FirstOrDefault(p => p.FPostCode.ToString() == company.FDistrict);
            if (c != null)
            {
                c.FAddress = company.FAddress;
                c.FName = company.FName;
                c.FCity = company.FCity;
                c.FDistrict = city.FDistrictName;
                c.FEmail = company.FEmail;
                c.FFaxCode = company.FFaxCode;
                c.FFax = company.FFax;
                c.FPhoneCode = company.FPhoneCode;
                c.FPhone = company.FPhone;
                c.FContactPerson = company.FContactPerson;
                c.FBenefits = company.FBenefits;
                c.FCustomInfo = company.FCustomInfo;
                c.FCapitalAmount = company.FCapitalAmount;
                c.FWebsite = company.FWebsite;
                c.FRelatedLink = company.FRelatedLink;
                c.FDistrictCode = city.FPostCode.ToString();
                hello.SaveChanges();
            }
            return Content("success");
        }

        //LOGO存取
        [HttpPost]
        public IActionResult SaveLogoPicture(IFormFile img,string id)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        if (p.photo != null)
            //        {
            //            string photoName = Guid.NewGuid().ToString() + ".jpg";
            //            //Cloudinary api https://mlog.club/article/5681377
            //            var myAccount = new Account { ApiKey = "756611421435346", ApiSecret = "q9LV4-S7fbX7leQjNUZjfEDcjMs", Cloud = "ispansystem" };
            //            Cloudinary cloudinary = new Cloudinary(myAccount);

            //            using (var memory = new MemoryStream())
            //            {

            //                await p.photo.CopyToAsync(memory);
            //                memory.Position = 0;// set cursor to the beginning of the stream.

            //                ImageUploadParams uploadParams = new ImageUploadParams();
            //                uploadParams.File = new FileDescription(photoName, memory);
            //                ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            //                var url = uploadResult.SecureUrl.ToString();
            //                p.ImgPath = url;
            //            }
            //        }
            //        else
            //        {
            //            p.ImgPath = "noImg.jpg";
            //        }
            //        hello.Add(p.prodcut);
            //        await hello.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
            TCompanyBasic c = hello.TCompanyBasics.FirstOrDefault(p => p.CompanyTaxid == id);
            string photoName = Guid.NewGuid().ToString() + ".jpg";
            c.FLogo = photoName;
            img.CopyTo(new FileStream(
                _enviroment.WebRootPath + @"\images\company\" + photoName, FileMode.Create));
            hello.SaveChanges();
            return Json(new { suc="ok"});
        }

        [HttpPost]
        public IActionResult SaveFile(IFormFile file,string id)
        {
            var ph = (from p in hello.TPhotos where p.FAccount == id select p).Count();
            if (ph > 7)
            {
                return Json(new { err = "超過張數"});
            }
            string photoName = Guid.NewGuid().ToString() + ".jpg";
            using (FileStream fs = new FileStream(_enviroment.WebRootPath + @"\images\company\" + photoName,FileMode.Create))
            {
                file.CopyTo(fs);
                TPhoto photo = new TPhoto()
                {
                    FAccount = id,
                    FPhoto = photoName,
                };
                hello.TPhotos.Add(photo);
                hello.SaveChanges();
            }
            return Json(new { suc = "上傳成功"});
        }
        [HttpPost]
        public IActionResult DeleteFile()
        {
            Array filename = Request.Form["filename[]"];
            if (filename != null)
            {
                foreach(var i in filename)
                {
                    FileInfo file = new FileInfo(_enviroment.WebRootPath + @"\images\company\" + i);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    TPhoto photo = hello.TPhotos.FirstOrDefault(p => p.FPhoto == i.ToString());
                    if (photo != null)
                    {
                        hello.TPhotos.Remove(photo);
                    }
                }
                hello.SaveChanges();
                return Content("Deleted");
            }
            return Content("fail");
        }
        public IActionResult CreateJobVacancy(TNewJobVacancy joblist)
        {
            var job = hello.TNewJobVacancies.FirstOrDefault(p => p.Fid == joblist.Fid);
            if(job == null)
            {
                joblist.FCreatTime = DateTime.Now.ToString("yyyy-MM-dd");
                var district = hello.TCityContrasts.FirstOrDefault(p => p.FPostCode.ToString() == joblist.FDistrict);
                joblist.FDistrict = district.FDistrictName;
                hello.TNewJobVacancies.Add(joblist);
                hello.SaveChanges();
                return Json(new { suc = "新增成功" });
            }
            return Json(new { fail="已有相同職缺"});
        }

        public IActionResult JobVacancyEdit(CJobVacancyViewModel jobedit)
        {
            if(jobedit != null)
            {
                //helloContext h = new helloContext();
                TNewJobVacancy job = hello.TNewJobVacancies.FirstOrDefault(p => p.Fid == jobedit.Fid);
                TCityContrast city = hello.TCityContrasts.FirstOrDefault(p => p.FPostCode.ToString() == jobedit.FDistrict);
                if (job != null)
                {
                    job.FCity = jobedit.FCity;
                    job.FCompanyTaxid = jobedit.FCompanyTaxid;
                    job.FContactEmail = jobedit.FContactEmail;
                    job.FContactFax = jobedit.FContactFax;
                    job.FContactPerson = jobedit.FContactPerson;
                    job.FContactPhone = jobedit.FContactPhone;
                    job.FCreatTime = jobedit.FCreatTime;
                    job.FDistrict = city.FDistrictName;
                    job.FEducation = jobedit.FEducation;
                    job.FEmployeeType = jobedit.FEmployeeType;
                    job.Fid = jobedit.Fid;
                    job.FJobListId = jobedit.FJobListId;
                    job.FJobName = jobedit.FJobName;
                    job.FJobSkill = jobedit.FJobSkill;
                    job.FJobStatus = jobedit.FJobStatus;
                    job.FLeaveSystem = jobedit.FLeaveSystem;
                    job.FNeedPerson = jobedit.FNeedPerson;
                    job.FOther = jobedit.FOther;
                    job.FSalaryMax = jobedit.FSalaryMax;
                    job.FSalaryMin = jobedit.FSalaryMin;
                    job.FSalaryMode = jobedit.FSalaryMode;
                    job.FWorkAddress = jobedit.FWorkAddress;
                    job.FWorkExp = jobedit.FWorkExp;
                    job.FWorkHours = jobedit.FWorkHours;
                    job.FModifyTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd tt HH:mm:ss"));
                    hello.SaveChanges();
                    return ViewComponent("JobVacancy");
                    //return Json(jobedit);
                    //return Json(new { suc = "更新成功" });
                }
            }
            return PartialView("JobVacancy");
        }
        [HttpPost]
        public IActionResult JobVacancyDelete(int id)
        {
            TNewJobVacancy job = hello.TNewJobVacancies.FirstOrDefault(p => p.Fid == id);
            if (job != null)
            {
                hello.TNewJobVacancies.Remove(job);
                hello.SaveChanges();
                return Content("Deleted");
            }
            return Content("fail");
        }

    }
}
