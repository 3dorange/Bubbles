using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
	//Класс отвечающий за взаимодействие с интерфейсом
	//задаем интерфейсные лейблы
	public UILabel PlayerScore;
	public UILabel PlayerLives;
	public UILabel PlayerLevel;

	public UILabel EnemyScore;
	public UILabel EnemyLives;

	public void ActivatePlayerLevelChange(int levelNumber)
	{
		//активируем надпись о смене левела
		NGUITools.SetActive(PlayerLevel.gameObject,true);
		PlayerLevel.text = "LEVEL " + levelNumber;

		PlayerLevel.GetComponent<TweenScale>().Reset();
		PlayerLevel.GetComponent<TweenScale>().enabled = true;

		PlayerLevel.GetComponent<TweenAlpha>().Reset();
		PlayerLevel.GetComponent<TweenAlpha>().enabled = true;
	}

	public void LevelLabelAlphaFinished()
	{
		//анимация закончена, отключаем
		PlayerLevel.GetComponent<TweenAlpha>().enabled = false;
		PlayerLevel.GetComponent<TweenScale>().enabled = false;
		NGUITools.SetActive(PlayerLevel.gameObject,false);
	}
}
