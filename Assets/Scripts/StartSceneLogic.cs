using UnityEngine;
using System.Collections;
using System;

public class StartSceneLogic : MonoBehaviour 
{
	//Логики закрузки игры
	public static GameObject BubblePrefabObject1;				//префаб шарика
	public static GameObject BubblePrefabObject2;				//префаб шарика
	public static GameObject BubblePrefabObject3;				//префаб шарика
	public static GameObject BubblePrefabObject4;				//префаб шарика

	public static GameObject BubbleBoomPrefabObject1;			//префаб поломанного шарика
	public static GameObject BubbleBoomPrefabObject2;			//префаб поломанного шарика
	public static GameObject BubbleBoomPrefabObject3;			//префаб поломанного шарика
	public static GameObject BubbleBoomPrefabObject4;			//префаб поломанного шарика

	public static Texture GranizaTexture;						//текстура границы посередине

	public static Material Diskmat;								//материал шаров
	public static Material GranizaMat;							//материал границы посередине

	public static AudioClip MusicTrack;							//музыка
	public static AudioClip BoomTrack;							//звук тыкания по шарикам

	private void Start () 
	{
		DontDestroyOnLoad(gameObject);																				//не удаляем объект, так как потом будем ссылаться на его объекты

		StartCoroutine(DownloadBundle("file://" + Application.dataPath + "/AssetBundles/GameResourses.unity3d"));	//загружаем бандл
	}

	private IEnumerator DownloadBundle (string urlPath)
	{
		using (WWW www = new WWW(urlPath))
		{
			yield return www;

			if (www.error != null)
			{
				throw new Exception("WWW download had an error:" + www.error);									//выводим ошибку загрузки в случае каких либо проблем
			}

			AssetBundle newBundle = www.assetBundle;

			//выгружаем нужные нам ассеты
			AssetBundleRequest request = newBundle.LoadAsync("Bubble_1", typeof(GameObject));
			yield return request;
			BubblePrefabObject1 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_2", typeof(GameObject));
			yield return request;
			BubblePrefabObject2 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_3", typeof(GameObject));
			yield return request;
			BubblePrefabObject3 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_4", typeof(GameObject));
			yield return request;
			BubblePrefabObject4 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_Broken_1", typeof(GameObject));
			yield return request;
			BubbleBoomPrefabObject1 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_Broken_2", typeof(GameObject));
			yield return request;
			BubbleBoomPrefabObject2 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_Broken_3", typeof(GameObject));
			yield return request;
			BubbleBoomPrefabObject3 = request.asset as GameObject;

			request = newBundle.LoadAsync("Bubble_Broken_4", typeof(GameObject));
			yield return request;
			BubbleBoomPrefabObject4 = request.asset as GameObject;

			request = newBundle.LoadAsync("graniza", typeof(Texture));
			yield return request;
			GranizaTexture = request.asset as Texture;

			request = newBundle.LoadAsync("Disk", typeof(Material));
			yield return request;
			Diskmat = request.asset as Material;

			request = newBundle.LoadAsync("Graniza", typeof(Material));
			yield return request;
			GranizaMat = request.asset as Material;

			request = newBundle.LoadAsync("black_hole_rem", typeof(AudioClip));
			yield return request;
			MusicTrack = request.asset as AudioClip;

			request = newBundle.LoadAsync("impact_van_panel", typeof(AudioClip));
			yield return request;
			BoomTrack = request.asset as AudioClip;

			newBundle.Unload(false);	//освобождаем память после бандла

			LoadGameLevel();
		}
	}

	private void LoadGameLevel()
	{
		//загружаем игровой уровень
		Application.LoadLevel(1);
	}

}
