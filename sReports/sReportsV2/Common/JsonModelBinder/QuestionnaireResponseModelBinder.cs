using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using sReportsV2.Common.Extensions;
using System.IO;
using System.Text;

namespace sReportsV2.Common.JsonModelBinder
{
    public class JsonFhirModelBinder : IModelBinder
    {
        public async System.Threading.Tasks.Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Ensure.IsNotNull(bindingContext, nameof(bindingContext));

            var request = bindingContext.HttpContext.Request;
            request.EnableBuffering();

            string requestBody;
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            var parser = new FhirJsonParser();
            var fhirResource = parser.Parse<Resource>(requestBody);

            bindingContext.Result = ModelBindingResult.Success(fhirResource);
        }
    }
}
