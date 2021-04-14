using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SFElement : MonoBehaviourPunCallbacks
{
    public SFApplication app { get { return GameObject.FindObjectOfType<SFApplication>(); } }
    public Player[] playersShared;
}
