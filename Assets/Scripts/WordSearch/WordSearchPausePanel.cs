﻿using UnityEngine;

public class WordSearchPausePanel : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }
}
