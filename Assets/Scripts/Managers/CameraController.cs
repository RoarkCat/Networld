using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public float smoothTime = .6f;
    public Transform target;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            Vector3 targetPosition = target.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + offset, ref velocity, smoothTime);
        }
    }
}