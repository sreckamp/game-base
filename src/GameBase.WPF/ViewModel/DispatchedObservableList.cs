using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Collections;
using GameBase.Model;

namespace GameBoard.WPF.ViewModel
{
    public class DispatchedObservableList<T> : INotifyCollectionChanged, IEnumerable<T>, IList<T>
    {
        private readonly IObservableList<T> m_list;

        public DispatchedObservableList(IObservableList<T> list) 
        {
            m_list = list;
            list.CollectionChanged += new NotifyCollectionChangedEventHandler(collectionChanged);
        }

        private void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (System.Windows.Application.Current.Dispatcher?.CheckAccess() ?? true)
            {
                CollectionChanged?.Invoke(sender, e);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<object, NotifyCollectionChangedEventArgs>((o, ea) =>
                        { collectionChanged(o, ea); }), sender, e);
            }
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
                return m_list[index];
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
            m_list.CopyTo(array, arrayIndex);
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
            return m_list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #endregion
    }
}
