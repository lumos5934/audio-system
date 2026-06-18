using System;
using UnityEngine;

[Serializable]
public class AudioClipData
{
    [field: SerializeField] public AudioClip Clip { get; private set; }
    [field: SerializeField] public float Volume { get; private set; } = 1f;
    [field: SerializeField] public float Pitch { get; private set; } = 1f;
    
}