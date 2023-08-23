using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.HealthCheck
{
    public class HealthCheckResponseWriter
    {
        public static  Task WriteResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json";

            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using(var jsonWritter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWritter.WriteStartObject();
                jsonWritter.WriteString("status", healthReport.Status.ToString());
                jsonWritter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWritter.WriteStartObject(healthReportEntry.Key);
                    jsonWritter.WriteString("status", healthReportEntry.Value.Status.ToString());
                    jsonWritter.WriteString("description", healthReportEntry.Value.Description);
                    jsonWritter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWritter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWritter, item.Value , item.Value?.GetType() ?? typeof(object));
                    }
                    jsonWritter.WriteEndObject();
                    jsonWritter.WriteEndObject();
                }
                jsonWritter.WriteEndObject();
                jsonWritter.WriteEndObject();
            }
            return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }
}
