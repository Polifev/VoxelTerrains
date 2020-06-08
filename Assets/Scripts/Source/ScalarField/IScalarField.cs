using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public interface IScalarField
    {
        float ValueAt(Vector3 vector);
    }
}
