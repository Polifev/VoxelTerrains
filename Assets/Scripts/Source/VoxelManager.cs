using UnityEngine;

namespace VoxelTerrains
{
    [ExecuteInEditMode]
    public class VoxelManager: MonoBehaviour
    {
        private static VoxelManager _instance;
        public static VoxelManager Instance => _instance;

        private CubeConfiguration[] _configurations;

        [SerializeField]
        private string dataFilePath = "";

        public CubeConfiguration Configuration(int index)
        {
            return _configurations[index];
        }

        private void OnValidate()
        {
            Debug.Log("Editor causes this Awake");
            if (_instance == null)
            {
                CsvCubeConfigurationsLoader loader = new CsvCubeConfigurationsLoader();
                _configurations = loader.LoadFromFile(dataFilePath);
                _instance = this;
            }
        }
    }
}
