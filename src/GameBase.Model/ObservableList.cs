using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace GameBase.Model
{
    public interface IObservableList<T> : IList<T>, INotifyCollectionChanged { }
    public class ObservableList<T> : IObservableList<T>
    {
        private readonly List<T> m_base;

        public int Count => m_base.Count;

        public bool IsReadOnly => false;

        public T this[int index] {
            get => m_base[index];
            set
            {
                if(Count > index)
                {
                    RemoveAt(index);
                }
                Insert(index, value);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableList()
        {
            CollectionChanged += (sender, args) => { }; //Debug.WriteLine($"ObservableList.CollectionChanged {typeof(T).Name}"); };
            m_base = new List<T>();
        }

        public void Add(T t)
        {
            m_base.Add(t);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, m_base.IndexOf(t));
            CollectionChanged.Invoke(this, args);
        }

        // ReSharper disable once UnusedMember.Global
        public void AddRange(IEnumerable<T> values)
        {
            foreach (var t in values)
            {
                Add(t);
            }
        }

        public void Insert(int idx, T t)
        {
            m_base.Insert(idx, t);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, idx);
            CollectionChanged.Invoke(this, args);
        }

        public bool Remove(T item)
        {
            var idx = m_base.IndexOf(item);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx);
            CollectionChanged.Invoke(this, args);
            var result = m_base.Remove(item);
            return result;
        }

        public void RemoveAt(int idx)
        {
            Remove(m_base[idx]);
        }

        // ReSharper disable once UnusedMember.Global
        public int RemoveAll(Predicate<T> match)
        {
            var remove = m_base.Where(t => match(t)).ToList();
            foreach (var t in remove)
            {
                Remove(t);
            }
            return remove.Count;
        }

        public void Clear()
        {
            m_base.Clear();
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged.Invoke(this, args);
        }

        // ReSharper disable once UnusedMember.Global
        public int IndexOf(T item)
        {
            return m_base.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return m_base.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_base.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_base.GetEnumerator();
        }
    }
}
