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
    public class FakeAsyncList<T> :  IQueryable, IQueryProvider
    {
        public List<T> SourceList { get; }

        public FakeAsyncList(List<T> source)
        {
            SourceList = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            throw new NotImplementedException();
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
            var lambda = Expression.Lambda(expression, Expression.Parameter(typeof(List<T>)));
            var x = lambda.Compile();
            var objResult = x.DynamicInvoke(SourceList);
            var syncResult = (List<TResult>)objResult;

            return syncResult.ToAsyncEnumerable();
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