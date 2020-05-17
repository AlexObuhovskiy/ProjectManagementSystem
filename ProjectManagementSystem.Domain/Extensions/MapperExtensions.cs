using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace ProjectManagementSystem.Domain.Extensions
{
    /// <summary>
    /// Class MapperExtensions.
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Ignores all virtual.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDestination&gt;.</returns>
        public static IMappingExpression<TSource, TDestination> IgnoreAllVirtual<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var destinationType = typeof(TDestination);

            IEnumerable<PropertyInfo> virtualProps = destinationType.GetProperties().Where(p => p.GetGetMethod().IsVirtual);

            foreach (var property in virtualProps)
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }
    }
}