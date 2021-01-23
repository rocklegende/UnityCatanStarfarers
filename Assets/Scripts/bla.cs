﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bla : MonoBehaviour
{
    public SpacePoint point;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.white;
    }

    private void OnMouseDown()
    {
        point.print();

        // Send notification that this button was clicked
        this.transform.parent.gameObject.GetComponent<Space.SpacePointButtonScript>().OnClick();
    }


}
