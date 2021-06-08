using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Models.ContentFilter
{
    public class ContentFilterViewModel
    {
        public string IndexCatalogue { get; set; }
        public string Language { get; set; }
        public string ParamPrefix { get; set; }
        public string ListPageUrl { get; set; }
        public List<FlatTaxon> Tags { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapper div of the widget.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }
    }
}