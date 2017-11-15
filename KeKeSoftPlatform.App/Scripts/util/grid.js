(function ($, util) {
    
    var OPTIONS_KEY = "__options__";
    util.grid = {
        instance: function (o) {
            var $box,
                $table,
                $tbody,
                _query,
                _ajaxUrl;
            //如果o是string，那么传递的是一个jquery选择器；否则传递的是一个jquery对象
            if (o.constructor === String) {
                $box = $(o);
            } else {
                $box = o;
            }

            var _grid = {},
                options = $box.data(OPTIONS_KEY) || {};

            //创建表格头部（标题、操作按钮）
            _grid.title = function (title) {
                if (arguments.length === 0) {
                    return options.title;
                }
                options.title = title;
                return _grid;
            }
            options.tools = [];
            options.tools.push($('<button type="button" id="btnOpenSearch" class="btn btn-sm btn-primary"><i class="glyphicon glyphicon-search"></i> 查询</button>').click(function () { _grid.openSearch(); }));
            options.tools.push($('<button type="button" class="btn btn-sm btn-primary"><i class="glyphicon glyphicon-refresh"></i> 刷新</button>').click(function () { _grid.reload(); }));
            _grid.tools = function (element) {
                options.tools = options.tools || [];
                if (arguments.length === 0) {
                    return options.tools;
                }

                options.tools.push(element);
                return _grid;
            };
            _grid.clearTools = function () {

            };
            //创建查询条件
            _grid.searchGroup = function (a) {
                options.searchGroup = options.searchGroup || [];
                var labelText;
                if (a instanceof $) {
                    options.searchGroup.push(a);
                    return _grid;
                }
                else {
                    labelText = a;
                }

                return {
                    textbox: function (a, b) {
                        var id,
                            decorate;

                        var $group = $("<div>").addClass("form-group")
                                        .append('<label class="col-sm-2 control-label">' + labelText + '</label>')
                                        .append('<div class="col-sm-10"><input type="text" class="form-control" placeholder="' + labelText + '" /></div>');
                        if (arguments.length === 1 && a.constructor === String) {
                            id = a;
                        }
                        if (arguments.length === 1 && a.constructor === Function) {
                            decorate = a;
                        }
                        if (arguments.length === 2) {
                            id = a;
                            decorate = b;
                        }
                        if (id) {
                            $group.find("input").attr("id", id).attr("name",id);
                        }
                        if (decorate) {
                            decorate($group);
                        }
                        options.searchGroup.push($group);
                        return _grid;
                    },
                    checkbox: function (a, b) {
                        var id,
                            decorate;

                        var $group = $("<div>").addClass("form-group")
                                        .append('<div class="col-sm-offset-2 col-sm-10"><div class="checkbox"><label><input type="checkbox"  /> ' + labelText + '</label></div></div>');
                        if (arguments.length === 1 && a.constructor === String) {
                            id = a;
                        }
                        if (arguments.length === 1 && a.constructor === Function) {
                            decorate = a;
                        }
                        if (arguments.length === 2) {
                            id = a;
                            decorate = b;
                        }
                        if (id) {
                            $group.find("input").attr("id", id).attr("name", id);
                        }
                        if (decorate) {
                            decorate($group);
                        }
                        options.searchGroup.push($group);
                        return _grid;
                    },
                    range: function (a, b, c) {
                        var minDateId,
                            maxDateId,
                            decorate;

                        var $group = $("<div>").addClass("form-group")
                                        .append('<label class="col-sm-2 control-label">' + labelText + '</label>')
                                        .append('<div class="col-sm-10"><div class="input-group"></div></div>');
                        $group.find(".input-group").append('<input type="text" class="form-control">');
                        $group.find(".input-group").append('<span class="input-group-addon">-</span>');
                        $group.find(".input-group").append('<input type="text" class="form-control">');
                        if (arguments.length === 1 && a.constructor === String) {
                            minDateId = a;
                        }
                        if (arguments.length === 1 && a.constructor === Function) {
                            decorate = a;
                        }
                        if (arguments.length === 2) {
                            minDateId = a;
                            maxDateId = b;
                        }
                        if (arguments.length === 3) {
                            minDateId = a;
                            maxDateId = b;
                            decorate = c;
                        }
                        if (minDateId) {
                            $group.find("input:first").attr("id", minDateId).attr("name", minDateId);
                        }
                        if (maxDateId) {
                            $group.find("input:last").attr("id", maxDateId).attr("name", maxDateId);
                        }
                        if (decorate) {
                            decorate($group);
                        }
                        options.searchGroup.push($group);
                        return _grid;
                    },
                    select: function (a, b, c) {
                        var id,
                            decorate;

                        var $group = $("<div>").addClass("form-group")
                                        .append('<label class="col-sm-2 control-label">' + labelText + '</label>')
                                        .append('<div class="col-sm-10"></div>');
                        var selectItems = $(a).map(function () {
                            if (this.selected) {
                                return "<option selected='selected' value='" + this.value + "'>" + this.text + "</option>";
                            }
                            else {
                                return "<option value='" + this.value + "'>" + this.text + "</option>";
                            }
                        }).get().join(" ");
                        $group.find(".col-sm-10").append("<select class='form-control'>" + selectItems + "</select>");

                        if (arguments.length === 2 && b.constructor === String) {
                            id = b;
                        }
                        if (arguments.length === 2 && b.constructor === Function) {
                            decorate = b;
                        }
                        if (arguments.length === 3) {
                            id = b;
                            decorate = c;
                        }
                        if (id) {
                            $group.find("select").attr("id", id).attr("name", id);
                        }
                        if (decorate) {
                            decorate($group);
                        }
                        options.searchGroup.push($group);
                        return _grid;
                    },
                    custom: function (label, element) {
                        var $group = $("<div>").addClass("form-group")
                                        .append('<label class="col-sm-2 control-label">' + labelText + '</label>')
                                        .append('<div class="col-sm-10"></div>');

                        $group.find(".col-sm-10").append(element);

                        options.searchGroup.push($group);
                        return _grid;
                    }
                };
            }

            //打开查询窗口
            _grid.openSearch = function () {
                $search.modal("show");
            }

            var $search;
            function initSearch() {
                var modalHtml = '<div class="modal fade " tabindex="-1" role="dialog" aria-hidden="true" >' +
                                    '<div class="modal-dialog">' +
                                        '<div class="modal-content">' +
                                            '<div class="modal-header">' +
                                                '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
                                                '<h4 class="modal-title">综合查询</h4>' +
                                            '</div>' +
                                            '<div class="modal-body">' +
                                                '<form class="form-horizontal" action="#" method="post">' +

                                                '</form>' +
                                            '</div>' +
                                            '<div class="modal-footer">' +
                                                '<button class="btn btn-primary"><i class="glyphicon glyphicon-search"></i> 查询</button>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>';

                var $modal = $(modalHtml);
                $modal.find(".modal-footer button.btn").click(function () {
                    $modal.modal("hide");
                    load(1);
                });
                $(options.searchGroup).each(function () {
                    $modal.find("form").append(this);
                });
                $search = $modal;

                if (options.onSearchInit) {
                    options.onSearchInit($modal);
                }
            }

            //查询窗体创建完成事件
            _grid.onSearchInit = function (callback) {
                options.onSearchInit = callback;
                return _grid;
            }

            //创建列
            _grid.column = function (name) {
                options.columns = options.columns || [];
                var _value;//单元格值计算器，类型为function(item){}
                var _column = {
                    end: function () {
                        return _grid;
                    },
                    name: function () {
                        return name;
                    },
                    value: function (a, b, c) {
                        if (arguments.length === 0) {
                            if (!_value) {
                                throw "grid单元格值计算器未设置";
                            }
                            return _value;
                        }

                        if (!a) {
                            throw "参数不能为空";
                        }
                        if (arguments.length === 1 && a.constructor === String) {
                            _value = function (item) {
                                return item[a];
                            }
                            return this;
                        }
                        if (arguments.length === 1 && a.constructor === Function) {
                            _value = a;
                            return this;
                        }
                        if (arguments.length === 2 && a.constructor === String && b.constructor === Object) {
                            _value = function (item) {
                                for (var key in b) {
                                    if (b.hasOwnProperty(key) && b[key].value === item[a]) {
                                        return b[key].text;
                                    }
                                }
                            }
                            return this;
                        }
                        if (arguments.length === 3) {
                            _value = function (item) {
                                if (item[a]) {
                                    return b;
                                }
                                else {
                                    return c;
                                }
                            }
                            return this;
                        }
                    }
                }
                options.columns.push(_column);

                return _column;
            };

            //设置数据
            _grid.data = function (a) {
                if (a.constructor === Function) {
                    _query = a;
                    return _grid;
                }
                if (a.constructor === String) {
                    _ajaxUrl = a;
                    return _grid;
                }
            }

            //添加数据行
            function insertRow(item) {
                var $tr = $("<tr>");
                for (var i = 0; i < options.columns.length; i++) {
                    var $td = $("<td>");

                    $td.append(options.columns[i].value()(item));
                    $tr.append($td);
                }
                $tbody.append($tr);

                if (options.onRowInserted) {
                    options.onRowInserted(_grid,$tr,item);
                }
            }

            //加载数据
            function load(pageNum) {
                if (options.userPage && !pageNum) {
                    pageNum = 1;
                }
                $tbody.empty();
                var parameters = $search.find("form").serializeArray();
                if (_ajaxUrl) {
                    $.blockUI();

                    if (options.userPage) {
                        parameters.push({ name: 'pageNum', value: pageNum });
                    }
                    var url;
                    if (_ajaxUrl.indexOf("?") > -1) {
                        url = _ajaxUrl + "&" + $.param(parameters);
                    } else {
                        url = _ajaxUrl + "?" + $.param(parameters);
                    }

                    $.get(url, function (result) {
                        var data = result.data;
                        if (options.userPage) {
                            page(result.data);
                            data = result.data.data;
                        }
                        $(result.data.data).each(function () {
                            insertRow(this);
                        });
                        
                        
                        if (options.onLoaded) {
                            options.onLoaded(_grid, result.data);
                        }
                        $.unblockUI();
                    });
                }
                else {
                    $(_query(parameters)).each(function () {
                        insertRow(this);
                    });
                    if (options.onLoaded) {
                        options.onLoaded(_grid, result.data);
                    }
                }
            }

            //创建分页
            function page(pager) {
                $box.find(".box-footer").empty();
                if (pager.itemCount === 0) {
                    $box.find(".box-footer").append($("<div>").addClass("pull-right hidden-xs pagination-tongji").text("无数据"));
                    return;
                }
                var $left = $("<div>").addClass("pull-left hidden-xs pagination-tongji")
                                        .text("共 " + pager.itemCount + " 条记录 当前第 " + pager.pageNum + "/" + pager.pageCount + " 页 每页 " + pager.pageSize + " 条");

                var $right = $("<div>").addClass("pagination pagination-sm no-margin pull-right");
                if (pager.pageNum === 1) {
                    $right.append($('<li class="disabled"><a href="#">«</a></li>'));
                    $right.append($('<li class="disabled"><a href="#">&lsaquo;</a></li>'));
                }
                else {
                    $right.append($('<li><a href="#">«</a></li>').find("a").click(function () { load(1);}).end());
                    $right.append($('<li><a href="#">&lsaquo;</a></li>').find("a").click(function () { load(pager.pageNum - 1); }).end());
                }

                var n,
                    min = 0,
                    max = 0;

                for (n = 1; n <= 5; n++) {
                    if ((pager.pageNum - n) % 4 == 0) {
                        min = pager.pageNum - (n - 1);
                        max = pager.pageNum + (5 - n);
                        break;
                    }
                }

                for (n = min; n <= Math.min(max,pager.pageCount); n++) {
                    if (n === pager.pageNum) {
                        $right.append('<li class="active"><a href="#">' + n + '</a></li>');
                    }
                    else {
                        $right.append($('<li><a href="#">' + n + '</a></li>').find("a").click((function (n) {
                            return function () { load(n) };
                        })(n)).end());
                    }
                }

                if (pager.pageNum === pager.pageCount||pager.pageCount===0) {
                    $right.append($('<li class="disabled"><a href="#">&rsaquo;</a></li>'));
                    $right.append($('<li class="disabled"><a href="#">»</a></li>'));
                }
                else {
                    $right.append($('<li><a href="#">&rsaquo;</a></li>').find("a").click(function () { load(pager.pageNum + 1); }).end());
                    $right.append($('<li><a href="#">»</a></li>').find("a").click(function () { load(pager.pageCount); }).end());
                }


                

                $(".box-footer").append($left).append($right);
            }

            //重新加载数据
            _grid.reload = function () {
                if ($box.find(".box-footer li.active a").length === 0) {
                    load(1);
                    return;
                }
                load(parseInt($box.find(".box-footer li.active a").text()));
                return;
            };

            //初始化
            _grid.init = function () {
                //持久化options数据
                $box.data(OPTIONS_KEY, options);

                $box.addClass("box")
                    .append($("<div>")
                                .addClass("box-header with-border")
                                .append($("<h3>").text(options.title).addClass("box-title"))
                                .append($("<div>").addClass("box-tools")))
                    .append($("<div>")
                                .addClass("box-body table-responsive no-padding")
                                .append($("<table class='table table-hover'></table>")));

                if (options.userPage) {
                    $box.append($("<div>")
                                .addClass("box-footer clearfix"));
                }

                //添加工具栏
                if (options.tools) {
                    var $box_tools = $box.find(".box-tools");
                    $(options.tools).each(function () {
                        $box_tools.append(this).append(" ");
                    });
                }

                //初始化查询窗体
                initSearch();

                //创建table
                //创建表头
                $table = $box.find("table");
                var $thead = $("<thead>");
                var $thead_tr = $("<tr>").appendTo($thead);
                $(options.columns).each(function () {
                    var $th = $("<th>");
                    $th.text(this.name());
                    $th.appendTo($thead_tr);
                });
                $table.append($thead);

                //创建数据行
                $tbody = $("<tbody>").appendTo($table);
                
                //加载数据
                load(1);
                
                if (options.onInitialized) {
                    options.onInitialized(_grid);
                }
                return _grid;

            };

            //表格初始化，只执行一次
            _grid.onInitialized = function (callback) {
                options.onInit = callback;
                return _grid;
            }

            //数据加载完毕，每次加载数据都会执行
            _grid.onLoaded = function (callback) {
                options.onLoad = callback;
                return _grid;
            }

            //行数据插入到表格之后
            _grid.onRowInserted = function (callback) {
                options.onRowInserted = callback;
                return _grid;
            }

            //使用分页，使用分页，那么返回数据的格式需要加上分页数据
            _grid.userPage = function () {
                options.userPage = true;
                return _grid;
            }

            return _grid;
        }
    };

    window.util = util;
})(jQuery, window.util || {})