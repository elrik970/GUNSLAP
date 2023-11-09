using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rb;
    public float MovementTime;
    [SerializeField] private bool right = false;
    private float StartSpeed;
    private bool jumping = false;
    public Collider2D PlayerCollider;
    public float CloseToGround;
    private float CoyoteTime;
    public float WallJumpCastDistance = 0.1f;
    public GameObject CurrentBoundingBox;
    public float CoyoteMaxTime;
    public float WallJumpGrav;
    public LayerMask GroundLayerMask;
    public CinemachineConfiner2D confiner;
    public float VariableJumpHeightMult;
    private RaycastHit2D GroundHit;
    private RaycastHit2D ConfinerCast;
    private RaycastHit2D WallHit;
    private RaycastHit2D WallHitRight;
    private bool CanJump = true;
    public float fallingGravity;
    public float ApexZone = 1f;
    private float otherJump = 1f;
    private float CantMovePostWallJump = 0f;
    public float CantMovePostWallJumpMax;
    public float WallJumpSpeed;
    public float WallJumpDeacceleration;
    public float ApexGravity = 1f;
    public float DoubleJumpMultiplier;
    public bool OnGround;
    private float GroundCount;
    public Transform particleFloat;
    private bool WallJumpingLeft;
    private bool WallJumped = false;
    public float WallJumpForce;
    private bool WallJumpingRight;
    public float risingGravity;
    public float maxDoubleJump = 1f;
    public float JumpForce;
    private float targetSpeed;
    [SerializeField] private float MovementAcceleration;
    public LayerMask ConfinerLayerMask;
    public float MaxSpeed;
    public float MaxJumpBufferTime;
    private float JumpBufferTime;
    [SerializeField] private bool left = false;
    public List<GameObject> Confiners = new List<GameObject>();
    public float SlowDown;
    public Camera Camera;
    private PlayerInputs movement;
    void Awake() {
        movement = new PlayerInputs();
    }
    void OnEnable() {
        movement.Enable();
    }

    void OnDisable() {
        if (movement != null) movement.Disable();
    }
    void Start()
    {
        // Debug.Log(GameObject.FindWithTag("Cinemachine"));
        // confiner = GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineConfiner2D>();
        rb = GetComponent<Rigidbody2D> ();
        Camera = Camera.main;
        transform.position = new Vector3(PlayerPrefs.GetFloat("RespawnSpotX"),PlayerPrefs.GetFloat("RespawnSpotY"),0f);
        movement.Player.Jump.performed += OnJump;
        movement.Player.Right.performed += OnWalkRight;
        movement.Player.Left.performed += OnWalkLeft;
    }

    // Update is called once per frame
    
    void MoveDown () {

        float SpeedDif = targetSpeed - rb.velocity.y;
        float movement = SpeedDif * WallJumpDeacceleration;
        rb.AddForce(movement*Vector2.up,ForceMode2D.Impulse);
    }
    void Move () {

        float SpeedDif = targetSpeed - rb.velocity.x;
        float movement = SpeedDif * MovementAcceleration;
        rb.AddForce(movement*Vector2.right,ForceMode2D.Impulse);
    }
    void MoveRight () {
        if (rb.velocity.x < targetSpeed) {

            float SpeedDif = targetSpeed - rb.velocity.x;
            float movement = SpeedDif * MovementAcceleration;
            rb.AddForce(movement*Vector2.right,ForceMode2D.Impulse);
        }
    }
    void MoveLeft () {
        if (rb.velocity.x > targetSpeed) {
            float SpeedDif = targetSpeed - rb.velocity.x;
            float movement = SpeedDif * MovementAcceleration;
            rb.AddForce(movement*Vector2.right,ForceMode2D.Impulse);
        }
    }
    void Update()
    {
        
        // if (Confiners.Count != 0) {
        //     // confiner.InvalidateCache();
        //     Debug.Log(Confiners.Count);
        //     confiner.m_BoundingShape2D = Confiners[0].GetComponent<Collider2D>();
            
        // }
        JumpBufferTime += Time.deltaTime;
        if (WallJumped) {
            CantMovePostWallJump += Time.deltaTime;
        }
        GroundHit = Physics2D.Raycast(transform.position, Vector2.down, CloseToGround,GroundLayerMask);
        if (GroundHit.collider != null) {
            OnGround = true;
            CoyoteTime = 0f;
        }
        else {
            CoyoteTime+=Time.deltaTime;
            if (CoyoteTime > CoyoteMaxTime) {
                OnGround = false;
            }
        }
        if (!OnGround) {
            CanJump = false;

        }
        
    }
    void FixedUpdate () {
        
        if (jumping && JumpBufferTime<MaxJumpBufferTime&&OnGround) {
            Jump();
        }
        if (right) {
                targetSpeed = MaxSpeed;
                MoveRight();
            
            
        }
        if (left) {
                targetSpeed = -MaxSpeed;
                MoveLeft();
            
        }
        if (!right&&!left) {
            targetSpeed = 0f;
            Move();
            
        }
        if (rb.velocity.y > -ApexZone && rb.velocity.y < ApexZone/2f){
            rb.gravityScale = ApexGravity;
        }
        else if (rb.velocity.y < -ApexZone) {
            rb.gravityScale = fallingGravity;
        }
        if (OnGround) {
            WallJumped = false;
            otherJump = maxDoubleJump;
            CanJump = true;
            rb.gravityScale = risingGravity;
        }
    }
    void OnWalkRight(InputAction.CallbackContext context) {
        right = ! right;
        if (right) {
            transform.localScale = new Vector3(1f,1f,1f);
        }
    }
    void OnWalkLeft(InputAction.CallbackContext context) {
        left = ! left;
        // if (left) {
        // transform.localScale = new Vector3(-1f,1f,1f);
        // }
    }
    void Jump() {
        JumpBufferTime = 0f;
        if (CanJump) {
            if (jumping) {
                JumpBufferTime = MaxJumpBufferTime;
                CanJump = false;
                rb.gravityScale = risingGravity;
                rb.velocity = new Vector2(rb.velocity.x,0f);
                rb.AddForce(Vector3.up * JumpForce, ForceMode2D.Impulse);
                CoyoteTime = CoyoteMaxTime;
                
            }
        }
        if (!jumping) {
            if (rb.velocity.y > 0f) {
                rb.gravityScale = fallingGravity;
                rb.AddForce((rb.velocity.y/VariableJumpHeightMult)*Vector2.down,ForceMode2D.Impulse);
            }
            
        } 
    }
    void DoubleJump() {
        JumpBufferTime = 0f;
        if (jumping) {
            JumpBufferTime = MaxJumpBufferTime;
            otherJump-=1;
            rb.gravityScale = risingGravity;
            rb.velocity = new Vector2(rb.velocity.x,0f);
            rb.AddForce(Vector3.up * (JumpForce*DoubleJumpMultiplier), ForceMode2D.Impulse);
            CoyoteTime = CoyoteMaxTime;
            
        }
        else {
            if (rb.velocity.y > 0f) {
                rb.gravityScale = fallingGravity;
                rb.AddForce((rb.velocity.y/VariableJumpHeightMult)*Vector2.down,ForceMode2D.Impulse);
            }
            
        } 
    }
    void OnJump(InputAction.CallbackContext context) {
        jumping = ! jumping;
        if (jumping) {
            if (CanJump) {
                Jump();
            }
            else if (otherJump > 0f) {
                DoubleJump();
            }
            
        }
        else {
            Jump();
        }
    } 
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Kill") {
            IDamageable Health = gameObject.GetComponent<IDamageable>();
            if (Health != null) {
                Health.Damage(10000);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Kill") {
            IDamageable Health = gameObject.GetComponent<IDamageable>();
            if (Health != null) {
                Health.Damage(10000);
            }
        }
        if (col.gameObject.tag == "OneWayPlat") {
            if (rb.velocity.y > 0f){
                Physics2D.IgnoreCollision(col.GetComponent<OneWayPlat>().thisCollider, PlayerCollider,true);
            }
            else {
                Physics2D.IgnoreCollision(col.GetComponent<OneWayPlat>().thisCollider, PlayerCollider,false);
            }
        }
        if (col.gameObject.tag == "Bounds") {
            Confiners.Add(col.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Bounds") {
            // Debug.Log("EXITED");
            Confiners.Remove(col.gameObject);
            // confiner.InvalidatePathCache();
            //  confiner.m_BoundingShape2D = null;
        }
    }
}
