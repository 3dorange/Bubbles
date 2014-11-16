using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class SocketManagement : MonoBehaviour 
{
	private IPAddress _IP ;				
	private int _PORT;					
	private TcpListener _TCP;			
	private TcpClient _CLIENT;			
	private NetworkStream _STREAM;		

	public SocketManagement(string ip_Adress,int port)
	{
		_IP = IPAddress.Parse(ip_Adress);		//переводим строчку в ip адресс
		_PORT = port;
	}

	public bool StartAsServer() 
	{
		//начинаем как сервер
		try
		{
			_TCP = new TcpListener(_IP, _PORT);
			_TCP.Start();
			_CLIENT = GetTcpClient();
			_STREAM = _CLIENT.GetStream();
		}
		catch (Exception ex) 
		{ 
			Debug.Log("StartAsServer error");
			return false; 
		}
		return true;
	}
	
	public TcpClient GetTcpClient() 
	{
		Debug.Log("GetTcpClient");
		return _TCP.AcceptTcpClient();
	}

	public bool StartAsClient() 
	{
		//начинаем как клиент
		try 
		{
			_CLIENT = new TcpClient();
			_CLIENT.Connect(_IP, _PORT);
			_STREAM = _CLIENT.GetStream();
		}
		catch (Exception ex) 
		{ 
			Debug.Log("StartAsClient error");
			return false; 
		}
		return true;
	}

	public bool SendCommand() 
	{
		//отправляем сообщение в поток
		try
		{
			string temp = "Test Message 100143445";
			Debug.Log("SendCommand");	
			byte[] bytes = new byte[255];
			bytes = new ASCIIEncoding().GetBytes(temp);
			_STREAM.Write(bytes, 0, bytes.Length);
		}
		catch (Exception ex) 
		{ 
			Debug.Log("SendCommand error");
			return false; 
		}
		return true;
	}

	public void GetCommand() 
	{
		//забираем сообщение из потока
		if (_STREAM.DataAvailable)
		{
			byte[] bytes = new byte[255];
			_STREAM.Read(bytes,0,bytes.Length);

			string temp = new ASCIIEncoding().GetString(bytes);
			char[] charOfTemp = temp.ToCharArray();

			Debug.Log("Get Command :" + temp);
		}
	}
}

