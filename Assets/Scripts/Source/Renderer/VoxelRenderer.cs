using UnityEditor;
using UnityEngine;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.Renderer
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public abstract class VoxelRenderer : MonoBehaviour
    {
        [SerializeField]
        private bool _debugMode = false;
        [SerializeField]
        private bool _realTime = false;
        [SerializeField, Range(0.001f, 10.0f)]
        private float _tileSize = 1.0f;
        [SerializeField]
        private Vector3 _size = Vector3.one;
        [SerializeField]
        private AbstractScalarField _scalarField = null;

        public AbstractScalarField ScalarField
        {
            get => _scalarField;
            set => _scalarField = value;
        }

        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        public float TileSize
        {
            get => _tileSize;
            set => _tileSize = value;
        }

        protected MeshFilter MeshFilter { get; private set; } = null;
        protected MeshCollider MeshCollider { get; private set; } = null;
        protected bool DebugMode => _debugMode;
        

        private void Update()
        {
            if (_realTime)
            {
                RefreshMesh();
            }
        }

        public virtual void RefreshMesh()
        {
            MeshFilter = GetComponent<MeshFilter>();
            MeshCollider = GetComponent<MeshCollider>();
        }

        public bool isEmpty()
        {
            return MeshFilter.mesh.vertices.Length == 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, _size);

            if (DebugMode)
            {
                Handles.color = Color.red;
                for(float x = -_size.x / 2; x < _size.x/2+1; x++)
                    for (float y = -_size.y / 2; y < _size.y/2+1; y++)
                        for (float z = -_size.z / 2; z < _size.z/2+1; z++)
                        {
                            var style = GUIStyle.none;
                            style.richText = true;
                            var v = new Vector3(x, y, z) + transform.position;
                            //Handles.Label(v, $"<color=red>{}</color>", style);
                            if(_scalarField.ValueAt(v) > 0)
                            {
                                Gizmos.color = Color.blue;
                            }
                            else
                            {
                                Gizmos.color = Color.red;
                            }
                            
                            Gizmos.DrawSphere(v, 0.1f);
                        }
            }
        }
    }
}
