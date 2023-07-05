using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class SceneSelection : EditorWindow
{
	[MenuItem ("Custom Editor/Selection Scenes")]
	public static void ShowWindow ()
	{
		GetWindow<SceneSelection> ("Scene Selection");
	}
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    public Vector2 scrollPosition = Vector2.zero;
    private GUIContent[] bunchOfButtons;

    void OnEnable()
    {
        bunchOfButtons = new GUIContent[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < bunchOfButtons.Length; i++)
        {
            bunchOfButtons[i] = new GUIContent("Button" + i.ToString());
        }        
    }

    static void OpenScene (string pathScene)
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
		EditorSceneManager.OpenScene (pathScene);
	}
    private void OnGUI()
    {        
        DoScrollArea(new Rect(0, 0, position.width, position.height), bunchOfButtons, 20);        
    }

    private void DoScrollArea(Rect position, GUIContent[] buttons, int buttonHeight)
    {
        float height = 0; int index = 0;
        if (buttons.Length > 0)
            height = ((buttons.Length - 1) * buttonHeight);
        scrollPosition = GUI.BeginScrollView(position, scrollPosition, new Rect(0, 0, position.width - 20, height + buttonHeight));
        for (index = 0; index < buttons.Length; index++)
            if (((index + 1) * buttonHeight) > scrollPosition.y) break;
        for (; index < buttons.Length && (index * buttonHeight) < scrollPosition.y + position.height; index++)
        {
            var scene = EditorBuildSettings.scenes[index];
            var sceneName = Path.GetFileNameWithoutExtension(scene.path);
            if (GUI.Button(new Rect(0, index * buttonHeight, position.width - 16, buttonHeight), buttons[index].text = index + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")){ alignment = TextAnchor.MiddleLeft}))
            {
                OpenScene(EditorBuildSettings.scenes[index].path);
            }
        }
        GUI.EndScrollView();
    }
}