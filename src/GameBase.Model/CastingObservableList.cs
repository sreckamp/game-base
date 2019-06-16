using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using GameBase.Model;

namespace GameBoard.Model
{
    public class CastingObservableList<T, B> : IObservableList<T>
        where T : B
    {
        private readonly IObservableList<B> m_list;

        public CastingObservableList(IObservableList<B> list)
        {
            m_list = list;
            list.CollectionChanged += new NotifyCollectionChangedEventHandler(collectionChanged);
        }

        private void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            get
            {
                return (m_list[index] is T t) ? t : default(T);
            }
            set
            {
                m_list[index] = value;
            }
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
            for (int idx = 0; idx < Count; idx++)
            {
                array[arrayIndex + idx] = this[idx];
            }
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

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
            private readonly IEnumerator<B> m_baseEnum;
            public CastingEnumerator(IEnumerator<B> baseEnum)
            {
                m_baseEnum = baseEnum;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get
                {
                    return (m_baseEnum.Current is T t) ? t : default(T);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                m_baseEnum.Dispose();
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return m_baseEnum.Current; }
            }

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
