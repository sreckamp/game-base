using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Model
{
    public class OverlayDispatchableObservableList<T> : INotifyCollectionChanged, IEnumerable<T>, IList<T>
    {
        private readonly IObservableList<T>[] m_lists;

        public OverlayDispatchableObservableList(params IObservableList<T>[] lists)
        {
            m_lists = lists;
            foreach(var list in lists)
            {
                list.CollectionChanged += collectionChanged;
            }
        }

        private void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (System.Windows.Application.Current.Dispatcher?.CheckAccess() ?? true)
            {
                CollectionChanged?.Invoke(this, e);
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
            int idx = 0;
            foreach(var l in m_lists)
            {
                if(l.Contains(item))
                {
                    return idx + l.IndexOf(item);
                }
                else
                {
                    idx += l.Count;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                int idx = 0;
                foreach (var l in m_lists)
                {
                    if (index >= idx && index < idx + l.Count)
                    {
                        return l[index - idx];
                    }
                    else
                    {
                        idx += l.Count;
                    }
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            foreach (var l in m_lists)
            {
                if (l.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int idx = arrayIndex;
            foreach (var l in m_lists)
            {
                l.CopyTo(array, idx);
                idx += Count;
            }
        }

        public int Count
        {
            get {
                int count = 0;
                foreach (var l in m_lists)
                {
                    count += l.Count;
                }
                return count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new OverlayListEnumerator(m_lists);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new OverlayListEnumerator(m_lists);
        }
        #endregion

        public class OverlayListEnumerator : IEnumerator<T>
        {
            private List<IList<T>> m_lists = new List<IList<T>>();
            private IEnumerator<IEnumerator<T>> m_active = null;

            public OverlayListEnumerator(params IList<T>[] lists)
            {
                m_lists.AddRange(lists);
                Reset();
            }
            public T Current => m_active.Current.Current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!m_active.Current?.MoveNext() ?? true)
                {
                    if (!m_active.MoveNext() || !m_active.Current.MoveNext())
                    {
                        return false;
                    }
                }
                return true;
            }

            public void Reset()
            {
                var enums = new List<IEnumerator<T>>();
                foreach (var l in m_lists)
                {
                    enums.Add(l.GetEnumerator());
                }
                m_active = enums.GetEnumerator();
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~OverlayListEnumerator() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

        }
    }
}
