using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Aruco
{
    public struct MarkerTransform
    {
        public double M00;
        public double M01;
        public double M02;
        public double M03;
        public double M10;
        public double M11;
        public double M12;
        public double M13;
        public double M20;
        public double M21;
        public double M22;
        public double M23;
        public double M30;
        public double M31;
        public double M32;
        public double M33;

        public override string ToString()
        {
            return string.Format("{{{{{0}, {1}, {2}, {3}}}, {{{4}, {5}, {6}, {7}}}, {{{8}, {9}, {10}, {11}}}, {{{12}, {13}, {14}, {15}}}}}",
                M00,
                M01,
                M02,
                M03,
                M10,
                M11,
                M12,
                M13,
                M20,
                M21,
                M22,
                M23,
                M30,
                M31,
                M32,
                M33);
        }
    }
}
