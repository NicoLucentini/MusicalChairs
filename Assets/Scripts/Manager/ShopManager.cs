using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ShopManager : MonoBehaviour
{

    [Header("References")]
    public Transform musicTransform;
    public Transform backgroundTransform;
    public ScrollRect lowRect;
    public AudioSource playSource;
    public Sprite playMusic;
    public Sprite stopMusic;  

    [Tooltip("El Parent donde estan todos los personajes en el shop")]
    public GameObject visualCharacterParent;


    [Header("UI Prefabs")]
    public GameObject uiMusicPrefab;
    public GameObject uiCharacterPrefab;
    public GameObject uiBackgroundPrefab;


    [Header("Tabs")]
    public Color selectedText;
    public Color selectedBackground;
    public Color nonSelectedText;
    public Color nonSelectedBackground;

    public Button charactersButton;
    public Button songsButton;
    public Button backgroundsButton;

    public Button currentButton;
   

    [Header("Shop UI")]
    public UIPreBuy uiPreBuyCharacter;
    public Text moneyText;
    public Animation moneyAnim;
    public List<GameObject> uiItems;

    [Header("Songs Color")]
    public Color buyColor;
    public Color equipColor;
    public Color unequipColor;

    [Header("Other")]
    private int displaySize = 192;
    public GameObject songsLog;
   
    public GameObject character;
    List<GameObject> visualChars = new List<GameObject>();

    [Tooltip("El Background Actual")]
    public GameObject background;

    [Header("Buyables")]
    public List<BuyableSongs> songs;
    public List<BuyableCharacter> characters;
    public List<BuyableBackground> backgrounds;

    [Header("Equipped")]
    public BuyableCharacter focusCharacterInfo;
    public BuyableSongs focusSongInfo;
    public BuyableBackground focusBackgroundInfo;
    int focusCharIndex = 0;

    [Header("Free Coins")]
    public Button freeMoneyButton;
    public Text freeMoneyTimerText;
    public int freeCoinsCd = 600;
    public int freeCoinsAmount = 1000;

    private void Start()
    {
        SwipeDetector.onSwipeMagnitud += OnSwipeMagnitud;
      
        GameManager.instance.player = characters.First(x => x.itemName == GameManager.instance.profile.selectedCharacter);
        GameManager.instance.background = backgrounds.First(x => x.itemName == GameManager.instance.profile.selectedBackground);


        LoadMoney();
        UpdateMoneyText();

        DisplayBackground(GameManager.instance.background, false);
        DisplayMusic();
        DisplayCharacters2();
        DisplayBackgrounds();

        LoadSongs();
    }
    void LoadSongs()
    {
        foreach (var song in songs)
        {
            if (GameManager.instance.profile.equippedSongs.Contains(song.itemName)
                && !GameManager.instance.musicPlayer.clips.Contains(song.clip))
            {
                GameManager.instance.musicPlayer.clips.Add(song.clip);
            }
        }

    }
    void OnSwipeMagnitud(SwipeDirection dir, int mag)
    {
        if (dir == SwipeDirection.Left)
            MoveCharacters(mag);
        else if (dir == SwipeDirection.Right)
            MoveCharacters(-mag);
    }
    
    #region FREE COINS

   
    void LoadMoney()
    {      
        StartCoroutine(TimeHelp.GetInternetTime(ClbkLoadLastTime));
    }
    public void OnClickFreeCoins()
    {
        AdManager.instance.AdShow("rewardedVideo", FreeCoinsClbk);
    }

    void FreeCoinsClbk()
    {
        ChangeMoney(-freeCoinsAmount);
               
        StartCoroutine(TimeHelp.GetInternetTime(ClbkFreeCoinsUpdateTime));

        freeMoneyButton.interactable = false;
        StartCoroutine(MoneyTimer(freeCoinsCd));
    }
    void ClbkFreeCoinsUpdateTime(System.DateTime time)
    {
        GameManager.instance.profile.last = time;
        SaveManager.SaveData(GameManager.instance.profile);
    }
    void ClbkLoadLastTime(System.DateTime time)
    {
        int secs = freeCoinsCd - TimeHelp.SecondsDifference(time, GameManager.instance.profile.last);

        if (secs < 0)
        {
            freeMoneyTimerText.text = "Get $" + freeCoinsAmount + " !!!";
            freeMoneyButton.interactable = true;
        }
        else
        {
            freeMoneyButton.interactable = false;
            StartCoroutine(MoneyTimer(secs));
        }
    }
    IEnumerator MoneyTimer(int totalSec)
    {
        int secs = totalSec;
        while (secs > 0)
        {
            Vector2 span = TimeHelp.GetMinAndSecFromSeconds(secs);
            freeMoneyTimerText.text = span.x.ToString("00") + " : " + span.y.ToString("00");
            secs--;
            yield return new WaitForSeconds(1);
        }
        freeMoneyTimerText.text = "Get $" + freeCoinsAmount + " !!!";
        freeMoneyButton.interactable = true;
    }

    #endregion

    #region SET MONEY
    public void MoneyAnim()
    {
        moneyAnim.Play();
    }
   
    public void ChangeMoney(int val)
    {
        GameManager.instance.profile.money -= val;
        UpdateMoneyText();
    }
    public void UpdateMoneyText()
    {
        moneyText.text = "$ " + GameManager.instance.profile.money.ToString();
    }
    public void SetMoney(int amount)
    {
        GameManager.instance.profile.money = amount;
        UpdateMoneyText();
    }


    #endregion
  
    #region UI TABS BUTTON

    public void OnClickCharacters()
    {
        OnChangeTab(charactersButton);
      //  DisplayCharacters();
        DisplayCharacters2();
    }
    public void OnClickMusics()
    {
        OnChangeTab(songsButton);
        backgroundTransform.gameObject.SetActive(false);
        musicTransform.gameObject.SetActive(true);
        lowRect.content = musicTransform.GetComponent<RectTransform>();
        //lowRect.
        //uiPreBuyBackground.gameObject.SetActive(false);
       // DisplayMusic();
    }
    public void OnClickBackgrounds()
    {
        OnChangeTab(backgroundsButton);
        backgroundTransform.gameObject.SetActive(true);
        musicTransform.gameObject.SetActive(false);
        lowRect.content = backgroundTransform.GetComponent<RectTransform>();
        //uiPreBuyBackground.gameObject.SetActive(true);
        DisplayBackground(GameManager.instance.background);
        //DisplayBackgrounds();
    }

    #endregion

    #region MENU TABS / ACTIONS 

    public void OnChangeTab(Button target)
    {
        if (currentButton != null)
        {
            currentButton.GetComponent<Image>().color = nonSelectedBackground;
            currentButton.GetComponentInChildren<Text>().color = nonSelectedText;
        }
        if (target != null)
        {
            currentButton = target;

            currentButton.GetComponent<Image>().color = selectedBackground;
            currentButton.GetComponentInChildren<Text>().color = selectedText;
        }
    }


    public void OnShop(bool on)
    {
       visualCharacterParent.SetActive(!on);
       uiPreBuyCharacter.gameObject.SetActive(!on);

        //if (!on) uiPreBuyBackground.gameObject.SetActive(false);
    }


    public void OnGoingToGame()
    {
        StopMusic();
     
        uiPreBuyCharacter.gameObject.SetActive(false);
        visualCharacterParent.SetActive(false);

        if (background != null)
            Destroy(background);

        DisplayBackground(GameManager.instance.background);
    }

    public void OnGoingToMenu()
    {
       // StopMusic();
       // uiPreBuy.gameObject.SetActive(false);
        visualCharacterParent.SetActive(true);
        uiPreBuyCharacter.gameObject.SetActive(true);
    }


    #endregion
    
    #region ALL
    public void SetDisplaySize(int displaySize)
    {
        this.displaySize = displaySize;
    }

    public void DestroyItems()
    {
        for (int i = 0; i < uiItems.Count; i++)
        {
            Destroy(uiItems[i]);
        }

        uiItems.Clear();
    }

    #endregion

    #region CHARACTERS
    //Display Characters2
    void DisplayCharacters2()
    {

        //DestroyItems();

        uiPreBuyCharacter.gameObject.SetActive(true);
        int i = 0;

        foreach (var settings in characters)
        //foreach (var settings in GameManager.instance.materialBank.entitySettings)
        {
            GameObject go = GameObject.Instantiate(settings.prefab, visualCharacterParent.transform);
            go.transform.position += new Vector3(i, 0, 0);
            i++;
            visualChars.Add(go);
        }
        focusCharacterInfo = characters[0];
        UpdateVisualChar();
    }

    //Display in center // NOT USED
    public void DisplayCharacter(GameObject prefab)
    {
        if (character != null)
            Destroy(character);

        character = GameObject.Instantiate(prefab, visualCharacterParent.transform);

    }


    void UpdateVisualChar()
    {
        bool exist = GameManager.instance.profile.characters.Exists(x => x == focusCharacterInfo.itemName);

        uiPreBuyCharacter.Set(
            focusCharacterInfo.itemName,
            focusCharacterInfo.cost,
            focusCharacterInfo.isAdBuyable,
            () => { return GameManager.instance.profile.money >= focusCharacterInfo.cost; },
            exist,
            BuyCharacter
            );
    }

    void BuyCharacter()
    {
        GameManager.instance.profile.characters.Add(focusCharacterInfo.itemName);
        ChangeMoney(focusCharacterInfo.cost);
        SaveManager.SaveData(GameManager.instance.profile);

        UpdateVisualChar();
    }

    public void EquipCharacter()
    {
        GameManager.instance.player = focusCharacterInfo;
        GameManager.instance.profile.selectedCharacter = focusCharacterInfo.itemName;
        SaveManager.SaveData(GameManager.instance.profile);
        UpdateVisualChar();
    }

    public void MoveCharacters(int val)
    {
        if (!endedMoving) return;

        int tempVal = val;

        Debug.Log("Val: " + val);

        if (tempVal < 0)
        {
            if (focusCharIndex >= characters.Count - 1) return;

            if (tempVal < -1)
            {
                if (focusCharIndex - tempVal >= characters.Count - 1)
                {
                    // 10 - -3 = 13;

                    //t = 13 - 12 - 1;
                    int endPoint = focusCharIndex - tempVal;

                    int t = endPoint - (characters.Count - 1);


                    Debug.Log("Endpoint " + endPoint + ", T: " + t);
                    tempVal += t;
                }
            }
        }
        else if (tempVal > 0)
        {
            if (focusCharIndex <= 0) return;

            if (tempVal > 1)
            {
                if (focusCharIndex - tempVal <= 0)
                {

                    int t = focusCharIndex - tempVal;
                    tempVal += t;
                }

            }
        }


        Debug.Log("Dps Val " + tempVal);

        if (moveChars != null)
            StopCoroutine(moveChars);

        moveChars = StartCoroutine(MoveChars(tempVal));

        focusCharIndex -= tempVal;

        focusCharacterInfo = characters[focusCharIndex];

        UpdateVisualChar();
    }

    bool endedMoving = true;
    Coroutine moveChars;

    IEnumerator MoveChars(int val)
    {
        float t = 0;

        float duration = .5f;
        float step = 1 / duration;
        endedMoving = false;
        while (t < duration)
        {
            visualCharacterParent.transform.position += new Vector3(val * step, 0, 0) * Time.deltaTime;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        visualCharacterParent.transform.position = new Vector3(Mathf.RoundToInt(visualCharacterParent.transform.position.x), 0, 0);
        endedMoving = true;
    }

    #endregion

    #region BACKGROUND

    public void DisplayBackgrounds()
    {
        RectTransform rt = backgroundTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1920, 0);
        int i = 0;

        foreach (var bp in backgrounds)
        {
            bool exist = GameManager.instance.profile.backgrounds.Exists(x => x == bp.name);

            GameObject go = GameObject.Instantiate(uiBackgroundPrefab, backgroundTransform);

            bool equip = GameManager.instance.background == bp;
            //go.GetComponent<UIItemBackground>().Set(bp.prefab, exist);
            go.GetComponent<UIItemBackground>().Set(bp, exist, equip);
            i++;
            uiItems.Add(go);

            if (i * displaySize > 1920)
                rt.sizeDelta += new Vector2(displaySize, 0);
        }
    }

    public void DisplayBackground(BuyableBackground targeted, bool showName = true)
    {
        if (background != null)
            Destroy(background);

        focusBackgroundInfo = targeted;
        background = GameObject.Instantiate(focusBackgroundInfo.prefab, Vector3.zero, Quaternion.identity);
    }

    public UIItemBackground currentUIbackground;
    public void EquipBackground(BuyableBackground target, UIItemBackground newItem)
    {
        if (currentUIbackground != null)
            currentUIbackground.Set(currentUIbackground.buyable, true, false);

        if (newItem != null)
            currentUIbackground = newItem;

        GameManager.instance.background = target;
    }


    #endregion

    #region Music

    //Music State Machine
    UIItemMusic current;
    public void DisplayMusic()
    {
        //DestroyItems();
        uiPreBuyCharacter.gameObject.SetActive(false);
        RectTransform rt = musicTransform.GetComponent<RectTransform>();
        int i = 0;
        var ordered = songs.OrderBy(x => x.cost);
        foreach (var clip in ordered)
        {
            GameObject go = GameObject.Instantiate(uiMusicPrefab, musicTransform);

            bool exist = GameManager.instance.profile.musics.Exists(x => x == clip.itemName);
            bool equipped = GameManager.instance.profile.equippedSongs.Exists(x => x == clip.itemName);
            go.GetComponent<UIItemMusic>().Set(clip, exist, equipped);
            i++;
            uiItems.Add(go);

            if (i * displaySize > 1920)
                rt.sizeDelta += new Vector2(displaySize, 0);
        }
    }


    public void ChangeMusic(UIItemMusic item, BuyableSongs buyableSong = null)
    {
        if (current != null)
        {
            current.OffMusic();
            StopMusic();
        }
        if (item != null)
        {
            current = item;
            this.focusSongInfo = buyableSong;

            PlayMusic(current.song.clip);
        }
    }

    public void PlayMusic(AudioClip clip)
    {     
        playSource.clip = clip;
        playSource.Play();
    }
    public void StopMusic()
    {
        playSource.Stop();
    }

    public bool EquipSong(BuyableSongs song, bool equip)
    {
        if (equip)
        {
            GameManager.instance.musicPlayer.clips.Add(song.clip);
            GameManager.instance.profile.equippedSongs.Add(song.itemName);
            SaveManager.SaveData(GameManager.instance.profile);
            return true;
        }
        else
        {
            if (AreSongs())
            {
                GameManager.instance.musicPlayer.clips.Remove(song.clip);
                GameManager.instance.profile.equippedSongs.Remove(song.itemName);
                SaveManager.SaveData(GameManager.instance.profile);
                return true;
            }
            else
            {
                Debug.Log("No Se puede remover");
                StopAllCoroutines();
                StartCoroutine(SongLog());
                return false;
            }
        }
    }

    bool AreSongs()
    {
        return GameManager.instance.musicPlayer.clips.Count > 1;
    }

    IEnumerator SongLog()
    {
        songsLog.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        songsLog.SetActive(false);
    }

    
    #endregion
}
