using SitefinityWebApp.Mvc.Models.ContentFilter;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(
        Title = "Content Filter", 
        Name = "ContentFilter_MVC",
        SectionName = ToolboxesConfig.ContentToolboxSectionName,
        CssClass = ContentFilterController.WidgetIconCssClass)]
    public class ContentFilterController : Controller, ICustomWidgetVisualizationExtended
    {
        #region ICustomWidgetVisualizationExtended

        /// <inheritdoc />
        [Browsable(false)]
        public string EmptyLinkText
        {
            get
            {
                return "Set where to search.";
            }
        }

        /// <summary>
        /// Gets a value indicating whether widget is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if widget has no image selected; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return Model.IsEmpty();
            }
        }

        /// <summary>
        /// Gets the widget CSS class.
        /// </summary>
        /// <value>
        /// The widget CSS class.
        /// </value>
        [Browsable(false)]
        public string WidgetCssClass
        {
            get
            {
                return ContentFilterController.WidgetIconCssClass;
            }
        }

        /// <summary>
        /// Gets the is design mode.
        /// </summary>
        /// <value>The is design mode.</value>
        protected virtual bool IsDesignMode
        {
            get
            {
                return SystemManager.IsDesignMode;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the template that widget will be displayed.
        /// </summary>
        /// <value></value>
        public string TemplateName
        {
            get
            {
                return templateName;
            }

            set
            {
                templateName = value;
            }
        }

        /// <summary>
        /// Gets the Content Filter widget model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IContentFilterModel Model
        {
            get
            {
                if (this.model == null)
                    this.model = ControllerModelFactory.GetModel<IContentFilterModel>(this.GetType());

                return this.model;
            }
        }


        #endregion

        // GET: ContentFilter
        public ActionResult Index()
        {
            if (IsEmpty)
            {
                return new EmptyResult();
            }

            var viewModel = Model.GetViewModel();

            return View(TemplateName, viewModel);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        #region Private fields and constants

        internal const string WidgetIconCssClass = "sfSearchBoxIcn sfMvcIcn";
        private IContentFilterModel model;
        private string templateName = "ContentFilter";
        #endregion      
    }
}