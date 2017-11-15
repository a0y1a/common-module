(function ($,window) {
    var modal_box_id = "__modal_box__";
    if ($("#" + modal_box_id).length === 0) {
        $(document.body).append("<div id='" + modal_box_id + "'></div>");
    }

    var oc = {};
    oc.job = {};
    oc.employee = {};
    oc.department = {};

    var generateLeftButtons = function (component) {
        var options = component.getOptions();
        if (options.leftButtons.groupName) {
            var $group = $("<div>").addClass("btn-group");
            var $topButton = $("<button>").addClass("btn btn-default dropdown-toggle")
                                    .prop("type", "button")
                                    .prop("data-toggle", "dropdown")
                                    .append(options.leftButtons.groupName + "<span class='caret'></span>");
            var $ul = $("<div>").addClass("dropdown-menu");
            $(options.leftButtons.buttons).each(function () {
                var $li = $("<li><a href='#'>" + this.text + "</a></li>");
                $ul.append($li);
                this.init.call($li, component);
            });
            $group.append($topButton.append($ul));
            return $group;
        }

        var $button = $("<button>").addClass("btn btn-default pull-left").text(options.leftButtons.text);
        options.leftButtons.init.call($button, component);
        return $button;
    };

    var generateRightButtons = function (component) {
        var options = component.getOptions();
        if (options.rightButtons.groupName) {
            var $group = $("<div>").addClass("btn-group");
            var $topButton = $("<button>").addClass("btn btn-primary dropdown-toggle")
                                    .prop("type", "button")
                                    .attr("data-toggle", "dropdown")
                                    .append(options.rightButtons.groupName + " <span class='caret'></span>");
            var $ul = $("<div>").addClass("dropdown-menu");
            $(options.rightButtons.buttons).each(function () {
                var $li = $("<li><a href='#'>" + this.text + "</a></li>");
                $ul.append($li);
                this.init.call($li, component);
            });
            $group.append($topButton).append($ul);
            return $group;
        }
        var $button = $("<button>").addClass("btn btn-primary").text(options.rightButtons.text);
        options.rightButtons.init.call($button, component);
        return $button;
    };

    oc.job.enter = function (options) {
        /*
         * 填写职务信息
         * 
         * options={
         *     userData:{}传给组件的其他用户数据
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     viewBag:{}组件产生的临时数据,
         *     beforeInit:function 组件dom加载之前,编写符合Promise的方法
         *     afterInit:function 组件dom加载之后,编写符合Promise的方法
         *     title:组件的标题
         * }
         */
        var component;
        var cache_job_enter_key = "cache_job_enter_key";
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                this.restore();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        component.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    $(this).click(function () {
                        
                    });
                }
            }

        };

        var options = $.extend(defaults, options);
        var viewBag = {};
        var $modal;
        component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Job_Enter", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            options.validate = util.validate().element("job-name").required().end();

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });

                return this;
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_job_enter_key);
                }
                else {
                    util.cache(cache_job_enter_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    jobName: $("#job-name").val(),
                    isGroup: $("#is-group").is(":checked")
                };
                util.cache(cache_job_enter_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_job_enter_key);
                if (data) {
                    $("#job-name").val(data.jobName);
                    $("#is-group").prop("checked", data.isGroup);
                }
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        }
        return component;
    };

    oc.job.chooseMultiple = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_job_choose_multiple_key = "cache_job_choose_multiple_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);
        var jobs;
        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();

                        $.get("/orgchart/system/Job_ChooseMultiple", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html).dblclick;

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                        }).then(function () {
                            var dtd = $.Deferred();
                            $.get("/orgchart/system/QueryDepartmentJob", function (result) {
                                jobs = result;
                                var orgChart = $.fn.zTree.init($("#componentMultipleOrgChart"), {
                                    callback: {
                                        onClick: function (event, treeId, treeNode) {
                                            $("#componentMultipleJobItems tbody").empty();
                                            $(result.departmentJobs).each(function () {
                                                if (this.departmentId === treeNode.userData.id) {
                                                    $(this.jobs).each(function () {
                                                        $("#componentMultipleJobItems tbody").append("<tr data-id='" + this.id + "'><td>" + this.name + "</td></tr>");
                                                    });
                                                    $("#componentMultipleJobItems>tbody>tr").click(function () {
                                                        $("#componentMultipleJobItems>tbody>tr").removeClass("success");
                                                        $(this).addClass("success");
                                                    }).dblclick(function () {
                                                        var jobId = $(this).data("id");
                                                        var exists = false;
                                                        $("#componentMultipleJob>tbody>tr").each(function () {
                                                            if ($(this).data("id") === jobId) {
                                                                exists = true;
                                                                $("#componentMultipleJob>tbody>tr").removeClass("success");
                                                                $(this).addClass("success");
                                                                return false;
                                                            }
                                                        });
                                                        if (exists) {
                                                            alert("该人员再已选人员中已存在");
                                                            return;
                                                        }

                                                        var $tr = $(this).clone().removeClass("success");
                                                        $tr.click(function () {
                                                            $("#componentMultipleJob>tbody>tr").removeClass("success");
                                                            $(this).addClass("success");
                                                        }).dblclick(function () {
                                                            $(this).remove();
                                                        });
                                                        $("#componentMultipleJob").append($tr);
                                                    });
                                                    return false;
                                                }
                                            });
                                        }
                                    }
                                }, [result.department]);

                                dtd.resolve();
                            });

                            return dtd.promise();
                        }).then(function () {
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data, restore) {
                if (arguments.length === 0) {
                    return util.cache(cache_job_choose_multiple_key);
                }
                else {
                    util.cache(cache_job_choose_multiple_key, data);
                    if (restore) {
                        this.restore();
                    }
                    return this;
                }
            },
            persist: function () {
                var data = [];
                $("#componentMultipleJob>tbody>tr").each(function () {
                    data.push({
                        jobId: $(this).data("id"),
                        jobName: $(this).find("td:first").text()
                    });
                });
                util.cache(cache_job_choose_multiple_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_job_choose_multiple_key);
                if (!data) {
                    return this;
                }
                $(data).each(function () {
                    var job;
                    var jobId = this.jobId;
                    $(jobs.departmentJobs).each(function () {
                        $(this.jobs).each(function () {
                            if (this.id === jobId) {
                                job = this;
                            }
                        });
                    });
                    var $tr = $("<tr data-id='" + jobId + "'><td>" + job.name + "</td></tr>");
                    $tr.click(function () {
                        $("#componentMultipleJob>tbody>tr").removeClass("success");
                        $(this).addClass("success");
                    }).dblclick(function () {
                        $(this).remove();
                    });
                    $("#componentMultipleJob").append($tr);
                });
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.enter = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_employee_enter_key = "cache_employee_enter_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            },
            userData: {}
        };
        var options = $.extend(defaults, options);
        var component = {
            init: function () {
                options.beforeInit.call(component,options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/employee_Enter", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            options.validate = util.validate()
                                .element("employee-name")
                                    .required()
                                    .end()
                                .element("employee-number")
                                    .required()
                                    .must(function (value) {
                                        value = $("#employee-number").data("prev-fix") + value;
                                        if (options.userData && options.userData.employeeNumbers) {
                                            for (var i = 0; i < options.userData.employeeNumbers.length; i++) {
                                                if (value.toLowerCase() === options.userData.employeeNumbers[i].toLowerCase()) {
                                                    return false;
                                                }
                                            }
                                        }

                                        return true;
                                    })
                                    .message("该编号已存在")
                                    .async(function (value,success,fail) {
                                        value = $("#employee-number").data("prev-fix") + value;
                                        $.post("/orgchart/system/CheckEmployeeNumber", { number: value }, function (result) {
                                            if (result.isSuccess) {
                                                success();
                                                return;
                                            }
                                            else {
                                                fail();
                                                return;
                                            }
                                        });
                                    })
                                    .message("该编号已存在")
                                    .end()
                                .element("employee-password")
                                    .required()
                                    .end();

                            $modal.modal("show");
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_enter_key);
                }
                else {
                    util.cache(cache_employee_enter_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    employeeName: $("#employee-name").val(),
                    employeeNumber: $("#employee-number").data("prev-fix") + $("#employee-number").val(),
                    employeePassword: $("#employee-password").val()
                };
                util.cache(cache_employee_enter_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_enter_key);
                if (data) {
                    $("#employee-name").val(data.employeeName);
                    $("#employee-number").val(data.employeeNumber.substring($("#employee-number").data("prev-fix").length));
                    $("#employee-password").val(data.employeePassword);
                }
                
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.chooseFromOrgChart = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_employee_choose_from_org_chart_key = "cache_employee_choose_from_org_chart_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);

        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Employee_ChooseFromOrgChart", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                        }).then(function () {
                            var dtd = $.Deferred();
                            $.get("/orgchart/system/QueryDepartmentEmployee", function (result) {
                                var orgChart = $.fn.zTree.init($("#componentOrgChart"), {
                                    callback: {
                                        onClick: function (event, treeId, treeNode) {
                                            $("#componentEmployee tbody").empty();
                                            $(result.departmentEmployees).each(function () {
                                                if (this.departmentId === treeNode.userData.id) {
                                                    $(this.employees).each(function () {
                                                        $("#componentEmployee tbody").append("<tr data-id='" + this.id + "'><td>" + this.number + "</td><td>" + this.name + "</td><td>" + this.jobName + "</td></tr>");
                                                    });
                                                    $("#componentEmployee>tbody>tr").click(function () {
                                                        $("#componentEmployee>tbody>tr").removeClass("success");
                                                        $(this).addClass("success");
                                                    });
                                                    return false;
                                                }
                                            });
                                        }
                                    }
                                }, [result.department]);

                                dtd.resolve();
                            });
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_choose_from_org_chart_key);
                }
                else {
                    util.cache(cache_employee_choose_from_org_chart_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    employeeId: $("#componentEmployee tr.success").data("id")
                };
                util.cache(cache_employee_choose_from_org_chart_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_choose_from_org_chart_key);
                if (!data) {
                    return this;
                }
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.chooseFromNoneJob = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_employee_choose_from_none_job_key = "cache_employee_choose_from_none_job_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);

        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Employee_ChooseFromNoneJob", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                        }).then(function () {
                            $("#componentNoneJob>tbody>tr").click(function () {
                                $("#componentNoneJob>tbody>tr").removeClass("success");
                                $(this).addClass("success");
                            });

                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_choose_from_none_job_key);
                }
                else {
                    util.cache(cache_employee_choose_from_none_job_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    employeeId: $("#componentNoneJob tr.success").data("id")
                };
                util.cache(cache_employee_choose_from_none_job_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_choose_from_none_job_key);
                if (data) {
                    $("#componentNoneJob>tbody>tr[data-id='" + data.employeeId + "']").addClass("success");
                }

                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.chooseMethod = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_employee_choose_method_key = "cache_employee_choose_method_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    component.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);
        var component = {
            init: function () {
                options.beforeInit.call(component,options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Employee_ChooseMethod", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component,options);
                    });

                return this;
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    data = util.cache(cache_employee_choose_method_key);
                    return data;
                }
                else {
                    util.cache(cache_employee_choose_method_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    method: $('input[name="chooseMethod"]:checked ').val()
                };
                util.cache(cache_employee_choose_method_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_choose_method_key);
                if (data) {
                    $('input[name="chooseMethod"][value="' + data.method + '"]').prop("checked", true);
                }
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }


    oc.employee.multiple = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */
        var cache_employee_multiple_key = "cache_employee_multiple_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        component.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                groupName:"选择操作",
                buttons: [{
                    text: "输入员工信息",
                    init: function () {
                        component.getModal().modal("hide");
                    }
                },
                {
                    text: "从组织结构选择",
                    init: function () {
                        component.getModal().modal("hide");
                    }
                },
                {
                    text: "从无职务人员选择",
                    init: function () {
                        component.getModal().modal("hide");
                    }
                },
                {
                    text: "操作完成，提交数据",
                    init: function () {
                        component.getModal().modal("hide");
                    }
                }]
            }
        };
        var options = $.extend(defaults, options);

        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Employee_Multiple", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");

                        }).then(function () {
                            $("#componentMultipleEmployee button.delete").click(function () {
                                $(this).closest("tr").remove();
                            });

                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_multiple_key);
                }
                else {
                    util.cache(cache_employee_multiple_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                /*
                 *data-item={
                 *     employeeId,
                 *     employeeNumber,
                 *     employeeName,
                 *     employeePassword,
                 *     chooseAddUserMethod:来源
                 * }
                 * 
                 */
                var data = [];
                $("#componentMultipleEmployee>tbody>tr").each(function () {
                    data.push($(this).data("val"));
                });
                util.cache(cache_employee_multiple_key, data);
                return this;
            },
            restore: function () {
                var c = this;
                var data = util.cache(cache_employee_multiple_key);
                if (!data) {
                    return this;
                }
                $(data).each(function () {
                    var chooseAddUserMethodText;
                    if (this.chooseAddUserMethod === "EnterEmployeeInfo") {
                        chooseAddUserMethodText = "新建员工";
                    } else if (this.chooseAddUserMethod === "ChooseFromOrgChart") {
                        chooseAddUserMethodText = "从组织结构选择";
                    }
                    else {
                        chooseAddUserMethodText = "从无职务人员选择";
                    }
                    var $tr = $("<tr data-val='" + JSON.stringify(this) + "'><td>" + this.employeeNumber + "</td><td>" + this.employeeName + "</td><td>" + chooseAddUserMethodText + "</td><td><button type='button' class='btn btn-danger btn-sm delete'>删除</button></td></tr>");
                    $tr.find(".delete").click(function () {
                        $(this).closest("tr").remove();
                        c.persist();
                    });
                    $("#componentMultipleEmployee>tbody").append($tr);
                });
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.chooseMultipleFromOrgChart = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_employee_choose_multiple_from_org_chart_key = "cache_employee_choose_multiple_from_org_chart_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);
        var employees;
        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();

                        $.get("/orgchart/system/Employee_ChooseMultipleFromOrgChart", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html).dblclick;

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                        }).then(function () {
                            var dtd = $.Deferred();
                            $.get("/orgchart/system/QueryDepartmentEmployee", function (result) {
                                employees = result;
                                var orgChart = $.fn.zTree.init($("#componentMultipleOrgChart"), {
                                    callback: {
                                        onClick: function (event, treeId, treeNode) {
                                            $("#componentMultipleEmployeeItems tbody").empty();
                                            $(result.departmentEmployees).each(function () {
                                                if (this.departmentId === treeNode.userData.id) {
                                                    $(this.employees).each(function () {
                                                        $("#componentMultipleEmployeeItems tbody").append("<tr data-id='" + this.id + "'><td>" + this.number + "</td><td>" + this.name + "</td><td>" + this.jobName + "</td></tr>");
                                                    });
                                                    $("#componentMultipleEmployeeItems>tbody>tr").click(function () {
                                                        $("#componentMultipleEmployeeItems>tbody>tr").removeClass("success");
                                                        $(this).addClass("success");
                                                    }).dblclick(function () {
                                                        var employeeId = $(this).data("id");
                                                        var exists = false;
                                                        $("#componentMultipleEmployee>tbody>tr").each(function () {
                                                            if ($(this).data("id") === employeeId) {
                                                                exists = true;
                                                                $("#componentMultipleEmployee>tbody>tr").removeClass("success");
                                                                $(this).addClass("success");
                                                                return false;
                                                            }
                                                        });
                                                        if (exists) {
                                                            alert("该人员再已选人员中已存在");
                                                            return;
                                                        }

                                                        var $tr = $(this).clone().removeClass("success");
                                                        $tr.click(function () {
                                                            $("#componentMultipleEmployee>tbody>tr").removeClass("success");
                                                            $(this).addClass("success");
                                                        }).dblclick(function () {
                                                            $(this).remove();
                                                        });
                                                        $("#componentMultipleEmployee").append($tr);
                                                    });
                                                    return false;
                                                }
                                            });
                                        }
                                    }
                                }, [result.department]);

                                dtd.resolve();
                            });

                            return dtd.promise();
                        }).then(function () {
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data, restore) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_choose_multiple_from_org_chart_key);
                }
                else {
                    util.cache(cache_employee_choose_multiple_from_org_chart_key, data);
                    if (restore) {
                        this.restore();
                    }
                    return this;
                }
            },
            persist: function () {
                var data = [];
                $("#componentMultipleEmployee>tbody>tr").each(function () {
                    data.push({
                        employeeId: $(this).data("id"),
                        employeeName: $(this).find("td:first").text(),
                        employeeNumber:$(this).find("td:eq(1)").text()
                    });
                });
                util.cache(cache_employee_choose_multiple_from_org_chart_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_choose_multiple_from_org_chart_key);
                if (!data) {
                    return this;
                }
                $(data).each(function () {
                    var employee;
                    var employeeId = this.employeeId;
                    $(employees.departmentEmployees).each(function () {
                        $(this.employees).each(function () {
                            if (this.id === employeeId) {
                                employee = this;
                            }
                        });
                    });
                    var $tr = $("<tr data-id='" + employeeId + "'><td>" + employee.number + "</td><td>" + employee.name + "</td><td>" + employee.jobName + "</td></tr>");
                    $tr.click(function () {
                        $("#componentMultipleEmployee>tbody>tr").removeClass("success");
                        $(this).addClass("success");
                    }).dblclick(function () {
                        $(this).remove();
                    });
                    $("#componentMultipleEmployee").append($tr);
                });
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.employee.chooseMultipleFromNoneJob = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var modalId;
        var cache_employee_choose_multiple_from_none_job_key = "cache_employee_choose_multiple_from_none_job_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);

        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Employee_ChooseMultipleFromNoneJob", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            modalId = $html.prop("id");
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.on("click", "#" + modalId + "componentMultipleNoneJobItems>tbody>tr", function () {
                                $("#" + modalId + "componentMultipleNoneJobItems>tbody>tr").removeClass("success");
                                $(this).addClass("success");
                            }).on("dblclick", "#" + modalId + "componentMultipleNoneJobItems>tbody>tr", function () {
                                $("#" + modalId + "componentMultipleNoneJobItems>tbody>tr").removeClass("success");
                                $(this).addClass("success");

                                $("#" + modalId + "componentMultipleNoneJob>tbody>tr").removeClass("success");

                                var $tr = $(this);
                                var stop = false;
                                $("#" + modalId + "componentMultipleNoneJob>tbody>tr").each(function () {
                                    if ($(this).data("id") === $tr.data("id")) {
                                        stop = true;
                                        $(this).addClass("success");
                                        return false;
                                    }
                                });
                                if (stop) {
                                    return;
                                }
                                $("#" + modalId + "componentMultipleNoneJob>tbody").append($(this).clone());
                            }).on("click", "#" + modalId + "componentMultipleNoneJob>tbody>tr", function () {
                                $("#" + modalId + "componentMultipleNoneJob>tbody>tr").removeClass("success");
                                $(this).addClass("success");
                            }).on("dblclick", "#" + modalId + "componentMultipleNoneJob>tbody>tr", function () {
                                $(this).remove();
                            });

                            $modal.modal("show");
                        }).then(function () {
                            $("#" + modalId + "componentMultipleNoneJobItems>tbody>tr").click(function () {
                                $("#" + modalId + "componentMultipleNoneJobItems>tbody>tr").removeClass("success");
                                $(this).addClass("success");
                            });

                            dtd.resolve();
                        }).then(function () {
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_employee_choose_multiple_from_none_job_key);
                }
                else {
                    util.cache(cache_employee_choose_multiple_from_none_job_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = [];
                $("#" + modalId + "componentMultipleNoneJob>tbody>tr").each(function () {
                    data.push({
                        employeeId: $(this).data("id"),
                        employeeName: $(this).find("td:first").text(),
                        employeeNumber: $(this).find("td:last").text()
                    });
                });
                util.cache(cache_employee_choose_multiple_from_none_job_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_employee_choose_multiple_from_none_job_key);
                if (data) {
                    $(data).each(function () {
                        var $tr = $("<tr data-id='" + this.employeeId + "'><td>" + this.employeeNumber + "</td><td>" + this.employeeName + "</td></tr>");
                        
                        $("#" + modalId + "componentMultipleNoneJob>tbody").append($tr);
                    });
                }

                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    oc.department.enter = function (options) {
        /*
         * 填写部门信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var modalId;
        var cache_department_enter_key = "cache_department_enter_key";
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        component.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    component.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);

        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();
                        $.get("/orgchart/system/Department_Enter", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            modalId = $html.prop("id");
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html);

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            options.validate = util.validate()
                                .element(modalId + "department-name")
                                    .required()
                                    .end();

                            $modal.modal("show");
                        }).then(function () {
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data) {
                if (arguments.length === 0) {
                    return util.cache(cache_department_enter_key);
                }
                else {
                    util.cache(cache_department_enter_key, data);
                    this.restore();
                    return this;
                }
            },
            persist: function () {
                var data = {
                    departmentName: $("#" + modalId + "department-name").val()
                };
                
                util.cache(cache_department_enter_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_department_enter_key);
                if (data) {
                    $("#" + modalId + "department-name").val(data.departmentName);
                }
                return this;
            }
        };

        return component;
    }

    oc.department.chooseMultiple = function (options) {
        /*
         * 填写人员信息
         *    
         * options={
         *     data:{}组件需要的初始化数据
         *     userData:{}传给组件的其他用户数据,
         *     beforeInit:functoin(options){} dom数据加载前,编写符合Promise的方法
         *     afterInit:functoin(options){} dom数据加载后,编写符合Promise的方法,
         *     title:string 标题
         * }
         * 
         * defaults={
         *     leftButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     },
         *     rightButtons={
         *         groupName:string,按钮组的名称
         *         buttons:[{text:'按钮名称',init:function(){初始化方法}}]
         *     }
         * }
         */

        var cache_department_choose_multiple_key = "cache_department_choose_multiple_key";
        var viewBag = {};
        var defaults = {
            beforeInit: function (options) {
                var dtd = $.Deferred();
                dtd.resolve();
                return dtd.promise();
            },
            afterInit: function (options) {
                var dtd = $.Deferred();
                this.restore();
                dtd.resolve();
                return dtd.promise();
            },
            leftButtons: {
                text: "取消",
                init: function () {
                    $(this).click(function () {
                        o.getModal().modal("hide");
                    });
                }
            },
            rightButtons: {
                text: "下一步",
                init: function () {
                    o.getModal().modal("hide");
                }
            }
        };
        var options = $.extend(defaults, options);
        var departments;
        var component = {
            init: function () {
                options.beforeInit.call(component, options)
                    .then(function () {
                        var dtd = $.Deferred();

                        $.get("/orgchart/system/Department_ChooseMultiple", function (html) {
                            var $html = $(html);
                            $modal = $html;
                            $html.find(".modal-title").text(options.title);

                            $("#" + modal_box_id).empty().append($html).dblclick;

                            $modal.find(".modal-footer").append(generateLeftButtons(component));
                            $modal.find(".modal-footer").append(generateRightButtons(component));

                            $modal.modal("show");
                        }).then(function () {
                            var dtd = $.Deferred();
                            $.get("/orgchart/system/QueryChooseDepartment", function (result) {
                                departments = result;
                                
                                $("#componentMultipleDepartments tbody").empty();
                                $(result).each(function () {
                                    var $newTr = $("<tr data-id='" + this.id + "'><td>" + this.fullName + "</td></tr>");
                                    $("#componentMultipleDepartments tbody").append($newTr);

                                    $newTr.click(function () {
                                        $("#componentMultipleDepartments>tbody>tr").removeClass("success");
                                        $(this).addClass("success");
                                    }).dblclick(function () {
                                        var departmentId = $(this).data("id");
                                        var exists = false;
                                        $("#componentMultipleChosenDepartments>tbody>tr").each(function () {
                                            if ($(this).data("id") === departmentId) {
                                                exists = true;
                                                $("#componentMultipleChosenDepartments>tbody>tr").removeClass("success");
                                                $(this).addClass("success");
                                                return false;
                                            }
                                        });
                                        if (exists) {
                                            alert("该部门在已选部门中已存在");
                                            return;
                                        }

                                        var $tr = $(this).clone().removeClass("success");
                                        $tr.click(function () {
                                            $("#componentMultipleChosenDepartments>tbody>tr").removeClass("success");
                                            $(this).addClass("success");
                                        }).dblclick(function () {
                                            $(this).remove();
                                        });
                                        $("#componentMultipleChosenDepartments").append($tr);
                                    });
                                });

                                dtd.resolve();
                            });

                            return dtd.promise();
                        }).then(function () {
                            dtd.resolve();
                        });
                        return dtd.promise();
                    }).then(function () {
                        return options.afterInit.call(component, options);
                    });
            },
            getModal: function () {
                return $modal;
            },
            getOptions: function () {
                return options;
            },
            result: function (data, restore) {
                if (arguments.length === 0) {
                    return util.cache(cache_department_choose_multiple_key);
                }
                else {
                    util.cache(cache_department_choose_multiple_key, data);
                    if (restore) {
                        this.restore();
                    }
                    return this;
                }
            },
            persist: function () {
                var data = [];
                $("#componentMultipleChosenDepartments>tbody>tr").each(function () {
                    data.push({
                        departmentId: $(this).data("id"),
                        departmentFullName: $(this).find("td:first").text()
                    });
                });
                util.cache(cache_department_choose_multiple_key, data);
                return this;
            },
            restore: function () {
                var data = util.cache(cache_department_choose_multiple_key);
                if (!data) {
                    return this;
                }
                $(data).each(function () {
                    var department,
                        departmentId = this.departmentId;
                    $(departments).each(function () {
                        if (this.id === departmentId) {
                            department = this;
                            return false;
                        }
                    });
                    var $tr = $("<tr data-id='" + departmentId + "'><td>" + department.fullName + "</td></tr>");
                    $tr.click(function () {
                        $("#componentMultipleChosenDepartments>tbody>tr").removeClass("success");
                        $(this).addClass("success");
                    }).dblclick(function () {
                        $(this).remove();
                    });
                    $("#componentMultipleChosenDepartments").append($tr);
                });
                return this;
            },
            viewBag: function () {
                return viewBag;
            }
        };

        return component;
    }

    window.oc = oc;
})(jQuery,window)