﻿using UnityEngine;
using TMPro;

namespace Lean.Localization
{
    // This component will update a Text component with localized text, or use a fallback if none is found
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LeanLocalizedTextMeshProUGUI : LeanLocalizedBehaviour
    {
        [Tooltip("If PhraseName couldn't be found, this text will be used")]
        public string FallbackText;

        // This gets called every time the translation needs updating
        public override void UpdateTranslation(LeanTranslation translation)
        {
            // Get the Text component attached to this GameObject
            var text = GetComponent<TextMeshProUGUI>();
            //text.font = FontsManager.instance.GetFont(LeanLocalization.CurrentLanguage);
            // Use translation?
            if (translation != null)
            {
                text.text = translation.Text;
            }
            // Use fallback?
            else
            {
                text.text = FallbackText;
            }
        }

        protected virtual void Awake()
        {
            // Should we set FallbackText?
            if (string.IsNullOrEmpty(FallbackText) == true)
            {
                // Get the Text component attached to this GameObject
                var text = GetComponent<TextMeshProUGUI>();

                // Copy current text to fallback
                FallbackText = text.text;
            }
        }
    }
}