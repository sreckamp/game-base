using System;
using System.Collections.Generic;
using GameBase.Model.Rules;
using System.Drawing;
using System.Linq;

namespace GameBase.Model
{
    public abstract class GameBoard<TP, TM> : IGameBoard<TP, TM> where TP:Piece where TM:Move
    {
        private readonly IPlaceRule<TP,TM> m_placeRule;
        private readonly Point m_minCorner;
        private readonly Point m_maxCorner;

        protected GameBoard(IPlaceRule<TP, TM> placeRule):this(placeRule,
            new Point(int.MinValue, int.MinValue),
            new Point(int.MaxValue, int.MaxValue)) { }

        protected GameBoard(IPlaceRule<TP, TM> placeRule, int width, int height):
            this(placeRule,
            new Point(0, 0),
            new Point(width -1, height-1))
        { }

        private GameBoard(IPlaceRule<TP, TM> placeRule, Point minCorner, Point maxCorner)
        {
            MinXChanged += (sender, args) => { };
            MinYChanged += (sender, args) => { };
            MaxXChanged += (sender, args) => { };
            MaxYChanged += (sender, args) => { };
            m_placeRule = placeRule;
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
            AvailableLocations = new ObservableList<Point>();
            Placements = new ObservableList<Placement<TP, TM>>();
        }

        public ObservableList<Point> AvailableLocations { get; }
        public ObservableList<Placement<TP,TM>> Placements { get; }

        public IEnumerable<TM> GetAvailableMoves(TP piece)
        {
            return piece == GetEmptyPiece()
                ? Enumerable.Empty<TM>()
                : AvailableLocations.SelectMany(GetOptions)
                    .Where(m => m_placeRule.Applies(this, piece, m) && m_placeRule.Fits(this, piece, m));
        }

        protected virtual IEnumerable<TM> GetOptions(Point point)
        {
            return Enumerable.Empty<TM>();
        }

        public event EventHandler<ChangedValueArgs<int>> MinXChanged;
        private int m_minX;
        public int MinX
        {
            get => m_minX;
            private set
            {
                var old = m_minX;
                m_minX = value;
                MinXChanged.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> MaxXChanged;
        private int m_maxX;
        public int MaxX
        {
            get => m_maxX;
            private set
            {
                var old = m_maxX;
                m_maxX = value;
                MaxXChanged.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> MinYChanged;
        private int m_minY;
        public int MinY
        {
            get => m_minY;
            private set
            {
                var old = m_minY;
                m_minY = value;
                MinYChanged.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> MaxYChanged;
        private int m_maxY;
        public int MaxY
        {
            get => m_maxY;
            private set
            {
                var old = m_maxY;
                m_maxY = value;
                MaxYChanged.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        protected TP this[int col, int row] => this[new Point(col, row)];
 
        public TP this[Point pnt]
        {
            get
            {
                foreach (var p in Placements)
                {
                    if (p.Move.IsEmpty) continue;
                    var cmpX = p.Move.Location.X.CompareTo(pnt.X);
                    var cmpY = p.Move.Location.Y.CompareTo(pnt.Y);
                    if (cmpX == 0 && cmpY == 0)
                    {
                        return p.Piece;
                    }
                    if (cmpX > 0 || (cmpX == 0 && cmpY > 0))
                    {
                        break;
                    }
                }
                return GetEmptyPiece();
            }
        }

        protected abstract TP GetEmptyPiece();

        /// <summary>
        /// Add available locations based on the given placement
        /// </summary>
        /// <param name="placement">The newly added placement</param>
        protected virtual void AddAvailableLocations(Placement<TP, TM> placement)
        {

        }

        public bool Add(Placement<TP, TM> placement)
        {
            return Add(placement, true);
        }

        private bool Add(Placement<TP,TM> placement, bool mustBeAvailable)
        {
            if (placement.Move.Location.X < m_minCorner.X
                || placement.Move.Location.Y < m_minCorner.Y
                || placement.Move.Location.X > m_maxCorner.X
                || placement.Move.Location.Y > m_maxCorner.Y)
            {
                throw new IndexOutOfRangeException();
            }

            if (!AvailableLocations.Contains(placement.Move))
            {
                if (mustBeAvailable)
                {
                    return false;
                }
            }
            else
            {
                AvailableLocations.Remove(placement.Move);
            }
            var col = placement.Move.Location.X;
            var row = placement.Move.Location.Y;
            AddAvailableLocations(placement);
            var i = 0;
            for (; i < Placements.Count; i++)
            {
                var comp = placement.CompareTo(Placements[i]);
                if (comp == 0)
                {
                    Placements.RemoveAt(i);
                    Placements.Insert(i, placement);
                    break;
                }
                else if (comp < 0)
                {
                    Placements.Insert(i, placement);
                    break;
                }
            }
            if (i == Placements.Count)
            {
                Placements.Add(placement);
            }
            if (col < MinX)
            {
                MinX = col;
            }
            if (col > MaxX)
            {
                MaxX = col;
            }
            if (row < MinY)
            {
                MinY = row;
            }
            if (row > MaxY)
            {
                MaxY = row;
            }
            return true;
        }

        public bool IsEmpty => Placements.Count == 0;

        public virtual void Clear()
        {
            MaxX = MinX = MaxY = MinY = 0;
            AvailableLocations.Clear();
            Placements.Clear();
        }
    }
}
