using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoxelTerrains.Noise;

namespace VoxelTerrains.ScalarField
{
    [ExecuteInEditMode]
    public class SimplexTerrainScalarField : AbstractScalarField
    {
        [SerializeField]
        private long _seed = 0;
        [SerializeField]
        private float _scale = 1.0f;

        private OpenSimplexNoise _noise;

        private void OnValidate()
        {
            _noise = new OpenSimplexNoise(_seed);
        }

        public override float ValueAt(Vector3 vector)
        {
            Vector3 temp = vector / _scale;
            return (float)_noise.eval(temp.x, temp.y, temp.z);
        }
    }
}
