using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicNavScript : MonoBehaviour
{

    public Transform target;
    NavMeshAgent agent;
    public NavMeshPath personalPath;

    public float speed;
    public float maxSpeed;
    public float turnSpeed;
    int cornerNum = 0;

    Vector3 force;
    Vector3 v;
    Vector3 velocity;
    Vector3 steering;
    Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        personalPath = new NavMeshPath();
        agent = GetComponent<NavMeshAgent>();
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
        cornerNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (cornerNum + 1 > personalPath.corners.Length)
        {
            cornerNum--;
        }

        if(target.position != prevPosition)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, personalPath);
            cornerNum = 0;
        }

        if(Vector3.Distance(transform.position, personalPath.corners[cornerNum+1]) < 1.1f)
        {
            cornerNum++;
        }

        for (int i = 0; i < personalPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(personalPath.corners[i], personalPath.corners[i + 1], Color.red);
        }
        Debug.DrawLine(transform.position, personalPath.corners[cornerNum+1], Color.blue);
        Debug.Log(Vector3.Distance(transform.position, personalPath.corners[cornerNum + 1]));
        Seek(personalPath.corners[cornerNum + 1]);
        prevPosition = target.position;
    }

    void Seek(Vector3 cornerVal)
    {
        v = (cornerVal - transform.position).normalized * speed;
        force = v - velocity;
        velocity += force;
        velocity = Vector3.ClampMagnitude(velocity, Vector3.Distance(transform.position, personalPath.corners[cornerNum + 1]));
        if (velocity != Vector3.zero)
        {
            Quaternion goalRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, turnSpeed * Time.deltaTime);
        }
        transform.position += transform.forward * velocity.magnitude * Time.deltaTime;

        Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
    }
}
