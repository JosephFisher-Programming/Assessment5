using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicNavScript : MonoBehaviour
{
    //  Transforms and Navigation data for the target, enemy, and itself.
    public Transform target;
    public Transform enemy;
    NavMeshAgent agent;
    public NavMeshPath personalPath;

    //  Numbers required to control speed, and turn speed.
    public float speed;
    public float fleeSpeed;
    public float maxSpeed;
    public float maxFleeSpeed;
    public float turnSpeed;
    int cornerNum = 0;
    float timer = 1;

    //  Values required to seek towards their target and flee from them too.
    Vector3 force;
    Vector3 v;
    Vector3 velocity;
    Vector3 steering;
    Vector3 prevPosition;

    //  Start is called before the first frame update.
    void Start()
    {
        //  Calculates a path to the current target and sets the corner count to 0.
        personalPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
        cornerNum = 0;
    }

    //  Public void for the Runner to call.
    public void MoveToTarget()
    {
        //  Recalculates a path incase the target moves.
        if (timer < 1)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
            cornerNum = 0;
        }

        //  When you reach your final target, that target is still your current one.
        if (cornerNum + 1 > personalPath.corners.Length)
        {
            cornerNum--;
        }

        //  If the target moves, reset the route.
        if(target.position != prevPosition)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
            cornerNum = 0;
        }

        //  When you move close enoughht to the current paths corner, focus on the next. 
        if(Vector3.Distance(transform.position, personalPath.corners[cornerNum+1]) < 1.6f)
        {
            cornerNum++;
        }

        //  Debug Script for the current corner targeted.
        Debug.DrawLine(transform.position, personalPath.corners[cornerNum+1], Color.blue);

        //  "Seeks" the current corner of the path.
        Seek(personalPath.corners[cornerNum + 1]);
        prevPosition = target.position;

        
    }

    // Update is called once per frame and checks for flee conditions.
    public void Update()
    {
       //  Reduces the timer each fram to recalc the path.
       timer -= Time.deltaTime;

        //  Debug tool for the current path to the target.
       for (int i = 0; i < personalPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(personalPath.corners[i], personalPath.corners[i + 1], Color.red);
        }

       //  If too close to the enemy, then flee from them.
       if(Vector3.Distance(this.transform.position, enemy.position) < 3f)
        {
            Flee();
        }
       
    }

    //  Seek void that calculates the target velocity and moves towards the target's position.
    void Seek(Vector3 cornerVal)
    {
        v = (cornerVal - transform.position).normalized * speed;
        force = v - velocity;
        velocity += force;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        if (velocity != Vector3.zero)
        {
            Quaternion goalRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, turnSpeed * Time.deltaTime);
        }
        transform.position += transform.forward * velocity.magnitude * Time.deltaTime;
    }

    //  Seek void that calculates the target velocity and runs away from the enemies position.
    void Flee()
    {
        v = (transform.position - enemy.position).normalized * fleeSpeed;
        force = v - velocity;
        velocity += force;
        velocity = Vector3.ClampMagnitude(velocity, maxFleeSpeed);
        if (velocity != Vector3.zero)
        {
            Quaternion goalRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, turnSpeed * Time.deltaTime);
        }
        transform.position += transform.forward * velocity.magnitude * Time.deltaTime;
    }


}
