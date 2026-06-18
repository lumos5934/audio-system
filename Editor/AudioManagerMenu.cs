using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace LLib.Editor
{
    public static class AudioManagerMenu
    {
        [MenuItem("GameObject/Audio/Audio Manager", false, 10)]
        [MenuItem("Audio/Create Audio Manager")]
        public static void CreateAudioManager()
        {
            GameObject audioManagerObject = new GameObject("AudioManager");
            audioManagerObject.AddComponent<AudioManager>();

            GameObjectUtility.EnsureUniqueNameForSibling(audioManagerObject);
            
            Undo.RegisterCreatedObjectUndo(audioManagerObject, "Audio Manager");
            Selection.activeGameObject = audioManagerObject;
        }
    }
}