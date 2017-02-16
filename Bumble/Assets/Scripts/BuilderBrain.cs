using UnityEngine;
using System.Collections.Generic;

public class BuilderBrain : BeeBrain {

    bool collecting;

    override protected void brain()
    {
        float sqrMag = 131072; //first power of 2 above 100k, allows for 100x100 distance between bee and destination, should never occur.
        if (collecting)
        {
            collecting = false;
            target = home;
        }
        else
        {
            collecting = true;
            List<GameObject> cells = new List<GameObject>(scriptManager.GetComponent<ResourceManager>().getCellList(0));
            cells.Add(scriptManager.GetComponent<ResourceManager>().getQueen());
            foreach (GameObject cell in cells)
            {
                Vector3 dist = gameObject.transform.position - cell.transform.position;
                if (dist.sqrMagnitude < sqrMag)
                {
                    sqrMag = dist.sqrMagnitude;
                    target = cell;
                }
            }
        }
        
        pIndex = 0;
        path.Add(target);
    }
}
