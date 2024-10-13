using AwakeComponents.DebugUI;
using UnityEngine;

namespace AwakeComponents.Utils
{
    /// <summary>
    /// <c>CustomDataTypes</c> is a component that demonstrates custom data types in the application.
    /// </summary>
    [ComponentInfo("0.1", "04.04.2024")]
    public class CustomDataTypes : IDebuggableComponent
    {
        /// <summary>
        /// Custom enum type that represents a direction.
        /// <list type="bullet">
        /// <item><description><see cref="Left"/> - Left, counter-clockwise or "previous" direction.</description></item>
        /// <item><description><see cref="Right"/> - Right, clockwise or "next" direction.</description></item>
        /// </list>
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Left, counter-clockwise or "previous" direction.
            /// </summary>
            Left,
            /// <summary>
            /// Right, clockwise or "next" direction.
            /// </summary>
            Right,
        }
        
        public void RenderDebugUI()
        {
            // This is a debug UI method, it will be called by the DebugUIManager
            GUILayout.Label("Custom data types:");
            
            GUILayout.Label("* " + typeof(Direction));
        }
    }
}