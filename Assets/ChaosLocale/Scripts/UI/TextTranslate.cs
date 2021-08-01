using ChaosLocale.Scripts.AssetLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosLocale.Scripts.UI
{
    [RequireComponent(typeof(Text))]
    public class TextTranslate : MonoBehaviour
    {
        [SerializeField] private string translationGroup;
        [SerializeField] private string translationKey;
        [SerializeField] private LocalizedFont font;
        [TextArea] [SerializeField] private string text;
        private Text txt;
        private void Awake()
        {
            txt = GetComponent<Text>();
        }
        
        private void OnEnable()
        {
            UpdateTranslation();
        }

        private void UpdateTranslation()
        {
            var translate = Localization.GetTranslation(translationGroup, translationKey);
            if(font != null && font.GetFont() != null) txt.font = font.GetFont();
            txt.text = text.Replace("{t}", translate);
        }
        
    }
}
