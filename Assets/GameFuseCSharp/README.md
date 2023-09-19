# Game Fuse C#
Use Game Fuse C# in your Unity project to easily add authenticaion, user data, leaderboards, in game stores and more all without writing and hosting any servers, or writing any API code. Its never been easier to store your data online! Game Fuse is always free for hobby and small indie projects. With larger usage there are metered usage fees.

## Getting Started
The first step of integrating GameFuse with your project, is to make an account at https://www.gamefuse.co.
After creating your account, add your first game and note the ID and API Token.
With this setup, you can now connect via your game client. Download and unzip the code from https://github.com/game-fuse/game-fuse-cs
then add to your Unity project in the Scripts folder.
If you would like to see an example of how the code works, check out https://github.com/game-fuse/game-fuse-unity-example

At this point in time, you would add the prefab in this folder GameFuseInitializer
You can also build this manually by adding new GameObject to your first scene, and add a number of script componenets:
- GameFuse.cs
- GameFuseLeaderboardEntry.cs
- GameFuseUser.cs
- GameFuseUtilities.cs
- GameFuseStoreItem.cs

At this point in time you have the library installed in your game, and you are ready to connect


## Connecting to Game Fuse

The first step in using GameFuse after it is installed and your account is regestered is to run the SetUpGame function. After this step you can run other functions to register users, sign in users, read and write game data.

In any script on your first scene you can run:

```

void Start () {
    var gameID = 'Your Game ID Here';
    var gameToken 'your Game Token Here';

    # 3rd param is the function below, GameFuse will call this function when it is done connecting your game
    GameFuse.SetUpGame(gameID, gameToken, GameSetUpCallback);
}



void GameSetUpCallback(string message, bool hasError) {
    if (hasError)
    {
        Debug.Error("Error connecting game: "+message);
    }
    else
    {
        Debug.Log("Game Connected Successfully")
        foreach (GameFuseStoreItem storeItem in GameFuse.GetStoreItems())
        {
            Debug.Log(storeItem.GetName() + ": " + storeItem.GetCost());
        }
    }
}

```

After completion of this GameFuse function, for this code example, we print out names and costs of all the store items in your virtual store. This prooves the initial sync worked. Note this will only print out store items if you added some on your GameFuse dashboard.

## Signing game users up

Enable users to sign up in your Unity game with the following code. They will be saved on your GameFuse Game and can then login from other devices since the data is saved online.
Add a method on a MonoBehavior on your sign up scene after you have collected your inputted username and password. Maybe this is on a a button function for a 'submit' or 'register' button.
Username is mandatory but it is just for display. Later sign in attempts will use email not username

```
#Feed in your users email, username and password here
void SignUp (email, password, password_confirmation, username) {
	#5th parameter is the callback when execution is complete
  	GameFuse.SignUp(userEmail, "password", "password", username, SignedUp);
}

void SignedUp(string message, bool hasError) {
  	if (hasError)
  	{
    	Debug.Error("Error signign up: "+message);
  	}
  	else
  	{
    	Debug.Log("Signed Up: " + GameFuseUser.CurrentUser.GetName());
  	}
}
```

## Signing game users in


Signing In Follows the same protocal as signing up, just with different parateters. As always, there is a callback function to let you know your sign in has been successful or not.
Email and password (not username), will be used to sign in

```
#Feed in your users email and password here
void SignIn (email, password, SignedIn) {
	#3rd parameter is the callback when execution is complete
  	GameFuse.SignIn(userEmail, password, SignedIn);
}

void SignedIn(string message, bool hasError) {
  	if (hasError)
  	{
      	Debug.Error("Error signign up: "+message);
  	}
  	else
  	{
      	Debug.Log("Logged In: " + GameFuseUser.CurrentUser.GetName());
      	Debug.Log("Current Credits: " + GameFuseUser.CurrentUser.GetCredits());
  	}
}
```

