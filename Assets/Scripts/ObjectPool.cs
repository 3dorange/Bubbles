using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour 
{
	//Класс для создания пула какиз-либо объектов
	public int ObjectNumber = 50;												// кол-во объектов в пуле
	public List<GameObject> ObjectInPool = new List<GameObject>();				//сами объкты
	public GameObject ObjectPrefab;												//префаб того, что нужно хранить в пуле
	public int CurrentActiveNumber = 0;											//Кол-во текущих активных
	private int curN = 0;														//текущий номер
	private LevelManager levelManager;

	//варианты того, что может быть в пуле
	public enum ObjectType
	{
		Bubble,
		BubbleBoom
	}

	public ObjectType objectType;

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
		GameObject newObject = Instantiate(ObjectPrefab,new Vector3(500,500,0),ObjectPrefab.transform.rotation) as GameObject;
		newObject.transform.parent = this.transform;
		newObject.name = ObjectPrefab.name + "_" + curN;

		if (objectType == ObjectType.Bubble)
		{
			newObject.GetComponent<Bubble>().SetPool(this);
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

	public void Spawn(Vector3 posToSpawn,Vector3 scaleToUse)
	{
		//спавним объект в заданных координатах
		if (CurrentActiveNumber < ObjectInPool.Count)
		{
			GameObject objectToSpawn = GetObjectToSpawn();

			if (objectToSpawn != null)
			{
				objectToSpawn.SetActive(true);
				objectToSpawn.transform.position = posToSpawn;
				objectToSpawn.transform.localScale = scaleToUse;
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
