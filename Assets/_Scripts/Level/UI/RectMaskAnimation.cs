using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Level.UI
{
    public class RectMaskAnimation : MonoBehaviour
    {
        public float animationDuration = 3f;
        public float softness = 2000f;
        public bool hideOnStart = false;
        private RectMask2D _rectMask => GetComponent<RectMask2D>();
        private Vector4 _initialPadding;
        private Vector4 _targetPadding;
        private float _imageHeight = 3840f;

        private void Start()
        {
            if (hideOnStart)
            {
                SetStartPosition(); 
            }
        }

        public void SetStartPosition(bool isReversed = false)
        {
            if (isReversed)
            {
                _initialPadding = new Vector4(0, -softness, 0, _imageHeight);
                _targetPadding = new Vector4(0, -softness, 0, -softness / 2);
            }
            else
            {
                _initialPadding = new Vector4(0, _imageHeight, 0, -softness);
                _targetPadding = new Vector4(0, -softness / 2, 0, -softness);
            }

            _rectMask.softness = new Vector2Int(0, (int)softness);
            _rectMask.padding = _initialPadding;
        }

        public void StartMaskAnimation(bool isReversed = false, System.Action onComplete = null)
        {
            SetStartPosition(isReversed);
            StartCoroutine(AnimateMask(animationDuration, onComplete));
        }
        
        public void DelayedStartMaskAnimation(float delay, bool isReversed = false, System.Action onComplete = null)
        {
            StartCoroutine(DelayedAnimateMask(delay, animationDuration, isReversed, onComplete));
        }

        private IEnumerator DelayedAnimateMask(float delay, float duration, bool isReversed, System.Action onComplete = null)
        {
            yield return new WaitForSeconds(delay);
            SetStartPosition(isReversed);
            yield return StartCoroutine(AnimateMask(duration, onComplete));
        }

        private IEnumerator AnimateMask(float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                _rectMask.padding = Vector4.Lerp(_initialPadding, _targetPadding, t);
                yield return null;
            }

            _rectMask.padding = _targetPadding;
            onComplete?.Invoke();
        }
    
        public void StartReverseMaskAnimation(bool isReversed = false, System.Action onComplete = null)
        {
            SetStartPosition(!isReversed);
            StartCoroutine(AnimateMaskReverse(animationDuration, onComplete));
        }

        private IEnumerator AnimateMaskReverse(float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                _rectMask.padding = Vector4.Lerp(_targetPadding, _initialPadding, t);
                yield return null;
            }

            _rectMask.padding = _initialPadding;
            onComplete?.Invoke();
        }
    
        public void SetInstantlyVisible(bool isReversed)
        {
            StopAllCoroutines();
            SetStartPosition(isReversed);
            _rectMask.padding = _targetPadding;
        }
    }
}
