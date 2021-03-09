using UnityEngine;
using UnityEditor;

/// <summary>
///   Select the parent object and run this to set all the child objects as
///   active.
/// </summary>
public class SetAllChildObjectsActive
{
    [MenuItem("GameObject/Set All Child Objects Active")]
    static void SetChildObjectsActive()
    {
        Selection.activeGameObject.SetActive(true);
    }
}

/// <summary>
///   Select the parent object and run this to set all the child objects as
///   inactive.
/// </summary>
public class SetAllChildObjectsInactive
{
    [MenuItem("GameObject/Set All Child Objects Inactive")]
    static void SetChildObjectsInactive()
    {
        Selection.activeGameObject.SetActive(false);
    }
}