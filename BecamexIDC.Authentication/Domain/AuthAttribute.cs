using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BecamexIDC.Authentication.Domain
{
    public class AuthAttribut
    {
        void GetAll()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            asm.GetTypes()
                .Where(type=> typeof(Controller).IsAssignableFrom(type)) //filter controllers
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && ! method.IsDefined(typeof(NonActionAttribute)));
        }
    }
}