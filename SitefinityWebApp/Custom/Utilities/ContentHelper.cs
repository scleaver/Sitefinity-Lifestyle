using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Custom.Utilities
{
    /// <summary>
    /// A content helper.
    /// </summary>
    public static class ContentHelper
    {


        /// <summary>
        /// Creates or extends an exisiting filter expression for a content list for the specified categories
        /// </summary>
        /// <param name="categories">The list of category urlnames separated by a comma.</param>
        /// <param name="filterExpression">The exisiting filter expression to extend.</param>
        /// <returns>
        /// A filter expression for content items which includes tag filters https://www.progress.com/documentation/sitefinity-cms/filtering-and-sorting-the-items
        /// </returns>
        public static string CreateCategoriesFilterExpressionFromString(string categories, string filterExpression)
        {
            if (!categories.IsNullOrWhitespace())
            {
                // We are using full urls to validate the right category however we can't retrieve urls by this
                // So will use the full url later.
                var catsFullUrl = categories.ToLower().Split(',').Select(x => String.Concat("/", x.Trim())).ToList();
                var cats = catsFullUrl.Select(x => x.Split('/').Last()).ToList();
                var taxManager = TaxonomyManager.GetManager();
                var categoriesTaxonomy = taxManager.GetTaxonomy<HierarchicalTaxonomy>(TaxonomyManager.CategoriesTaxonomyId);
                var categoryTaxa = categoriesTaxonomy.Taxa.Where(t => cats.Contains(t.UrlName));

                if (categoryTaxa != null && categoryTaxa.Count() > 0)
                {
                    var resultIds = new List<string>();

                    foreach (HierarchicalTaxon id in categoryTaxa)
                    {
                        if (catsFullUrl.Contains(id.FullUrl))
                        {
                            resultIds.Add(string.Format("Category.Contains(Guid.Parse(\"{0}\"))", id.Id));
                        }
                    }
                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";

                    filterExpression += String.Format("({0})", string.Join(" OR ", resultIds));
                }
            }
            return filterExpression;
        }

        /// <summary>
        /// Creates or extends an exisiting filter expression for a content list with items published within the specified date.
        /// </summary>
        /// <param name="dateRange">The string representing the period to filter by. Can be days, weeks, months or years eg. 6-weeks</param>
        /// <param name="filterExpression">The exisiting filter expression to extend.</param>
        /// <returns>
        /// A filter expression for content items which includes a expression limiting the date range https://www.progress.com/documentation/sitefinity-cms/filtering-and-sorting-the-items
        /// </returns>
        public static string CreateDateRangeFilterExpressionFromString(string dateRange, string filterExpression)
        {
            if (!dateRange.IsNullOrWhitespace())
            {
                var rangeValues = dateRange.Split('/');
                int month = 0;
                int day = 0;

                int.TryParse(rangeValues[0], out int year);
                if (rangeValues.Length > 1)
                {
                    int.TryParse(rangeValues[1], out month);
                }
                if (rangeValues.Length > 2)
                {
                    int.TryParse(rangeValues[2], out day);
                }

                if (day != 0)
                {
                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";
                    var filterDate = new DateTime(year, month, day);
                    filterExpression += string.Format("PublicationDate = ({0})", filterDate.Date);
                }
                else if (month != 0)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";
                    filterExpression += string.Format("PublicationDate >= ({0}) AND PublicationDate <= ({1})", startDate.Date, endDate.Date);
                }
                else if (year != 0)
                {
                    var startDate = new DateTime(year, 1, 1);
                    var endDate = startDate.AddMonths(12).AddDays(-1);
                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";
                    filterExpression += string.Format("PublicationDate >= ({0}) AND PublicationDate <= ({1})", startDate.Date, endDate.Date);
                }
            }

            return filterExpression;
        }

        /// <summary>
        /// Creates or extends an exisiting filter expression for a content list for the specified tags
        /// </summary>
        /// <param name="tags">The list of tags urlnames separated by a comma.</param>
        /// <param name="filterExpression">The exisiting filter expression to extend.</param>
        /// <returns>
        /// A filter expression for content items which includes tag filters https://www.progress.com/documentation/sitefinity-cms/filtering-and-sorting-the-items
        /// </returns>
        public static string CreateTagsFilterExpressionFromString(string tags, string filterExpression)
        {
            if (!tags.IsNullOrWhitespace())
            {
                var tagsList = tags.ToLower().Split(',').Select(x => x.Trim()).ToList();
                var taxManager = TaxonomyManager.GetManager();
                var tagsTaxonomy = taxManager.GetTaxonomy<FlatTaxonomy>(TaxonomyManager.TagsTaxonomyId);
                var tagsTaxa = tagsTaxonomy.Taxa.Where(t => tagsList.Contains(t.UrlName));

                if (tagsTaxa != null && tagsTaxa.Count() > 0)
                {
                    var resultIds = new List<string>();

                    foreach (var id in tagsTaxa)
                    {
                        resultIds.Add(string.Format("Tags.Contains(Guid.Parse(\"{0}\"))", id.Id));
                    }
                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";

                    filterExpression += String.Format("({0})", string.Join(" OR ", resultIds));
                }
            }
            return filterExpression;
        }


        /// <summary>
        /// Creates a sort expression for Sitefinity content list widgets from a query string.
        /// </summary>
        /// <param name="sortBy">The string values from query string. It should be in the format title-desc or publication-date-asc </param>
        /// <returns>
        /// Sort expression for content items https://www.progress.com/documentation/sitefinity-cms/filtering-and-sorting-the-items
        /// </returns>
        public static string CreateSortExpressionFromString(string sortBy)
        {
            var sortExpr = string.Empty;

            if (!sortBy.IsNullOrWhitespace())
            {
                var sortList = new List<string>();
                foreach (var item in sortBy.ToLower().Split(','))
                {
                    if (item.EndsWith("-desc"))
                    {
                        sortList.Add(string.Concat(item.Replace("-desc", string.Empty).ToPascalCase(new char[] { '-', ' ' }), " DESC"));
                    }
                    else
                    {
                        sortList.Add(string.Concat(item.Replace("-asc", string.Empty).ToPascalCase(new char[] { '-', ' ' }), " ASC"));
                    }
                }
                sortExpr = string.Join(",", sortList);
            }

            return sortExpr;
        }

        /// <summary>
        /// Using the Sitefinity search, find content matching search query in the specified index.
        /// </summary>
        /// <param name="searchFor">The query terms to search by.</param>
        /// <param name="indexName">The search index to use.</param>
        /// <returns>
        /// A list of ids where the content matches the search requirements
        /// </returns>
        public static List<Guid> GetContentWithSearchKeywords(string searchFor, string indexName)
        {
            if (!searchFor.IsNullOrWhitespace() && !indexName.IsNullOrWhitespace())
            {
                var ids = new List<Guid>();

                var searchFields = new string[] { "Title", "Name", "Content" };
                var hitCount = -1;
                var searcher = ObjectFactory.Resolve<ISearchResultsBuilder>();
                var result = searcher.Search(searchFor, indexName, searchFields, null, 0, 0, out hitCount, true);

                if (hitCount > 0)
                {
                    return result.Select(r => new Guid((string)r.GetValue("Id"))).ToList();
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Creates or extends an exisiting filter expression with the ids of content matching a search query in the specified index.
        /// </summary>
        /// <param name="searchFor">The query terms to search by.</param>
        /// <param name="indexName">The search index to use.</param>
        /// <param name="filterExpression">The exisiting filter expression to extend.</param>
        /// <returns>
        /// A filter expression of content ids matching the search query https://www.progress.com/documentation/sitefinity-cms/filtering-and-sorting-the-items
        /// </returns>
        public static string CreateFilterExpressionFromSearchKeywords(string searchFor, string indexName, string filterExpression)
        {
            if (!searchFor.IsNullOrWhitespace() && !indexName.IsNullOrWhitespace())
            {
                var ids = GetContentWithSearchKeywords(searchFor, indexName);

                if (ids != null && ids.Count > 0)
                {
                    var resultIds = new List<string>();

                    foreach (var id in ids)
                    {
                        resultIds.Add(string.Format("Id = {0}", id));
                    }

                    filterExpression += filterExpression.IsNullOrWhitespace() ? string.Empty : " AND ";

                    filterExpression += String.Format("({0})", string.Join(" OR ", resultIds));
                }
            }

            return filterExpression;
        }
    }
}