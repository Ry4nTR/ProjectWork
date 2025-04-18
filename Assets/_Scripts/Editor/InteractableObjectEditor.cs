using UnityEditor;
using UnityEngine;

namespace ProjectWork
{
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectEditor : Editor
    {
        SerializedProperty scriptReference;
        SerializedProperty canInteractAtStart;
        SerializedProperty isUsingBlackScreen;
        SerializedProperty blackScreenData;

        private bool showInteractionSettings = true;
        private bool showBlackScreenSettings = true;
        private GUIStyle boldFoldoutStyle;

        private void OnEnable()
        {
            scriptReference = serializedObject.FindProperty("m_Script");
            canInteractAtStart = serializedObject.FindProperty("canInteractAtStart");
            isUsingBlackScreen = serializedObject.FindProperty("isUsingBlackScreen");
            blackScreenData = serializedObject.FindProperty("blackScreenData");
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
                    EditorGUILayout.PropertyField(blackScreenData);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(10);
            DrawPropertiesExcluding(serializedObject, "canInteractAtStart", "isUsingBlackScreen", "blackScreenData", "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }

}