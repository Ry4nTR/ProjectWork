using UnityEngine;
using System;

namespace ProjectWork
{
    /// <summary>
    /// Displays black screenObject with text for transitions or narrative moments.
    /// </summary>
    [CreateAssetMenu(fileName = "BlackScreenData", menuName = "Scriptable Objects/Black Screen Data")]
    public class  BlackScreenData : ScriptableObject
    {
        [Serializable]
        public struct FadeSettings
        {
            public bool UseFade;
            public bool FadeOnlyText;
            [Min(0.01f)] public float FadeDuration;
        }

        [Header("Text Settings")]
        [TextArea(1,10)] public string TextToShow;
        [Min(0.01f)] public float TextStayDuration = 2f;

        [Header("Fading Settings")]
        public FadeSettings FadeInSettings;
        public FadeSettings FadeOutSettings;
    }
}

