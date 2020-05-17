using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameBase.Model.Rules;

namespace GameBase.Model
{
    public abstract class GameBoard<T> : IGameBoard<T>
    {
        // private readonly IPlaceRule<T> m_placeRule;
        private readonly Point m_minCorner;
        private readonly Point m_maxCorner;
        private readonly T m_emptyPiece;

        // ReSharper disable once UnusedMember.Global
        protected GameBoard(/*IPlaceRule<T> placeRule, */T emptyPiece, int width = 0, int height = 0):
            this(//placeRule,
                emptyPiece,
                new Point(width == 0 ? int.MinValue : 0, height == 0 ? int.MinValue : 0),
                new Point(width == 0 ? int.MaxValue : width -1, height == 0 ? int.MaxValue : height-1))
        { }

        private GameBoard(/*IPlaceRule<T> placeRule, */T emptyPiece, Point minCorner, Point maxCorner)
        {
            MinXChanged += (sender, args) => { };
            MinYChanged += (sender, args) => { };
            MaxXChanged += (sender, args) => { };
            MaxYChanged += (sender, args) => { };
            m_emptyPiece = emptyPiece;
            // m_placeRule = placeRule;
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
            // AvailableLocations = new ObservableList<Point>();
            Placements = new ObservableList<Placement<T>>();
        }

        // public ObservableList<Point> AvailableLocations { get; }
        public ObservableList<Placement<T>> Placements { get; }

        // public IEnumerable<Point> GetAvailableMoves(T piece)
        // {
        //     return piece.Equals(m_emptyPiece)
        //         ? Enumerable.Empty<Point>()
        //         : AvailableLocations.SelectMany(GetOpenLocations)
        //             .Where(m => m_placeRule.Applies(this, piece, m) && m_placeRule.Fits(this, piece, m));
        // }

        protected virtual IEnumerable<Point> GetOpenLocations(Point point)
        {
            return Enumerable.Empty<Point>();
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

        protected T this[int col, int row] => this[new Point(col, row)];

        public T this[Point pnt]
        {
            get
            {
                foreach (var p in Placements)
                {
                    var cmpX = p.Location.X.CompareTo(pnt.X);
                    var cmpY = p.Location.Y.CompareTo(pnt.Y);
                    if (cmpX == 0 && cmpY == 0)
                    {
                        return p.Piece;
                    }
                    if (cmpX > 0 || cmpX == 0 && cmpY > 0)
                    {
                        break;
                    }
                }
                return m_emptyPiece;
            }
        }

        // /// <summary>
        // /// Add available locations based on the given placement
        // /// </summary>
        // /// <param name="placement">The newly added placement</param>
        // protected virtual void AddAvailableLocations(Placement<T> placement)
        // {
        //
        // }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool Add(Placement<T> placement)
        {
            return Add(placement, true);
        }

        private bool Add(Placement<T> placement, bool mustBeAvailable)
        {
            if (placement.Location.X < m_minCorner.X
                || placement.Location.Y < m_minCorner.Y
                || placement.Location.X > m_maxCorner.X
                || placement.Location.Y > m_maxCorner.Y)
            {
                throw new IndexOutOfRangeException();
            }

            
            // if (!AvailableLocations.Contains(placement.Location))
            // {
            //     if (mustBeAvailable)
            //     {
            //         return false;
            //     }
            // }
            // else
            // {
            //     AvailableLocations.Remove(placement.Location);
            // }
            var col = placement.Location.X;
            var row = placement.Location.Y;
            // AddAvailableLocations(placement);
            var i = 0;
            for (; i < Placements.Count; i++)
            {
                var comp = placement.CompareTo(Placements[i]);
                if (comp == 0)
                {
                    if (mustBeAvailable) return false;
                    Placements.RemoveAt(i);
                    Placements.Insert(i, placement);
                    break;
                }

                if (comp >= 0) continue;
                Placements.Insert(i, placement);
                break;
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
            // AvailableLocations.Clear();
            Placements.Clear();
        }
    }
}
