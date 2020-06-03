using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WordSearch_DeletePlayerPrefs : EditorWindow
{
    [MenuItem("Game/Clear WordSearch PlayerPrefs")]
    private static void Delete()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Word Search PlayerPrefs has been deleted");
    }
}
