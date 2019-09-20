using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GenericRestApi.Helpers
{
    public abstract class ExpressionHelper<T>
    {
        public static IQueryable<T> GenerateSearchQuery(List<string> filterProps, IQueryable<T> query, string filter)
        {
            if (string.IsNullOrEmpty(filter) || filterProps.Count == 0)
                throw new Exception("No es posible crear el arbol de expresiones");

            var parameter = Expression.Parameter(typeof(T), "e");
            var members = GenerateFilterableMembers(filterProps, parameter);
            var constant = Expression.Constant(filter.ToLower());
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

            var predicate = Expression.Lambda<Func<T, bool>>(searchExp, parameter);

            return query.Where(predicate);
        }

        public static IQueryable<T> GenerateSoftDeleteQuery(IQueryable<T> query)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var member = Expression.Property(parameter, "Activo");
            var constant = Expression.Constant(true);
            // e => e.Activo == true
            var activeExp = Expression.Equal(member, constant);
            var predicate = Expression.Lambda<Func<T, bool>>(activeExp, parameter);

            return query.Where(predicate);
        }

        public static IQueryable<T> GenerateOrderByQuery(IQueryable<T> query, List<string> orderByProperties)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            for (var i = 0; i < orderByProperties.Count(); i++)
            {
                var splitedOrder = orderByProperties[i].Split(':');
                var columnName = splitedOrder[0];
                var orderType = splitedOrder.Count() > 1 ? splitedOrder[1] : "asc";
                var member = columnName.Split('.')
                    .Aggregate((Expression)parameter, Expression.PropertyOrField);
                var expression = Expression.Lambda(member, parameter);
                var orderMethod = "";

                if (i == 0)
                {
                    // la primera vez es orderBy
                    orderMethod = orderType == "asc" ? "OrderBy" : "OrderByDescending";
                }
                else
                {
                    // luego es ThenBy
                    orderMethod = orderType == "asc" ? "ThenBy" : "ThenByDescending";
                }

                // typeof(T) is the type of the Entity
                // exp.Body.Type is the type of the property. 
                Type[] types = new Type[] { type, expression.ReturnType };

                // Build the call expression
                // It will look something like:
                //     OrderBy*(x => x.Cassette) or Order*(x => x.SlotNumber)
                //     ThenBy*(x => x.Cassette) or ThenBy*(x => x.SlotNumber)
                var callExpression = Expression.Call(typeof(Queryable), orderMethod, types,
                    query.Expression, expression);

                query = query.Provider.CreateQuery<T>(callExpression);
            }

            return query;
        }

        // convierte un string a capitalize
        private static string FirstCharToUpper(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return char.ToUpper(value[0]) + value.Substring(1);
        }

        private static MemberExpression[] GenerateFilterableMembers(List<string> filterProps, ParameterExpression parameter)
        {
            var members = new MemberExpression[filterProps.Count()];

            for (int i = 0; i < filterProps.Count(); i++)
            {
                if (filterProps[i].Contains('.'))
                {   // el filtro es una propiedad de una entidad anidada
                    // ej. u => u.Rol.Nombre
                    Expression nestedMember = parameter;
                    foreach (var prop in filterProps[i].Split('.'))
                    {
                        nestedMember = Expression.PropertyOrField(nestedMember, prop);
                    }
                    members[i] = (MemberExpression)nestedMember;
                }
                else
                {
                    // el filtro es una propiedad de la entidad
                    // ej. u => u.Username
                    members[i] = Expression.Property(parameter, filterProps[i]);
                }
            }

            return members;
        }
    }
}