using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using GameBase.Model.Rules;
using System.Drawing;

namespace GameBase.Model
{
    public interface IGameBoard<P,M> where P : Piece where M : Move
    {
        P this[Point location] { get; }
        bool IsEmpty { get; }
        ObservableList<Point> AvailableLocations { get; }
    }
}
