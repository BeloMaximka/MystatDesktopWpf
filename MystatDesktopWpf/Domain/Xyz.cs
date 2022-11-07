using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal struct Xyz
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Xyz(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
