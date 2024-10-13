using _Scripts.Level.UI;
using AwakeComponents.AwakeMediaPlayer;
using UnityEngine;

public class Slide : MonoBehaviour
{
    [SerializeField] private AMP _mediaPlayer;
    [SerializeField] private RectMaskAnimation _rectMaskAnimation;

    public void SetContent(string foldername, string filename)
    {
        _mediaPlayer.Open(foldername, filename);
    }
    
    public void FadeIn(System.Action onComplete = null)
    {
        Debug.Log("Slide Fading in");
        onComplete += () => _mediaPlayer.Play();
        _rectMaskAnimation.StartMaskAnimation(false, onComplete);
    }
    
    public void FadeOut(System.Action onComplete = null)
    {
        _mediaPlayer.Stop();
        Debug.Log("Slide Fading Out");
        _rectMaskAnimation.StartReverseMaskAnimation(false, onComplete);
    }

    public void Pause()
    {
        _mediaPlayer.Pause();
    }
}