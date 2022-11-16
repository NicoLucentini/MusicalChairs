using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameState
{
    STARTED_MUSIC_RUNNING,
    STARTED_MUSIC_STOPPED,
    ENDED,
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //EVENTS
    public static System.Action onGameStarted;
    public static System.Action onGameLose;
    public static System.Action allChairsOccuped;


    [Header("References")]
    public MusicPlayer musicPlayer;
    public PlayersSpawner playersSpawner;
    public GuiManager guiManager;
    public ShopManager shopManager;
    public UIController uiController;
    public Profile profile;


    [Header("Settings")]

    public float defaultReactionTime = 1;

    [Header("Game Entities")]
    public List<Chair> chairs;
    public List<BaseEntity> players;
    public List<Transform> waypoints = new List<Transform>();
    public List<GameObject> bananas = new List<GameObject>();

    [Header("Waypoints")]
    public Transform waypointsContainer;
    public GameObject wpPrefab;
    public Elipse elipse;
    public int elipseAmount;

    [Header("Chairs")]
    public int startChairIndex = 10;
    public bool chairsSizeFixed = true;
    //MAKE IT OBSOLETE
    public GameObject chairPrefab;
    [Header("Bananas")]
    //MAKE IT OBSOLETE
    public GameObject bananaPrefab;
    public int bananaCount;

    [Header("Center")]
    public GameObject centerPrefab;
    public GameObject middle;


    [Header("Game Status")]
    public bool gameRunning = true; // si el juego esta andando con la musica
    public bool gameEnded = false; //cuando el juego termina
    public bool isPaused = false;
    private int chairIndex = 10;
    public GameState state;
    public bool continueClicked = false;
    public float chairSize = .2f;
    public float chairMult = .5f;
    public float waypointsSize = 1f;
    public float waypointsMult = .5f;

    public int moneyEarned = 0;

    bool won = false;

    [Header("Ad")]
    [SerializeField] private int gameLostCount = 0;
    [SerializeField] private int gameLostForAd = 2;

    [Header("Prefabs")]
    public GameObject chair;

    [Header("Player Settings")]
    public BuyableCharacter player;
    public BuyableBackground background;
    public GameObject visualBackground;
    public float playerReactionTime = 0;

    [Header("Pause")]
    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image pauseImage;


    public float GetWpSize(int val)
    {
        float size = 1;

        switch (val)
        {
            case 10: return 1f;
            case 9: return .95f;
            case 8: return .9f;
            case 7: return .85f;
            case 6: return .8f;
            case 5: return .75f;
            case 4: return .7f;
            case 3: return .65f;
            case 2: return .6f;
            case 1: return .4f;
        }
        return size;
    }

    public float GetChairSize(int val)
    {
        /*
        if (chairsSizeFixed)
            return chairMult * waypointsSize;
            */
        if (chairsSizeFixed)
            return GetWpSize(val) * chairMult;
        float size = 1;
     
        switch (val)
        {

            case 10: return .5f;
            case 9: return .475f;
            case 8: return .45f;
            case 7: return .425f;
            case 6: return .4f;
            case 5: return .35f;
            case 4: return .3f;
            case 3: return .25f;
            case 2: return .2f;
            case 1: return .1f;
        }
        return size;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        MusicPlayer.onMusicStopped += OnMusicStopped;
        Chair.onChairOccuped += OnChairPopulated;
        playerReactionTime = defaultReactionTime;
        SaveManager.LoadData(OnLoadData);
    }

    

    public void AddReactionTime(float value = 0)
    {
        if (value == 0)
        {
            profile.totalReactionTime += defaultReactionTime;
        }
        else
        {
            profile.totalReactionTime += (value);
        }

        profile.roundsPlayed++;
        playerReactionTime = (profile.totalReactionTime / (float)profile.roundsPlayed);
    }

    void SaveProfile()
    {
        SaveManager.SaveData(profile);
    }
  
    void OnLoadData(object data)
    {
        if (data != null)       
        {
            profile = data as Profile;
            bool isNull = profile.equippedSongs == null;
            Debug.Log(isNull);

           
            playerReactionTime = profile.totalReactionTime / profile.roundsPlayed;

            if (profile.equippedSongs == null)
            {
                profile.equippedSongs = new List<string>();
                profile.equippedSongs.Add("Original");
                profile.equippedSongs.Add("Circus");
                profile.equippedSongs.Add("Classic");
            }
        }
        
    }

    private void Start()
    {
        chairIndex = startChairIndex;
    }

    void TestInstance()
    {
        for (int i = 10; i > 0; i--)
        {
            
            CreateWaypoints(elipse, i);
            CreateChairs(elipse, i);
            transform.position += Vector3.right * 10;
        }
    }


    #region CREATIONS
    void CreateCenter()
    {
        
        if (middle != null)
            Destroy(middle);
            
        GameObject go = GameObject.Instantiate(centerPrefab, transform.position, Quaternion.identity);

        float size = (chairSize + waypointsSize) / 2;
        size += 0.1f;
        //float size = waypointsSize;
        go.transform.localScale = new Vector3(elipse.xAxis * size, 2, elipse.yAxis * size);
        middle = go;
    }

    private void CreateChairs(Elipse e, int amount)
    {
        chairSize = GetChairSize(amount);
        chairPrefab = background.chairPrefab;
        for (int i = 0; i < amount; i++)
        {
            float angle = ((float)i / (float)amount);
            Vector2 pos = Elipse.Evaluate(angle, e.xAxis * chairSize, e.yAxis * chairSize);
            pos += new Vector2(transform.position.x, transform.position.y);


            GameObject go = GameObject.Instantiate(chair, new Vector3(pos.x, .1f, pos.y), Quaternion.identity);
            GameObject go2 = GameObject.Instantiate(chairPrefab, new Vector3(pos.x, .1f, pos.y), Quaternion.identity, go.transform);
            go.name = "Chair " + i;

           // go2.transform.SetParent(go.transform);
            //go.GetComponent<Chair>().rend = go2.GetComponent<MeshRenderer>();


            //Vector3 dir = VectorHelp.Dir2D(go.transform.position, Vector3.zero);
            Vector3 dir = VectorHelp.Dir2D(go.transform.position, transform.position).normalized;
            go.transform.LookAt(transform.position - dir * 2);
           // go2.transform.rotation = go.transform.rotation;
            chairs.Add(go.GetComponent<Chair>());
        }

        CreateCenter();
    }
    void CreateBananas()
    {
        StartCoroutine(CTCreateBananas());
    }
    IEnumerator CTCreateBananas()
    {
        bananaCount = 1 + Mathf.FloorToInt( chairIndex / 4 );
        int step = Mathf.CeilToInt( waypoints.Count / bananaCount);
        int initialIndex = Random.Range(0, waypoints.Count);

        for (int i = 0; i < bananaCount; i++)
        {
            var pos = waypoints[initialIndex].position;

            GameObject go = GameObject.Instantiate(bananaPrefab, pos, Quaternion.identity);
            bananas.Add(go);


            initialIndex += step;
            if (initialIndex >= waypoints.Count)
                initialIndex = (initialIndex - waypoints.Count);

            yield return new WaitForEndOfFrame();
        }
    }

    public static Action<List<Transform>> onWaypointsInstantiated;

    private void CreateWaypoints(Elipse e, int amount)
    {
        waypointsSize = GetWpSize(chairIndex) * waypointsMult;
        int wpCount = waypoints.Count;
        for (int i = 0; i < amount; i++)
        {
            float angle = ((float)i / (float)amount);
            Vector2 pos = Elipse.Evaluate(angle, e.xAxis * waypointsSize, e.yAxis * waypointsSize);
            pos += new Vector2(transform.position.x, transform.position.y);

            if (wpCount == 0)
            {
                GameObject go = GameObject.Instantiate(wpPrefab, new Vector3(pos.x, .1f, pos.y), Quaternion.identity);
                go.transform.SetParent(waypointsContainer);
                waypoints.Add(go.transform);
            }
            else
            {
                waypoints[i].position = new Vector3(pos.x, .1f, pos.y);
            }
        }
        
        FindObjectOfType<ElipseRenderer>().DrawElipse(e, elipseAmount, waypointsSize);

        CreateBananas();
    }

    #endregion

    
    #region REGISTERED EVENTS
    
    void OnChairPopulated(Chair chair)
    {
        if (!chairs.All(x => x.occuped)) return;
        
        allChairsOccuped?.Invoke();
        Invoke("DestroyBadPlayers", 2f);
    }

    void OnMusicStopped()
    {
        if (!gameRunning) return;

        gameRunning = false;
        state = GameState.STARTED_MUSIC_STOPPED;
        DestroyAllBananas();
        Invoke("DestroyBadPlayers", 5f);
    }

    #endregion

    #region REMOVES
    void DestroyAllBananas()
    {
        for (int i = 0; i < bananas.Count; i++)
            Destroy(bananas[i].gameObject);

        bananas.Clear();
    }
    void DestroyBadPlayers()
    {
        CancelInvoke("DestroyBadPlayers");
        playersSpawner.survivors.Clear();
        BaseEntity tempP = null;
        //tempP = players.First(x => !x.sit);
        foreach (var p in players)
        {
            if (!p.IsSit())
                tempP = p;
        }

        foreach (var p in players)
        {
            if (!p.isHuman && p != tempP)
                playersSpawner.survivors.Add(p.settings);
        }

        if (tempP != null)
        {
            if (tempP.isHuman)
            {
                OnGameLose();
                Debug.Log("Perdiste");
            }
            else
            {
                Destroy(tempP);
                if (chairs.Count == 1)
                    OnGameWon();
            }
        }

        if (!gameEnded)
            StartNewLevel();
    }

    private void RemoveThings()
    {
        RemoveAllPlayers();
        DestroyAllBananas();
        RemoveAllChairs();
        //RemoveAllWaypoints();
    }

    private void RemoveAllChairs()
    {
        for (int i = 0; i < chairs.Count; i++)
            Destroy(chairs[i].gameObject);

        chairs.Clear();
    }

    private void RemoveAllPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].waypoints.Clear();
            Destroy(players[i].gameObject);
        }
        players.Clear();
    }
    private void RemoveAllWaypoints()
    {
        for (int i = 0; i < waypoints.Count; i++)
            Destroy(waypoints[i].gameObject);
        waypoints.Clear();
    }


    #endregion

    #region GAME

   
    public void OnClickPause()
    {
        isPaused = !isPaused;
        musicPlayer.Pause(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        pauseImage.sprite = isPaused ? playIcon : pauseIcon;
    }

    private void ChangePause(bool paused)
    {
        isPaused = paused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseImage.sprite = isPaused ? playIcon : pauseIcon;
    }

    public void OnGameLose()
    {
        gameLostCount++;

        won = false;
        guiManager.ChangeEndText(true, "¡Better Luck Next Time!", 1, continueClicked, true, !continueClicked);
        guiManager.Continue();
        guiManager.endGo.GetComponent<Animation>().Play("EndGameAnim");

        int reward = RewardTable.GetScore(chairIndex);
        moneyEarned += reward;
        guiManager.ShowScore(reward);

        
        onGameLose?.Invoke();

        OnGameEnd();

        
    }

    private void OnGameWon()
    {
        won = true;
        guiManager.ChangeEndText(true, "¡GANASTE!", 10);
        guiManager.endGo.GetComponent<Animation>().Play("EndGameAnim");
        int reward = RewardTable.GetScore(chairIndex);
        moneyEarned += reward;
        guiManager.SetRestartButton(true, "Play Again");
        OnGameEnd();
    }
   

    public void OnClickContinue()
    {
        Debug.Log("Apreto Continue");
        continueClicked = true;
        guiManager.Restart(false, "");
        guiManager.StopContinueCt();
       // Time.timeScale = 1; // necesito la timeScale en 1?
        AdManager.instance.AdShow("rewardedVideo",() => StartLevel(chairIndex));
    }

    public void OnClickRestart()
    {
        Restart();
    }

    //Restart vuelve al level inicial de sillas
    public void Restart(bool showAd = true)
    {
        ChangePause(false);
        CancelInvoke("DestroyBadPlayers");
        Debug.Log("Apreto restart");
        chairIndex = startChairIndex;
       // waypointsSize = GetWpSize(chairIndex);
        
        Time.timeScale = 1;
        continueClicked = false;
        playersSpawner.Cancel();


        if (gameLostCount > 1 && gameLostCount % gameLostForAd == 0) {
            AdManager.instance.AdShow("endgameobligatory", null , null);
        }
      
        StartCoroutine(RestartGame(3));
    }

    private void StartLevel(int level)
    {
        chairIndex = level;
        Time.timeScale = 1;
        RemoveThings();
        StartCoroutine(RestartGame(3));
    }

    private void StartGame()
    {
        ChangePause(false);
        Time.timeScale = 1;

        guiManager.ChangeChairsText(chairIndex.ToString());

        CreateWaypoints(elipse, 20);
        CreateChairs(elipse, chairIndex);

        playersSpawner.SpawnAll(elipse, waypointsSize);
        musicPlayer.StartMusic();

        gameEnded = false;
        gameRunning = true;
        state = GameState.STARTED_MUSIC_RUNNING;
    }
    

    //Empieza un nuevo nivel --
    private void StartNewLevel()
    {
        chairIndex--;
        StartLevel(chairIndex);
    }

    private void OnGameEnd()
    {
        StopAllCoroutines();
        CancelInvoke("DestroyBadPlayers");
        RemoveThings();
        gameRunning = false;
        gameEnded = true;
        playersSpawner.survivors.Clear();
        playersSpawner.Cancel();
        musicPlayer.StopMusic();
        state = GameState.ENDED;
    }

    IEnumerator DoInTime(float t, System.Action action)
    {
        yield return new WaitForSeconds(t);

        action();
    }

    IEnumerator RestartGame(int time)
    {
        int i = time;
        guiManager.ChangeEndText(false);
        guiManager.Restart(true, i.ToString());
        SaveProfile();
        while (i > 0)
        {
            guiManager.Restart(true, i.ToString());
            yield return new WaitForSeconds(1);
            i--;
        }
        guiManager.Restart(false);
        
        StartGame();
    }

    public void GoToMenu()
    {
        OnGameEnd();

        if (moneyEarned > 0)
        {
            Menu.instance.ShowBlocker(true, "Coins earned \n" + moneyEarned, null, 3);

            CoinsAnim.instance.StartAnim(10 + Mathf.FloorToInt(moneyEarned / 25), () =>
            {
                shopManager.MoneyAnim();
                Menu.instance.ShowBlocker(false);
            });

            shopManager.ChangeMoney(-moneyEarned);
            moneyEarned = 0;
            SaveManager.SaveData(profile);
        }
    }
    
    #endregion

}