## Creating store items on the web


To create store items on the web, navigate to your GameFuse.co home page, and sign in if you are not already
You can click on your Game on the homepage you want to add items for. On this page if you scroll down to the Store Items section, you will see + STORE ITEM button, here you can add in Name, Cost, Description, and Category. All are mandatory but do not need to used in your game. The store feature does not integrate real payment systems, this is for items you want your users to be able to "unlock" with in-game-currency or with achievements in the game. How you configure that is up to you.

## Using the store in your game

Store Items Library are downloaded upon SignIn() and SignUp(), The Items will be refreshed every time the user signs in or signs up again.
To access store items and attributes by calling  the following code. This doesnt sync them with the available items on the server, it is simply showing you the results downloaded on sign in or sign up.

```
foreach (GameFuseStoreItem storeItem in GameFuse.GetStoreItems())
{
    Debug.Log(storeItem.GetName());  //FireBow
    Debug.Log(storeItem.GetCategory()); //BowAndArrows
    Debug.Log(storeItem.GetId()); //12
    Debug.Log(storeItem.GetDescription());  //A bow and arrow item that shoots fire arrows
    Debug.Log(storeItem.GetCost()); // 500 (credits)
}
```

To access purchased store items by your current logged in user call the following. Because these are downloaded on login, there is no callback for this! It is already available!
This will throw an error if you are not signed in already

```
List < GameFuseStoreItem > items = GameFuseUser.CurrentUser.GetPurchasedStoreItems();
```

To Purchase a store item simply call the code below.
Because this function talks to the server, it will require a callback. If the user doesnt have enough credits on their account (see next section), the purchase will fail
This function will refresh the GameFuseUser.CurrentUser.purchasedStoreItems List with the new item

```
void PurchaseItem(store_item){
  Debug.Log(GameFuseUser.CurrentUser.purchasedStoreItems.Count); // Prints 0
  GameFuseUser.PurchaseStoreItem(GameFuse.GetStoreItems().First, PurchasedItemCallback)
}

void PurchasedItemCallback(string message, bool hasError) {
  if (hasError)
  {
      Debug.Error("Error purchasing item: "+message);
  }
  else
  {
      Debug.Log("Purchased Item");
      Debug.Log(GameFuseUser.CurrentUser.purchasedStoreItems.Count); // Prints 1
  }
}
```


## Using Credits


Credits are a numeric attribute of each game user. It is a simple integer value.
You can add them manually and they are detracted automatically upon store item purchases
Below is a script to demonstrate the full lifecycle of credits on a signed in user. First it prints the credits your signed in user has, then prints the cost of the first store item, then it adds credits to your user. Because this syncs with the server, it requires a callback. Upon success, you will see the user now has more credits when logged. At this point in time you can then run the purchase store item function successfully.

```
void Start(){
    Debug.Log(GameFuseUser.CurrentUser.credits;  // Prints 0
    Debug.Log(GameFuse.GetStoreItems().First.cost) // Prints 25 (or whatever you set your first item to on the web dashboard)
    GameFuseUser.CurrentUser.AddCredits(50, AddCreditsCallback);
}

void AddCreditsCallback(string message, bool hasError)
{
    if (hasError)
    {
        Debug.Error("Error adding credits: " + message);
    }
    else
    {
      Debug.Log(GameFuseUser.CurrentUser.credits;  // Prints 50
      GameFuseUser.PurchaseStoreItem(GameFuse.GetStoreItems().First, PurchasedItemCallback)

    }

}

void PurchasedItemCallback(string message, bool hasError) {
  if (hasError)
  {
      Debug.Error("Error purchasing item: "+message);
  }
  else
  {
      Debug.Log("Purchased Item");
      Debug.Log("Current Credits: " + GameFuseUser.CurrentUser.GetCredits());
  }
}

```
## Custom user data


