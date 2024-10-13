using AwakeComponents.AwakeMediaPlayer;
using UnityEngine;

[RequireComponent(typeof(AMP))]
public class LoopedAMP : MonoBehaviour
{
    private AMP _loopedMediaPlayer => GetComponent<AMP>();
    public float loopTime;
    
    void Start()
    {
        _loopedMediaPlayer.onFinished.AddListener(() =>
        {
            _loopedMediaPlayer.Seek(loopTime);
        });
    }
}
