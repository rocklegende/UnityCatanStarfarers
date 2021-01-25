using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFElement : MonoBehaviour
{
    public SFApplication app { get { return GameObject.FindObjectOfType<SFApplication>(); } }
}
