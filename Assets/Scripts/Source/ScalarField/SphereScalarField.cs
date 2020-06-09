using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.ScalarField
{
    public class SphereScalarField: IScalarField
    {
        private float _squaredRadius;
        public Vector3 Center { get; private set; }
        public float Radius { get; private set; }

        public SphereScalarField(float radius) : this(radius, Vector3.zero) {}

        public SphereScalarField(float radius, Vector3 center)
        {
            Radius = radius;
            _squaredRadius = radius * radius;
            Center = center;
        }

        public float ValueAt(Vector3 point)
        {
            return Vector3.Distance(point, Center) >= Radius ? -1.0f : 1.0f;
        }
    }
}
