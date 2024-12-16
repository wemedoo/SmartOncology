using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Common.Helpers
{
    public static class ScriptHelper
    {
        private static List<BundleConfig> _bundles;
        static ScriptHelper()
        {
            LoadBundleConfig();
        }

        private static void LoadBundleConfig()
        {
            var json = System.IO.File.ReadAllText($@"{DirectoryHelper.ProjectBaseDirectory}\App_Start\bundleconfig.json");
            _bundles = JsonConvert.DeserializeObject<List<BundleConfig>>(json);
        }

        public static IHtmlContent RenderBundle(string outputFileName, string integrity = "", string crossorigin = "")
        {
            var bundle = _bundles.Find(b => b.OutputFileName == outputFileName);

            if (IsCssBundle(outputFileName))
            {
                var stylesheets = bundle.InputFiles.Select(url =>
                {
                    return $"<link rel=\"stylesheet\" type=\"text/css\" href=\"/{url}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\">";
                });

                return new HtmlString(string.Join("\n", stylesheets));
            }
            else if (IsJavaScriptBundle(outputFileName))
            {
                var scripts = bundle.InputFiles.Select(url =>
                {
                    return $"<script type=\"text/javascript\" src=\"/{url}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\"></script>";
                });

                return new HtmlString(string.Join("\n", scripts));
            }
            else
            {
                return HtmlString.Empty;
            }
        }

        public static IHtmlContent RenderScript(string url, string integrity, string crossorigin)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var scriptTag = $"<script type=\"text/javascript\" src=\"{url}?v={version}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\"></script>";
            return new HtmlString(scriptTag);
        }

        public static IHtmlContent RenderStylesheet(string href, string integrity = null, string crossorigin = null)
        {
            var linkTag = new TagBuilder("link");
            linkTag.Attributes.Add("href", href);
            linkTag.Attributes.Add("rel", "stylesheet");

            if (!string.IsNullOrEmpty(integrity))
            {
                linkTag.Attributes.Add("integrity", integrity);
            }

            if (!string.IsNullOrEmpty(crossorigin))
            {
                linkTag.Attributes.Add("crossorigin", crossorigin);
            }

            return linkTag;
        }

        public static IHtmlContent RenderHeadMetadata(bool hasShrinkToFit = false)
        {
            string viewport = hasShrinkToFit ? "width=device-width, initial-scale=1, shrink-to-fit=no" : "width=device-width, initial-scale=1";
            IEnumerable<string> tags = new List<string>()
            {
                @"<meta charset=""utf-8"">",
                $@"<meta name=""viewport"" content=""{viewport}"">",
                $@"<meta name=""author"" content=""{ResourceTypes.CompanyAuthor}"">",
            };

            return new HtmlString(string.Join("\n", tags));
        }

        private static bool IsCssBundle(string outputFileName)
        {
            return outputFileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsJavaScriptBundle(string outputFileName)
        {
            return outputFileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
        }
    }
}
