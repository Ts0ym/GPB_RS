using System.Collections;
using System.Collections.Generic;
using AwakeComponents.AwakeMediaPlayer;
using UnityEngine;

namespace AwakeComponents.SoftUI.SingleLevelMenu
{
    public class SlideLoader : MonoBehaviour
    {
        public string slidesPath = "Media/Menu";

        public List<AMP> dependentAmps = new();

        public void LoadSlide(int index)
        {
            foreach (var amp in dependentAmps)
            {
                amp.SetFolder(slidesPath + "/" + index);
            }
        }
    }
}