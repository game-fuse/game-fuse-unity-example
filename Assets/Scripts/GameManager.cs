using System;
using System.Collections.Generic;
using GameFuseCSharp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject hurdlePrefab;
    public PlayerController playerController;

    [Header("API")]
    public string gameID;
    public string gameToken;

    [Header("Sign In")]
    public TMP_InputField signInEmail;
    public TMP_InputField signInPassword;
    public TextMeshProUGUI signInErrorText;


    [Header("Sign Up")]
    public TMP_InputField signUpUsername;
    public TMP_InputField signUpEmail;
    public TMP_InputField signUpPassword;
    public TMP_InputField signUpCPassword;
    public TextMeshProUGUI signUpErrorText;

    [Header("Leaderboard")]
    public TextMeshProUGUI leaderboardListsText;

    [Header("Shop")]
    public TextMeshProUGUI creditsText;
    public GameObject[] storeItems;
    public GameObject items;


    [Header("Game")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameText;

    [Header("Screens - All UI Screens (SignIn, SignUp, Main, Leaderboard, Shop, Game)")]
    public GameObject[] screens;

    private bool isConnected = false;
    private bool canGameStart = false;
    private bool isGameStarted = false;
    private int score = 0;
    private bool isGameOver = false;
    private List<GameObject> hurdles;

    void Start()
    {

        GameFuse.SetUpGame(gameID, gameToken, this.GameSetUpCallback);

        hurdles = new List<GameObject>();
        CreateHurdles();
    }

    // Function to create hurdles
    private void CreateHurdles()
    {
        for (int i = 0; i < 14; i++)
        {
            GameObject hurdle = Instantiate(hurdlePrefab);
            int randomZ = UnityEngine.Random.Range(-2, 2) > 0 ? 4 : -4;
            hurdle.transform.position = new Vector3(40 + i * 15, 0.75f, randomZ);

            hurdles.Add(hurdle);
        }
    }

    // Callback function for game setup
    void GameSetUpCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse setup success");
            isConnected = true;
        }
        else
        {
            Debug.Log("GameFuse setup failed: " + message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if user press enter
        if (Input.GetKeyDown(KeyCode.Return) && !isGameStarted && canGameStart)
        {
            canGameStart = false;
            isGameStarted = true;
            gameText.gameObject.SetActive(false);

            scoreText.gameObject.SetActive(true);
            scoreText.text = "Score: 0";

            playerController.StartRun();
            playerController.SetMaterial(GameFuseUser.CurrentUser.GetAttributeValue("Color"));

            foreach (GameObject hurdle in hurdles)
            {
                hurdle.GetComponent<HurdleController>().StartRun();
            }
        }
    }

    // Function to increase score
    public void IncreaseScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    // Function to show Game Over Screen
    public void GameOver()
    {
        if (!this.isGameOver)
        {
            this.isGameOver = true;
            playerController.StopRun();
            foreach (GameObject hurdle in hurdles)
            {
                hurdle.GetComponent<HurdleController>().StopRun();
            }

            gameText.gameObject.SetActive(true);
            gameText.text = "Game Over";
            gameText.fontSize = 50;


            string lastScore = GameFuseUser.CurrentUser.GetAttributeValue("Score");
            if (Int32.Parse(lastScore) < this.score)
            {
                GameFuseUser.CurrentUser.SetAttribute("Score", $"{score}");
            }
            if (score > 100)
            {
                GameFuseUser.CurrentUser.SetAttribute("IsPassed100Points", "true");
            }
            if (score > 200)
            {
                GameFuseUser.CurrentUser.SetAttribute("IsPassed200Points", "true");
            }
        }

        GameFuseUser.CurrentUser.AddLeaderboardEntry("GameLeaderboard", score, new Dictionary<string, string>(), this.LeaderboardEntryCallback);
    }

    public void LeaderboardEntryCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse leaderboard entry success");

            Invoke("RestartGame", 4f);
        }
        else
        {
            Debug.Log("GameFuse leaderboard entry failed: " + message);
        }
    }

    // Function to restart game
    public void RestartGame()
    {
        this.isGameStarted = false;
        this.isGameOver = false;
        this.score = 0;
        screens[5].SetActive(false);
        screens[2].SetActive(true);

        scoreText.gameObject.SetActive(false);

        gameText.text = "Press Enter to Start";
        gameText.fontSize = 32;

        //delte all hurdles
        foreach (GameObject hurdle in hurdles)
        {
            Destroy(hurdle);
        }
        hurdles.Clear();

        CreateHurdles();
    }

    // Function to show Sign In Screen
    public void SignInUser()
    {
        string email = signInEmail.text;
        string password = signInPassword.text;

        if (email == "" || password == "")
        {
            return;
        }

        if (isConnected)
        {
            GameFuse.SignIn(email, password, this.SignInCallback);
        }
        else
        {
            signInErrorText.text = "Not connected with Server! Please try again later.";
        }
    }

    // Callback function for sign in
    public void SignInCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse sign in success");
            screens[0].SetActive(false);
            screens[2].SetActive(true);


            // Initialize user attributes
            float credits = GameFuseUser.CurrentUser.GetCredits();
            if (credits == 0)
            {
                GameFuseUser.CurrentUser.AddCredits(100);
            }

            string isPassed100Points = GameFuseUser.CurrentUser.GetAttributeValue("IsPassed100Points");
            if (isPassed100Points == null || isPassed100Points == "")
            {
                GameFuseUser.CurrentUser.SetAttribute("IsPassed100Points", "false");
            }
            string isPassed200Points = GameFuseUser.CurrentUser.GetAttributeValue("IsPassed200Points");
            if (isPassed200Points == null || isPassed200Points == "")
            {
                GameFuseUser.CurrentUser.SetAttribute("IsPassed200Points", "false");
            }
            string score = GameFuseUser.CurrentUser.GetAttributeValue("Score");
            if (score == null || score == "")
            {
                GameFuseUser.CurrentUser.SetAttribute("Score", "0");
            }
            string color = GameFuseUser.CurrentUser.GetAttributeValue("Color");
            if (color == null || color == "")
            {
                GameFuseUser.CurrentUser.SetAttribute("Color", "red");
            }

            GameFuse.Instance.GetLeaderboard(5, false, "GameLeaderboard", this.LeaderboardCallback);
        }
        else
        {
            Debug.Log("GameFuse sign in failed: " + message);
            signInErrorText.text = message;
        }
    }

    // Function to show Sign Up Screen
    public void SignUpUser()
    {
        string username = signUpUsername.text;
        string email = signUpEmail.text;
        string password = signUpPassword.text;
        string cpassword = signUpCPassword.text;

        if (username == "" || email == "" || password == "" || cpassword == "")
        {
            return;
        }

        if (password != cpassword)
        {
            signUpErrorText.text = "Password and Confirm Password does not match!";
            return;
        }

        if (isConnected)
        {
            GameFuse.SignUp(email, password, cpassword, username, this.SignUpCallback);
        }
        else
        {
            signUpErrorText.text = "Not connected with Server! Please try again later.";
        }
    }

    // Callback function for sign up
    public void SignUpCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse sign up success");
            screens[1].SetActive(false);
            screens[0].SetActive(true);
            GameFuseUser.CurrentUser.SetAttributeLocal("TEST1", "6");
            GameFuseUser.CurrentUser.SetAttributeLocal("TEST2", "7");
            GameFuseUser.CurrentUser.SyncLocalAttributes(this.AttributesSynced);

        }
        else
        {
            Debug.Log("GameFuse sign up failed: " + message);
            signUpErrorText.text = message;
        }
    }

    public void AttributesSynced(string message, bool hasError){
        Debug.Log("Attributes Synced!");
    }


    // Function to show Sign In Screen
    public void SignOutUser()
    {
        screens[2].SetActive(false);
        screens[0].SetActive(true);

        signInEmail.text = "";
        signInPassword.text = "";
        signInErrorText.text = "";
    }

    // Function to show Sign Up Screen
    public void StartGame()
    {
        canGameStart = true;
        screens[2].SetActive(false);
        screens[5].SetActive(true);

        playerController.SetMaterial(GameFuseUser.CurrentUser.GetAttributeValue("Color"));
    }

    // Function to show Leaderboard
    public void ShowLeaderboard()
    {
        screens[2].SetActive(false);
        screens[3].SetActive(true);

        updateLeaderBoard();

        GameFuse.Instance.GetLeaderboard(5, false, "GameLeaderboard", this.LeaderboardCallback);
    }

    // Callback function for leaderboard
    public void LeaderboardCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse leaderboard success");
            updateLeaderBoard();
        }
        else
        {
            Debug.Log("GameFuse leaderboard failed: " + message);
        }
    }

    // Function to update leaderboard
    private void updateLeaderBoard()
    {
        leaderboardListsText.text = "";

        // Get leaderboard entries
        List<GameFuseLeaderboardEntry> leaderboardEntries = GameFuse.Instance.leaderboardEntries;
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            GameFuseLeaderboardEntry entry = leaderboardEntries[i];
            Debug.Log(entry.GetUsername() + " - " + entry.GetScore());

            string gap = " ";
            for (int j = 0; j < 50 - (entry.GetUsername().Length + entry.GetScore().ToString().Length); j++)
            {
                gap += " ";
            }
            leaderboardListsText.text += (i + 1) + ".        " + entry.GetUsername() + gap + entry.GetScore() + "\n";
        }
    }

    // Function to show Shop
    public void ShowShop()
    {
        screens[2].SetActive(false);
        screens[4].SetActive(true);

        updateShop();
    }

    // Function to update shop
    private void updateShop()
    {
        creditsText.text = "Your have " + GameFuseUser.CurrentUser.GetCredits() + " credits";

        for (int i = 0; i < items.transform.childCount; i++)
        {
            items.transform.GetChild(i).gameObject.SetActive(false);
        }
        items.transform.GetChild(0).gameObject.SetActive(true);

        // Set purchased text
        List<GameFuseStoreItem> purchasedItems = GameFuseUser.CurrentUser.GetPurchasedStoreItems();
        foreach (GameFuseStoreItem item in purchasedItems)
        {
            if (item.GetCategory() == "Player")
            {
                // Set selected item
                for (int i = 0; i < items.transform.childCount; i++)
                {
                    if (items.transform.GetChild(i).name.ToLower() == item.GetName().Split(' ')[0].ToLower())
                    {
                        items.transform.GetChild(i).gameObject.SetActive(true);
                        break;
                    }
                }

                // Set purchased text
                foreach (GameObject storeItem in storeItems)
                {
                    if (storeItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text.ToLower() == item.GetName().ToLower())
                    {
                        storeItem.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Purchased";
                        break;
                    }
                }
            }
        }

        // Set selected item   
        string color = GameFuseUser.CurrentUser.GetAttributeValue("Color");
        for (int i = 0; i < items.transform.childCount; i++)
        {
            if (items.transform.GetChild(i).name.ToLower() == color.ToLower())
            {
                items.transform.GetChild(i).GetComponent<Image>().color = new Color(147f / 255f, 135f / 255f, 135f / 255f);
            }
            else
            {
                items.transform.GetChild(i).GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
            }
        }

        // Set price for each item
        List<GameFuseStoreItem> gameFustStoreItems = GameFuse.GetStoreItems();
        foreach (GameFuseStoreItem item in gameFustStoreItems)
        {
            if (item.GetCategory() == "Player")
            {
                foreach (GameObject storeItem in storeItems)
                {
                    if (storeItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text.ToLower() == item.GetName().ToLower())
                    {
                        storeItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = item.GetCost().ToString();
                        break;
                    }
                }
            }
        }
    }

    // Function to purchase item
    public void PurchaseItem(string value)
    {
        List<GameFuseStoreItem> storeItems = GameFuse.GetStoreItems();
        bool isFound = false;
        GameFuseStoreItem storeItem = null;
        for (int i = 0; i < storeItems.Count; i++)
        {
            if (storeItems[i].GetCategory() == "Player")
            {
                if (storeItems[i].GetName().ToLower() == value.ToLower())
                {
                    Debug.Log(storeItems[i].GetName().ToLower() + " - " + value.ToLower());
                    storeItem = storeItems[i];
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            Debug.Log("Store item not found");
            return;
        }

        GameFuseUser.CurrentUser.PurchaseStoreItem(storeItem, this.PurchaseItemCallback);
    }

    // Callback function for purchase item
    public void PurchaseItemCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse purchase item success");
            updateShop();
        }
        else
        {
            Debug.Log("GameFuse purchase item failed: " + message);
        }
    }

    // Function to select item
    public void SelectItem(string value)
    {
        GameFuseUser.CurrentUser.SetAttribute("Color", value, this.SelectItemCallback);
    }

    // Callback function for select item
    public void SelectItemCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse select item success");
            updateShop();
        }
        else
        {
            Debug.Log("GameFuse select item failed: " + message);
        }
    }

    // Function to show Main Screen
    public void ShowMain()
    {
        screens[3].SetActive(false);
        screens[4].SetActive(false);
        screens[5].SetActive(false);
        screens[2].SetActive(true);
    }
}
