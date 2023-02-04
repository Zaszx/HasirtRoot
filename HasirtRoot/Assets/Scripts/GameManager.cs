using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    ItemSelect,
    Game,
    GameOver,
}
public class GameManager : MonoBehaviour
{
    public Image girecekItem;
    public string leftPlayerInput;
    public string rightPlayerInput;

    public bool isLeftPlayerTurn = true;

	public TMP_Text leftPlayerInputText;
	public TMP_Text rightPlayerInputText;

	public TMP_Text leftPlayerSqrtText;
	public TMP_Text rightPlayerSqrtText;

	public GameObject gameOverObject;
	public TMP_Text gameOverText;

	int leftPlayerSqrtNumber;
	int rightPlayerSqrtNumber;


	public float girecekItemBaseSpeed = 10.0f;
	float girecekItemSpeed;

	GameState gameState = GameState.Game;

	void Start()
    {
		SwitchSides();
    }

	void GameOver()
	{
		if(isLeftPlayerTurn)
		{
			gameOverText.text = "Soldakine girdi";
		}
		else
		{
			gameOverText.text = "Sagdakine girdi";
		}

		gameOverObject.SetActive(true);
		gameState = GameState.GameOver;
	}

	void SwitchSides()
	{
		girecekItemSpeed = girecekItemBaseSpeed;
		leftPlayerInput = "";
		rightPlayerInput = "";

		UpdateTextInputs();

		isLeftPlayerTurn = !isLeftPlayerTurn;

		leftPlayerSqrtNumber = Random.Range(20, 80);
		rightPlayerSqrtNumber = Random.Range(20, 80);

		leftPlayerSqrtText.text = "" + leftPlayerSqrtNumber;
		rightPlayerSqrtText.text = "" + rightPlayerSqrtNumber;
	}

    void Update()
    {
		if(gameState == GameState.Game)
		{
			ProcessInput();
			girecekItem.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * girecekItemSpeed * (isLeftPlayerTurn ? -1 : 1);
			if(CheckGameOver())
			{
				GameOver();
			}
		}
		else if(gameState == GameState.GameOver)
		{
			SceneManager.LoadScene("game");
		}
    }

	bool CheckGameOver()
	{
		if(girecekItem.transform.position.x < 0 || girecekItem.transform.position.x > Screen.width)
		{
			return true;
		}
		return false;
	}

    void ProcessInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			leftPlayerInput += "0";
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			leftPlayerInput += "1";
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			leftPlayerInput += "2";
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			leftPlayerInput += "3";
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			leftPlayerInput += "4";
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			leftPlayerInput += "5";
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			leftPlayerInput += "6";
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			leftPlayerInput += "7";
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			leftPlayerInput += "8";
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			leftPlayerInput += "9";
		}
		
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			rightPlayerInput += "0";
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			rightPlayerInput += "1";
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			rightPlayerInput += "2";
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			rightPlayerInput += "3";
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			rightPlayerInput += "4";
		}
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			rightPlayerInput += "5";
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			rightPlayerInput += "6";
		}
		if (Input.GetKeyDown(KeyCode.Keypad7))
		{
			rightPlayerInput += "7";
		}
		if (Input.GetKeyDown(KeyCode.Keypad8))
		{
			rightPlayerInput += "8";
		}
		if (Input.GetKeyDown(KeyCode.Keypad9))
		{
			rightPlayerInput += "9";
		}

		if(leftPlayerInput.Length > 6)
		{
			leftPlayerInput = leftPlayerInput.Substring(0, 6);
		}
		if(rightPlayerInput.Length > 6)
		{
			rightPlayerInput = rightPlayerInput.Substring(0, 6);
		}

		UpdateTextInputs();

		if(Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			if(!isLeftPlayerTurn)
			{
				int rightPlayerInputNumber = int.Parse(rightPlayerInput);
				if (CheckInput(rightPlayerInputNumber, rightPlayerSqrtNumber))
				{
					SwitchSides();
				}
				else
				{
					rightPlayerInput = "";
					UpdateTextInputs();
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(isLeftPlayerTurn)
			{
				int leftPlayerInputNumber = int.Parse(leftPlayerInput);
				if(CheckInput(leftPlayerInputNumber, leftPlayerSqrtNumber))
				{
					SwitchSides();
				}
				else
				{
					leftPlayerInput = "";
					UpdateTextInputs();
				}
			}
		}
	}

	bool CheckInput(int input, int sqrtNumber)
	{
		float sqrt = Mathf.Sqrt((float)sqrtNumber);
		if(Mathf.Abs(sqrt - input) < 1)
		{
			return true;
		}
		return false;
	}

	void UpdateTextInputs()
	{
		leftPlayerInputText.text = leftPlayerInput;
		rightPlayerInputText.text = rightPlayerInput;
	}
}
