using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AwakeComponents.DebugUI;
using UnityEngine.Serialization;
using UnityEngine.Video;
// ReSharper disable ParameterHidesMember

namespace AwakeComponents.AwakeMediaPlayer
{
    /// <summary>
    /// <c>Awake! Media Player</c> <c>(AMP)</c> is a component that plays any video or image file from StreamingAssets folder.
    /// <br/>
    /// It provides a set of events that can be used to handle media playback.
    /// </summary>

    // Awake! Media Player component service settings
    [Icon("AwakeComponents/AMP/Icons/AMP_icon.png")]
    [AddComponentMenu("Awake! Components/Awake! Media Player")]

    // Required Unity components
    [RequireComponent(typeof(RawImage))]

    // Version and release date
    [ComponentInfo("2.2.2", "06.04.2024")]

    // ReSharper disable once InconsistentNaming
    public class AMP : MonoBehaviour, IDebuggableComponent
    {
        #region Public Variables

        /// <summary>
        /// Contains the path to the folder where the media file is located.
        /// <br />
        /// The path is relative to the <c>StreamingAssets</c> folder.
        /// </summary>
        /// <remarks>We do not recommend using empty <c>folderPath</c> or special characters in the path.</remarks>
        [Space, Header("Путь к медиа")]
        public string folderPath;
        /// <summary>
        /// Contains the name of the media file.
        /// <br />
        /// The file must be located in the folder specified in the <c>folderPath</c> variable.
        /// </summary>
        /// <remarks>Do not include the file extension in the name!</remarks>
        public string fileName;

        /// <summary>
        /// Enables or disables automatic playback of the media file when the component is initialized or the media file is loaded.
        /// </summary>
        [Space, Header("Настройки воспроизведения")]
        public bool autoplay = true;

