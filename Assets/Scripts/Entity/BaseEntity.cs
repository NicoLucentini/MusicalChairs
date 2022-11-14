using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class BaseEntity : Entity, ISitable
{
    public List<Transform> waypoints = new List<Transform>();

    [Header("References")]
    public Renderer render;

    public Animation anim;
    public GameObject baseVisual;
    public Rigidbody rb;
    public Collider myCollider;
    public NavMeshAgent agent;

    public GameObject knockedGo;


    [Header("Sit")]
    [ReadOnly] public Chair tempChair;
    [ReadOnly] public bool last = false;
    public bool sit;
    public Vector2 reactionTime;
    [Header("Custom Properties")]
    public EntitySettings settings;
    public bool useSettings = false;

    [Header("Properties")]
    public bool isHuman;
    public float baseOffset = .2f;

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

    public Vector2 speedToChair;
    public float speedChair;
    public float jumpChance;

    private float slowDistance = 1.25f;

    [ReadOnly] public int waypointIndex = 0;
    [ReadOnly] public Transform target;

    [ReadOnly] public float jumpTimer;

    [Header("Empujar")]
    public float pushCd = 1.5f;
    public float pushDistance;
    public float pushFallDuration = 1.5f;
    public float pushChance;

    [ReadOnly] public bool hasAttacked = true;
    [ReadOnly] public bool hasBeenAttacked = false;
    [ReadOnly] public Transform whoPushMe;
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
    public Coroutine releaseCorutine;
    public Coroutine thinkCoroutine;
    public Coroutine checkCloseCoroutine;
    Coroutine closestChair;
    Coroutine raysCt;


    [Header("Other")]
    public List<float> directions;
    public List<float> angles;




    #region UNITY
    float Y
    {
        get { return transform.position.y; }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        //GameManager.instance.players.Add(this);
    }
    private void OnEnable()
    {
        GameManager.allChairsOccuped += AllChairsOccuped;
        MusicPlayer.onMusicStopped += OnMusicStopped;
        Chair.onChairOccuped += OnChairOccuped;
        Banana.onGetHit += OnBananaHit;
        //PlayersSpawner.allPlayersInstantiated += () => thinkCoroutine = StartCoroutine(Think(1.5f, CheckDistance));
    }
    private void OnDestroy()
    {
        GameManager.instance.players.Remove(this);
        GameManager.allChairsOccuped -= AllChairsOccuped;
        MusicPlayer.onMusicStopped -= OnMusicStopped;
        Chair.onChairOccuped -= OnChairOccuped;
        Banana.onGetHit -= OnBananaHit;
        // PlayersSpawner.allPlayersInstantiated -= () => thinkCoroutine = StartCoroutine(Think(1.5f, CheckDistance));
    }

    bool humanHasClicked = false;

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
        useSettings = true;
        if (!useSettings) return;
        if (settings == null) return;

        //baseSpeed = settings.baseSpeed;
        //maxSpeed = settings.maxSpeed;
        //minSpeed = settings.minSpeed;
        //accel = settings.accel;
        //maxSpeedVariation = settings.maxSpeedVariation;
        reactionTime = settings.reactionTime;

        //speedToChair = settings.speedToChair;
        pushChance = settings.pushChance;
        jumpChance = settings.jumpChance;
        
        GameObject go = GameObject.Instantiate(settings.prefab, transform);
        anim = go.GetComponent<Animation>();
    }



    float sitTime;
    private void Start()
    {
        rb.Sleep();
        waypoints.AddRange(GameManager.instance.waypoints);

        if (waypoints.Count > 0)
        {
            var temps = waypoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position));
            Transform temp = temps.First(x => VectorHelp.CheckSide(transform.position, x.position - transform.position, transform.up) > 0);

            waypointIndex = GameManager.instance.waypoints.IndexOf(temp);
            target = waypoints[waypointIndex];
        }

        if (isHuman)
        {
            GameObject model = GameObject.Instantiate(GameManager.instance.player.prefab, transform);
            anim = model.GetComponent<Animation>();
            GameManager.instance.uiController.SetPlayer(this);
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

        closestChair = StartCoroutine(Think(.5f, () => { tempChair = SearchClosestChair(); }));

    }

    public void SetAnim()
    {
        anim["walk"].speed = speed;
    }

    public void Update()
    {
        if (sit) return;

        if (!arrivedChair)
        {
            MoveToChairAgent();
        }
        else
        {

            if (MusicPlayer.isRunning)
            {
                Rays();
                CheckCloseToSlow();
                EvadeFloorThings();
                CheckPushDistance();
            }

            MoveToWaypoint();

        }

    }

    Vector3 fromWpPos;
    Vector3 toWpPos;

    void Side()
    {
        if (tempChair == null) return;
        Transform from = waypoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        Transform to = waypoints.OrderBy(x => Vector3.Distance(x.transform.position, tempChair.transform.position)).First();

        fromWpPos = from.position;
        toWpPos = to.position;

        int fromIndex = waypoints.IndexOf(from);
        int toIndex = waypoints.IndexOf(to);

        side = Pathfinding.GetShortestPath(fromIndex, toIndex, waypoints.Count);
        if (side == 0)
            side = (int)VectorHelp.CheckFront(transform, from);
    }

    bool arrivedChair = true;
    Vector3 direction;
    float distanceToChair = 0;
    int side = 0;


    #endregion

    #region EVENTS

    void OnMusicStopped()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
            rb.Sleep();
        }

        if (!isHuman)
            baseVisual.SetActive(false);

        if (checkCloseCoroutine != null)
            StopCoroutine(checkCloseCoroutine);
    }

    void AllChairsOccuped()
    {
        if (sit) return;

        Debug.Log("Alguna vez entro aca?");
        Last();
    }
    void OnBananaHit(Transform t)
    {
        if (visualizeBanana == t)
            visualizeBanana = null;
    }
    void OnChairOccuped(Chair temp)
    {

        Debug.Log($"La silla {temp.gameObject.name} fue ocupada");
        if (tempChair != temp) return;
        if (tempChair.owner == this) return;

        Debug.Log($"Mi silla ({gameObject.name}) la ocuparon " + temp.gameObject.name);

        tempChair = SearchClosestChair();
        direction = Vector3.zero;
        tempChair = null;
        anim.Play("idle");
        //Invoke("Occupe", .1f);
        Sit();
    }


    #endregion

    #region Accel
    public void Accelerate()
    {
        speed += accel * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        anim["walk"].speed = speed;
    }
    public void Desaccelerate()
    {
        speed -= accel * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        anim["walk"].speed = speed;
    }

    public void Release()
    {
        if (releaseCorutine != null)
            StopCoroutine(releaseCorutine);

        releaseCorutine = StartCoroutine(CTRelease());
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

    public void Fall()
    {
        StartCoroutine(CTFall(0, fallDuration));
    }

    #endregion

    #region PUSH

    public void CheckCloseToSlow()
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

    //no me gusta este checkdistance pero we
    public void CheckPushDistance()
    {
        if (hasAttacked || onFloor || onAir) return;

        if (visualizeEnemy == null) return;

        if (visualizeEnemy.onAir || visualizeEnemy.onFloor || visualizeEnemy.hasBeenAttacked) return;

        if (Vector3.Distance(transform.position, visualizeEnemy.transform.position) < pushDistance)
        {
            hasAttacked = true;

            if (Random.Range(0, 100) <= pushChance)
            {
                Push(visualizeEnemy.transform);
            }

            DoInTime(pushCd, () => { hasAttacked = false; });

        }
    }

    void DoInTime(float t, System.Action func)
    {
        StartCoroutine(CTTime(t, func));
    }


    void WalkAnim()
    {
        if (sit) return;

        CancelInvoke("WalkAnim");
        anim.Play("walk");
    }

    public void Push(Transform t)
    {
        anim.Play("attack");
        stop = true;
        float animLength = anim["attack"].length;
        Invoke("WalkAnim", animLength);
        t.GetComponent<BaseEntity>().GetPushed(animLength / 2, transform);
        StartCoroutine(CTTime(animLength / 2, () => { stop = false; }));

    }

    public void GetPushed(float inTime, Transform pushed = null)
    {
        whoPushMe = pushed;
        hasBeenAttacked = true;

        //quizas cuando te empujan y termina la partida?
        StartCoroutine(CTFall(inTime, pushFallDuration));
    }
    #endregion

    #region MOVEMENT

    void Rays()
    {
        float totalDistance = 0;
        RaycastHit hit;
        BaseEntity enemy = null;
        Transform banana = null;
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 to = waypoints[waypointIndex].position + new Vector3(0, 0.4f, 0);

        totalDistance += Vector3.Distance(origin, to);


        if (Physics.Linecast(origin, to, out hit, 1 << 10 | 1 << 11))
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

                if (Physics.Linecast(origin, to, out hit, 1 << 10))
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
        if (onAir || onFloor || sit || !MusicPlayer.isRunning) return;
        Debug.Log("Jump");
        StartCoroutine(CTJump());
    }

    void MoveToChairAgent() {
        if (onFloor || onAir || sit || arrivedChair) return;
        if (tempChair == null) return;

        agent.enabled = true;
        float distanceToChair;
        distanceToChair = VectorHelp.Distance2D(transform.position, tempChair.transform.position);

        if (distanceToChair < .5f)
        {
            if (isHuman && GameManager.instance.gameRunning)
            {
                StartCoroutine(CTTime(.5f, GameManager.instance.OnGameLose));
            }
            agent.enabled = false;
            arrivedChair = true;
            tempChair.Set(this);
        }
        else
        {
            agent.SetDestination(tempChair.transform.position);
            agent.Move(Vector3.zero);
        }

    }

    void MoveToWaypoint()
    {
        if (stop) return;

        Vector3 toPos = VectorHelp.XZ(target.position, Y);

        transform.position += transform.forward * speed * Time.deltaTime;

        float distanceToWp = VectorHelp.Distance2D(transform.position, toPos);

        if (distanceToWp < offset)
        {
            GoNext();
        }

        if (useSlerp)
            LookAtTarget(toPos);
        else
            transform.LookAt(toPos);
    }


    public void EvadeFloorThings()
    {
        if (isHuman) return;
        if (triesToJump) return;
        if (visualizeBanana == null) return;
        float distanceToBanana = VectorHelp.Distance2D(transform.position, visualizeBanana.position);
        if (distanceToBanana > .75f) return;

        //Debug.Log("Jump at " + distanceToBanana);
        if (Random.Range(0, 100) <= jumpChance)
        {
            Jump();
        }
        triesToJump = true;
        lastBananaDodge = visualizeBanana;

    }
    public void LookAtTarget(Vector3 toPos)
    {
        Vector3 pos = VectorHelp.XZ(toPos, transform.position.y);
        Vector3 lookDir = (pos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10);
    }

    public void GoNext()
    {
        if (waypointIndex < waypoints.Count - 1)
            waypointIndex++;
        else
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

    //GENERICAS
    IEnumerator CTTime(float t, System.Action func)
    {
        yield return new WaitForSeconds(t);

        if (func != null)
            func();
    }

    IEnumerator Think(float dur, System.Action action)
    {
        while (true)
        {
            yield return new WaitForSeconds(dur);
            action?.Invoke();
        }
    }

    IEnumerator ThinkUpdate(System.Action action)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }

    IEnumerator ThinkWithStop(float dur = 1, System.Action action = null, System.Func<bool> stop = null)
    {
        while (true)
        {
            if (stop != null)
            {
                if (stop())
                    yield break;
            }

            yield return new WaitForSeconds(dur);

            action?.Invoke();
        }
    }

    //jump / fall / Occupe
    IEnumerator CTFall(float inT, float dur)
    {
        CancelInvoke("WalkAnim");
        yield return new WaitForSeconds(inT);
        // knockedGo.SetActive(true);
        anim.Play("death");

        stop = true;
        onFloor = true;
        myCollider.enabled = false;

        yield return new WaitForSeconds(dur);
        //knockedGo.SetActive(false);

        onFloor = false;
        myCollider.enabled = true;
        stop = false;
        anim.Play("walk");
    }

    IEnumerator CTJump()
    {
        onAir = true;

        myCollider.enabled = false;

        float jumpForce = 7.5f;
        Vector3 goDown = Vector3.zero;

        Vector3 goUp = Vector3.up * jumpForce;

        float oldSpeed = speed;
        speed = 1.5f;

        Vector3 initPos = transform.position;
        while (transform.position.y >= 0)
        {
            goDown += Vector3.down * 15f * Time.deltaTime;
            Vector3 pos = goUp + goDown;
            transform.position += pos * Time.deltaTime;
            anim.transform.Rotate(new Vector3(-360, 0, 0) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        // agent.enabled = true;

        speed = oldSpeed;
        myCollider.enabled = true;

        onAir = false;
        anim.transform.localEulerAngles = Vector3.zero;
        transform.position = VectorHelp.XZ(transform.position, 0);
        jumpTimer = VectorHelp.Distance2D(transform.position, initPos);
    }

    IEnumerator CTOccupe(float t)
    {
        yield return new WaitForSeconds(t);

        while (onFloor || onAir)
            yield return new WaitForEndOfFrame();

        Occupe();
    }

    #endregion

    #region SIT

    public void OnClickSit() {
        humanHasClicked = true;
        Sit();
    }

    public void Sit()
    {
        if (sit) return;
        if (isHuman && !humanHasClicked) return;
        float time = 0;

        if (!isHuman)
        {
            time = GameManager.instance.playerReactionTime;
        }
        else
        {
            sitTime = Time.time;
            float reac = 0; 
            string msg = "Early";
            if (!MusicPlayer.isRunning)
            {
                reac = sitTime - MusicPlayer.stopppedTime;
                msg =  reac.ToString("n2") + " s";
            }
            GameManager.instance.guiManager.ChangeReactionText(msg);
            GameManager.instance.AddReactionTime(reac);
           
        }

        //StartCoroutine(CTOccupe(time));
     
        GoToChair();
    }

    void GoToChair() {
        tempChair = SearchClosestChair();
        if (tempChair == null)
        {
            Last();
        }
        arrivedChair = false;
        stop = true;
        
    }

    //ONLY CALLED ON HUMAN?????
    //esto deberia sacarlo a la bosta

        /*
        public void Sit(Chair chair)
        {
            if (closestChair != null)
                StopCoroutine(closestChair);

            sitTime = Time.time;
            float reac = sitTime - MusicPlayer.stopppedTime;
            GameManager.instance.guiManager.ChangeReactionText(reac.ToString("n2"));

            tempChair = chair;
            stop = true;
            arrivedChair = false;
            tempChair.visual.SetActive(true); //????
            anim.Play("run");
            Side();
            speedChair = speedToChair.x;
        }
    */
    bool readyToSit = false;
    //ocuppe is called when my chair is occuped, doesnt matter if im on floor or air..or my reaction time or 
    public void Occupe()
    {
        if (last)
        {
            tempChair = null;
            return;
        }

        readyToSit = true;

        CancelInvoke("Occupe");

        if (closestChair != null)
            StopCoroutine(closestChair);

        if (tempChair == null)
            tempChair = SearchClosestChair();

        if (tempChair == null)
        {
            Debug.Log("Si esto te da null perdiste macho");
            Last();
            return;
        }
        else
        {
            stop = true;
            arrivedChair = false;
            direction = Vector3.zero;
            anim.Play("run");
            speedChair = speedToChair.x;
            Side();
        }

    }

    public Chair SearchClosestChair()
    {
        return GameManager.instance.chairs.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).Where(x => !x.occuped).FirstOrDefault();
    }
    void Last()
    {
        last = true;
        agent.enabled = false;
        anim.Play("idle");
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

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
    }
    #endregion

    public int GetWp(int current)
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
}


