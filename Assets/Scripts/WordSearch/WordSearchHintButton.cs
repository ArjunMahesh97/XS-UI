using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WordSearchHintButton : MonoBehaviour
{
    public Text LeftText;
    public Text CoinsText;
    public WordSearchGameManager GameManager;
    public GameObject CoinIcon;
    public GameObject LockIcon;
    public GameObject AdsIcon;

    public HintType Type;
    public WordSearchPowerTutorial Tutorial;

    public bool DisableAfterUsage;

    private int initialCount;
    private int price;
    private int unlockLevel;

    public enum HintType
    {
        Hint,
        Wind
    }
    
    private void Start()
    {
#if ADVERTISE
		IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
#endif
        
        switch (Type)
        {
            case HintType.Hint:
                initialCount = WordSearchConstantVariables.PowerHintInitial;
                price = WordSearchConstantVariables.PowerHintPrice;
                unlockLevel = WordSearchConstantVariables.PowerHintUnlockLevel-1;
                break;
            case HintType.Wind:
                initialCount = WordSearchConstantVariables.PowerWindInitial;
                price = WordSearchConstantVariables.PowerWindPrice;
                unlockLevel = WordSearchConstantVariables.PowerWindUnlockLevel-1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
		
        if (PlayerPrefs.GetInt("WordSearchLevel", 0) >= unlockLevel && 
            PlayerPrefs.GetInt("WordSearchPowerTutorialCompleted" + (int)Type, 0) == 0 &&
            !WordSearchPowerTutorial.TutorialSpawned)
        {
            WordSearchPowerTutorial.TutorialSpawned = true;
            Invoke(nameof(ShowTutorial), 1.6f);
        }
		
        UpdateAppearance();
    }
	
#if ADVERTISE
	void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
	{
		GameManager.Hint(Type);

		UpdateAppearance();
	}
#endif

    private void ShowTutorial()
    {
        Tutorial.gameObject.SetActive(true);
		
        LeftText.text = "";
        CoinIcon.SetActive(false);
		
        StartCoroutine(DisableLock());
    }
    
    public void RemoveHint()
    {
        if (PlayerPrefs.GetInt("WordSearchLevel", 0) < unlockLevel)
        {
            return;
        }

        if (WordSearchPowerTutorial.ActiveInstance != null)
        {
            if (WordSearchPowerTutorial.ActiveInstance == Tutorial)
            {
                WordSearchPowerTutorial.ActiveInstance.gameObject.SetActive(false);
                PlayerPrefs.SetInt("WordSearchPowerTutorialCompleted" + (int) Type, 1);
                GameManager.Hint(Type);
        
                UpdateAppearance();

                if (DisableAfterUsage) GetComponent<Button>().interactable = false;
            }
			
            return;
        }
        

        int hints = PlayerPrefs.GetInt("WordSearchHints"+ (int)Type, initialCount);
        int coins = PlayerPrefs.GetInt("Coins", 100);
        if (hints > 0)
        {
            if (!GameManager.Hint(Type)) return;
            
            hints--;
            PlayerPrefs.SetInt("WordSearchHints"+ (int)Type, hints);
        }
        else
        {
            if (coins >= price)
            {
                if (!GameManager.Hint(Type)) return;
				
                coins -= price;
                PlayerPrefs.SetInt("Coins", coins);
            }
            else
            {
#if ADVERTISE
				IronSource.Agent.showRewardedVideo();
#endif
            }
        }

        UpdateAppearance();

        if (DisableAfterUsage) GetComponent<Button>().interactable = false;
    }

    private IEnumerator DisableLock()
    {
        var component = LockIcon.GetComponent<Image>();
        for (float i = 0; i < 1; i+=Time.unscaledDeltaTime)
        {
            component.color = new Color(1, 1, 1, 1 - i);
            yield return null;
        }
        component.color = new Color(1, 1, 1, 0);
    }

    private void UpdateAppearance()
    {
        if (PlayerPrefs.GetInt("WordSearchLevel", 0) < unlockLevel)
        {
            LockIcon.SetActive(true);
			
            LeftText.text = "";
            CoinIcon.gameObject.SetActive(false);
            AdsIcon.SetActive(false);

            GetComponent<Button>().interactable = false;
            return;
        }
		
        if (PlayerPrefs.GetInt("WordSearchPowerTutorialCompleted" + (int)Type, 0) == 1)
            LockIcon.SetActive(false);
        
        int hints = PlayerPrefs.GetInt("WordSearchHints"+ (int)Type, initialCount);
        int coins = PlayerPrefs.GetInt("Coins", 100);

        if (hints > 0)
        {
            LeftText.text = hints.ToString();
            CoinIcon.SetActive(false);
            AdsIcon.SetActive(false);
            CoinIcon.gameObject.SetActive(false);
        }
        else
        {
            LeftText.text = "";
            if (coins >= price)
            {
                CoinsText.text = price.ToString();
                CoinIcon.gameObject.SetActive(true);
                CoinIcon.SetActive(true);
                AdsIcon.SetActive(false);				
            }
            else
            {
                CoinIcon.gameObject.SetActive(false);
                AdsIcon.SetActive(true);
            }
        }

        GetComponent<Button>().interactable = true;
    }
}
