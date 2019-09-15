using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RestApiBase.Annotations;
using RestApiBase.Models;

namespace GenericRestApi.Helpers
{
    public class ExpressionTreeHelper<TEntity> where TEntity : EntityBase
    {
        private readonly List<PropertyInfo> filterProps;
        private readonly ParameterExpression parameter;
        private readonly MethodInfo containsMethod;
        private readonly MethodInfo toLowerMethod;
        private readonly MemberExpression[] members;

        public ExpressionTreeHelper()
        {
            filterProps = GetFilterProps();
            parameter = Expression.Parameter(typeof(TEntity), "e");
            containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
            members = GetExpressionMembers(filterProps, parameter);
        }

        /// <summary>
        /// Crea y retorna un arbol de expresiones de busqueda.
        /// </summary>
        /// <param name="query">La consulta a filtrar</param>
        /// <param name="filter">El filtro de busqueda en string</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> CreateSearchExpression(IQueryable<TEntity> query, string filter)
        {
            // https://stackoverflow.com/questions/34192488/build-an-expression-tree-with-multiple-parameters
            // https://stackoverflow.com/questions/57209466/generic-search-with-expression-trees-gives-system-nullreferenceexception-for-nul
            if (string.IsNullOrEmpty(filter) || this.filterProps.Count == 0)
                throw new Exception("Argumentos invalidos. No es posible crear el arbol de expresiones");

            ConstantExpression constant = Expression.Constant(filter.ToLower());
            Expression searchExp = null;

            foreach (var member in members)
            {
                // e => e.Member != null
                BinaryExpression notNullExp = Expression.NotEqual(member, Expression.Constant(null));
                // e => e.Member.ToLower() 
                MethodCallExpression toLowerExp = Expression.Call(member, toLowerMethod);
                // e => e.Member.Contains(value)
                MethodCallExpression containsExp = Expression.Call(toLowerExp, containsMethod, constant);
                // e => e.Member != null && e.Member.Contains(value)
                BinaryExpression filterExpression = Expression.AndAlso(notNullExp, containsExp);

                searchExp = searchExp == null ? (Expression)filterExpression : Expression.OrElse(searchExp, filterExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(searchExp, parameter);
        }

        public Expression<Func<TEntity, bool>> CreateActiveExpression(IQueryable<TEntity> query)
        {
            var constant = Expression.Constant(true);
            var activeProp = Expression.Property(parameter, "Activo");
            // e => e.Activo == true
            var activeExp = Expression.Equal(activeProp, constant);

            return Expression.Lambda<Func<TEntity, bool>>(activeExp, parameter);
        }

        // Obtiene los atributos filtrables de TEntity
        private List<PropertyInfo> GetFilterProps()
        {
            var t = typeof(TEntity);
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

        private MemberExpression[] GetExpressionMembers(List<PropertyInfo> filterProps, ParameterExpression parameter)
        {
            MemberExpression[] members = new MemberExpression[filterProps.Count()];

            for (int i = 0; i < filterProps.Count(); i++)
            {
                var attr = (SearchFilter[])filterProps[i].GetCustomAttributes(typeof(SearchFilter), false);
                var nestedProp = attr[0].nestedProp;

                if (nestedProp != null)
                {   // si el filtro es una propiedad de una entidad anidada
                    members[i] = GetNestedMemberExpression(parameter, nestedProp);
                }
                else
                {
                    members[i] = Expression.Property(parameter, filterProps[i]);
                }
            }

            return members;
        }

        private MemberExpression GetNestedMemberExpression(ParameterExpression parameter, string propertyName)
        {
            Expression expression = parameter;
            foreach (var member in propertyName.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, member);
            }
            return (MemberExpression)expression;
        }

    }
}