(function ($, window) {
    var app = {};
    var initializationItems = [];

    app.registerInit = function (init) {
        initializationItems.push(init);
    };
    app.init = function () {
        for (var i = 0; i < initializationItems.length; i++) {
            initializationItems[i]();
        }
    };

    app.registerInit(function () {
        $(".alert").each(function () {
            var $self = $(this);
            setInterval(function () {
                $self.remove();
            }, 1000 * 3);
        });
        
    });

    app.registerInit(function () {
        util.textOverflow();
    });

    app.registerInit(function () {
        $("#sidebar-toggle").click(function () {
            setTimeout(function () {
                $.cookie('sidebar-collapse', $("body").hasClass("sidebar-collapse"), { expires: 30,path:"/" });
            }, 100);
        });
    });
    app.init();

    console.log("app");
})(jQuery, window)