using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public class SimplexTerrainScalarField : AbstractScalarField
    {
        public override float ValueAt(Vector3 vector)
        {
            return -1.0f;
        }
    }
}
