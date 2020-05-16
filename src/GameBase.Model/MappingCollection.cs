using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GameBase.Model
{
    /// <summary>
    /// Make a collection that mirrors a base collection.  The target class
    /// should take the base collection as a constructor argument.
    /// </summary>
    /// <typeparam name="T">The target Type, needs to have a constructor that takes
    /// the base as an argument</typeparam>
    /// <typeparam name="TB">The base type that the source collection is based on.</typeparam>
    public class MappingCollection<T, TB> : IObservableList<T> where T : notnull where TB : notnull
    {
        private readonly IObservableList<TB> m_models;
        private readonly Dictionary<TB, T> m_modelToViewModel = new Dictionary<TB, T>();
        private readonly Dictionary<T, TB> m_viewModelToModel = new Dictionary<T, TB>();
        private readonly ConstructorInfo m_constructor;
        private readonly object[] m_constructorParams;

        public MappingCollection(IObservableList<TB> models, params object[] constructorParams)
        {
            CollectionChanged += (sender, args) => { };
            m_constructorParams = new object[constructorParams.Length + 1];
            var types = new Type[m_constructorParams.Length];
            types[0] = typeof(TB);
            for (var i = 0; i < constructorParams.Length; i++)
            {
                m_constructorParams[i + 1] = constructorParams[i];
                types[i + 1] = constructorParams[i].GetType();
            }

            m_constructor = typeof(T).GetConstructor(types) ??
                            throw new ArgumentException(typeof(T).Name + " cannot find constructor.");
            m_models = models;
            m_models.CollectionChanged += models_CollectionChanged;
            foreach (var m in models)
            {
                CoreAdd(m);
            }
        }

        private void models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.WriteLine($"Add:{e.NewStartingIndex}");
                    foreach (var t in e.NewItems)
                    {
                        Add((TB) t);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        Remove((TB) e.OldItems[i], e.OldStartingIndex + i);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        Remove((TB) e.OldItems[i], e.OldStartingIndex + i);
                    }

                    Debug.WriteLine($"ReplaceAdd:{e.NewStartingIndex}");
                    foreach (var t in e.NewItems)
                    {
                        Add((TB) t);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_modelToViewModel.Clear();
                    m_viewModelToModel.Clear();
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void Add(TB m)
        {
            CoreAdd(m);
        }

        private void CoreAdd(TB m)
        {
            if (m_modelToViewModel.ContainsKey(m)) return;
            Debug.WriteLine($"Add:{m}");
            m_constructorParams[0] = m;
            var vm = (T) m_constructor.Invoke(m_constructorParams);
            m_modelToViewModel[m] = vm;
            m_viewModelToModel[vm] = m;
            NotifyCollectionChanged(NotifyCollectionChangedAction.Add, vm, m_models.IndexOf(m));
        }

        protected virtual void Remove(TB m, int idx)
        {
            if (!m_modelToViewModel.ContainsKey(m)) return;
            Debug.WriteLine($"Remove:{m},{idx}");
            var vm = m_modelToViewModel[m];
            m_modelToViewModel.Remove(m);
            m_viewModelToModel.Remove(vm);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, vm, idx);
        }

        public T this[TB item]
        {
            get
            {
                var val = default(T);
                if (item != null)
                {
                    val = m_modelToViewModel[item];
                }

                return val;
            }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void NotifyCollectionChanged(NotifyCollectionChangedAction action, T vm = default,
            int idx = -1)
        {
            var args = new NotifyCollectionChangedEventArgs(action, vm, idx);
            CollectionChanged.Invoke(this, args);
        }

        #endregion

        #region IEnumerable<VM> Members

        public IEnumerator<T> GetEnumerator()
        {
            var viewModels = m_models.Where(m => m_modelToViewModel.ContainsKey(m)).Select(m => m_modelToViewModel[m]);

            return viewModels.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            var viewModels = m_models.Where(m => m_modelToViewModel.ContainsKey(m)).Select(m => m_modelToViewModel[m]);

            return viewModels.GetEnumerator();
        }

        #endregion

        #region IList<VM> Members

        public int IndexOf(T item)
        {
            return item != null ? m_models.IndexOf(m_viewModelToModel[item]) : -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get => m_modelToViewModel[m_models[index]];
            set => throw new NotImplementedException();
        }

        #endregion

        #region ICollection<VM> Members

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return m_models.Contains(m_viewModelToModel[item]);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (var i = 0; i < m_models.Count; i++)
            {
                array[arrayIndex + i] = m_modelToViewModel[m_models[i]];
            }
        }

        public int Count => m_models.Count;

        public bool IsReadOnly => true;

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
