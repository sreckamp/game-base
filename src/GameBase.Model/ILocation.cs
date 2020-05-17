using System;
using System.Drawing;

namespace GameBase.Model
{
    public interface IMove : IEquatable<IMove>
    {
        Point Location { get; }
    }
}