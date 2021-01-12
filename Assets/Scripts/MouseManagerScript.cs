using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManagerScript : MonoBehaviour
{
    Vector3? initialMousePosition = null;
    Vector3? initialCameraPosition = null;
    float scrollSensitivity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Mouse Position" + Input.mousePosition);

        //Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("World Point" + worldPoint);

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Physics.Raycast(ray))
        //{
        //    Debug.Log("Raycast hit something");
        //}

        //RaycastHit2D hitinfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, (float)1000000.0);

        //if (hitinfo.collider != null)
        //{
        //    Debug.Log(hitinfo.collider);

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        MeshRenderer mr = hitinfo.collider.gameObject.GetComponentInChildren<MeshRenderer>();
        //        mr.material.color = Color.red;
        //    }

        //}
        //else
        //{
        //    Debug.Log("nothing");
        //}

        //Debug.Log(Input.GetAxis("Mouse ScrollWheel"));


        AdjustCameraBasedOnUserInput();
        

    }

    void AdjustCameraBasedOnUserInput()
    {
        HandleDragMotion();
        HandleZoomMotion();
    }

    void HandleDragMotion()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = new Vector3(0, 0, 0);
            if (initialMousePosition == null && initialCameraPosition == null)
            {
                initialMousePosition = Input.mousePosition;
                initialCameraPosition = Camera.main.transform.position;

            }
            delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint((Vector3)initialMousePosition);
            Debug.Log(delta);


            Camera.main.transform.position = (Vector3)initialCameraPosition - delta;

        }
        else
        {
            this.initialMousePosition = null;
            this.initialCameraPosition = null;
        }
    }

    void HandleZoomMotion()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel") * this.scrollSensitivity;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scrollDelta, 1.0f, 30.0f);
    }
}
