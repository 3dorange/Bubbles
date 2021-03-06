﻿using UnityEngine;
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

	public bool isSingle = false;					// работает ли мультиплеер или играем одни

	void Update()
	{
		if (!isSingle)
		{
			if (connected)
			{
				StartCoroutine(Wait(netWait));
			}
		}
	}

	private IEnumerator Wait(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
		GetCommands(socketManager.GetCommand());
	}

	public void ThisGameIsSingle()
	{
		//переходим в одиночный режим игры
		isSingle = true;
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

			case 101:
				RecievedOtherPlayerScore(recievedCommand.Score);
				break;

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

	private void RecievedOtherPlayerScore(int otherScore)
	{
		//получаем кол-во очков другого игрока
		levelManager.UpdateOtherPlayerScore(otherScore);
	}

	private void SetEnemyName(string enemyName)
	{
		//отправляем имя противника в интерфейс
		levelManager.guiManager.UpdateEnemysName(enemyName);
	}

	private void CreateNewEnemyBubble(string NewBubbleName, float NewBubbleScale, Vector3Serializer NewBubblePosition, QuaternionSerializer NewBubbleRotation)
	{
		//нужно создать новый шарик с пришедшеми параметрами
		if (!isSingle)
		{
			Vector3Serializer recievedPos = new Vector3Serializer();
			recievedPos = NewBubblePosition;

			Vector3 bubblePos = new Vector3(recievedPos.x,recievedPos.y,recievedPos.z);
			Quaternion bubbleRot = new Quaternion(NewBubbleRotation.x,NewBubbleRotation.y,NewBubbleRotation.z,NewBubbleRotation.w);

			levelManager.CreateNewOtherPlayerBubble(NewBubbleName,NewBubbleScale,bubblePos,bubbleRot);
		}
	}

	private void RemoveEnemyBubble(string PressedBubbleName)
	{
		//нужно убрать с пришедшеми именем
		if (!isSingle)
		{
			GameObject objectToRemove = levelManager.Other_Bubbles_Pool.GetByName(PressedBubbleName);

			if (objectToRemove)
			{
				objectToRemove.GetComponent<Bubble>().DestroyBubble(true,false);
			}
		}
	}

	public void ConnectAsServerPressed(string ip_Adress, string port)
	{
		//была нажата кнопка игры за сервер
		if (!isSingle)
		{
			if (checkIPandPort(ip_Adress, port))
			{
				isServer = true;
				ConnectAsServer(ip_Adress, Int32.Parse(port));
			}
		}
	}

	public void Disconnect()
	{
		//отключаемся
		if (!isSingle)
		{
			socketManager.Disconnect();
		}
	}

	public void ConnectAsClientPressed(string ip_Adress, string port)
	{
		//была нажата кнопка игры за клиент
		if (!isSingle)
		{
			if (checkIPandPort(ip_Adress, port))
			{
				isServer = true;
				ConnectAsClient(ip_Adress, Int32.Parse(port));
			}
		}
	}

	private void ConnectAsServer(string ip_Adress, int port)
	{
		//коннектимся как сервер
		if (!isSingle)
		{
			socketManager = new SocketManagement(ip_Adress,port);

			if (socketManager.StartAsServer())
			{
				//если удалось начать игру как сервер, то продолжаем
				connected = true;
			}
		}
	}

	private void ConnectAsClient(string ip_Adress, int port)
	{
		//была нажата кнопка игры как клиент
		if (!isSingle)
		{
			socketManager = new SocketManagement(ip_Adress,port);

			if (socketManager.StartAsClient())
			{
				//если удалось начать игру как клиент, то продолжаем	
				connected = true;
			}
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
		if (!isSingle)
		{
			NetworkCommand StartButtonPressedCommand = new NetworkCommand();
			
			StartButtonPressedCommand.keyCode = 100;
			StartButtonPressedCommand.PlayerNameN = null;
			StartButtonPressedCommand.BubbleNameToDestroy = null;
			StartButtonPressedCommand.CreatedBubble_Name = null;
			StartButtonPressedCommand.CreatedBubble_Scale = 0;
			StartButtonPressedCommand.CreatedBubble_Pos = null;
			StartButtonPressedCommand.CreatedBubble_Rot = null;
			StartButtonPressedCommand.Score = 0;

			socketManager.SendCommand(StartButtonPressedCommand);
		}
	}

	public void SendScore(int scoreToSend)
	{
		//отправляем очки
		if (!isSingle)
		{
			NetworkCommand SendScoreGameCommand = new NetworkCommand();
			
			SendScoreGameCommand.keyCode = 101;
			SendScoreGameCommand.PlayerNameN = null;
			SendScoreGameCommand.BubbleNameToDestroy = null;
			SendScoreGameCommand.CreatedBubble_Name = null;
			SendScoreGameCommand.CreatedBubble_Scale = 0;
			SendScoreGameCommand.CreatedBubble_Pos = null;
			SendScoreGameCommand.CreatedBubble_Rot = null;
			SendScoreGameCommand.Score = scoreToSend;

			socketManager.SendCommand(SendScoreGameCommand);
		}
	}

	public void SendBubbleCreated(string NewBubbleName, float NewBubbleScale, Vector3 NewBubblePosition, Quaternion NewBubbleRotation)
	{
		//отправляем каманду на создание шарика тех же размеров, в тех же кординатах и с тем же именем
		if (!isSingle)
		{
			NetworkCommand SendBubbleCreatedCommand = new NetworkCommand();

			Vector3Serializer newVector = new Vector3Serializer();
			newVector.Fill(NewBubblePosition);

			QuaternionSerializer newRot = new QuaternionSerializer();
			newRot.Fill(NewBubbleRotation);

			SendBubbleCreatedCommand.keyCode = 102;
			SendBubbleCreatedCommand.PlayerNameN = null;
			SendBubbleCreatedCommand.BubbleNameToDestroy = null;
			SendBubbleCreatedCommand.CreatedBubble_Name = NewBubbleName + "_OtherPlayer";
			SendBubbleCreatedCommand.CreatedBubble_Scale = NewBubbleScale;
			SendBubbleCreatedCommand.CreatedBubble_Pos = newVector;
			SendBubbleCreatedCommand.CreatedBubble_Rot = newRot;
			SendBubbleCreatedCommand.Score = 0;

			socketManager.SendCommand(SendBubbleCreatedCommand);
		}
	}

	public void SendBubbleWasPressed(string PressedBubbleName)
	{
		//отправляем каманду на уничтожение конекретного шарика
		if (!isSingle)
		{
			NetworkCommand SendBubbleWasPressedCommand = new NetworkCommand();
			
			SendBubbleWasPressedCommand.keyCode = 104;
			SendBubbleWasPressedCommand.PlayerNameN = null;
			SendBubbleWasPressedCommand.BubbleNameToDestroy = PressedBubbleName + "_OtherPlayer";
			SendBubbleWasPressedCommand.CreatedBubble_Name = null;
			SendBubbleWasPressedCommand.CreatedBubble_Scale = 0;
			SendBubbleWasPressedCommand.CreatedBubble_Pos = null;
			SendBubbleWasPressedCommand.CreatedBubble_Rot = null;
			SendBubbleWasPressedCommand.Score = 0;

			socketManager.SendCommand(SendBubbleWasPressedCommand);
		}
	}

	public void SendPlayerName(string name)
	{
		//отправить имя игрока по сети
		if (!isSingle)
		{
			NetworkCommand SendPlayerNameCommand = new NetworkCommand();

			SendPlayerNameCommand.keyCode = 103;
			SendPlayerNameCommand.PlayerNameN = name;
			SendPlayerNameCommand.BubbleNameToDestroy = null;
			SendPlayerNameCommand.CreatedBubble_Name = null;
			SendPlayerNameCommand.CreatedBubble_Scale = 0;
			SendPlayerNameCommand.CreatedBubble_Pos = null;
			SendPlayerNameCommand.CreatedBubble_Rot = null;
			SendPlayerNameCommand.Score = 0;

			socketManager.SendCommand(SendPlayerNameCommand);
		}
	}
}

