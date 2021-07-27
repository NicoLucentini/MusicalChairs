using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{

    public GameObject endGo;
    public GameObject adButton;
    public Text endText;
    public GameObject restartButton;

    public Text chairs;
    public Text reactionTime;
    public Text continueText;

    public Text restartText;

    public Text scoreText;
    public Text endGameText;

    public void Restart(bool on,string val = "0")
    {
        restartText.gameObject.SetActive(on);
        restartText.text = val;
        restartText.GetComponent<Animation>().Play();
    }
    public void RestartTimer()
    {
       //StartCoroutine(CTRestartTimer());
    }
    public void SetRestartButton(bool on, string val = "Restart") {

        restartButton.gameObject.SetActive(on);
        endGo.SetActive(on);
        endGameText.text = val;
    }

   
    public void ChangeChairsText(string val)
    {
        chairs.text =  val;
    }
    public void ChangeReactionText(string val)
    {
        reactionTime.text = val;
    }

    Coroutine continueCt;
    public void Continue()
    {
        continueCt = StartCoroutine(CTContinue());
    }

    IEnumerator CTContinue()
    {
        int t = 5;
        Debug.Log("Ct Continue Start");
        while (t > 0)
        {
            continueText.text = "Continue " + t;
            Debug.Log($"Continue{t}" );
            t--;
            yield return new WaitForSeconds(1);
        }
        
        Debug.Log("Ct Continue End");
        adButton.SetActive(false);
        restartButton.SetActive(true);
        SetRestartButton(true, "Play Again");
        
        //mostar el score?
    }

    public void StopContinueCt() {
        if (continueCt != null)
            StopCoroutine(continueCt);
    }
    public void ChangeEndText(bool on = true, string msg = "", float duration = 0f, bool showRestartButton = false, bool showImg = true, bool showAdButton = false)
    {
        CancelInvoke("OffEndText");
        endGo.SetActive(on);
        endText.text = msg;
        restartButton.SetActive(showRestartButton);

        endGo.GetComponent<Image>().enabled = showImg;
        adButton.SetActive(showAdButton);

        /*
        if (duration != 0)
            Invoke("OffEndText", duration);
            */
    }
    void OffEndText()
    {
        CancelInvoke("OffEndText");
        ChangeEndText(false, "");
    }
    public void ShowScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
