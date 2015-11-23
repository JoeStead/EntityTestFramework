using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Query.Internal;

namespace EntityTestFramework
{
    public class FakeAsyncList<T> : IQueryable<T>, IAsyncQueryProvider, IAsyncEnumerable<T>
    {
        public List<T> Source { get; }

        public FakeAsyncList(IEnumerable<T> source)
        {
            Source = source.ToList();
        }
        
        public IQueryable CreateQuery(Expression expression)
        {
            var result = GetListFromExpression<object>(expression);

            return result.AsQueryable();
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            var result = GetListFromExpression<TResult>(expression);

            return result;
        }

        public object Execute(Expression expression)
        {
            var lambda = Expression.Lambda(expression, Expression.Parameter(typeof(FakeAsyncList<T>)));
            var x = lambda.Compile();
            return x.DynamicInvoke(this);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var syncResult = GetListFromExpression<TResult>(expression);

            return syncResult;
        }

        private FakeAsyncList<TResult> GetListFromExpression<TResult>(Expression expression)
        {
            var lambda = Expression.Lambda(expression, Expression.Parameter(typeof(FakeAsyncList<T>)));
            var x = lambda.Compile();
            var objResult = x.DynamicInvoke(this);
            var syncResult = new FakeAsyncList<TResult>((IEnumerable<TResult>)objResult);
            return syncResult;
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var result = Execute<TResult>(expression);
            return Task.FromResult(result);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        public IQueryProvider Provider => this;

        public Type ElementType => Source.AsQueryable().ElementType;

        public Expression Expression => Source.AsQueryable().Expression;

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return Source.ToAsyncEnumerable().GetEnumerator();
        }
    }
}