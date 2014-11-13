using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour 
{
	//класс отвечающий за поведение шариков
	private float ScaleFactor = 1;				//множитель размера
	private int Points = 100;					//кол-во очков за уничтожение шарика
	private float Speed = 2;					//скорость падения шарика
	private float MaxSpeed = 10;				//максимально допустимая скорость
	private bool CanMove = false;				//может ли шар начать движения, или должен висеть на месте
	public ObjectPool pool;					//пул к которому принадлежит данный объект

	void OnEnable()
	{
		GenerateBubble();
	}

	void Update()
	{
		if (CanMove)
		{
			MoveBubble();

			if (transform.position.y < -4.5f)
			{
				DestroyBubble();
			}
		}
	}

	public void SetPool(ObjectPool p)
	{
		pool = p;
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
	}

	private void GenerateBubble()
	{
		//функция которая генерит параметры шарика при его создании
		ResetToDefault();

		ScaleFactor = Random.Range(0.3f,1.0f);
		Points = (int) (Points/ScaleFactor);
		Speed = Speed/ScaleFactor;
		Speed = Mathf.Clamp(Speed,0.1f,MaxSpeed);
		CanMove = true;

		transform.localScale = new Vector3(transform.localScale.x*ScaleFactor,transform.localScale.y*ScaleFactor,transform.localScale.z*ScaleFactor);
	}

	public void DestroyBubble()
	{
		//функция уничтожения шарика, которая отключает шар
		CanMove = false;
		pool.DeSpawn(this.name);
	}
}
