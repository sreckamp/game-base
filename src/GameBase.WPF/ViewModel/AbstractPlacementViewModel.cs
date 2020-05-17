using System;
using System.ComponentModel;
using System.Windows;
using GameBase.Model;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF.ViewModel
{
    public abstract class AbstractPlacementViewModel<TP,TM> : INotifyPropertyChanged where TP:IPiece where TM:Move
    {
        protected readonly Placement<TP, TM> Placement;
        protected readonly IGridManager GridManager;

        protected AbstractPlacementViewModel(Placement<TP, TM> placement, IGridManager gridManager = null)
        {
            PropertyChanged += (sender, args) => { };
            Placement = placement;
            GridManager = gridManager ?? DefaultGridManager.Instance;
            GridManager.StartColumnChanged += OnStartColumnChanged;
            GridManager.StartRowChanged += OnStartRowChanged;
            m_move = GetEmptyMove();
        }

        private TM m_move;
        public TM Move => Placement.Move;

        protected abstract TM GetMove(int locationX, int locationY);
        protected abstract TM GetEmptyMove();

        public virtual void SetCell(DPoint cell)
        {
            m_move = GetMove(cell.X + GridManager.StartColumn, cell.Y + GridManager.StartRow);
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        private void OnStartRowChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        private void OnStartColumnChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        public bool IsOnGrid => !Move.IsEmpty
                                && Column >= 0 && Column < GridManager.Columns
                                && Row >= 0 && Row < GridManager.Rows;
        public int Column => Move.Location.X - GridManager.StartColumn;
        public int Row => Move.Location.Y - GridManager.StartRow;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action<string>(NotifyPropertyChanged), name);
            }
        }

        #endregion

        private class DefaultGridManager : IGridManager
        {
            public static readonly IGridManager Instance = new DefaultGridManager();

            private DefaultGridManager()
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