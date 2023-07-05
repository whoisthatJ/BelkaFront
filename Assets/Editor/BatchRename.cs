using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
public class BatchRename : ScriptableWizard
{
    public string baseName = "MyObject_";
    public int startNumber = 0;
    public int increment = 1;

    [MenuItem("GameObject/Batch Rename", false, 0)]
    private static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Batch Rename", typeof(BatchRename), "Rename");
    }

    private void OnEnable()
    {
        UpdateSelectionHelper();
    }

    private void OnSelectionChange()
    {
        UpdateSelectionHelper();
    }

    private void UpdateSelectionHelper()
    {
        helpString = "";
        if (Selection.objects != null)
            helpString = "Number of objects selected: " + Selection.objects.Length;
    }

    private void OnWizardCreate()
    {
        if (Selection.objects == null) return;
        int postFix = startNumber;

        foreach (Object o in Selection.objects)
        {
            o.name = baseName + postFix;
            postFix += increment;
        }
    }
}
#endif