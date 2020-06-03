using UnityEngine;
using UnityEngine.UI;

public class WordSearchLevelText : MonoBehaviour
{
	public bool IsMenu;
	
	private void Start ()
	{
		int level = PlayerPrefs.GetInt("WordSearchLevel", 0) + 1;
		var text = GetComponent<Text>();
		text.text = level.ToString();
		text.color = IsMenu ? WordSearchConstantVariables.LevelNumColorMenu : WordSearchConstantVariables.LevelNumColor;
	}
}
