using System.Collections;
using AwakeComponents.AwakeMediaPlayer;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeComponents.Transition
{
    public class AwakeTransition : MonoBehaviour
    {
        public AMP transitionPlayer;
        public float fullCoverTime = 0.5f;
        public UnityEvent onFullCover = new UnityEvent();

        void Play()
        {
            StartCoroutine(_Play());
        }

        IEnumerator _Play()
        {
            transitionPlayer.Seek(0);
            transitionPlayer.Play();

            yield return new WaitForSeconds(fullCoverTime);
            
            onFullCover?.Invoke();
        }
    }
}
