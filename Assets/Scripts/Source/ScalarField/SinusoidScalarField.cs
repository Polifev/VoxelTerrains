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
        public override event TerrainChangedEventHandler OnTerrainChanged;

        public override float ValueAt(Vector3 vector)
        {
            float height = Mathf.Sin(vector.x / 16) * Mathf.Cos(vector.z / 16) * 16;

            return (vector.y > height) ? -1f : 1f;
        }
    }
}
