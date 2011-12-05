using System;
using System.Linq;
using System.Linq.Expressions;

namespace Xps2ImgUI.Utils
{
    public static class PropertiesComparator
    {
        public static bool Equals<T>(T x, T y)
        {
            return Cache<T>.Compare(x, y);
        }

        private static class Cache<T>
        {
            // ReSharper disable StaticFieldInGenericType
            internal static readonly Func<T, T, bool> Compare;
            // ReSharper restore StaticFieldInGenericType

            static Cache()
            {
                var properties = typeof(T).GetProperties();
                if (properties.Length == 0)
                {
                    Compare = delegate { return true; };
                    return;
                }

                var x = Expression.Parameter(typeof(T), "x");
                var y = Expression.Parameter(typeof(T), "y");

                var body = properties
                            .Select(t => Expression.Equal(Expression.Property(x, t), Expression.Property(y, t)))
                            .Aggregate<BinaryExpression, Expression>(null, (current, propEqual) => current == null ? propEqual : Expression.AndAlso(current, propEqual));

                Compare = Expression.Lambda<Func<T, T, bool>>(body, x, y).Compile();
            }
        }
    }
}
