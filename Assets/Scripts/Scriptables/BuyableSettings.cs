using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable", menuName = "Buyable Settings", order = 1)]
public class BuyableSettings : ScriptableObject//, ISerializationCallbackReceiver
{

    public string itemName;
    public int cost;
    public bool isAdBuyable;

    /*
    public virtual void OnAfterDeserialize()
    {
        Debug.Log("OnAfterDeserialize");
    }

    public virtual void OnBeforeSerialize()
    {
        Debug.Log("OnBeforeSerialize");
    }
    */
}



//hago esto porq me pinta flashear cosas


public enum BuyableStatus
{
    EQUIPPED,
    OWNED,
    NOT_OWNED,
    UNDISCOVERED,
}

/*
[System.Serializable]
public abstract class Buyable 
{
    public string itemName;
    public int cost;
    public bool isAdBuyable;
    
}
public class BuyableChar
{

}
public class BuyableSong
{

}
public class BuyableBackground
{

}
*/