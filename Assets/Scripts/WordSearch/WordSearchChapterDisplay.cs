using System;
using UnityEngine;
using UnityEngine.UI;

public class WordSearchChapterDisplay : MonoBehaviour
{
    public Image SquarePrefab;
    
    private Image animatedImage;
    
    private void Start()
    {
        if (!WordSearchConstantVariables.ChaptersEnabled || WordSearchGameManager.isMultiplayerDemo)
        {
            Destroy(gameObject);
            return;
        }

        var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 1)-1; // -1 as we should already switch PlayerPrefs to the next level

        int positionInChapter;
        int totalInChapter;
        WordsManager.GetLevelsInChapter(levelNum, out positionInChapter, out totalInChapter);
        Debug.Log(positionInChapter+"/"+totalInChapter);

        for (int i = 0; i < totalInChapter; i++)
        {
            Image square = Instantiate(SquarePrefab,transform);
            square.color = i < positionInChapter
                ? WordSearchConstantVariables.ChapterCompleted
                : WordSearchConstantVariables.ChapterNotCompleted;

            if (i == positionInChapter - 1) animatedImage = square;
        }
    }

    private void Update()
    {
        if (animatedImage != null)
        {
            animatedImage.color = Color.Lerp(
                WordSearchConstantVariables.ChapterCompleted,
                WordSearchConstantVariables.ChapterNotCompleted,
                Mathf.PingPong(Time.time * 3, 1) * .5f);
        }
    }
}