        /// <summary>
        /// Field that enables or disables looping of the media file.
        /// </summary>
        /// <remarks>Do not use this field to control playback from the scripts. Use the <c>Loop</c> property instead.</remarks>
        /// <seealso cref="Loop"/>
        [SerializeField]
        private bool loop = true;
        /// <summary>
        /// Property that enables or disables looping of the media file.
        /// </summary>
        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;
                Controls.SetLooping(value);
            }
        }

        /// <summary>
        /// Field that sets the playback speed of the media file.
        /// </summary>
        /// <remarks>Do not use this field to control playback from the scripts. Use the <c>Speed</c> property instead.</remarks>
        /// <seealso cref="Speed"/>
        [SerializeField, Range(-10, 10)]
        private float speed = 1f;
        /// <summary>
        /// Property that sets the playback speed of the media file.
        /// </summary>
        public float Speed 
        {
            get => speed;
            set
            {
                speed = value;
                Controls.SetSpeed(value);
            }
        }
        
        /// <summary>
        /// Field that sets the volume of the media file.
        /// </summary>
        /// <remarks>Do not use this field to control playback from the scripts. Use the <c>Volume</c> property instead.</remarks>
        /// <seealso cref="Volume"/>
        [SerializeField] private float volume = 1f;
        /// <summary>
        /// Property that sets the volume of the media file.
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                Controls.SetVolume(value);
            }
        }
        

        /// <summary>
        /// If enabled, the fileName field will be used as a key to search for a localized file.
        /// </summary>
        /// <remarks>fileNames for different languages should be stored in the StreamingAssets folder with the following structure: <c>folderPath/fileName_languageCode.fileExtension</c></remarks>
        /// <seealso cref="AwakeComponents.Localization.LocaleManager"/>
        [Space, Header("Локализация")]
        public bool localizable;

        /// <summary>
        /// Aspect ratio of the video in the MP4/WEBM container.
        /// </summary>
        //TODO: Add support for MOV container and images
        [Space, Header("Настройки контейнера MP4/WEBM")]
        public VideoAspectRatio aspectRatio = VideoAspectRatio.FitOutside;
        
        /// <summary>If enabled, the loader will be displayed while the video is loading.</summary>
        public bool showVideoLoader = false;
        
        /// <summary>Sprite that will be displayed while the video is loading.</summary>
        public Sprite loaderSprite;

        /// <summary>
        /// If enabled, the image will preserve its aspect ratio.
        /// </summary>
        [Space, Header("Настройки контейнера изображения")]
        public bool preserveAspect;
        /// <summary>
        /// If enabled, the image will be resized to its native size.
        /// </summary>
        public bool setNativeSize;
        
        /// <summary>
        /// Event that is triggered when the media file is loaded.
        /// </summary>
        [Space,Header("События")]
        public UnityEvent onLoaded = new UnityEvent();
        /// <summary>
        /// Event that is triggered when the media file starts playing.
        /// </summary>
        /// <remarks>Not tested yet, may not work as expected.</remarks>
        public UnityEvent onFinished = new UnityEvent();

        /// <returns>
        /// Current playback status of the media file.
        /// </returns>
        public bool IsPlaying => Controls.IsPlaying();
        /// <returns>
        /// Current playback time of the media file in seconds.
        /// </returns>
        public double Time => Controls.GetTime();
        /// <returns>
        /// Duration of the media file in seconds.
        /// </returns>
        public double Duration => Controls.GetLength();

        /// <summary>
        /// If enabled, the component will output the full log to the console including debug info messages.
        /// </summary>
        [Space, Header("Выводить полный лог в консоль")]
        public bool debug;

        #endregion

        #region Private Variables

        /// <summary>
        /// Contains the <c>AMPControls</c> component that provides media playback control.
        /// </summary>
        /// <remarks>Do not use this field to control playback from the scripts. Use the <c>Controls</c> property instead.</remarks>
        /// <seealso cref="Controls"/>
        private AMPControls _controls;
        /// <summary>
        /// Property that provides media playback control.
        /// </summary>
        /// <seealso cref="AMPControls"/>
        internal AMPControls Controls => _controls ??= new AMPControls(this);

        /// <summary>
        /// Contains the <c>AMPFileManagement</c> component that provides media file management.
        /// </summary>
        /// <remarks>Do not use this field from the scripts. Use the <c>FileManagement</c> property instead.</remarks>
        /// <seealso cref="FileManagement"/>
        private AMPFileManagement _fileManagement;
        /// <summary>
        /// Property that provides media file management.
        /// </summary>
        /// <seealso cref="AMPFileManagement"/>
        private AMPFileManagement FileManagement => _fileManagement ??= new AMPFileManagement(this);

        /// <summary>
        /// Contains the <c>AMPDebugger</c> component that provides debug information.
        /// </summary>
        /// <remarks>Do not use this field from the scripts. Use the <c>Debugger</c> property instead.</remarks>
        /// <seealso cref="Debugger"/>
        private AMPDebugger _debugger;
        /// <summary>
        /// Property that provides debug information.
        /// </summary>
        /// <seealso cref="AMPDebugger"/>
        private AMPDebugger Debugger => _debugger ??= new AMPDebugger(this);

        /// <summary>
        /// Contains the <c>AMPLocalizator</c> component that provides localization support.
        /// </summary>
        /// <remarks>Do not use this field from the scripts. Use the <c>Localizator</c> property instead.</remarks>
        private AMPLocalizator _localizator;
        /// <summary>
        /// Property that provides localization support.
        /// </summary>
        /// <seealso cref="AMPLocalizator"/>
        public AMPLocalizator Localizator
        {
            get
            {
                if (_localizator == null)
                {
                    _localizator = gameObject.GetComponent<AMPLocalizator>() ?? gameObject.AddComponent<AMPLocalizator>();
                }
                return _localizator;
            }
        }

        private RawImage _rawImage;
        public RawImage RawImage
        {
            get {
                if (_rawImage == null) _rawImage = GetComponent<RawImage>();
                if (_rawImage == null) _rawImage = gameObject.AddComponent<RawImage>();
                if (_rawImage == null) throw new NullReferenceException("RawImage component is missing.");
                return _rawImage;
            }
        }

        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();
        
        #endregion

        private void Start()
        {
            if (debug) Debug.Log("[AMP] Start");

            Open(folderPath, fileName, autoplay, Loop);
        }

        void Update()
        {
            Controls.HandlePlaybackEvents();
        }

        /// <summary>
        /// Opens a file with the given folder path and file name.
        /// </summary>
        /// <remarks>
        /// If <see cref="AMP"/> is set to <see cref="localizable"/>, the file will be automatically localized using the <see cref="Localization.LocaleManager"/> component.<br/>
        /// Read the rules for the <see cref="folderPath"/> and <see cref="fileName"/> parameters in the <see cref="AMP"/> class description.<br/>
        /// File will be automatically played if the <see cref="autoplay"/> parameter is set to <c>true</c>.
        /// </remarks>
        /// <param name="folderPath">The path to the folder relative to the "StreamingAssets" folder.</param>
        /// <param name="fileName">The name of the media file without the extension.</param>
        /// <exception cref="NullReferenceException">Thrown when the file is not found.</exception>
        public void Open(string folderPath, string fileName) => FileManagement.Open(folderPath, fileName, autoplay, Loop);

        /// <inheritdoc cref="Open(string,string)"/>
        /// <param name="autoplay">If set to <c>true</c>, the media file will be played automatically.</param>
        /// <param name="loop">If set to <c>true</c>, the media file will be played in a loop.</param>
        /// <seealso cref="loop"/>
        public void Open(string folderPath, string fileName, bool autoplay, bool loop) =>
            FileManagement.Open(folderPath, fileName, autoplay, loop);
        
        /// <summary>
        /// Sets only the <see cref="folderPath"/> and runs the <see cref="Open(string, string)"/> method with the specified earlier file name.
        /// </summary>
        /// <param name="folderPath">The path to the folder relative to the "StreamingAssets" folder.</param>
        public void SetFolder(string folderPath) => Open(folderPath, fileName);
        /// <summary>
        /// Sets only the <see cref="fileName"/> and runs the <see cref="Open(string, string)"/> method with the specified earlier folder path.
        /// </summary>
        /// <param name="fileName">The name of the media file without the extension.</param>
        public void SetFile(string fileName) => Open(folderPath, fileName);

        /// <summary>
        /// Plays the media file from the current position.
        /// </summary>
        /// <seealso cref="Pause"/>
        /// <seealso cref="Stop"/>
        /// <seealso cref="Seek(double)"/>
        /// <seealso cref="Time"/>
        /// <seealso cref="Duration"/>
        /// <seealso cref="IsPlaying"/>
        /// <seealso cref="Loop"/>
        /// <seealso cref="Speed"/>
        [ContextMenu("Play")]
        public void Play() => Controls.Play();
        
        /// <summary>
        /// Pauses the media file.
        /// </summary>
        /// <seealso cref="Play"/>
        /// <seealso cref="Stop"/>
        /// <seealso cref="Seek(double)"/>
        /// <seealso cref="Time"/>
        /// <seealso cref="Duration"/>
        /// <seealso cref="IsPlaying"/>
        /// <seealso cref="Loop"/>
        /// <seealso cref="Speed"/>
        [ContextMenu("Pause")]
        public void Pause() => Controls.Pause();
        
        /// <summary>
        /// Stops the media file and resets the playback position to the beginning.
        /// </summary>
        /// <seealso cref="Play"/>
        /// <seealso cref="Pause"/>
        /// <seealso cref="Seek(double)"/>
        /// <seealso cref="Time"/>
        /// <seealso cref="Duration"/>
        /// <seealso cref="IsPlaying"/>
        /// <seealso cref="Loop"/>
        /// <seealso cref="Speed"/>
        [ContextMenu("Stop")]
        public void Stop() => Controls.Stop();
        
        /// <summary>
        /// Seeks the media file to the specified time in seconds.
        /// </summary>
        /// <param name="time">The time in seconds.</param>
        /// <seealso cref="Play"/>
        /// <seealso cref="Pause"/>
        /// <seealso cref="Stop"/>
        /// <seealso cref="Time"/>
        /// <seealso cref="Duration"/>
        /// <seealso cref="IsPlaying"/>
        /// <seealso cref="Loop"/>
        /// <seealso cref="Speed"/>
        public void Seek(double time) => Controls.Seek(time);
        
        /// <summary>
        /// Shows the UI in DebugUI component.
        /// </summary>
        public void RenderDebugUI() => Debugger.RenderDebugUI();
        
        /// <summary>
        /// OnValidate is called in the editor when the script is loaded or a value is changed in the inspector.
        /// </summary>
        private void OnValidate()
        {
            Loop = loop;
            Speed = speed;
        }
    }
}
