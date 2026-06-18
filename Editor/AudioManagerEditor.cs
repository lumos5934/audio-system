using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace LLib.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _mixerProp;
        private SerializedProperty _preRegisteredSoundsProp;

        private void OnEnable()
        {
            _mixerProp = serializedObject.FindProperty("_mixer");
            _preRegisteredSoundsProp = serializedObject.FindProperty("_setupList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_mixerProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            AudioMixer mixer = _mixerProp.objectReferenceValue as AudioMixer;

            if (mixer == null)
            {
                EditorGUILayout.HelpBox("audio mixer is needed", MessageType.Info);
                _preRegisteredSoundsProp.arraySize = 0;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var mixerGroups = mixer.FindMatchingGroups("");
            _preRegisteredSoundsProp.arraySize = mixerGroups.Length;

            for (int i = 0; i < mixerGroups.Length; i++)
            {
                var element = _preRegisteredSoundsProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("groupName").stringValue = mixerGroups[i].name;
            }

            EditorGUILayout.Space(20f);
            EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1.0f)); 

            for (int i = 0; i < _preRegisteredSoundsProp.arraySize; i++)
            {
                var element = _preRegisteredSoundsProp.GetArrayElementAtIndex(i);
                string targetGroupName = element.FindPropertyRelative("groupName").stringValue;
                SerializedProperty audioDataListProp = element.FindPropertyRelative("audioDataList");

                EditorGUILayout.PropertyField(audioDataListProp, new GUIContent(targetGroupName), true);
                EditorGUILayout.Space(2);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}