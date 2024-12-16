using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.User.DTO;
using System.Reflection;

namespace sReportsV2.Common.Extensions
{
    using AutoMapper;
    using System.Reflection;

    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination>
        IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var destinationProperty in destinationProperties)
            {
                var sourceProperty = sourceType.GetProperty(destinationProperty.Name, BindingFlags.Public | BindingFlags.Instance);

                if (sourceProperty == null)
                {
                    expression.ForMember(destinationProperty.Name, opt => opt.Ignore());
                }
            }

            return expression;
        }
    }
}