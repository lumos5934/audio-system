using UnityEditor;
using UnityEngine;

namespace LLib.Editor
{
    public static class AudioMenu
    {
        [MenuItem("GameObject/Audio/Audio Manager", false, 10)]
        public static void CreateAudioManager()
        {
            CreateObject<AudioManager>("AudioManager");
        }
        
        
        [MenuItem("GameObject/Audio/Audio Player", false, 10)]
        public static void CreateAudioPlayer()
        {
            CreateObject<AudioPlayer>("AudioPlayer");
        }


        private static void CreateObject<T>(string objectName) where T : Component
        {
            GameObject audioManagerObject = new GameObject(objectName);
            audioManagerObject.AddComponent<T>();

            GameObjectUtility.EnsureUniqueNameForSibling(audioManagerObject);
            
            Undo.RegisterCreatedObjectUndo(audioManagerObject, objectName);
            Selection.activeGameObject = audioManagerObject;
        }
    }
}