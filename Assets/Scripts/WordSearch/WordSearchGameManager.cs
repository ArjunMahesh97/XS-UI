using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

public class WordSearchGameManager : MonoBehaviour
{
    public WordSearchAdsGameResumer adsResumerScript;
    public Transform levelGridParent;
    public RectTransform levelGridMovingContainer;
    public GameObject letterPrefab;
    int[,] levelGrid;
    RectTransform[,] levelGridRT;

    public int levelAdsIndex;
    public int plamUpAdsIndex;
    public Transform solutionParent;
    public GameObject solutionPanelPrefab, solutionPanelStubPrefab, letterSolutionPrefab;
    
    public Text YourScoreText;
    public Text EnemyScoreText;

    public static bool isMultiplayerDemo;
    public int multiplayerPlayerScore;
    public int multiplayerEnemyScore;

    private static List<Sprite> letterSprites = new List<Sprite>();
    private Vector2 levelGridSpeed;

    public string[] solStrings;
    bool[] solFoundIndexes;
    public Button[] solutionCont;

    public GameObject levelCompletedPanel;
    public GameObject levelMultiplayerCompletedPanel;
    public Text levelMultiplayerResultsText;
    public GameObject levelCompletedCongrats;
    public GameObject levelCompletedCongratsChapter;
    public GameObject levelFailedPanel;

    public Transform lineConnectorParent;
    public GameObject lineConnectorPrefab;
    public WordSearchTutorial Tutorial;
    public UILineConnector uiLineConnector;
    public List<GameObject> staticLetters = new List<GameObject>();
    public Transform letterParent;
    public GameObject letterStaticPrefab;
    public List<string> lineArray = new List<string>();
    public List<string> lineStrings = new List<string>();
    public string startLine, endLine;

    int totalRows = 12;
    int totalColumns = 6;
    int difficultyPercent = 0;

    public GameObject pausePanelGo;
    
    List<GameObject> lettersToClearWithWind = new List<GameObject>();

    private Dictionary<string, List<Vector2Int>> tutorLetters = new Dictionary<string, List<Vector2Int>>();

    public static void SetBannerShownWordSearch(WordSearchBanner show, float height)
    {
#if ADVERTISE
        AdvertiseController.Instance.HideAllBannerAds();
        AdvertiseController.Instance.ShowBanner((int)show,(int)height);

		switch (show)
		{
			case WordSearchBanner.No:
                Camera.main.pixelRect = new Rect(0, 0, Screen.width, Screen.height);
				break;
			case WordSearchBanner.Top:
                Camera.main.pixelRect = new Rect(0, 0, Screen.width, Screen.height - height);
				break;
			case WordSearchBanner.Bottom:
                Camera.main.pixelRect = new Rect(0, height, Screen.width, Screen.height - height);
				break;
		}
		AdvertiseController.Instance.UnHideAllBannerAds();
#endif
    }

    public void GoToLevel(int offset)
    {
        if (levelAdsIndex > 0 && GameObject.Find("AdvertiseController") != null)
        {
            #if ADVERTISE     
                ShowAd();
             #endif
        }

        var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0);
        levelNum += offset;

        if (levelNum < 0) levelNum = WordsManager.GetLevelCount() - 1;
        if (levelNum > WordsManager.GetLevelCount() - 1) levelNum = 0;

