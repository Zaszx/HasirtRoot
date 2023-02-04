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
	Switching,
	WrongInput,
	Giriyor,
	GameOver,
}
public class GameManager : MonoBehaviour
{
    public Image girecekItem;
    public string leftPlayerInput;
    public string rightPlayerInput;

    public bool isLeftPlayerTurn = true;

	public GameObject uiParent;
	public GameObject leftSqrtParentObject;
	public GameObject rightSqrtParentObject;

	public TMP_Text leftPlayerInputText;
	public TMP_Text rightPlayerInputText;

	public TMP_Text leftPlayerSqrtText;
	public TMP_Text rightPlayerSqrtText;

	public GameObject gameOverObject;
	public TMP_Text gameOverText;

	public AnimationCurve girmeAnimationCurve;

	int sqrtMinNumberBase = 20;
	int sqrtMaxNumberBase = 100;
	int sqrtMaxMaxNumber = 100000;
	int sqrtMaxMinNumber = 10000;

	int leftPlayerSqrtNumber;
	int rightPlayerSqrtNumber;


	public float girecekItemBaseSpeed = 10.0f;
	float girecekItemSpeed;

	GameState gameState = GameState.Game;

	void Start()
    {
		SwitchSides();
    }

	IEnumerator ScreenShake()
	{
		const float duration = 2f;
		var magnitude = 10f;
		var initPos = uiParent.transform.position;
		for (var f = 0f; f < duration; f += Time.deltaTime)
		{
			var r = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
			uiParent.transform.position = initPos + (r * magnitude * Mathf.Sign(Random.Range(-1f, 1f)));
			magnitude = Mathf.Lerp(10, 0, f / duration);
			yield return null;
		}

		uiParent.transform.position = initPos;

		GameOver();
	}

	IEnumerator GiriyorCoroutine()
	{
		gameState = GameState.Giriyor;
		float girmeXPos = isLeftPlayerTurn ? 0 : Screen.width;

		float currentTime = 0.0f;
		float totalTime = 1.0f;

		Vector3 initialPos = new Vector3(Screen.width * 0.5f, girecekItem.transform.position.y, girecekItem.transform.position.z);
		Vector3 targetPos = new Vector3(girmeXPos, initialPos.y, initialPos.z);

		while(currentTime < totalTime)
		{
			girecekItem.transform.position = Vector3.Lerp(initialPos, targetPos, girmeAnimationCurve.Evaluate(currentTime / totalTime));
			yield return new WaitForEndOfFrame();

			currentTime = currentTime + Time.deltaTime;
		}

		StartCoroutine(ScreenShake());
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

	IEnumerator WrongCoroutine()
	{
		gameState = GameState.WrongInput;

		if (isLeftPlayerTurn)
		{
			leftPlayerInputText.color = Color.red;
		}
		else
		{
			rightPlayerInputText.color = Color.red;
		}

		yield return new WaitForSeconds(0.4f);

		if (isLeftPlayerTurn)
		{
			leftPlayerInput = "";
			leftPlayerInputText.color = Color.black;
		}
		else
		{
			rightPlayerInput = "";
			rightPlayerInputText.color = Color.black;
		}

		girecekItemSpeed = girecekItemSpeed * 1.2f;

		UpdateTextInputs();

		gameState = GameState.Game;
	}

	IEnumerator SwitchCoroutine()
	{
		gameState = GameState.Switching;
		Vector3 initialScale = girecekItem.transform.localScale;
		Vector3 targetScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
		float currentTime = 0.0f;
		float totalTime = 1.0f;

		while(currentTime < totalTime)
		{
			girecekItem.transform.localScale = Vector3.Lerp(initialScale, targetScale, currentTime / totalTime);
			yield return new WaitForEndOfFrame();
			currentTime = currentTime + Time.deltaTime;
		}
		girecekItem.transform.localScale = targetScale;
		SwitchSides();
		gameState = GameState.Game;
	}


	void SwitchSides()
	{
		girecekItemSpeed = girecekItemBaseSpeed;
		leftPlayerInput = "";
		rightPlayerInput = "";

		UpdateTextInputs();

		isLeftPlayerTurn = !isLeftPlayerTurn;

		sqrtMinNumberBase = (int)((float)sqrtMinNumberBase * 1.5f);
		sqrtMaxNumberBase = (int)((float)sqrtMaxNumberBase * 1.5f);

		if (sqrtMaxNumberBase > sqrtMaxMaxNumber)
			sqrtMaxNumberBase = sqrtMaxMaxNumber;
		if (sqrtMinNumberBase > sqrtMaxMinNumber)
			sqrtMinNumberBase = sqrtMaxMinNumber;

		leftPlayerSqrtNumber = Random.Range(sqrtMinNumberBase, sqrtMaxNumberBase);
		rightPlayerSqrtNumber = Random.Range(sqrtMinNumberBase, sqrtMaxNumberBase);

		leftPlayerSqrtText.text = "" + leftPlayerSqrtNumber;
		rightPlayerSqrtText.text = "" + rightPlayerSqrtNumber;

		leftPlayerInputText.color = Color.black;
		rightPlayerInputText.color = Color.black;
	}

    void Update()
    {
		if(gameState == GameState.Game || gameState == GameState.WrongInput)
		{
			ProcessInput();
			girecekItem.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * girecekItemSpeed * (isLeftPlayerTurn ? -1 : 1);
			if(CheckGameOver())
			{
				StopAllCoroutines();
				StartCoroutine(GiriyorCoroutine());
			}
		}
		else if(gameState == GameState.GameOver)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				SceneManager.LoadScene("game");
			}
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
		if (gameState != GameState.WrongInput || !isLeftPlayerTurn)
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
		}

		if (gameState != GameState.WrongInput || isLeftPlayerTurn)
		{ 
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

		if(Input.GetKeyDown(KeyCode.KeypadEnter) && rightPlayerInput.Length > 0)
		{
			if(!isLeftPlayerTurn)
			{
				int rightPlayerInputNumber = int.Parse(rightPlayerInput);
				if (CheckInput(rightPlayerInputNumber, rightPlayerSqrtNumber))
				{
					rightPlayerInputText.color = Color.green;
					StartCoroutine(SwitchCoroutine());
				}
				else
				{
					StartCoroutine(WrongCoroutine());
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Return) && leftPlayerInput.Length > 0)
		{
			if(isLeftPlayerTurn)
			{
				int leftPlayerInputNumber = int.Parse(leftPlayerInput);
				if(CheckInput(leftPlayerInputNumber, leftPlayerSqrtNumber))
				{
					leftPlayerInputText.color = Color.green;
					StartCoroutine(SwitchCoroutine());
				}
				else
				{
					StartCoroutine(WrongCoroutine());
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
