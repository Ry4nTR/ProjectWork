using UnityEditor;
using UnityEngine;

namespace ITSProjectWork
{
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectEditor : Editor
    {
        SerializedProperty canInteractAtStart;
        SerializedProperty isUsingBlackScreen;
        SerializedProperty screenMessage;

        private bool showBlackScreenSettings = true;
        private GUIStyle boldFoldoutStyle;

        private void OnEnable()
        {
            canInteractAtStart = serializedObject.FindProperty("canInteractAtStart");
            isUsingBlackScreen = serializedObject.FindProperty("isUsingBlackScreen");
            screenMessage = serializedObject.FindProperty("screenMessage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(canInteractAtStart);
            EditorGUILayout.Space();

            // Lazy init of bold foldout style
            if (boldFoldoutStyle == null)
            {
                boldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            showBlackScreenSettings = EditorGUILayout.Foldout(showBlackScreenSettings, "Black Screen Settings", true, boldFoldoutStyle);

            if (showBlackScreenSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(isUsingBlackScreen);
                if (isUsingBlackScreen.boolValue)
                {
                    EditorGUILayout.PropertyField(screenMessage);
                }
                EditorGUI.indentLevel--;
            }
            DrawPropertiesExcluding(serializedObject, "canInteractAtStart", "isUsingBlackScreen", "screenMessage", "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }

}