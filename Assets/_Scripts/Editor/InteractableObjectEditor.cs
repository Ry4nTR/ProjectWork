using UnityEditor;
using UnityEngine;

namespace ProjectWork
{
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectEditor : Editor
    {
        SerializedProperty scriptReference;
        SerializedProperty canInteractAtStart;
        SerializedProperty isInteractionInstant;
        SerializedProperty isUsingBlackScreen;
        SerializedProperty screenMessage;

        private bool showInteractionSettings = true;
        private bool showBlackScreenSettings = true;
        private GUIStyle boldFoldoutStyle;

        private void OnEnable()
        {
            scriptReference = serializedObject.FindProperty("m_Script");
            canInteractAtStart = serializedObject.FindProperty("canInteractAtStart");
            isInteractionInstant = serializedObject.FindProperty("isInteractionInstant");
            isUsingBlackScreen = serializedObject.FindProperty("isUsingBlackScreen");
            screenMessage = serializedObject.FindProperty("screenMessage");
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || target == null)
                return;

            serializedObject.Update();

            EditorGUILayout.PropertyField(scriptReference);
            // Lazy init of bold foldout style
            if (boldFoldoutStyle == null)
            {
                boldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            showInteractionSettings = EditorGUILayout.Foldout(showInteractionSettings, "Interaction Settings", true, boldFoldoutStyle);
            if(showInteractionSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(canInteractAtStart);
                EditorGUILayout.PropertyField(isInteractionInstant);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

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
            EditorGUILayout.Space(10);
            DrawPropertiesExcluding(serializedObject, "canInteractAtStart", "isInteractionInstant", "isUsingBlackScreen", "screenMessage", "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }

}