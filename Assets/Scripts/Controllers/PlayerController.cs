using Cinemachine;
using MLAPI;
using System.Collections;
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
        Jumping
    }

    [Space]
    public float Speed;
    public float MaxSpeed;
    public float RotationSpeed;
    public float JumpStength;
    public float PullDistance;
    public float SprintMultiplier;

    [Space]
    public float SC_CooldownAmount;

    [Space]
    public GameObject PlayerCamera;

    public static ulong NetworkClientId;

    protected Rigidbody _rb;
    protected GameObject _objectBeingPulled;
    protected float _defaultSpeed;
    protected Transform _mainCamera;

    public CooldownTimer _switchCharacterCooldown;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _defaultSpeed = Speed;

        _mainCamera = GameObject.Find("MainCamera").transform;

        _switchCharacterCooldown
            = InterfaceController.SwitchCharacterCooldown = new CooldownTimer(SC_CooldownAmount);

        Setup();
        Respawn();
    }

    void Update()
    {
        if (PlayerState == State.Inactive)
        {
            return;
        }

        if (InputManager.GetButtonDown("ChangeCharacterKeybind")
            && !LevelController.IsMultiplayer)
        {
            if (_switchCharacterCooldown.IsActive)
            {
                return;
            }

            StartCoroutine(ChangeCharacter());
            _switchCharacterCooldown.StartTimer(5);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (InputManager.GetButtonDown("JumpKeybind") 
            && PlayerState == State.Grounded)
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
            CheckIfPullableObject();
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

    void CheckIfPullableObject()
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

    void TogglePauseMenu()
    {
        if (LevelController.IsPaused)
        {
            LevelController.PauseMenu.SetActive(false);
            _mainCamera.GetComponent<CinemachineBrain>().enabled = true;

            if (LevelController.IsMultiplayer)
            {
                PlayerState = State.Inactive;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else
        {
            LevelController.PauseMenu.SetActive(true);
            _mainCamera.GetComponent<CinemachineBrain>().enabled = false;

            if (LevelController.IsMultiplayer)
            {
                PlayerState = State.Grounded;
            } 
            else
            {
                Time.timeScale = 0f;
            }
        }

        LevelController.IsPaused = !LevelController.IsPaused;
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

    void Respawn()
    {
        var playerPosition = FindRespawnPoint();
        transform.position = playerPosition;
    }

    protected abstract bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint);

    protected abstract IEnumerator ChangeCharacter();

    protected abstract float GetPullingObjectWeight(Rigidbody @object);

    protected abstract void Rotate(Vector3 movementDirection);

    protected abstract void Setup();
}
