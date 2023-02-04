using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameState
{
    ItemSelect,
	Countdown,
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

	public TMP_Text countdownText;

	public TMP_Text leftPlayerCorrectAnswerText;
	public TMP_Text rightPlayerCorrectAnswerText;

	public TMP_Text leftPlayerInputText;
	public TMP_Text rightPlayerInputText;

	public TMP_Text leftPlayerSqrtText;
	public TMP_Text rightPlayerSqrtText;

	public GameObject gameOverObject;
	public TMP_Text gameOverText;

	public AnimationCurve girmeAnimationCurve;

	public Image leftIndicator;
	public Image rightIndicator;

	public Sprite upSprite;
	public Sprite downSprite;

	int sqrtMinNumberBase = 20;
	int sqrtMaxNumberBase = 40;
	int sqrtMaxMaxNumber = 100000;
	int sqrtMaxMinNumber = 10000;

	int leftPlayerSqrtNumber;
	int rightPlayerSqrtNumber;


	public float girecekItemBaseSpeed = 10.0f;
	float girecekItemSpeed;

	GameState gameState = GameState.ItemSelect;

	bool hasirtRootDone = false;

	public GameObject wheel;
	public GameObject spinParent;
	public AnimationCurve spinCurve;

	public IEnumerator Spin()
	{
		girecekItem.gameObject.SetActive(false);
		wheel.SetActive(true);
		var images = new List<Image>();

		// Populate wheel
		int previousRandom = -1;
		for (int i = 0; i < 100; i++)
		{
			int random = previousRandom;
			while(random == previousRandom)
			{
				random = Random.Range(0, Items.items.Length);
			}
			var itemPrefab = Items.items[random];
			previousRandom = random;
			var item = GameObject.Instantiate(itemPrefab);
			item.transform.SetParent(spinParent.transform, true);
			images.Add(item.GetComponent<Image>());
		}

		const float spinTime = 5f;
		var accumulatedTime = 0f;
		while (accumulatedTime < spinTime)
		{
			spinParent.transform.position -=
				spinCurve.Evaluate(accumulatedTime / spinTime) * Vector3.right * 10;

			accumulatedTime += Time.deltaTime;

			yield return null;
		}

		var mid = Screen.width / 2;
		var selectedImage =
			images.Aggregate(
				(curImage, next) => Mathf.Abs(curImage.transform.position.x - mid) < Mathf.Abs(next.transform.position.x - mid)
					? curImage
					: next);

		foreach (var image in images)
		{
			if (image != selectedImage)
			{
				image.CrossFadeAlpha(0, 2, true);
				GameObject.Destroy(image.gameObject, 2);
			}
		}

		float currentTime = 0;
		float totalTime = 1.0f;

		Vector3 initialPos = selectedImage.transform.position;
		Vector3 targetPos = girecekItem.transform.position;

		float initialZRot = 0;
		float targetZRot = 90.0f;

		while(currentTime < totalTime)
		{
			selectedImage.transform.position = Vector3.Lerp(initialPos, targetPos, currentTime / totalTime);
			float rot = Mathf.Lerp(initialZRot, targetZRot, currentTime / totalTime);
			Quaternion qrot = Quaternion.Euler(0, 0, rot);
			selectedImage.transform.rotation = qrot;
			yield return new WaitForEndOfFrame();
			currentTime = currentTime + Time.deltaTime;
		}
		girecekItem.sprite = selectedImage.sprite;
		girecekItem.gameObject.SetActive(true);

		wheel.SetActive(false);
		Reset(false);
		yield return new WaitForSeconds(0.5f);
		gameState = GameState.Countdown;
		StartCoroutine(CountdownCoroutine());
	}

	void Start()
    {
		StartCoroutine(Spin());
		//SwitchSides();
    }

	IEnumerator CountdownCoroutine()
	{
		leftSqrtParentObject.SetActive(false);
		rightSqrtParentObject.SetActive(false);
		for (int i = 3; i > 0; i--)
		{
			countdownText.text = "" + i;
			yield return new WaitForSeconds(1f);
		}
		leftSqrtParentObject.SetActive(true);
		rightSqrtParentObject.SetActive(true);
		gameState = GameState.Game;
		countdownText.text = "GO!";
		yield return new WaitForSeconds(1f);
		countdownText.gameObject.SetActive(false);
	}

	IEnumerator ScreenShake()
	{
		const float duration = 3f;
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

		leftPlayerCorrectAnswerText.gameObject.SetActive(true);
		rightPlayerCorrectAnswerText.gameObject.SetActive(true);

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
			gameOverText.text = "Right player wins";
		}
		else
		{
			gameOverText.text = "Left player wins";
		}

		gameOverObject.SetActive(true);
		gameState = GameState.GameOver;
	}

	IEnumerator WrongCoroutine()
	{
		gameState = GameState.WrongInput;

		if (isLeftPlayerTurn)
		{
			float realAnswer = Mathf.Sqrt(leftPlayerSqrtNumber);
			int leftInput = int.Parse(leftPlayerInput);
			if (leftInput < realAnswer)
				leftIndicator.sprite = upSprite;
			else
				leftIndicator.sprite = downSprite;
			leftIndicator.gameObject.SetActive(true);
			leftPlayerInputText.color = Color.red;
		}
		else
		{
			float realAnswer = Mathf.Sqrt(rightPlayerSqrtNumber);
			int rightInput = int.Parse(rightPlayerInput);
			if (rightInput < realAnswer)
				rightIndicator.sprite = upSprite;
			else
				rightIndicator.sprite = downSprite;
			rightIndicator.gameObject.SetActive(true);
			rightPlayerInputText.color = Color.red;
		}

		yield return new WaitForSeconds(0.4f);

		if (isLeftPlayerTurn)
		{
			leftPlayerInput = "";
			leftPlayerInputText.color = Color.black;
			leftIndicator.gameObject.SetActive(false);
		}
		else
		{
			rightPlayerInput = "";
			rightPlayerInputText.color = Color.black;
			rightIndicator.gameObject.SetActive(false);
		}

		girecekItemSpeed = girecekItemSpeed * 1.2f;

		UpdateTextInputs();

		gameState = GameState.Game;
	}

	IEnumerator SwitchCoroutine()
	{
		gameState = GameState.Switching;
		Vector3 initialScale = girecekItem.transform.localScale;
		Vector3 targetScale = new Vector3(initialScale.x, -initialScale.y, initialScale.z);
		float currentTime = 0.0f;
		float totalTime = 1.0f;

		leftPlayerCorrectAnswerText.gameObject.SetActive(true);
		rightPlayerCorrectAnswerText.gameObject.SetActive(true);

		while (currentTime < totalTime)
		{
			girecekItem.transform.localScale = Vector3.Lerp(initialScale, targetScale, currentTime / totalTime);
			yield return new WaitForEndOfFrame();
			currentTime = currentTime + Time.deltaTime;
		}
		girecekItem.transform.localScale = targetScale;
		Reset(true);
		gameState = GameState.Game;
	}

	void Reset(bool switchSides)
	{
		girecekItemSpeed = girecekItemBaseSpeed;
		leftPlayerInput = "";
		rightPlayerInput = "";

		leftPlayerCorrectAnswerText.gameObject.SetActive(false);
		rightPlayerCorrectAnswerText.gameObject.SetActive(false);

		hasirtRootDone = false;

		UpdateTextInputs();

		if(switchSides)
			isLeftPlayerTurn = !isLeftPlayerTurn;

		sqrtMinNumberBase = (int)((float)sqrtMinNumberBase * 1.5f);
		sqrtMaxNumberBase = (int)((float)sqrtMaxNumberBase * 1.5f);

		if (sqrtMaxNumberBase > sqrtMaxMaxNumber)
			sqrtMaxNumberBase = sqrtMaxMaxNumber;
		if (sqrtMinNumberBase > sqrtMaxMinNumber)
			sqrtMinNumberBase = sqrtMaxMinNumber;

		leftPlayerSqrtNumber = Random.Range(sqrtMinNumberBase, sqrtMaxNumberBase);
		rightPlayerSqrtNumber = Random.Range(sqrtMinNumberBase, sqrtMaxNumberBase);

		leftPlayerCorrectAnswerText.text = "" + Mathf.Sqrt(leftPlayerSqrtNumber);
		if (leftPlayerCorrectAnswerText.text.Length > 7)
			leftPlayerCorrectAnswerText.text = leftPlayerCorrectAnswerText.text.Substring(0, 7);

		rightPlayerCorrectAnswerText.text = "" + Mathf.Sqrt(rightPlayerSqrtNumber);
		if (rightPlayerCorrectAnswerText.text.Length > 7)
			rightPlayerCorrectAnswerText.text = rightPlayerCorrectAnswerText.text.Substring(0, 7);

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
		if ((gameState != GameState.WrongInput || !isLeftPlayerTurn) && !(!isLeftPlayerTurn && hasirtRootDone))
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

		if ((gameState != GameState.WrongInput || isLeftPlayerTurn) && !(isLeftPlayerTurn && hasirtRootDone))
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
			int rightPlayerInputNumber = int.Parse(rightPlayerInput);
			if (CheckInput(rightPlayerInputNumber, rightPlayerSqrtNumber))
			{
				if(!isLeftPlayerTurn)
				{
					rightPlayerInputText.color = Color.green;
					StartCoroutine(SwitchCoroutine());
				}
				else
				{
					girecekItemSpeed *= 3;
					rightPlayerInputText.color = Color.green;
					hasirtRootDone = true;
					rightPlayerCorrectAnswerText.gameObject.SetActive(true);
				}
			}
			else
			{
				if (!isLeftPlayerTurn)
				{
					StartCoroutine(WrongCoroutine());
				}
				else
				{
					rightPlayerInputText.color = Color.red;
					hasirtRootDone = true;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Return) && leftPlayerInput.Length > 0)
		{
			int leftPlayerInputNumber = int.Parse(leftPlayerInput);
			if (CheckInput(leftPlayerInputNumber, leftPlayerSqrtNumber))
			{
				if (isLeftPlayerTurn)
				{
					leftPlayerInputText.color = Color.green;
					StartCoroutine(SwitchCoroutine());
				}
				else
				{
					girecekItemSpeed *= 3;
					leftPlayerInputText.color = Color.green;
					hasirtRootDone = true;
					leftPlayerCorrectAnswerText.gameObject.SetActive(true);
				}
			}
			else
			{
				if (isLeftPlayerTurn)
				{
					StartCoroutine(WrongCoroutine());
				}
				else
				{
					leftPlayerInputText.color = Color.red;
					hasirtRootDone = true;
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
