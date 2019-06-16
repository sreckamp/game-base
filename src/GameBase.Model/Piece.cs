using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Drawing;

namespace GameBase.Model
{
    public class Piece
    {
        public virtual Piece Clone()
        {
            var p = new Piece();
            return p;
        }

    }
}