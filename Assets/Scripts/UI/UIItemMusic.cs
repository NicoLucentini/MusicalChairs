using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIItemMusic : MonoBehaviour
{
    //REFERENCES
    public Text musicName;
    public Image playAndStopImage;
    public Button buyButton;
    public Button playButton;

    public Text lowButtonText;
    public Image lowButtonImage;
    public Image lowButtonAdImage;
    //SET
    public BuyableSongs song;
    bool owned = false;
    System.Action onClickButton;

    System.Action onClickLowButton;

    private void Awake()
    {
        lowButtonText = buyButton.GetComponentInChildren<Text>();
        lowButtonImage = buyButton.GetComponent<Image>();
    }

   

    public void BuyWithMoney()
    {
        if (GameManager.instance.profile.money < song.cost)
        {
            Menu.instance.ShowBlocker(true, "No Tienes suficientes monedas !");
            return;
        }
        
        GameManager.instance.shopManager.ChangeMoney(song.cost);
        OnItemBuy();

    }
    public void BuyWithAd()
    {
        AdManager.instance.AdShow("rewardedVideo", OnItemBuy, null);
    }
   

    public void OnClickLowButton()
    {
        if (onClickLowButton != null)
            onClickLowButton();
    }
    void OnItemBuy()
    {
        GameManager.instance.profile.musics.Add(song.name);
        SaveManager.SaveData(GameManager.instance.profile);

        Menu.instance.ShowBlocker(true, "¡MUSIC UNLOCKED! \n " + "\n" +
            "SONG: " + song.itemName, AfterBlocker, 1f);
    }
    void AfterBlocker()
    {
        lowButtonAdImage.enabled = false;
        lowButtonText.enabled = true;
        lowButtonImage.color = GameManager.instance.shopManager.equipColor;
        lowButtonText.text = "Equip";
        onClickLowButton = Equip;
    }
        
    void Equip()
    {
        GameManager.instance.shopManager.EquipSong(song, true);
        lowButtonText.text = "Remove";
        lowButtonImage.color = GameManager.instance.shopManager.unequipColor;
        onClickLowButton = UnEquip;
    }
    void UnEquip()
    {
        if (GameManager.instance.shopManager.EquipSong(song, false))
        {
            lowButtonImage.color = GameManager.instance.shopManager.equipColor;
            lowButtonText.text = "Equip";
            onClickLowButton = Equip;
        }
    }

    public void Set(BuyableSongs song, bool owned = false, bool equipped = false)
    {
        this.song = song;
        this.owned = owned;
        musicName.text = song.name;
        playButton.onClick.AddListener(()=>OnClickButton());

        if (owned)
        {
            if (!equipped)
            {
                lowButtonText.text = "Equip";
                lowButtonImage.color = GameManager.instance.shopManager.equipColor;
                onClickLowButton = Equip;

            }
            else
            {
                lowButtonText.text = "Remove";
                onClickLowButton = UnEquip;
                lowButtonImage.color = GameManager.instance.shopManager.unequipColor;
            }
        }
        else
        {
            if (!song.isAdBuyable)
            {
                lowButtonText.text = "$ " + song.cost;
                lowButtonAdImage.enabled = false;
                onClickLowButton = BuyWithMoney;
            }
            else
            {
                lowButtonText.enabled = false;
                lowButtonAdImage.enabled = true;
                onClickLowButton = BuyWithAd;
            }
            lowButtonImage.color = GameManager.instance.shopManager.buyColor;
            //onClickLowButton = Buy;
        }
          

        onClickButton = Play;
    }

    void OnClickButton()
    {
        if (onClickButton != null)
            onClickButton();
    }

    public void Play()
    {
        GameManager.instance.shopManager.ChangeMusic(this, this.song);
       // GameManager.instance.shopManager.PlayMusic(clip);
        playAndStopImage.sprite = GameManager.instance.shopManager.stopMusic;
        onClickButton = Stop;

    }
   
    public void Stop()
    {
        GameManager.instance.shopManager.ChangeMusic(null);
        OffMusic();
    }

    public void OffMusic()
    {
        playAndStopImage.sprite = GameManager.instance.shopManager.playMusic;
        onClickButton = Play;

    }
}
