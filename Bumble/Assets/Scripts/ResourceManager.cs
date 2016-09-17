using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

    //supplies
    float food;
    int maxFood;
    float wax;
    int maxWax;
    public int storageSize;
    int fNestSize;
    int bNestSize;
    int gNestSize;

    int sCounter;

    //bees
    int foragers;
    int builders;
    int maxGuards;
    int guards;

    //modifiers
    float forageSpeed;
    int buildHunger;
    float guardHunger;

    float lastUpdate;

    public Text foodLabel;
    public Text waxLabel;
    public Text foragerLabel;
    public Text builderLabel;
    public Text guardLabel;

    public List<GameObject> guardList;
    public GameObject guard;

    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;

	// Use this for initialization
	void Start ()
    {
        fNestSize = 1;
        bNestSize = 1;
        gNestSize = 2;

        food = 500;
        wax = 0;
        foragers = 0;
        builders = 0;
        forageSpeed = 2;
        buildHunger = 1;
        guardHunger = 0.5f;
        maxFood = 500;
        maxWax = 500;
        lastUpdate = Time.time;

        sCounter = 0;   //how many seconds has the game been going for.
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Update every x seconds.
        if (Time.time > nextActionTime)
        {

            if (sCounter == 600) sCounter = 0;   //reset sCounter every 10 minutes so the value doesn't get too high and risk overflow.
            //sCounter code.
            if (sCounter % 3 == 0 && guards < maxGuards)
            {
                int delta = guards;
                guards = Mathf.Clamp(guards + (maxGuards / gNestSize), 0, maxGuards);
                delta = guards - delta;
                Debug.Log(delta);
                for(int i = 0; i < delta; i++)
                {
                    Vector3 beeSpot = guardList[i].transform.position;
                    beeSpot.z -= 0.1f;
                    Instantiate(guard, beeSpot, Quaternion.identity);
                }
            }
            sCounter++;
            //end sCounter code.


            nextActionTime += period;

            //Food drain
            int drain = (builders * buildHunger) + Mathf.CeilToInt(guards * guardHunger);
            food -= drain;

            //wax production
            if (wax < maxWax && food > 200)
            {
                wax += builders;
            }

            if (food < maxFood) food += foragers * forageSpeed;   //modify to update every second, rather than via deltaTime.

            //If food < 0: for killing bees, take a random number between 0 and the total number of bees that use food. Then, for each type of bee, check if the number is more 
            //than the number of bees in that type. If so, take the number of that type of bee off the search number before checking the next type.
        }

        food = Mathf.Clamp(food, -500, maxFood);
        wax = Mathf.Clamp(wax, 0, maxWax);

        foodLabel.text = "Food: " + (int)food + "/" + maxFood;
        foragerLabel.text = "Foragers: " + foragers;
        builderLabel.text = "Builders:  " + builders;
        guardLabel.text = "Guards: " + guards;
        waxLabel.text = "Wax:  " + (int)wax + "/" + maxWax;
    }

    public int getFood()
    { return (int)food; }

    public void setFood(int i)
    { food += i; }

    public int getWax()
    { return (int)wax; }

    public void setWax(int i)
    { wax += i; }

    public void newStorage()
    {
        maxFood += storageSize;
    }

    public void newForager()
    {
        foragers += fNestSize;
    }

    public void newBuilder()
    {
        builders += bNestSize;
    }

    public void newGuard(GameObject cell)
    {
        maxGuards += gNestSize;
        guardList.Add(cell);
    }

    public void timeDilation(int multiplier)
    {
        period = 1.0f / multiplier;
        Debug.Log(period);
    }

}
