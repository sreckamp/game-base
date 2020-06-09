using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using GameBase.Model;

namespace GameBase.WPF.ViewModel
{
    public class OverlayObservableList<T> : INotifyCollectionChanged, IEnumerable<T> where T : notnull
    {
        private readonly IObservableList<T>[] m_lists;

        public OverlayObservableList(params IObservableList<T>[] lists)
        {
            m_lists = lists;
            foreach (var list in lists)
            {
                list.CollectionChanged += OnCollectionChanged;
            }

            CollectionChanged += (sender, args) => { };
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // if (Application.Current.Dispatcher?.CheckAccess() ?? true)
            // {
                CollectionChanged?.Invoke(this, e);
            // }
            // else
            // {
            //     Application.Current.Dispatcher.Invoke(
            //         new Action<object, NotifyCollectionChangedEventArgs>(OnCollectionChanged), sender, e);
            // }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            var idx = 0;
            foreach (var l in m_lists)
            {
                if (l.Contains(item))
                {
                    return idx + l.IndexOf(item);
                }

                idx += l.Count;
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
                var idx = 0;
                foreach (var l in m_lists)
                {
                    if (index >= idx && index < idx + l.Count)
                    {
                        return l[index - idx];
                    }

                    idx += l.Count;
                }

                throw new IndexOutOfRangeException();
            }
            set => throw new NotSupportedException();
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
            var idx = arrayIndex;
            foreach (var l in m_lists)
            {
                l.CopyTo(array, idx);
                idx += Count;
            }
        }

        public int Count => m_lists.Sum(l => l.Count);

        public bool IsReadOnly => true;

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
            private readonly List<IObservableList<T>> m_lists = new List<IObservableList<T>>();
            private IEnumerator<IEnumerator<T>> m_active;

            public OverlayListEnumerator(params IObservableList<T>[] lists)
            {
                m_lists.AddRange(lists);
                Reset();
            }

            public T Current => m_active.Current.Current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (m_active.Current?.MoveNext() ?? false) return true;
                return m_active.MoveNext() && (m_active.Current?.MoveNext() ?? false);
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

            private bool m_disposedValue; // To detect redundant calls

            protected void Dispose(bool disposing)
            {
                if (m_disposedValue) return;
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                m_disposedValue = true;
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
