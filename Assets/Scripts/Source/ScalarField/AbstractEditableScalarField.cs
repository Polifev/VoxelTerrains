using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public abstract class AbstractEditableScalarField : AbstractScalarField
    {
        public abstract void AddValueAt(Vector3 location, float value);
    }
}