Custom user data or Key Value pairs are a simple way to save any kind of data for a particular user.
Some examples might be {"world_2_unlocked":"true"}, {"player_color","red"}, {"favorite_food","Onion"}
These are downloaded to your system upon login, and synced when one is updated. You can access with GameFuseUser.CurrentUser.attributes

All values and keys must be strings. If you want to use other data structures like arrays, you could stringify the array on save, and convert the saved string to an array on load.

```
void Start(){
    Debug.Log(GameFuseUser.CurrentUser.attributes.Count);  // Prints 0
    Debug.Log(GameFuseUser.CurrentUser.GetAttributeValue("CURRENT_LEVEL") == null); // Prints true
    GameFuseUser.CurrentUser.SetAttribute("CURRENT_LEVEL", "5", SetAttributeCallback);
}

void SetAttributeCallback(string message, bool hasError) {
  if (hasError)
  {
      Debug.Error("Error setting attribute: "+message);
  }
  else
  {
      Debug.Log(GameFuseUser.CurrentUser.GetAttributeValue("CURRENT_LEVEL")); // Prints "5"
  }
}
```

## In game leaderboard

Leaderboards can be easily created within GameFuse
From the Unity game client, a Leaderboard Entry can be added with a leaderboard_name, score, and extra_attributes (metadata) for the current signed in user
Leaderboards can be downloaded for a specific leaderboard_name, which would gather all the high scores in order for all users in the game or
Leaderboards can be downloaded for a specific user, so that you can download the current users leaderboard data for all leaderboard_names
The below example shows submitting 2 leaderboard entries, then retrieving them for the game, and for the current user


```
void Start(){
  var extraAttributes = new Dictionary < string, string > ();
  extraAttributes.Add("deaths", "15");
  extraAttributes.Add("Jewels", "12");
  GameFuseUser.CurrentUser.AddLeaderboardEntry("Game1Leaderboard",10, extraAttributes, LeaderboardEntryAdded);
}

void LeaderboardEntryAdded(string message, bool hasError)
{
    if (hasError)
    {
        print("Error adding leaderboard entry: " + message);
    }
    else
    {

        print("Set Leaderboard Entry 2");
        var extraAttributes = new Dictionary < string, string > ();
        extraAttributes.Add("deaths", "25");
        extraAttributes.Add("Jewels", "15");

        GameFuseUser.CurrentUser.AddLeaderboardEntry("Game1Leaderboard", 7, extraAttributes, LeaderboardEntryAdded2);

    }
}

void LeaderboardEntryAdded2(string message, bool hasError)
{
    if (hasError)
    {
        print("Error adding leaderboard entry 2: " + message);
    }
    else
    {
        print("Set Leaderboard Entry 2");
        GameFuseUser.CurrentUser.GetLeaderboard(5, true, LeaderboardEntriesRetrieved);
    }
}

void LeaderboardEntriesRetrieved(string message, bool hasError)
{
    if (hasError)
    {
        print("Error loading leaderboard entries: " + message);
    }
    else
    {

        print("Got leaderboard entries for specific user!");
        foreach( GameFuseLeaderboardEntry entry in GameFuse.Instance.leaderboardEntries)
        {
            print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName() );
            foreach (KeyValuePair < string,string > kvPair in entry.GetExtraAttributes())
            {
                print(kvPair.Key + ": " + kvPair.Value);
            }

        }
        GameFuse.Instance.GetLeaderboard(5, true, "Game1Leaderboard", LeaderboardEntriesRetrievedAll);

    }
}

void LeaderboardEntriesRetrievedAll(string message, bool hasError)
{
    if (hasError)
    {
        print("Error loading leaderboard entries: " + message);
    }
    else
    {
        print("Got leaderboard entries for whole game!");
        foreach (GameFuseLeaderboardEntry entry in GameFuse.Instance.leaderboardEntries)
        {
            print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName());
            foreach (KeyValuePair < string, string > kvPair in entry.GetExtraAttributes())
            {
                print(kvPair.Key + ": " + kvPair.Value);
            }

        }

    }
}
```

