using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

namespace EntityTestFramework
{
    public class FakeDbSet<T> : DbSet<T>, IEnumerable<T>, IQueryable where T : class
    {

        public readonly FakeAsyncList<T> _data;

        public List<T> Entities => _data.SourceList;

        public FakeDbSet(List<T> data)
        {
            _data = new FakeAsyncList<T>(data);
        }

        public override EntityEntry<T> Add(T entity, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            _data.SourceList.Add(entity);
            return null;
        }

        public override void AddRange(IEnumerable<T> entities, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            _data.SourceList.AddRange(entities);
        }

        public override void AddRange(params T[] entities)
        {
            _data.SourceList.AddRange(entities);
        }

        public override EntityEntry<T> Remove(T entity)
        {
            _data.SourceList.Remove(entity);

            return null;
        }

        public override void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _data.SourceList.Remove(entity);
            }
        }

        public override void RemoveRange(params T[] entities)
        {
            foreach (var entity in entities)
            {
                _data.SourceList.Remove(entity);
            }
        }

        Type IQueryable.ElementType => _data.ElementType;

        Expression IQueryable.Expression => _data.Expression;

        IQueryProvider IQueryable.Provider => _data.Provider;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.SourceList.GetEnumerator();
        }
    }
}
