using Klak.Hap;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Video;

namespace AwakeComponents.AwakeMediaPlayer
{
    /// <summary>
    /// <c>AMPControls</c> is a class that provides a set of methods to control the playback of the media.
    /// </summary>
    /// <remarks>Used in combination with <c>AMP</c> component to control the playback of the media.</remarks>
    /// <seealso cref="AMP"/>
    
    // ReSharper disable once InconsistentNaming
    public class AMPControls
    {
        /// <summary>
        /// Stores the AMP component that this class is controlling.
        /// </summary>
        private readonly AMP _amp;

        /// <summary>
        /// Player type of the media loaded in the AMP component.<br/><br/>
        /// </summary>
        /// <seealso cref="PlayerType"/>
        private PlayerType _playerType;
        
        private VideoPlayer _videoPlayer;
        private HapPlayer _hapPlayer;

        /// <summary>
        /// Constructor for the <c>AMPControls</c> class.
        /// <br/>
        /// Stores the AMP component that this class is controlling and sets the player type to NONE.
        /// </summary>
        public AMPControls(AMP amp)
        {
            _amp = amp;
            _playerType = PlayerType.NONE;
        }

        /// <inheritdoc cref="AMP.IsPlaying"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.IsPlaying()</c> instead.</remarks>
        /// <seealso cref="AMP.IsPlaying"/>
        public bool IsPlaying()
        {
            return _playerType switch
            {
                PlayerType.VIDEO => _videoPlayer != null && _videoPlayer.isPlaying,
                PlayerType.HAP => _hapPlayer != null && _amp.Speed != 0,
                _ => false,
            };
        }

        /// <inheritdoc cref="AMP.Play"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Play()</c> instead.</remarks>
        /// <seealso cref="AMP.Play"/>
        public void Play()
        {
            _amp.Speed = 1;
            
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.Play();
                    break;
                case PlayerType.HAP:
                    _amp.Speed = 1;
                    break;
                case PlayerType.IMAGE:
                    // There's no play action for images
                    break;
            }
        }

