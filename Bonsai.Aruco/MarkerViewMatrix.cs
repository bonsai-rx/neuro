using Aruco.Net;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Aruco
{
    [Description("Returns the modelview matrix given the extrinsic parameters of the specified input marker.")]
    public class MarkerViewMatrix : Transform<Marker, Matrix4>
    {
        public override IObservable<Matrix4> Process(IObservable<Marker> source)
        {
            return source.Select(input =>
            {
                if (input.IsValid)
                {
                    var extrinsics = input.GetGLModelViewMatrix();
                    var modelView = new Matrix4(
                        (float)extrinsics[0], (float)extrinsics[1], (float)extrinsics[2], (float)extrinsics[3],
                        (float)extrinsics[4], (float)extrinsics[5], (float)extrinsics[6], (float)extrinsics[7],
                        (float)extrinsics[8], (float)extrinsics[9], (float)extrinsics[10], (float)extrinsics[11],
                        (float)extrinsics[12], (float)extrinsics[13], (float)extrinsics[14], (float)extrinsics[15]);
                    return modelView * Matrix4.CreateScale(1, -1, 1);
                }
                return default(Matrix4);
            });
        }
    }
}
