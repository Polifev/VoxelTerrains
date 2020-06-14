using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public class SinusoidScalarField : AbstractScalarField
    {
        public override float ValueAt(Vector3 vector)
        {
            float height = Mathf.Sin( 2 * vector.x) * Mathf.Cos(2* vector.z);

            return (vector.y > height) ? -1f : 1f;
        }
    }
}
