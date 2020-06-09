using System;
using System.Drawing;

namespace GameBase.Model
{
    public abstract class Board<T> : IGameBoard<T>
    {
        private readonly Point m_minCorner;
        private readonly Point m_maxCorner;
        private readonly T m_emptyPiece;

        // ReSharper disable once UnusedMember.Global
        protected Board(T emptyPiece, int width = 0, int height = 0):
            this(
                emptyPiece,
                new Point(width == 0 ? int.MinValue : 0, height == 0 ? int.MinValue : 0),
                new Point(width == 0 ? int.MaxValue : width -1, height == 0 ? int.MaxValue : height-1))
        { }

        private Board(T emptyPiece, Point minCorner, Point maxCorner)
        {
            MinXChanged += (sender, args) => { };
            MinYChanged += (sender, args) => { };
            MaxXChanged += (sender, args) => { };
            MaxYChanged += (sender, args) => { };
            m_emptyPiece = emptyPiece;
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
            Placements = new ObservableList<Placement<T>>();
        }

        public ObservableList<Placement<T>> Placements { get; }

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

        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool Add(Placement<T> placement)
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
                if (comp == 0) return false;
                // {
                //     if (mustBeAvailable) return false;
                //     Placements.RemoveAt(i);
                //     Placements.Insert(i, placement);
                //     break;
                // }

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
            Placements.Clear();
        }
    }
}
