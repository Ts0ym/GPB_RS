using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _Scripts.Level.Controllers
{
    public class SlidesController : MonoBehaviour
    {
        public static SlidesController Instance { get; private set;}
    
        [SerializeField] private Transform _slidesParent;   
        [SerializeField] private GameObject _slidePrefab;

        private List<string> _slidePaths = new List<string>();  
        private Dictionary<int, Slide> _loadedSlides = new Dictionary<int, Slide>();  
        private int _currentSlideIndex = -1;                
        private bool _isAnimating = false;
        private string _currentFolderName;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(Instance);
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Prev slide called");
                PreviousSlide();
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Next slide called");
                NextSlide();
            }
        }

        public void StartSlideShow(string folderName)
        {
            _currentFolderName = folderName;
            ClearSlides();

            _slidePaths = GetSlidePaths(folderName);

            if (_slidePaths.Count > 0)
            {
                _currentSlideIndex = 0;
            
                StartCoroutine(LoadSlidesCoroutine());
            }
        }
    
        public void ClearSlides()
        {
            foreach (var slide in _loadedSlides.Values)
            {
                Destroy(slide.gameObject);
            }

            _loadedSlides.Clear();
            _currentSlideIndex = -1;
            _isAnimating = false;
        }

        public void PauseSlides()
        {
            foreach (var slide in _loadedSlides.Values)
            {
                slide.Pause();
            }
        }
    
        private IEnumerator LoadSlidesCoroutine()
        {
            LoadSlide(0);
            _loadedSlides[0].FadeIn();
        
            if (_slidePaths.Count > 1)
            {
                yield return null;
                LoadSlide(1);
            }
        }
    
        private void LoadSlide(int index)
        {
            if (!_loadedSlides.ContainsKey(index) && index >= 0 && index < _slidePaths.Count)
            {
                GameObject slideObject = Instantiate(_slidePrefab, _slidesParent);
                Slide slide = slideObject.GetComponent<Slide>();
                string fileName = Path.GetFileName(_slidePaths[index]);
                slide.SetContent($"Slides/{_currentFolderName}", fileName);
                _loadedSlides[index] = slide;
            }
        }

        public void NextSlide()
        {
            LevelController.Instance.ResetIdleTimer();
        
            if (_isAnimating || _currentSlideIndex == -1 || _currentSlideIndex >= _slidePaths.Count - 1) return;

            Slide currentSlide = _loadedSlides[_currentSlideIndex];
            _isAnimating = true;

            currentSlide.FadeOut(() =>
            {
                _currentSlideIndex++;
                if (!_loadedSlides.ContainsKey(_currentSlideIndex))
                {
                    LoadSlide(_currentSlideIndex);
                }

                Slide nextSlide = _loadedSlides[_currentSlideIndex];
                nextSlide.FadeIn(() => _isAnimating = false);
            

                if (_currentSlideIndex + 1 < _slidePaths.Count)
                {
                    LoadSlide(_currentSlideIndex + 1);
                }

                UnloadUnusedSlides();
            });
        }

        public void PreviousSlide()
        {
            LevelController.Instance.ResetIdleTimer();
        
            if (_isAnimating || _currentSlideIndex == -1 || _currentSlideIndex <= 0) return;

            Slide currentSlide = _loadedSlides[_currentSlideIndex];
            _isAnimating = true;

            currentSlide.FadeOut(() =>
            {
                _currentSlideIndex--;
                if (!_loadedSlides.ContainsKey(_currentSlideIndex))
                {
                    LoadSlide(_currentSlideIndex);
                }

                Slide previousSlide = _loadedSlides[_currentSlideIndex];
                previousSlide.FadeIn(() => _isAnimating = false);

                if (_currentSlideIndex - 1 >= 0)
                {
                    LoadSlide(_currentSlideIndex - 1);
                }

                UnloadUnusedSlides();
            });
        }

        private void UnloadUnusedSlides()
        {
            List<int> slidesToRemove = new List<int>();

            foreach (var slideIndex in _loadedSlides.Keys)
            {
                if (Mathf.Abs(slideIndex - _currentSlideIndex) > 1)
                {
                    slidesToRemove.Add(slideIndex);
                }
            }

            foreach (var slideIndex in slidesToRemove)
            {
                Destroy(_loadedSlides[slideIndex].gameObject);
                _loadedSlides.Remove(slideIndex);
            }
        }

        private List<string> GetSlidePaths(string folderName)
        {
            string slidesPath = Path.Combine(Application.streamingAssetsPath, "Slides", folderName);

            if (Directory.Exists(slidesPath))
            {
                string[] imageFiles = Directory.GetFiles(slidesPath, "*.*")
                    .Where(file => file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".mp4") || file.EndsWith(".mov"))
                    .ToArray();

                return imageFiles.ToList();
            }

            Debug.LogError($"Папка {slidesPath} не существует.");
            return new List<string>();
        }
    }
}
