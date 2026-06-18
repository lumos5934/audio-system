using System.Collections.Generic;
using UnityEngine.Audio;


internal class AudioGroup
{
    internal AudioMixerGroup MixerGroup;
    internal HashSet<AudioHandle> ActiveHandles = new();
}

