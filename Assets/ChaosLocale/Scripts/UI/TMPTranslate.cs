using TMPro;
using UnityEngine;

namespace ChaosLocale.Scripts.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTranslate : MonoBehaviour
    {
        [SerializeField] private string translationGroup;
        [SerializeField] private string translationKey;
        [TextArea] [SerializeField] private string text;
        private TextMeshProUGUI tmp;
        private void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        
        private void OnEnable()
        {
            UpdateTranslation();
        }

        private void UpdateTranslation()
        {
            var translate = Localization.GetTranslation(translationGroup, translationKey);
            tmp.text = text.Replace("{t}", translate);
        }
        
    }
}
