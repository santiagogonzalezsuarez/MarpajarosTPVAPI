using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MarpajarosTPVAPI.Classes
{
    public class ReflectionQueryable
    {
        private static readonly MethodInfo OrderByASCMethod = typeof(Queryable).GetMethods().Where(method => method.Name == "OrderBy").Where(method => method.GetParameters().Length == 2).Single();
        private static readonly MethodInfo OrderByDESMethod = typeof(Queryable).GetMethods().Where(method => method.Name == "OrderByDescending").Where(method => method.GetParameters().Length == 2).Single();
        private static readonly MethodInfo ThenByASCMethod = typeof(Queryable).GetMethods().Where(method => method.Name == "ThenBy").Where(method => method.GetParameters().Length == 2).Single();
        private static readonly MethodInfo ThenByDESMethod = typeof(Queryable).GetMethods().Where(method => method.Name == "ThenByDescending").Where(method => method.GetParameters().Length == 2).Single();

        public static IQueryable<TSource> OrderByProperty<TSource>(IQueryable<TSource> source, string propertyName, bool DESC = false, bool SortById = true)
        {
            var parameter = Expression.Parameter(typeof(TSource), "SourceType");
            Expression orderByPropertyExp = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(orderByPropertyExp, new ParameterExpression[] { parameter });
            MethodInfo genericMethod;
            if (DESC)
            {
                genericMethod = OrderByDESMethod.MakeGenericMethod(new Type[] { typeof(TSource), orderByPropertyExp.Type });
            }
            else
            {
                genericMethod = OrderByASCMethod.MakeGenericMethod(new Type[] { typeof(TSource), orderByPropertyExp.Type });
            }

            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            if (!SortById)
            {
                return (IQueryable<TSource>)ret;
            }
            else
            {
                Expression idPropertyExp = Expression.Property(parameter, "Id");
                var lambda2 = Expression.Lambda(idPropertyExp, new ParameterExpression[] { parameter });
                MethodInfo genericMethod2;
                if (DESC)
                {
                    genericMethod2 = ThenByDESMethod.MakeGenericMethod(new Type[] { typeof(TSource), idPropertyExp.Type });
                }
                else
                {
                    genericMethod2 = ThenByASCMethod.MakeGenericMethod(new Type[] { typeof(TSource), idPropertyExp.Type });
                }

                var ret2 = genericMethod2.Invoke(null, new object[] { ret, lambda2 });
                return (IQueryable<TSource>)ret2;
            }
        }
    }
}