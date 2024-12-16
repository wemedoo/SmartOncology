using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.JsonModelBinder
{
    public class JsonNetModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
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

            var model = JsonConvert.DeserializeObject(requestBody, bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
