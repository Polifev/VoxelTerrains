using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public class HorizontalScalarField : IScalarField
    {
        public float Height { get; private set; }

        public HorizontalScalarField(float height)
        {
            Height = height;
        }

        public float ValueAt(Vector3 point)
        {
            var result = 1.0f;
            if(point.y > Height)
            {
                result =  -1.0f;
            }
            return result;
        }
    }
}
