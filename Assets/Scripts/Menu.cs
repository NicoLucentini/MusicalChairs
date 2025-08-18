using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Menu : MonoBehaviour
{

    public static Menu instance;


    System.Action onClickClose;

    public GameObject gamePanel;
    public GameObject shopPanel;
    public GameObject menuPanel;

    public CameraScript cameraScript;

    [Header("Blocker")]
    public GameObject blocker;
    public Text blockerText;
    public GameObject blockerImage;
    public GameObject blockerClose;
    public GameObject blockerBigButton;
    public static System.Action onBlockerOut;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        OnClickMenu();
    }
    
    public void ShowBlocker(bool on = true, string msg = "",  System.Action action = null, float time = 0)
    {
        blocker.SetActive(on);

        if (time > 0)
            StartCoroutine(BlockerClose(time));
        else
            blockerClose.SetActive(true);

        if (msg != "")
        {
            blockerText.text = msg;
            blockerImage.SetActive(true);
            blockerBigButton.SetActive(true);
        }
        else
        {
            blockerImage.SetActive(false);
            blockerBigButton.SetActive(false);

        }

        onBlockerOut = null;
        onBlockerOut = action;
    }
    IEnumerator BlockerClose(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        blockerBigButton.SetActive(true);
        blockerClose.SetActive(true);
    }
    public void OnClickBlocker()
    {
        blocker.SetActive(false);
        if (onBlockerOut != null)
            onBlockerOut();
    }

    public void OnClickPlay()
    {
        gamePanel.SetActive(true);
        shopPanel.SetActive(false);
        menuPanel.SetActive(false);

        GameManager.instance.shopManager.OnGoingToGame();

        GameManager.instance.Restart();
        cameraScript.Change(false);

        onClickClose = () => { GameManager.instance.GoToMenu(); OnClickMenu(); };
    }

    bool shopOff = true;
    public void OnClickShop()
    {
        shopOff = !shopOff;

        Animation anim = shopPanel.GetComponent<Animation>();
        anim.Play(shopOff ? "shopout" : "shopin");

        shopPanel.SetActive(true);
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);

        GameManager.instance.shopManager.OnShop(!shopOff);

        if (shopOff)
            OnClickMenu();

        onClickClose = () => 
        {
            shopOff = true;
            Animation a = shopPanel.GetComponent<Animation>();
            a.Play("shopout");

            OnClickMenu();
            GameManager.instance.shopManager.OnShop(!shopOff);
        };
    }
    /*
    Vector2 offShop = new Vector2(0, 0);
    Vector2 onShop = new Vector2(0, 392);

    IEnumerator ShopLerp(bool shopOn)
    {
        //
        Animation anim = shopPanel.GetComponent<Animation>();

        if (shopOn)
            anim.Play("shopout");
        else
            anim.Play("shopin");

        yield break;
    }
    */
    public void OnClickMenu()
    {        
        menuPanel.SetActive(true);
        shopPanel.SetActive(true);
        gamePanel.SetActive(false);

        cameraScript.Change(true);
        GameManager.instance.shopManager.OnGoingToMenu();
        Time.timeScale = 1;
        onClickClose = ()=>Application.Quit();
    }
    public void OnClickClose()
    {
        if (onClickClose != null)
            onClickClose();
    }

}
