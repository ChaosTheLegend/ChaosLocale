using ChaosLocale.Scripts.AssetLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosLocale.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageTranslate : MonoBehaviour
    {
        public LocalizedSprite sprite;
        
        private Image img;
        private void Awake()
        {
            img = GetComponent<Image>();
        }
        
        private void OnEnable()
        {
            UpdateTranslation();
        }

        private void UpdateTranslation()
        {
            if(sprite == null || sprite.GetSprite() == null) return;
            img.sprite = sprite.GetSprite();
        }

    }
}