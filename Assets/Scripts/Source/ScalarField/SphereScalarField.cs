using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.ScalarField
{
    public class SphereScalarField: AbstractScalarField
    {
        [SerializeField]
        private Vector3 _center = Vector3.zero;

        [SerializeField]
        private float _radius = 1.0f;

        public override event TerrainChangedEventHandler OnTerrainChanged;

        public override float ValueAt(Vector3 point)
        {
            return (point - _center).sqrMagnitude >= _radius * _radius ? -1.0f : 1.0f;
        }
    }
}
