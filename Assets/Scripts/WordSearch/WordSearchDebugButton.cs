using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSearchDebugButton : MonoBehaviour 
{
    private static bool debugDisabledInPause;
    public bool IgnoreDisabledInPause;

    private static List<WordSearchDebugButton> buttons = new List<WordSearchDebugButton>(); 

    private void Start()
    {
        if (!WordSearchConstantVariables.DebugButtonsActive)
        {
            Destroy(gameObject);
            return;
        }

        if (IgnoreDisabledInPause)
        {
            GetComponentInChildren<Text>().text = "Debug: " + (debugDisabledInPause ? "off" : "on");
        }
        else
        {
            gameObject.SetActive(!debugDisabledInPause);
            buttons.Add(this);
        }
    }

    public void ToggleDisabledInPause()
    {
        debugDisabledInPause = !debugDisabledInPause;

        foreach (WordSearchDebugButton button in buttons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(!debugDisabledInPause);
            }
        }

        GetComponentInChildren<Text>().text = "Debug: " + (debugDisabledInPause ? "off" : "on");
    }
}
