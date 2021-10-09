using Cinemachine;
using System.Collections;
using UnityEngine;

public class Character2Controller : PlayerController
{
    protected override IEnumerator ChangeCharacter()
    {
        PlayerState = State.Inactive;
        LevelController.Player2Camera.enabled = false;

        yield return new WaitForEndOfFrame();

        LevelController.Player1.PlayerState = State.Grounded;
        LevelController.Player1Camera.enabled = true;
    }

    protected override bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint)
    {
        if (!LevelController.Player1)
        {
            return false;
        }
        return respawnPoint.transform.position.x == LevelController.Player1.transform.position.x
            && respawnPoint.transform.position.z == LevelController.Player1.transform.position.z;
    }

    protected override float GetPullingObjectWeight(Rigidbody @object)
    {
        return @object.mass * 10000000000000;
    }

    protected override void Rotate(Vector3 movementDirection)
    {
        if (movementDirection != Vector3.zero)
        {
            var cameraToPlayerVector = new Vector3(_mainCamera.forward.x, 0, _mainCamera.forward.z);
            var forwardRotation = Quaternion.LookRotation(cameraToPlayerVector, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, forwardRotation, RotationSpeed * Time.deltaTime);
        }
    }

    protected override void Setup()
    {
        PlayerCamera = Instantiate(PlayerCamera);
        var camera = PlayerCamera.GetComponent<CinemachineFreeLook>();

        LevelController.Player2 = this;
        LevelController.Player2Camera = camera;

        camera.Follow = transform.Find("POV");
        camera.LookAt = transform.Find("POV");

        camera.m_Lens.FieldOfView = PlayerPrefs.GetFloat("FieldOfViewPreference");
        camera.m_YAxis.m_MaxSpeed =
            camera.m_XAxis.m_MaxSpeed = PlayerPrefs.GetFloat("SensitivityPreference") * 5;

        if (!LevelController.IsMultiplayer)
        {
            PlayerState = State.Inactive;
            camera.enabled = false;
        }
    }
}
