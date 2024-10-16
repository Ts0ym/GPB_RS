using System;
using System.Collections;
using _Scripts.Level.UI;
using AwakeComponents.ArtNet;
using AwakeComponents.AwakeMediaPlayer;
using AwakeComponents.Utils;
using UnityEngine;

namespace _Scripts.Level.Controllers
{
    enum LevelState{
     Idle = 0,
     SlideShow = 1,
     Transition = 2,
    }

    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { get; private set; }
        
        private LevelState _currentState = LevelState.Idle;
        private float _idleTimeout = 60f;
        private float _timeSinceLastInteraction;
        
        [SerializeField] private AMP _transitionMask;
        [SerializeField] private AMP _transition;
        [SerializeField] private AMP _idle;
        [SerializeField] private RectMaskAnimation _slidesMask;
        [SerializeField] private float _delayBeforeFadeIn;

        [SerializeField] private GameObject _whiteBG;
        [SerializeField] private AMP _whiteTransition;
        
        public LedAnimationController ledAnimationController;
        
        private KeyCode[] numberKeys = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6
        };

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _transitionMask.onFinished.AddListener(() => _transitionMask.Seek(6));
            
            _slidesMask.SetStartPosition();
            
            if (ledAnimationController == null)
            {
                Debug.LogError("LedAnimationController не установлен в LevelController!");
            }
            else
            {
                ledAnimationController.SetState(LedAnimationController.AnimationState.Idle);
            }
        }

        private void Update()
        {
            // IdleStateTimer();
            
            for (int i = 0; i < numberKeys.Length; i++)
            {
                if (Input.GetKeyDown(numberKeys[i]))
                {
                    SetSlideShowState(i + 1);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SetIdleState();
            }
        }
        
        
        public void HandleControllerRotation(CustomDataTypes.Direction direction)
        {
            switch (direction)
            {
                case CustomDataTypes.Direction.Left:
                    SlidesController.Instance.SetPreviousSlide();
                    break;
                
                case CustomDataTypes.Direction.Right:
                    SlidesController.Instance.SetNextSlide();
                    break;
            }
        }
        
        public void ResetIdleTimer()
        {
            _timeSinceLastInteraction = 0f;
        }
        
        private void StartIdleFadeOut(Action onComplete = null)
        {
            _transitionMask.Play();
            _transition.Play();
            
            _slidesMask.DelayedStartMaskAnimation(_delayBeforeFadeIn, false,() => onComplete?.Invoke());
        }

        private void IdleStateTimer()
        {
            if (_currentState != LevelState.Idle)
            {
                _timeSinceLastInteraction += Time.deltaTime;
                
                if (_timeSinceLastInteraction >= _idleTimeout)
                {
                    SetIdleState();
                }
            }
        }

        private void StartIdleFadeIn()
        {
            SlidesController.Instance.PauseSlides();
            RestartIdle();
            
            _transitionMask.Seek(0);
            _transitionMask.Pause();
            _transitionMask.GetComponent<RectMaskAnimation>().SetStartPosition();
            
            _slidesMask.StartReverseMaskAnimation(false,() =>
            {
                _transitionMask.GetComponent<RectMaskAnimation>().StartMaskAnimation();
                _currentState = LevelState.Idle;
            });
        }

        private void RestartIdle()
        {
            _idle.Seek(0);
            _idle.Play();
            
            if (ledAnimationController != null)
            {
                ledAnimationController.SetState(LedAnimationController.AnimationState.Idle);
            }
        }

        private void SetIdleState()
        {
            if (_currentState == LevelState.Idle || _currentState == LevelState.Transition) return;

            if (_whiteBG.activeSelf)
            {
                _whiteBG.SetActive(false);
            }
            
            _currentState = LevelState.Transition;
            StartIdleFadeIn();
            
            if (ledAnimationController != null)
            {
                ledAnimationController.SetState(LedAnimationController.AnimationState.ReturnToIdle);
            }
        }
        
        private void SetSlideShowState(int sectionIndex)
        {
            ResetIdleTimer();
            if (_currentState == LevelState.SlideShow || _currentState == LevelState.Transition) return;
            
            if (sectionIndex == 4)
            {
                StartCoroutine(DelayedShowWhiteBG(2));
                _whiteTransition.Play();
            }
            
            _currentState = LevelState.Transition;
            StartIdleFadeOut(() => _currentState = LevelState.SlideShow);
            StartCoroutine(DelayedStartSlideShow(sectionIndex, 0f)); //3.7
            
            if (ledAnimationController != null)
            {
                ledAnimationController.SetState(LedAnimationController.AnimationState.Active, sectionIndex);
            }
        }

        private IEnumerator DelayedShowWhiteBG(float delay)
        {
            yield return new WaitForSeconds(delay);
            _whiteBG.SetActive(true);
        }

        private IEnumerator DelayedStartSlideShow(int sectionIndex, float delay)
        {
            yield return new WaitForSeconds(delay);
            SlidesController.Instance.StartSlideShow(sectionIndex.ToString());
        }
    }
}
