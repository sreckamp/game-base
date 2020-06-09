using System;
using System.ComponentModel;
using System.Windows;
using GameBase.Model;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF.ViewModel
{
    public abstract class PlacementViewModel<T> : INotifyPropertyChanged
    {
        protected readonly Placement<T> Placement;
        protected readonly IGridManager GridManager;

        protected PlacementViewModel(Placement<T> placement)
            : this(placement, NopGridManager.Instance)
        {
        }

        protected PlacementViewModel(Placement<T> placement, IGridManager gridManager)
        {
            PropertyChanged += (sender, args) => { };
            Placement = placement;
            GridManager = gridManager;
            GridManager.StartColumnChanged += (sender, args) => NotifyPropertyChanged(nameof(Column));
            GridManager.StartColumnChanged += (sender, args) => NotifyPropertyChanged(nameof(IsOnGrid));
            GridManager.StartRowChanged += (sender, args) => NotifyPropertyChanged(nameof(Row));
            GridManager.StartRowChanged += (sender, args) => NotifyPropertyChanged(nameof(IsOnGrid));
            GridManager.ColumnsChanged += (sender, args) => NotifyPropertyChanged(nameof(IsOnGrid));
            GridManager.RowsChanged += (sender, args) => NotifyPropertyChanged(nameof(IsOnGrid));
        }

        public DPoint Location => Placement.Location;
        public T Piece => Placement.Piece;

        public virtual void SetCell(DPoint cell)
        {
            Placement.Location = new DPoint(cell.X + GridManager.StartColumn, cell.Y + GridManager.StartRow);
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        public bool IsOnGrid => RawColumn >= 0 && RawColumn < GridManager.Columns
                                              && RawRow >= 0 && RawRow < GridManager.Rows;

        private int RawColumn => Location.X - GridManager.StartColumn;
        public int Column => Math.Max(0, RawColumn);
        private int RawRow => Location.Y - GridManager.StartRow;
        public int Row => Math.Max(0, RawRow);

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            // if (Application.Current.Dispatcher.CheckAccess())
            // {
                // Debug.WriteLine($"PlacementViewModel<{typeof(T).Name}>.NotifyPropertyChanged({name})");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            // }
            // else
            // {
            //     // Debug.WriteLine($"Invoke PlacementViewModel<{typeof(T).Name}>.NotifyPropertyChanged({name})");
            //     Application.Current.Dispatcher.Invoke(new Action<string>(NotifyPropertyChanged), name);
            // }
        }

        #endregion

        private class NopGridManager : IGridManager
        {
            public static readonly IGridManager Instance = new NopGridManager();

            private NopGridManager()
            {
                StartColumnChanged += (sender, args) => { };
                StartRowChanged += (sender, args) => { };
                ColumnsChanged += (sender, args) => { };
                RowsChanged += (sender, args) => { };
            }

            public event EventHandler<ChangedValueArgs<int>> StartColumnChanged;
            public int StartColumn => 0;
            public event EventHandler<ChangedValueArgs<int>> StartRowChanged;
            public int StartRow => 0;
            public event EventHandler<ChangedValueArgs<int>> ColumnsChanged;
            public int Columns => 0;
            public event EventHandler<ChangedValueArgs<int>> RowsChanged;
            public int Rows => 0;
        }
    }

    public interface IGridManager
    {
        event EventHandler<ChangedValueArgs<int>> StartColumnChanged;
        int StartColumn { get; }
        event EventHandler<ChangedValueArgs<int>> ColumnsChanged;
        int Columns { get; }
        event EventHandler<ChangedValueArgs<int>> StartRowChanged;
        int StartRow { get; }
        event EventHandler<ChangedValueArgs<int>> RowsChanged;
        int Rows { get; }
    }
}