using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Entity : MonoBehaviour{

}

public class Character : Entity
{

    

    NavMeshAgent agent;
    CharacterController cc;
    Animation anim;

    List<Transform> waypoints = new List<Transform>();
    int waypointIndex;
    float speed = 3f;

    Chair targetChair;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();

    }
    private void Start()
    {
        waypoints.AddRange(GameManager.instance.waypoints);

        if (waypoints.Count > 0)
        {
            var temps = waypoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position));
            Transform temp = temps.First(x => VectorHelp.CheckSide(transform.position, x.position - transform.position, transform.up) > 0);

            waypointIndex = GameManager.instance.waypoints.IndexOf(temp);

            agent.SetDestination(waypoints[waypointIndex].position);
        }

    }
   
    public void CreateVisual(GameObject prefab) {
        GameObject go = GameObject.Instantiate(prefab, transform);
        anim = go.GetComponent<Animation>();
    }

    private void OnEnable()
    {

        Chair.onChairOccuped += OnChairOccuped;
        /*
        GameManager.allChairsOccuped += AllChairsOccuped;
        MusicPlayer.onMusicStopped += OnMusicStopped;
        Banana.onGetHit += OnBananaHit;
        */
        //PlayersSpawner.allPlayersInstantiated += () => thinkCoroutine = StartCoroutine(Think(1.5f, CheckDistance));
    }
    private void OnDestroy()
    {

        Chair.onChairOccuped -= OnChairOccuped;
        /*
        GameManager.instance.players.Remove(this);
        GameManager.allChairsOccuped -= AllChairsOccuped;
        MusicPlayer.onMusicStopped -= OnMusicStopped;
        Banana.onGetHit -= OnBananaHit;
        */
        // PlayersSpawner.allPlayersInstantiated -= () => thinkCoroutine = StartCoroutine(Think(1.5f, CheckDistance));
    }


    void OnChairOccuped(Chair temp)
    {

        Debug.Log($"La silla {temp.gameObject.name} fue ocupada");
        if (targetChair != temp) return;
        if (targetChair.owner == this) return;

        Debug.Log($"Mi silla ({gameObject.name}) la ocuparon " + temp.gameObject.name);

        anim.Play("idle");

        //Busca una nueva silla
        Sit();
    }

    void Update() {
        if(targetChair != null){

            if (Vector3.Distance(transform.position, targetChair.transform.position) < .5f) {
                agent.Warp(targetChair.transform.position);
            }
        }

        if (targetChair == null) {
            if (agent.remainingDistance < .5f) {
                agent.SetDestination(GetNextWaypointPos());
            }
        }
    }

    public void Sit() {
        Debug.Log("Sit");
        targetChair = SearchClosestChair();
        agent.SetDestination(targetChair.transform.position);
    }
    
    void MoveTo(Vector3 pos) {

        Vector3 direction = (pos - transform.position).normalized;
        agent.SetDestination(pos);
       // cc.Move(direction * speed * Time.deltaTime);
    }

    void Move(Vector3 targetPos, System.Action onArrived = null) {
        Vector3 direction = (targetPos - transform.position).normalized;
        cc.Move(direction * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < .5f) {
            onArrived();
        }
    }

    Chair SearchClosestChair()
    {
        return GameManager.instance.chairs
            .Where(x => !x.occuped)
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .FirstOrDefault();
    }

    public void GoNext()
    {
        if (waypointIndex < waypoints.Count - 1)
            waypointIndex++;
        else
            waypointIndex = 0;
    }

    public Vector3 GetNextWaypointPos() {
        if (waypointIndex < waypoints.Count - 1)
            waypointIndex++;
        else
            waypointIndex = 0;

        return waypoints[waypointIndex].position;


    }

}
