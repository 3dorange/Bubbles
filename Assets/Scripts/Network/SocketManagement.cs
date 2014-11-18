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

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SocketManagement : MonoBehaviour 
{
	private IPAddress _IP ;							
	private int _PORT;					
	private TcpListener _TCP;			
	private TcpClient _CLIENT;			
	private NetworkStream _STREAM;

	private ManualResetEvent clientConnected = new ManualResetEvent(false);

	public SocketManagement(string ip_Adress,int port)
	{
		_IP = IPAddress.Parse(ip_Adress);		//переводим строчку в ip адресс
		_PORT = port;
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER","yes");
	}

	public void Disconnect()
	{
		//отключаем соединение
		_STREAM.Close();
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
			Debug.Log("StartAsServer error " + ex);
			return false; 
		}
		return true;
	}
	
	public TcpClient GetTcpClient() 
	{
		//ждем ответа от клиента
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
			Debug.Log("StartAsClient error" + ex);
			return false; 
		}
		return true;
	}

	public bool SendCommand(NetworkCommand CommandToSend) 
	{
		//отправляем сообщение в поток
		try
		{			
			BinaryFormatter formatter = new BinaryFormatter();

			try
			{
				formatter.Serialize(_STREAM,CommandToSend);
			}
			catch (SerializationException e)
			{
				Debug.Log(e);
			}
		}
		catch (Exception ex) 
		{ 
			Debug.Log("SendCommand error" + ex);
			return false; 
		}
		return true;
	}

	public NetworkCommand GetCommand() 
	{
		//забираем сообщение из потока
		NetworkCommand RecievedCommnad = new NetworkCommand();

		if (_STREAM.CanRead)
		{
			if (_STREAM.DataAvailable)
			{
				BinaryFormatter formatter = new BinaryFormatter();

				try
				{
					RecievedCommnad = (NetworkCommand) formatter.Deserialize(_STREAM);
				}
				catch (SerializationException e)
				{
					Debug.Log(e);
				}
			}
		}

		return RecievedCommnad;
	}
}

