using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Core;
using System.Data.Entity;
using Newtonsoft.Json;

namespace KeKeSoftPlatform.App.Areas.OrgChart.Controllers
{
    [Right(Identity.Employee | Identity.SuperAdmin | Identity.Admin)]
    [AreaName(OrgChartAreaRegistration.AREA_NAME)]
    public class SystemController : BaseController
    {
        #region 组织结构基础数据显示
        [Menu("组织结构管理")]
        public ActionResult Index()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View();
            }
        }

        public JsonResult QueryOrgChart()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var departments = db.Department.Where(m => m.AccountId == _User.Value.AccountId).ToList();
                var jobs = db.Job.Include(m => m.Parent).Where(m => m.AccountId == _User.Value.AccountId).ToList();

                var rootDepartment = departments.Single(m => m.ParentId.HasValue == false);
                var rootNode = new OrgChartTreeNode
                {
                    Name = rootDepartment.Name,
                    UserData = new OrgChartTreeNodeUserData
                    {
                        Id = rootDepartment.Id,
                        Name = rootDepartment.Name,
                        Type = GroupMemeberType.Department
                    },
                    Icon = "/images/orgchart/department.png"
                };
                var rootJob = jobs.Single(m => m.ParentId.HasValue == false);
                var rootJobNode = new OrgChartTreeNode
                {
                    Name = rootJob.Name,
                    UserData = new OrgChartTreeNodeUserData
                    {
                        Id = rootJob.Id,
                        Name = rootJob.Name,
                        Type = GroupMemeberType.Job
                    },
                    Icon = "/images/orgchart/job.png"
                };
                rootNode.Children.Add(rootJobNode);
                AppendChildren(rootJobNode, departments, jobs);
                return new JsonNetResult { Data = rootNode, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public void AppendChildren(OrgChartTreeNode parentNode, List<T_Department> departments, List<T_Job> jobs)
        {
            if (parentNode.UserData.Type == GroupMemeberType.Department)
            {
                //如果是父节点是部门，则需要添加该父部门下面的主管领导职务
                var managerJob = jobs.Single(m => m.DepartmentId == parentNode.UserData.Id && m.Parent.DepartmentId != parentNode.UserData.Id);
                var node = new OrgChartTreeNode
                {
                    Name = managerJob.Name,
                    UserData = new OrgChartTreeNodeUserData
                    {
                        Id = managerJob.Id,
                        Name = managerJob.Name,
                        Type = GroupMemeberType.Job
                    },
                    Icon = "/images/orgchart/job.png"
                };
                parentNode.Children.Add(node);
                AppendChildren(node, departments, jobs);
            }
            else if (parentNode.UserData.Type == GroupMemeberType.Job)
            {
                //如果父节点是职务，则添加该职务下面的直接子职务
                foreach (var job in jobs.Where(m => m.ParentId == parentNode.UserData.Id).ToList())
                {
                    if (job.DepartmentId == job.Parent.DepartmentId)
                    {
                        //如果该子职务跟父节点在同一个部门，则在父节点下面添加该子职务
                        var node = new OrgChartTreeNode
                        {
                            Name = job.Name,
                            UserData = new OrgChartTreeNodeUserData
                            {
                                Id = job.Id,
                                Name = job.Name,
                                Type = GroupMemeberType.Job,
                                IsGroup = job.IsGroup
                            },
                            Icon = "/images/orgchart/job.png"
                        };
                        parentNode.Children.Add(node);

                        AppendChildren(node, departments, jobs);
                    }
                    else
                    {
                        //如果该子职务跟父节点不在同一个部门，则说明该子职务是一个部门主管，需要在父节点下面先添加一个部门节点
                        var department = departments.Single(m => m.Id == job.DepartmentId);
                        var node = new OrgChartTreeNode
                        {
                            Name = department.Name,
                            UserData = new OrgChartTreeNodeUserData
                            {
                                Id = department.Id,
                                Name = department.Name,
                                Type = GroupMemeberType.Department
                            },
                            Icon = "/images/orgchart/department.png"
                        };
                        parentNode.Children.Add(node);
                        AppendChildren(node, departments, jobs);
                    }
                }
            }
        }

        public PartialViewResult DepartmentInfo(Guid departmentId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var department = db.Department.Find(departmentId);
                var managerJob = db.Job.Single(m => m.DepartmentId == departmentId && (m.ParentId.HasValue == false || m.Parent.DepartmentId != departmentId));
                var sql = db.Employee.Where(m => m.EmployeeJobLinks.Any(n => n.Job.DepartmentId == departmentId)).ToString();
                var model = new DepartmentInfoData
                {
                    Department = department,
                    DirectEmployee = db.Employee.Where(m => m.EmployeeJobLinks.Any(n => n.Job.DepartmentId == departmentId)).ToList(),
                    AllEmployee = db.Employee.Where(m => m.EmployeeJobLinks.Any(n => n.Job.Path.StartsWith(managerJob.Path))).ToList()
                };
                return PartialView(model);
            }
        }

        public PartialViewResult JobInfo(Guid jobId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var model = new JobInfoData()
                {
                    Job = db.Job.Find(jobId),
                    Employee = db.Employee.Where(m => m.EmployeeJobLinks.Any(n => n.JobId == jobId)).ToList()
                };

                return PartialView(model);
            }
        }
        #endregion

        #region 组织结构维护基础组件
        /// <summary>
        /// 填写职务信息
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Job_Enter()
        {
            return PartialView();
        }

        /// <summary>
        /// 填写员工信息
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_Enter()
        {
            return PartialView();
        }

        /// <summary>
        /// 检查输入的员工编号是否已存在
        /// </summary>
        /// <param name="number">需要验证的员工编号</param>
        /// <returns></returns>
        public JsonResult CheckEmployeeNumber(string number)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return Json(new ReturnValue { IsSuccess = !db.Employee.Any(m => m.AccountId == _User.Value.AccountId && m.Number == number) });
            }
        }

        /// <summary>
        /// 从组织结构选择人员（单选）
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_ChooseFromOrgChart()
        {
            return PartialView();
        }

        /// <summary>
        /// 从无职务人员中选择人员（单选）
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_ChooseFromNoneJob()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return PartialView(db.Employee.Where(m => m.EmployeeJobLinks.Any() == false && m.AccountId == _User.Value.AccountId).ToList());
            }
        }

        /// <summary>
        /// 选择 【选择人员】 的方式
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_ChooseMethod()
        {
            return PartialView();
        }

        /// <summary>
        /// 获取组织结构以及人员信息，主要用在Employee_ChooseFromOrgChart、Employee_ChooseMultipleFromOrgChart
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryDepartmentEmployee()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var departments = db.Department.Where(m => m.AccountId == _User.Value.AccountId).ToList();
                var employees = db.EmployeeJobLink.Where(m => m.Job.AccountId == _User.Value.AccountId).Select(m => new DeparmentEmployeeData.EmployeeItem
                {
                    DepartmentId = m.Job.DepartmentId,
                    Id = m.Employee.Id,
                    Name = m.Employee.Name,
                    JobName = m.Job.Name,
                    Number = m.Employee.Number
                }).ToList();
                var rootDepartment = departments.Single(m => m.ParentId.HasValue == false);
                var model = new DeparmentEmployeeData();
                model.Department = new ZTreeNode<DeparmentEmployeeData.DepartmentItem>
                {
                    Name = rootDepartment.Name,
                    Icon = "/images/orgchart/department.png",
                    UserData = new DeparmentEmployeeData.DepartmentItem
                    {
                        Id = rootDepartment.Id,
                        Name = rootDepartment.Name
                    }
                };

                this.BuildDepartmentTree(model.Department, departments);

                model.DepartmentEmployees = employees.GroupBy(m => m.DepartmentId).Select(m => new DeparmentEmployeeData.DepartmentEmployeeItem
                {
                    DepartmentId = m.Key,
                    Employees = m.GroupBy(n => n.Id).Select(k => new DeparmentEmployeeData.EmployeeItem
                    {
                        Id = k.Key,
                        DepartmentId = m.Key,
                        Name = k.First().Name,
                        JobName = string.Join(",", k.Select(g => g.JobName)),
                        Number = k.First().Number
                    }).ToList()
                }).ToList();

                return JsonNet(model);
            }
        }

        /// <summary>
        /// 构建组织结构数据，递归调用，供QueryDepartmentEmployee调用
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="departments"></param>
        private void BuildDepartmentTree(ZTreeNode<DeparmentEmployeeData.DepartmentItem> parent, List<T_Department> departments)
        {
            foreach (var item in departments.Where(m => m.ParentId == parent.UserData.Id))
            {
                var node = new ZTreeNode<DeparmentEmployeeData.DepartmentItem>()
                {
                    Name = item.Name,
                    Icon = "/images/orgchart/department.png",
                    UserData = new DeparmentEmployeeData.DepartmentItem
                    {
                        Id = item.Id,
                        Name = item.Name
                    }
                };

                parent.Children.Add(node);

                this.BuildDepartmentTree(node, departments);
            }
        }

        private void BuildDepartmentTree(ZTreeNode<DepartmentJobData.DepartmentItem> parent, List<T_Department> departments)
        {
            foreach (var item in departments.Where(m => m.ParentId == parent.UserData.Id))
            {
                var node = new ZTreeNode<DepartmentJobData.DepartmentItem>()
                {
                    Name = item.Name,
                    Icon = "/images/orgchart/department.png",
                    UserData = new DepartmentJobData.DepartmentItem
                    {
                        Id = item.Id,
                        Name = item.Name
                    }
                };

                parent.Children.Add(node);

                this.BuildDepartmentTree(node, departments);
            }
        }


        /// <summary>
        /// 已选择人员
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_Multiple()
        {
            return PartialView();
        }

        /// <summary>
        /// 从组织结构选择人员（多选）
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_ChooseMultipleFromOrgChart()
        {
            return PartialView();
        }

        /// <summary>
        /// 从无职务人员中选择人员（多选）
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Employee_ChooseMultipleFromNoneJob()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return PartialView(db.Employee.Where(m => m.EmployeeJobLinks.Any() == false).ToList());
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 15:17:50
        /// 开发内容：填写部门信息
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Department_Enter()
        {
            return PartialView();
        }

        public PartialViewResult Job_ChooseMultiple()
        {
            return PartialView();
        }

        public JsonResult QueryDepartmentJob()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var departments = db.Department.Where(m => m.AccountId == _User.Value.AccountId).ToList();
                var jobs = db.Job.Where(m => m.AccountId == _User.Value.AccountId).Select(m => new DepartmentJobData.JobItem
                {
                    DepartmentId = m.DepartmentId,
                    Id = m.Id,
                    Name = m.Name
                }).ToList();
                var rootDepartment = departments.Single(m => m.ParentId.HasValue == false);
                var model = new DepartmentJobData();
                model.Department = new ZTreeNode<DepartmentJobData.DepartmentItem>
                {
                    Name = rootDepartment.Name,
                    Icon = "/images/orgchart/department.png",
                    UserData = new DepartmentJobData.DepartmentItem
                    {
                        Id = rootDepartment.Id,
                        Name = rootDepartment.Name
                    }
                };

                this.BuildDepartmentTree(model.Department, departments);

                model.DepartmentJobs = jobs.GroupBy(m => m.DepartmentId).Select(m => new DepartmentJobData.DepartmentJobItem
                {
                    DepartmentId = m.Key,
                    Jobs = m.GroupBy(n => n.Id).Select(k => new DepartmentJobData.JobItem
                    {
                        Id = k.Key,
                        DepartmentId = m.Key,
                        Name = k.First().Name
                    }).ToList()
                }).ToList();

                return JsonNet(model);
            }
        }

        public PartialViewResult Department_ChooseMultiple()
        {
            return PartialView();
        }

        public JsonResult QueryChooseDepartment()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var departments = db.Department.Where(m => m.AccountId == _User.Value.AccountId).ToList();
                var sortedDepartments = ListProviderBus.GetListItemCollection("Account_Departments").Select(m => new ChooseDepartmentData
                {
                    Id = Guid.Parse(m.Value),
                    FullName = m.Text,
                    Name = m.Text.Split(new string[] { "==>" }, StringSplitOptions.RemoveEmptyEntries).First()
                });
                return JsonNet(sortedDepartments);
            }
        }
        #endregion

        /// <summary>
        /// 添加职务-保存数据
        /// </summary>
        /// <param name="data">json字符串</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddJob([JsonModelBinder("data")]AddJobInfoData model)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var parentJob = db.Job.Find(model.ParentJobId);
                var job = new T_Job
                {
                    Id = PF.Key(),
                    AccountId = parentJob.AccountId,
                    DepartmentId = parentJob.DepartmentId,
                    IsGroup = model.IsGroup,
                    ParentId = parentJob.Id,
                    Name = model.JobName,
                    Path = parentJob.Path + UtilEncoder.Instance.NextToString()
                };
                db.Job.Add(job);

                foreach (var item in model.AddEmployees)
                {
                    switch (item.ChooseAddUserMethod)
                    {
                        case ChooseAddUserMethod.EnterEmployeeInfo:
                            var employee = new T_Employee
                            {
                                AccountId = job.AccountId,
                                Id = PF.Key(),
                                Name = item.EmployeeName,
                                Number = item.EmployeeNumber,
                                Password = EncryptUtils.MD5Encrypt(item.EmployeePassword)
                            };
                            db.Employee.Add(employee);
                            db.EmployeeJobLink.Add(new T_EmployeeJobLink
                            {
                                Id = PF.Key(),
                                JobId = job.Id,
                                EmployeeId = employee.Id
                            });
                            break;
                        case ChooseAddUserMethod.ChooseFromOrgChart:
                            db.EmployeeJobLink.Add(new T_EmployeeJobLink
                            {
                                Id = PF.Key(),
                                JobId = job.Id,
                                EmployeeId = item.EmployeeId.Value
                            });
                            break;
                        case ChooseAddUserMethod.ChooseFromNoneJob:
                            db.EmployeeJobLink.Add(new T_EmployeeJobLink
                            {
                                Id = PF.Key(),
                                JobId = job.Id,
                                EmployeeId = item.EmployeeId.Value
                            });
                            break;
                        default:
                            break;
                    }
                }

                db.SaveChanges();

                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new OrgChartTreeNode
                    {
                        Name = job.Name,
                        Icon = "/images/orgchart/job.png",
                        UserData = new OrgChartTreeNodeUserData
                        {
                            Id = job.Id,
                            Name = job.Name,
                            Type = GroupMemeberType.Job
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-03 18:14:28
        /// 开发内容：根据jobId删除职务
        /// </summary>
        /// <param name="jobId">需要修改的职务的Id</param>
        /// <returns></returns>
        public JsonResult DeleteJob(Guid jobId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                try
                {
                    var job = db.Job.Find(jobId);
                    var jobWithChildren = db.Job.Where(m => m.Path.StartsWith(job.Path)).ToList().OrderByDescending(m => m.Id).ToList();
                    foreach (var item in jobWithChildren)
                    {
                        db.EmployeeJobLink.RemoveRange(item.EmployeeJobLinks);
                    }
                    db.EmployeeJobLink.RemoveRange(job.EmployeeJobLinks);
                    db.Job.RemoveRange(jobWithChildren);
                    db.SaveChanges();
                    return Json(new ReturnValue { IsSuccess = true });
                }
                catch (Exception ex)
                {
                    return Json(new ReturnValue { IsSuccess = false, Error = ex.ToString() });
                }
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-03 21:38:44
        /// 开发内容：修改职务——获取职务原始数据
        /// </summary>
        /// <param name="jobId">需要修改的职务的Id</param>
        /// <returns></returns>
        public JsonResult EditJob(Guid jobId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var job = db.Job.Find(jobId);
                return JsonNet(new ReturnValue { IsSuccess = true, Data = new { jobName = job.Name, isGroup = job.IsGroup } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 15:14:46
        /// 开发内容：修改职务——保存修改职务基本信息 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditJob(EditJobData model)
        {
            if (ModelState.IsValid == false)
            {
                return JsonNet(new ReturnValue
                {
                    IsSuccess = false,
                    Error = ModelState.First(m => m.Value.Errors.Any()).Value.Errors.First().ErrorMessage
                });
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var job = db.Job.Find(model.JobId);
                job.Name = model.JobName;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 18:21:46
        /// 开发内容：修改部门——后去部门原始信息 
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <returns></returns>
        public JsonResult EditDepartment(Guid departmentId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new
                    {
                        departmentName = db.Department.Find(departmentId).Name
                    }
                });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 18:25:29
        /// 开发内容：修改职务——保存修改职务基本信息 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditDepartment(EditDepartmentData model)
        {
            if (ModelState.IsValid == false)
            {
                return JsonNet(new ReturnValue
                {
                    IsSuccess = false,
                    Error = ModelState.First(m => m.Value.Errors.Any()).Value.Errors.First().ErrorMessage
                });
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var department = db.Department.Find(model.DepartmentId);
                department.Name = model.DepartmentName;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 19:09:23
        /// 开发内容：根据DepartmentId删除部门
        /// </summary>
        /// <param name="departmentId">需要删除的部门的Id</param>
        /// <returns></returns>
        public JsonResult DeleteDepartment(Guid departmentId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                try
                {
                    var department = db.Department.Find(departmentId);
                    var managerJob = department.Jobs.First(m => m.Parent.DepartmentId != department.Id);
                    var jobWithChildren = db.Job.Where(m => m.Path.StartsWith(managerJob.Path)).OrderByDescending(m => m.Id).ToList();
                    foreach (var item in jobWithChildren)
                    {
                        db.EmployeeJobLink.RemoveRange(item.EmployeeJobLinks);
                    }
                    db.EmployeeJobLink.RemoveRange(managerJob.EmployeeJobLinks);
                    db.Job.RemoveRange(jobWithChildren);
                    db.Department.RemoveRange(db.Department.Where(m => m.AccountId == department.AccountId && m.Path.StartsWith(department.Path)).OrderByDescending(m => m.Id));
                    db.Job.Remove(managerJob);
                    db.Department.Remove(department);
                    db.SaveChanges();
                    return Json(new ReturnValue { IsSuccess = true });
                }
                catch (Exception ex)
                {
                    return Json(new ReturnValue { IsSuccess = false, Error = ex.ToString() });
                }
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-04 20:59:54
        /// 开发内容：添加部门
        /// </summary>
        /// <param name="model">部门数据</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddDepartment([JsonModelBinder("data")]AddDepartmentInfoData model)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var parentJob = db.Job.Find(model.ParentJobId);
                var department = new T_Department
                {
                    Name = model.DepartmentName,
                    AccountId = parentJob.AccountId,
                    Id = PF.Key(),
                    ParentId = parentJob.DepartmentId,
                    Path = parentJob.Department.Path + UtilEncoder.Instance.NextToString()
                };
                db.Department.Add(department);
                var job = new T_Job
                {
                    Id = PF.Key(),
                    AccountId = parentJob.AccountId,
                    DepartmentId = department.Id,
                    IsGroup = false,
                    ParentId = parentJob.Id,
                    Name = model.JobName,
                    Path = parentJob.Path + UtilEncoder.Instance.NextToString()
                };
                db.Job.Add(job);

                switch (model.ChooseAddUserMethod)
                {
                    case ChooseAddUserMethod.EnterEmployeeInfo:
                        var employee = new T_Employee
                        {
                            AccountId = job.AccountId,
                            Id = PF.Key(),
                            Name = model.EmployeeName,
                            Number = model.EmployeeNumber,
                            Password = EncryptUtils.MD5Encrypt(model.EmployeePassword)
                        };
                        db.Employee.Add(employee);
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = employee.Id
                        });
                        break;
                    case ChooseAddUserMethod.ChooseFromOrgChart:
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = model.EmployeeId.Value
                        });
                        break;
                    case ChooseAddUserMethod.ChooseFromNoneJob:
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = model.EmployeeId.Value
                        });
                        break;
                    default:
                        break;
                }

                db.SaveChanges();


                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new OrgChartTreeNode
                    {
                        Name = department.Name,
                        Icon = "/images/orgchart/department.png",
                        UserData = new OrgChartTreeNodeUserData
                        {
                            Id = department.Id,
                            Name = department.Name,
                            Type = GroupMemeberType.Department
                        },
                        Children = new List<OrgChartTreeNode>
                        {
                            new OrgChartTreeNode
                            {
                                Name = job.Name,
                                Icon = "/images/orgchart/job.png",
                                UserData = new OrgChartTreeNodeUserData
                                {
                                    Id = job.Id,
                                    Name = job.Name,
                                    IsGroup = false,
                                    Type = GroupMemeberType.Job
                                }
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-05 13:37:42
        /// 开发内容：添加职务人员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddJobEmployee(AddJobEmployeeData model)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var job = db.Job.Find(model.JobId);
                if (job.IsGroup == false)
                {
                    db.EmployeeJobLink.RemoveRange(job.EmployeeJobLinks);
                }
                switch (model.ChooseAddUserMethod)
                {
                    case ChooseAddUserMethod.EnterEmployeeInfo:
                        var employee = new T_Employee
                        {
                            AccountId = job.AccountId,
                            Id = PF.Key(),
                            Name = model.EmployeeName,
                            Number = model.EmployeeNumber,
                            Password = EncryptUtils.MD5Encrypt(model.EmployeePassword)
                        };
                        db.Employee.Add(employee);
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = employee.Id
                        });
                        break;
                    case ChooseAddUserMethod.ChooseFromOrgChart:
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = model.EmployeeId.Value
                        });
                        break;
                    case ChooseAddUserMethod.ChooseFromNoneJob:
                        db.EmployeeJobLink.Add(new T_EmployeeJobLink
                        {
                            Id = PF.Key(),
                            JobId = job.Id,
                            EmployeeId = model.EmployeeId.Value
                        });
                        break;
                    default:
                        break;
                }

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-05 13:38:02
        /// 开发内容：移除职务人员
        /// </summary>
        /// <param name="employees">要移除的人员Id</param>
        /// <param name="jobId">要移除人员所在职务Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveJobEmployee([JsonModelBinder("employees")]List<Guid> employees, Guid jobId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                foreach (var employeeId in employees)
                {
                    db.EmployeeJobLink.Remove(db.EmployeeJobLink.Single(m => m.JobId == jobId && m.EmployeeId == employeeId));
                }

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-05 16:04:02
        /// 开发内容：查看员工详情
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        public PartialViewResult EmployeeDetail(Guid employeeId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return PartialView(db.Employee.Find(employeeId));
            }
        }

        public PartialViewResult EditEmployee(Guid employeeId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var employee = db.Employee.Find(employeeId);
                return PartialView(new EditEmployeeData { EmployeeId = employeeId, Name = employee.Name });
            }
        }

        /// <summary>
        /// 开发人员：杜冠梦
        /// 开发日期：2017-01-05 16:09:50
        /// 开发内容：修改员工基本信息——保存
        /// </summary>
        /// <param name="model">员工信息</param>
        /// <returns></returns>
        [HttpPost]
        public JsonNetResult EditEmployee(EditEmployeeData model)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var employee = db.Employee.Find(model.EmployeeId);
                employee.Name = model.Name;
                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public JsonNetResult Departments()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var departments = db.Department.Where(m => m.AccountId == _User.Value.AccountId).ToList();
                var items = departments
                    .OrderBy(m => m.Path.Length)
                    .ThenBy(m => m.Name)
                    .Select(m => new
                    {
                        text = string.Join("==>", departments.Where(n => m.Path.Contains(n.Path))
                                                                .OrderBy(n => n.Path.Length)
                                                                .Select(n => n.Name)
                                                                .ToList()),
                        value = m.Id.ToString()
                    })
                    .ToList();

                return JsonNet(new ReturnValue { IsSuccess = true, Data = items });
            }
        }

        [Menu("工作组管理")]
        public ActionResult Group()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.Group.Where(m => m.AccountId == _User.Value.AccountId).ToList());
            }
        }

        public PartialViewResult GroupDetail(Guid groupId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var model = new GroupDetailData
                {
                    Employees = db.Employee.Where(m => m.GroupEmployees.Any(n => n.EmployeeId == m.Id && n.GroupId == groupId))
                        .Select(m => new GroupDetailData.EmployeeItem
                        {
                            Name = m.Name,
                            Number = m.Number
                        }).ToList(),
                    GroupMembers = db.GroupMemeber.Where(m => m.GroupId == groupId)
                        .Select(m => new GroupDetailData.GroupMember
                        {
                            Id = m.Id,
                            MemeberType = m.MemeberType,
                            MemberId = m.MemberId
                        }).ToList()
                };

                foreach (var item in model.GroupMembers)
                {
                    switch (item.MemeberType)
                    {
                        case GroupMemeberType.Department:
                            var department = db.Department.Find(item.MemberId);
                            item.Name = string.Join("==>", db.Department.Where(m => department.Path.Contains(m.Path)).ToList().OrderBy(m => m.Path).Select(m => m.Name).ToList());
                            break;
                        case GroupMemeberType.Job:
                            item.Name = db.Job.Find(item.MemberId).Name;
                            break;
                        case GroupMemeberType.Employee:
                            var employee = db.Employee.Find(item.MemberId);
                            item.Name = employee.Name;
                            item.Number = employee.Number;
                            break;
                        default:
                            break;
                    }
                }

                return PartialView(model);
            }
        }

        public PartialViewResult CreateGroup()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateGroup(CreateGroupData model)
        {
            if (ModelState.IsValid == false)
            {
                return PartialView(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Group.Add(new T_Group
                {
                    Id = PF.Key(),
                    AccountId = _User.Value.AccountId,
                    Name = model.Name,
                    Description = model.Description
                });

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public PartialViewResult EditGroup(Guid groupId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var group = db.Group.Find(groupId);
                return PartialView(new EditGroupData
                {
                    Id = groupId,
                    Name = group.Name,
                    Description = group.Description
                });
            }
        }

        [HttpPost]
        public ActionResult EditGroup(EditGroupData model)
        {
            if (ModelState.IsValid == false)
            {
                return PartialView(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var group = db.Group.Find(model.Id);
                group.Name = model.Name;
                group.Description = model.Description;

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public JsonNetResult DeleteGroup(Guid groupId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Group.Remove(db.Group.Find(groupId));
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public JsonNetResult AddEmployeeToGroup(Guid groupId, [JsonModelBinder("employeeIds")]List<Guid> employeeIds)
        {
            var result = KeKeSoftPlatformDbContext.UseTransaction(db =>
            {
                var groupMembers = db.GroupMemeber.Where(m => m.GroupId == groupId).ToList();
                foreach (var item in groupMembers.Where(m => m.MemeberType == GroupMemeberType.Employee && employeeIds.Contains(m.MemberId) == false).ToList())
                {
                    db.GroupMemeber.Remove(item);
                }

                foreach (var employeeId in employeeIds)
                {
                    if (groupMembers.Any(m => m.MemberId == employeeId))
                    {
                        continue;
                    }
                    db.GroupMemeber.Add(new T_GroupMemeber
                    {
                        Id = PF.Key(),
                        GroupId = groupId,
                        MemberId = employeeId,
                        MemeberType = GroupMemeberType.Employee
                    });
                }

                db.GroupEmployee.RemoveRange(db.GroupEmployee.Where(m => m.GroupId == groupId).ToList());
                db.SaveChanges();

                var employees = new Dictionary<Guid, T_GroupEmployee>();
                foreach (var item in db.GroupMemeber.Where(m => m.GroupId == groupId).ToList())
                {
                    switch (item.MemeberType)
                    {
                        case GroupMemeberType.Department:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.Job.DepartmentId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Job:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.JobId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Employee:
                            if (employees.ContainsKey(item.MemberId) == false)
                            {
                                employees.Add(item.MemberId, new T_GroupEmployee
                                {
                                    Id = PF.Key(),
                                    GroupId = groupId,
                                    EmployeeId = item.MemberId
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
                db.GroupEmployee.AddRange(employees.Values.Select(m => m).ToList());

                db.SaveChanges();
                return new ReturnValue { IsSuccess = true };
            });

            return JsonNet(result);
        }


        public JsonNetResult AddJobToGroup(Guid groupId, [JsonModelBinder("jobIds")]List<Guid> jobIds)
        {
            var result = KeKeSoftPlatformDbContext.UseTransaction(db =>
            {
                var groupMembers = db.GroupMemeber.Where(m => m.GroupId == groupId).ToList();
                foreach (var item in groupMembers.Where(m => m.MemeberType == GroupMemeberType.Job && jobIds.Contains(m.MemberId) == false).ToList())
                {
                    db.GroupMemeber.Remove(item);
                }

                foreach (var jobId in jobIds)
                {
                    if (groupMembers.Any(m => m.MemberId == jobId))
                    {
                        continue;
                    }
                    db.GroupMemeber.Add(new T_GroupMemeber
                    {
                        Id = PF.Key(),
                        GroupId = groupId,
                        MemberId = jobId,
                        MemeberType = GroupMemeberType.Job
                    });
                }

                db.GroupEmployee.RemoveRange(db.GroupEmployee.Where(m => m.GroupId == groupId).ToList());
                db.SaveChanges();

                var employees = new Dictionary<Guid, T_GroupEmployee>();
                foreach (var item in db.GroupMemeber.Where(m => m.GroupId == groupId).ToList())
                {
                    switch (item.MemeberType)
                    {
                        case GroupMemeberType.Department:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.Job.DepartmentId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Job:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.JobId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Employee:
                            if (employees.ContainsKey(item.MemberId) == false)
                            {
                                employees.Add(item.MemberId, new T_GroupEmployee
                                {
                                    Id = PF.Key(),
                                    GroupId = groupId,
                                    EmployeeId = item.MemberId
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
                db.GroupEmployee.AddRange(employees.Values.Select(m => m).ToList());
                db.SaveChanges();

                return new ReturnValue { IsSuccess = true };
            });
            return JsonNet(result);
        }

        public JsonNetResult AddDepartmentToGroup(Guid groupId, [JsonModelBinder("departmentIds")]List<Guid> departmentIds)
        {
            var result = KeKeSoftPlatformDbContext.UseTransaction(db =>
            {
                var groupMembers = db.GroupMemeber.Where(m => m.GroupId == groupId).ToList();
                foreach (var item in groupMembers.Where(m => m.MemeberType == GroupMemeberType.Department && departmentIds.Contains(m.MemberId) == false).ToList())
                {
                    db.GroupMemeber.Remove(item);
                }

                foreach (var departmentId in departmentIds)
                {
                    if (groupMembers.Any(m => m.MemberId == departmentId))
                    {
                        continue;
                    }
                    db.GroupMemeber.Add(new T_GroupMemeber
                    {
                        Id = PF.Key(),
                        GroupId = groupId,
                        MemberId = departmentId,
                        MemeberType = GroupMemeberType.Department
                    });
                }

                db.GroupEmployee.RemoveRange(db.GroupEmployee.Where(m => m.GroupId == groupId).ToList());
                db.SaveChanges();

                var employees = new Dictionary<Guid, T_GroupEmployee>();
                foreach (var item in db.GroupMemeber.Where(m => m.GroupId == groupId).ToList())
                {
                    switch (item.MemeberType)
                    {
                        case GroupMemeberType.Department:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.Job.DepartmentId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Job:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.JobId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Employee:
                            if (employees.ContainsKey(item.MemberId) == false)
                            {
                                employees.Add(item.MemberId, new T_GroupEmployee
                                {
                                    Id = PF.Key(),
                                    GroupId = groupId,
                                    EmployeeId = item.MemberId
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
                db.GroupEmployee.AddRange(employees.Values.Select(m => m).ToList());
                db.SaveChanges();

                return new ReturnValue { IsSuccess = true };
            });
            return JsonNet(result);
        }

        public JsonNetResult RemoveGroupMember(Guid groupId, [JsonModelBinder("groupMemberIds")]List<Guid> groupMemberIds)
        {
            var result = KeKeSoftPlatformDbContext.UseTransaction(db =>
            {
                foreach (var id in groupMemberIds)
                {
                    db.GroupMemeber.Remove(db.GroupMemeber.Find(id));
                }

                db.GroupEmployee.RemoveRange(db.GroupEmployee.Where(m => m.GroupId == groupId).ToList());
                db.SaveChanges();

                var employees = new Dictionary<Guid, T_GroupEmployee>();
                foreach (var item in db.GroupMemeber.Where(m => m.GroupId == groupId).ToList())
                {
                    switch (item.MemeberType)
                    {
                        case GroupMemeberType.Department:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.Job.DepartmentId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Job:
                            foreach (var employeeId in db.EmployeeJobLink.Where(m => m.JobId == item.MemberId).Select(m => m.EmployeeId).ToList())
                            {
                                if (employees.ContainsKey(employeeId) == false)
                                {
                                    employees.Add(employeeId, new T_GroupEmployee
                                    {
                                        Id = PF.Key(),
                                        GroupId = groupId,
                                        EmployeeId = employeeId
                                    });
                                }
                            }
                            break;
                        case GroupMemeberType.Employee:
                            if (employees.ContainsKey(item.MemberId) == false)
                            {
                                employees.Add(item.MemberId, new T_GroupEmployee
                                {
                                    Id = PF.Key(),
                                    GroupId = groupId,
                                    EmployeeId = item.MemberId
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
                db.GroupEmployee.AddRange(employees.Values.Select(m => m).ToList());
                db.SaveChanges();

                return new ReturnValue { IsSuccess = true };
            });

            return JsonNet(result);
        }
    }
}
