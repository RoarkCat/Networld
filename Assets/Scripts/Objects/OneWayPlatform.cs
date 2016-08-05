using UnityEngine;
using System.Collections;

namespace maymays
{
    public class OneWayPlatform : MonoBehaviour
    {
        void OnTriggerEnter (Collider player)
        {
            var platform = transform.parent;
            if (player.GetComponent<CapsuleCollider>())
            {
                Physics.IgnoreCollision(player.GetComponent<CapsuleCollider>(), platform.GetComponent<BoxCollider>());
            }
        }

        void OnTriggerExit (Collider player)
        {
            var platform = transform.parent;
            if (player.GetComponent<CapsuleCollider>())
            {
                Physics.IgnoreCollision(player.GetComponent<CapsuleCollider>(), platform.GetComponent<BoxCollider>(), false);
            }
        }
    }
}
