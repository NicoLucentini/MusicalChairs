using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersSpawner : MonoBehaviour {

    public Transform spawn;
    public GameObject playerPrefab;
    public GameObject otherPrefab;


    public List<EntitySettings> charactersSettings;

    public List<EntitySettings> survivors = new List<EntitySettings>();

    public EntitySettings GetMainCharacterSetting()
    {
        var e = ScriptableObject.CreateInstance<EntitySettings>();
        e.prefab = GameManager.instance.player.prefab;
        e.characterImage = GameManager.instance.player.image;
        return e;
    }

    public GameObject GetRandomPrefab()
    {
        return charactersPrefab[Random.Range(0, charactersPrefab.Count)];
    }

    public int amountChairs = 0;
    public int freq = 1;
  
    public void SpawnAll(Elipse e, float size = 1)
    {
        amountChairs = GameManager.instance.chairs.Count + 1;
        StartCoroutine(StartSpawning(e, amountChairs, size));
    }

    public void Cancel()
    {
        StopAllCoroutines();
    }
    public static System.Action allPlayersInstantiated;

    IEnumerator StartSpawning(Elipse e, int amount, float size = 1)
    {
        int playerPos = Random.Range(0, amountChairs);

        GameObject go = null;
        BaseEntity be = null;

        int survCount = survivors.Count;
        int survIndex = 0;
        for (int i = 0; i < amount; i++)
        {
            go = null;
            float angle = ((float)i / (float)amount);
            Vector2 pos = Elipse.Evaluate(angle, e.xAxis * size, e.yAxis *size);

            //Creo el player
            if (i == playerPos)
            {
                go = GameObject.Instantiate(playerPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);              
                go.name = "Player " + i;
                
                BaseEntity ch = go.GetComponent<BaseEntity>();
                ch.ApplySettings(GetMainCharacterSetting());
                //GameManager.instance.uiController.SetCharacter(ch);
                GameManager.instance.uiController.SetPlayer(ch);

            }
            //Creo la ia
            else
            {
                go = GameObject.Instantiate(otherPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                go.name = "Other " + i;

                Vector3 dir = VectorHelp.Dir2D(go.transform.position, Vector3.zero);

                go.transform.LookAt(transform.position + dir * 2);
                BaseEntity ch = go.GetComponent<BaseEntity>();
              
                if (survCount == 0)
                {
                    ch.ApplySettings(GetRandomSetting());
                }
                else
                {
                    ch.ApplySettings(survivors[survIndex]);
                    survIndex++;
                }

            }

         
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log("All players instantiated");
        
        allPlayersInstantiated?.Invoke();
    }
}
