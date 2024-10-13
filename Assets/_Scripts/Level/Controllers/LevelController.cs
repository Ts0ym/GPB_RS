using System;
using System.Collections;
using _Scripts.Level.UI;
using AwakeComponents.AwakeMediaPlayer;
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
        }

        private void Update()
        {
            if (_currentState != LevelState.Idle)
            {
                _timeSinceLastInteraction += Time.deltaTime;
                
                if (_timeSinceLastInteraction >= _idleTimeout)
                {
                    SetIdleState();
                }
            }
            
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
        }

        private void SetIdleState()
        {
            if (_currentState == LevelState.Idle || _currentState == LevelState.Transition) return;
            
            _currentState = LevelState.Transition;
            StartIdleFadeIn();
        }
        
        private void SetSlideShowState(int sectionIndex)
        {
            ResetIdleTimer();
            if (_currentState == LevelState.SlideShow || _currentState == LevelState.Transition) return;
            
            _currentState = LevelState.Transition;
            StartIdleFadeOut(() => _currentState = LevelState.SlideShow);
            StartCoroutine(DelayedStartSlideShow(sectionIndex, 3.8f));
        }

        private IEnumerator DelayedStartSlideShow(int sectionIndex, float delay)
        {
            yield return new WaitForSeconds(delay);
            SlidesController.Instance.StartSlideShow(sectionIndex.ToString());
        }
    }
}
