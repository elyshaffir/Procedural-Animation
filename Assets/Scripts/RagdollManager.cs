using UnityEngine;

class RagdollManager : MonoBehaviour
{
    public GameObject playerModel;
    public GameObject playerRagdoll;

    void Update()
    {
        // add the forces of the rigidbody
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerModel.transform.parent.gameObject.SetActive(false);
            playerRagdoll.transform.parent.gameObject.SetActive(true);
            ToRagdoll(playerModel.transform, playerRagdoll.transform, playerModel.transform.parent.GetComponent<Rigidbody>());
        }
    }

    void ToRagdoll(Transform currentAtModel, Transform currentAtRagdoll, Rigidbody playerRigidbody)
    {
        if (currentAtModel.childCount != currentAtRagdoll.childCount)
        {
            throw new UnityException("The model and the ragdoll are not exactly the same!");
        }

        Rigidbody ragdollRigidbody;
        if (currentAtRagdoll.TryGetComponent<Rigidbody>(out ragdollRigidbody))
        {
            ragdollRigidbody.velocity = playerRigidbody.velocity;
        }

        currentAtRagdoll.position = currentAtModel.position;
        currentAtRagdoll.rotation = currentAtModel.rotation;

        for (int i = 0; i < currentAtModel.childCount; i++)
        {
            ToRagdoll(currentAtModel.GetChild(i), currentAtRagdoll.GetChild(i), playerRigidbody);
        }
    }
}
