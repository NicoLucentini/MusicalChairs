using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaterialBank : MonoBehaviour
{
    [SerializeField]private List<Material> allMats = new List<Material>();
    [SerializeField]private List<GameObject> characters = new List<GameObject>();
    [SerializeField]private List<EntitySettings> entitySettings = new List<EntitySettings>();

    public Material GetRandomMaterial() => allMats.Random();
    public GameObject GetRandomChar() => characters.Random();
    public EntitySettings GetRandomSetting() => entitySettings.Random();
}
