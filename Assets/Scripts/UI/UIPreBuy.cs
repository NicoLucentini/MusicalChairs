using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPreBuy : MonoBehaviour
{

    public Text itemName;
    public Button buyWithCoins;
    public Button buyWithAd;
    public Button close;
    public Text costText;
    public GameObject or;
    public Button equipButton;

    public bool owned = false;
    bool adBuyable = false;
    //tengo q ver el item q tengo q comprar no se bien como hacer eso...
    
    public System.Action onBuySuccesful;
    public System.Func<bool> checkMoney;

    private void Awake()
    {
        buyWithCoins.onClick.AddListener(() => OnClickBuyWithMoney());
        buyWithAd.onClick.AddListener(() => OnClickBuyWithAd());
        close.onClick.AddListener(() => OnClickClose());
        
    }

    public void OnClickClose()
    {
        Close();
    }

    public void OnClickBuyWithAd()
    {
        AdManager.instance.AdShow("rewardedVideo", OnSucces, null);
    }

    public void OnClickBuyWithMoney()
    {

        if (checkMoney())
            OnSucces();
        else
            Menu.instance.ShowBlocker(true, "No Tienes suficientes monedas!");
    }

    void Close()
    {
        gameObject.SetActive(false);
    }

    public void Set(string txt, int cost, bool adBuyable, System.Func<bool> check, bool owned = false, System.Action onBuy = null)
    {
        if (onBuy == null)
            onBuySuccesful = UpdateOwned;
        else
            onBuySuccesful = onBuy;

        checkMoney = check;
        itemName.text = txt;
        costText.text = "$ " + cost.ToString();
        this.owned = owned;
        this.adBuyable = adBuyable;
        buyWithAd.gameObject.SetActive(adBuyable);
        UpdateOwned();
       
    }

    void OnSucces()
    {
        if (onBuySuccesful != null)
            onBuySuccesful();

        Menu.instance.ShowBlocker(true, "¡CHARACTER UNLOCKED!\n" + "NEW\n" + itemName.text, UpdateOwned, 1f);

        //UpdateOwned();
    }

    public void OnEquip()
    {
        GameManager.instance.shopManager.EquipCharacter();
        //equipButton.GetComponentInChildren<Text>().text = "Equipped";
    }

    public void UpdateOwned()
    {
        bool show = owned && GameManager.instance.player.itemName != itemName.text;

        bool showAdButton = !owned && adBuyable;


        if (!owned )
        {
            RectTransform rt = buyWithCoins.GetComponent<RectTransform>();

            if(!showAdButton)
                rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
            else
                rt.anchoredPosition = new Vector2(-105, rt.anchoredPosition.y);
        }

        equipButton.gameObject.SetActive(show);
        buyWithCoins.gameObject.SetActive(!owned);
        buyWithAd.gameObject.SetActive(showAdButton);
        or.SetActive(showAdButton);
    }

}
