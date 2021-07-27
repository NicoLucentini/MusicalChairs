using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable Songs", menuName = "Buyable Songs", order = 1)]
public class BuyableSongs : BuyableSettings
{
    public AudioClip clip;

    /*
    public override void OnAfterDeserialize()
    {
        Debug.Log("Nothing");
      
    }

    public override void OnBeforeSerialize()
    {     
        itemName = clip.name;
    }
    */
}