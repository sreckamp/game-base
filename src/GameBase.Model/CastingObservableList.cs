using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GameBase.Model
{
    public class CastingObservableList<T, TB> : IObservableList<T>
        where T : TB
    {
        private readonly IObservableList<TB> m_list;

        public CastingObservableList(IObservableList<TB> list)
        {
            m_list = list;
            list.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return m_list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            m_list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            m_list.RemoveAt(index);
        }

        public T this[int index]
        {
            get => (m_list[index] is T t) ? t : default(T);
            set => m_list[index] = value;
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            m_list.Add(item);
        }

        public void Clear()
        {
            m_list.Clear();
        }

        public bool Contains(T item)
        {
            return m_list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (var idx = 0; idx < Count; idx++)
            {
                array[arrayIndex + idx] = this[idx];
            }
        }

        public int Count => m_list.Count;

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            return m_list.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new CastingEnumerator(m_list.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion

        public class CastingEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<TB> m_baseEnum;
            public CastingEnumerator(IEnumerator<TB> baseEnum)
            {
                m_baseEnum = baseEnum;
            }

            #region IEnumerator<T> Members

            public T Current => (m_baseEnum.Current is T t) ? t : default(T);

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                m_baseEnum.Dispose();
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current => m_baseEnum.Current;

            public bool MoveNext()
            {
                return m_baseEnum.MoveNext();
            }

            public void Reset()
            {
                m_baseEnum.Reset();
            }

            #endregion
        }
    }
}