        PlayerPrefs.SetInt("WordSearchLevel", levelNum);
#if ADVERTISE
        AdvertiseController.Instance.UnHideAllBannerAds();
        
#endif
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        SceneManager.LoadScene("WordSearchHome");
    }

    public void MakeBotMove()
    {
        if (tutorLetters.Count == 0) return;
        
        var allWords = tutorLetters.Values;
        List<Vector2Int> word = allWords.ElementAt(Random.Range(0, allWords.Count));
        
        GameObject go = Instantiate(lineConnectorPrefab, lineConnectorParent);
        var lineConnector = go.GetComponent<UILineConnector>();
        go.GetComponent<UILineRenderer>().color = WordSearchConstantVariables.EnemyLineColor;

        string checkLine1 = "";
        for (int i = 0; i < word.Count; i++)
        {
            checkLine1 += (i == 0 ? "" : "-") + levelGrid[word[i].y, word[i].x];
            
            RectTransform rectTransform = levelGridRT[word[i].y, word[i].x];
            lineConnector.transforms.Add(rectTransform);
            rectTransform.GetComponent<Image>().color = WordSearchConstantVariables.LineCharColor;
        }
        
        for (int i = 0; i < solStrings.Length; i++)
        {
            if (solStrings[i].Equals(checkLine1))
            {
                tutorLetters.Remove(solStrings[i]);
                Tutorial.gameObject.SetActive(false);
                solFoundIndexes[i] = true;
                solutionCont[i].interactable = false;
                multiplayerEnemyScore++;
                if (isMultiplayerDemo) EnemyScoreText.text = "Enemy - " + multiplayerEnemyScore;

                foreach (Image image in solutionCont[i].GetComponentsInChildren<Image>())
                {
                    image.color = WordSearchConstantVariables.EnemySolutionForeground;
                }
                break;
            }
        }

        CheckLevelCompleted(WordSearchConstantVariables.EnemyWinDelay);
    }

    private void Awake()
    {
        if (letterSprites.Count == 0)
        {
            foreach (var letter in Resources.LoadAll<Sprite>(WordSearchConstantVariables.LettersFolder))
            {
                letterSprites.Add(letter);
            }
            Debug.Log("letter:"+letterSprites.Count);
            letterSprites.Sort((a, b) => int.Parse(a.name) - int.Parse(b.name));
        
            if (letterSprites[0].name == "1")
            {
                letterSprites.Insert(0, null); // For indexes to be from 1
            }
            else if (letterSprites[0].name == "0")
            {
                // Do nothing
            }
            else
            {
                Debug.LogError("No 1.png or 0.png char found, unable to start loading");
            }
        }

        var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0);
        //Random.InitState(levelNum);
        //Debug.Log("IN:"+levelNum);
        WordSearchLevel level = WordsManager.Get(levelNum);
        solStrings = level.Words;
        totalRows = level.Rows;
        totalColumns = level.Columns;
        difficultyPercent = level.DifficultyPercent;
        timer = level.Seconds;
        levelAdsIndex = level.AdsIndex;
        plamUpAdsIndex = level.PlamAdsIndex;
        var parentRectTransform = ((RectTransform)levelGridParent);
        if (!isMultiplayerDemo && level.Boss == BossType.Horizontal)
        {
            Vector2 size = parentRectTransform.sizeDelta;
            float oldSize = size.x;
            size.x *= level.BossMultiplier;
            parentRectTransform.sizeDelta = size;

            Vector2 pos = levelGridMovingContainer.anchoredPosition;
            pos.x += (size.x - oldSize) / 2;
            levelGridMovingContainer.anchoredPosition = pos;

            levelGridSpeed.x = -(size.x - 50) / level.Seconds;
            totalColumns *= level.BossMultiplier;
        }
        else if (!isMultiplayerDemo && level.Boss == BossType.Vertical)
        {
            Vector2 size = parentRectTransform.sizeDelta;
            float oldSize = size.y;
            size.y *= level.BossMultiplier;
            parentRectTransform.sizeDelta = size;

            Vector2 pos = levelGridMovingContainer.anchoredPosition;
            pos.y -= (size.y - oldSize) / 2;
            levelGridMovingContainer.anchoredPosition = pos;

            levelGridSpeed.y = (size.y - 50) / level.Seconds;
            totalRows *= level.BossMultiplier;
        }

        if (isMultiplayerDemo)
        {
            YourScoreText.text = "You - 0";
            EnemyScoreText.text = "Enemy - 0";
            Destroy(timerText.transform.parent.gameObject);
        }
        else
        {
            Destroy(YourScoreText.transform.parent.gameObject);
        }
        
        SetBannerShownWordSearch(WordSearchConstantVariables.BannerInGameScreen, WordSearchConstantVariables.BannerHeightGame);

        WordSearchAnalyticsHelper.AnalyticsEvent("WordSearchLevelLoaded", levelNum + 1);

        levelGrid = new int[totalRows, totalColumns];
        levelGridRT = new RectTransform[totalRows, totalColumns];

        var grid = levelGridParent.GetComponent<AutoGridLayout>();
        grid.spacing = new Vector2(WordSearchConstantVariables.LettersSpacingX, WordSearchConstantVariables.LettersSpacingY);
        
        RectOffset rectOffset = grid.padding;
        rectOffset.top -= WordSearchConstantVariables.LettersOffsetY;
        rectOffset.bottom += WordSearchConstantVariables.LettersOffsetY;
        grid.padding = rectOffset;

        rectOffset = letterParent.GetComponent<HorizontalLayoutGroup>().padding;
        rectOffset.top -= WordSearchConstantVariables.LettersTopOffsetY;
        rectOffset.bottom += WordSearchConstantVariables.LettersTopOffsetY;
        letterParent.GetComponent<HorizontalLayoutGroup>().padding = rectOffset;
    }

    private void Start()
    {
        #if ADVERTISE
        IronSourceEvents.onInterstitialAdClosedEvent += AdsCloseMethod;
        #endif
        GenerateLevelGrid();
        GenerateSolutionLetters();
        solFoundIndexes = new bool[solStrings.Length];

        timerStarted = true;

        if (isMultiplayerDemo)
        {
            InvokeRepeating(nameof(MakeBotMove), WordSearchConstantVariables.EnemyMoveTimeout,WordSearchConstantVariables.EnemyMoveTimeout);
        }
    }

    private void OnDisable()
    {
        #if ADVERTISE
        IronSourceEvents.onInterstitialAdClosedEvent -= AdsCloseMethod;
        #endif
    }

    void GenerateSolutionLetters()
    {
        solutionCont = new Button[solStrings.Length];
        for (int i = 0; i < solStrings.Length; i++)
        {
            GameObject sol = Instantiate(solutionPanelPrefab, solutionParent) as GameObject;
            solutionCont[i] = sol.GetComponent<Button>();
            ColorBlock colorBlock = solutionCont[i].colors;
            colorBlock.disabledColor = WordSearchConstantVariables.SolutionBackground;
            solutionCont[i].colors = colorBlock;

            RectOffset rectOffset = sol.GetComponent<HorizontalLayoutGroup>().padding;
            rectOffset.top -= WordSearchConstantVariables.LettersBottomOffsetY;
            rectOffset.bottom += WordSearchConstantVariables.LettersBottomOffsetY;
            sol.GetComponent<HorizontalLayoutGroup>().padding = rectOffset;

            string[] sl = solStrings[i].Split("-"[0]);
            for (int j = 0; j < sl.Length; j++)
            {
                GameObject letter = Instantiate(letterSolutionPrefab, sol.transform) as GameObject;
                int solIndex = int.Parse(sl[j]);
                var image = letter.GetComponent<Image>();
                image.sprite = GetSprite(solIndex);

                letter.transform.localScale *= WordSearchConstantVariables.LettersBottomScale;
            }
        }

        for (int i = 0; i < (9 - solStrings.Length) % 3; i++)
        {
            Instantiate(solutionPanelStubPrefab, solutionParent);
        }
    }

    public Sprite GetSprite(int index)
    {
        if (index < 0 || index >= letterSprites.Count)
        {
            Debug.LogWarning("Letter with index " + (index) + " is out of sprites range");
            index = 1;
        }

        return letterSprites[index];
    }

    bool CheckForSolutions()
    {
        if (lineStrings.Count == 0)
        {
            Debug.Log("There is no line!");
            Destroy(uiLineConnector.gameObject);
            return false;
        }

        string checkLine1 = lineStrings[0];
        for (int i = 1; i < lineStrings.Count; i++)
        {
            checkLine1 += "-" + lineStrings[i];
        }

        Debug.Log(checkLine1);
        string checkLine2 = lineStrings[lineStrings.Count - 1];
        for (int i = lineStrings.Count - 2; i >= 0; i--)
        {
            checkLine2 += "-" + lineStrings[i];
        }

        Debug.Log(checkLine2);
        bool isSolution = false;
        string solutionFoundString = "";
        for (int i = 0; i < solStrings.Length; i++)
        {
            if (solFoundIndexes[i]) continue; //This solution already found so no checking again
            bool check1 = false;
            bool check2 = false;

            if (solStrings[i].Equals(checkLine1)) check1 = true;
            if (solStrings[i].Equals(checkLine2)) check2 = true;

            if (check1 || check2)
            {
                isSolution = true;
                solutionFoundString = solStrings[i];
                tutorLetters.Remove(solStrings[i]);
                Tutorial.gameObject.SetActive(false);
                solFoundIndexes[i] = true;
                solutionCont[i].interactable = false;
                multiplayerPlayerScore++;
                if (isMultiplayerDemo) YourScoreText.text = "You - " + multiplayerPlayerScore;

                foreach (Image image in solutionCont[i].GetComponentsInChildren<Image>())
                {
                    image.color = WordSearchConstantVariables.SolutionForeground;
                }
                break;
            }
        }

        if (isSolution)
        {
            Debug.Log("Its a solution! " + solutionFoundString);

            CheckLevelCompleted(0);
            
            if (Tutorial != null) Tutorial.gameObject.SetActive(false);
        }
        else
        {
            Destroy(uiLineConnector.gameObject);
        }

        return isSolution;
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && uiLineConnector != null)
        {
            Destroy(uiLineConnector.gameObject);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && uiLineConnector != null)
        {
            Destroy(uiLineConnector.gameObject);
        }
    }

    private void CheckLevelCompleted(float delay)
    {
        bool levelCompleted = true;
        foreach (bool b in solFoundIndexes)
        {
            if (!b)
            {
                levelCompleted = false;
                break;
            }
        }

        if (levelCompleted)
        {
            enabled = false;
#if ADVERTISE
            AdvertiseController.Instance.HideAllBannerAds();
#endif
#if PALMUP

            if (plamUpAdsIndex>0 && AdvertiseController.Instance.isPalmAdReady)
            {
                PalmUpAds();
            }
#endif

            Invoke(nameof(ShowLevelCompletedScreen), delay);
        }
    }

    private void ShowLevelCompletedScreen()
    {
            var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0);

            if (isMultiplayerDemo)
            {
                levelMultiplayerCompletedPanel.SetActive(true);

                levelMultiplayerResultsText.text = "";
                if (multiplayerEnemyScore == multiplayerPlayerScore)
                {
                    levelMultiplayerResultsText.text += "Draw!";
                }
                else if(multiplayerEnemyScore > multiplayerPlayerScore)
                {
                    levelMultiplayerResultsText.text += "You lost";
                }
                else
                {
                    levelMultiplayerResultsText.text += "You won!";
                }
                levelMultiplayerResultsText.text += $"\nYour score: {multiplayerPlayerScore}\nEnemy score: {multiplayerEnemyScore}";
            }
            else
            {
                levelCompletedPanel.SetActive(true);

                int positionInChapter;
                int totalInChapter;
                WordsManager.GetLevelsInChapter(levelNum, out positionInChapter, out totalInChapter);
                bool showChapterCompleted =
                    WordSearchConstantVariables.ChaptersEnabled && positionInChapter == totalInChapter;
                levelCompletedCongrats.SetActive(!showChapterCompleted);
                levelCompletedCongratsChapter.SetActive(showChapterCompleted);
            }
                
            timerStarted = false;

            WordSearchAnalyticsHelper.AnalyticsEvent("WordSearchLevelCompleted", levelNum + 1,
                (int) Time.timeSinceLevelLoad);
            levelNum++;
            if (levelNum > WordsManager.GetLevelCount() - 1) levelNum = 0;
            PlayerPrefs.SetInt("WordSearchLevel", levelNum);
                
            //SetBannerShown(WordSearchConstantVariables.BannerInWinScreen, WordSearchConstantVariables.BannerHeightWin);

            //if (levelAdsIndex > 0 && GameObject.Find("AdvertiseController") != null)
            //{
            //    Invoke(nameof(ShowAd), 1);
            //}
    }

    private void ShowAd()
    {
        //SDKIDs.isAdsAreDisplaying = true;
        GameObject.Find("AdvertiseController").SendMessage("ShowAd", levelAdsIndex);
        
#if !UNITY_EDITOR
        Time.timeScale = 0;
#endif
    }

    private void PalmUpAds()
    {
#if PALMUP
        SDKIDs.isAdsAreDisplaying = true;
        AdvertiseController.Instance.PalmupShowAds();
#endif
        
    }


    // For searching in all 8 direction 
    int[] xDir = {-1, 0, 0, 1, 1, -1, 1, -1};
    int[] yDir = {0, -1, 1, 0, 1, -1, -1, 1};

    //If line is possible in a square array
    string[] ValidLine(string start, string end, int gridLength = 11)
    {
        string[] sl = start.Split("-"[0]);
        string[] el = end.Split("-"[0]);

        int x1 = int.Parse(sl[0]);
        int y1 = int.Parse(sl[1]);

        int x2 = int.Parse(el[0]);
        int y2 = int.Parse(el[1]);

        //Debug.Log(end + ":" + x2 + "," + y2);

        for (int g = 1; g <= gridLength; g++)
        {
            for (int d = 0; d < xDir.Length; d++)
            {
                if (x2 == (x1 + g * xDir[d]) && y2 == (y1 + g * yDir[d]))
                {
                    //Debug.Log("Line possible, Letters between them:" + (g - 1));

                    string[] lineArray = new string[g + 1];
                    lineArray[0] = "" + x1 + "-" + y1;
                    lineArray[g] = "" + x2 + "-" + y2;

                    //Letters
                    for (int l = 1; l <= (g - 1); l++)
                    {
                        int l1 = (x1 + l * xDir[d]);
                        int l2 = (y1 + l * yDir[d]);
                        //Debug.Log("Letter: " + l1 + l2);

                        lineArray[l] = "" + l1 + "-" + l2;
                    }

                    //lineFound = true;
                    return lineArray;
                }
            }
        }

        return null;
    }

    public void UpdateLine(string letterName)
    {
        //CLear static letters first
        foreach (GameObject go in staticLetters)
        {
            Destroy(go);
        }

        staticLetters.Clear();
        lineStrings.Clear();
        if (uiLineConnector) uiLineConnector.transforms.Clear();

        if (string.IsNullOrEmpty(startLine))
        {
            startLine = letterName;
            //Debug.Log("Only one letter!"+startLine);

            GameObject letterGo = Instantiate(letterStaticPrefab, letterParent);
            letterGo.transform.SetAsLastSibling();
            staticLetters.Add(letterGo);

            string[] la = startLine.Split('-');
            int x1 = int.Parse(la[0]);
            int y1 = int.Parse(la[1]);
            int index = levelGrid[x1, y1];
            letterGo.GetComponent<Image>().sprite = GetSprite(index);
        }
        else
        {
            endLine = letterName;
            string[] lineOut = ValidLine(startLine, endLine);
            
            foreach (string ch in lineArray)
            {
                string[] la = ch.Split('-');
                int x1 = int.Parse(la[0]);
                int y1 = int.Parse(la[1]);
                levelGridRT[x1, y1].GetComponent<Image>().color = WordSearchConstantVariables.NotLineCharColor;
            }
            
            lineArray.Clear();

            if (lineOut != null)
            {
                for (int i = 0; i < lineOut.Length; i++) //We will start with 1 index because startLine is already added
                {
                    GameObject letterGo = Instantiate(letterStaticPrefab, letterParent);
                    letterGo.transform.SetAsLastSibling();
                    staticLetters.Add(letterGo);

                    lineArray.Add(lineOut[i]); //Add to line array
                    string[] la = lineArray[i].Split("-"[0]);
                    int x1 = int.Parse(la[0]);
                    int y1 = int.Parse(la[1]);
                    int index = levelGrid[x1, y1];
                    lineStrings.Add(index.ToString());
                    letterGo.GetComponent<Image>().sprite = GetSprite(index);

                    levelGridRT[x1, y1].GetComponent<Image>().color =
                        WordSearchConstantVariables.LineCharColor;

                    //Line connecter transform update
                    uiLineConnector.transforms.Add(levelGridRT[x1, y1]);
                }
            }
        }
    }

    public bool Hint(WordSearchHintButton.HintType type)
    {
        if (Tutorial.gameObject.activeSelf) return false;
        
        var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0);
        
        switch (type)
        {
            case WordSearchHintButton.HintType.Hint:
                ShowTutorial();
                WordSearchAnalyticsHelper.AnalyticsEvent("WordSearchHintUsed", levelNum + 1);
                break;
            case WordSearchHintButton.HintType.Wind:
                Wind();
                WordSearchAnalyticsHelper.AnalyticsEvent("WordSearchWindHintUsed", levelNum + 1);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        return true;
    }

    public void Wind()
    {
        foreach (var letter in lettersToClearWithWind)
        {
            letter.GetComponent<Image>().enabled = false;
        }
    }

    void GenerateLevelGrid()
    {
        int[,] grid = GenerateGridFromSols();
        levelGridParent.GetComponent<AutoGridLayout>().m_Column = totalColumns;
        levelGridParent.GetComponent<AutoGridLayout>().m_Row = totalRows;

        for (int y = 0; y < totalRows; y++)
        {
            for (int x = 0; x < totalColumns; x++)
            {
                GameObject letterGo = Instantiate(letterPrefab, levelGridParent);
                letterGo.GetComponent<Image>().color = WordSearchConstantVariables.NotLineCharColor;
                int index = grid[y, x] - 1;

                //Random number if index == -1
                if (index == -1)
                {
                    index = Random.Range(1, letterSprites.Count);
                    lettersToClearWithWind.Add(letterGo);
                }

                letterGo.GetComponent<Image>().sprite = GetSprite(index + 1);
                letterGo.transform.SetAsLastSibling();

                letterGo.name = y + "-" + x; //Array
                levelGrid[y, x] = index + 1; //Letter
                levelGridRT[y, x] = letterGo.GetComponent<RectTransform>();
            }
        }

        if (isMultiplayerDemo)
        {
            if (WordSearchConstantVariables.MultiplayerDemoTutorialEnabled)
            {
                ShowTutorial();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("WordSearchLevel", 0) == 0 && levelGridSpeed == Vector2.zero)
            {
                ShowTutorial();
            }
        }

        while (lettersToClearWithWind.Count > 7)
        {
            lettersToClearWithWind.RemoveAt(Random.Range(0, lettersToClearWithWind.Count));
        }
    }

    private void ShowTutorial()
    {
        var allWords = tutorLetters.Values;
        var word = allWords.ElementAt(Random.Range(0, allWords.Count));

        Tutorial.Transforms.Clear();
        foreach (Vector2Int letter in word)
        {
            Tutorial.Transforms.Add(levelGridRT[letter.y, letter.x]);
        }

        Tutorial.currentTransform = 0;
        Tutorial.gameObject.SetActive(true);
    }

    int[,] GenerateGridFromSols()
    {
        int[,] res = new int[totalRows, totalColumns];

        // Put normal words
        bool allWordsFitted = false;
        while (!allWordsFitted)
        {
            allWordsFitted = true;
            foreach (string sol in solStrings)
            {
                if (!TryToPutWord(res, sol.Split('-'), true))
                {
                    allWordsFitted = false;
                    break;
                }
            }
        }

        // Put trap words (e.g. foresg)
        int trapsToAdd = Random.Range(0, difficultyPercent) / 10;
        while (trapsToAdd-- > 0)
        {
            string[] word = solStrings[Random.Range(0, solStrings.Length)].Split('-');

            int lettersToReplace = Random.Range(1, solStrings.Length / 2);
            while (lettersToReplace-- > 0)
            {
                word[Random.Range(0, word.Length)] = Random.Range(1, letterSprites.Count + 1).ToString();
            }

            if (!TryToPutWord(res, word, false))
                break;
        }

        return res;
    }

    private bool TryToPutWord(int[,] grid, string[] letters, bool saveAsHint)
    {
        int attepmts = 100;
        bool placeFound;
        while (attepmts-- > 0)
        {
            int y = Random.Range(0, grid.GetLength(0));
            int x = Random.Range(0, grid.GetLength(1));

            int dir = Random.Range(2, 4); // Right, down
            if (Random.Range(0, 50) < difficultyPercent) dir = Random.Range(0, 4); 
            if (Random.Range(0, 100) < difficultyPercent) dir = Random.Range(0, 8);

            placeFound = true;
            if (saveAsHint) tutorLetters.Add(string.Join("-", letters), new List<Vector2Int>());
            foreach (var letter in letters)
            {
                if (saveAsHint) tutorLetters[string.Join("-", letters)].Add(new Vector2Int(x, y));
                int letterNum = int.Parse(letter);
                bool withinGrid = x >= 0 && y >= 0 && x < grid.GetLength(1) && y < grid.GetLength(0);
                if (withinGrid && (grid[y, x] == 0 || grid[y, x] == letterNum))
                {
                    x += xDir[dir];
                    y += yDir[dir];
                }
                else
                {
                    placeFound = false;
                    tutorLetters.Remove(string.Join("-", letters));
                    break;
                }
            }

            if (placeFound)
            {
                x -= xDir[dir] * letters.Length;
                y -= yDir[dir] * letters.Length;

                foreach (var letter in letters)
                {
                    grid[y, x] = int.Parse(letter);
                    x += xDir[dir];
                    y += yDir[dir];
                }

                return true;
            }
        }

        return false;
    }

    float timer = 180f;
    bool timerStarted = false;
    public Text timerText;

    void Update()
    {
        
#if PALMUP
        if(AdvertiseController.Instance.isPalmAdClosed)
        {
            SDKIDs.isAdsAreDisplaying = false;
            AdvertiseController.Instance.isPalmAdReady = false;
            AdvertiseController.Instance.isPalmAdClosed = false;
        }
#endif
          
           levelGridMovingContainer.anchoredPosition += levelGridSpeed * Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                GameObject go = Instantiate(lineConnectorPrefab, lineConnectorParent);
                uiLineConnector = go.GetComponent<UILineConnector>();
                go.GetComponent<UILineRenderer>().color = isMultiplayerDemo ?
                    WordSearchConstantVariables.LineColor :
                    WordSearchConstantVariables.LineColors[Random.Range(0, WordSearchConstantVariables.LineColors.Length)];

                if (Tutorial != null && Tutorial.gameObject.activeSelf) Tutorial.SetVisible(false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Clear letters
                startLine = null;
                endLine = null;

                foreach (GameObject go in staticLetters)
                {
                    Destroy(go);
                }

                staticLetters.Clear();

                if (!CheckForSolutions())
                {
                    // If not correct line, set char colors back to brown

                    foreach (string ch in lineArray)
                    {
                        string[] la = ch.Split('-');
                        int x1 = int.Parse(la[0]);
                        int y1 = int.Parse(la[1]);
                        levelGridRT[x1, y1].GetComponent<Image>().color = WordSearchConstantVariables.NotLineCharColor;
                    }
                }
                lineArray.Clear();
                uiLineConnector = null;

                if (Tutorial != null && Tutorial.gameObject.activeSelf) Tutorial.SetVisible(true);
            }

            if (isMultiplayerDemo || !timerStarted) return;

            timer -= Time.deltaTime;

            float minutes = Mathf.Floor(timer / 60);
            float seconds = Mathf.RoundToInt(timer % 60);

            if (Mathf.CeilToInt(seconds) == 0 && Mathf.CeilToInt(minutes) == 0)
            {
                timerStarted = false;
                levelFailedPanel.SetActive(true);

                //SetBannerShown(WordSearchConstantVariables.BannerInWinScreen, WordSearchConstantVariables.BannerHeightWin);

                var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0);
                WordSearchAnalyticsHelper.AnalyticsEvent("WordSearchTimeout", levelNum + 1);

                //if (levelAdsIndex > 0 && GameObject.Find("AdvertiseController") != null)
                //{
                //    ShowAd();
                //}
            }

            string minS = "" + minutes;
            string secS = "" + seconds;
            if (minutes < 10)
            {
                minS = "0" + minS;
            }

            if (seconds < 10)
            {
                secS = "0" + secS;
            }

            string timeS = minS + ":" + secS;
            timerText.text = timeS;
        
    }

    void AdsCloseMethod()
    {
        pausePanelGo.SetActive(false);
        Debug.Log("addd closeddddd");
        Time.timeScale = 1;
    }
}