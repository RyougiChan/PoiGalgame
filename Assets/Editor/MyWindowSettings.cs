using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWindowSettings : ScriptableObject
{
    [SerializeField]
    string m_SomeSettings;

    public void Init(MyEditorWindow window)
    {
        m_SomeSettings = window.someSettings;
    }

    public void ApplySettings(MyEditorWindow window)
    {
        window.someSettings = m_SomeSettings;
        window.Repaint();
    }
}