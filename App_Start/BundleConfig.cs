using System.Web;
using System.Web.Optimization;

namespace HRMSWithTheme
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new Bundle("~/Content/pluginsCSS").Include(
                      "~/Content/assets/vendors/mdi/css/materialdesignicons.min.css",
                      "~/Content/assets/vendors/css/vendor.bundle.base.css"));

            bundles.Add(new Bundle("~/Content/pluginCSSForThisPage").Include(
                      "~/Content/assets/vendors/flag-icon-css/css/flag-icon.min.css",
                      "~/Content/assets/vendors/jvectormap/jquery-jvectormap.css"));

            bundles.Add(new Bundle("~/Content/layoutStyles").Include(
                      "~/Content/assets/css/demo/style.css"));

            bundles.Add(new Bundle("~/Content/datatableCSS").Include(
                "~/Content/DataTables/datatables.min.css"));

            bundles.Add(new Bundle("~/bundles/pluginJs").Include(
                        "~/Content/assets/vendors/js/vendor.bundle.base.js"));

            bundles.Add(new Bundle("~/bundles/pluginJsForThisPage").Include(
                       "~/Content/assets/vendors/chartjs/Chart.min.js",
                       "~/Content/assets/vendors/jvectormap/jquery-jvectormap.min.js",
                       "~/Content/vendors/jvectormap/jquery-jvectormap-world-mill-en.js"));

            bundles.Add(new Bundle("~/bundles/injectJs").Include(
                       "~/Content/assets/js/material.js",
                       "~/Content/assets/js/misc.js"));

            bundles.Add(new Bundle("~/bundles/customJs").Include(
                        "~/Content/assets/js/dashboard.js"));

            bundles.Add(new Bundle("~/bundles/preloader").Include(
                        "~/Content/assets/js/preloader.js"));

            

            bundles.Add(new Bundle("~/bundles/datatableJS").Include(
                "~/Content/DataTables/datatables.min.js"));

        }
    }
}
