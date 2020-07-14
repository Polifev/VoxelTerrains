using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public class HorizontalScalarField : AbstractScalarField
    {
        [SerializeField]
        private float _height = 0.0f;

        public override event TerrainChangedEventHandler OnTerrainChanged;

        public override float ValueAt(Vector3 point)
        {
            var result = 1.0f;
            if(point.y > _height)
            {
                result =  -1.0f;
            }
            return result;
        }
    }
}
