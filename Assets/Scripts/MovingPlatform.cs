using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public Transform start;
    public Transform end;
    [SerializeField] private float speed = 2f;

    Vector3 nextPos;
    // Start is called before the first frame update
    void Start() {
        nextPos = start.position;
    }

    // Update is called once per frame
    void Update() {
        if (transform.position == start.position) {
            nextPos = end.position;
        } else if (transform.position == end.position) {
            nextPos = start.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(start.position, end.position);
    }
}
