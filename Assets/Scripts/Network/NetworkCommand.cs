using UnityEngine;
using System.Collections;

[System.Serializable()]

public class NetworkCommand 
{
	//класс команд для передачи через сеть
	public int keyCode;								//код команды
	public string PlayerNameN;						//имя игрока(свое для посылки, чужое для получения)
	public string BubbleNameToDestroy;				//Имя шара, по которому кликнули и который нужно убрать
	public string CreatedBubble_Name;				//имя нового шара
	public float CreatedBubble_Scale;				//размер нового шара
	public Vector3Serializer CreatedBubble_Pos;		//положение нового шара
	public QuaternionSerializer CreatedBubble_Rot;	//вращение нового шара
	public int Score;								//кол-во очков

}
