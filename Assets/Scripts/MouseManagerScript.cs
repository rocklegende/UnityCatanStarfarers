using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManagerScript : MonoBehaviour
{
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

        RaycastHit2D hitinfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, (float)1000000.0);

        if (hitinfo.collider != null)
        {
            Debug.Log(hitinfo.collider);

            if (Input.GetMouseButtonDown(0))
            {
                MeshRenderer mr = hitinfo.collider.gameObject.GetComponentInChildren<MeshRenderer>();
                mr.material.color = Color.red;
            }

        } else
        {
            Debug.Log("nothing");
        }
        
    }
}
