using UnityEngine;
using System.Collections.Generic;

public class BeeBrain : MonoBehaviour {

    protected GameObject home;
    public GameObject scriptManager;
    protected GameObject target;
    protected List<GameObject> path = new List<GameObject>();
    protected int pIndex;

    public GameObject sprite;
    protected bool moving;

    protected float period = 1.0f;

	// Use this for initialization
	void Start ()
    {
        target = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (target != null)
        {
            moveTowards(target.transform.position);
        }
        else
        {
            brain();
        }
	}


    void moveTowards(Vector3 dir)
    {
        Vector3 newPos = gameObject.transform.position;
        Vector3 move = dir - gameObject.transform.position; move.Normalize();

        newPos.x += (move.x * Time.deltaTime) ;// period;
        newPos.y += (move.y * Time.deltaTime) ;// period;
        gameObject.transform.position = newPos;

        //set z rotation
        Vector3 rot = sprite.transform.rotation.eulerAngles;

        rot.z = Mathf.Atan(move.y / move.x); rot.z *= 180 / Mathf.PI;
        if (move.y == 0)
        {
            if (move.y > 0)
                rot.z = 180;
            else rot.z = 0;
        }
        else if (move.x == 0)
        {
            rot.z = 90;
        }
        if (move.x < 0)
            rot.z += 180;
        //if (move. y < 0) rot.z *= -1;
        rot.z += 90;
        sprite.transform.rotation = Quaternion.Euler(rot);

        Vector3 dist = gameObject.transform.position - target.transform.position;
        dist.z = 0;
        if (dist.sqrMagnitude < 0.0001)
        {
            if (pIndex < path.Count - 1)
            {
                pIndex++;
                target = path[pIndex];
            }
            else
            {
                path.Clear();
                target = null;
            }
        }
    }

    protected virtual void brain()
    {
        //A* to find new target.
    }

    public void setSM(GameObject gO)
    {
        scriptManager = gO;
    }

    public void setHome(GameObject h)
    { home = h; }
}
