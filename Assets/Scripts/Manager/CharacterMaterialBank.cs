using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaterialBank : MonoBehaviour
{
    public List<Material> allMats = new List<Material>();
    public List<GameObject> characters = new List<GameObject>();
    public List<EntitySettings> entitySettings = new List<EntitySettings>();

    public Material GetRandom()
    {
        return allMats[Random.Range(0, allMats.Count)];
    }
    public GameObject GetRandomChar()
    {
        return characters[Random.Range(0, characters.Count)];
    }
    public EntitySettings GetRandomSetting()
    {
        return entitySettings[Random.Range(0, entitySettings.Count)];
    }
}
