using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading;
using System.Text.RegularExpressions;


public class NetworkManager : MonoBehaviour 
{
	//Класс отвечающий за мультиплеерную часть
	private LevelManager levelManager;
	private bool isServer = false;
	private SocketManagement socketManager;			//объект для соединения
	private bool connected = false;
	private float netWait = 0.0f;					//время между запросом данных из потока

	void Update()
	{
		if (connected)
		{
			StartCoroutine(Wait(netWait));
		}
	}

	private IEnumerator Wait(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
		GetCommands(socketManager.GetCommand());
	}

	public void SetLevelManager(LevelManager lM)
	{
		levelManager = lM;
	}

	private void GetCommands(NetworkCommand recievedCommand)
	{
		//обработчик пришедших данных по сети

		if (recievedCommand == null)
		{
			return;
		}

		switch (recievedCommand.keyCode)
		{
			case 100:
				OtherPlayerIsReady();
				break;

//			case 101:
//				StartTheGame();
//				break;

			case 102:
				CreateNewEnemyBubble(recievedCommand.CreatedBubble_Name,recievedCommand.CreatedBubble_Scale,recievedCommand.CreatedBubble_Pos,recievedCommand.CreatedBubble_Rot);
				break;

			case 103:
				SetEnemyName(recievedCommand.PlayerNameN);
				break;

			case 104:
				RemoveEnemyBubble(recievedCommand.BubbleNameToDestroy);
				break;

			default:
//				Debug.Log("Error Command");
				break;
		}
	}

	private void OtherPlayerIsReady()
	{
		//другой игрок нажал старт
		levelManager.guiManager.OtherIsReady();
	}

//	private void StartTheGame()
//	{
//		//все готово и можно начинать игру
//	}

	private void SetEnemyName(string enemyName)
	{
		//отправляем имя противника в интерфейс
		levelManager.guiManager.UpdateEnemysName(enemyName);
	}

	private void CreateNewEnemyBubble(string NewBubbleName, float NewBubbleScale, Vector3Serializer NewBubblePosition, QuaternionSerializer NewBubbleRotation)
	{
		//нужно создать новый шарик с пришедшеми параметрами
//		Vector3 bubblePos = new Vector3(NewBubblePosition.x,NewBubblePosition.y,NewBubblePosition.z);
		Vector3 bubblePos = Vector3.zero;
//		Quaternion bubbleRot = new Quaternion(NewBubbleRotation.x,NewBubbleRotation.y,NewBubbleRotation.z,NewBubbleRotation.w);
		Quaternion bubbleRot = Quaternion.identity;
		levelManager.CreateNewOtherPlayerBubble(NewBubbleName,NewBubbleScale,bubblePos,bubbleRot);
	}

	private void RemoveEnemyBubble(string PressedBubbleName)
	{
		//нужно убрать с пришедшеми именем
	}

	public void ConnectAsServerPressed(string ip_Adress, string port)
	{
		//была нажата кнопка игры за сервер
		if (checkIPandPort(ip_Adress, port))
		{
			isServer = true;
			ConnectAsServer(ip_Adress, Int32.Parse(port));
		}
	}

	public void ConnectAsClientPressed(string ip_Adress, string port)
	{
		//была нажата кнопка игры за клиент
		if (checkIPandPort(ip_Adress, port))
		{
			isServer = true;
			ConnectAsClient(ip_Adress, Int32.Parse(port));
		}
	}

	private void ConnectAsServer(string ip_Adress, int port)
	{
		//коннектимся как сервер
		socketManager = new SocketManagement(ip_Adress,port);

		if (socketManager.StartAsServer())
		{
			//если удалось начать игру как сервер, то продолжаем
			connected = true;
		}
	}

	private void ConnectAsClient(string ip_Adress, int port)
	{
		//была нажата кнопка игры как клиент
		socketManager = new SocketManagement(ip_Adress,port);

		if (socketManager.StartAsClient())
		{
			//если удалось начать игру как клиент, то продолжаем	
			connected = true;
		}
	}

