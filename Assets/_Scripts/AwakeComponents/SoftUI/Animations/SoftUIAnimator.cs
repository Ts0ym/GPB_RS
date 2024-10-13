using System;
using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using AwakeComponents.SoftUI.Animations.DataTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AwakeComponents.SoftUI.Animations
{
    /// <summary>
    /// Animator for SoftUI components
    /// </summary>
    [ComponentInfo("0.3.1", "06.04.2024")]
    public class SoftUIAnimator : MonoBehaviour, IDebuggableComponent
    {
        private CanvasGroup _canvasGroup;

        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        public float quickAnimationDuration = 0.1f;
        public float defaultAnimationDuration = 0.3f;
        public float slowAnimationDuration = 0.5f;

        public float slightAlphaChangeDelta = 0.2f;

        public float slideDistance = 100f;

        public UnityEvent<float> onAlphaChanged = new();

        private Transform Transform => transform;
        private RectTransform RectTransform => (RectTransform) Transform;
        
        private Vector3 _startPosition;
        private Vector3 _startRotation;
        private Vector3 _startScale;
        private float _startAlpha;
        
        // Random color based on GameObject name
        private Color GizmoColor => Color.HSVToRGB(Mathf.Abs(name.Split(" ")[0].GetHashCode()) % 255 / 255f, 1, 1);

        public bool IsAnimating => LeanTween.isTweening(gameObject);
        
        public bool debugMode;

        private void Awake()
        {
            if (debugMode) Debug.Log($"[SoftUIAnimator] Enabled on {name}\nIsAnimating: {IsAnimating}\nAlpha: {CanvasGroup.alpha}");

            if (IsAnimating)
            {
                Debug.LogWarning("[SoftUIAnimator] Сначала включайте GameObject, а уже потом анимируйте его, а то могут быть проблемы с запоминанием стартовых значений и последующим их восстановлением!");
                return;
            }

            _startPosition = Transform.localPosition;
            _startRotation = Transform.localEulerAngles;
            _startScale = Transform.localScale;
            _startAlpha = CanvasGroup.alpha;
        }

        private void OnDisable()
        {
            // Не имееет смысла, так как при завершении анимации Transform значения возвращаются к стартовым
            // А также стартовые значения сохраняются в Awake и при каждом запуске анимации берутся оттуда
            /*Transform.localPosition = _startPosition;
            Transform.localEulerAngles = _startRotation;
            Transform.localScale = _startScale;
            CanvasGroup.alpha = _startAlpha;*/
        }


        public void Animate(AnimationType animationType, Action action = null) =>
            Animate(animationType, AnimationSpeed.Normal, action);

        public void Animate(AnimationType animationType, AnimationSpeed animationSpeed, Action action = null)
        {
            Animate(animationType, GetDuration(animationSpeed), action);
        }

        public void Animate(AnimationType animationType, float duration, Action action = null)
        {
            if (debugMode) Debug.Log($"[SoftUIAnimator] on {name} animating {animationType} with duration {duration}");
            
            switch (animationType)
            {
                case AnimationType.AlphaIn:
                    AnimateAlpha(0, 1, duration, action);
                    break;
                case AnimationType.AlphaSlightIn:
                    AnimateAlpha(1f - slightAlphaChangeDelta, 1, duration, action);
                    break;
                case AnimationType.ToOpaque:
                    AnimateAlpha(CanvasGroup.alpha, 1, duration, action);
                    break;

                case AnimationType.AlphaOut:
                    AnimateAlpha(1, 0, duration, action);
                    break;
                case AnimationType.AlphaSlightOut:
                    AnimateAlpha(1, 1f - slightAlphaChangeDelta, duration, action);
                    break;
                case AnimationType.ToTransparent:
                    AnimateAlpha(CanvasGroup.alpha, 0, duration, action);
                    break;


                case AnimationType.SlideUpIn:
                    AnimatePosition(Vector3.down * slideDistance, Vector3.zero, duration, LeanTweenType.easeOutCubic,
                        action);
                    break;
                case AnimationType.SlideDownIn:
                    AnimatePosition(Vector3.up * slideDistance, Vector3.zero, duration, LeanTweenType.easeOutCubic,
                        action);
                    break;
                case AnimationType.SlideLeftIn:
                    AnimatePosition(Vector3.right * slideDistance, Vector3.zero, duration, LeanTweenType.easeOutCubic,
                        action);
                    break;
                case AnimationType.SlideRightIn:
                    AnimatePosition(Vector3.left * slideDistance, Vector3.zero, duration, LeanTweenType.easeOutCubic,
                        action);
                    break;

                case AnimationType.SlideUpOut:
                    AnimatePosition(Vector3.zero, Vector3.up * slideDistance, duration, LeanTweenType.easeInCubic,
                        action);
                    break;
                case AnimationType.SlideDownOut:
                    AnimatePosition(Vector3.zero, Vector3.down * slideDistance, duration, LeanTweenType.easeInCubic,
                        action);
                    break;
                case AnimationType.SlideLeftOut:
                    AnimatePosition(Vector3.zero, Vector3.left * slideDistance, duration, LeanTweenType.easeInCubic,
                        action);
                    break;
                case AnimationType.SlideRightOut:
                    AnimatePosition(Vector3.zero, Vector3.right * slideDistance, duration, LeanTweenType.easeInCubic,
                        action);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        #region Main animation methods

        public void AnimateAlpha(float from, float to, float duration, Action action = null)
        {
            LeanTween.value(gameObject, from, to, duration)
                .setOnUpdate((float value) =>
                {
                    CanvasGroup.alpha = value;
                    onAlphaChanged.Invoke(value);
                })
                .setOnComplete(() => action?.Invoke());
        }

        public void AnimatePosition(Vector3 from, Vector3 to, float duration,
            LeanTweenType easeType = LeanTweenType.easeOutQuad, Action action = null)
        {
            // Set start position
            Transform.localPosition = _startPosition + from;

            // Calculate end position
            Vector3 endPosition = _startPosition + to;

            // EaseOut from startPosition to endPosition
            LeanTween.moveLocal(gameObject, endPosition, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    action?.Invoke();
                    
                    // Return to start position
                    Transform.localPosition = _startPosition;
                });
        }

        #endregion

        private void DelayedAction(Action action, float delay)
        {
            if (delay > 0)
                StartCoroutine(DelayedActionCoroutine(action, delay));
            else
                action.Invoke();
        }

        private IEnumerator DelayedActionCoroutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        private float GetDuration(AnimationSpeed animationSpeed)
        {
            return animationSpeed switch
            {
                AnimationSpeed.Instant => 0.01f,
                AnimationSpeed.Quick => quickAnimationDuration,
                AnimationSpeed.Normal => defaultAnimationDuration,
                AnimationSpeed.Slow => slowAnimationDuration,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }

        public void DelayedAlphaIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.AlphaIn), delay);
        public void DelayedAlphaSlightIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.AlphaSlightIn), delay);
        public void DelayedToOpaque(float delay = 0) => DelayedAction(() => Animate(AnimationType.ToOpaque), delay);

        public void DelayedAlphaOut(float delay = 0) => DelayedAction(() => Animate(AnimationType.AlphaOut), delay);

        public void DelayedAlphaSlightOut(float delay = 0) =>
            DelayedAction(() => Animate(AnimationType.AlphaSlightOut), delay);

        public void DelayedToTransparent(float delay = 0) => DelayedAction(() => Animate(AnimationType.ToTransparent), delay);

        public void DelayedSlideUpIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideUpIn), delay);
        public void DelayedSlideDownIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideDownIn), delay);
        public void DelayedSlideLeftIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideLeftIn), delay);
        public void DelayedSlideRightIn(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideRightIn), delay);

        public void DelayedSlideUpOut(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideUpOut), delay);
        public void DelayedSlideDownOut(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideDownOut), delay);
        public void DelayedSlideLeftOut(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideLeftOut), delay);
        public void DelayedSlideRightOut(float delay = 0) => DelayedAction(() => Animate(AnimationType.SlideRightOut), delay);




        public void WaitForChildAnimations(Action action)
        {
            StartCoroutine(WaitForChildAnimationsCoroutine(action));
        }

        private IEnumerator WaitForChildAnimationsCoroutine(Action action)
        {
            // Get all child SoftUIAnimators
            SoftUIAnimator[] childSoftUIAnimators = GetComponentsInChildren<SoftUIAnimator>();

            bool isAllAnimationsFinished = false;

            while (!isAllAnimationsFinished)
            {
                // Check if any child SoftUIAnimator is animating
                isAllAnimationsFinished = true;

                foreach (SoftUIAnimator childSoftUIAnimator in childSoftUIAnimators)
                {
                    if (childSoftUIAnimator.IsAnimating)
                    {
                        isAllAnimationsFinished = false;
                        break;
                    }
                }

                yield return null;
            }

            action.Invoke();

            yield return null;
        }



        #region Debug

        public void RenderDebugUI()
        {
            GUILayout.Label("Not implemented yet");
            GUILayout.Label("Better create separate class SoftUIDebugger for this purpose");
        }
        
        // Draw debug rectanlge by rectTransform
        public void DrawDebugRect(RectTransform rectTransform, Color color)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(corners[i], corners[(i + 1) % 4], color);
            }
        }

        void OnDrawGizmos()
        {
            DrawDebugRect(RectTransform, GizmoColor);
        }

        #endregion
    }
}