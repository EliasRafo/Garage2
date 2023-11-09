using Garage2.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System.Text;
using System.Text.Encodings.Web;
using static System.Net.Mime.MediaTypeNames;

namespace Garage2.TagHelpers
{
    [HtmlTargetElement("parkingSpace")]
    public class ParkingSpaceTagHelper : TagHelper
    {
        public string item { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ParkingSpace parkingSpace = JsonConvert.DeserializeObject<ParkingSpace>(item);
            output.TagName = "div";
            output.AddClass("col-lg-2", HtmlEncoder.Default);
            output.AddClass("col-md-2", HtmlEncoder.Default);
            output.AddClass("col-sm-4", HtmlEncoder.Default);
            output.AddClass("col-6", HtmlEncoder.Default);
            output.AddClass("mb-2", HtmlEncoder.Default);

            var builder = new StringBuilder();
            // Your existing code to build parking space content
            output.Content.SetHtmlContent(builder.ToString());
        }
}    }
