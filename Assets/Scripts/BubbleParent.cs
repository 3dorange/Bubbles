using UnityEngine;
using System.Collections;

public class BubbleParent : MonoBehaviour 
{
	protected int WaveGenerated = 0;			//в какой волне был создан
	protected ObjectPool pool;					//пул к которому принадлежит данный объект

	public void SetPool(ObjectPool p)
	{
		pool = p;
	}

	public void SetWave(int num)
	{
		WaveGenerated = num;
		CheckMaterial();
	}

	virtual public void CheckMaterial()
	{
		//проверяем какой материал использовать
	}

}
