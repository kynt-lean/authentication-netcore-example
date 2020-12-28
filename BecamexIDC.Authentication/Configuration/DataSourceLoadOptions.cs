using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Configuration
{
    [ModelBinder(typeof(DataSourceLoadOptionsHttpBinder))]
    public class DataSourceLoadOptions : DataSourceLoadOptionsBase
    {
        public string Key { set; get; }
        public string SpecialKey { set; get; }
    }

    class DataSourceLoadOptionsHttpBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var loadOptions = new DataSourceLoadOptions();
            DataSourceLoadOptionsParser.Parse(loadOptions, key => bindingContext.ValueProvider.GetValue(key).FirstOrDefault());
            bindingContext.Result = ModelBindingResult.Success(loadOptions);
            return Task.CompletedTask;
        }

    }
}