You can also clear all leaderboard entries for a particular leaderboard_name for the current user like this:

```
void Start(){
  var extraAttributes = new Dictionary < string, string > ();
  extraAttributes.Add("deaths", "15");
  extraAttributes.Add("Jewels", "12");
  GameFuseUser.CurrentUser.AddLeaderboardEntry("Game2Leaderboard",10, extraAttributes, LeaderboardEntryAdded);
}

void LeaderboardEntryAdded(string message, bool hasError)
{
    if (hasError)
    {
        print("Error adding leaderboard entry: " + message);
    }
    else
    {

        print("Clear Leaderboard Entry 2");
        GameFuseUser.CurrentUser.ClearLeaderboardEntries("Game2Leaderboard", LeaderboardEntryCleared);

    }
}

void LeaderboardEntryCleared(string message, bool hasError)
{
    if (hasError)
    {
        print("Error adding leaderboard entry: " + message);
    }
    else
    {
        print("User will no longer have leaderboard entries for 'Game2Leaderboard'");

    }
}
```


## Class Methods

Check each model below for a list of methods and attributes.

```
###GameFuseUser.cs
your current signed in user can be retrieved with:
GameFuseUser user = GameFuse.CurrentUser;

public bool IsSignedIn();
public int GetNumberOfLogins();
public DateTime GetLastLogin();
public string GetUsername();
public int GetScore();
public int GetCredits();

public void AddCredits(int credits, Action < string, bool > callback = null);
public void SetCredits(int credits, Action < string, bool > callback = null);
public void AddScore(int credits, Action < string, bool > callback = null);
public void SetScore(int score, Action < string, bool > callback = null);
public Dictionary < string,string >  GetAttributes();
public Dictionary < string,string > .KeyCollection GetAttributesKeys();
public string GetAttributeValue(string key);
public void SetAttribute(string key, string value, Action < string, bool > callback = null);
public void RemoveAttribute(string key, Action < string, bool > callback = null);
public List < GameFuseStoreItem > GetPurchasedStoreItems();
public void PurchaseStoreItem(GameFuseStoreItem storeItem, Action < string, bool > callback = null);
public void PurchaseStoreItem(int storeItemId, Action < string, bool > callback = null);
public void RemoveStoreItem(int storeItemID, bool reimburseUser, Action < string, bool > callback = null);
public void RemoveStoreItem(GameFuseStoreItem storeItem, bool reimburseUser, Action < string, bool > callback = null);
public void AddLeaderboardEntry(string leaderboardName, int score, Dictionary extraAttributes = null, Action < string, bool > callback = null);
public void AddLeaderboardEntry(string leaderboardName, int score, Action < string, bool > callback = null);
public void GetLeaderboard(int limit, bool onePerUser, Action < string, bool > callback = null); //Get all leaderboard entries for current signed in user


###GameFuse.cs
public static void SetUpGame(string gameId, string token, Action < string, bool > callback = null);
public static string GetGameId();
public static string GetGameName();
public static string GetGameDescription();
public static List < GameFuseStoreItem > GetStoreItems() //Gets all store items (your library)
public static void SignIn(string email, string password, Action < string, bool > callback = null);
public static void SignUp(string email, string password, string password_confirmation, string username, Action < string, bool > callback = null);
public void GetLeaderboard(int limit, bool onePerUser, string LeaderboardName, Action < string, bool > callback = null); //Retrieves leaderboard for one specific Leaderboard Name


###GameFuseStoreItem.cs
public string GetName();
public string GetCategory();
public string GetDescription();
public int GetCost();
public int GetId();

###GameFuseLeaderboardEntry.cs
public string GetUsername();
public int GetScore();
public string GetLeaderboardName();
public Dictionary GetExtraAttributes();
```