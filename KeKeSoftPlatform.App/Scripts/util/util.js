(function ($, util) {

    util.isNull = function (str) {
        return (str == undefined || str == null || str.trim() == "");
    };

    util.toDecimal = function (value, pos) {
        return Math.round(value * Math.pow(10, pos)) / Math.pow(10, pos);
    };

    util.date = {
        format: function (date, fmt) {
            date = date == undefined ? new Date() : date;
            date = typeof date == 'number' ? new Date(date) : date;
            fmt = fmt || 'yyyy-MM-dd HH:mm:ss';
            var obj =
            {
                'y': date.getFullYear(), // 年份，注意必须用getFullYear
                'M': date.getMonth() + 1, // 月份，注意是从0-11
                'd': date.getDate(), // 日期
                'q': Math.floor((date.getMonth() + 3) / 3), // 季度
                'w': date.getDay(), // 星期，注意是0-6
                'H': date.getHours(), // 24小时制
                'h': date.getHours() % 12 == 0 ? 12 : date.getHours() % 12, // 12小时制
                'm': date.getMinutes(), // 分钟
                's': date.getSeconds(), // 秒
                'S': date.getMilliseconds() // 毫秒
            };
            var week = ['天', '一', '二', '三', '四', '五', '六'];
            for (var i in obj) {
                fmt = fmt.replace(new RegExp(i + '+', 'g'), function (m) {
                    var val = obj[i] + '';
                    if (i == 'w') return (m.length > 2 ? '星期' : '周') + week[val];
                    for (var j = 0, len = val.length; j < m.length - len; j++) val = '0' + val;
                    return m.length == 1 ? val : val.substring(val.length - m.length);
                });
            }
            return fmt;
        }
    }

    var cache = [];
    util.cache = function (key, value) {
        if (arguments === 0) {
            return undefined;
        }
        if (arguments[0].constructor !== String) {
            throw "key必须是字符串"
        }
        if (arguments.length === 1) {
            for (var i = 0; i < cache.length; i++) {
                if (cache[i].key === key) {
                    return cache[i].value;
                }
            }

            return undefined;
        }
        else {
            for (var i = 0; i < cache.length; i++) {
                if (cache[i].key === key) {
                    cache[i].value = value;
                    return;
                }
            }

            cache.push({ key: key, value: value });
            return;
        }
    };

    util.random = function (prevFix) {
        var timestamp = (new Date()).valueOf().toString();
        var right = Math.round(Math.random() * 1000000).toString();
        for (var i = 1; i <= right.length - 6; i++) {
            right = "0" + right;
        }
        if (prevFix) {
            return prevFix + timestamp + right;
        }
        return timestamp + right;
    }

    util.validate = function () {
        /*
         * validation:{
         *     id:string,
         *     rules:[{
         *         message:string,
         *         validator:function(value){}
         *     }]
         * }
         */
        var validations = [];
        var validators = {
            required: function (value) {
                if (value) { return true; }
                else { return false; }
            },
            must: function (value, validator) {
                return validator(value);
            },
            match: function (value, reg) {
                return reg.test(value);
            },
            async: function (value, fn, success, fail) {
                fn(value, success, fail);
                return true;
            }
        };
        var validatorMessages = {
            required: "必填项",
            must: "输入错误",
            match: "输入错误",
            async: "异步验证失败"
        };

        var validOne = function (validation) {
            for (var j = 0; j < validation.rules.length; j++) {
                if (validation.rules[j].validator()()) {
                    var $valmsg = $(".help-block[data-valmsg-for='" + validation.id + "']");
                    if ($valmsg.length !== 0) {
                        $valmsg.text("");
                    }

                    $("#" + validation.id).closest(".form-group").removeClass("has-error");
                }
                else {
                    //显示错误信息
                    var $valmsg = $(".help-block[data-valmsg-for='" + validation.id + "']");
                    if ($valmsg.length === 0) {
                        $valmsg = $("<span class='help-block' data-valmsg-for='" + validation.id + "'></span>");
                        $("#" + validation.id).after($valmsg);
                    }
                    $valmsg.text(validation.rules[j].message);
                    $("#" + validation.id).closest(".form-group").addClass("has-error");
                    return false;
                }
            }

            return true;
        };

        var ruleSuccess = function (id) {
            var $valmsg = $(".help-block[data-valmsg-for='" + id + "']");
            if ($valmsg.length !== 0) {
                $valmsg.text("");
            }

            $("#" + id).closest(".form-group").removeClass("has-error");
        }

        var ruleFail = function (id) {
            //debugger;
            //显示错误信息
            var $valmsg = $(".help-block[data-valmsg-for='" + id + "']");
            if ($valmsg.length === 0) {
                $valmsg = $("<span class='help-block' data-valmsg-for='" + id + "'></span>");
                $("#" + id).after($valmsg);
            }
            $valmsg.text(this.message);
            $("#" + id).closest(".form-group").addClass("has-error");
        }

        var validate = {
            element: function (id) {
                $("#" + id).blur(function () {
                    validOne(validation);
                });
                var validation = {
                    id: id,
                    rules: []
                };
                validations.push(validation);
                return {
                    required: function () {
                        validation.rules.push({
                            message: validatorMessages.required,
                            validator: function () {
                                return function () {
                                    return validators.required($("#" + id).val());
                                }
                            }
                        });
                        return this;
                    },
                    must: function (validator) {
                        validation.rules.push({
                            message: validatorMessages.must,
                            validator: function () {
                                return function () {
                                    return validators.must($("#" + id).val(), validator);
                                }
                            }
                        });

                        return this;
                    },
                    match: function (reg) {
                        validation.rules.push({
                            message: validatorMessages.match,
                            validator: function () {
                                return function () {
                                    return validators.match($("#" + id).val(), reg);
                                }
                            }
                        });

                        return this;
                    },
                    async: function (fn) {
                        /*
                         * fn:function(value,success,fail){}
                         */
                        validation.rules.push({
                            message: validatorMessages.async,
                            validator: function () {
                                var rule = this;
                                return function () {
                                    return validators.async($("#" + id).val(), fn, function () {
                                        ruleSuccess.call(rule, id);
                                    }, function () {
                                        ruleFail.call(rule, id);
                                    });
                                }
                            }
                        });

                        return this;
                    },
                    message: function (message) {
                        if (validation.rules.length === 0) {
                            throw "验证器没有添加需要验证的元素";
                        }
                        validation.rules[validation.rules.length - 1].message = message;
                        return this;
                    },
                    end: function () {
                        return validate;
                    }
                };
            },
            valid: function () {
                var isValid = true;
                for (var i = 0; i < validations.length; i++) {
                    if (!validOne(validations[i])) {
                        isValid = false;
                    }
                }

                return isValid;
            }
        };

        return validate;
    };

    util.getHtmlBox = function () {
        var boxId = "__dynamic_html_box__"
        var $box = $("#" + boxId);
        if ($box.length === 0) {
            $box = $("<div>").attr("id", boxId);
            $(document.body).append($box);
        }
        return $box;
    }

    util.tip = function (message) {
        var $tip = $("<div>").addClass("alert alert-success alert-dismissable");
        $tip.append('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>');
        $tip.append('<strong>提示</strong>');
        $tip.append(message);

        $("#__alert_message__").append($tip);
    }

    util.warn = function (message) {
        var $tip = $("<div>").addClass("alert alert-danger alert-dismissable");
        $tip.append('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>');
        $tip.append('<strong>提示</strong>');
        $tip.append(message);

        $("#__alert_message__").append($tip);
    }

    util.form = function () {
        var form,
            $modal,
            options = {
                cache: true,
                params: {}
            };
        form = {
            title: function (title) {
                options.title = title;
                return this;
            },
            url: function (url) {
                options.url = url;
                return this;
            },
            params: function (params) {
                options.params = params || {};
                return this;
            },
            onLoad: function (onLoad) {
                options.onLoad = onLoad;
                return this;
            },
            onShown: function (onShown) {
                options.onShown = onShown;
                return this;
            },
            cache: function (enable) {
                options.cache = enable;
                return this;
            },
            onSuccess: function (onSuccess) {
                options.onSuccess = onSuccess;
                return this;
            },
            onFail: function (onFail) {
                options.onFail = onFail;
                return this;
            },
            beforeSubmit: function (beforeSubmit) {
                options.beforeSubmit = beforeSubmit;
                return this;
            },
            close: function () {
                $modal.modal("hide");
            },
            open: function () {
                if ($.blockUI) {
                    $.blockUI();
                }
                else {
                    console.warn("$.blockUI未引入");
                }
                var $box = util.getHtmlBox().empty();
                if (!options.cache) {
                    options.params.timestamp = new Date().getTime();
                }
                $.get(options.url, options.params, function (result) {
                    if ($.unblockUI) {
                        $.unblockUI();
                    }
                    var modalHtml = '<div class="modal fade " tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">' +
                                    '<div class="modal-dialog">' +
                                        '<div class="modal-content">' +
                                            '<div class="modal-header">' +
                                                '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
                                                '<h4 class="modal-title"></h4>' +
                                            '</div>' +
                                            '<div class="modal-body">' +
                                            '</div>' +
                                            '<div class="modal-footer">' +
                                                '<button class="btn btn-primary"><i class="glyphicon glyphicon-floppy-disk"></i> 提交</button>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>';

                    $modal = $(modalHtml);
                    $modal.find(".modal-title").text(options.title);
                    $modal.find(".modal-body").append(result);
                    $modal.find("form").keypress(function (event) {
                        if (event.keyCode === 13) {
                            event.preventDefault();
                            return;
                        }
                    });
                    $box.append($modal);
                    if (options.onLoad) {
                        options.onLoad($modal);
                    }
                    $modal.find(".modal-footer button").click(function () {
                        if (options.beforeSubmit) {
                            options.beforeSubmit($modal);
                        }
                        if ($modal.find("form").valid() === false) {
                            return;
                        }

                        if ($.blockUI) {
                            $.blockUI();
                        }
                        $.ajax({
                            url: $modal.find("form").prop("action"),
                            type: $modal.find("form").prop("method"),
                            data: $modal.find("form").serialize(),
                            success: function (result) {
                                if (result.constructor === Object) {
                                    if (result.isSuccess && options.onSuccess) {
                                        options.onSuccess.call(form, result.data);
                                        return;
                                    }
                                    if (result.isSuccess == false && options.onFail) {
                                        options.onFail.call(form, result.error);
                                        return;
                                    }
                                    form.close();
                                    return;
                                }


                                $modal.find(".modal-body").empty().append(result);
                                if (options.onLoad) {
                                    options.onLoad($modal);
                                }
                                if (options.onShown) {
                                    options.onShown($modal);
                                }
                            },
                            error: function (result) {
                                $.toast({
                                    text: "系统异常，稍后重试或联系管理员", // Text that is to be shown in the toast
                                    heading: '提示', // Optional heading to be shown on the toast
                                    icon: 'error', // Type of toast icon
                                    position: 'mid-center'
                                });
                                console.log(result);
                            },
                            complete: function () {
                                if ($.unblockUI) {
                                    $.unblockUI();
                                }
                            }
                        });
                    });
                    $.validator.unobtrusive.parse($modal.find("form"));

                    if (options.onShown) {
                        $modal.on('shown.bs.modal', function (e) {
                            options.onShown($modal);
                        })
                    }
                    $modal.modal("show");
                    return;
                });
            }
        };

        return form;
    };

    util.page = function () {
        var page,
            $modal,
            options = {
                cache: true,
                params: {}
            };
        page = {
            title: function (title) {
                options.title = title;
                return this;
            },
            url: function (url) {
                options.url = url;
                return this;
            },
            params: function (params) {
                options.params = params || {};
                return this;
            },
            onLoad: function (onLoad) {
                options.onLoad = onLoad;
                return this;
            },
            onShown: function (onShown) {
                options.onShown = onShown;
                return this;
            },
            cache: function (enable) {
                options.cache = enable;
                return this;
            },
            close: function () {
                $modal.modal("hide");
            },
            open: function () {
                if ($.blockUI) {
                    $.blockUI();
                }
                else {
                    console.warn("$.blockUI未引入");
                }
                var $box = util.getHtmlBox().empty();
                if (!options.cache) {
                    options.params.timestamp = new Date().getTime();
                }
                $.get(options.url, options.params, function (result) {
                    if ($.unblockUI) {
                        $.unblockUI();
                    }
                    var modalHtml = '<div class="modal fade " tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">' +
                        '<div class="modal-dialog">' +
                        '<div class="modal-content">' +
                        '<div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
                        '<h4 class="modal-title"></h4>' +
                        '</div>' +
                        '<div class="modal-body">' +
                        '</div>' +
                        '<div class="modal-footer">' +
                        '<button class="btn btn-default" data-dismiss="modal"> 关闭</button>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>';

                    $modal = $(modalHtml);
                    $modal.find(".modal-title").text(options.title);
                    $modal.find(".modal-body").append(result);
                    $box.append($modal);
                    if (options.onLoad) {
                        options.onLoad($modal);
                    }
                    if (options.onShown) {
                        $modal.on('shown.bs.modal', function (e) {
                            if (options.onShown) {
                                options.onShown.call(page, $modal);
                            }
                        })
                    }
                    $modal.modal("show");
                    return;
                });
            }
        };

        return page;
    };

    util.textOverflow = function ($elements) {
        $elements = $elements || $(document);
        if ($elements.is("[data-text-overflow='true']")) {
            $elements = $elements.add($elements.find("[data-text-overflow='true']"));
        }
        else {
            $elements = $elements.find("[data-text-overflow='true']");
        }

        $elements.each(function () {
            $(this).width($(this).width());
            $(this).addClass("cut");
        });
    };

    window.util = util;
})(jQuery, window.util || {});


