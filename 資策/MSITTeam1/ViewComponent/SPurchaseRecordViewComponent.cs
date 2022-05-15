﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSITTeam1.ViewComponent
{
    [Microsoft.AspNetCore.Mvc.ViewComponent]
    public class SPurchaseRecordViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly helloContext hello;
        public SPurchaseRecordViewComponent(helloContext _hello)
        {
            hello = _hello;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.Name = CDictionary.username;
            ViewBag.Type = CDictionary.memtype;
            ViewBag.account = CDictionary.account;
           var  Odatas = (hello.TProductOrders.Where(p => p.MemberId == CDictionary.account)).OrderByDescending(p=>p.Date).ToList();
            var Cdatas = (hello.TClassOrders.Where(p => p.MemberId == CDictionary.account)).OrderByDescending(p => p.Date).ToList();
            var OdatasD =( from d in hello.TProductOrderDetails
                          join o in hello.TProductOrders on d.OrderId equals o.OrderId
                          select d).OrderBy(c => c.ProductId).ToList();
            var CdatasD = (from d in hello.TClassOrderDetails
                           join o in hello.TClassOrders on d.OrderId equals o.OrderId
                           select d).ToList();
            var pro = (from p in hello.TProducts
                       join d in hello.TProductOrderDetails on p.ProductId equals d.ProductId
                       select p).OrderBy(c => c.ProductId).ToList();
            var classN = (from p in hello.TClassInfos
                          join d in hello.TClassOrderDetails on p.FClassExponent equals d.ClassExponent
                          select p).OrderBy(c => c.FClassExponent).ToList();
            if (Odatas != null||Cdatas!=null)
            {
                OrderAndOrderDetailViewModel list = new OrderAndOrderDetailViewModel();
                List<CheckOutViewModel> temp = new List<CheckOutViewModel>();
                List<OrderDetailViewModel> tempDetail = new List<OrderDetailViewModel>();
                List<CProductViewModel> tempPro = new List<CProductViewModel>();
                List<ClassCheckOutViewModel> classTemp = new List<ClassCheckOutViewModel>();
                List<ClassOrderDetailViewModel> classTempDetail = new List<ClassOrderDetailViewModel>();
                //list.order = Odatas.ToList();
                //list.orderDetail = OdatasD.ToList();
                //若OrderAndOrderDetailViewModel裡面是包資料表可直接使用上面寫法
                foreach (TProductOrder t in Odatas)
                {
                    temp.Add(new CheckOutViewModel { order = t });
                }
                foreach (TProductOrderDetail t in OdatasD)
                {
                    tempDetail.Add(new OrderDetailViewModel { orderDetail = t });
                }
                foreach (TProduct t in pro)
                {
                    tempPro.Add(new CProductViewModel { prodcut = t });
                }
                foreach (TClassOrder t in Cdatas)
                {
                    classTemp.Add(new ClassCheckOutViewModel { classOrder = t });
                }
                foreach (TClassOrderDetail t in CdatasD)
                {
                    classTempDetail.Add(new ClassOrderDetailViewModel { classOrderDetail = t });
                }
                list.order = temp;
                list.orderDetail = tempDetail;
                list.product = tempPro;
                list.ClassInfo = classN.ToList();
                list.classOrder = classTemp;
                list.classOrderDetail = classTempDetail;
                return View(list);
                //return new HtmlContentViewComponentResult(new HtmlString("<tbody>< tr>< th ></ th >< td colspan='4'></ td ></ tr ></ tbody >"));
            }
                return Content("沒東西");
        }
    }
}
