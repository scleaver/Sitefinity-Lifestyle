using SitefinityWebApp.Custom.Utilities;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Frontend.News.Mvc.Models;

namespace SitefinityWebApp.Custom.Models
{
    public class CustomNewsModel : NewsModel
    {
        protected override IQueryable<TItem> SetExpression<TItem>(IQueryable<TItem> query, string filterExpression, string sortExpr, int? itemsToSkip, int? itemsToTake, ref int? totalCount)
        {
            var queryStrings = HttpContext.Current.Request.QueryString;
            var prefix = "cf.";

            ContentFilterHelper.CreateContentFilter(queryStrings, prefix, ref sortExpr, ref filterExpression);

            return base.SetExpression(query, filterExpression, sortExpr, itemsToSkip, itemsToTake, ref totalCount);
        }
    }
}