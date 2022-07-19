using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 mousePos;
    public Vector3 mouseDir;
    public Camera cam;
    public LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
    }

    private void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseDir = mousePos - gameObject.transform.position;
        mouseDir.z = 0;
        mouseDir = mouseDir.normalized;

        if (Input.GetMouseButtonDown(0))
        {
            lr.enabled = true;
        }

        // if (Input.GetMouseButton(0))
        {
            startPos = gameObject.transform.position;
            startPos.z = 0;
            lr.SetPosition(0, startPos);
            endPos = mousePos;
            endPos.z = 0;
            lr.SetPosition(1,endPos);
        }

        // if (Input.GetMouseButtonUp(0))
            // lr.enabled = false;
    }
}
