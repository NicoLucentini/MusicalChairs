using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [Header("Inspector Set")]    
    public Button jumpButton;
    public Button plusButton;
    public Button minusButton;
    public Button sitButton;
    public Button sitAutoButton;

    [Header("Runtime Set")]
    public BaseEntity player;

    private void OnEnable()
    {
        MusicPlayer.onMusicStopped += StopAllCoroutines;
    }

    public void SetPlayer(BaseEntity player)
    {
        this.player = player;
    }

    public void Sit()
    {
        StopAllCoroutines();
        if(player!= null)
            player.OnClickSit();
    }
    
    public void LeftDown()
    {
        StopAllCoroutines();
        if (player != null)
            StartCoroutine(CT(player.Desaccelerate));
    }
    public void AnyUp()
    {
        StopAllCoroutines();
        if (player != null)
            player.Release();
    }
    public void RightDown()
    {
        StopAllCoroutines();
        if (player)
            StartCoroutine(CT(player.Accelerate));
    }

    public void Jump()
    {
        StopAllCoroutines();
        if (player)
            player.Jump();
    }

    IEnumerator CT(System.Action func)
    {
        while (true)
        {
            func();
            yield return new WaitForEndOfFrame();
        }
    }
}
