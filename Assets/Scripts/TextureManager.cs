using UnityEngine;
using System.Collections;
using System;

public class TextureManager : MonoBehaviour 
{
	//класс отвечающий за генерацию и удаление неиспользуемых текстур для шариков
	public Texture2D CurrentTexture = null;					//текущая активная текстура
	public Texture2D OldTexture = null;					//следующая текстура

	public Color[] ColorSet1;
	public Color[] ColorSet2;
	public Color[] ColorSet3;
	public Color[] ColorSet4;
	public Color[] ColorSet5;
	public Color[] ColorSet6;
	public Color[] ColorSet7;
	public Color[] ColorSet8;

	private int CurrentLevel = 0;

	void Awake()
	{
		CurrentTexture = CreateTexture(0);

		StartSceneLogic.Diskmat.mainTexture = CurrentTexture;
		StartSceneLogic.Diskmat_Old.mainTexture = OldTexture;
	}

	public Texture2D GetCurrentTexture()
	{
		return CurrentTexture;
	}

	public Texture2D GetOldTexture()
	{
		return OldTexture;
	}

	public void UpdateTextures(int levelNumber)
	{
		//обновляем текстуры у материалов
		OldTexture = CurrentTexture;
		CurrentTexture = CreateTexture(levelNumber);

		StartSceneLogic.Diskmat.mainTexture = CurrentTexture;
		StartSceneLogic.Diskmat_Old.mainTexture = OldTexture;
	}

	private Texture2D CreateTexture(int levelNumber)
	{
		Texture2D tempTexture = null;
		CurrentLevel = levelNumber;

		tempTexture = GenerateTextureAtlas(128);

		//выгружаем неиспользуемые текстуры
		Resources.UnloadUnusedAssets();

		return	tempTexture;
	}

	private Color32 GetColor(int x,int y, int size)
	{
		//функция возвращает цвет
		//назначаем цвет на основе того что получили
		Color Color1;
		Color Color2;
		Color Color3;
		Color Color4;

		switch(CurrentLevel)
		{
		case 0:
			Color1 = ColorSet1[0];
			Color2 = ColorSet1[1];
			Color3 = ColorSet1[2];
			Color4 = ColorSet1[3];
			break;
		case 1:
			Color1 = ColorSet2[0];
			Color2 = ColorSet2[1];
			Color3 = ColorSet2[2];
			Color4 = ColorSet2[3];
			break;
		case 2:
			Color1 = ColorSet3[0];
			Color2 = ColorSet3[1];
			Color3 = ColorSet3[2];
			Color4 = ColorSet3[3];
			break;
		case 3:
			Color1 = ColorSet4[0];
			Color2 = ColorSet4[1];
			Color3 = ColorSet4[2];
			Color4 = ColorSet4[3];
			break;
		case 4:
			Color1 = ColorSet5[0];
			Color2 = ColorSet5[1];
			Color3 = ColorSet5[2];
			Color4 = ColorSet5[3];
			break;
		case 5:
			Color1 = ColorSet6[0];
			Color2 = ColorSet6[1];
			Color3 = ColorSet6[2];
			Color4 = ColorSet6[3];
			break;
		case 6:
			Color1 = ColorSet7[0];
			Color2 = ColorSet7[1];
			Color3 = ColorSet7[2];
			Color4 = ColorSet7[3];
			break;
		case 7:
			Color1 = ColorSet8[0];
			Color2 = ColorSet8[1];
			Color3 = ColorSet8[2];
			Color4 = ColorSet8[3];
			break;
		default:
			Color1 = ColorSet8[0];
			Color2 = ColorSet8[1];
			Color3 = ColorSet8[2];
			Color4 = ColorSet8[3];
			break;
		}

		if (x >= size/2 && y >= size/2)
		{
			return Color1;
		}
		else if (x < size/2 && y >= size/2)
		{
			return Color2;
		}
		else if (x >= size/2 && y < size/2)
		{
			return Color3;
		}
		else if (x < size/2 && y < size/2)
		{
			return Color4;
		}

		return Color.black;
	}

	private Texture2D GenerateTextureAtlas (int size)
	{
		//создаем текстуру
		Texture2D newTexture = new Texture2D(size, size, TextureFormat.RGB24,false);
		//делим текстуру на 4 квадрата и заливаем их 4 цветами в зависимости от уровня
		for (int x = -size; x < size;x++)
		{
			for (int y = -size; y < size;y++)
			{
				newTexture.SetPixel(x,y,GetColor(x,y,size));

				//генерим черные полоски
				if (x%12 == 0)
				{
					newTexture.SetPixel(x,y,Color.black);
				}
			}
		}
		
		newTexture.Apply();
		
		return newTexture;
	}
}
