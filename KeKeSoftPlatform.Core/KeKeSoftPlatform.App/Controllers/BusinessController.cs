using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Zip;

namespace KeKeSoftPlatform.App.Controllers
{
    public class BusinessController : BaseController
    {
        #region 桥梁管理
        [Menu("桥梁管理")]
        public ActionResult Bridge(string key, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.Bridge.Include(m => m.CreateEmployee);
                if (string.IsNullOrWhiteSpace(key) == false)
                {
                    query = query.Where(m => m.Number == key || m.Name.Contains(key) || m.Address.Contains(key) || m.Description.Contains(key));
                }
                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        [MenuAction("/business/bridge", "create", "添加")]
        public ActionResult CreateBridge(string returnUrl = "/business/Bridge")
        {
            return View(new CreateBridgeData { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public ActionResult CreateBridge(CreateBridgeData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Bridge.Add(new T_Bridge
                {
                    Id = PF.Key(),
                    Address = model.Address,
                    CreateDate = DateTime.Now,
                    CreateEmployeeId = _User.Value.Id,
                    Description = model.Description,
                    IsStatic = false,
                    LiMianTu = model.LiMianTu,
                    Name = model.Name,
                    Number = model.Number,
                    PingMianTu = model.PingMianTu,
                    YuDiTieGuanXiTu = model.YuDiTieGuanXiTu,
                    JiBenJieGouXingShi = model.JiBenJieGouXingShi,
                    LianItemsJsonString = model.LianItemsJsonString,
                    DunTaiItemsJsonString = model.DunTaiItemsJsonString
                });

                db.SaveChanges();

                return Redirect(model.ReturnUrl);
            }
        }

        [MenuAction("/business/bridge", "edit", "编辑")]
        public ActionResult EditBridge(Guid bridgeId, string returnUrl = "/business/Bridge")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(bridgeId);
                return View(new EditBridgeData
                {
                    BridgeId = bridge.Id,
                    Address = bridge.Address,
                    Description = bridge.Description,
                    LiMianTu = bridge.LiMianTu,
                    Name = bridge.Name,
                    Number = bridge.Number,
                    ReturnUrl = returnUrl,
                    JiBenJieGouXingShi = bridge.JiBenJieGouXingShi,
                    YuDiTieGuanXiTu = bridge.YuDiTieGuanXiTu,
                    PingMianTu = bridge.PingMianTu
                });
            }
        }

        [HttpPost]
        public ActionResult EditBridge(EditBridgeData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(model.BridgeId);
                bridge.Address = model.Address;
                bridge.Description = model.Description;
                bridge.LiMianTu = model.LiMianTu;
                bridge.Name = model.Name;
                bridge.Number = model.Number;
                bridge.PingMianTu = model.PingMianTu;
                bridge.YuDiTieGuanXiTu = model.YuDiTieGuanXiTu;
                bridge.JiBenJieGouXingShi = model.JiBenJieGouXingShi;

                db.SaveChanges();

                return Redirect(model.ReturnUrl);
            }
        }

