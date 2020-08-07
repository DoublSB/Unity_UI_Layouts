using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace doublsb.UI
{
    [CustomEditor(typeof(PagingScrollView))]
    public class PagingScrollViewEditor : Editor
    {
        public PagingScrollView origin;

        private int temp;

        private void OnEnable()
        {
            origin = (PagingScrollView)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Option", EditorStyles.boldLabel);

            temp = EditorGUILayout.IntSlider("Visible Count", origin.visibleCount, 0, 10);
            if(temp != origin.visibleCount)
            {
                origin.visibleCount = temp;
                origin.forceUpdateAtEditor();
            }
            
            origin.padding = EditorGUILayout.FloatField("Left Padding", origin.padding);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Page Indexer Color", EditorStyles.boldLabel);
            origin.enabledColor = EditorGUILayout.ColorField("Enabled Color", origin.enabledColor);
            origin.disabledColor = EditorGUILayout.ColorField("Enabled Color", origin.disabledColor);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(origin);
            }
        }
    }
}