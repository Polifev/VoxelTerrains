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
        private AbstractEditableScalarField _terrain = null;
        [SerializeField]
        private float _range = 25.0f;
        [SerializeField]
        private float _strength = 10.0f;
        [SerializeField]
        private LayerMask _voxelMask = 0;

        private bool _digging = false;
        private bool _placing = false;

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

        public void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                _placing = true;
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                _placing = false;
            }
        }

        private void Update()
        {
            if (_digging)
            {
                Dig();
            }
            else if(_placing)
            {
                Place();
            }   
        }

        private void Dig()
        {
            if (RaycastForward(out Vector3 target))
            {
                _terrain.AddValueAt(target, Time.deltaTime * _strength * -1.0f);
            }
        }

        private void Place()
        {
            if(RaycastForward(out Vector3 target))
            {
                _terrain.AddValueAt(target, Time.deltaTime * _strength);
            }
        }

        private bool RaycastForward(out Vector3 hitPosition)
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, _range, _voxelMask))
            {
                hitPosition = hit.point;
                return true;
            }
            hitPosition = Vector3.zero;
            return false;
        }
    }
}

