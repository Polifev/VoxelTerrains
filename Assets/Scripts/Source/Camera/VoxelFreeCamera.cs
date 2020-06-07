using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VoxelTerrains.Camera
{
    public class VoxelFreeCamera : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Vector2 clampVerticalRotation = new Vector2(-90f, 90f);

        Transform _parent = null;
        Vector2 _movement = Vector2.zero;
        Vector2 _rotation = Vector2.zero;
        float _YRotation = 0f;
        float _ascend = 0f;

        private void Start()
        {
            _parent = transform.parent;
            Cursor.visible = false;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
        }

        public void OnRotation(InputAction.CallbackContext context)
        {
            _rotation = context.ReadValue<Vector2>();
        }

        public void OnAscend(InputAction.CallbackContext context)
        {
            _ascend = context.ReadValue<float>();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            _parent.position += (transform.forward * _movement.y + transform.right * _movement.x + transform.up * _ascend) * deltaTime * movementSpeed;

            _YRotation += -_rotation.y * deltaTime * rotationSpeed;
            _YRotation = Mathf.Clamp(_YRotation, clampVerticalRotation.x, clampVerticalRotation.y);

            Quaternion newRotation = Quaternion.Euler(_YRotation, 0f, 0f);
            transform.localRotation = newRotation;
            _parent.localRotation = Quaternion.Euler(0f, _rotation.x * deltaTime * rotationSpeed, 0f) * _parent.localRotation;
        }
    }
}