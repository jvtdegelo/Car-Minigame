using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCar : MonoBehaviour
{
    public Transform Target;
    public float pixPerUnit = 128;

    // private void LateUpdate() {
    //     Vector3 targetPositon = Target.localPosition;
    //     Vector3 newCameraPosition = new Vector3(targetPositon.x, targetPositon.y, -10f);
    //     transform.localPosition = newCameraPosition;
        
    // }
    void LateUpdate()
    {
        Vector3 targetPositon = Target.localPosition;
        transform.localPosition = new Vector3(
            Mathf.Round(targetPositon.x * pixPerUnit) / pixPerUnit,
            Mathf.Round(targetPositon.y * pixPerUnit) / pixPerUnit,
            -10f
        );        
    }
}

