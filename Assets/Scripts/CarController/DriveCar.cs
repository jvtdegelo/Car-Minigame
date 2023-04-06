using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;


public class DriveCar : Agent
{
    public bool isPlayer;
    public RacingTrack racingTrack;
    private TopDownCarController topDownCarController;
    private int lastBlock;
    private float lastVelocity;
    private int throughStart;
    private bool inStart;
    private bool inEnd;
    public float delay = 0.00f;
    float timer;
    
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        throughStart = 0;
        inStart = false;
        inEnd = false;

        Vector3 currentPosition = transform.localPosition;
 
        float x = 192f;
        float y = isPlayer? 128f*(4)+70f: 128f*(4)+54f;
        // float y = Random.Range(210f, 230f);
        // float x = Random.Range(50f, 70f);

        Vector3 position = Vector3.zero;

        position.x = x;
        position.y = y;

        lastBlock = 1;

        // topDownCarController.MoveTo(position);
        transform.localPosition = position;
        topDownCarController.carRigidbody2D.velocity = Vector3.zero;
        topDownCarController.carRigidbody2D.angularVelocity = 0f;
        topDownCarController.stop = true;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float velocity = topDownCarController.velocityVsUp/200f;
        sensor.AddObservation(velocity);
        float acceleration = topDownCarController.acceleration/180f;
        sensor.AddObservation(acceleration);
        float angle = CalculateCarAngleToNextTile();
        sensor.AddObservation(angle); 
        float distanceToWallFront = CalculateDistanceToWallFront();
        sensor.AddObservation(distanceToWallFront);
        float distanceToWallRight = CalculateDistanceToWallRight();
        sensor.AddObservation(distanceToWallRight);
        float distanceToWallLeft = CalculateDistanceToWallLeft();
        sensor.AddObservation(distanceToWallLeft);
        float distanceToNextTurn = CalculateDistanceToNextTurn();
        sensor.AddObservation(distanceToNextTurn);
        int nextTurnDirection = CalculateNextTurnDirection();
        sensor.AddObservation(nextTurnDirection);
        int nextTurnTurnDirection = CalculateNextTurnTurnDirection();
        sensor.AddObservation(nextTurnTurnDirection);
        float nextTurnLength = CalculateNextTurnLength();
        sensor.AddObservation(nextTurnLength);

