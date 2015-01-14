angular.module('tabularApp').controller('TableController', ["$scope", "DTOptionsBuilder", "DTColumnBuilder", "$resource", "$q", 'DataService', '$rootScope', function (h, j, k, l, m, n, $rootScope) {
    var o = {
        _l: [],
        _k: function (a, b) {
            a[0] = b.IsViewable;
            a[1] = b.IsEditable;
            a[2] = b.IsDeleteable
        },
        _a: function (b, c, d, f) {
            return o._l[f] ? '<a data-toggle="tooltip" data-placement="top" title="' + b + '" href="' + $("#baseNode").val() + '/' + b + '/' + c + '" class="btn btn-sm tp-' + String(b).toLowerCase() + '"><span class="fa fa-' + d + '"></span></a>' : ''
        },
        _u: {
            _t: null,
            _d: null
        },
        _r: function () {
            return m(function (b, c) {
                if (o._u._d == null || (Date.now() - o._u._t) > 60000) {
                    o._u._d = n.query();
                    o._u._t = Date.now()
                }

                if (o._u._d.$promise) {
                    o._u._d.$promise.then(function (a) {
                        if (a.Type == "SUCCESS") {
                            o._k(o._l, a);
                            b(a.List)
                        } else if (a.Type == "ERROR") {
                            c("Authentication Failed. You are not authorized to access the requested resource")
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                        }
                    }, function (a) {
                        $rootScope.$broadcast("alert", { type: "Error", message: a });
                        console.log(a);
                        c(a)
                    });
                } else {
                    console.log(o._u._d)
                }
            })
        },
        _t: function () {
            return "<'top-toolbar'<'left-toolbar'<'toolbar'T><'toolbar copyInfo'>><'right-toolbar'<'toolbar'l><'toolbar'f>>>"
        },
        _b: function () {
            return "<'bottom-toolbar'<'left-toolbar'<'toolbar'i>><'right-toolbar'<'toolbar'p>>>"
        },
        _d: function () {
            var a = [];
            $.each(n.init(), function (i, e) {
                a.push(k.newColumn(e.a).withTitle(e.b))
            });
            a.push(k.newColumn(null).withTitle('').notSortable().renderWith(function (a, b, c, d) {
                return o._a('Details', a.Key, 'th-list', 0) + o._a('Edit', a.Key, 'edit', 1) + o._a('Delete', a.Key, 'trash-o', 2)
            }));
            return a
        }
    };
    h.$on('event:dataTableLoaded', function (a, b) {
        _app.enableTooltip()
    });
    h.$on('event:dataTableReLoaded', function (a, b) {
        _app.enableTooltip()
    });
    this.reloadData = function () {
        h.dtOptions.reloadData()
    };
    h.dtOptions = j.fromFnPromise(function () {
        return o._r()
    }).withPaginationType('full_numbers').withDisplayLength(10).withBootstrap().withBootstrapOptions({
        TableTools: {
            classes: {
                container: 'btn-group',
                buttons: {
                    normal: 'btn btn-sm btn-primary inverse-btn'
                }
            }
        }
    }).withTableTools('Content/Swf/copy_csv_xls_pdf.swf').withTableToolsButtons(['copy',
    {
        'sExtends': 'collection',
        'sButtonText': 'Save <span class="spaced caret"></span>',
        'aButtons': ['csv', 'xls', 'pdf']
    }, ]).withDOM(o._t() + "<'table-space'<t>>" + o._b()).withLanguage({
        sSearch: '<span class="input-group-addon addon-icon"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></span>',
        sSearchPlaceholder: "Search",
        sInfo: "Showing <span class='ui-bold'>_START_</span> to <span class='ui-bold'>_END_</span> of <span class='ui-bold'>_TOTAL_</span> entries",
        sInfoEmpty: 'No entries to show',
        sZeroRecords: 'No records were found that match the specified search.',
        sEmptyTable: 'No data available to show here.'
    }).withOption('fnRowCallback', function (c, d, e, f) {
        var g = $('.dataTables_filter input').val();
        $('td', c).each(function (i) {
            var b = $(c).find("td:nth-child(" + i + ")");
            b.html(b.text().replace(new RegExp(g, 'i'), function (a) {
                return "<span class='highlight'>" + _app.escapeHtml(a) + "</span>"
            }))
        });
        return c
    });
    h.dtColumns = o._d()
}]);