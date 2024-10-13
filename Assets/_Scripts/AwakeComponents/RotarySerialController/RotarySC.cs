using AwakeComponents.DebugUI;
using AwakeComponents.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeComponents.RotarySerialController
{
    /// <summary>
    /// <c>RotarySC</c> is a component that listens to rotary encoder and button events.
    /// <br/>
    /// It provides a set of events that can be used to handle rotation and button press.
    /// </summary>
    /// <remarks>Used in combination with <see cref="AwakeComponents.Serial.SerialConnectionManager"/> component to control rotary encoder and button.</remarks>
    [ComponentInfo("1.0", "04.04.2024")]
    public class RotarySC : MonoBehaviour, IDebuggableComponent
    {
        /// <summary>
        /// Enum that represents the <see cref="CustomDataTypes.Direction"/> of the rotation.
        /// </summary>
        public UnityEvent<CustomDataTypes.Direction> onRotated = new();
        /// <summary>
        /// Event that is invoked when the button is pressed.
        /// </summary>
        public UnityEvent onButtonPressed = new();
        /// <summary>
        /// Event that is invoked when the button is released.
        /// </summary>
        public UnityEvent onButtonReleased = new();

        private int _lastRotationValue;

        /// <summary>
        /// If set to <c>true</c>, the component will listen to keyboard input.
        /// </summary>
        public bool allowKeyboardInput = true;
        /// <summary>
        /// If set to <c>true</c>, the component will log debug messages.
        /// </summary>
        public bool debugMode = false;

        /// <summary>
        /// Method that is called when a message is received from the serial connection.
        /// </summary>
        /// <param name="message">The message received from the serial connection.</param>
        public void OnMessage(string message)
        {
            if (message.Contains("press") || message.Contains("down"))
                ButtonPressed();

            else if (message.Contains("release") || message.Contains("up"))
                ButtonReleased();

            else if (message.Contains("rot") || message.Contains("enc"))
            {
                int rotationValue = int.Parse(message.Split(' ', '-', '_')[^1]);

                CustomDataTypes.Direction direction = rotationValue > _lastRotationValue
                    ? CustomDataTypes.Direction.Right
                    : CustomDataTypes.Direction.Left;

                if (rotationValue != _lastRotationValue && Mathf.Abs(rotationValue - _lastRotationValue) < 100)
                    Rotate(direction);

                _lastRotationValue = rotationValue;
            }
        }
        
        
        void ButtonPressed()
        {
            if (debugMode)
                Debug.Log("[RotarySC] Button pressed");

            onButtonPressed.Invoke();
        }
        
        void ButtonReleased()
        {
            if (debugMode)
                Debug.Log("[RotarySC] Button released");
            
            onButtonReleased.Invoke();
        }
        
        /// <summary>
        /// Method that is called when the rotary encoder is rotated.
        /// </summary>
        /// <param name="direction">The <see cref="CustomDataTypes.Direction"/> of the rotation.</param>
        void Rotate(CustomDataTypes.Direction direction)
        {
            if (debugMode)
                Debug.Log("[RotarySC] Rotated " + direction);
            
            onRotated.Invoke(direction);
        }

        
        // Keyboard input
        private void Update()
        {
            if (!allowKeyboardInput)
                return;
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Rotate(CustomDataTypes.Direction.Left);
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ButtonPressed();
                    
            if (Input.GetKeyDown(KeyCode.UpArrow))
                ButtonReleased();
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
                Rotate(CustomDataTypes.Direction.Right);

            if (debugMode)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    Rotate(CustomDataTypes.Direction.Left);
                
                if (Input.GetKey(KeyCode.RightArrow))
                    Rotate(CustomDataTypes.Direction.Right);
                
                if (Input.GetKey(KeyCode.DownArrow))
                    ButtonPressed();
                
                if (Input.GetKey(KeyCode.UpArrow))
                    ButtonReleased();
            }
        }
    

        // Debug UI
        public void RenderDebugUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("◄"))
                Rotate(CustomDataTypes.Direction.Left);
            
            if (GUILayout.Button("▼"))
                ButtonPressed();
                    
            if (GUILayout.Button("▲"))
                ButtonReleased();
            
            if (GUILayout.Button("►"))
                Rotate(CustomDataTypes.Direction.Right);

            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            GUILayout.Box("Last rotation value: " + _lastRotationValue + "\nThis value is updated only when the rotary encoder is rotated");
        }
    }
}