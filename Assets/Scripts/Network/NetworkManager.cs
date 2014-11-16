using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.ComponentModel;


using System.Linq;

using System.Threading;

using System.Text.RegularExpressions;


public class NetworkManager : MonoBehaviour 
{
	//Класс отвечающий за мультиплеерную часть
	private LevelManager levelManager;
	private bool isServer = false;
	private SocketManagement socketManager;			//объект для соединения
	private bool connected = false;
	private float netWait = 0;						//время между запросом данных из потока
//	private float LastNetGetTime = 0;				//время последнего забора

	void Update()
	{
		if (connected)
		{
//			socketManager.GetCommand();
			StartCoroutine(Wait(netWait));
		}
	}

	private IEnumerator Wait(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
		socketManager.GetCommand();
	}

	public void SetLevelManager(LevelManager lM)
	{
		levelManager = lM;
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
			GameStart();
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
			GameStart();
//			connected = true;
		}
	}

	private void GameStart()
	{
		//начинаем игру
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

	public void SendStartGame()
	{
		//отправляем команду на то что игру можно начинать
	}

	public void SendBubbleCreated()
	{
		//отправляем каманду на создание шарика тех же размеров, в тех же кординатах и с тем же именем
	}

	public void SendBubbleWasPressed()
	{
		//отправляем каманду на уничтожение конекретного шарика
	}

	public void SendPlayerName(string name)
	{
		//отправить имя игрока по сети
		socketManager.SendCommand();
	}

}

