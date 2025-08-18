using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersSpawner : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject otherPrefab;

    
    public static System.Action allPlayersInstantiated;

    public List<EntitySettings> charactersSettings;

    public List<EntitySettings> survivors = new List<EntitySettings>();

    private EntitySettings GetMainCharacterSetting()
    {
        var e = ScriptableObject.CreateInstance<EntitySettings>();
        e.prefab = GameManager.instance.player.prefab;
        e.characterImage = GameManager.instance.player.image;
        e.pushChance = 100;
        return e;
    }

    public GameObject GetRandomPrefab()
    {
        return charactersSettings.GetRandom().prefab;
    }

  
    public void SpawnAll(Elipse e, int playersAmount, float size = 1)
    {
        StartCoroutine(StartSpawning(e, playersAmount, size));
    }
    

    public void Cancel()
    {
        StopAllCoroutines();
    }

    IEnumerator StartSpawning(Elipse e, int amount, float size = 1)
    {
        int playerPos = Random.Range(0, amount);

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
                Debug.Log(ch == null);
                ch.ApplySettings(GetMainCharacterSetting());
                GameManager.instance.uiController.SetPlayer(ch);

            }
            else
            {
                go = GameObject.Instantiate(otherPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                go.name = "Other " + i;

                Vector3 dir = VectorHelp.Dir2D(go.transform.position, Vector3.zero);

                go.transform.LookAt(transform.position + dir * 2);
                BaseEntity ch = go.GetComponent<BaseEntity>();
              
                if (survCount == 0)
                {
                    var asd = charactersSettings.GetRandom();
                    Debug.Log(asd == null);
                    ch.ApplySettings(charactersSettings.GetRandom());
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
