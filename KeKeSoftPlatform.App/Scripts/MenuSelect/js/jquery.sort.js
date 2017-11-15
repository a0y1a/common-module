///**
//$(window).load(function () {
//    //要执行的方法体
//  intProvince(jsonData);
//});
//*/

//var expressP, expressC, expressD, expressArea, areaCont;
//var arrow = " <font>&gt;</font> ";
//var dataJson = '';
//function intProvince(jsonData) {
//    dataJson = jsonData;
//    if (dataJson.length > 0) {
//        var dataObject = JSON.parse(dataJson);
//        areaCont = "";
//        $.each(dataObject, function (index, item) {
//            if (!item.ParentId || item.ParentId.length <= 0) {
//                var right = "";
//                if (parseInt(item.ChildrenCount) > 0) {
//                    right = "<label>></label>";
//                }
//                areaCont += '<li style="width:380px" id="' + item.Id + 'li" onClick="selectM(\'' + item.Id + '\',\'' + 1 + '\',\'' + item.Value + '\');"><a href="javascript:void(0)" class="you" >' + item.Value + right + '</a></li>';
//            }
//        });
//        $("#sort1").html(areaCont);
//    }
//}

//function selectM(id, cla, pName) {
//    var nextCla = parseInt(cla) + 1;
//    $("#urDiv ul").each(function () {
//        if (parseInt($(this).attr("data-cla")) >= nextCla) {
//            $(this).remove();
//        }
//    });
//    var dataObject = JSON.parse(dataJson);
//    areaCont = "";
//    var checkId = "";
//    var checkName = "";
//    $.each(dataObject, function (index, item) {
//        if (item.ParentId && item.ParentId.length > 0 && item.ParentId == id) {
//            checkId = item.id;
//            checkName = item.Value;
//            var right = "";
//            if (parseInt(item.ChildrenCount) > 0) {
//                right = "<label>></label>";
//            }
//            areaCont += '<li style="width:380px;" id="' + item.Id + 'li" onClick="selectM(\'' + item.Id + '\',\'' + item.Level + '\',\'' + pName + arrow + item.Value + '\');"><a href="javascript:void(0)">' + item.Value + right + '</a></li>';
//        }
//    });
//    if (areaCont.length > 0) {
//        var newSort = '<ul style="width:400px;" id="sort' + nextCla + '" data-cla="' + nextCla + '">' + areaCont + '</ul>';
//    }
//    $("#urDiv").append(newSort);
//    $("#sort" + cla + " li").removeClass("active");
//    $("#" + id + "li").addClass("active");
//    expressP = pName;
//    $("#selectedSort").html(expressP);
//    $("#selectTypeId").val(id);
//    $("#selectedSort").attr("data-typekey", id);
//    //$("#submitBtn").removeClass("disabled");
//    if (areaCont.length === 0) {
//        $("#submitBtn").removeClass("disabled");
//    }
//    else {
//        $("#submitBtn").addClass("disabled");
//    }
//}
/**
 * 以上代码，点击产品分类，如果点击的产品分类没有子分类，则下一步按钮可用，否则不可用 disabled
 */


var expressP, expressC, expressD, expressArea, areaCont;
var arrow = " <font>&gt;</font> ";
var dataJson = '';
function intProvince(jsonData) {
    dataJson = jsonData;
    if (dataJson.length > 0) {
        var dataObject = JSON.parse(dataJson);
        areaCont = "";
        $.each(dataObject, function (index, item) {
            if (!item.ParentId || item.ParentId.length <= 0) {
                var right = "";
                if (parseInt(item.ChildrenCount) > 0) {
                    right = "<label>></label>";
                }
                areaCont += '<li style="width:380px" id="' + item.Id + 'li" onClick="selectM(\'' + item.Id + '\',\'' + 1 + '\',\'' + item.Value + '\');"><a href="javascript:void(0)" class="you" >' + item.Value + right + '</a></li>';
            }
        });
        $("#sort1").html(areaCont);
    }
}

function selectM(id, cla, pName) {
    var nextCla = parseInt(cla) + 1;
    $("#urDiv ul").each(function () {
        if (parseInt($(this).attr("data-cla")) >= nextCla) {
            $(this).remove();
        }
    });
    var dataObject = JSON.parse(dataJson);
    areaCont = "";
    var checkId = "";
    var checkName = "";
    $.each(dataObject, function (index, item) {
        if (item.ParentId && item.ParentId.length > 0 && item.ParentId == id) {
            checkId = item.id;
            checkName = item.Value;
            var right = "";
            if (parseInt(item.ChildrenCount) > 0) {
                right = "<label>></label>";
            }
            areaCont += '<li style="width:380px;" id="' + item.Id + 'li" onClick="selectM(\'' + item.Id + '\',\'' + item.Level + '\',\'' + pName + arrow + item.Value + '\');"><a href="javascript:void(0)">' + item.Value + right + '</a></li>';
        }
    });
    if (areaCont.length > 0) {
        var newSort = '<ul style="width:400px;" id="sort' + nextCla + '" data-cla="' + nextCla + '">' + areaCont + '</ul>';
    }
    $("#urDiv").append(newSort);
    $("#sort" + cla + " li").removeClass("active");
    $("#" + id + "li").addClass("active");
    expressP = pName;
    $("#selectedSort").html(expressP);
    $("#selectTypeId").val(id);
    $("#selectedSort").attr("data-typekey", id);
    if (parseInt(cla) === 2) {
        $("#submitBtn").removeClass("disabled");
    }
    else {
        $("#submitBtn").addClass("disabled");
    }
}
