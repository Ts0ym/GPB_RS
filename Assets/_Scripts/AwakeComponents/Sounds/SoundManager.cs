using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using AwakeComponents.DebugUI;

namespace AwakeComponents.Sounds
{
  public class SoundManager : MonoBehaviour, IDebuggableComponent
  {
    [Header("Управление звуком")]
    [Tooltip("Эта переменная просто хранит описание работы и не используется в коде")]
    [TextArea(1, 50)]
    public string Инструкция = "" +
        "В папке StreamingAssets создаем папку Sounds, и накидываем туда mp3 и wav звуки.\n\n" +
        "Этот скрипт должен висеть на сцене и не требует AudioSource.\n\n" +
        "В коде вызываем звук так, если не луп: SoundManager.Play(\"file_name_without_extension\");\n\n" +
        "Залупленные звуки поддерживаются так: SoundManager.Play(\"file_name_without_extension\", true);\n\n" +
        "Для остановки звуков: SoundManager.Stop(\"file_name_without_extension\");\n\n" +
        "Для остановки всех звуков: SoundManager.StopAll();\n\n" +
        "Версия от 24.09.2024";

    private static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    private static Dictionary<string, AudioSource> activeAudioSources = new Dictionary<string, AudioSource>(); // Хранение активных AudioSource

    private static List<(string, bool)> playQueue = new List<(string, bool)>();

    public static bool isLoaded = false;
    public static SoundManager instance;

    private void Awake()
    {
      if (instance != null)
      {
        Destroy(instance.gameObject);
        instance = null;
      }

      instance = this;
    }

    public static void Play(string name, bool loop = false)
    {
      if (!isLoaded)
      {
        Debug.Log($"Sounds are not loaded yet. Queueing Play request for \"{name}\".");
        playQueue.Add((name, loop)); // Добавляем в очередь
        return;
      }

      if (sounds.ContainsKey(name))
      {
        PlayClip(sounds[name], name, loop);
      }
      else
      {
        Debug.LogError($"Sound \"{name}\" isn't loaded yet.");
      }
    }

    private static void PlayClip(AudioClip audioClip, string name, bool loop)
    {
      // Проверяем, существует ли уже активный AudioSource для данного звука
      if (activeAudioSources.ContainsKey(name))
      {
        Debug.Log($"Sound \"{name}\" is already playing.");
        return; // Если звук уже воспроизводится, не создаем новый
      }

      GameObject tempGO = new GameObject("One shot audio");
      AudioSource audioSource = tempGO.AddComponent<AudioSource>();
      audioSource.clip = audioClip;
      audioSource.loop = loop;

      audioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
      audioSource.mute = audioSource.mute;
      audioSource.bypassEffects = audioSource.bypassEffects;
      audioSource.bypassListenerEffects = audioSource.bypassListenerEffects;
      audioSource.bypassReverbZones = audioSource.bypassReverbZones;
      audioSource.playOnAwake = audioSource.playOnAwake;
      audioSource.loop = audioSource.loop;
      audioSource.priority = audioSource.priority;
      audioSource.volume = audioSource.volume;
      audioSource.pitch = audioSource.pitch;
      audioSource.panStereo = audioSource.panStereo;
      audioSource.spatialBlend = 0;
      audioSource.reverbZoneMix = audioSource.reverbZoneMix;
      audioSource.dopplerLevel = audioSource.dopplerLevel;
      audioSource.rolloffMode = audioSource.rolloffMode;
      audioSource.spread = audioSource.spread;

      audioSource.minDistance = 10000f;
      audioSource.maxDistance = 10000f;

      audioSource.Play();
      activeAudioSources[name] = audioSource; // Добавляем AudioSource в список активных

      if (!loop)
      {
        MonoBehaviour.Destroy(tempGO, audioSource.clip.length); // Удаляем объект, если звук не повторяется
        activeAudioSources.Remove(name); // Удаляем из активных после воспроизведения
      }
    }

    public static void Stop(string name)
    {
      if (activeAudioSources.ContainsKey(name))
      {
        AudioSource audioSource = activeAudioSources[name];
        audioSource.Stop();
        MonoBehaviour.Destroy(audioSource.gameObject); // Удаляем объект после остановки
        activeAudioSources.Remove(name); // Удаляем из списка активных
      }
      else
      {
        Debug.LogWarning($"Sound \"{name}\" is not playing.");
      }
    }

    public static void StopAll()
    {
      foreach (var audioSource in activeAudioSources.Values)
      {
        audioSource.Stop();
        MonoBehaviour.Destroy(audioSource.gameObject); // Удаляем все активные объекты
      }
      activeAudioSources.Clear(); // Очищаем список активных звуков
    }

    void Start()
    {
      LoadAll();
    }

    private static async void LoadAll()
    {
      Debug.Log("Sounds loading started!");

      string[] allMp3 = Directory.GetFiles(Application.streamingAssetsPath + "/Sounds", "*.mp3", SearchOption.AllDirectories);
      string[] allWav = Directory.GetFiles(Application.streamingAssetsPath + "/Sounds", "*.wav", SearchOption.AllDirectories);

      Debug.Log($"MP3 count: {allMp3.Length}");
      Debug.Log($"WAV count: {allWav.Length}");

      for (int i = 0; i < allMp3.Length; i++)
      {
        await Load(allMp3[i], AudioType.MPEG);
      }

      for (int i = 0; i < allWav.Length; i++)
      {
        await Load(allWav[i], AudioType.WAV);
      }

      isLoaded = true;
    }

    private static async Task Load(string path, AudioType audioType)
    {
#if UNITY_EDITOR_OSX
            path = "file://" + path;
#endif
      string key = Path.GetFileNameWithoutExtension(path); // Получаем имя файла без расширения

      Debug.Log($"Sound loading: {key}");

      using (UnityWebRequest audioLoadRequest = UnityWebRequestMultimedia.GetAudioClip(@path, audioType))
      {
        var operation = audioLoadRequest.SendWebRequest();

        while (!operation.isDone)
          await Task.Yield();

        if (audioLoadRequest.result == UnityWebRequest.Result.Success)
        {
          AudioClip clip = DownloadHandlerAudioClip.GetContent(audioLoadRequest);
          sounds.Add(key, clip);
          Debug.Log($"Sound loaded: {key}");
        }
        else
        {
          Debug.LogError($"Sound loading error: {key}");
        }
      }
    }

    public void RenderDebugUI()
    {
      GUILayout.Label("Sounds count: " + sounds.Count);

      if (GUILayout.Button("Reload"))
      {
        sounds.Clear();
        isLoaded = false;
        Start();
      }

      GUILayout.Space(10);

      GUILayout.Label("Sounds:");

      foreach (var sound in sounds)
      {
        if (GUILayout.Button(sound.Key + " (" + sound.Value.length + " sec)"))
          Play(sound.Key);
      }
    }
  }
}
