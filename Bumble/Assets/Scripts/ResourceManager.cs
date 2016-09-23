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
    public int foodStorage;
    public int waxStorage;
    int fNestSize;
    int bNestSize;
    int gNestSize;
    int nNestSize;

    List<GameObject> repairList;

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
    List<GameObject> guardCells;
    public List<GameObject> enemyList;
    public GameObject guard;
    public GameObject[] enemyTypes;

    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;

    // Use this for initialization
    void Start()
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
        guardCells = new List<GameObject>();

        sCounter = 0;   //how many seconds has the game been going for.
    }

    // Update is called once per frame
    void Update()
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
                for (int i = 0; i < delta; i++)
                {
                    Vector3 beeSpot = guardCells[i].transform.position;
                    beeSpot.z -= 0.1f;
                    guardList.Add((GameObject)Instantiate(guard, beeSpot, Quaternion.identity));
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


            //ENEMY TARGETING
            for (int i = enemyList.Count-1; i >= 0; i--)
            {
                Debug.Log(enemyList[i].GetComponent<EnemyManager>().getHealth());
                if(enemyList[i].GetComponent<EnemyManager>().getHealth() <= 0)
                {
                    GameObject temp = enemyList[i];
                    enemyList.RemoveAt(i);
                    Destroy(temp);
                }
            }
            for (int i = guardList.Count - 1; i >= 0; i--)
            {
                if(guardList[i].GetComponent<GuardController>().stinger())
                {
                    GameObject temp = guardList[i];
                    guardList.RemoveAt(i);
                    Destroy(temp);
                    guards--;
                }
            }
            getEnemy();
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
        maxFood += foodStorage;
        maxWax += waxStorage;
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
        guardCells.Add(cell);
    }

    public void timeDilation(int multiplier)
    {
        period = 1.0f / multiplier;
        Debug.Log(period);
    }

    //guard brains
    public void getEnemy()
    {
        int j = 0;
        int counter = 0;
        for (int i = 0; i < guardList.Count; i++)
        {
            guardList[counter].GetComponent<GuardController>().setTarget(enemyList[j]);
            counter++;
            if (counter > enemyList[j].GetComponent<EnemyManager>().getHealth())
            {
                j++;
                if (j == enemyList.Count) i = guardList.Count;
            }
        }
    }

    public void newEnemy(int i)
    {
        enemyList.Add((GameObject)Instantiate(enemyTypes[i], Vector3.zero, Quaternion.identity));
    }

}
