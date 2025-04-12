using UnityEditor;
using UnityEngine;

namespace ITSProjectWork
{
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectEditor : Editor
    {
        SerializedProperty isUsingBlackScreen;
        SerializedProperty screenMessage;

        private void OnEnable()
        {
            isUsingBlackScreen = serializedObject.FindProperty("isUsingBlackScreen");
            screenMessage = serializedObject.FindProperty("screenMessage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(isUsingBlackScreen);

            GUI.enabled = isUsingBlackScreen.boolValue;
            EditorGUILayout.PropertyField(screenMessage);
            GUI.enabled = true;

            // Draw everything else (concrete class properties) excluding the handled base ones
            DrawPropertiesExcluding(serializedObject, "isUsingBlackScreen", "screenMessage", "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}