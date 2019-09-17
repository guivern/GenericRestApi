using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RestApiBase.Annotations;

namespace GenericRestApi.Helpers
{
    public class ExpressionTreeHelper<T>
    {
        /// <summary>
        /// Crea y retorna un arbol de expresiones de busqueda.
        /// </summary>
        /// <param name="query">La consulta a filtrar</param>
        /// <param name="filter">El filtro de busqueda en string</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateSearchExpression(IQueryable<T> query, string filter)
        {
            var filterProps = GetFilterProps();
            if (string.IsNullOrEmpty(filter) || filterProps.Count == 0)
                throw new Exception("No es posible crear el arbol de expresiones");

            var parameter = Expression.Parameter(typeof(T), "e");
            var members = GenerateFilterableMembers(filterProps, parameter);
            var constant = Expression.Constant(filter.ToLower());
            var searchExp = GenerateSearchExpression(members, constant);

            return Expression.Lambda<Func<T, bool>>(searchExp, parameter);
        }

        public static Expression<Func<T, bool>> CreateSoftDeleteExpression(IQueryable<T> query)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var member = Expression.Property(parameter, "Activo");
            var constant = Expression.Constant(true);
            // e => e.Activo == true
            var activeExp = Expression.Equal(member, constant);

            return Expression.Lambda<Func<T, bool>>(activeExp, parameter);
        }

        // Obtiene los atributos filtrables de T
        private static List<PropertyInfo> GetFilterProps()
        {
            var t = typeof(T);
            var props = t.GetProperties();
            var filterProps = new List<PropertyInfo>();

            foreach (var prop in props)
            {
                var attr = (SearchFilter[])prop.GetCustomAttributes(typeof(SearchFilter), false);
                if (attr.Length > 0)
                {
                    filterProps.Add(prop);
                }
            }

            return filterProps;
        }

        private static MemberExpression[] GenerateFilterableMembers(List<PropertyInfo> filterProps, ParameterExpression parameter)
        {
            MemberExpression[] members = new MemberExpression[filterProps.Count()];

            for (int i = 0; i < filterProps.Count(); i++)
            {
                var attr = (SearchFilter[])filterProps[i].GetCustomAttributes(typeof(SearchFilter), false);
                var nestedProp = attr[0].nestedProp;

                if (nestedProp != null)
                {   // si el filtro es una propiedad de una entidad anidada
                    members[i] = GenerateNestedFilterableMember(parameter, nestedProp);
                }
                else
                {
                    members[i] = Expression.Property(parameter, filterProps[i]);
                }
            }

            return members;
        }

        private static MemberExpression GenerateNestedFilterableMember(ParameterExpression parameter, string propertyName)
        {
            Expression expression = parameter;
            foreach (var member in propertyName.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, member);
            }
            return (MemberExpression)expression;
        }
        
        private static Expression GenerateSearchExpression(MemberExpression[] members, ConstantExpression constant)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
            Expression searchExp = null;

            foreach (var member in members)
            {
                // e => e.Member != null
                var notNullExp = Expression.NotEqual(member, Expression.Constant(null));
                // e => e.Member.ToLower() 
                var toLowerExp = Expression.Call(member, toLowerMethod);
                // e => e.Member.Contains(value)
                var containsExp = Expression.Call(toLowerExp, containsMethod, constant);
                // e => e.Member != null && e.Member.Contains(value)
                var filterExpression = Expression.AndAlso(notNullExp, containsExp);

                searchExp = searchExp == null ? (Expression)filterExpression : Expression.OrElse(searchExp, filterExpression);
            }

            return searchExp;
        }
    }
}