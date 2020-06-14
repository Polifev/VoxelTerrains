using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoxelTerrains.Noise;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.ScalarField
{
    [ExecuteInEditMode]
    public class NoiseScalarField : AbstractScalarField
    {
        [SerializeField]
        private float _highestValueAboveSeaLevel = 64.0f;
        [SerializeField]
        private float _deepestValueBelowSeaLevel = 64.0f;


        public override float ValueAt(Vector3 vector)
        {
            float height = NoiseHandler.Noise(
                vector.x,
                vector.z,
                6,
                2,
                0.5f,
                0.025f,
                0,
                2,
                0,
                NoiseHandler.NoiseType.OpenSimplexNoise,
                NoiseHandler.NoiseAdditionType.Turbulence);

            if(height > 0.0f)
            {
                height *= _highestValueAboveSeaLevel;
            }
            else
            {
                height *= _deepestValueBelowSeaLevel;
            }

            return (vector.y > height) ? -1.0f : 1.0f;
        }
    }
}
