using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Groupbase : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        setTarget();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.autoTraverseOffMeshLink = true;
        navMesh.speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        check_chi();
        checkTarget();
    }
    void setTarget()
    {
        target = GameObject.FindGameObjectsWithTag("points")[Random.Range(0, GameObject.FindGameObjectsWithTag("points").Length)].gameObject.transform.position;
    }
    void checkTarget()
    {
        float dist = Vector3.Distance(target, transform.position);
        if (dist<4)
        {
            setTarget();
        }
        try
        {
            navMesh.SetDestination(target);
        }
        catch
        {

        }
    }
    void check_chi()
    {
        if(transform.childCount==0)
        {
            Destroy(gameObject);
        }
    }
}

