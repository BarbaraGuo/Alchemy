using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (IsOnLayer(collision, "Ground")) {
            player.isGrounded = true;
        } else if (IsOnLayer(collision, "MovingPlatform")) {
            player.isGrounded = true;
            player.transform.parent = collision.gameObject.transform; // Move with platform
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (IsOnLayer(collision, "Ground")) {
            player.isGrounded = false;
        } else if (IsOnLayer(collision, "MovingPlatform")) {
            player.isGrounded = false;
            player.transform.parent = null; // No longer move with platform
        }
    }


    private bool IsOnLayer(Collider2D collision, string layer) {
        return collision.gameObject.layer == LayerMask.NameToLayer(layer);
    }

}
