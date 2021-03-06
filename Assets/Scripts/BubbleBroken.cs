﻿using UnityEngine;
using System.Collections;

public class BubbleBroken : BubbleParent 
{
	//Класс отвечающий за разламывание шариков
	public GameObject[] Parts;					//кусочки шарика

	private float GeneratedTime = 0;			//время генерации
	private float timeToLive = 1.5f;			//время жизни
	private AudioSource boomSound;

	void Awake()
	{
		boomSound = GetComponent<AudioSource>();
		boomSound.clip = StartSceneLogic.BoomTrack;

		for (int i = 0; i < Parts.Length;i++)
		{
			Parts[i].GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat;
		}
	}

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

	override public void CheckMaterial()
	{
		//проверяем какой материал использовать
		if (pool.GetLevelManager().DifficultyLevel != WaveGenerated)
		{
			for (int i = 0; i < Parts.Length;i++)
			{
				Parts[i].GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat_Old;
			}
		}
		else
		{
			for (int i = 0; i < Parts.Length;i++)
			{
				Parts[i].GetComponent<Renderer>().sharedMaterial = StartSceneLogic.Diskmat;
			}
		}
	}

	private void Boom()
	{
		//разрываем шарик на куски, летящие в рандомные направления
		if (pool)
		{
			pool.GetLevelManager().GetBubbleBoomsOnStage().Add(this);
		}

		for (int i = 0; i < Parts.Length;i++)
		{
			float power = Random.Range(100,400);
			Parts[i].rigidbody2D.AddForce(new Vector2(Random.Range(-1,1),Random.Range(0,1)*power));
		}
		boomSound.Play();
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
		pool.GetLevelManager().GetBubbleBoomsOnStage().Remove(this);
		pool.DeSpawn(this.name);
	}
}
