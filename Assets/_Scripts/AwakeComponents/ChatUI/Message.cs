using UnityEngine;
using UnityEngine.UI;

namespace AwakeComponents.ChatUI
{
    public class Message : MonoBehaviour
    {
        public Chat chat;
        
        public enum Direction
        {
            Incoming,
            Outgoing
        }

        private Direction _direction;
        public Direction direction
        {
            get => _direction;
            set
            {
                _direction = value;
                UpdateAlignment();
            }
        }
        
        private string _text;
        public string text 
        {
            get => _text;
            set
            {
                _text = value;
                textObject.text = value;
            }
        }
        
        public Text textObject;
        
        // Если сообщение от пользователя, то выравниваем его по правому краю
        // Если сообщение от бота, то выравниваем его по левому краю
        // Выравниваем целиком весь объект внутри родителя а не только текст
        private void UpdateAlignment()
        {
            var rectTransform = GetComponent<RectTransform>();

            Vector2 GetSide() => new(direction == Direction.Outgoing ? 1 : 0, 1);

            rectTransform.anchorMin = GetSide();
            rectTransform.anchorMax = GetSide();
            rectTransform.pivot     = GetSide();
        }
        
        public void SetOffsetTop(float y)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, y);
        }
    }
}