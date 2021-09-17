using UnityEngine;

public class Character2Controller : PlayerController
{
    public void SpawnOveride()
    {
        Respawn();
        PlayerState = State.Inactive;
    }

    protected override void ChangeCharacter()
    {
        var currentlyInactive = PlayerState == State.Inactive;
        PlayerState = currentlyInactive ? State.Grounded : State.Inactive;

        var player2 = LevelController.Player2Component;
        var player2Camera = LevelController.Player2CameraComponent;
        var player1Camera = LevelController.Player1CameraComponent;

        player2Camera.enabled = player2.PlayerState != State.Inactive;
        player1Camera.enabled = !player2Camera.enabled;
    }

    protected override bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint)
    {
        if (!LevelController.Player1Component)
        {
            return false;
        }
        return respawnPoint.transform.position.x == LevelController.Player1Component.transform.position.x
            && respawnPoint.transform.position.z == LevelController.Player1Component.transform.position.z;
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
}
