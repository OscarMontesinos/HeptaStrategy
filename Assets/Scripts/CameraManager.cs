using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [Range(10,50)]
    public float speed;

    public float spdZoom;
    public float maxZoom;
    public float minZoom;

    Camera cam;

    private void Awake()
    {
        cam = Camera.main;


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x +speed * Time.deltaTime, transform.position.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x -speed * Time.deltaTime, transform.position.y);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            cam.orthographicSize += spdZoom;
            if (cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize = maxZoom;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            cam.orthographicSize -= spdZoom;
            if (cam.orthographicSize < minZoom)
            {
                cam.orthographicSize = minZoom;
            }

        }
    }
}
