using UnityEngine;
using System.Collections;

public class MaterialManager_deleteiT : MonoBehaviour {
	public Material mat1;
	public Material mat2;

	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);

	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		mat1 = StartSceneLogic.Diskmat;
		mat2 = StartSceneLogic.Diskmat_Old;
	}
}
