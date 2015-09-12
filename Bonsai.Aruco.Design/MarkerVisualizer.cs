using Aruco.Net;
using Bonsai;
using Bonsai.Aruco.Design;
using Bonsai.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: TypeVisualizer(typeof(MarkerVisualizer), Target = typeof(Marker))]

namespace Bonsai.Aruco.Design
{
    public class MarkerVisualizer : ObjectTextVisualizer
    {
    }
}
