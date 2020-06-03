using UnityEngine;
using UnityEngine.UI;

public class WordSearchCoinsDisplay : MonoBehaviour
{
    private Text componentText;
    
    private void Start()
    {
        componentText = GetComponent<Text>();
        
        if (WordSearchGameManager.isMultiplayerDemo) Destroy(transform.parent.gameObject);
    }

    private void Update()
    {
        componentText.text = PlayerPrefs.GetInt("Coins", 0).ToString();
    }
}
