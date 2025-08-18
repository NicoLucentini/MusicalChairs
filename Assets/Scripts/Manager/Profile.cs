using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile 
{
    public List<string> musics = new List<string>();
    public List<string> characters = new List<string>();
    public List<string> backgrounds = new List<string>();

    public List<string> equippedSongs = new List<string>();

    public string selectedCharacter;
    public string selectedBackground;

    public int money;
    public float totalReactionTime;
    public int roundsPlayed;
    public System.DateTime last;
}
