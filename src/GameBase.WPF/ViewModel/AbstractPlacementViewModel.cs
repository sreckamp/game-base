using GameBase.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DPoint = System.Drawing.Point;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace GameBase.WPF.ViewModel
{
    public abstract class AbstractPlacementViewModel<P,M> : INotifyPropertyChanged where P:Piece where M:Move
    {
        protected readonly Placement<P, M> m_placement;
        protected readonly IGridManager m_GridManager;
        protected AbstractPlacementViewModel(Placement<P,M> placement, IGridManager gridManager = null)
        {
            m_placement = placement;
            m_GridManager = gridManager;
            if(m_GridManager != null)
            {
                m_GridManager.StartColumnChanged += startColumnChanged;
                m_GridManager.StartRowChanged += startRowChanged;
            }
        }

        private M m_move = null;
        public M Move
        {
            get => m_placement.Move ?? m_move;
        }

        protected abstract M GetMove(int locationX, int locationY);

        public virtual void SetCell(DPoint cell)
        {
            m_move = GetMove(cell.X + (m_GridManager?.StartColumn ?? 0), cell.Y + (m_GridManager?.StartRow ?? 0));
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }
        private void startRowChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Row));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        private void startColumnChanged(object sender, ChangedValueArgs<int> e)
        {
            NotifyPropertyChanged(nameof(Column));
            NotifyPropertyChanged(nameof(IsOnGrid));
        }

        public bool IsOnGrid => Move != null;
        public int Column => Move != null ? (Move.Location.X - (m_GridManager?.StartColumn ?? 0)) : 0;
        public int Row => Move != null ? (Move.Location.Y - (m_GridManager?.StartRow ?? 0)) : 0;

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
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<string>((n) =>
                { NotifyPropertyChanged(n); }), name);
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