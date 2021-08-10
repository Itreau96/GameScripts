using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private float yOffset;

    // Use this for initialization
    void Start()
    {
        yOffset = transform.position.y - player.transform.position.y;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera to player vertical position - offset
        transform.position = new Vector3(transform.position.x, player.transform.position.y + yOffset, transform.position.z);
    }
}
