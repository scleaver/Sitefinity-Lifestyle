using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Custom.Utilities
{
    internal class ContentFilterHelper
    {
        /// <summary>
        /// Creates a FilterExpression and Sort Expression based on query string parameters
        /// </summary>
        /// <param name="queryStrings">A collection of query string parameters</param>
        /// <param name="paramPrefix">The parameter prefix to use. This ensures params with the same name aren't mistakenly used.</param>
        /// <param name="sortExpr">The current sort expression. This will be replaced if there is a query string with a valid value otherwise the sort expression will remain the same as intially set.</param>
        /// <param name="filterExpression">The current filter expression to extend upon.</param>
        internal static void CreateContentFilter(NameValueCollection queryStrings, string paramPrefix, ref string sortExpr, ref string filterExpression)
        {
            if (paramPrefix.IsNullOrWhitespace())
            {
                paramPrefix = "cf.";
            }
            if (!paramPrefix.EndsWith("."))
            {
                paramPrefix += ".";
            }

            var filterParams = queryStrings.Cast<string>().Where(key => key.StartsWith(paramPrefix)).Select(key => new KeyValuePair<string, string>(key.Replace(paramPrefix, ""), HttpUtility.UrlDecode(queryStrings[key]))).ToList();
            foreach (var item in filterParams)
            {
                switch (item.Key)
                {
                    case "sortby":
                        // Get sort by from query string.String should be in the format[fieldname]-[asc | desc], asc is assumed by default,
                        // and can include items multiple separated by a comma if required eg. title-asc,publication-date-desc
                        sortExpr = item.Value.IsNullOrWhitespace() ? sortExpr : ContentHelper.CreateSortExpressionFromString(item.Value);
                        break;
                    case "squery":
                        // Get search terms and index name from query string. Uses the Sitefinity search to find matching content items
                        // and adds the ids of the content items to the filter expresison. Must include search terms and search index
                        if (filterParams.Any(l => l.Key == "sindex"))
                        {
                            filterExpression = ContentHelper.CreateFilterExpressionFromSearchKeywords(item.Value, filterParams.Where(l => l.Key == "sindex").First().Value, filterExpression);
                        }
                        break;
                    case "categories":
                        // Gets the categories url names from the query string. The categories should be in urlname format and 
                        // should be seaprated by a comma eg. categories=cats,dogs,guinea-pigs.
                        filterExpression = ContentHelper.CreateCategoriesFilterExpressionFromString(item.Value, filterExpression);
                        break;
                    case "tags":
                        // Gets the tags url names from the query string. The categories should be in urlname format and 
                        // should be seaprated by a comma eg. tags=cats,dogs,guinea-pigs.
                        filterExpression = ContentHelper.CreateTagsFilterExpressionFromString(item.Value, filterExpression);
                        break;
                    case "range":
                        // Gets the date range from the query string.
                        filterExpression = ContentHelper.CreateDateRangeFilterExpressionFromString(item.Value, filterExpression);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}