using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace HeyVoteWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/css").Include(
            //    "~/css/vendor/all.css",
            //    "~/css/app/app.css",
            //    "~/css/vendor/ng-img-crop.css"
            //).ForceOrdered());

            bundles.Add(new ScriptBundle("~/assets/js").Include(
                    "~/Scripts/jquery-2.1.1.js",
                   "~/Scripts/HeyVoteSpa/angular.min.js",
                   "~/Scripts/HeyVoteSpa/angular-route.js",
                   "~/Scripts/HeyVoteSpa/app.js",
                   "~/Scripts/HeyVoteSpa/HomeController.js",
                   "~/scripts/vendor/angular-file-upload.min.js",
                   "~/js/vendor/ng-img-crop.js"
                    /***************************************************************************/
                    /*************************** Add extra Js *********************************/
                    /***************************************************************************/
                    ).ForceOrdered());

        }
    }

    internal class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
    internal static class BundleExtensions
    {
        public static Bundle ForceOrdered(this Bundle sb)
        {
            sb.Orderer = new AsIsBundleOrderer();
            return sb;
        }
    }
}
