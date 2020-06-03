using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordSearchHomeManager : MonoBehaviour
{
    public Text CurrentLevel;

    public GameObject MultiplayerDemoButton;

    void Start()
    {
        var levelNum = PlayerPrefs.GetInt("WordSearchLevel", 0) + 1;
        CurrentLevel.text = levelNum.ToString();

      


        //WordSearchGameManager.SetBannerShown(WordSearchConstantVariables.BannerInHomeScreen, WordSearchConstantVariables.BannerHeightHome);

        if (!WordSearchConstantVariables.MultiplayerDemoEnabled) MultiplayerDemoButton.SetActive(false);
    }

    public void StartGame()
    {
        WordSearchGameManager.isMultiplayerDemo = false;
        SceneManager.LoadScene("WordSearchGame");
    }

    public void StartMultiplayerDemoGame()
    {
        WordSearchGameManager.isMultiplayerDemo = true;
        SceneManager.LoadScene("WordSearchGame");
    }

    public void Back()
    {
        SceneManager.LoadScene("SelectGame");
    }
}