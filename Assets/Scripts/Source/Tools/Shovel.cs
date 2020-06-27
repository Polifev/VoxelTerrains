using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.Tools
{
    public class Shovel : MonoBehaviour
    {
        [SerializeField]
        private ChunkBasedScalarField terrain;
        [SerializeField]
        private float _range = 10.0f;
        [SerializeField]
        private float _strength = 0.01f;
        [SerializeField]
        private LayerMask _voxelMask = 0;

        private bool _digging = false;

        public void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                _digging = true;
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                _digging = false;
            }
        }

        private void Update()
        {
            if (_digging)
            {
                Dig();
            }   
        }

        private void Dig()
        {
            Vector3 target = RaycastForward();
            
            if(target != Vector3.negativeInfinity)
            {
                terrain.AddValueAt(target, Time.deltaTime * _strength * -1.0f);
            }
            else
            {
                Debug.Log("Raycast failed");
            }
            
        }

        private Vector3 RaycastForward()
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, _range, _voxelMask))
            {
                Debug.Log(hit.point);
                return hit.point;
            }
            return Vector3.negativeInfinity;
        }
    }
}