        public ActionResult BridgeDetail(Guid bridgeId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.Bridge.Find(bridgeId));
            }
        }

        [MenuAction("/business/bridge", "delete", "删除")]
        public ActionResult DeleteBridge(Guid bridgeId, string returnUrl = "/business/Bridge")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(bridgeId);
                db.Bridge.Remove(bridge);
                db.SaveChanges();
                return Redirect(returnUrl);
            }
        }

        public FileResult DownloadLiMianTu(Guid bridgeId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(bridgeId);
                if (string.IsNullOrWhiteSpace(bridge.LiMianTu))
                {
                    throw new Exception("文件不存在");
                }
                var fullPath = MultipleDownload(JsonConvert.DeserializeObject<List<string>>(bridge.LiMianTu));
                return Download(fullPath);
            }
        }

        [MenuAction("/business/bridge", "marker", "标记")]
        public ActionResult CreateBridgeMap(Guid bridgeId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.Bridge.Find(bridgeId));
            }
        }

        [HttpPost]
        public JsonNetResult CreateBridgeMap(Guid bridgeId, string marker)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(bridgeId);
                bridge.Marker = marker;
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public ActionResult BridgeMap()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(new ChuanGanQiStatusHuiZong
                {
                    Green = db.ChuanGanQiBaseInfo.Count(m => m.Status == ChuanGanQiStatus.Green),
                    Red = db.ChuanGanQiBaseInfo.Count(m => m.Status == ChuanGanQiStatus.Red),
                    Yellow = db.ChuanGanQiBaseInfo.Count(m => m.Status == ChuanGanQiStatus.Yellow)
                });
            }
        }

        public JsonNetResult QueryBridgeMap()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = db.Bridge
                            .Where(m => m.Marker != null)
                            .Select(m => new
                            {
                                m.Name,
                                m.Marker,
                                m.Id
                            })
                            .ToList()
                            .Select(m => new
                            {
                                id = m.Id,
                                name = m.Name,
                                marker = Newtonsoft.Json.JsonConvert.DeserializeObject<Position>(m.Marker)
                            })
                            .ToList()
                });
            }
        }

        [HttpPost]
        public JsonNetResult ChangeBridge(Guid bridgeId)
        {
            BridgeMananger.ChangeBridge(bridgeId);
            return JsonNet(new ReturnValue { IsSuccess = true });
        }
        #endregion


        #region 传感器基础数据
        public JsonNetResult QueryBridge(string key)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.Bridge.AsQueryable();
                if (string.IsNullOrWhiteSpace(key) == false)
                {
                    query = query.Where(m => m.Number == key || m.Name.Contains(key) || m.Description.Contains(key));
                }
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = query.ToList().Select(m => new
                    {
                        id = m.Id,
                        number = m.Number,
                        name = m.Name,
                        description = m.Description,
                        lianItems = (string.IsNullOrWhiteSpace(m.LianItemsJsonString) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(m.LianItemsJsonString)),
                        dunTaiItems = (string.IsNullOrWhiteSpace(m.DunTaiItemsJsonString) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(m.DunTaiItemsJsonString))
                    }).ToList()
                });
            }
        }


        [Menu("传感器基础数据")]
        [BridgeRequired]
        public ActionResult ChuanGanQiBaseInfo(string key, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.ChuanGanQiBaseInfo.Where(m => m.BridgeId == BridgeMananger.CurrentBridgeId);
                if (string.IsNullOrWhiteSpace(key) == false)
                {
                    query = query.Where(m => m.CeDianNumber.Contains(key) || m.Name.Contains(key) || m.Number.Contains(key));
                }
                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        [MenuAction("/business/ChuanGanQiBaseInfo", "create", "添加")]
        public ActionResult CreateChuanGanQiBaseInfo(string returnUrl = "/business/ChuanGanQiBaseInfo")
        {
            return View(new CreateChuanGanQiBaseInfoData { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public ActionResult CreateChuanGanQiBaseInfo(CreateChuanGanQiBaseInfoData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = new T_ChuanGanQiBaseInfo
                {
                    Id = PF.Key(),
                    CreateDate = DateTime.Now,
                    CreateEmployeeId = _User.Value.Id,
                    Address = model.Address,
                    CeDianNumber = model.CeDianNumber,
                    Name = model.Name,
                    Number = model.Number,
                    ChuCeDate = model.ChuCeDate,
                    ChuCeValue = model.ChuCeValue,
                    BridgeId = db.Bridge.Single(m => m.Number == model.BridgeNumber).Id,
                    Waring = false,
                    Left = 100,
                    Top = 100,
                    DunTai = model.DunTai,
                    FenBianLv = model.FenBianLv,
                    Group = model.Group,
                    JingDu = model.JingDu,
                    Lian = model.Lian,
                    LiangCheng = model.LiangCheng,
                    VerticalPosition = model.VerticalPosition,
                    XingHao = model.XingHao,
                    Status = ChuanGanQiStatus.Green,
                    IsJiZhunDian=model.IsJiZhunDian
                };
                db.ChuanGanQiBaseInfo.Add(bridge);

                KeKeSoftPlatform.Core.BridgeConfig.Serialize(new Core.BridgeConfig
                {

                },bridge.Id);

                db.SaveChanges();

                return Redirect(model.ReturnUrl);
            }
        }

        [MenuAction("/business/ChuanGanQiBaseInfo", "edit", "传感器基础数据修改")]
        public ActionResult EditChuanGanQiBaseInfo(Guid chuanGanQiBaseInfoId, string returnUrl = "/business/CreateChuanGanQiBaseInfo")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId);
                return View(new EditChuanGanQiBaseInfoData
                {
                    Warning = chuanGanQiBaseInfo.Waring,
                    ChuanGanQiBaseInfoId = chuanGanQiBaseInfo.Id,
                    BridgeNumber = chuanGanQiBaseInfo.Bridge.Number,
                    Address = chuanGanQiBaseInfo.Address,
                    Name = chuanGanQiBaseInfo.Name,
                    Number = chuanGanQiBaseInfo.Number,
                    ReturnUrl = returnUrl,
                    CeDianNumber = chuanGanQiBaseInfo.CeDianNumber,
                    ChuCeDate = chuanGanQiBaseInfo.ChuCeDate,
                    ChuCeValue = chuanGanQiBaseInfo.ChuCeValue,
                    XingHao = chuanGanQiBaseInfo.XingHao,
                    Group = chuanGanQiBaseInfo.Group,
                    VerticalPosition = chuanGanQiBaseInfo.VerticalPosition,
                    DunTai = chuanGanQiBaseInfo.DunTai,
                    Lian = chuanGanQiBaseInfo.Lian,
                    FenBianLv = chuanGanQiBaseInfo.FenBianLv,
                    JingDu = chuanGanQiBaseInfo.JingDu,
                    LiangCheng = chuanGanQiBaseInfo.LiangCheng,
                    IsJiZhunDian = chuanGanQiBaseInfo.IsJiZhunDian
                });
            }
        }

        [HttpPost]
        public ActionResult EditChuanGanQiBaseInfo(EditChuanGanQiBaseInfoData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(model.ChuanGanQiBaseInfoId);
                chuanGanQiBaseInfo.BridgeId = db.Bridge.Single(m => m.Number == model.BridgeNumber).Id;
                chuanGanQiBaseInfo.Address = model.Address;
                chuanGanQiBaseInfo.Name = model.Name;
                chuanGanQiBaseInfo.Number = model.Number;
                chuanGanQiBaseInfo.CeDianNumber = model.CeDianNumber;
                chuanGanQiBaseInfo.ChuCeDate = model.ChuCeDate;
                chuanGanQiBaseInfo.ChuCeValue = model.ChuCeValue;
                chuanGanQiBaseInfo.XingHao = model.XingHao;
                chuanGanQiBaseInfo.Group = model.Group;
                chuanGanQiBaseInfo.VerticalPosition = model.VerticalPosition;
                chuanGanQiBaseInfo.DunTai = model.DunTai;
                chuanGanQiBaseInfo.Lian = model.Lian;
                chuanGanQiBaseInfo.FenBianLv = model.FenBianLv;
                chuanGanQiBaseInfo.JingDu = model.JingDu;
                chuanGanQiBaseInfo.LiangCheng = model.LiangCheng;
                chuanGanQiBaseInfo.IsJiZhunDian = model.IsJiZhunDian;

                db.SaveChanges();

                return Redirect(model.ReturnUrl);
            }
        }

        public ActionResult ChuanGanQiBaseInfoDetail(Guid chuanGanQiBaseInfoId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.ChuanGanQiBaseInfo.Include(m => m.Bridge).Single(m => m.Id == chuanGanQiBaseInfoId));
            }
        }

        [MenuAction("/business/ChuanGanQiBaseInfo", "delete", "传感器基础数据删除")]
        public ActionResult DeleteChuanGanQiBaseInfo(Guid chuanGanQiBaseInfoId, string returnUrl = "/business/CreateChuanGanQiBaseInfo")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId);
                db.ChuanGanQiBaseInfo.Remove(chuanGanQiBaseInfo);
                db.SaveChanges();
                return Redirect(returnUrl);
            }
        }

        [MenuAction("/business/ChuanGanQiBaseInfo", "position", "位置")]
        public ActionResult CreateChuanGanQiPosition(Guid chuanGanQiBaseInfoId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Include(m => m.Bridge).Single(m => m.Id == chuanGanQiBaseInfoId);
                if (string.IsNullOrWhiteSpace(chuanGanQiBaseInfo.Bridge.LiMianTu) || JsonConvert.DeserializeObject<List<string>>(chuanGanQiBaseInfo.Bridge.LiMianTu).Any() == false)
                {
                    TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("该桥梁没有立面图，请上传");
                    return RedirectToAction("EditBridge", new { bridgeId = chuanGanQiBaseInfo.Bridge.Id, returnUrl = $"/business/CreateChuanGanQiPosition?chuanGanQiBaseInfoId={chuanGanQiBaseInfoId}" });
                }
                return View(db.ChuanGanQiBaseInfo.Include(m => m.Bridge).Single(m => m.Id == chuanGanQiBaseInfoId));
            }
        }

        [HttpPost]
        public JsonNetResult CreateChuanGanQiPosition(Guid chuanGanQiBaseInfoId, int left, int top)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId);
                chuanGanQiBaseInfo.Left = left;
                chuanGanQiBaseInfo.Top = top;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [Menu("传感器布设图")]
        public ActionResult ChuanGanQiPosition(Guid? bridgeId)
        {
            if (bridgeId.HasValue)
            {
                BridgeMananger.ChangeBridge(bridgeId.Value);
            }
            if (BridgeMananger.CurrentBridgeId.HasValue == false)
            {
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("请先在左上角选择桥梁，然后再继续此操作");
                return RedirectToAction("bridgeMap");
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(BridgeMananger.CurrentBridgeId.Value);
                if (bridge == null)
                {
                    TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("请添加桥梁");
                    return RedirectToAction("createBridge");
                }
                if (string.IsNullOrWhiteSpace(bridge.LiMianTu) || JsonConvert.DeserializeObject<List<string>>(bridge.LiMianTu).Any() == false)
                {
                    TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("请先上传立面图");
                    return RedirectToAction("EditBridge", new { bridgeId = bridge.Id, returnUrl = "/business/ChuanGanQiPosition?bridgeId=" + bridge.Id });
                }
                return View(new ChuanGanQiStatusHuiZong
                {
                    Green = bridge.ChuanGanQiBaseInfos.Count(m => m.Status == ChuanGanQiStatus.Green),
                    Red = bridge.ChuanGanQiBaseInfos.Count(m => m.Status == ChuanGanQiStatus.Red),
                    Yellow = bridge.ChuanGanQiBaseInfos.Count(m => m.Status == ChuanGanQiStatus.Yellow)
                });
            }
        }

        public ActionResult QueryChuanGanQiPosition()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var bridge = db.Bridge.Find(BridgeMananger.CurrentBridgeId.Value);
                if (string.IsNullOrWhiteSpace(bridge.LiMianTu) || JsonConvert.DeserializeObject<List<string>>(bridge.LiMianTu).Any() == false)
                {
                    return JsonNet(new ReturnValue { IsSuccess = false, Error = "立面图不存在" });
                }
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new
                    {
                        liMianTu = JsonConvert.DeserializeObject<List<string>>(bridge.LiMianTu)[0],
                        chuanGanQiItems = db.ChuanGanQiBaseInfo.Where(m => m.BridgeId == bridge.Id && m.IsJiZhunDian == false).Select(m => new
                        {
                            id = m.Id,
                            name = m.Name,
                            left = m.Left,
                            top = m.Top,
                            number = m.Number,
                            address = m.Address,
                            status = m.Status
                        }).ToList()
                    }
                });
            }
        }

        public ActionResult QueryChuanGanQiDataNewest(Guid chuanGanQiId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiData = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfoId == chuanGanQiId && m.ChenJiangSuLv != null).OrderByDescending(m => m.Id).FirstOrDefault();
                if (chuanGanQiData != null)
                {
                    return JsonNet(new ReturnValue
                    {
                        IsSuccess = true,
                        Data = new
                        {
                            Temperature = chuanGanQiData.LeiJiChenJiang,
                            ChenJiangSuLv = chuanGanQiData.ChenJiangSuLv,
                            ChenJiangChaZhi = chuanGanQiData.ChenJiangChaZhi,
                            chuanGanQiName=chuanGanQiData.ChuanGanQiBaseInfo.Name
                        }
                    });
                }
                else {
                    return JsonNet(new ReturnValue
                    {
                        IsSuccess = false
                    });
                }
            }
        }

        public ActionResult ChuanGanQiData(Guid chuanGanQiBaseInfoId, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var minDate = DateTime.Now.Date.AddDays(-3);
                var model = new ChuanGanQiData
                {
                    ChuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId),
                    Pager = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfoId == chuanGanQiBaseInfoId && m.TriggerDateTime >= minDate).OrderByDescending(m => m.TriggerDateTime).ThenByDescending(m=>m.Id).Page(pageNum)
                };
                return View(model);
            }
        }

        public JsonNetResult QueryChuanGanQiData(Guid chuanGanQiBaseInfoId)
        {
            var items = CacheProvider.Instance.Get(nameof(ChuanGanQiData), 1, () =>
            {
                using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                    var minDate = DateTime.Now.Date.AddDays(-3);
                    return db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfoId == chuanGanQiBaseInfoId && m.TriggerDateTime >= minDate).Select(m => new
                    {
                        triggerDateTime = m.TriggerDateTime,
                        leiJiChenJiang = m.LeiJiChenJiang,
                        chenJiangSuLv = m.ChenJiangSuLv,
                        chenJiangChaZhi = m.ChenJiangChaZhi
                    }).OrderBy(m => m.triggerDateTime).ToList();
                }
            });

            return JsonNet(new ReturnValue
            {
                IsSuccess = true,
                Data = items
            });
        }

        public ActionResult Seed()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var data = new List<T_ChuanGanQiData>();
                var minDate = DateTime.Now.Date.AddDays(-5);
                var random = new Random();
                for (int i = 0; i < 8000; i++)
                {
                    var date = minDate.AddMinutes(i);
                    var value = 0;
                    if (i < 2000)
                    {
                        value = random.Next(0, 3);
                    }
                    else if (i < 4000)
                    {
                        value = random.Next(0, 6);
                    }
                    else if (i < 6000)
                    {
                        value = random.Next(6, 9);
                    }
                    else
                    {
                        value = random.Next(0, 9);
                    }
                    data.Add(new T_ChuanGanQiData
                    {
                        ChuanGanQiBaseInfoId = Guid.Parse("8b22bad2-0a82-43f4-a463-a7653017559b"),
                        CreateDate = date,
                        LeiJiChenJiang = value,
                        Temperature = random.Next(-30, 30),
                        TriggerDateTime = date
                    });
                }

                db.ChuanGanQiData.AddRange(data);

                db.SaveChanges();
                return Content("完成");
            }
        }
        #endregion

        [Menu("警告处理结果模板")]
        public ActionResult ChuLiResultTemplate()
        {
            return View();
        }

        public JsonNetResult QueryChuLiResultTemplate()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = db.WarningResultTemplate.Select(m => new { id = m.Id, content = m.Content }).ToList()
                });
            }
        }

        [HttpPost]
        public JsonNetResult CreateChuLiResultTemplate(string content)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var entity = new T_WarningResultTemplate
                {
                    Id = PF.Key(),
                    Content = content
                };
                db.WarningResultTemplate.Add(entity);
                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true, Data = new { id = entity.Id, content = entity.Content } });
            }
        }

        [HttpPost]
        public JsonNetResult EditChuLiResultTemplate(Guid id, string content)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.WarningResultTemplate.Find(id).Content = content;
                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        public JsonNetResult DeleteChuLiResultTemplate(Guid id)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.WarningResultTemplate.Remove(db.WarningResultTemplate.Find(id));
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [BridgeRequired]
        [Menu("预警指标")]
        public ActionResult YuJingZhiBiao()
        {
            return View(KeKeSoftPlatform.Core.BridgeConfig.Instance(BridgeMananger.CurrentBridgeId.Value));
        }

        [Menu("桥梁参数设置")]
        [BridgeRequired]
        public ActionResult BridgeConfig()
        {
            return View(KeKeSoftPlatform.Core.BridgeConfig.Instance(BridgeMananger.CurrentBridgeId.Value));
        }

        [HttpPost]
        [BridgeRequired]
        [ValidateInput(false)]
        public ActionResult BridgeConfig(KeKeSoftPlatform.Core.BridgeConfig model)
        {
            KeKeSoftPlatform.Core.BridgeConfig.Serialize(model, BridgeMananger.CurrentBridgeId.Value);
            TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
            return RedirectToAction("BridgeConfig");
        }

        [Menu("上中下游趋势图")]
        [BridgeRequired]
        public ActionResult ChuanGanQiVerticalPositionRecord(DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                if (minDate.HasValue == false && maxDate.HasValue == false)
                {
                    return RedirectToAction("ChuanGanQiVerticalPositionRecord", new { minDate = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss"), maxDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") });
                }
                return View();
            }
        }

        public ActionResult QueryChuanGanQiVerticalPositionRecord(DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfo.BridgeId == BridgeMananger.CurrentBridgeId);
                var chuanGanQiItems = db.ChuanGanQiBaseInfo.Where(m => m.BridgeId == BridgeMananger.CurrentBridgeId).ToDictionary(m => m.Id);
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime < maxDate.Value);
                }
                var data = query.ToList();
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new
                    {
                        items = data.GroupBy(m => m.TriggerDateTime)
                                    .Select(m => new
                                    {
                                        triggerDateTime = m.Key,
                                        topLeiJiChenJiang = decimal.Round(m.Where(n => chuanGanQiItems[n.ChuanGanQiBaseInfoId].VerticalPosition == ChuanGanQiVerticalPosition.Top)
                                                                .Select(n => n.LeiJiChenJiang)
                                                                .DefaultIfEmpty()
                                                                .Average(),3),
                                        middleLeiJiChenJiang = decimal.Round(m.Where(n => chuanGanQiItems[n.ChuanGanQiBaseInfoId].VerticalPosition == ChuanGanQiVerticalPosition.Middle)
                                                                .Select(n => n.LeiJiChenJiang)
                                                                .DefaultIfEmpty()
                                                                .Average(),3),
                                        bottomLeiJiChenJiang = decimal.Round(m.Where(n => chuanGanQiItems[n.ChuanGanQiBaseInfoId].VerticalPosition == ChuanGanQiVerticalPosition.Bottom)
                                                                .Select(n => n.LeiJiChenJiang)
                                                                .DefaultIfEmpty()
                                                                .Average(),3)
                                    }).ToList()
                    }
                });

            }
        }

        [Menu("各墩台游趋势图")]
        [BridgeRequired]
        public ActionResult ChuanGanQiDunTaiRecord(DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                if (minDate.HasValue == false && maxDate.HasValue == false)
                {
                    return RedirectToAction("ChuanGanQiDunTaiRecord", new { minDate = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss"), maxDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") });
                }
                return View();
            }
        }

        public ActionResult QueryChuanGanQiDunTaiRecord(DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfo.BridgeId == BridgeMananger.CurrentBridgeId);
                var chuanGanQiItems = db.ChuanGanQiBaseInfo.Where(m => m.BridgeId == BridgeMananger.CurrentBridgeId).ToDictionary(m => m.Id);
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime < maxDate.Value);
                }
                var data = query.ToList();
                
                var dunTaiItems = JsonConvert.DeserializeObject<List<string>>(db.Bridge.Find(BridgeMananger.CurrentBridgeId).DunTaiItemsJsonString);
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new
                    {
                        items = data.GroupBy(m => m.TriggerDateTime)
                                    .Select(m => new
                                    {
                                        triggerDateTime = m.Key,
                                        values = dunTaiItems.Select(n => new
                                        {
                                            dunTai = n,
                                            leiJiChenJiang = decimal.Round(m.Where(k => chuanGanQiItems[k.ChuanGanQiBaseInfoId].DunTai == n)
                                                                .Select(k => k.LeiJiChenJiang)
                                                                .DefaultIfEmpty()
                                                                .Average(),3)
                                        }).ToList()
                                    }).ToList(),
                        dunTaiItems
                    }
                });

            }
        }

        /// <summary>
        /// 历史查询数据（每个小时候的统计值）
        /// </summary>
        /// <returns></returns>
        [BridgeRequired]
        [Menu("历史趋势查询")]
        public ActionResult ChuanGanQiDataRecord(Guid? chuanGanQiBaseInfoId, DateTime? minDate, DateTime? maxDate, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                if (chuanGanQiBaseInfoId.HasValue == false)
                {
                    return RedirectToAction("ChuanGanQiDataRecord", new { chuanGanQiBaseInfoId = db.Bridge.Find(BridgeMananger.CurrentBridgeId).ChuanGanQiBaseInfos.First().Id });
                }
                if (minDate.HasValue == false && maxDate.HasValue == false)
                {
                    return RedirectToAction("ChuanGanQiDataRecord", new { chuanGanQiBaseInfoId = chuanGanQiBaseInfoId, minDate = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:00"), maxDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:00") });
                }
                var query = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfoId == chuanGanQiBaseInfoId.Value && m.ChenJiangChaZhi.HasValue);
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime < maxDate.Value);
                }

                return View(new ChuanGanQiDataRecordData
                {
                    Pager = query.OrderByDescending(m => m.TriggerDateTime).ThenByDescending(m=>m.Id).Page(pageNum),
                    ChuanGanQiBaseInfo = db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId.Value)
                });
            }
        }

        public JsonNetResult QueryChuanGanQiDataRecord(Guid chuanGanQiBaseInfoId, DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.ChuanGanQiData.Where(m => m.ChuanGanQiBaseInfoId == chuanGanQiBaseInfoId && m.ChenJiangChaZhi.HasValue);
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime < maxDate.Value);
                }

                var data = query.ToList();
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new
                    {
                        items = data.Select(m => new
                        {
                            triggerDateTime = m.TriggerDateTime,
                            leiJiChenJiang = m.LeiJiChenJiang,
                            chenJiangSuLv = m.ChenJiangSuLv,
                            chenJiangChaZhi = m.ChenJiangChaZhi
                        }).ToList(),
                        huiZong = data.Select(m => new { m, v1 = 1 }).GroupBy(m => m.v1).Select(m => new
                        {
                            leiJiChenJiangMin = m.Min(n => Math.Abs(n.m.LeiJiChenJiang)),
                            leiJiChenJiangMax = m.Max(n => Math.Abs(n.m.LeiJiChenJiang)),
                            chenJiangSuLvMin = m.Min(n => Math.Abs(n.m.ChenJiangSuLv.Value)),
                            chenJiangSuLvMax = m.Max(n => Math.Abs(n.m.ChenJiangSuLv.Value)),
                            chenJiangChaZhiMin = m.Min(n => Math.Abs(n.m.ChenJiangChaZhi.Value)),
                            chenJiangChaZhiMax = m.Max(n => Math.Abs(n.m.ChenJiangChaZhi.Value))
                        }).FirstOrDefault()
                    }
                });
            }
        }

        [Menu("预警记录查询")]
        public ActionResult Warning(ChuanGanQiStatus? status, string ceDianNumber, DateTime? minDate, DateTime? maxDate,int pageNum=1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var query = db.ChuanGanQiWarning.Include(m=>m.ChuanGanQiBaseInfo);
                if (status.HasValue)
                {
                    query = query.Where(m => m.Status == status);
                }
                if (string.IsNullOrWhiteSpace(ceDianNumber) == false)
                {
                    query = query.Where(m => m.CeDianNumber.Contains(ceDianNumber));
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.TriggerDateTime < maxDate.Value);
                }

                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        [HttpPost]
        public ActionResult ChuLi(Guid chuanGanQiWarningId, Guid jieGuoId,string chuLiRen)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var chuanGanQiWarn = db.ChuanGanQiWarning.Find(chuanGanQiWarningId);
                var jieGuo = db.WarningResultTemplate.Find(jieGuoId);
                chuanGanQiWarn.ChuLiRen = chuLiRen;
                chuanGanQiWarn.ChuLiResult = jieGuo.Content;
                db.SaveChanges();

                return RedirectToAction("Warning");
            }
        }

        public ActionResult GuanZhu(Guid chuanGanQiBaseInfoId,string returnUrl= "/business/Warning")
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId).HandleStatus = WarningStatus.YiGuanZhu;
                return Redirect(returnUrl);
            }
        }

        public ActionResult HuiFu(Guid chuanGanQiBaseInfoId, string returnUrl = "/business/Warning")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.ChuanGanQiBaseInfo.Find(chuanGanQiBaseInfoId).HandleStatus = WarningStatus.YiHuiFu;
                return Redirect(returnUrl);
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public JsonNetResult Upload()
        {
            if (Request.Files.Count == 0)
            {
                return JsonNet(new ReturnValue { IsSuccess = false });
            }
            var item = Request.Files[0];
            var virtualPath = UploadManager.GetFullVirtualPath(UploadManager.LI_MIAN_TU, item.FileName);
            item.SaveAs(PF.GetPath(virtualPath));
            return JsonNet(new ReturnValue { IsSuccess = true, Data = virtualPath });
        }

        /// <summary>
        /// 批量下载文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        static string MultipleDownload(List<string> files)
        {
            if (files.Count == 1)
            {
                return PF.GetPath(files.First());
            }
            var tmp = PF.GetPath("/tmp");
            if (Directory.Exists(tmp) == false)
            {
                Directory.CreateDirectory(tmp);
            }
            var zipFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + RandomId.Create(6, RandomId.NUMBER) + ".zip";
            using (var zip = ZipFile.Create(PF.GetPath(Path.Combine(tmp, zipFileName))))
            {
                zip.BeginUpdate();
                foreach (var item in files)
                {
                    var fileFullPath = PF.GetPath(item);
                    if (System.IO.File.Exists(fileFullPath) == false)
                    {
                        continue;
                    }
                    zip.Add(fileFullPath, Path.GetFileName(fileFullPath));
                }
                zip.CommitUpdate();
            }
            return PF.GetPath(Path.Combine(tmp, zipFileName));
        }

        [BridgeRequired]
        [Menu("报告查询")]
        public ActionResult Report(ReportType? reportType,int pageNum=1)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var query = db.Report.Where(m => m.BridgeId == BridgeMananger.CurrentBridgeId.Value);
                if (reportType.HasValue)
                {
                    query = query.Where(m => m.Type == reportType);
                }
                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        public ActionResult DownloadReport(string fileFullName)
        {
            return base.Download(Server.MapPath(fileFullName));
        }
    }
}