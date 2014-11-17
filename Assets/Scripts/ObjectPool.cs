using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour 
{
	//Класс для создания пула какиз-либо объектов
	public int ObjectNumber = 50;												// кол-во объектов в пуле
	public List<GameObject> ObjectInPool = new List<GameObject>();				//сами объкты
	public List<GameObject> ObjectPrefab;												//префаб того, что нужно хранить в пуле
	public int CurrentActiveNumber = 0;											//Кол-во текущих активных
	private int curN = 0;														//текущий номер
	private LevelManager levelManager;

	//варианты того, что может быть в пуле
	public enum ObjectType
	{
		Bubble,
		BubbleBoom,
		BubbleOtherPlayer
	}

	public ObjectType objectType;

	void Awake()
	{
		//добавляем вариенты префабов для генерации
		if (objectType == ObjectType.Bubble)
		{
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject1);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject2);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject3);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject4);
		}
		else if (objectType == ObjectType.BubbleBoom)
		{
			ObjectPrefab.Add(StartSceneLogic.BubbleBoomPrefabObject1);
			ObjectPrefab.Add(StartSceneLogic.BubbleBoomPrefabObject2);
			ObjectPrefab.Add(StartSceneLogic.BubbleBoomPrefabObject3);
			ObjectPrefab.Add(StartSceneLogic.BubbleBoomPrefabObject4);
		}
		else if (objectType == ObjectType.BubbleOtherPlayer)
		{
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject1);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject2);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject3);
			ObjectPrefab.Add(StartSceneLogic.BubblePrefabObject4);
		}
	}

	void Start()
	{
		CreateObjects();
	}

	public void SetLevelManager(LevelManager lM)
	{
		levelManager = lM;
	}

	public LevelManager GetLevelManager()
	{
		return levelManager;
	}

	private void CreateObjects()
	{
		//генерим объекты для пула
		for (int i = 0; i < ObjectNumber;i++)
		{
			CreateObject();
			curN++;
		}
	}
	
	private void CreateObject()
	{
		//Создаем объект
		//делаем его child-ом нашего пула
		//добавляем в список объектов пула и делаем неактивным
		int randomN = Random.Range(0,ObjectPrefab.Count);
		GameObject newObject = Instantiate(ObjectPrefab[randomN],new Vector3(500,500,0),ObjectPrefab[randomN].transform.rotation) as GameObject;
		newObject.transform.parent = this.transform;
		newObject.name = ObjectPrefab[randomN].name + "_" + curN;

		if (objectType == ObjectType.Bubble)
		{
			Bubble newBubble = newObject.GetComponent<Bubble>();
			newBubble.SetPool(this);

			if (objectType == ObjectType.BubbleOtherPlayer)
			{
				newBubble.OtherPlayerIsOwner = true;
			}
		}
		else if (objectType == ObjectType.BubbleBoom)
		{
			newObject.GetComponent<BubbleBroken>().SetPool(this);
		}

		ObjectInPool.Add(newObject);
		newObject.SetActive(false);
	}

	private GameObject GetObjectByName(string name)
	{
		//поиск объектов в спике по имени
		GameObject temp = null;

		for (int i = 0; i < ObjectInPool.Count;i++)
		{
			if (ObjectInPool[i].name == name)
			{
				return ObjectInPool[i];
			}
		}

		return temp;
	}

	private GameObject GetObjectToSpawn()
	{
		//Ищем объект для спауна среди еще неактивных
		GameObject temp = null;

		for (int i = 0; i < ObjectInPool.Count;i++)
		{
			if (!ObjectInPool[i].activeInHierarchy)
			{
				return ObjectInPool[i];
			}
		}

		return temp;
	}

	public void Spawn(Vector3 posToSpawn,Vector3 scaleToUse,Quaternion rot)
	{
		//спавним объект в заданных координатах (Для разломанных)
		if (CurrentActiveNumber < ObjectInPool.Count)
		{
			GameObject objectToSpawn = GetObjectToSpawn();

			if (objectToSpawn != null)
			{
				objectToSpawn.SetActive(true);
				objectToSpawn.transform.position = posToSpawn;
				objectToSpawn.transform.rotation = rot;
				objectToSpawn.transform.localScale = scaleToUse;

				if (objectType == ObjectType.BubbleBoom)
				{
					objectToSpawn.GetComponent<BubbleBroken>().SetWave(levelManager.DifficultyLevel);
				}
				CurrentActiveNumber++;
			}
		}
	}

	public void Spawn(Vector3 posToSpawn,float scaleFactor,Quaternion rot,string bubbleName)
	{
		//спавним объект в заданных координатах (Для шариков другого игрока)
		if (CurrentActiveNumber < ObjectInPool.Count)
		{
			GameObject objectToSpawn = GetObjectToSpawn();
			
			if (objectToSpawn != null)
			{
				if (objectType == ObjectType.BubbleOtherPlayer)
				{
					Bubble newBubble = objectToSpawn.GetComponent<Bubble>();

					objectToSpawn.SetActive(true);
					objectToSpawn.transform.position = posToSpawn;
					objectToSpawn.transform.rotation = rot;	

					newBubble.SetParametersFromNetwork(scaleFactor,bubbleName);
					newBubble.SetWave(levelManager.DifficultyLevel);
				}
				CurrentActiveNumber++;
			}
		}
	}

	public void Spawn(Vector3 posToSpawn)
	{
		//спавним объект в заданных координатах
		if (CurrentActiveNumber < ObjectInPool.Count)
		{
			GameObject objectToSpawn = GetObjectToSpawn();
			
			if (objectToSpawn != null)
			{
				objectToSpawn.SetActive(true);
				objectToSpawn.transform.position = posToSpawn;

				if (objectType == ObjectType.Bubble)
				{
					objectToSpawn.GetComponent<Bubble>().SetWave(levelManager.DifficultyLevel);
				}

				CurrentActiveNumber++;
			}
		}
	}

	public void DeSpawn(string ObjectName)
	{
		//деспавним объект
		GameObject objectToDespawn = GetObjectByName(ObjectName);

		if (objectToDespawn != null)
		{
			objectToDespawn.transform.position = new Vector3 (500,500,0);
			objectToDespawn.SetActive(false);
			CurrentActiveNumber--;
		}
		else
		{
			Debug.Log("DeSpawn working wrong");
		}
	}



}
