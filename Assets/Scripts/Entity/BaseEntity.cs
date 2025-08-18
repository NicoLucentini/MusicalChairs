using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EntityState
{
    WALKING,
    TO_CHAIR,
    SEATED
}
public class BaseEntity : Entity
{
    private List<Transform> waypoints = new List<Transform>();

    [Header("References")]

    [SerializeField]private GameObject baseVisual;
    private Animation anim;
    private Rigidbody rb;
    private Collider myCollider;
    private NavMeshAgent agent;
    
    [Header("Sit")]
    [ReadOnly] public Chair targetChair;
    
    [Header("Custom Properties")]
    public EntitySettings settings;

    [Header("Properties")]
    public bool isHuman;

    [Header("Movement")]

    [ReadOnly] public float speed = 1;
    [ReadOnly] public bool stop;
    [ReadOnly] public bool onAir = false;
    [ReadOnly] public bool onFloor = false;


    public float baseSpeed = 1;
    public float maxSpeed = 1.5f;
    public float minSpeed = 0.5f;
    public float accel = .1f;
    public Vector2 maxSpeedVariation = new Vector2(-.2f, .2f);
    public float offset = .2f;
    public bool useSlerp;
    
    private float slowDistance = 1.25f;

    [ReadOnly] public int waypointIndex = 0;
    [ReadOnly] public Transform target;


    [Header("Empujar")]
    public float pushCd = 1.5f;
    public float pushDistance;
    public float pushFallDuration = 1.5f;
    
    [ReadOnly] public bool hasAttacked = true;
    [ReadOnly] public bool hasBeenAttacked = false;
    [ReadOnly] public BaseEntity visualizeEnemy;
    public Ray pushRay;
    public RaycastHit pushHit;

    [Header("Banan")]
    public float fallDuration;
    [ReadOnly] public Transform lastBananaDodge;
    [ReadOnly] public Transform visualizeBanana;
    [ReadOnly] public bool triesToJump = false;
    [ReadOnly] public bool canFallWithBanana = true;

    [Header("Corutinas")]
    Coroutine ct_release;
    Coroutine ct_think;
    Coroutine ct_checkClose;
    Coroutine ct_closestChair;
    Coroutine ct_rays;


    private Action stateAction;
    public EntityState state;
    private Action<Vector3> lookAtPosAction;

    #region UNITY
    float Y => transform.position.y;

    void ChangeState(EntityState state, Action action)
    {
        this.state = state;
        this.stateAction = action;
    }

    private void Awake()
    {
        GameManager.instance.players.Add(this);
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        
        GameManager.allChairsOccuped += AllChairsOccuped;
        MusicPlayer.onMusicStopped += OnMusicStopped;
        Chair.onChairOccuped += OnChairOccuped;
        Banana.onGetHit += OnBananaHit;
    }
   
    private void OnDestroy()
    {
        GameManager.instance.players.Remove(this);
        GameManager.allChairsOccuped -= AllChairsOccuped;
        MusicPlayer.onMusicStopped -= OnMusicStopped;
        Banana.onGetHit -= OnBananaHit;
    }

