using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSearchAdsGameResumer : MonoBehaviour
{
    
    private void OnAdClosed()
    {
        Time.timeScale = 1;
    }
}
