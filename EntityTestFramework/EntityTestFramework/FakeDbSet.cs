using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

namespace EntityTestFramework
{
    public class FakeDbSet<T> : DbSet<T>, IEnumerable<T>, IQueryable, IAsyncEnumerable<T> where T : class
    {

        private readonly FakeAsyncList<T> _data;

        public List<T> Entities => _data.Source;

        public FakeDbSet(List<T> data)
        {
            _data = new FakeAsyncList<T>(data);
        }

        public override EntityEntry<T> Add(T entity, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            _data.Source.Add(entity);
            return null;
        }

        public override void AddRange(IEnumerable<T> entities, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            _data.Source.AddRange(entities);
        }

        public override void AddRange(params T[] entities)
        {
            _data.Source.AddRange(entities);
        }

        public override EntityEntry<T> Remove(T entity)
        {
            _data.Source.Remove(entity);

            return null;
        }

        public override void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _data.Source.Remove(entity);
            }
        }

        public override void RemoveRange(params T[] entities)
        {
            foreach (var entity in entities)
            {
                _data.Source.Remove(entity);
            }
        }

        public override EntityEntry<T> Update(T entity, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            _data.Source.Remove(entity);
            _data.Source.Add(entity);
            return null;
        }

        public override void UpdateRange(IEnumerable<T> entities, GraphBehavior behavior = GraphBehavior.IncludeDependents)
        {
            var entitiyArray = entities.ToArray();
            foreach (var entity in entitiyArray)
            {
                _data.Source.Remove(entity);
            }

            _data.Source.AddRange(entitiyArray);
        }

        public override void UpdateRange(params T[] entities)
        {
            UpdateRange(entities.AsEnumerable());
        }

        Type IQueryable.ElementType => _data.ElementType;

        Expression IQueryable.Expression => _data.Expression;

        IQueryProvider IQueryable.Provider => _data.Provider;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.Source.GetEnumerator();
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return _data.Source.ToAsyncEnumerable().GetEnumerator();
        }
    }
}
