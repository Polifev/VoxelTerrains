using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public delegate void TerrainChangedEventHandler(Vector3 location);

    public abstract class AbstractScalarField : MonoBehaviour
    {
        public abstract event TerrainChangedEventHandler OnTerrainChanged;
        public abstract float ValueAt(Vector3 vector);
    }
}
