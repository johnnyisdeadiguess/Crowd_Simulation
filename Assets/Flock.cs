using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    float speed;
    bool turning = false;


    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockManger.FM.minSpeed, FlockManger.FM.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockManger.FM.transform.position, FlockManger.FM.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
            turning = false;

        if (turning)
        {
            Vector3 direction = FlockManger.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                   FlockManger.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManger.FM.minSpeed, FlockManger.FM.maxSpeed);
            }
            if (Random.Range(0, 100) < 10)
            {
                ApplyRules();
            }
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockManger.FM.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= FlockManger.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;
                    
                    if (nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent< Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;

                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre/groupSize + (FlockManger.FM.goalPos - this.transform.position);
            speed = gSpeed/groupSize;
            if (speed > FlockManger.FM.maxSpeed)
            {
                speed= FlockManger.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction), 
                                                      FlockManger.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }

}
