﻿// using System;
// // using System.Collections.Specialized;
// using System.Windows;
// using GameBase.Model;
//
// namespace GameBase.WPF.ViewModel
// {
//     /// <summary>
//     /// Make a collection that mirrors a base collection.  The target class
//     /// should take the base collection as a constructor argument.
//     /// </summary>
//     /// <typeparam name="T">The target Type, needs to have a constructor that takes
//     /// the base as an argument</typeparam>
//     /// <typeparam name="TB">The base type that the source collection is based on.</typeparam>
//     public class DispatchedMappingCollection<T, TB> : MappingCollection<T, TB> where T:notnull where TB:notnull
//     {
//         public DispatchedMappingCollection(IObservableList<TB> models, params object[] constructorParams)
//             :base(models, constructorParams)
//         {
//         }
//
//         protected override void Add(TB b)
//         {
//             if (Application.Current.Dispatcher?.CheckAccess() ?? true)
//             {
//                 base.Add(b);
//             }
//             else
//             {
//                 Application.Current.Dispatcher.Invoke(new Action<TB>(Add), b);
//             }
//         }
//
//         protected override void Remove(TB b, int idx)
//         {
//             if (Application.Current.Dispatcher?.CheckAccess() ?? true)
//             {
//                 base.Remove(b, idx);
//             }
//             else
//             {
//                 Application.Current.Dispatcher.Invoke(new Action<TB, int>(Remove), b, idx);
//             }
//         }
//
//         #region INotifyCollectionChanged Members
//
//         // protected override void NotifyCollectionChanged(NotifyCollectionChangedAction action, T vm = default, int idx = -1)
//         // {
//         //     if (Application.Current.Dispatcher?.CheckAccess() ?? true)
//         //     {
//         //         // Debug.WriteLine($"DispatchedMappingCollection.NotifyCollectionChanged {typeof(T).Name}=>{typeof(TB).Name}");
//         //         base.NotifyCollectionChanged(action, vm, idx);
//         //     }
//         //     else
//         //     {
//         //         // Debug.WriteLine($"Invoke: DispatchedMappingCollection.NotifyCollectionChanged {typeof(T).Name}=>{typeof(TB).Name}");
//         //         Application.Current.Dispatcher.Invoke(new Action<NotifyCollectionChangedAction, T, int>(NotifyCollectionChanged), action, vm, idx);
//         //     }
//         // }
//
//         #endregion
//     }
// }
