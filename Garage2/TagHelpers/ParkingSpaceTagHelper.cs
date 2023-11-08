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
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("parkingSpace")]
    public class ParkingSpaceTagHelper : TagHelper
    {
        public string item { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ParkingSpace parkingSpace = JsonConvert.DeserializeObject<ParkingSpace>(item);

            output.TagName = "div";
            output.AddClass("col-lg-4", HtmlEncoder.Default);
            output.AddClass("col-md-6", HtmlEncoder.Default);
            output.AddClass("mb-3", HtmlEncoder.Default);

            var builder = new StringBuilder();

            builder.Append($"<div class='card py-4 px-lg-5 h-100'>");
            builder.Append($"<div class='card-body d-flex flex-column'>");
            builder.Append($"<div class='text-center'>");
            builder.Append($"<img src='https://drive.google.com/uc?export=view&id=1HswgEjS9kRoAKUDOMmDhnhUyyah7wBW9' class='img-fluid  mb-5' alt='Websearch'>");

            builder.Append($"</div>");
            builder.Append($"<div class='card-title mb-4 text-center fs-2'>A {parkingSpace.Id}</div>");
            builder.Append($"<div class='pricing'>");
            builder.Append($"<ul class='list-unstyled'>");

            if (parkingSpace.Reserved == true)
            {
                builder.Append($"<li class='mb-3'>");
                builder.Append($"<span class='small ms-3'>Registration number: {parkingSpace.Vehicle.RegNum}</span>");
                builder.Append($"</li>");
                builder.Append($"<li class='mb-3'>");
                builder.Append($"<span class='small ms-3'>Registration number: {parkingSpace.Vehicle.Type}</span>");
                builder.Append($"</li>");
                builder.Append($"<li class='mb-3'>");
                builder.Append($"<span class='small ms-3'>Parking time {parkingSpace.Vehicle.ParkingTime}</span>");
                builder.Append($"</li>");
                builder.Append($"</ul>");
                    builder.Append($"</div>");
                    builder.Append($"<div class='text-center'><a asp-controller='Vehicles' asp-action='Unparking' asp-route-id='{parkingSpace.Vehicle.VehicleId}'>Unparking</a></div>");

                    builder.Append($"</div>");
                builder.Append($"</div>");
                builder.Append($"</div>");


                
            }
            else
            {
                //output.AddClass("bg-success", HtmlEncoder.Default);

                //builder.Append($"<img src='...' class='card-img-top' alt='...'>");
                //builder.Append($"<div class='card-body'>");
                //builder.Append($"<h5 class='card-title'>A {parkingSpace.Id}</h5>");
                //builder.Append($"<p class='card-text'> </p>");


                //builder.Append($"<a asp-controller='Vehicles' asp-action='Park' asp-route-id='{parkingSpace.Id}'>parking</a>");

                //builder.Append($"</div>");
                //builder.Append($"<div class='card-footer'>");
                //builder.Append($"<small class='text-muted'></small>");
                //builder.Append($"</div>");
                //builder.Append($"</div>");
            }

            output.Content.SetHtmlContent(builder.ToString());
        }
    }
}
