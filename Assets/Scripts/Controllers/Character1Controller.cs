using Cinemachine;
using System.Collections;
using UnityEngine;

public class Character1Controller : PlayerController
{
    protected override IEnumerator ChangeCharacter()
    {
        PlayerState = State.Inactive;
        LevelController.Player1Camera.enabled = false;

        yield return new WaitForEndOfFrame();

        LevelController.Player2.PlayerState = State.Grounded;
        LevelController.Player2Camera.enabled = true;
    }

    protected override bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint)
    {
        if (!LevelController.Player2)
        {
            return false;
        }
        return respawnPoint.transform.position.x == LevelController.Player2.transform.position.x 
            && respawnPoint.transform.position.z == LevelController.Player2.transform.position.z;
    }

    protected override float GetPullingObjectWeight(Rigidbody @object)
    {
        return @object.mass;
    }

    protected override void Rotate(Vector3 movementDirection)
    {
        var cameraToPlayerVector = new Vector3(_mainCamera.forward.x, 0, _mainCamera.forward.z);
        var forwardRotation = Quaternion.LookRotation(cameraToPlayerVector, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, forwardRotation, RotationSpeed * Time.deltaTime);
    }

    protected override void Setup()
    {
        PlayerCamera = Instantiate(PlayerCamera);
        var camera = PlayerCamera.GetComponent<CinemachineVirtualCamera>();

        LevelController.Player1 = this;
        LevelController.Player1Camera = camera;

        camera.Follow = transform.Find("POV");
        camera.m_Lens.FieldOfView = PlayerPrefs.GetFloat("FieldOfViewPreference");

        PlayerState = State.Grounded;
    }
}
