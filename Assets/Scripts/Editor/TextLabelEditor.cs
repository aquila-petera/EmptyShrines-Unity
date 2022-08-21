using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TextLabel))]
public class TextLabelEditor : Editor
{
    TextLabel label;

    public override VisualElement CreateInspectorGUI()
    {
        label = (TextLabel)target;
        label.UpdateText();
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Text"))
        {
            label.UpdateText();
        }
    }
}
