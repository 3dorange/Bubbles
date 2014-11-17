using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour 
{
	//Скрипт отвечающий за общуюю логику игровой сцены
	public ObjectPool Bubbles_Pool;							//пул шаров, для генераци
	public ObjectPool Bubble_Boom_Pool;						//пул сломанных шаров
	public ObjectPool Other_Bubbles_Pool;							//пул шаров, для генераци
	public InputManager inputManager;						//мeнеджер ввода
	public GUIManager guiManager;							//менеджер интерфейса
	public NetworkManager networkManager;					//менеджер мультиплеера

	private int PointsEarned = 0;
	private int PlayerLives = 10;

	private bool GameStarted = false;

	public int DifficultyLevel = 0;									//уровень сложности игры
	private float TimeToWaitForNextBubble = 0;						//время между появлением следующего шарика
	private int BubblesToGenerateInWave = 0;						//кол-во шариков в волне

	public int ActiveBubbles = 0;									//кол-во шариков на экране в данный момент
	private float LastBubbleGenerateTime = 0;

	public AudioSource MusicSource;
	public Renderer GranizaRenderer;
	public TextureManager textureManager;	

	private List<Bubble> BubblesOnStage = new List<Bubble>();						//список шариков на экране
	public List<BubbleBroken> BubbleBoomsOnStage = new List<BubbleBroken>();		//список сломанных шариков на экране

	//network
	private bool StartPressed = false;
	private bool OtherIsReady = false;

	void Awake()
	{
		inputManager.SetLevelManager(this);		//указываем менеджеру ввода на этот класс
		Bubbles_Pool.SetLevelManager(this);
		Bubble_Boom_Pool.SetLevelManager(this);
		guiManager.SetLevelManager(this);
		networkManager.SetLevelManager(this);

		GranizaRenderer.sharedMaterial = StartSceneLogic.GranizaMat;
		GranizaRenderer.sharedMaterial.mainTexture = StartSceneLogic.GranizaTexture;

		MusicSource.clip = StartSceneLogic.MusicTrack;
		MusicSource.Play();
	}

	void Start()
	{
		ResetGame();
//		StartGame();
	}

	void Update()
	{
		if (GameStarted)
		{
			if (BubblesToGenerateInWave > 0)
			{
				if (Time.time - LastBubbleGenerateTime > TimeToWaitForNextBubble)
				{
					GenerateBubble();
				}
			}
		}
	}

	//
	public void OtherIsReadyRecieved()
	{
		OtherIsReady = true;

		if (StartPressed)
		{
			StartGame();
		}
	}

	public bool GetOtherIsReady()
	{
		return OtherIsReady;
	}

	public void StartButtonPressed()
	{
		StartPressed = true;
		
		if (OtherIsReady)
		{
			StartGame();
		}
	}
	//

	public void CheckForNextWave()
	{
		if (ActiveBubbles <= 0)
		{
			NextWave();
		}
	}

	private void NextWave()
	{
		//меняем уровень сложности
		DifficultyLevel++;

		if (DifficultyLevel >= 8)
		{
			Win();
			return;
		}
		else
		{
			guiManager.ActivatePlayerLevelChange(DifficultyLevel);
			textureManager.UpdateTextures(DifficultyLevel);
			SetDifficulty();
			UpdateMaterials();
		}
	}

	private void UpdateMaterials()
	{
		// обновляем материалы, так чтобы они соотвествовали новым текстурам
		for (int i = 0;i < BubblesOnStage.Count;i++)
		{
			BubblesOnStage[i].CheckMaterial();
		}

		for (int i = 0;i < BubbleBoomsOnStage.Count;i++)
		{
			BubbleBoomsOnStage[i].CheckMaterial();
		}
	}

	private void Win()
	{
		// все уровни успешно пройдены
	}

	private void GameOver()
	{
		// жизни закончились
	}

	private void GenerateBubble()
	{
		//спавним шарик из пула
		//обновляем время последней генерации шарика
		//уменьшаем оставшееся кол-во шариков для генеарции в текущей волне
		Bubbles_Pool.Spawn(new Vector3(Random.Range(-6,-0.7f),Random.Range(6,8.0f),0));
		LastBubbleGenerateTime = Time.time;
		BubblesToGenerateInWave--;
		ActiveBubbles++;
	}

	public void CreateNewOtherPlayerBubble(string NewBubbleName, float NewBubbleScale, Vector3 NewBubblePosition, Quaternion NewBubbleRotation)
	{
		//создаем новый шарик для другого игрока
		Vector3 PosToGenerate = new Vector3(NewBubblePosition.x + Screen.width/2,NewBubblePosition.y,NewBubblePosition.z);
		Other_Bubbles_Pool.Spawn(PosToGenerate,NewBubbleScale,NewBubbleRotation,NewBubbleName);
	}

	private void SetDifficulty()
	{
		//устанавливаем параметры генерации шариков в зависимости от сложности игры
		switch (DifficultyLevel)
		{
			case 0:
				TimeToWaitForNextBubble = 0.9f;
				BubblesToGenerateInWave = 5;
				break;
			case 1:
				TimeToWaitForNextBubble = 0.8f;
				BubblesToGenerateInWave = 10;
				break;
			case 2:
				TimeToWaitForNextBubble = 0.7f;
				BubblesToGenerateInWave = 20;
				break;
			case 3:
				TimeToWaitForNextBubble = 0.6f;
				BubblesToGenerateInWave = 30;
				break;
			case 4:
				TimeToWaitForNextBubble = 0.4f;
				BubblesToGenerateInWave = 40;
				break;
			case 5:
				TimeToWaitForNextBubble = 0.3f;
				BubblesToGenerateInWave = 40;
				break;
			case 6:
				TimeToWaitForNextBubble = 0.3f;
				BubblesToGenerateInWave = 50;
				break;
			case 7:
				TimeToWaitForNextBubble = 0.2f;
				BubblesToGenerateInWave = 60;
				break;

			default:
				TimeToWaitForNextBubble = 0.5f;
				BubblesToGenerateInWave = 5;
				break;
		}		
	}

	private void ResetGame()
	{
		//обнуляем параметры игры
		UpdatePlayerScore();
		UpdatePlayerLive();
	}

	public void AddPoints(int points)
	{
		PointsEarned +=points;					//добавляем очки за шар, к уже заработанным
		UpdatePlayerScore();
	}

	public void LooseLive()
	{
		//вычитаем 1 жизнь
		PlayerLives--;
		UpdatePlayerLive();
	}

	private void UpdatePlayerLive()
	{
		//обновляем кол-во очков игрока в интерфейсе
		guiManager.PlayerLives.text = "Lives : " + PlayerLives;
	}

	public List<Bubble> GetBubblesOnStage()
	{
		return BubblesOnStage;
	}
	
	public List<BubbleBroken> GetBubbleBoomsOnStage()
	{
		return BubbleBoomsOnStage;
	}

	private void UpdatePlayerScore()
	{
		//обновляем кол-во очков игрока в интерфейсе
		guiManager.PlayerScore.text = "Score : " + PointsEarned;
	}

	public void StartGame()
	{
		//Игра началась
		GameStarted = true;
		SetDifficulty();
	}
}
