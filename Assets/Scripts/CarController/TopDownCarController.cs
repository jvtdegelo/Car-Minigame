using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]

    public bool stop=false; 
    public float acceleration;
    private Vector2 lastVelocity;
    public float velocityVsUp      = 0;

    public float driftFactor        = 0.95f;
    public float accelerationFactor = 40.0f;
    public float turnFactor         = 2.3f; 
    public float maxSpeed           = 40;

    float accelerationInput = 0;
    float steeringInput     = 0;
    float rotationAngle     = 0;    
    
    public Rigidbody2D carRigidbody2D;

    void Start () 
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // public void Stop ()
    // {   
    //     carRigidbody2D.isKinematic = true;     
    //     carRigidbody2D.velocity = Vector3.zero;
    //     carRigidbody2D.angularVelocity = 0f;
    //     carRigidbody2D.MoveRotation(45f);
    //     stop=false;
    // }
    
    void FixedUpdate()
    {
        // if (stop){
        //     Stop();
        //     return;
        // }
        ApplyEngineForce();

        killOrthogonalVelocity();
        
        ApplySteering();

        CalculateAcceleration();
    }

    void CalculateAcceleration()
    {
        Vector2 accelerationVector = ((carRigidbody2D.velocity - lastVelocity) / Time.fixedDeltaTime); 
        acceleration = Vector2.Dot(transform.up, accelerationVector);
        lastVelocity = carRigidbody2D.velocity;
    }
    

    void ApplyEngineForce() 
    {
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        if ( velocityVsUp > maxSpeed && accelerationInput > 0 )
            return;

        if ( velocityVsUp < - 0.5 * maxSpeed && accelerationInput < 0 )
            return;

        if ( carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 )
            return;

        if( accelerationInput == 0 )
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 1.0f, Time.fixedDeltaTime*3);
        
        else
            carRigidbody2D.drag = 0;

        Vector2 engineForceVector;

        if (accelerationInput > 0)
            engineForceVector = transform.up * accelerationInput * accelerationFactor;
        else if (velocityVsUp<0)
            engineForceVector = transform.up * accelerationInput * accelerationFactor;
        else
            engineForceVector = transform.up * 3 * accelerationInput * accelerationFactor;
        
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = carRigidbody2D.velocity.magnitude / 8;
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor); 

        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void killOrthogonalVelocity () 
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity   = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity*driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput       = inputVector.x;
        accelerationInput   = inputVector.y;
    }

    public float VelocityVsUp()
    {
        return Vector2.Dot(transform.up, carRigidbody2D.velocity);
    }

    float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if(accelerationInput<0 && velocityVsUp>0)
            return true;

        if(Mathf.Abs(GetLateralVelocity())>60.0f)
            return true;

        return false;

    }

    // public IEnumerator MoveTo(Vector3 position)
    // {
    //     yield return new WaitForFixedUpdate();

    //     carRigidbody2D.isKinematic = true;

        
    //     carRigidbody2D.velocity = Vector3.zero;
    //     carRigidbody2D.angularVelocity = 0f;
    //     carRigidbody2D.MoveRotation(45f);

    //     yield return new WaitForFixedUpdate();
    //     transform.localPosition = position;
    //     carRigidbody2D.isKinematic = false;
    // }
}