	private bool checkIPandPort(string ip, string port)
	{
		//проверка ip и порта на валидность
		if (Regex.IsMatch(ip, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$") 
		    && Regex.IsMatch(port, "^[0-9]{1,6}$"))
		{
			string[] temp = ip.Split('.');
			foreach (string q in temp)
			{
				try
				{
					if (Int32.Parse(q) > 255) return false;
				}
				catch (Exception) { return false; }
			}
			return true;
		}
		return false;
	}

	public void SendStartButtonPressed()
	{
		//отправляем команду на то что игру можно начинать
		NetworkCommand StartButtonPressedCommand = new NetworkCommand();
		
		StartButtonPressedCommand.keyCode = 100;
		StartButtonPressedCommand.PlayerNameN = null;
		StartButtonPressedCommand.BubbleNameToDestroy = null;
		StartButtonPressedCommand.CreatedBubble_Name = null;
		StartButtonPressedCommand.CreatedBubble_Scale = 0;
		StartButtonPressedCommand.CreatedBubble_Pos = null;
		StartButtonPressedCommand.CreatedBubble_Rot = null;
		
		socketManager.SendCommand(StartButtonPressedCommand);
	}

	public void SendStartGame()
	{
		//отправляем команду на то что игру можно начинать
		NetworkCommand SendStartGameCommand = new NetworkCommand();
		
		SendStartGameCommand.keyCode = 101;
		SendStartGameCommand.PlayerNameN = null;
		SendStartGameCommand.BubbleNameToDestroy = null;
		SendStartGameCommand.CreatedBubble_Name = null;
		SendStartGameCommand.CreatedBubble_Scale = 0;
		SendStartGameCommand.CreatedBubble_Pos = null;
		SendStartGameCommand.CreatedBubble_Rot = null;
		
		socketManager.SendCommand(SendStartGameCommand);
	}

	public void SendBubbleCreated(string NewBubbleName, float NewBubbleScale, Vector3 NewBubblePosition, Quaternion NewBubbleRotation)
	{
		//отправляем каманду на создание шарика тех же размеров, в тех же кординатах и с тем же именем
		NetworkCommand SendBubbleCreatedCommand = new NetworkCommand();

		Vector3Serializer newVector = new Vector3Serializer();
		newVector.Fill(NewBubblePosition);

		QuaternionSerializer newRot = new QuaternionSerializer();
		newRot.Fill(NewBubbleRotation);

		SendBubbleCreatedCommand.keyCode = 102;
		SendBubbleCreatedCommand.PlayerNameN = null;
		SendBubbleCreatedCommand.BubbleNameToDestroy = null;
		SendBubbleCreatedCommand.CreatedBubble_Name = NewBubbleName;
		SendBubbleCreatedCommand.CreatedBubble_Scale = NewBubbleScale;
		SendBubbleCreatedCommand.CreatedBubble_Pos = null;
		SendBubbleCreatedCommand.CreatedBubble_Rot = null;
		
		socketManager.SendCommand(SendBubbleCreatedCommand);
	}

	public void SendBubbleWasPressed(string PressedBubbleName)
	{
		//отправляем каманду на уничтожение конекретного шарика
		NetworkCommand SendBubbleWasPressedCommand = new NetworkCommand();
		
		SendBubbleWasPressedCommand.keyCode = 104;
		SendBubbleWasPressedCommand.PlayerNameN = null;
		SendBubbleWasPressedCommand.BubbleNameToDestroy = PressedBubbleName;
		SendBubbleWasPressedCommand.CreatedBubble_Name = null;
		SendBubbleWasPressedCommand.CreatedBubble_Scale = 0;
		SendBubbleWasPressedCommand.CreatedBubble_Pos = null;
		SendBubbleWasPressedCommand.CreatedBubble_Rot = null;
		
		socketManager.SendCommand(SendBubbleWasPressedCommand);
	}

	public void SendPlayerName(string name)
	{
		//отправить имя игрока по сети
		NetworkCommand SendPlayerNameCommand = new NetworkCommand();

		SendPlayerNameCommand.keyCode = 103;
		SendPlayerNameCommand.PlayerNameN = name;
		SendPlayerNameCommand.BubbleNameToDestroy = null;
		SendPlayerNameCommand.CreatedBubble_Name = null;
		SendPlayerNameCommand.CreatedBubble_Scale = 0;
		SendPlayerNameCommand.CreatedBubble_Pos = null;
		SendPlayerNameCommand.CreatedBubble_Rot = null;

		socketManager.SendCommand(SendPlayerNameCommand);
	}
}

