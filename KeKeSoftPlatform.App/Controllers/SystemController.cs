using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using System.Reflection;
using System.Data.Entity;

namespace KeKeSoftPlatform.App.Controllers
{
    public class SystemController : BaseController
    {
        public ActionResult Modules()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.Module.Include(m => m.Modules).OrderByDescending(m => m.Sequence).ToList());
            }
        }

        public PartialViewResult CreateModule()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateModule(CreateModuleData model)
        {
            if (ModelState.IsValid == false)
            {
                return PartialView(model);
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Module.Add(new T_Module
                {
                    Name = model.Name,
                    ParentId = model.ParentId,
                    Url = model.Url,
                    Sequence = db.Module.Where(m => m.ParentId == model.ParentId).Max(m => m.Sequence) + 1
                });
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Json(new ReturnValue { IsSuccess = true });
            }
        }

        //public ActionResult EditModule(Guid moduleId, string returnUrl = "/system/Modules")
        //{
        //    using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
        //    {
        //        var module = db.Module.Find(moduleId);
        //        return View(new EditModuleData
        //        {
        //            ReturnUrl = returnUrl,
        //            ModuleId = moduleId,
        //            Number = module.Number,
        //            Name = module.Name,
        //            AdminNumber = module.AdminNumber,
        //            CompanyName = module.CompanyName,
        //            CompanyNumber = module.CompanyNumber,
        //            RepresentativeName = module.RepresentativeName,
        //            RepresentativeIDNumber = module.RepresentativeIDNumber,
        //            ContactName = module.ContactName,
        //            ContactPhone = module.ContactPhone,
        //            BalanceName = module.BalanceName,
        //            BalanceBankArea = module.BalanceBankArea,
        //            BalanceNumber = module.BalanceNumber,
        //            BalanceBank = module.BalanceBank,
        //            BalanceType = module.BalanceType
        //        });
        //    }
        //}

        //[HttpPost]
        //public ActionResult EditModule(EditModuleData model)
        //{
        //    if (ModelState.IsValid == false)
        //    {
        //        return View(model);
        //    }

        //    using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
        //    {
        //        var module = db.Module.Find(model.ModuleId);

        //        module.Name = model.Name;
        //        module.CompanyName = model.CompanyName;
        //        module.CompanyNumber = model.CompanyNumber;
        //        module.RepresentativeName = model.RepresentativeName;
        //        module.RepresentativeIDNumber = model.RepresentativeIDNumber;
        //        module.ContactName = model.ContactName;
        //        module.ContactPhone = model.ContactPhone;
        //        module.BalanceName = model.BalanceName;
        //        module.BalanceBankArea = model.BalanceBankArea;
        //        module.BalanceNumber = model.BalanceNumber;
        //        module.BalanceBank = model.BalanceBank;
        //        module.BalanceType = model.BalanceType;

        //        db.SaveChanges();
        //        TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
        //        return Redirect(model.ReturnUrl);
        //    }
        //}

        public ActionResult DeleteModule(Guid moduleId, string returnUrl = "/System/Modules")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Module.Remove(db.Module.Find(moduleId));
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(returnUrl);
            }
        }
    }
}
