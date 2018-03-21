using Sitecore.Data.Items;

namespace Sitecore.Foundation.Assets.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Sitecore.Foundation.Assets.Models;
    using Sitecore.Foundation.Assets.Repositories;
    using Sitecore.Foundation.SitecoreExtensions.Extensions;

    /// <summary>
    ///     A service which helps add the required JavaScript at the end of a page, and CSS at the top of a page.
    ///     In component based architecture it ensures references and inline scripts are only added once.
    /// </summary>
    public class RenderAssetsService
    {
        private static RenderAssetsService _current;
        public static RenderAssetsService Current => _current ?? (_current = new RenderAssetsService());

        public HtmlString RenderScript(ScriptLocation location)
        {
            var assets = AssetRepository.Current.Items.Where(asset => (asset.Type == AssetType.JavaScript || asset.Type == AssetType.Raw) && asset.Location == location && this.IsForContextSite(asset));

            var sb = new StringBuilder();
            foreach (var item in assets)
            {
                if (item.Type == AssetType.Raw)
                {
                    sb.Append(item.Content).AppendLine();
                }
                else
                {
                    switch (item.ContentType)
                    {
                        case AssetContentType.File:
                            sb.AppendFormat("<script src=\"{0}\"></script>", item.Content).AppendLine();
                            break;
                        case AssetContentType.Inline:
                            if (item.Type == AssetType.Raw)
                            {
                                sb.AppendLine(HttpUtility.HtmlDecode(item.Content));
                            }
                            else
                            {
                                sb.AppendLine("<script>\njQuery(document).ready(function() {");
                                sb.AppendLine(item.Content);
                                sb.AppendLine("});\n</script>");
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            return new HtmlString(sb.ToString());
        }

        public HtmlString RenderStyles(bool async)
        {
            var sb = new StringBuilder();
            string asyncVal = async.ToString().ToLower();
            foreach (var item in AssetRepository.Current.Items.Where(asset => asset.Type == AssetType.Css && this.IsForContextSite(asset)))
            {
                switch (item.ContentType)
                {
                    case AssetContentType.File:
                        sb.AppendFormat($"<link href=\"{0}\" rel=\"stylesheet\" async=\"{asyncVal}\" />", item.Content).AppendLine();
                        break;
                    case AssetContentType.Inline:
                        sb.AppendLine("<style type=\"text/css\">");
                        sb.AppendLine(item.Content);
                        sb.AppendLine("</style>");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new HtmlString(sb.ToString());
        }

        private bool IsForContextSite(Asset asset)
        {
            if (asset.Site == null)
            {
                return true;
            }

            foreach (var part in asset.Site.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var siteWildcard = part.Trim().ToLowerInvariant();
                if (siteWildcard == "*" || Context.Site.Name.Equals(siteWildcard, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasInlineStyles(HttpRequestBase request)
        {
            return AssetRepository.Current.Items.Any(asset => asset.Type == AssetType.Css && asset.ContentType == AssetContentType.Inline && this.IsForContextSite(asset));
        }

        public HtmlString RenderScriptAsyncBootup(ScriptLocation location, string jqueryLocation)
        {
            var sb = new StringBuilder();
            var assets = AssetRepository.Current.Items.Where(
                asset => (asset.Type == AssetType.JavaScript || asset.Type == AssetType.Raw)
                         && asset.Location == location && this.IsForContextSite(asset));

            sb.Append("<script data-cfasync=\"false\">\n var scriptsToLoad = [");
            var assetsArray = assets as Asset[] ?? assets.ToArray();
            for (int i = 0; i < assetsArray.Count(); i++)
            {
                var sciptUri = assetsArray[i].Content;
                if (!string.IsNullOrWhiteSpace(sciptUri))
                {
                    sb.Append($"'{sciptUri.Replace("\r", string.Empty)}'");
                    if (assetsArray.Any() && (i + 1) != assetsArray.Count())
                    {
                        sb.Append(",");
                    }
                }
            }
            sb.Append("]; \n</script>");
            sb.AppendLine($"<script data-cfasync=\"false\" src=\"{jqueryLocation}\" async defer></script> ");
            return new HtmlString(sb.ToString());
        }

        public static string GetPageKey(Item item)
        {
            string key = string.Empty;
            if (item.Url() == "/" || item.Name == "Home")
                key = "homepage";
            else
            {
                key = item.Url().Replace("/", "_");
            }
            return key;
        }

        public static string GetPageKey(HttpRequestBase request)
        {
            string key = request.Url.PathAndQuery;
            if (key == "/")
                key = "homepage";
            else
            {
                key = request.Url.PathAndQuery.Replace("/", "_");
            }
            return key;
        }
    }
}