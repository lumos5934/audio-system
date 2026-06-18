using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioData" ,menuName = "Audio Data", order = 0)]
public class AudioData : ScriptableObject
{
    public string id;
    public bool loop;
    [Range(0, 256)] public int priority = 128;

    [field: SerializeField] public AudioClipData[] ClipData { get; private set; }

    public AudioSource sourcePrefab;

    public AudioClipData GetClipData()
    {
        var rand = Random.Range(0, ClipData.Length);
        return ClipData[rand];
    }
}