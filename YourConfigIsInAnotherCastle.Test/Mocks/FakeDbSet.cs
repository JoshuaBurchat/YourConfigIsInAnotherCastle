using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepEqual.Syntax;
namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public class FakeDbSet<T> : IDbSet<T>
      where T : class
    {
        ObservableCollection<T> _data;
        ObservableCollection<T> _local;
        IQueryable _query;

        private Func<T, object[]> _buildKey;
        public void ClearLocal()
        {
            this._local.Clear();
        }
        private bool DoesLocalHave(T record)
        {
            if (record != null)
            {
                var key = _buildKey(record);
                return _local.FirstOrDefault(l => _buildKey(l).IsDeepEqual(key)) != null;
            }
            return false;
        }

        private void TryAddLocal(params T[] records)
        {
            foreach (var record in records)
            {
                if (DoesLocalHave(record))
                {
                    _local.Add(record);
                }
            }
        }
        public FakeDbSet(Func<T, object[]> buildKey, NotifyCollectionChangedEventHandler change = null)
        {
            _buildKey = buildKey;
            _local = new ObservableCollection<T>();
            _data = new ObservableCollection<T>();
            _query = _data.AsQueryable();
            if (change != null)
                _data.CollectionChanged += change;
        }

        public virtual T Find(params object[] keyValues)
        {
            var results = _data.Where(d => _buildKey(d).IsDeepEqual(keyValues)).FirstOrDefault();
            TryAddLocal(results);
            return results;
        }

        public T Add(T item)
        {
            _data.Add(item);
            _local.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            _local.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            //If its reference is in local its np
            if (!Local.Contains(item) && DoesLocalHave(item))
            {
                throw new Exception("item is already attached locally");
            }
            _data.Add(item);
            _local.Add(item);
            return item;
        }

        public T Detach(T item)
        {
            _data.Remove(item);
            _local.Remove(item);
            return item;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public ObservableCollection<T> Local
        {
            get { return _local; }
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
