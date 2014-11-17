using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	//Класс отвечающий за получение команд от игрока и их последующую обработку
	private LevelManager levelManager;			//основной класс логики

	public void SetLevelManager(LevelManager levM)
	{
		levelManager = levM;
	}

	void Update()
	{
		GetMouseInputs();
	}

	private void GetMouseInputs()
	{
		//обработка нажатий мышки
		if (Input.GetMouseButtonDown(0))
		{
			MouseLeftButtonClicked();
		}
	}

	private void MouseLeftButtonClicked()
	{
		//Кликнули левой кнопкой мышки
		//Пускаем луч в координаты клика мышки
		//Проверяем что объект в который мы попали имеет нужный tag
		//вызываем функцию уничтожения у шарика
		//вместо GetComponent можно было бы использовать SendMessage

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit))
		{
			if (hit.collider.tag == "Bubble")
			{
				Bubble hitBubble = hit.collider.gameObject.GetComponent<Bubble>();

				if (!hitBubble.OtherPlayerIsOwner)
				{
					hitBubble.DestroyBubble(true);
				}
			}
		}
	}

}