        /// <inheritdoc cref="AMP.Pause"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Pause()</c> instead.</remarks>
        /// <seealso cref="AMP.Pause"/>
        public void Pause()
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.Pause();
                    break;
                case PlayerType.HAP:
                    _amp.Speed = 0;
                    break;
                case PlayerType.IMAGE:
                    // There's no pause action for images
                    break;
            }
        }

        /// <inheritdoc cref="AMP.Stop"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Stop()</c> instead.</remarks>
        /// <seealso cref="AMP.Stop"/>
        public void Stop()
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.Stop();
                    break;
                case PlayerType.HAP:
                    _amp.Speed = 0;
                    _hapPlayer.time = 0;
                    break;
                case PlayerType.IMAGE:
                    // There's no stop action for images
                    break;
            }
        }

        /// <inheritdoc cref="AMP.Seek"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Seek(time)</c> instead.</remarks>
        /// <seealso cref="AMP.Seek"/>
        public void Seek(double time)
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.time = time;
                    break;
                case PlayerType.HAP:
                    _hapPlayer.time = (float) time;
                    break;
                case PlayerType.IMAGE:
                    // There's no seek action for images
                    break;
            }
        }

        /// <inheritdoc cref="AMP.Duration"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Duration()</c> instead.</remarks>
        /// <seealso cref="AMP.Duration"/>
        public double GetLength()
        {
            return _playerType switch
            {
                PlayerType.VIDEO => _videoPlayer.length,
                PlayerType.HAP   => _hapPlayer.streamDuration,
                _ => 0
            };
        }
        
        /// <inheritdoc cref="AMP.Time"/>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Time()</c> instead.</remarks>
        /// <seealso cref="AMP.Time"/>
        public double GetTime()
        {
            return _playerType switch
            {
                PlayerType.VIDEO => _videoPlayer.time,
                PlayerType.HAP   => _hapPlayer.time,
                _ => 0
            };
        }

        /// <summary>
        /// Sets the player type and loads the media from the given path.
        /// </summary>
        /// <param name="playerType">Type of the player to be set.</param>
        /// <param name="exactPath">Exact path of the media to be loaded.</param>
        /// <remarks>Do not use this method directly. All media loading should be done through the <c>AMP</c> component.</remarks>
        /// <seealso cref="AMP.Open(string, string)"/>
        /// <seealso cref="PlayerType"/>
        public void SetPlayer(PlayerType playerType, string exactPath)
        {
            if (_amp.debug) Debug.Log($"Setting player to {playerType} with path {exactPath}");
            
            RemoveExistingPlayers();
            
            _playerType = playerType;

            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    SetupVideoPlayer(exactPath);
                    break;
                case PlayerType.HAP:
                    SetupHapPlayer(exactPath);
                    break;
                case PlayerType.IMAGE:
                    SetupImagePlayer(exactPath);
                    break;
            }
        }
        
        private void SetupVideoPlayer(string exactPath)
        {
            if (_amp.showVideoLoader)
                AMPUtilities.ShowVideoLoader(_amp.gameObject, _amp.loaderSprite);

            _videoPlayer = _amp.gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.url = exactPath;
            _videoPlayer.playbackSpeed = _amp.Speed;
            _videoPlayer.isLooping = _amp.Loop;
            _videoPlayer.aspectRatio = _amp.aspectRatio;
            _videoPlayer.waitForFirstFrame = false;
            _videoPlayer.skipOnDrop = true;
            _videoPlayer.prepareCompleted += source => AMPUtilities.HideVideoLoader(_amp.gameObject);
            
            SetVolume(_amp.Volume);
            
            if (_amp.RawImage.texture == null || _amp.RawImage.texture.GetType() != typeof(RenderTexture))
            {
                RenderTexture renderTexture = CreateRenderTexture();
                _amp.RawImage.texture = renderTexture;
                _videoPlayer.targetTexture = renderTexture;
            } else {
                _videoPlayer.targetTexture = (RenderTexture) _amp.RawImage.texture;
            }

            if (_amp.autoplay)
                _videoPlayer.Play();
            else
                _videoPlayer.Pause();
        }

        private void SetupHapPlayer(string exactPath)
        {
            _hapPlayer = _amp.gameObject.AddComponent<HapPlayer>();
            _hapPlayer.Open(exactPath);
            _hapPlayer.loop = _amp.Loop;
            
            if (_amp.RawImage.texture == null || _amp.RawImage.texture.GetType() != typeof(RenderTexture))
            {
                RenderTexture renderTexture = CreateRenderTexture();
                _amp.RawImage.texture = renderTexture;
                _hapPlayer.targetTexture = renderTexture;
            } else {
                _hapPlayer.targetTexture = (RenderTexture) _amp.RawImage.texture;
            }
            
            _amp.Speed = _amp.autoplay ? 1 : 0;
        }

        private void SetupImagePlayer(string exactPath)
        {
            if (_amp.debug) Debug.Log($"Setting image to {exactPath}");
            
            _amp.StartCoroutine(AMPUtilities.LoadTextureAsync(exactPath, texture =>
            {
                _amp.RawImage.texture = texture;
                
                if (_amp.preserveAspect)
                    throw new System.NotImplementedException();

                if (_amp.setNativeSize)
                    _amp.RawImage.SetNativeSize();
            }));
        }

        private void RemoveExistingPlayers()
        {
            if (_videoPlayer != null) Object.Destroy(_videoPlayer);
            if (_hapPlayer != null) Object.Destroy(_hapPlayer);
        }

        private RenderTexture CreateRenderTexture()
        {
            var rect = _amp.RectTransform.rect;
            return new RenderTexture((int)rect.width, (int)rect.height, 24);
        }

        /// <summary>
        /// Handles the playback events for the current player.
        /// </summary>
        /// <remarks>Primarily used for checking if the media has finished playing in HapPlayer.</remarks>
        public void HandlePlaybackEvents()
        {
            if (IsPlaying() && _playerType == PlayerType.HAP && (_hapPlayer.time >= _hapPlayer.streamDuration || _hapPlayer.time < 0))
            {
                if (_amp.debug)
                    Debug.Log("[AMP] HapPlayer finished playing " + _hapPlayer.time + " / " + _hapPlayer.streamDuration);

                if (_amp.Loop)
                    Seek(_amp.Speed > 0 ? 0 : (float)_hapPlayer.streamDuration);
                else
                {
                    Seek(_amp.Speed > 0 ? 0 : (float)_hapPlayer.streamDuration);
                    Pause();
                }

                _amp.onFinished?.Invoke();
            }
        }

        /// <inheritdoc cref="AMP.Loop"/>
        /// <param name="value">Whether the media should loop or not.</param>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Loop</c> instead.</remarks>
        /// <seealso cref="AMP.Loop"/>
        public void SetLooping(bool value)
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.isLooping = value;
                    break;
                case PlayerType.HAP:
                    _hapPlayer.loop = value;
                    break;
            }
        }

        /// <inheritdoc cref="AMP.Speed"/>
        /// <param name="value">Speed value to be set.</param>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Speed</c> instead.</remarks>
        /// <seealso cref="AMP.Speed"/>
        public void SetSpeed(float value)
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.playbackSpeed = value;
                    break;
                case PlayerType.HAP:
                    _hapPlayer.speed = value;
                    break;
            }
        }
        
        /// <inheritdoc cref="AMP.Volume"/>
        /// <param name="value">Volume value to be set.</param>
        /// <remarks>It is not recommended to use this method directly.<br/>Use <c>AMP.Volume</c> instead.</remarks>
        /// <seealso cref="AMP.Volume"/>
        public void SetVolume(float value)
        {
            switch (_playerType)
            {
                case PlayerType.VIDEO:
                    _videoPlayer.SetDirectAudioVolume(0, value);
                    break;
                case PlayerType.HAP:
                    throw new System.NotImplementedException();
            }
        }
    }
}