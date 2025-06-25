using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMain.Scripts.test
{
    public class InputControlScript : MonoBehaviour
    {
        private Vector3 _pos;
    
        private void Start()
        {
            _pos = GetComponent<Transform>().position;
        }

        private void Update()
        {
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                _pos.z += 1;
            }
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                _pos.z -= 1;
            }
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                _pos.x -= 1;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                _pos.x += 1;
            }
            GetComponent<Transform>().SetPositionAndRotation(_pos, Quaternion.identity);
        }
    }
}
