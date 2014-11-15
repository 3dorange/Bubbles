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

	public UILabel PlayerNameLabel;
	public UILabel EnemyNameLabel;

	public GameObject EnterNamePanel;
	public UILabel EnterNameLabel;
	public GameObject NetworkParametersPanel;

	private string PlayerName = "";
	private string EnemysName = "";

	private LevelManager levelManager;			//основной класс логики
	
	public void SetLevelManager(LevelManager levM)
	{
		levelManager = levM;
	}

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

	private void UpdatePlayerName()
	{
		//обновляем label
		PlayerNameLabel.text = PlayerName;
	}

	private void UpdateEnemysName()
	{
		//обновляем label
		EnemyNameLabel.text = EnemysName;
	}

	public void OnSubmitName(string name)
	{
		//введенное имя игрока
		if (name == "Your name")
		{
			name = "Player";
		}

		PlayerName = name;
	}

	public void StartPressed()
	{
		//Нажата кнопка старта игры в панели введения имени
		NGUITools.SetActive(EnterNamePanel,false);

		if (PlayerName == "")
		{
			OnSubmitName(EnterNameLabel.text);
		}

		UpdatePlayerName();
		levelManager.StartGame();
	}
}
