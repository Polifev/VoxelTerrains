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

        private IDictionary<Vector2, float> _heightAtXZ = new Dictionary<Vector2, float>();

        public override event TerrainChangedEventHandler OnTerrainChanged;

        public override float ValueAt(Vector3 vector)
        {
            var y = vector.y;
            var xz = new Vector2(vector.x, vector.z);

            if (!_heightAtXZ.ContainsKey(xz))
            {
                _heightAtXZ[xz] = NoiseHandler.Noise(
                xz.x,
                xz.y,
                6,
                2,
                0.5f,
                0.01f,
                0,
                2,
                0,
                NoiseHandler.NoiseType.OpenSimplexNoise,
                NoiseHandler.NoiseAdditionType.FBM);
            }
            float height = _heightAtXZ[xz];

            if (height > 0.0f)
            {
                height *= _highestValueAboveSeaLevel;
            }
            else
            {
                height *= _deepestValueBelowSeaLevel;
            }

            return (y > height) ? -1.0f : 1.0f;
        }
    }
}
