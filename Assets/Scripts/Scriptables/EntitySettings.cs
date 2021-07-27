using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Entity Settings", order = 1)]
public class EntitySettings : ScriptableObject
{
    public GameObject prefab;
    public float baseSpeed = 1;
    public float maxSpeed = 1.5f;
    public float minSpeed = 0.5f;
    public float accel = .1f;
    public Vector2 maxSpeedVariation = new Vector2(-.2f, .2f);
    public Vector2 reactionTime;
  
    public Vector2 speedToChair;
    public float pushChance;
    public float jumpChance;

    public Sprite characterImage;
}
