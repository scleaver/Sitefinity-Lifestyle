using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;

namespace SitefinityWebApp.Mvc.Models.ContentFilter
{
    public class ContentFilterModel : IContentFilterModel
    {
        #region Properties

        /// <inheritdoc />
        public string IndexCatalogue { get; set; }

        /// <inheritdoc />
        public string Language { get; set; }

        /// <inheritdoc />
        public Guid ListPageId { get; set; }

        /// <inheritdoc />
        public string SiteRootName { get; set; }

        /// <inheritdoc />
        public string ParamPrefix { get; set; } = "cf";

        /// <inheritdoc />
        public string CssClass { get; set; }

        #endregion

        #region Private methods
        private SiteMapProvider GetSiteMapProvider()
        {
            SiteMapProvider provider;
            try
            {
                if (string.IsNullOrEmpty(this.SiteRootName))
                    provider = SiteMapBase.GetSiteMapProvider(SiteMapBase.DefaultSiteMapProviderName);
                else
                    provider = SiteMapBase.GetSiteMapProvider(this.SiteRootName);

                return provider;
            }
            catch (Exception)
            {
                provider = null;
            }
            return provider;
        }

        private string GetListPageUrl()
        {
            if (this.ListPageId == Guid.Empty)
            {
                return null;
            }

            var listPageUrl = string.Empty;
            var provider = this.GetSiteMapProvider();

            if (ListPageId != Guid.Empty && provider != null)
            {
                // Using the SiteMapProvider as it is more performant than the PageManager

                    var sitemapBase = provider as SiteMapBase;
                    var node = sitemapBase == null ? provider.FindSiteMapNodeFromKey(ListPageId.ToString()) : sitemapBase.FindSiteMapNodeFromKey(ListPageId.ToString(), false);

                    if (node != null)
                    {
                        listPageUrl = node.Url;
                    }
                
            }
            else
            {
                var node = SiteMapBase.GetActualCurrentNode();
                if (node != null)
                    listPageUrl = node.Url;
            }

            if(!listPageUrl.IsNullOrEmpty())
            {
                return UrlPath.ResolveUrl(listPageUrl, Config.Get<SystemConfig>().SiteUrlSettings.GenerateAbsoluteUrls);
            }

            return null;
        }

        public bool IsEmpty()
        {
            return IndexCatalogue.IsNullOrEmpty();
        }

        public ContentFilterViewModel GetViewModel()
        {
            var viewModel = new ContentFilterViewModel()
            {
                IndexCatalogue = IndexCatalogue,
                ParamPrefix = ParamPrefix,
                Language = Language,
                ListPageUrl = GetListPageUrl(),
                Tags = GetTags(),
                CssClass = CssClass
            };


            return viewModel;
        }

        private List<FlatTaxon> GetTags()
        {
            var manager = TaxonomyManager.GetManager();
            var tagsId = manager.GetTaxonomies<FlatTaxonomy>().Where(t => t.Name.ToLower() == "tags").FirstOrDefault();
            var taxons = manager.GetTaxa<FlatTaxon>().Where(t => t.TaxonomyId == tagsId.Id);

            // Would recommend remove the tags that don't have any content assigned see: https://github.com/Sitefinity/feather-widgets/blob/master/Telerik.Sitefinity.Frontend.Taxonomies/Mvc/Models/TaxonomyModel.cs
            return taxons.ToList();
        }

        #endregion

    }
}