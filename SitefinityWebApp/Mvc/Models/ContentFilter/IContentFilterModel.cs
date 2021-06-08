using System;

namespace SitefinityWebApp.Mvc.Models.ContentFilter
{
    /// <summary>
    /// An interface that provides all common properties for ContentFilter model.
    /// </summary>
    public interface IContentFilterModel
    {
        /// <summary>
        /// Gets or sets the name of the index catalogue which will be used for searching.
        /// </summary>
        string IndexCatalogue { get; set; }

        /// <summary>
        /// Gets or sets the current UI language.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the Id of the page where the content list will be displayed. This content list will be filtered.
        /// </summary>
        Guid ListPageId { get; set; }

        /// <summary>
        /// The prefix for the params to ensure they don't clash with anything else.
        /// </summary>
        string ParamPrefix { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapper div of the Search widget (if such is presented).
        /// </summary>
        string CssClass { get; set; }

        /// <summary>
        /// Checks if the model is empty.
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <returns></returns>
        ContentFilterViewModel GetViewModel();
    }
}