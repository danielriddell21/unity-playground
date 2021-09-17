using UnityEngine;

public class Character1Controller : PlayerController
{
    protected override void ChangeCharacter()
    {
        var currentlyInactive = PlayerState == State.Inactive;
        PlayerState = currentlyInactive ? State.Grounded : State.Inactive;

        var player1 = LevelController.Player1Component;
        var player1Camera = LevelController.Player1CameraComponent;
        var player2Camera = LevelController.Player2CameraComponent;

        player1Camera.enabled = player1.PlayerState != State.Inactive;
        player2Camera.enabled = !player1Camera.enabled;
    }

    protected override bool CheckOtherPlayerIsntAtGivenRespawnPoint(GameObject respawnPoint)
    {
        if (!LevelController.Player2Component)
        {
            return false;
        }
        return respawnPoint.transform.position.x == LevelController.Player2Component.transform.position.x 
            && respawnPoint.transform.position.z == LevelController.Player2Component.transform.position.z;
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
}
