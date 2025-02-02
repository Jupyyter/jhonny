using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.Tilemaps;
//using UnityEngine.SceneManagement;
//  (((＼（✘෴✘）／)))
public class JhonnyScript : NetworkBehaviour
{   //local variables for BODY
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool OnGroundBoolTrue = false;
    private bool OnGroundBoolLate = false;
    private bool canSpawnRunDust = true;
    [SerializeField] private GameObject runDust;
    [SerializeField] private GameObject jumpDust;
    [SerializeField] private GameObject fallDust;
    [SerializeField] private GameObject climbDust;
    [SerializeField] private ParticleSystem blood;
    private bool canClimb = false;
    private bool onWalls = false;
    [SerializeField] private GameObject body;
    private Rigidbody2D rb;
    private Camera cam;
    private CinemachineVirtualCamera machine;
    private Animator anim;
    [SerializeField] private Tilemap backWall2;
    private float dirx = 0f;
    private BoxCollider2D coll;
    [SerializeField] private LayerMask jumpGround;
    private SpriteRenderer sprite;
    private float MoveSpeed = 10f;
    private float JumpForce = 30f;
    [SerializeField] private bool onLadder = false;
    private bool facingright = true;
    private string currentState;
    private AnimationState anima;
    private int hp=1;
    private const string MOVE_BODY_ANIMATION = "MoveBodyAnimation";
    private const string IDLE_BODY_ANIMATION = "IdleBodyAnimation";
    private const string NEW_JHONNY_JUMP_RUN_ANIMATION = "NewJhonnyJumpRun";
    private const string NEW_JHONNY_JUMP_STAY_ANIMATION = "NewJhonnyJumpStay";
    private const string NEW_JHONNY_FALL_RUN_ANIMATION = "NewJhonnyFallRun";
    private const string NEW_JHONNY_FALL_STAY_ANIMATION = "NewJhonnyFallStay";
    private const string NEW_JHONNY_CLIMB_WALLS = "ClimbWalls";

