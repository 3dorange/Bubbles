using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour 
{
	//Скрипт отвечающий за общуюю логику игровой сцены
	public ObjectPool Bubbles_Pool;							//пул шаров, для генераци
	public ObjectPool Bubble_Boom_Pool;						//пул сломанных шаров
	public InputManager inputManager;						//мeнеджер ввода
	public GUIManager guiManaher;							//менеджер интерфейса

	public int PointsEarned = 0;

	void Start()
	{
		inputManager.SetLevelManager(this);		//указываем менеджеру ввода на этот класс
		GameStarted();
	}

	public void AddPoints(int points)
	{
		PointsEarned +=points;					//добавляем очки за шар, к уже заработанным
	}

	private void UpdatePlayerScore()
	{
		//обновляем кол-во очков игрока в интерфейсе
		guiManaher.PlayerScore.text = "Score : " + PointsEarned;
	}

	private void GameStarted()
	{
		//Игра началась
		for (int i = 0; i < 10;i++)
		{
			Bubbles_Pool.Spawn(new Vector3(Random.Range(-6,-0.3f),Random.Range(6,8.0f),0));
		}
	}
}
