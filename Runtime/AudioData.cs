using UnityEngine;
using UnityEngine.Audio;

namespace LLib
{
    [CreateAssetMenu(fileName = "NewAudioData", menuName = "Audio Data", order = 0)]
    public class AudioData : ScriptableObject
    {
        [field: SerializeField] public bool Loop { get; private set; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField, Range(0, 1)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(0, 1)] public float Pitch { get; private set; } = 1f;
        [field: SerializeField, Range(0, 256)] public int Priority { get; private set; } = 128;
        [field: SerializeField] public AudioSource SourcePrefab { get; private set; }
    }
}