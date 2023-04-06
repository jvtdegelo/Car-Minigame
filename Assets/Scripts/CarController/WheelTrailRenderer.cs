using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRenderer : MonoBehaviour
{
    
    TopDownCarController topDownCarController;
    TrailRenderer trailRenderer;

    void Awake()
    {
        topDownCarController = GetComponentInParent<TopDownCarController>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting=false;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        if(topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            trailRenderer.emitting = true;

        else
            trailRenderer.emitting = false;

        
    }
}
