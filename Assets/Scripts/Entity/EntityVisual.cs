using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityVisual : MonoBehaviour
{
    public List<Renderer> renderers = new List<Renderer>();

    public Material newSkin;

   
    private void Start()
    {
        renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    public void ChangeSkin()
    {
        ChangeSkin(newSkin);
    }
    public void ChangeSkin(Material mat)
    {
        foreach (var r in renderers)
            r.material = mat;    
    }

}
