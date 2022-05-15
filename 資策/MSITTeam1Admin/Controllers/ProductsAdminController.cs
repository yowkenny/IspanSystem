﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSITTeam1Admin.Models;
using MSITTeam1Admin.ViewModels;

namespace MSITTeam1Admin.Controllers
{
    public class ProductsAdminController : Controller
    {
        private readonly helloContext hello;
        IWebHostEnvironment _enviroment;

        public ProductsAdminController(helloContext _hello, IWebHostEnvironment p)
        {
            hello = _hello;
            _enviroment = p;
        }
        public IActionResult Index()
        {
            IEnumerable<TProduct> datas = null;
            datas = from t in hello.TProducts
                    select t;
            List<CProductAdminViewModel> list = new List<CProductAdminViewModel>();
            foreach (TProduct t in datas)
            {
                list.Add(new CProductAdminViewModel() { prodcut = t });
            }
            return View(list);
        }

        // GET: Products/Create

        public IActionResult Create()
        {
            var id = hello.TProducts.Max(t => t.ProductId);
            if (id != null)
            {
                int pid = int.Parse(id.Substring(1, 8)) + 1;
                ViewBag.productId = $"P" + pid.ToString("00000000");
            }
            else
            {
                ViewBag.productId = "P00000001";
            }
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CProductAdminViewModel p)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (p.photo != null)
                    {
                        string photoName = Guid.NewGuid().ToString() + ".jpg";
                        //Cloudinary api https://mlog.club/article/5681377
                        var myAccount = new Account { ApiKey = "756611421435346", ApiSecret = "q9LV4-S7fbX7leQjNUZjfEDcjMs", Cloud = "ispansystem" };
                        Cloudinary cloudinary = new Cloudinary(myAccount);

                        using (var memory = new MemoryStream()) { 
                       
                        await p.photo.CopyToAsync(memory);
                        memory.Position = 0;// set cursor to the beginning of the stream.

                        ImageUploadParams uploadParams = new ImageUploadParams();
                        uploadParams.File = new FileDescription(photoName, memory);
                        ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
                        var url = uploadResult.SecureUrl.ToString();
                        p.ImgPath = url;
                        }
                    }
                    else
                    {
                        p.ImgPath = "noImg.jpg";
                    }
                    hello.Add(p.prodcut);
                    await hello.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View("Create");
        }

        // GET: Products/Edit/5

        public IActionResult Edit(string id)
        {
            if (id != null)
            {
                TProduct prod = hello.TProducts.FirstOrDefault(c => c.ProductId == id.ToString());
                if (prod != null)
                {
                    return View(new CProductAdminViewModel() { prodcut = prod });
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CProductAdminViewModel p)
        {
            TProduct prod = hello.TProducts.FirstOrDefault(c => c.ProductId == p.ProductId);
            if (prod != null)
            {
                if (p.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    var myAccount = new Account { ApiKey = "756611421435346", ApiSecret = "q9LV4-S7fbX7leQjNUZjfEDcjMs", Cloud = "ispansystem" };
                    Cloudinary cloudinary = new Cloudinary(myAccount);

                    using (var memory = new MemoryStream())
                    {

                        await p.photo.CopyToAsync(memory);
                        memory.Position = 0;// set cursor to the beginning of the stream.

                        ImageUploadParams uploadParams = new ImageUploadParams();
                        uploadParams.File = new FileDescription(photoName, memory);
                        ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
                        var url = uploadResult.SecureUrl.ToString();
                        p.ImgPath = url;
                        prod.ImgPath = p.ImgPath;
                    }
                    //else
                    //{
                    //    prod.ImgPath = "noImg.jpg";
                    //}
                }
                prod.ProductId = p.ProductId;
                prod.Name = p.Name;
                prod.Price = p.Price;
                prod.Cost = p.Cost;
                prod.Barcode = p.Barcode;
                prod.Description = p.Description;
                hello.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(string id)
        {
            if (id != null)
            {
                TProduct prod = hello.TProducts.FirstOrDefault(c => c.ProductId == id.ToString());
                if (prod != null)
                {
                    return View(new CProductAdminViewModel() { prodcut = prod });
                }
            }
            return RedirectToAction("Index");
        }

        // POST: Products/Delete/5
        [HttpPost]
        public IActionResult Delete(CProductAdminViewModel vModel)
        {
            TProduct prod = hello.TProducts.FirstOrDefault(t => t.ProductId == vModel.ProductId);
            if (prod != null)
            {
                hello.TProducts.Remove(prod);
                hello.SaveChanges();
            }
            //var tProduct = await hello.TProducts.FindAsync(id);
            //hello.TProducts.Remove(tProduct);
            //await hello.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TProductExists(string id)
        {
            return hello.TProducts.Any(e => e.ProductId == id);
        }
    }
}
