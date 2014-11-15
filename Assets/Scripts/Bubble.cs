using UnityEngine;
using System.Collections;

public class Bubble : BubbleParent 
{
	//класс отвечающий за поведение шариков
	private float ScaleFactor = 1;				//множитель размера
	private int Points = 100;					//кол-во очков за уничтожение шарика
	private float Speed = 0.5f;					//скорость падения шарика
	private float MaxSpeed = 4;					//максимально допустимая скорость
	private bool CanMove = false;				//может ли шар начать движения, или должен висеть на месте


	void OnEnable()
	{
		GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat;

		if (pool)
		{
			CheckMaterial();
		}
		GenerateBubble();
	}

	void Update()
	{
		if (CanMove)
		{
			MoveBubble();

			if (transform.position.y < -4.5f)
			{
				DestroyBubble(false);
			}
		}
	}

	private void MoveBubble()
	{
		float PosY = transform.position.y - Speed*Time.deltaTime;
		transform.position = new Vector3(transform.position.x,PosY,transform.position.z);
	}

	private void ResetToDefault()
	{
		//устанавливаем значения параметров на дефолтные
		ScaleFactor = 1;				
		Points = 100;					
		Speed = 2;					
		CanMove = false;

		transform.localScale = new Vector3(1,1,0.99f);
		transform.GetComponent<SphereCollider>().radius = 0.6f;

		GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat;
	}
	
	override public void CheckMaterial()
	{
		//проверяем какой материал использовать
		if (pool.GetLevelManager().DifficultyLevel != WaveGenerated)
		{
			GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat_Old;
		}
		else
		{
			GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat;
		}
	}

	private void GenerateBubble()
	{
		//функция которая генерит параметры шарика при его создании
		ResetToDefault();

		transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f,360.0f), transform.forward) * transform.rotation;

		ScaleFactor = Random.Range(0.4f,1.0f);
		Points = (int) (Points/ScaleFactor);
		Speed = Speed/ScaleFactor;
		Speed = Mathf.Clamp(Speed,0.1f,MaxSpeed) + pool.GetLevelManager().DifficultyLevel * 0.1f;
		CanMove = true;

		transform.localScale = new Vector3(transform.localScale.x*ScaleFactor,transform.localScale.y*ScaleFactor,transform.localScale.z*ScaleFactor);

		pool.GetLevelManager().GetBubblesOnStage().Add(this);
	}

	public void DestroyBubble(bool addPointsOrNot)
	{
		//функция уничтожения шарика, которая отключает шар
		CanMove = false;

		if (addPointsOrNot)
		{
			pool.GetLevelManager().AddPoints(Points);
			pool.GetLevelManager().Bubble_Boom_Pool.Spawn(transform.position,transform.localScale,transform.localRotation);
		}
		else
		{
			pool.GetLevelManager().LooseLive();
		}
		pool.GetLevelManager().ActiveBubbles--;
		pool.GetLevelManager().CheckForNextWave();
		pool.GetLevelManager().GetBubblesOnStage().Remove(this);
		pool.DeSpawn(this.name);
	}
}
