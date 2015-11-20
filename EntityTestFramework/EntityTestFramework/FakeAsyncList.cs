using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Query;

namespace EntityTestFramework
{
    public class FakeAsyncList<T> : IQueryable, IQueryProvider
    {
        public List<T> SourceList { get; }

        public FakeAsyncList(List<T> source)
        {
            SourceList = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var result = GetListFromExpression<object>(expression);

            return result.AsQueryable();
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            var result = GetListFromExpression<TResult>(expression);

            return result.AsQueryable();
        }

        public object Execute(Expression expression)
        {
            var lambda = Expression.Lambda(expression, Expression.Parameter(typeof(List<T>)));
            var x = lambda.Compile();
            return x.DynamicInvoke(SourceList);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var syncResult = GetListFromExpression<TResult>(expression);

            return syncResult.ToAsyncEnumerable();
        }

        private IEnumerable<TResult> GetListFromExpression<TResult>(Expression expression)
        {
            var lambda = Expression.Lambda(expression, Expression.Parameter(typeof(List<T>)));
            var x = lambda.Compile();
            var objResult = x.DynamicInvoke(SourceList);
            var syncResult = (IEnumerable<TResult>)objResult;
            return syncResult;
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var result = Execute<TResult>(expression);
            return Task.FromResult(result);
        }

        public IEnumerator GetEnumerator()
        {
            return SourceList.GetEnumerator();
        }

        public IQueryProvider Provider => this;

        public Type ElementType => SourceList.AsQueryable().ElementType;

        public Expression Expression => SourceList.AsQueryable().Expression;


    }
}