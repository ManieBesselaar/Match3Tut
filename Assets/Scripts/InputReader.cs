// Based on  https://www.youtube.com/watch?v=ErHEZ5YGQ5M
//Thank you git-amend!
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3
{
    public class InputReader : MonoBehaviour
    {
      PlayerInput _playerInput;
        InputAction _selectAction;
        InputAction _fireAction;

        public event Action Fire;
        public Vector2 Selected => _selectAction.ReadValue<Vector2>();

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _selectAction = _playerInput.actions["Position"];
            _fireAction = _playerInput.actions["Fire"];
            _fireAction.performed += OnFire;
        }
        private void OnDestroy()
        {
            _fireAction.performed -= OnFire;
        }

        private void OnFire(InputAction.CallbackContext context)
        {
         
            Fire?.Invoke();

          
        }
    }
}
    
