using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;
using System.Windows;
using GameBase.Model;
using System.Threading;

namespace GameBoard.Model
{
    /// <summary>
    /// Make a collection that mirrors a base collection.  The target class
    /// should take the base collection as a constructor argument.
    /// </summary>
    /// <typeparam name="T">The target Type, needs to have a constructor that takes
    /// the base as an argument</typeparam>
    /// <typeparam name="B">The base type that the source collection is based on.</typeparam>
    public class DispatchedMappingCollection<T, B> : MappingCollection<T, B>
    {
        public DispatchedMappingCollection(IObservableList<B> models, params object[] constructorParams)
            :base(models, constructorParams)
        {
        }

        protected override void Add(B m)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                base.Add(m);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<B>((_m) => { Add(_m); }), m);
            }
        }

        protected override void Remove(B m, int idx)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                base.Remove(m, idx);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<B, int>((_m, _i) => { Remove(_m, _i); }), m, idx);
            }
        }

        #region INotifyCollectionChanged Members

        protected override void NotifyCollectionChanged(NotifyCollectionChangedAction action, T vm = default(T), int idx = -1)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                base.NotifyCollectionChanged(action, vm, idx);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<NotifyCollectionChangedAction, T, int>((_a, _vm, _idx) =>
                { NotifyCollectionChanged(_a, _vm, _idx); }), action, vm, idx);
            }
        }

        #endregion
    }
}
