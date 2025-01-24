using System.Linq.Expressions;

namespace EntityFramework.Extensions
{
    public static class QueryableExtensions
    {
        public enum Order
        {
            Asc = 1,
            Desc = -1
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string property, int? direction)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
            MemberExpression memberExpression = Expression.PropertyOrField(parameterExpression, PropertyHelper.GetPropertyName(PropertyHelper.GetProperty<T>(property)));
            LambdaExpression expression = Expression.Lambda(memberExpression, parameterExpression);
            Order order = ((1 == direction) ? Order.Asc : Order.Desc);
            MethodCallExpression expression2 = Expression.Call(typeof(Queryable), (order == Order.Asc) ? "OrderBy" : "OrderByDescending",
            [
            typeof(T),
            memberExpression.Type
            ], query.Expression, Expression.Quote(expression));
            return query.Provider.CreateQuery<T>(expression2);
        }
    }
}
