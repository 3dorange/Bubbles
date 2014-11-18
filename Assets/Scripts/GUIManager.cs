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

	public GameObject WaitingLabel;

	public GameObject EnterNamePanel;
	public UILabel EnterNameLabel;
	public GameObject NetworkParametersPanel;
	public GameObject ReadyPanel;

	private string PlayerName = "";
	private string EnemysName = "";

	private LevelManager levelManager;			//основной класс логики

	public UILabel IP_Label;
	public UILabel Port_Label;

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

	public void UpdateEnemysName(string enemyName)
	{
		//обновляем label
		EnemysName = enemyName;
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

	public void ServerPressed()
	{
		//в интерфейсе была нажата кнопка коннекта как сервер
		string ipAdress = IP_Label.text;
		string port = Port_Label.text;

		levelManager.networkManager.ConnectAsServerPressed(ipAdress,port);

		NGUITools.SetActive(NetworkParametersPanel,false);
		NGUITools.SetActive(EnterNamePanel,true);
	}

	public void ClientPressed()
	{
		//в интерфейсе была нажата кнопка коннекта как клиент
		string ipAdress = IP_Label.text;
		string port = Port_Label.text;
		
		levelManager.networkManager.ConnectAsClientPressed(ipAdress,port);
		
		NGUITools.SetActive(NetworkParametersPanel,false);
		NGUITools.SetActive(EnterNamePanel,true);
	}

	public void NameEntered()
	{
		//Нажата кнопка старта игры в панели введения имени
		NGUITools.SetActive(EnterNamePanel,false);
		
		if (PlayerName == "")
		{
			OnSubmitName(EnterNameLabel.text);
		}
		
		UpdatePlayerName();
		levelManager.networkManager.SendPlayerName(PlayerName);

		NGUITools.SetActive(ReadyPanel,true);
	}

	public void OtherIsReady()
	{
		//второй игрок нажал "Start game"
		NGUITools.SetActive(WaitingLabel,false);
		levelManager.OtherIsReadyRecieved();
	}

	public void SinglePressed()
	{
		//нажата кнопка игры в сингл режим
		levelManager.networkManager.ThisGameIsSingle();
		levelManager.OtherIsReadyRecieved();
		levelManager.StartButtonPressed();
		NGUITools.SetActive(NetworkParametersPanel,false);
	}

	public void StartPressed()
	{
		//нажата кнопка готовности игры
		NGUITools.SetActive(ReadyPanel,false);

		if (!levelManager.GetOtherIsReady())
		{
			NGUITools.SetActive(WaitingLabel,true);
		}

		levelManager.networkManager.SendStartButtonPressed();
		levelManager.StartButtonPressed();
	}
}
