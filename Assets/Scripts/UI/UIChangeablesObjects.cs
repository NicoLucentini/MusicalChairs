using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeablesObjects : MonoBehaviour
{
    public Image shopButton;
    public Image playButton;
    public Image freeCoinsButton;
    public Image moneyBackground;
    public Text moneyText;

    public GameStyleSettings settings;

    public void ChangeStyle() {

        playButton.color = settings.primaryColor;
        shopButton.color = freeCoinsButton.color = settings.secondaryColor;
        moneyBackground.color = settings.thirdColor;
        moneyText.color = settings.secondaryFont;

    }
}
