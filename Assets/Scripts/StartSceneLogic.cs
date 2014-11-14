using UnityEngine;
using System.Collections;

public class StartSceneLogic : MonoBehaviour 
{
	//Логики закрузки игры
	private bool NowGameCanStart = false;

	void Update()
	{
		if (NowGameCanStart)
		{
			LoadGameLevel();
			NowGameCanStart = false;
		}
	}

	private void LoadGameLevel()
	{
		//загружаем игровой уровень
		Application.LoadLevel(1);
	}

}
