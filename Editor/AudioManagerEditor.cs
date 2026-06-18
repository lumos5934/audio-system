using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace LLib.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            serializedObject.Update();

            var mixerProp = serializedObject.FindProperty("_mixer");
            var setupListProp = serializedObject.FindProperty("_setupList");
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mixerProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            AudioMixer mixer = mixerProp.objectReferenceValue as AudioMixer;

            if (mixer == null)
            {
                EditorGUILayout.HelpBox("audio mixer is needed", MessageType.Info);
                setupListProp.arraySize = 0;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var mixerGroups = mixer.FindMatchingGroups("");
            setupListProp.arraySize = mixerGroups.Length;

            for (int i = 0; i < mixerGroups.Length; i++)
            {
                var element = setupListProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("groupName").stringValue = mixerGroups[i].name;
            }

            EditorGUILayout.Space(20f);
            EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1.0f)); 

            for (int i = 0; i < setupListProp.arraySize; i++)
            {
                var element = setupListProp.GetArrayElementAtIndex(i);
                string targetGroupName = element.FindPropertyRelative("groupName").stringValue;
                SerializedProperty audioDataListProp = element.FindPropertyRelative("audioDataList");

                EditorGUILayout.PropertyField(audioDataListProp, new GUIContent(targetGroupName), true);
                EditorGUILayout.Space(2);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}