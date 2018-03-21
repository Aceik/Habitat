namespace Sitecore.Foundation.SitecoreExtensions.Extensions
{
    using System;
    using System.Web;
    using Sitecore.Mvc.Helpers;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Resources.Media;
    using Sitecore.Xml;

    public static class RenderingExtensions
    {
        public static int GetIntegerParameter(this Rendering rendering, string parameterName, int defaultValue = 0)
        {
            if (rendering == null)
            {
                throw new ArgumentNullException(nameof(rendering));
            }

            var parameter = rendering.Parameters[parameterName];
            if (string.IsNullOrEmpty(parameter))
            {
                return defaultValue;
            }

            int returnValue;
            return !int.TryParse(parameter, out returnValue) ? defaultValue : returnValue;
        }

        public static bool GetUseStaticPlaceholderNames([NotNull] this Rendering rendering)
        {
            return MainUtil.GetBool(rendering.Parameters[Constants.DynamicPlaceholdersLayoutParameters.UseStaticPlaceholderNames], false);
        }

        public static HtmlString CachedRendering(this SitecoreHelper sitecoreHelper, string pathOrId, RenderingCachingSettings renderingCachingSettings)
        {
            return sitecoreHelper.Rendering(pathOrId, renderingCachingSettings);
        }
    }


    public class RenderingCachingSettings
    {
        public bool? Cacheable { get; set; }

        public TimeSpan? Cache_Timeout { get; set; }

        public bool? Cache_VaryByData { get; set; }

        public bool? Cache_VaryByDevice { get; set; }

        public bool? Cache_VaryByLogin { get; set; }

        public bool? Cache_VaryByParameters { get; set; }

        public bool? Cache_VaryByQueryString { get; set; }

        public bool? Cache_VaryByUser { get; set; }

        public string CacheKey { get; set; }
    }
}