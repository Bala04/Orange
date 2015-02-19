using System.Web;
using System.Web.Optimization;

namespace maQx
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/linq.js",
                        "~/Scripts/linq.jquery.js",
                        "~/Scripts/jquery.dataTables.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/angular.js",
                    "~/Scripts/ui-bootstrap-0.12.0.min.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/angular-animate.js",
                    "~/Scripts/angular-datatables.js",
                    "~/Scripts/TableTools.js",
                    "~/Scripts/ZeroClipboard.js",
                    "~/Scripts/dataTables.tableTools.js",
                    "~/Scripts/loading-bar.js",
                    "~/Scripts/app.js",
                    "~/Scripts/section/app-controller.js",
                    "~/Scripts/section/page-controller.js",
                    "~/Scripts/section/user-controller.js",
                    "~/Scripts/section/menu-controller.js",
                    "~/Scripts/section/frame-controller.js",
                    "~/Scripts/section/shared-controller.js",
                    "~/Scripts/components/services/app-service.js",
                    "~/Scripts/components/services/user-service.js",
                    "~/Scripts/components/services/menu-service.js",
                    "~/Scripts/components/directives/app-directive.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular-table-controller").Include("~/Scripts/section/table-controller.js"));
            bundles.Add(new ScriptBundle("~/bundles/section-invite-controller").Include("~/Scripts/section/invite-controller.js"));
            bundles.Add(new ScriptBundle("~/bundles/section-shared-controller").Include("~/Scripts/section/shared-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/department-controller").Include(
                "~/Scripts/components/services/department-service.js",
                "~/Scripts/section/department-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/department-menu-controller").Include(
                "~/Scripts/section/department-mapping-controller.js",
                "~/Scripts/section/department-menu-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/department-user-controller").Include(
                "~/Scripts/section/department-mapping-controller.js",
                "~/Scripts/section/department-user-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/access-level-controller").Include(
             "~/Scripts/section/access-level-controller.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.min.css",
                "~/Content/layout.css", "~/Content/loading-bar.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/jquery.dataTables.min.css",
                "~/Content/datatables.bootstrap.min.css",
                "~/Content/dataTables.tableTools.css"));

            // BundleTable.EnableOptimizations = true;
        }
    }
}
