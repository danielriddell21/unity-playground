using System.Collections;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    public GameObject TeleportDesintation;

    private Vector3 _objectvVelocity;

    IEnumerator OnCollisionEnter(Collision collision)
    {
        TeleportDesintation.GetComponent<BoxCollider>().enabled = false;

        GrabAndResetObjectVelocity(collision.gameObject.GetComponent<Rigidbody>());

        var dest = new Vector3(TeleportDesintation.transform.position.x, TeleportDesintation.transform.position.y + 1, TeleportDesintation.transform.position.z);
        collision.gameObject.transform.position = dest;

        collision.gameObject.GetComponent<Rigidbody>().velocity = _objectvVelocity / 2;

        yield return new WaitForSeconds(5);
        TeleportDesintation.GetComponent<BoxCollider>().enabled = true;
    }

    private void GrabAndResetObjectVelocity(Rigidbody collider)
    {
        _objectvVelocity = collider.velocity;
        collider.velocity = Vector3.zero;
    }
}
