jconfirm.defaults = {
    title: '提示',
    type: 'red',
    content: '确认删除',
    buttons: {},
    defaultButtons: {
        ok: {
            text: '确认',
            btnClass: 'btn-danger',
            action: function () {
            }
        },
        close: {
            text: '取消',
            action: function () {
            }
        },
    },
    contentLoaded: function (data, status, xhr) {
    },
    icon: '',
    bgOpacity: null,
    theme: 'white',
    animation: 'zoom',
    closeAnimation: 'scale',
    animationSpeed: 400,
    animationBounce: 1.2,
    rtl: false,
    container: 'body',
    containerFluid: false,
    backgroundDismiss: false,
    backgroundDismissAnimation: 'shake',
    autoClose: false,
    closeIcon: null,
    closeIconClass: false,
    columnClass: 'col-md-4 col-md-offset-4 col-sm-6 col-sm-offset-3 col-xs-10 col-xs-offset-1',
    boxWidth: '50%',
    useBootstrap: true,
    bootstrapClasses: {
        container: 'container',
        containerFluid: 'container-fluid',
        row: 'row',
    },
    onContentReady: function () { },
    onOpenBefore: function () { },
    onOpen: function () { },
    onClose: function () { },
    onDestroy: function () { },
    onAction: function () { }
};