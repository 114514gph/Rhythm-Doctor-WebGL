using UnityEngine;

namespace Rewired
{
    [AddComponentMenu("Rewired/Input Manager")]
    public sealed class InputManager : MonoBehaviour
    {
        public static InputManager instance { get; private set; }
        
        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
        
        void Update()
        {
            // WebGL 桩：使用 Unity 标准输入系统
        }
    }
}
