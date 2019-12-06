using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;

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
            m_base = new List<T>();
        }
        public ObservableList(int size)
        {
            m_base = new List<T>(size);
        }
        public void Add(T t)
        {
            m_base.Add(t);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, m_base.IndexOf(t));
            CollectionChanged?.Invoke(this, args);
        }

        public void AddRange(IEnumerable<T> values)
        {
            m_base.AddRange(values);
            if (CollectionChanged != null)
            {
                foreach (var t in values)
                {
                    var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, m_base.IndexOf(t));
                    CollectionChanged(this, args);
                }
            }
        }

        public void Insert(int idx, T t)
        {
            m_base.Insert(idx, t);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, idx);
            CollectionChanged?.Invoke(this, args);
        }

        public bool Remove(T item)
        {
            var idx = m_base.IndexOf(item);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx);
            CollectionChanged?.Invoke(this, args);
            var result = m_base.Remove(item);
            return result;
        }

        public void RemoveAt(int idx)
        {
            Remove(m_base[idx]);
            //var t = m_base[idx];
            //m_base.RemoveAt(idx);
            //var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, t, idx);
            //CollectionChanged?.Invoke(this, args);
        }

        public int RemoveAll(Predicate<T> match)
        {
            var result = m_base.RemoveAll(match);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove);
            CollectionChanged?.Invoke(this, args);
            return result;
        }

        public void Clear()
        {
            m_base.Clear();
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(this, args);
        }

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
