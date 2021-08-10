using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In-game physics object implementation.
/// Manages movement given physics engine variables (gravity for example).
/// Adapted from Unity tutorial: https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b68e
/// </summary>
public abstract class PhysicsObject : MonoBehaviour
{
    #region Properties

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    [HideInInspector]
    public float minGroundNormalY = .65f;
    [HideInInspector]
    public float minWallNormalX = .65f;

    public LayerMask collisionMask;
    public float gravityModifier = 1f;
    protected Vector2 targetVelocity;
    protected bool grounded;
    protected bool walled;
    protected bool ledge;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    #endregion

    // Retrieve rigid body component.
    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Called first frame at runtime. Provided by Monobehaviour.
    void Start()
    {
        // Subclass start function to be used by child classes
        Initialize();
        // Setup additional layer masks
        SetupLayerMask();
    }

    // Abstract function used to provide subclass-dependent initialization.
    protected virtual void Initialize() { /* No base implementation at present. */ }

    // Can be overriden by subclasses to override layer masking.
    void SetupLayerMask()
    {
        // Ignore designated layers and triggers
        contactFilter.SetLayerMask(collisionMask); // Selected in game engine GUI inspector.
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;
    }

    // Called once per frame.
    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    // Abstract velocity computation that is subclass dependent.
    protected abstract void ComputeVelocity();

    // Called every fixed-framerate frame. Used to apply velocity computed by subclass.
    void FixedUpdate()
    {
        // Determine movement vectors.
        DetermineMovement();
    }

    // Updates physics state and determines movement vectors.
    void DetermineMovement()
    {
        // Determine movement vectors.
        velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
        velocity.x = targetVelocity.x;
        grounded = false;
        walled = false;
        Vector2 deltaPosition = velocity * Time.fixedDeltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;
        ApplyMovement(move, false);
        move = Vector2.up * deltaPosition.y;
        ApplyMovement(move, true);
    }

    // Apply movement using vector and y-locking flag.
    // Adapted from Unity physics tutorial.
    void ApplyMovement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;
        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                // Determine if walled
                if (currentNormal.x < -minWallNormalX && move.x > 0 ||
                    currentNormal.x > minWallNormalX && move.x < 0)
                {
                    walled = true;
                }
                // Determine if grounded
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity -= projection * currentNormal;
                }
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb2d.position += move.normalized * distance;
    }
}
