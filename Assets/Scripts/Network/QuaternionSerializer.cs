using UnityEngine;
using System.Collections;

[System.Serializable()]

public class QuaternionSerializer 
{
	//эмуляция кватарниона для нужнд сериализации
	public float x;
	public float y;
	public float z;
	public float w;
	
	public void Fill(Quaternion q)
	{
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}
	
	public Quaternion Q
	{ 
		get 
		{ 
			return new Quaternion(x, y, z, w); 
		}
		set 
		{ 
			Fill(value); 
		}
	}
}
