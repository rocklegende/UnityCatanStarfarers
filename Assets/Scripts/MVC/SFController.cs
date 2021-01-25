﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SFController : SFElement
{
    public abstract void OnNotification(string p_event_path, Object p_target, params object[] p_data);
}

