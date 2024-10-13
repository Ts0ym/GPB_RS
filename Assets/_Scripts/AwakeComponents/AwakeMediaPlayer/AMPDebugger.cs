using UnityEngine;

namespace AwakeComponents.AwakeMediaPlayer
{
    /// <summary>
    /// <c>AMPDebugger</c> is a component that can be attached to any GameObject with an AMP component.
    /// <br/>
    /// It provides a UI for debugging and changing the AMP component's parameters in DebugUI component.
    /// </summary>
    
    // ReSharper disable once InconsistentNaming
    public class AMPDebugger
    {
        private readonly AMP _amp;
        
        public AMPDebugger(AMP amp)
        {
            _amp = amp;
        }

        public void RenderDebugUI()
        {
            Transform transform = _amp.transform;
            string hierarchy = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                hierarchy = transform.name + " / " + hierarchy;
            }
    
            GUILayout.Box(hierarchy);
    
            GUILayout.Space(10);
    
            // Отображение и изменение основных параметров
            GUILayout.Label("Основные настройки:");
            
            GUILayout.Label("Folder path: ");
            _amp.folderPath = GUILayout.TextField(_amp.folderPath, 256);
            GUILayout.Label("File name: ");
            _amp.fileName = GUILayout.TextField(_amp.fileName, 256);
            _amp.autoplay = GUILayout.Toggle(_amp.autoplay, "Autoplay");
            _amp.Loop = GUILayout.Toggle(_amp.Loop, "Loop");

            GUILayout.Space(10);

            // Отображение и изменение параметров воспроизведения
            GUILayout.Label("Параметры воспроизведения:");
            _amp.Speed = GUILayout.HorizontalSlider(_amp.Speed, -10f, 10f);
            GUILayout.Label("Speed: " + _amp.Speed.ToString("F2"));

            GUILayout.Space(10);

            // Отображение текущего состояния и параметров
            GUILayout.Label("Состояние воспроизведения:");
            GUILayout.Label("Is Playing: " + _amp.IsPlaying);
            GUILayout.Label("Current Time: " + _amp.Time.ToString("F2"));
            GUILayout.Label("Total Duration: " + _amp.Duration.ToString("F2"));

            GUILayout.Space(10);
            
            // Управление воспроизведением
            GUILayout.Label("Управление воспроизведением:");
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))  _amp.Play();
            if (GUILayout.Button("Pause")) _amp.Pause();
            if (GUILayout.Button("Stop"))  _amp.Stop();
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);

            // Отображение событий
            GUILayout.Label("События:");
            if (GUILayout.Button("Trigger On Loaded"))
            {
                _amp.onLoaded.Invoke();
            }
            if (GUILayout.Button("Trigger On Finished"))
            {
                _amp.onFinished.Invoke();
            }

            GUILayout.Space(10);

            // Отладочная информация
            GUILayout.Label("Отладочная информация:");
            _amp.debug = GUILayout.Toggle(_amp.debug, "Debug Mode");
        }
    }
}