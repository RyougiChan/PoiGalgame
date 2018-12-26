using UnityEditor.Presets;

// PresetSelector receiver to update the EditorWindow with the selected values.

public class MySettingsReceiver : PresetSelectorReceiver
{
    Preset initialValues;
    MyWindowSettings currentSettings;
    MyEditorWindow currentWindow;

    public void Init(MyWindowSettings settings, MyEditorWindow window)
    {
        currentWindow = window;
        currentSettings = settings;
        initialValues = new Preset(currentSettings);
    }

    public override void OnSelectionChanged(Preset selection)
    {
        if (selection != null)
        {
            // Apply the selection to the temporary settings
            selection.ApplyTo(currentSettings);
        }
        else
        {
            // None have been selected. Apply the Initial values back to the temporary selection.
            initialValues.ApplyTo(currentSettings);
        }

        // Apply the new temporary settings to our manager instance
        currentSettings.ApplySettings(currentWindow);
    }

    public override void OnSelectionClosed(Preset selection)
    {
        // Call selection change one last time to make sure you have the last selection values.
        OnSelectionChanged(selection);
        // Destroy the receiver here, so you don't need to keep a reference to it.
        DestroyImmediate(this);
    }
}