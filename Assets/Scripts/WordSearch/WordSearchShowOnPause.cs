using UnityEngine;

public class WordSearchShowOnPause : MonoBehaviour
{
    public GameObject Target;
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Target.SetActive(true);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Target.SetActive(true);
        }
    }
}
