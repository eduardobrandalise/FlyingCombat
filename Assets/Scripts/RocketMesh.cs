using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RocketMesh : MonoBehaviour
{
    public CollisionEvent collided;
    [System.Serializable]
    public class CollisionEvent : UnityEvent<Collider> { }

    private void OnTriggerEnter(Collider other)
    {
        collided.Invoke(other);
    }
}
