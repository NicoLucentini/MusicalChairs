using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBackground : MonoBehaviour
{

    public Text itemName;
    public Image playAndStopImage;
    public Button buyButton;
    public Button playButton;

    public Text buyText;

    public string chName;
    private void Awake()
    {
        buyText = buyButton.GetComponentInChildren<Text>();
    }
    public BuyableBackground buyable;

    public void Set(BuyableBackground buyable, bool owned = false, bool equipped = false)
    {
        this.buyable = buyable;

        itemName.text = buyable.itemName;
        playButton.onClick.AddListener(Show);

        if (owned)
        {
            if (equipped)
            {
                buyText.text = "Selected";
                buyButton.GetComponent<Image>().color = GameManager.instance.shopManager.unequipColor;
                if(GameManager.instance.shopManager.currentUIbackground == null)
                    GameManager.instance.shopManager.EquipBackground(buyable, this);
            }
            else
            {
                buyText.text = "Equip";
                buyButton.GetComponent<Image>().color = GameManager.instance.shopManager.equipColor;
                lowButtonAction = Equip;
            }

        }
        else
        {
            buyText.text = "$ " + buyable.cost;
            buyButton.GetComponent<Image>().color = GameManager.instance.shopManager.buyColor;
            lowButtonAction = Buy;
        }
    }
    public void Unequip()
    {
        buyText.text = "Equip";
        buyButton.GetComponent<Image>().color = GameManager.instance.shopManager.equipColor;
        lowButtonAction = Equip;
    }
    public void Show()
    {
        //GameManager.instance.shopManager.uiPreBuyBackground.gameObject.SetActive(true);

        GameManager.instance.shopManager.DisplayBackground(buyable);
    }
    
    System.Action lowButtonAction;

    public void OnClickLowButton()
    {
        if (lowButtonAction != null)
            lowButtonAction();

    }
    public void Buy()
    {
        if (GameManager.instance.profile.money > buyable.cost)
        {
            GameManager.instance.shopManager.ChangeMoney(buyable.cost);
            lowButtonAction = Equip;
            GameManager.instance.profile.backgrounds.Add(buyable.itemName);
            SaveManager.SaveData(GameManager.instance.profile);
          

            Menu.instance.ShowBlocker(true, "¡LOCATION UNLOCKED! \n " + "\n" +
          "THEME :" + buyable.itemName, OnItemBuy, 1f);

        }
        else
        {
            Menu.instance.ShowBlocker(true, "No tienes suficientes monedas");
            Debug.Log("No hay plata");
        }
    }

    void OnItemBuy()
    {
        Set(buyable, true, false);
    }

    public void Equip()
    {
        GameManager.instance.background = buyable;
        GameManager.instance.profile.selectedBackground = buyable.itemName;
        SaveManager.SaveData(GameManager.instance.profile);
        GameManager.instance.shopManager.EquipBackground(buyable, this);
        GameManager.instance.shopManager.DisplayBackground(buyable);
        Set(buyable, true, true);
        Debug.Log("Equip");
    }
  
}
