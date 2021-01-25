using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFApplication : MonoBehaviour
{
    public void Notify(string p_event_path, Object p_target, params object[] p_data)
    {
        SFController[] controller_list = GetAllControllers();
        foreach (SFController c in controller_list)
        {
            c.OnNotification(p_event_path, p_target, p_data);
        }
    }

    // Fetches all scene Controllers.
    public SFController[] GetAllControllers() {
        SFController[] cntrls = GameObject.FindObjectsOfType<SFController>();

        return GameObject.FindObjectsOfType<SFController>();
    }
}
