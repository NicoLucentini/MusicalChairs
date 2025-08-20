using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [Header("Inspector Set")]    
    public Button jumpButton;
    public Button plusButton;
    public Button minusButton;
    public Button sitButton;
    public Button sitAutoButton;


    public static System.Action OnClickSit;
    public static System.Action OnClickLeft;
    public static System.Action OnClickRight;
    public static System.Action OnClickJump;

    private void OnEnable()
    {
        MusicPlayer.onMusicStopped += StopAllCoroutines;
    }


    public void Sit()
    {
        OnClickSit?.Invoke();
    }
    
    public void LeftDown()
    {
        OnClickLeft?.Invoke();
    }
    public void RightDown()
    {
        OnClickRight?.Invoke();
    }

    public void Jump()
    {
        OnClickJump?.Invoke();
    }
}
