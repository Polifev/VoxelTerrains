using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public abstract class AbstractScalarField : MonoBehaviour
    {
        public abstract float ValueAt(Vector3 vector);
    }
}
