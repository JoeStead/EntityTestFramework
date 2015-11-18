using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Entity;

namespace EntityTestFramework
{
    public class ConfigurableContext<T> where T : new()
    {
        public readonly T Context;
        private readonly Dictionary<Type, object> _data;

        public ConfigurableContext(Action<ConfigurableContext<T>> configuration)
        {
            _data = new Dictionary<Type, object>();
            Context = new T();
            configuration.Invoke(this);
        }

        public static implicit operator T(ConfigurableContext<T> configurableContext)
        {
            return configurableContext.Context;
        }

        public void Setup<TU>(Expression<Func<T, DbSet<TU>>> property) where TU : class, new()
        {
            Setup(property, new List<TU>());
        }

        public void Setup<TU>(Expression<Func<T, DbSet<TU>>> property, List<TU> seed) where TU : class, new()
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member.Member as PropertyInfo;

            var fakeDbSet = new FakeDbSet<TU>(seed);

            if (_data.ContainsKey(typeof(TU)))
            {
                _data.Remove(typeof(TU));
            }

            _data.Add(typeof(TU), fakeDbSet);
            propertyInfo.SetValue(Context, fakeDbSet);
        }
    }
}