        // Debug.Log("distance to wall left: " + distanceToWallLeft.ToString());
        // Debug.Log("distance to wall right: " + distanceToWallRight.ToString());
        // Debug.Log("distance to wall front: " + distanceToWallFront.ToString());
        // Debug.Log("distance to turn: " + distanceToNextTurn.ToString());
        // Debug.Log("next turn direction: " + nextTurnDirection.ToString());
        // Debug.Log("next turn turn direction: " + nextTurnTurnDirection.ToString());
        // Debug.Log("velocity: " + velocity.ToString());
        // Debug.Log("acceleration: " + acceleration.ToString());
        // Debug.Log("angle: " + angle.ToString());
        // Debug.Log("O tamanho da proxima curva: " + nextTurnLength.ToString());
    }

    private float CalculateDistanceToWallLeft()
    {
        float maxDist=128f;
        int layerMask = 1 << 6;
        Vector3 angle = Quaternion.Euler(0, 0, 90f) * transform.up; 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, angle, maxDist, layerMask);
        if (hit)
            return (hit.distance-maxDist)/maxDist;
        else 
            return 1f;
    }
    
    private float CalculateDistanceToWallRight()
    {
        float maxDist=128f;
        int layerMask = 1 << 6;
        Vector3 angle = Quaternion.Euler(0, 0, -90f) * transform.up; 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, angle, maxDist, layerMask);
        if (hit)
            return (hit.distance-maxDist)/maxDist;
        else 
            return 1f;
    }

    private float CalculateDistanceToWallFront()
    {
        float maxDist=128f;
        int layerMask = 1 << 6;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxDist, layerMask);
        if (hit)
            return (hit.distance-maxDist)/maxDist;
        else 
            return 1f;
    }

    private int CalculateNextDirection()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        
        Vector2Int direction = racingTrack._map.NextTile(i, j);

        return direction.x + 2* direction.y;

    }

    private float CalculateNextTurnLength()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        
        Vector2Int nextTurnLength = racingTrack._map.NextTurnLength(i, j);

        float length = Mathf.Abs(nextTurnLength.x + nextTurnLength.y)*128f+128f; 
    
        return (length-640f)/384f;
    }

    private int CalculateNextTurnTurnDirection()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        
        int nextTurnTurnSide = racingTrack._map.NextTurnTurnSide(i, j);

        return nextTurnTurnSide;
    }

    private int CalculateNextTurnDirection()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        
        int nextTurnSide = racingTrack._map.NextTurnSide(i, j);

        return nextTurnSide;
    }

    private float CalculateDistanceToNextTurn()
    {
        Vector3 currentPosition = transform.localPosition;

        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        Vector2Int nextTurn = racingTrack._map.DirectionNextTurn(i, j);
        
        float amountToCompleteSquare;
        if (nextTurn.x == 0 && nextTurn.y>=0)
            amountToCompleteSquare = 128f - currentPosition.y % 128;
        else if (nextTurn.x == 0 && nextTurn.y<0)
            amountToCompleteSquare = currentPosition.y % 128;
        else if (nextTurn.y == 0 && nextTurn.x>=0)
            amountToCompleteSquare = 128f - currentPosition.x % 128;
        else
            amountToCompleteSquare = currentPosition.x % 128;

        
        float distance = Mathf.Abs(nextTurn.x + nextTurn.y)*128 + amountToCompleteSquare - 64f;
        return distance > 256 ? 1f : (distance-128)/128;
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        CheckLevel();
        CalculateReward();
        Vector2 inputVector = Vector2.zero;
        inputVector.x = actions.ContinuousActions[0];
        inputVector.y = actions.ContinuousActions[1];
        topDownCarController.SetInputVector(inputVector);
             
    }
    private void CheckLevel()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        bool start = (j==0) && (i==4);
        bool end = (j==0) && (i==3);

        if (inEnd && start) throughStart+=1;
        else if (inStart && end) throughStart-=1;

        inEnd = end;
        inStart = start;

        if (throughStart == 2 && isPlayer){
            float y = currentPosition.y/128 - 5f;
            if(y>0.3)
                SceneManager.LoadScene(0);
        }
            
        
    }

    private float CalculateCarAngleToNextTile()
    {
        Vector3 currentPosition = transform.localPosition;
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;
        
        Vector2 trackDirection = racingTrack._map.NextTile(i, j);
        Vector2 carDirection = topDownCarController.carRigidbody2D.velocity;
        
        float angle = Vector2.SignedAngle(carDirection, trackDirection);
        if (angle>90f) angle = 90f;
        else if (angle<-90f) angle=-90f;
        float sin = Mathf.Sin(Mathf.Deg2Rad*angle);
        return sin;
    }

    private void CalculateReward()
    {
        Vector3 currentPosition = transform.localPosition;
        
        int j = ((int)(currentPosition.x/128))-1; 
        int i = ((int)(currentPosition.y/128))-1;

        int [,] map = racingTrack._map.map;

        int currentBlock = map[i,j];

        bool completedLap = (currentBlock == 1) && (lastBlock>2);
        bool returnedAtStart = (lastBlock == 1) && (currentBlock>2);
        float angleToNextTile = CalculateCarAngleToNextTile();
        float reward = -10f; //-3*Mathf.Abs(angleToNextTile);
        if (!returnedAtStart && (currentBlock>lastBlock || completedLap))
            reward+=500f;

        else if(currentBlock<lastBlock || returnedAtStart)
            reward-=(1000f);

        else if(topDownCarController.velocityVsUp<0)
            reward-=10f;

        AddReward(reward);
        lastBlock = currentBlock;
    }
}
