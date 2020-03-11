using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAIScript : MonoBehaviour
{
    // Variables for the target and the AI's path.
    public Transform target;
    NavMeshPath personalPath;

    //  Public variables to influence their movement.
    public float speed;
    public float maxSpeed;
    public float turnSpeed;
    int cornerNum = 0;

    //  Values required to seek towards their target.
    Vector3 force;
    Vector3 v;
    Vector3 velocity;
    Vector3 steering;
    Vector3 prevPosition;

    //  Start is called before the first frame update.
    void Start()
    {
        //  Calculate a path for the target and set the corner num to zero for iteration.
        personalPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
        cornerNum = 0;
    }

    //  Update is called once per frame.
    void Update()
    {
        //  When you reach your final target, that target is still your current one.
        if (cornerNum + 1 > personalPath.corners.Length)
        {
            cornerNum--;
        }

        //  If thae target moves, recalculate a path.
        if (target.position != prevPosition)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
            cornerNum = 0;
        }

        //  When you get close enough to the corner, move towards the next.
        if (Vector3.Distance(transform.position, personalPath.corners[cornerNum + 1]) < 1.1f)
        {
            cornerNum++;
        }

        //  Debug tools to see their path in the future, and where they are moving currently.
        for (int i = 0; i < personalPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(personalPath.corners[i], personalPath.corners[i + 1], Color.black);
        }
        Debug.DrawLine(transform.position, personalPath.corners[cornerNum + 1], Color.blue);

        //  Seek the current corner/target after checking all of the previous times and record what the position of the target is.
        Seek(personalPath.corners[cornerNum + 1]);
        prevPosition = target.position;
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
}
