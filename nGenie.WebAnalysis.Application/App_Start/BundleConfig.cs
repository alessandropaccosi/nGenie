using System.Web;
using System.Web.Optimization;

namespace nGenie.WebAnalysis.Application
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Javascript
            
            #region jQuery
            bundles.Add(new ScriptBundle("~/jquery-js").Include("~/Content/assets/plugins/jquery/js/jquery.min.js"));
            bundles.Add(new ScriptBundle("~/jquery-ui-js").Include("~/Content/assets/plugins/jquery-ui/js/jquery-ui.min.js"));
            bundles.Add(new ScriptBundle("~/jquery-slimscroll-js").Include("~/Content/assets/plugins/jquery-slimscroll/js/jquery.slimscroll.js"));
            bundles.Add(new ScriptBundle("~/mCustomScrollbar-js").Include("~/Content/assets/js/jquery.mCustomScrollbar.concat.min.js"));
            bundles.Add(new ScriptBundle("~/jquery-base64-js").Include("~/Content/assets/plugins/jquery-base64/jquery.base64.min.js"));

            //Nuovo
            bundles.Add(new ScriptBundle("~/jquery-dataTables-js").Include("~/Content/assets/plugins/datatables.net/js/jquery.dataTables.min.js"));
            bundles.Add(new ScriptBundle("~/datatables-buttons-js").Include("~/Content/assets/plugins/datatables.net-buttons/js/dataTables.buttons.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            #endregion

            #region Bootstrap
            bundles.Add(new ScriptBundle("~/bootstrap-js").Include("~/Content/assets/plugins/bootstrap/js/bootstrap.min.js"));

            //Nuovo
            bundles.Add(new ScriptBundle("~/dataTables-bootstrap4-js").Include("~/Content/assets/plugins/datatables.net-bs4/js/dataTables.bootstrap4.min.js"));
            #endregion

            #region Plugins
            //bundles.Add(new ScriptBundle("~/popper-map").Include("~/Content/assets/plugins/popper/js/popper.min.js.map"));
            bundles.Add(new ScriptBundle("~/popper-js").Include("~/Content/assets/plugins/popper/js/popper.min.js"));

            bundles.Add(new ScriptBundle("~/modernizr-js").Include("~/Content/assets/plugins/modernizr/js/modernizr.js", "~/Content/assets/plugins/modernizr/js/css-scrollbars.js"));

            bundles.Add(new ScriptBundle("~/toastr-js").Include("~/Content/assets/plugins/toastr/js/toastr.js"));

            bundles.Add(new ScriptBundle("~/jszip-js").Include("~/Content/assets/plugins/jszip/jszip.min.js"));
            bundles.Add(new ScriptBundle("~/xml2json-js").Include("~/Content/assets/plugins/xml2json/jquery.xml2json.js"));

            bundles.Add(new ScriptBundle("~/i18next-js").Include("~/Content/assets/plugins/i18next/js/i18next.min.js"));
            bundles.Add(new ScriptBundle("~/i18nextXHRBackend-js").Include("~/Content/assets/plugins/i18next-xhr-backend/js/i18nextXHRBackend.min.js"));
            bundles.Add(new ScriptBundle("~/i18nextBrowserLanguageDetector-js").Include("~/Content/assets/plugins/i18next-browser-languagedetector/js/i18nextBrowserLanguageDetector.min.js"));
            bundles.Add(new ScriptBundle("~/jquery-i18next-js").Include("~/Content/assets/plugins/jquery-i18next/js/jquery-i18next.min.js"));

            bundles.Add(new ScriptBundle("~/modal-js").Include("~/Content/assets/js/modal.js"));
            bundles.Add(new ScriptBundle("~/classie-js").Include("~/Content/assets/js/classie.js"));
            bundles.Add(new ScriptBundle("~/modalEffects-js").Include("~/Content/assets/js/modalEffects.js"));

            bundles.Add(new ScriptBundle("~/bootbox-js").Include("~/Content/assets/plugins/bootbox/bootbox.min.js"));
            bundles.Add(new ScriptBundle("~/switchery-js").Include("~/Content/assets/plugins/switchery/js/switchery.min.js"));

            bundles.Add(new ScriptBundle("~/select2-js").Include("~/Content/assets/plugins/select2/js/select2.full.min.js"));
            bundles.Add(new ScriptBundle("~/bootstrap-multiselect-js").Include("~/Content/assets/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js"));
            bundles.Add(new ScriptBundle("~/multiselect-js").Include("~/Content/assets/plugins/multiselect/js/jquery.multi-select.js"));
            bundles.Add(new ScriptBundle("~/select2-custom-js").Include("~/Content/assets/pages/advance-elements/select2-custom.js"));
            bundles.Add(new ScriptBundle("~/jquery-quicksearch-js").Include("~/Content/assets/js/jquery/jquery.quicksearch.js"));
            
            //Nuovo
            bundles.Add(new ScriptBundle("~/buttons-html5-js").Include("~/Content/assets/plugins/datatables.net-buttons/js/buttons.html5.min.js"));
            bundles.Add(new ScriptBundle("~/responsive-bootstrap4-js").Include("~/Content/assets/plugins/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js"));
            bundles.Add(new ScriptBundle("~/dataTables-responsive-js").Include("~/Content/assets/plugins/datatables.net-responsive/js/dataTables.responsive.min.js"));
            #endregion

            #region Pages
            bundles.Add(new ScriptBundle("~/waves-js").Include("~/Content/assets/pages/waves/js/waves.min.js"));

            #endregion

            #region Analysis
            //bundles.Add(new ScriptBundle("~/kendo-all-js").Include("~/Content/assets/plugins/kui/kendo.all.js"));
            bundles.Add(new ScriptBundle("~/kendo-all-js").Include("~/Content/assets/plugins/kui/kendo.all.min.js"));

            bundles.Add(new ScriptBundle("~/kendo-culture-js").Include("~/Content/assets/plugins/kui/cultures/kendo.culture.it-IT.min.js"));
            bundles.Add(new ScriptBundle("~/kendo-messages-js").Include("~/Content/assets/plugins/kui/messages/kendo.messages.it-IT.min.js"));
            bundles.Add(new ScriptBundle("~/kendo-console-js").Include("~/Content/assets/kui-shared/js/console.js"));

            bundles.Add(new ScriptBundle("~/ui-pivot-control-js").Include("~/Content/assets/js/ui-pivot-control.js"));
            bundles.Add(new ScriptBundle("~/ui-admin-js").Include("~/Content/assets/js/ui-admin.js"));
            bundles.Add(new ScriptBundle("~/ui-common-js").Include("~/Content/assets/js/ui-common.js"));

            bundles.Add(new ScriptBundle("~/analysis-toolbar-js").Include("~/Content/assets/plugins/analysis-toolbar/analysis-toolbar.js"));


            #endregion

            bundles.Add(new ScriptBundle("~/pcoded-js").Include("~/Content/assets/js/pcoded.min.js"));
            bundles.Add(new ScriptBundle("~/vertical-js").Include("~/Content/assets/js/vertical/vertical-layout.min.js"));
            bundles.Add(new ScriptBundle("~/jsonQ-js").Include("~/Content/assets/js/jsonQ.js"));
            bundles.Add(new ScriptBundle("~/dService-js").Include("~/Content/assets/js/dService.js"));
            bundles.Add(new ScriptBundle("~/xdr-js").Include("~/Content/assets/js/xdr.js"));

            bundles.Add(new ScriptBundle("~/common-pages-js").Include("~/Content/assets/js/common-pages.js"));
            bundles.Add(new ScriptBundle("~/script-js").Include("~/Content/assets/js/script.js"));

            //Aggiunte per cercare di risolvere un problema di visualizzazione su tabella zero configuration amministrazione
            bundles.Add(new ScriptBundle("~/vfs-fonts-js").Include("~/Content/assets/pages/data-table/extensions/buttons/js/vfs_fonts.js"));
            bundles.Add(new ScriptBundle("~/data-table-custom-js").Include("~/Content/assets/pages/data-table/js/data-table-custom.js"));
            bundles.Add(new ScriptBundle("~/pcoded-min-js").Include("~/Content/assets/js/pcoded.min.js"));

            //Nuovo, script per utilizzare la libreria signalR
            bundles.Add(new ScriptBundle("~/jquery-signalR-js").Include("~/Scripts/jquery.signalR-2.4.1.min.js"));
            #endregion

            #region Stylesheet

            #region jQuery
            bundles.Add(new StyleBundle("~/mCustomScrollbar-css").Include("~/Content/assets/css/jquery.mCustomScrollbar.css"));

            #endregion

            #region Bootstrap
            bundles.Add(new StyleBundle("~/bootstrap-css").Include("~/Content/assets/plugins/bootstrap/css/bootstrap.min.css"));
            #endregion

            #region Plugins
            bundles.Add(new StyleBundle("~/toastr-css").Include("~/Content/assets/plugins/toastr/css/toastr.css"));
            bundles.Add(new StyleBundle("~/select2-css").Include("~/Content/assets/plugins/select2/css/select2.min.css"));
            bundles.Add(new StyleBundle("~/bootstrap-multiselect-css").Include("~/Content/assets/plugins/bootstrap-multiselect/css/bootstrap-multiselect.css"));
            bundles.Add(new StyleBundle("~/multiselect-css").Include("~/Content/assets/plugins/multiselect/css/multi-select.css"));

            //Nuovo
            bundles.Add(new StyleBundle("~/buttons-dataTables-css").Include(
                "~/Content/assets/pages/data-table/css/buttons.dataTables.min.css",
                "~/Content/assets/pages/data-table/extensions/buttons/css/buttons.dataTables.min.css"
            ));
            bundles.Add(new StyleBundle("~/responsive-bootstrap4-css").Include("~/Content/assets/plugins/datatables.net-responsive-bs4/css/responsive.bootstrap4.min.css"));
            bundles.Add(new StyleBundle("~/dataTables-bootstrap4-css").Include("~/Content/assets/plugins/datatables.net-bs4/css/dataTables.bootstrap4.min.css"));
            #endregion

            #region Pages
            bundles.Add(new StyleBundle("~/waves-css").Include("~/Content/assets/pages/waves/css/waves.min.css"));

            #endregion

            #region Icons
            bundles.Add(new StyleBundle("~/themify-css").Include("~/Content/assets/icon/themify-icons/themify-icons.css"));
            bundles.Add(new StyleBundle("~/font-awesome-css").Include("~/Content/assets/icon/font-awesome/css/font-awesome.min.css"));
            //bundles.Add(new StyleBundle("~/themify-css").Include("~/Content/assets/icon/themify-icons/themify-icons.css"));

            #endregion

            #region Analysis

            bundles.Add(new StyleBundle("~/kendo-examples-css").Include("~/Content/assets/kui-shared/styles/examples-offline.css"));

            bundles.Add(new StyleBundle("~/kendo-common-css").Include("~/Content/assets/css/kui/kendo.common.min.css"));

           

            bundles.Add(new StyleBundle("~/kendo-rtl-css").Include("~/Content/assets/css/kui/kendo.rtl.min.css"));
            bundles.Add(new StyleBundle("~/kendo-color-css").Include("~/Content/assets/css/kui/kendo.blueopal.min.css"));

            bundles.Add(new StyleBundle("~/analysis-toolbar-css").Include("~/Content/assets/plugins/analysis-toolbar/analysis-toolbar.css"));

            #endregion

            bundles.Add(new StyleBundle("~/style-css").Include("~/Content/assets/css/style.css"));

            #endregion

        }
    }
}