    //local variables of HANDS
    [SerializeField] private GameObject hands;
    [SerializeField] Transform FirePoint1;
    [SerializeField] Transform FirePoint2;
    [SerializeField] private GameObject BulletPrefab;
    public AnimationState COOOLLL;
    private float LastFrameDirx = 0;
    private bool useLeftArm = false;//spown the bullet position
    private bool canshoot = true;
    public bool ladder = false;
    private float number = 0f;//for timing hand and bodyanimations :)
    private float grav;
    private const string RUNNING_HANDS_ANIMATION = "RunningHandsAnimation";
    private const string IDLE_HANDS_ANIMATION = "IdleHandsAnimation";
    private const string JUMP_HANDS_ANIMATION = "IdleHandsAnimation";
    private const string SHOOTING_HANDS_ANIMATION = "ShootingHandsAnimation";
    private const string NOTHING = "nothing";
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        //anima=GetComponent<AnimationState>();
        if (!isLocalPlayer)
        {
            //cam.gameObject.SetActive(false);
            machine.gameObject.SetActive(false);
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        grav = rb.gravityScale;
        //cam = FindObjectOfType<Camera>();
        machine = FindObjectOfType<CinemachineVirtualCamera>();
        machine.Follow = transform;
        machine.LookAt = transform;
    }
    private void Update()
    {
        if (!isLocalPlayer)
        { return; }
        if (OnGroundLate() != OnGroundBoolLate)//if not on ground, jhonny can still jump for 0.1 seconds
        {
            StartCoroutine(JumpLogic());
        }
        if (OnGroundTrue() != OnGroundBoolTrue)
        {
            trueJumpLogic();
        }
        dirx = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirx * MoveSpeed, rb.velocity.y);//move
        if (Input.GetButtonDown("Jump") && OnGroundBoolLate && !onLadder)//jump
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            Vector3 middleBottom = new Vector3(coll.bounds.center.x, coll.bounds.min.y, 0);
            CmdspwnJumpDust(middleBottom);
        }
        HandleBody();//animate the body and flip children and climb logic
        HandleHands();//animate and shoot
        if (ladder)
        {//use ladder
            if (onLadder == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                if (onLadder == false)//enter ladder mode
                {
                    onLadder = true;
                    rb.gravityScale = 0;
                }
                rb.velocity = new Vector2(rb.velocity.x, Input.GetAxisRaw("Vertical") * 18);
            }
        }
        else
        {//exit enter ladder mode
            if (onLadder == true)
            {
                onLadder = false;
                rb.gravityScale = grav;
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 1);
                }
            }
        }
        /*if (Input.GetButtonDown("ResetLVL"))
        {//if press ESCAPE
            SceneManager.LoadScene("FirstScene");
            //EditorSceneManager.LoadScene("FirstScene");
        }*/
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnBecameInvisible()//not on screen
    {
        //SceneManager.LoadScene("FirstScene");
        Mirror.NetworkStartPosition[] startPositions = FindObjectsOfType<Mirror.NetworkStartPosition>();
        Mirror.NetworkStartPosition startPosition = startPositions[0];
        Vector3 resp = startPosition.transform.position;
        this.transform.position = resp;
    }
    private void HandleBody()
    {
        if (dirx > 0f)
        {//if moving rightOnGroundBoolLate
            if (!facingright)
            {
                flip();
                facingright = true;
            }
        }
        else if (dirx < 0f)
        {//if moving left
            if (facingright)
            {
                flip();
                facingright = false;
            }
        }
        if (!OnGroundBoolLate)
        {//if able to climb
            if (rb.velocity.y < 0) { canClimb = true; }
        }
        else { canClimb = false; }
        if (canClimb && Input.GetAxisRaw("Vertical") > 0 && !ladder && wall() && dirx != 0)//climb the wall
        {
            //actually climb if can
            anim.speed = 1;
            onWalls = true;
            rb.velocity = new Vector2(10 * dirx, 20);
            //ChangeAnimationState(NOTHING);
            ChangeAnimationState(NEW_JHONNY_CLIMB_WALLS);
        }
        else if (canClimb && !ladder && wall() && dirx != 0)//stay on wall
        {
            RaycastHit2D upEndOfWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), transform.TransformDirection(Vector2.right), 0.5f, jumpGround);
            if (!upEndOfWall && anim.speed != 0)//if half of johnny doesn't detect the wall, he just jumps it
            {
                rb.velocity = new Vector2(10 * dirx, 12);
            }
            else//actually stay on the wall
            {
                StartCoroutine(slipOnWall(2f));
                if (anim.speed != 0)
                {
                    anim.speed = 0;
                    StartCoroutine(spwnClimbDust());
                    if (!useLeftArm)
                    {
                        ChangeAnimationStateB(NEW_JHONNY_CLIMB_WALLS, 0.5f);
                        useLeftArm = true;
                    }
                    else
                    {
                        ChangeAnimationStateB(NEW_JHONNY_CLIMB_WALLS, 0);
                        useLeftArm = false;
                    }
                }
                rb.gravityScale = 0;
                onWalls = true;
                //rb.velocity = new Vector2(10 * dirx, -2);
                //ChangeAnimationState(NOTHING);
            }
        }
        else//do something else if not climbing
        {
            if (!onLadder) { rb.gravityScale = grav; }
            anim.speed = 1;
            if (onWalls == true)
            {//when exiting climbing
                onWalls = false;
                rb.velocity = new Vector2(14 * dirx, 5);
            }
            if (!OnGroundLate() || onLadder)
            {
                if (rb.velocity.y > 0 && dirx != 0)
                {
                    ChangeAnimationState(NEW_JHONNY_JUMP_RUN_ANIMATION);
                }
                else if (rb.velocity.y < 0 && dirx != 0)
                {
                    ChangeAnimationState(NEW_JHONNY_FALL_RUN_ANIMATION);
                }
                else if (rb.velocity.y > 0)
                {
                    ChangeAnimationState(NEW_JHONNY_JUMP_STAY_ANIMATION);
                }
                else if (rb.velocity.y < 0)
                {
                    ChangeAnimationState(NEW_JHONNY_FALL_STAY_ANIMATION);
                }

            }
            else if (dirx != 0 && OnGroundLate())
            {
                ChangeAnimationState(MOVE_BODY_ANIMATION);
                if (canSpawnRunDust)
                {
                    Vector3 middleBottom = new Vector3(coll.bounds.center.x-0.4f*dirx, coll.bounds.min.y, 0);
                    StartCoroutine(spawnRunDust(middleBottom));
                }
            }
            else if (OnGroundLate())//if idle Ω
            {
                ChangeAnimationState(IDLE_BODY_ANIMATION);
            }
        }
    }
    private void HandleHands()
    {
        dirx = Input.GetAxisRaw("Horizontal");
        number += Time.deltaTime;
        if (number > 0.05f)
        {//limiting shooting to once 0.05 seconds from god mode
            canshoot = true;
            number = 0;
        }
        if (onWalls == true)
        {
            ChangeAnimationState(NOTHING);
        }
        else if (Input.GetKey("z"))//if shoot
        {
            if (canshoot)
            {
                if (!useLeftArm)
                {
                    Cmdshoot(FirePoint2.position, FirePoint2.rotation);
                    useLeftArm = true;
                }
                else
                {
                    Cmdshoot(FirePoint1.position, FirePoint1.rotation);
                    useLeftArm = false;
                }
                canshoot = false;
                ChangeAnimationState(SHOOTING_HANDS_ANIMATION);
            }
        }
        else if (dirx == 0 || !OnGroundLate() || onLadder)//if stay or in air
        {
            ChangeAnimationState(IDLE_HANDS_ANIMATION);
        }
        else if (OnGroundLate())//if on ground
        {
            if (LastFrameDirx == 0f)
            {
                ChangeAnimationState(RUNNING_HANDS_ANIMATION);
            }
            else
            {//after shooting; hands run animation starts with the frame of the body animation
                float normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                float normalizedTimeInCurrentLoop = normalizedTime - Mathf.Floor(normalizedTime);
                ChangeAnimationStateB(RUNNING_HANDS_ANIMATION, normalizedTimeInCurrentLoop);
            }
        }
        LastFrameDirx = dirx;
    }
    [Command]
    private void Cmdshoot(Vector3 pos, Quaternion quar)//spawns bullets at fire points with their coordinates and random degrees 1 hand at time
    {
        Quaternion rotationn = Quaternion.Euler(0, 0, Random.Range(-3, 3));
        rotationn *= quar;
        GameObject bulletClone = Instantiate(BulletPrefab, pos, rotationn);
        NetworkServer.Spawn(bulletClone);
    }
    private bool OnGroundLate()
    {//if on ground
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.5f, jumpGround);
    }
    private bool OnGroundTrue()
    {//if on ground
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.01f, jumpGround);
    }
    private bool wall()
    {//if on ground
        jumpGround &= ~(1 << LayerMask.NameToLayer("Platform"));//exclude a layer https://forum.unity.com/threads/how-to-ignore-specific-layer.74350/
        bool bul;
        if (facingright)
        {
            bul = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpGround);
            jumpGround = jumpGround | (1 << LayerMask.NameToLayer("Platform"));
            return bul;
        }
        else
        {
            bul = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpGround);
            jumpGround = jumpGround | (1 << LayerMask.NameToLayer("Platform"));
            return bul;
        }
    }
    private void flip()
    {//flipping the motherfucker AND HIS CHILDREN :):):):):):):):):):):):):):):):):):):)
        transform.Rotate(0f, 180f, 0f);
        /*FirePoint1.transform.localPosition = new Vector3(-FirePoint1.transform.localPosition.x, FirePoint1.transform.localPosition.y, FirePoint1.transform.localPosition.z);
        FirePoint2.transform.localPosition = new Vector3(-FirePoint1.transform.localPosition.x, FirePoint1.transform.localPosition.y, FirePoint1.transform.localPosition.z);
        FirePoint1.transform.Rotate(0f, 180f, 0f);
        FirePoint2.transform.Rotate(0f, 180f, 0f);
        body.transform.Rotate(0f, 180f, 0f);
        hands.transform.Rotate(0f, 180f, 0f);*/
    }
    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;//if the same animation plays
        anim.Play(newState);
        currentState = newState;
    }
    private void ChangeAnimationStateB(string newState, float timee)
    {
        if (currentState == newState) return;//if the same animation plays
        anim.Play(newState, -1, timee);
        currentState = newState;
    }
    private IEnumerator JumpLogic()//used for clinbing and jump correctly
    {
        if (OnGroundBoolLate && !OnGroundLate())
        {
            yield return new WaitForSeconds(0.10f);
            OnGroundBoolLate = OnGroundLate();
        }
        else if (!OnGroundBoolLate && OnGroundLate())
        {
            OnGroundBoolLate = OnGroundLate();
        }
    }
    private void trueJumpLogic()//helfull TO KNOW when falling and hit the ground
    {
        if (OnGroundBoolTrue && !OnGroundTrue())
        {
            OnGroundBoolTrue = OnGroundTrue();
        }
        else if (!OnGroundBoolTrue && OnGroundTrue())
        {
            OnGroundBoolTrue = OnGroundTrue();

            Vector3 middleBottom = new Vector3(coll.bounds.center.x, coll.bounds.min.y, 0);
            CmdspwnFallDust(middleBottom);

        }
    }
    private IEnumerator spawnRunDust(Vector3 pos)
    {
        if (dirx != 0 && OnGroundTrue() && canSpawnRunDust)
        {
            canSpawnRunDust = false;
            //Vector3 middleBottom = new Vector3(coll.bounds.center.x, coll.bounds.min.y, 0);
            CmdspwnRunDust(pos);
            yield return new WaitForSeconds(0.06f);
            StartCoroutine(spawnRunDust(pos));
        }
        else
        {
            canSpawnRunDust = true;
        }
    }
    [Command]
    private void CmdspwnRunDust(Vector3 pos)
    {
        GameObject rundust = Instantiate(runDust, pos, transform.rotation);
        NetworkServer.Spawn(rundust);
    }
    [Command]
    private void CmdspwnJumpDust(Vector3 pos)
    {
        GameObject jumpdust = Instantiate(jumpDust, pos, transform.rotation);
        NetworkServer.Spawn(jumpdust);
    }
    private void CmdspwnFallDust(Vector3 pos)
    {
        GameObject jumpdust = Instantiate(fallDust, pos, transform.rotation);
        NetworkServer.Spawn(jumpdust);
    }
    private IEnumerator spwnClimbDust()
    {
        if (anim.speed == 0)
        {
            float offset = 0;
            if (useLeftArm)
            {
                offset = 0.05f;
            }
            else
            {
                offset = -0.07f;
            }
            if (facingright)
            {
                Vector3 topright = new Vector3(coll.bounds.max.x + 0.02f, coll.bounds.max.y - 0.20f - offset, 0);
                Instantiate(climbDust, topright, transform.rotation);
            }
            else
            {
                Vector3 topleft = new Vector3(coll.bounds.min.x - 0.02f, coll.bounds.max.y - 0.20f - offset, 0);
                Instantiate(climbDust, topleft, transform.rotation);
            }
            yield return new WaitForSeconds(0.07f);
            StartCoroutine(spwnClimbDust());
        }
    }
    public void takeDMG()
    {
        hp--;
        Instantiate(blood, transform.position, blood.transform.rotation);
        if (hp == 0)
        {
            hp=1;

            Instantiate(blood, transform.position, blood.transform.rotation);
            Mirror.NetworkStartPosition[] startPositions = FindObjectsOfType<Mirror.NetworkStartPosition>();
        Mirror.NetworkStartPosition startPosition = startPositions[0];
        Vector3 resp = startPosition.transform.position;
        this.transform.position = resp;
        if(!facingright){
            flip();
        }
        facingright=true;
        }
    }
    private void CmdClimbDust(Vector3 pos)
    {
        GameObject ClimbDust = Instantiate(climbDust, pos, transform.rotation);
        NetworkServer.Spawn(ClimbDust);
    }
    private IEnumerator slipOnWall(float slideSpeed)
    {
        if (anim.speed != 0)
        {
            rb.velocity = new Vector2(10 * dirx, 0);
            yield return new WaitForSeconds(0.5f);
            if (anim.speed == 0)
            {
                rb.velocity = new Vector2(10 * dirx, -slideSpeed);
            }
        }
        if (rb.velocity.y == -slideSpeed)
        {
            rb.velocity = new Vector2(10 * dirx, -slideSpeed);
        }
    }
}
/*
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⣤⣶⣶⣶⣶⣶⣦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⢿⣿⣿⡿⣿⣿⣿⣿⣿⣿⣿⣿⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⡇⣿⣷⣿⣿⣿⣿⣿⣿⣯⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⣿⣿⣿⣇⣿⣀⠸⡟⢹⣿⣿⣿⣿⣿⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢡⣿⣿⣿⡇⠝⠋⠀⠀⠀⢿⢿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⢸⠸⣿⣿⣇⠀⠀⠀⠀⠀⠀⠊⣽⣿⣿⣿⠁⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣷⣄⠀⠀⠀⢠⣴⣿⣿⣿⠋⣠⡏⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠾⣿⣟⡻⠉⠀⠀⠀⠈⢿⠋⣿⡿⠚⠋⠁⡁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣶⣾⣿⣿⡄⠀⣳⡶⡦⡀⣿⣿⣷⣶⣤⡾⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⡆⠀⡇⡿⠉⣺⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣯⠽⢲⠇⠣⠐⠚⢻⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⡐⣾⡏⣷⠀⠀⣼⣷⡧⣿⣿⣦⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣻⣿⣿⣿⣿⣿⣮⠳⣿⣇⢈⣿⠟⣬⣿⣿⣿⣿⣿⡦⢄⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢄⣾⣿⣿⣿⣿⣿⣿⣿⣷⣜⢿⣼⢏⣾⣿⣿⣿⢻⣿⣝⣿⣦⡑⢄⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⣠⣶⣷⣿⣿⠃⠘⣿⣿⣿⣿⣿⣿⣿⡷⣥⣿⣿⣿⣿⣿⠀⠹⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⣇⣤⣾⣿⣿⡿⠻⡏⠀⠀⠸⣿⣿⣿⣿⣿⣿⣮⣾⣿⣿⣿⣿⡇⠀⠀⠙⣿⣿⡿⡇⠀⠀⠀⠀⠀
⠀⠀⢀⡴⣫⣿⣿⣿⠋⠀⠀⡇⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⢘⣿⣿⣟⢦⡸⠀⠀⠀
⠀⡰⠋⣴⣿⣟⣿⠃⠀⠀⠀⠈⠀⠀⣸⣿⣿⣿⣿⣿⣿⣇⣽⣿⣿⣿⣿⣇⠀⠀⠀⠁⠸⣿⢻⣦⠉⢆⠀⠀
⢠⠇⡔⣿⠏⠏⠙⠆⠀⠀⠀⠀⢀⣜⣛⡻⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⡀⠀⠀⠀⠀⡇⡇⠹⣷⡈⡄⠀
⠀⡸⣴⡏⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣻⣿⣿⣿⣿⣿⣿⡄⠀⠀⠀⡇⡇⠀⢻⡿⡇⠀
⠀⣿⣿⣆⠀⠀⠀⠀⠀⠀⢀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡀⠀⣰⠿⠤⠒⡛⢹⣿⠄
⠀⣿⣷⡆⠁⠀⠀⠀⠀⢠⣿⣿⠟⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠻⢷⡀⠀⠀⠀⠀⠀⣸⣿⠀
⠀⠈⠿⢿⣄⠀⠀⠀⢞⠌⡹⠁⠀⠀⢻⡇⠹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠁⢳⠀⠀⠁⠀⠀⠀⠀⢠⣿⡏⠀
⠀⠀⠀⠈⠂⠀⠀⠀⠈⣿⠁⠀⠀⠀⡇⠁⠀⠘⢿⣿⣿⠿⠟⠋⠛⠛⠛⠀⢸⠀⠀⡀⠂⠀⠀⠐⠛⠉⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠕⣠⡄⣰⡇⠀⠀⠀⢸⣧⠀⠀⠀⠀⠀⠀⠀⢀⣸⠠⡪⠊⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢫⣽⡋⠭⠶⠮⢽⣿⣆⠀⠀⠀⠀⢠⣿⣓⣽⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⢹⣶⣦⣾⣿⣿⣿⡏⠀⠀⠀⠀⠀
*/