using BulkyBookPractice.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Pagination PageModel { get; set; }

        // CSS properties
        public string PageAction { get; set; } // Defines what action is invoked when a user selects another page
        public bool PageClassEnabled { get; set; }
        public string PageClass { get; set; } // Initial CSS properties
        public string PageClassNormal { get; set; } // CSS properties for pages that are not selected
        public string PageClassSelected { get; set; } // CSS properties for selected page

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder result = new("div");

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                // Create anchor tag with a url.
                // When a url is created, a ":" is appended to the end, and must be replaced with the count number
                TagBuilder tag = new("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                if (PageClassEnabled)
                {
                    // Apply Css
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }

                // Display the count number as text
                tag.InnerHtml.Append(i.ToString());

                // Append the Tag
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
