using UnityEngine;
using System.Collections;

public class BubbleBroken : MonoBehaviour 
{
	//Класс отвечающий за разламывание шариков
	public GameObject[] Parts;					//кусочки шарика
	private ObjectPool pool;					//пул к которому принадлежит данный объект
	private float GeneratedTime = 0;			//время генерации
	private float timeToLive = 1.5f;			//время жизни

	void OnEnable()
	{
		GeneratedTime = Time.time;
		ResetParts();
		Boom();
	}

	void Update()
	{
		if (Time.time - GeneratedTime > timeToLive)
		{
			DestroyBubbleBoom();
		}
	}

	public void SetPool(ObjectPool p)
	{
		pool = p;
	}

	private void Boom()
	{
		//разрываем шарик на куски, летящие в рандомные направления
		for (int i = 0; i < Parts.Length;i++)
		{
			float power = Random.Range(50,200);
			Parts[i].rigidbody2D.AddForce(new Vector2(Random.Range(-1,1),Random.Range(0,1)*power));
		}
	}

	private void ResetParts()
	{
		transform.localScale = new Vector3(1,1,0.99f);

		for (int i = 0; i < Parts.Length;i++)
		{
			Parts[i].rigidbody2D.velocity = Vector2.zero;
			Parts[i].transform.localPosition = Vector3.zero;
		}
	}

	public void DestroyBubbleBoom()
	{
		pool.DeSpawn(this.name);
	}
}
