using MLAPI;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class PlayerController : NetworkBehaviour
{
    [ReadOnly]
    public State PlayerState;
    public enum State
    {
        Inactive,
        Grounded,
        Jumping,
        Pulling
    }

    [Space]
    public float Speed;
    public float MaxSpeed;
    public float RotationSpeed;
    public float JumpStength;
    public float PullDistance;
    public float SprintMultiplier;

    protected Rigidbody _rb;
    protected GameObject _objectBeingPulled;
    protected float _defaultSpeed;
    protected Transform _mainCamera;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _defaultSpeed = Speed;

        _mainCamera = GameObject.Find("MainCamera").transform;

        Respawn();
    }

    void Update()
    {
        if (InputManager.GetButtonDown("ChangeCharacterKeybind") && !LevelController.IsMultiplayer)
        {
            ChangeCharacter();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LevelController.IsMultiplayer)
            {
                PlayerState = LevelController.IsPaused ? State.Inactive : State.Grounded;
            }
        }

        if (PlayerState == State.Inactive)
        {
            return;
        }

        if (InputManager.GetButtonDown("JumpKeybind") && PlayerState != State.Jumping)
        {
            _rb.AddForce(new Vector3(0, JumpStength, 0), ForceMode.Impulse);
            PlayerState = State.Jumping;
        }

        if (InputManager.GetButton("SprintKeybind"))
        {
            Speed *= SprintMultiplier;
        }

        if (InputManager.GetButtonUp("SprintKeybind"))
        {
            Speed = _defaultSpeed;
        }

        if (InputManager.GetButton("PullKeybind"))
        {
            var nextToObject = Physics.CheckSphere(transform.position, PullDistance);
            if (nextToObject)
            {
                GameObject closestPullableObject = null;
                foreach (var pullableObject in GameObject.FindGameObjectsWithTag("PullableObj"))
                {
                    if ((pullableObject.transform.position - transform.position).magnitude <= PullDistance)
                    {
                        closestPullableObject = pullableObject;
                    }
                }
                if (closestPullableObject != null)
                {
                    _objectBeingPulled = closestPullableObject;
                    PullObject(true);
                }
            }
        }

        if (InputManager.GetButtonUp("PullKeybind"))
        {
            PullObject(false);
        }
    }

    void FixedUpdate()
    {
        if (PlayerState == State.Inactive)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        var movementDirection = new Vector3(x, 0, z);

        Speed = Speed > MaxSpeed ? MaxSpeed : Speed;

        transform.Translate(movementDirection * Speed * Time.deltaTime);

        if (!Physics.Raycast(transform.position, -transform.up, 5))
        {
            _rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            _rb.constraints = (RigidbodyConstraints)80;

            var getUpRotation = new Quaternion(0f, transform.rotation.y, 0, transform.rotation.w);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, getUpRotation, RotationSpeed * Time.deltaTime);
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2, Color.cyan);
        Rotate(movementDirection);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PlayerState == State.Inactive)
        {
            return;
        }

        if (collision.transform.tag == "DeathPoint")
        {
            Respawn();
        }

        if (collision.gameObject.layer == 3)
        {
            PlayerState = State.Grounded;
        }
    }

    protected void Respawn()
    {
        var playerPosition = FindRespawnPoint();
        transform.position = playerPosition;
    }

    void PullObject(bool pull)
    {
        if (_objectBeingPulled != null)
        {
            var @object = _objectBeingPulled.GetComponent<Rigidbody>();
            var objectWeight = GetPullingObjectWeight(@object);

            FixedJoint objectJoint;
            if (!@object.gameObject.TryGetComponent(out objectJoint))
            {
                objectJoint = @object.gameObject.AddComponent<FixedJoint>();
            }

            if (pull)
            {
                objectJoint.connectedBody = _rb;
                Speed = _defaultSpeed / objectWeight;
                return;
            }
            Destroy(objectJoint);
            Speed = _defaultSpeed;
        }
    }

    Vector3 FindRespawnPoint()
    {
        var previousRespawnPointDistance = 9999999999999999999999999f;
        var respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        var closestrespawnPoint = respawnPoints[0];

        foreach (var respawnPoint in respawnPoints)
        {
            var respawnPointDistance = Vector3.Distance(transform.position, respawnPoint.transform.position);
            if (respawnPointDistance < previousRespawnPointDistance && !CheckOtherPlayerIsntAtGivenRespawnPoint(respawnPoint))
            {
                closestrespawnPoint = respawnPoint;
                previousRespawnPointDistance = respawnPointDistance;
            }
        }
        return new Vector3(closestrespawnPoint.transform.position.x, 2, closestrespawnPoint.transform.position.z);
    }

    protected abstract bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint);

    protected abstract void ChangeCharacter();

    protected abstract float GetPullingObjectWeight(Rigidbody @object);

    protected abstract void Rotate(Vector3 movementDirection);
}
