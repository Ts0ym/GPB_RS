using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeComponents.Localization
{
    public class LocalizableText : Localizable
    {
        /*public string stringRu;
        public string stringEn;*/

        public override void Localize(Language language)
        {
            throw new System.NotImplementedException();

            /*
            Text text = GetComponent<Text>();
            TMPro.TextMeshProUGUI tmpText = GetComponent<TMPro.TextMeshProUGUI>();

            if (text != null)
                text.text = nl2br(language.code == Language.RU ? stringRu : stringEn);

            if (tmpText != null)
                tmpText.text = language == Language.RU ? stringRu : stringEn;
            */
        }
    }
}