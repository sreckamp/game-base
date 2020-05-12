using GameBase.Model;
using System;
using System.ComponentModel;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF.ViewModel
{
    public abstract class AbstractPlacementViewModel<TP,TM> : INotifyPropertyChanged where TP:Piece where TM:Move
    {
        protected readonly Placement<TP, TM> Placement;
        protected readonly IGridManager GridManager;
        protected AbstractPlacementViewModel(Placement<TP,TM> placement, IGridManager gridManager = null)
        {
            Placement = placement;
            GridManager = gridManager;
            if(GridManager != null)
            {
                GridManager.StartColumnChanged += StartColumnChanged;
                GridManager.StartRowChanged += StartRowChanged;
            }
        }

        private TM m_move;
        public TM Move => Placement.Move ?? m_move;

        protected abstract TM GetMove(int locationX, int locationY);

        public virtual void SetCell(DPoint cell)
        {
            m_move = GetMove(cell.X + (GridManager?.StartColumn ?? 0), cell.Y + (GridManager?.StartRow ?? 0));
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }
        private void StartRowChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        private void StartColumnChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        public bool IsOnGrid => Move != null;
        public int Column => Move != null ? (Move.Location.X - (GridManager?.StartColumn ?? 0)) : 0;
        public int Row => Move != null ? (Move.Location.Y - (GridManager?.StartRow ?? 0)) : 0;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<string>(NotifyPropertyChanged), name);
            }
        }

        #endregion
    }

    public interface IGridManager
    {
        event EventHandler<ChangedValueArgs<int>> StartColumnChanged;
        int StartColumn { get; }
        event EventHandler<ChangedValueArgs<int>> StartRowChanged;
        int StartRow { get; }
    }
}