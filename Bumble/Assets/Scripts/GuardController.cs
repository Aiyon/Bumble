using UnityEngine;
using System.Collections;

public class GuardController : MonoBehaviour {

    //update test
    float nextActionTime = 0.0f;
    float period = 0.25f;
    float time;
    	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if(time > nextActionTime)
        {
            nextActionTime += period;
            Vector3 newPos = Vector3.zero;
            int dir = Random.Range(0, 5);
            switch (dir)
            {
                case 0:
                    newPos.x = -0.5f;
                    break;

                case 1:
                    newPos.x = 0.5f;
                    break;

                case 2:
                    newPos.y = -0.5f;
                    break;

                case 3:
                    newPos.y = 0.5f;
                    break;

                default:
                    newPos = Vector3.zero;
                    break;

            }
            gameObject.transform.position = newPos + gameObject.transform.position;
        }
    }
}