    public bool IsSeated() => state == EntityState.SEATED;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 11)
        {
            if (canFallWithBanana)
            {
                Fall();
                Banana bn = collision.gameObject.GetComponent<Banana>();
                bn.DestroyThis();
                canFallWithBanana = false;
                StartCoroutine(CTTime(5, () => { canFallWithBanana = true; }));
            }
        }
    } 
    
    public void ApplySettings(EntitySettings settings)
    {
        this.settings = settings;
        
        GameObject go = Instantiate(settings.prefab, transform);
        anim = go.GetComponent<Animation>();
    }


    Transform GetClosestAndAheadWaypoint()
    {
      return waypoints
          .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
          .First(x => VectorHelp.CheckSide(transform.position, x.position - transform.position, transform.up) > 0);
    }

    private void Start()
    {
        rb.Sleep();
        waypoints.AddRange(GameManager.instance.waypoints);

        if (waypoints.Count > 0)
        {
            target = GetClosestAndAheadWaypoint();
            waypointIndex = GameManager.instance.waypoints.IndexOf(target);
        }

        if (isHuman)
        {
            speed = baseSpeed;
        }
        else
        {
            baseVisual.SetActive(false);
            float variation = Random.Range(maxSpeedVariation.x, maxSpeedVariation.y);
            speed = baseSpeed + variation;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }

        myCollider = GetComponentInChildren<Collider>();
      
        DoInTime(Random.Range(2, 5), () => { hasAttacked = false; });

        ct_closestChair = StartCoroutine(Think(.5f,  SearchClosestChair));

        ChangeState(EntityState.WALKING, StateWalking);

        lookAtPosAction = StrategyForLookAt();
    }

    Action<Vector3> StrategyForLookAt()
    {
        return useSlerp ? LookAtPosSlerped : (Action<Vector3>)LookAtPosLegacy;
    }

    public void Update()
    {
        stateAction();
    }

    void StateAfk() { }

    void StateWalking()
    {
        if (MusicPlayer.isRunning)
        {
            CheckForEnemyAndBanana();
            CheckCloseToSlow(); // bot
            EvadeFloorThings(); // bot
            CheckPushDistance();
        }

        MoveToWaypoint();
    }
   

    
   
    #endregion

    #region EVENTS

    void OnMusicStopped()
    {
        if (ct_checkClose != null)
            StopCoroutine(ct_checkClose);
        
        
        rb.constraints = RigidbodyConstraints.None;
        rb.linearVelocity = Vector3.zero;
        rb.Sleep();

        if (isHuman) return;
        
        baseVisual.SetActive(false);
        Sit();
    }

    void AllChairsOccuped()
    {
        Debug.Log($"AllChairsOccuped I'm last {gameObject.name}");
        Last();
    }
    void OnBananaHit(Transform t)
    {
        if (visualizeBanana == t)
            visualizeBanana = null;
    }
    void OnChairOccuped(Chair occupedChair)
    {
        if (targetChair != occupedChair) return;

        Debug.Log($"Mi silla ({gameObject.name}) la ocuparon " + occupedChair.gameObject.name);

        SearchClosestChair();
    }


    #endregion

    #region Accel
    public void Accelerate()
    {
        ChangeSpeed(1f);
    }
    public void Desaccelerate()
    {
        ChangeSpeed(-1f);
    }
    private void ChangeSpeed(float forward)
    {
        speed += accel * forward * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        anim["walk"].speed = speed;
    }

    public void Release()
    {
        if (ct_release != null)
            StopCoroutine(ct_release);

        ct_release = StartCoroutine(CTRelease());
    }
    public IEnumerator CTRelease()
    {
        Debug.Log("Corutina de release");
        float sign = Mathf.Sign(speed - 1);
        float off = .1f;
        float dif = speed - 1;

        while (Mathf.Abs(dif) > off)
        {
            speed += accel * -sign * Time.deltaTime;
            dif = speed - 1;

            yield return new WaitForEndOfFrame();
        }
        speed = 1;
        anim["walk"].speed = speed;
    }
    #endregion

    #region FALL

    private void Fall()
    {
        StartCoroutine(CTFall(0, fallDuration));
    }

    #endregion

    #region PUSH

    private void CheckCloseToSlow()
    {
        if (isHuman) return;

        if (visualizeEnemy != null)
        {
            if (Vector3.Distance(transform.position, visualizeEnemy.transform.position) < slowDistance)
                Desaccelerate();
        }
        else
        {
            Accelerate();
        }
    }

    private void CheckPushDistance()
    {
        if (hasAttacked || onFloor || onAir) return;

        if (visualizeEnemy == null) return;

        if (visualizeEnemy.onAir || visualizeEnemy.onFloor || visualizeEnemy.hasBeenAttacked) return;

        if (Vector3.Distance(transform.position, visualizeEnemy.transform.position) < pushDistance)
        {
            hasAttacked = true;

            if (Random.Range(0, 100) <= settings.pushChance)
            {
                Push(visualizeEnemy.transform);
            }

            DoInTime(pushCd, () => { hasAttacked = false; });

        }
    }

    void DoInTime(float t, System.Action func) =>
        StartCoroutine(CTTime(t, func));


    void WalkAnim()
    {
        if (IsSeated()) return;

        CancelInvoke("WalkAnim");
        anim.Play("walk");
    }

    private void Push(Transform t)
    {
        anim.Play("attack");
        stop = true;
        float animLength = anim["attack"].length;
        Invoke("WalkAnim", animLength);
        t.GetComponent<BaseEntity>().GetPushed(animLength / 2, transform);
        StartCoroutine(CTTime(animLength / 2, () => { stop = false; }));

    }

    private void GetPushed(float inTime, Transform pushed = null)
    {
        hasBeenAttacked = true;
        StartCoroutine(CTFall(inTime, pushFallDuration));
    }
    #endregion

    #region MOVEMENT

    private int layerMaskEnemyAndBanana = 1 << 10 | 1 << 11;
    
    void CheckForEnemyAndBanana()
    {
        float totalDistance = 0;
        RaycastHit hit;
        BaseEntity enemy = null;
        Transform banana = null;
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 to = waypoints[waypointIndex].position + new Vector3(0, 0.4f, 0);

        totalDistance += Vector3.Distance(origin, to);


        if (Physics.Linecast(origin, to, out hit, layerMaskEnemyAndBanana))
        {
            if (hit.collider.gameObject.layer == 10)
                enemy = hit.collider.GetComponent<BaseEntity>();
            else if (hit.collider.gameObject.layer == 11)
                banana = hit.collider.transform;

        }

        Debug.DrawLine(origin, to, Color.blue);


        if (enemy == null)
        {

            for (int i = 0; i < 5; i++)
            {

                totalDistance += Vector3.Distance(origin, to);

                if (totalDistance > 3)
                    break;

                int originIndex = GetWp(waypointIndex + i);
                origin = waypoints[originIndex].position + new Vector3(0, 0.4f, 0);

                int toIndex = GetWp(waypointIndex + i + 1);
                to = waypoints[toIndex].position + new Vector3(0, 0.4f, 0);
                Debug.DrawLine(origin, to, Color.blue);

                if (Physics.Linecast(origin, to, out hit, layerMaskEnemyAndBanana))
                {
                    if (hit.collider.gameObject.layer == 10)
                        enemy = hit.collider.GetComponent<BaseEntity>();
                    else if (hit.collider.gameObject.layer == 11)
                        banana = hit.collider.transform;

                    break;
                }
            }
        }


        visualizeEnemy = enemy;
        visualizeBanana = banana;

        if (visualizeBanana != lastBananaDodge)
            triesToJump = false;
    }

    public void Jump()
    {
        Debug.Log("TryJump");
        if (onAir || onFloor || state != EntityState.WALKING || !MusicPlayer.isRunning) return;
        Debug.Log("Jump");
        StartCoroutine(CtJump());
    }

    

    void MoveToWaypoint()
    {
        if (stop) return;

        Vector3 toPos = target.position.WithY(Y);

        transform.position += transform.forward * speed * Time.deltaTime;

        float distanceToWp = VectorHelp.Distance2D(transform.position, toPos);

        if (distanceToWp < offset)
        {
            GoToNextWaypoint();
        }

        
        lookAtPosAction(toPos);
    }


    private void EvadeFloorThings()
    {
        if (isHuman) return;
        if (triesToJump) return;
        if (visualizeBanana == null) return;
        float distanceToBanana = VectorHelp.Distance2D(transform.position, visualizeBanana.position);
        if (distanceToBanana > .75f) return;

        //Debug.Log("Jump at " + distanceToBanana);
        if (Random.Range(0, 100) <= settings.jumpChance)
            Jump();
        
        triesToJump = true;
        lastBananaDodge = visualizeBanana;

    }

    private void LookAtPosSlerped(Vector3 toPos)
    {
        Vector3 pos = toPos.WithY(Y);
        Vector3 lookDir = (pos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10);
    }

    private void LookAtPosLegacy(Vector3 toPos) => transform.LookAt(toPos);
    

    private void GoToNextWaypoint()
    {
        waypointIndex++;
        if(waypointIndex >= waypoints.Count)
            waypointIndex = 0;

        target = waypoints[waypointIndex];

        if (!isHuman && !onAir)
        {
            float variation = Random.Range(maxSpeedVariation.x, maxSpeedVariation.y);
            speed = baseSpeed + variation;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
            anim["walk"].speed = speed;
        }

    }
    #endregion

    #region CORUTINAS

    IEnumerator CTTime(float t, System.Action func)
    {
        yield return new WaitForSeconds(t);
        func?.Invoke();
    }

    IEnumerator Think(float dur, System.Action action)
    {
        while (true)
        {
            yield return new WaitForSeconds(dur);
            action?.Invoke();
        }
    }

    IEnumerator CTFall(float inT, float dur)
    {
        CancelInvoke("WalkAnim");
        yield return new WaitForSeconds(inT);
        
        anim.Play("death");

        stop = true;
        onFloor = true;
        myCollider.enabled = false;

        yield return new WaitForSeconds(dur);

        onFloor = false;
        myCollider.enabled = true;
        stop = false;
        anim.Play("walk");
    }

    IEnumerator CtJump()
    {
        onAir = true;

        myCollider.enabled = false;

        var jumpForce = 7.5f;
        var goDown = Vector3.zero;

        var goUp = Vector3.up * jumpForce;

        var oldSpeed = speed;
        speed = 1.5f;

        while (transform.position.y >= 0)
        {
            goDown += Vector3.down * 15f * Time.deltaTime;
            var pos = goUp + goDown;
            transform.position += pos * Time.deltaTime;
            anim.transform.Rotate(new Vector3(-360, 0, 0) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        speed = oldSpeed;
        myCollider.enabled = true;

        onAir = false;
        anim.transform.localEulerAngles = Vector3.zero;
        transform.position = VectorHelp.XZ(transform.position, 0);
    }
    
    #endregion

    #region SIT

    public void OnClickSit() {
        
        var reactedTime = Time.time - MusicPlayer.stopppedTime;
        var msg = MusicPlayer.isRunning? "Too soon" : reactedTime.ToString("n2") + " s" ;
        
        GameManager.instance.guiManager.ChangeReactionText(msg);
        GameManager.instance.AddReactionTime(reactedTime);
        
        Sit();
    }

    void Sit()
    {
        Debug.Log($"Sit {gameObject.name}");
        
        if (IsSeated()) return;
        
        SearchClosestChair();
        if (targetChair == null)
        {
            Last();
        }
        stop = true;
        ChangeState(EntityState.TO_CHAIR, StateGoToChair);

    }
    
    void StateGoToChair() {
        if (onFloor || onAir || IsSeated()) return;
        if (targetChair == null) return;

        agent.enabled = true;
        agent.SetDestination(targetChair.transform.position);
        agent.Move(Vector3.zero);
        
        float distanceToChair = VectorHelp.Distance2D(transform.position, targetChair.transform.position);

        if (distanceToChair < .5f)
        {
            if (isHuman && GameManager.instance.gameRunning)
            {
                StartCoroutine(CTTime(.5f, GameManager.instance.OnGameLose));
            }
            SitOnChair();
        }
    }
    private void SitOnChair()
    {
        targetChair.Set(this);
        GameManager.allChairsOccuped -= AllChairsOccuped;
        Chair.onChairOccuped -= OnChairOccuped;
        
        agent.enabled = false;
        stop = true;
        rb.mass *= 30;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        anim.Play("idle");
        ChangeState(EntityState.SEATED, StateAfk);
    }


    private void SearchClosestChair()
    {
        targetChair = GameManager.instance.chairs
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .FirstOrDefault(x => !x.occuped);
    }
    void Last()
    {
        agent.enabled = false;
        anim.Play("idle");
        
        rb.constraints = RigidbodyConstraints.None;

        float xR =3f;
        transform.position = VectorHelp.XZ(transform.position, 1f);
        baseVisual.GetComponent<Renderer>().material.color = Color.gray;

        float x = Random.Range(-xR, xR);
        float y = Random.Range(3, 3);
        float z = Random.Range(-xR, xR);
        rb.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
        rb.AddTorque(new Vector3(x, 0, z) * 5, ForceMode.Impulse);
        rb.useGravity = true;
        stop = true;
        
        Chair.onChairOccuped -= OnChairOccuped;
        GameManager.allChairsOccuped -= AllChairsOccuped;
    }
    #endregion

    private int GetWp(int current)
    {
        if (current > waypoints.Count - 1)
            return 0;
        return current;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(fromWpPos, .25f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(toWpPos, .25f);

        /*
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, .5f, 0), transform.position + new Vector3(0, .5f, 0) + (transform.forward * slowDistance ));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + new Vector3(0, .5f, 0), transform.position + new Vector3(0, .5f, 0) + transform.forward * pushDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position  + transform.forward * 1f);

        */
    }
    
    //Legacy...
    void Side()
    {
        if (targetChair == null) return;
        
        Transform from = waypoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        Transform to = waypoints.OrderBy(x => Vector3.Distance(x.transform.position, targetChair.transform.position)).First();

        fromWpPos = from.position;
        toWpPos = to.position;

        int fromIndex = waypoints.IndexOf(from);
        int toIndex = waypoints.IndexOf(to);

        side = Pathfinding.GetShortestPath(fromIndex, toIndex, waypoints.Count);
        if (side == 0)
            side = (int)VectorHelp.CheckFront(transform, from);
    }

    int side = 0;
    Vector3 fromWpPos;
    Vector3 toWpPos;

}


