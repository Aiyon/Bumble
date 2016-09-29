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
    int queenStorage;
    int fNestSize;
    int bNestSize;
    int gNestSize;
    int nNestSize;

    int sCounter;

    //bees
    int foragers;
    int builders;
    int maxGuards;
    int guards;
    int nurses;

    //modifiers
    float forageSpeed;
    int buildHunger;
    float guardHunger;
    float nurseHunger;

    float lastUpdate;

    public Text foodLabel;
    public Text waxLabel;
    public Text foragerLabel;
    public Text builderLabel;
    public Text guardLabel;
    public Text nurseLabel;

    public List<GameObject> guardList;
    public List<GameObject> enemyList;
    public GameObject guard;
    public GameObject[] enemyTypes;

    //Cell Lists
    List<GameObject> foragerCells = new List<GameObject>();
    List<GameObject> storageCells = new List<GameObject>();
    List<GameObject> builderCells = new List<GameObject>();
    List<GameObject> guardCells = new List<GameObject>();
    List<GameObject> nurseCells = new List<GameObject>();
    List<GameObject> repairList = new List<GameObject>();
    int numCells;
    bool cellsCounted = false;

    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;

    // Use this for initialization
    void Start()
    {
        fNestSize = 1;
        bNestSize = 1;
        gNestSize = 2;
        nNestSize = 2;

        food = 500;
        wax = 0;
        foragers = 0;
        builders = 0;
        forageSpeed = 2;
        buildHunger = 1;
        guardHunger = 0.5f;
        nurseHunger = 1;
        queenStorage = 500;
        maxWax = maxFood = queenStorage;
        lastUpdate = Time.time;
        guardCells = new List<GameObject>();
        numCells = 7;

        sCounter = 0;   //how many seconds has the game been going for.
    }

    //UPDATE
    void Update()
    {

        //Update every x seconds.
        if (Time.time > nextActionTime)
        {

            //CELL COUNTERS:
            if (!cellsCounted)
            {
                int counter = foragerCells.Count;
                foragers = counter * fNestSize; // - # of broken cells.

                counter = storageCells.Count;
                maxFood = (counter * foodStorage) + queenStorage;
                maxWax = (counter * waxStorage) + queenStorage;

                counter = builderCells.Count;
                builders = counter * bNestSize;

                counter = guardCells.Count;
                maxGuards = counter * gNestSize;

                counter = nurseCells.Count;
                nurses = counter * nNestSize;
            }
            //END OF CELL COUNTERS

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
            int drain = (builders * buildHunger) + Mathf.CeilToInt(guards * guardHunger) + Mathf.CeilToInt(nurseCells.Count * nurseHunger);
            food -= drain;

            //wax production
            if (wax < maxWax && food > 200)
            {
                wax += builders;
                wax = Mathf.Clamp(wax, 0, maxWax);
            }

            //food production
            if (food < maxFood) food += foragers * forageSpeed;
            food = Mathf.Clamp(food, -500, maxFood);

            //If food < 0: for killing bees, take a random number between 0 and the total number of bees that use food. Then, for each type of bee, check if the number is more 
            //than the number of bees in that type. If so, take the number of that type of bee off the search number before checking the next type.


            //ENEMY TARGETING
            if (enemyList.Count > 0)
            {
                for (int i = enemyList.Count - 1; i >= 0; i--)
                {
                    if (enemyList[i].GetComponent<EnemyManager>().getHealth() <= 0)
                    {
                        GameObject temp = enemyList[i];
                        enemyList.RemoveAt(i);
                        Destroy(temp);
                    }
                }
                for (int i = guardList.Count - 1; i >= 0; i--)
                {
                    if (guardList[i].GetComponent<GuardController>().stinger())
                    {
                        GameObject temp = guardList[i];
                        guardList.RemoveAt(i);
                        Destroy(temp);
                        guards--;
                    }
                }
                getEnemy();
            }
        }

        

        foodLabel.text = "Food: " + (int)food + "/" + maxFood;
        foragerLabel.text = "Foragers: " + foragers;
        builderLabel.text = "Builders:  " + builders;
        guardLabel.text = "Guards: " + guards + "/" + maxGuards;
        nurseLabel.text = "Nurses: " + nurses;
        waxLabel.text = "Wax:  " + (int)wax + "/" + maxWax;
    }
    //END UPDATE

    public int getFood()
    { return (int)food; }

    public void setFood(int i)
    { food += i; }

    public int getWax()
    { return (int)wax; }

    public void setWax(int i)
    { wax += i; }

    //NEW CELLS
    public void newStorage(GameObject cell)
    {
        storageCells.Add(cell);
        numCells++;
        //maxFood += foodStorage;
        //maxWax += waxStorage;
    }

    public void newForager(GameObject cell)
    {
        foragerCells.Add(cell);
        numCells++;
        cellsCounted = false;
    }

    public void newBuilder(GameObject cell)
    {
        builderCells.Add(cell);
        numCells++;
        cellsCounted = false;
    }

    public void newGuard(GameObject cell)
    {
        guardCells.Add(cell);
        numCells++;
        cellsCounted = false;
    }

    public void newNurse(GameObject cell)
    {
        nurseCells.Add(cell);
        numCells++;
        cellsCounted = false;
    }
    //END OF NEW CELLS

    public void deadCell(GameObject cell)
    {
        switch (cell.GetComponent<CellManager>().getCellType())
        {
            case 0:
                for(int i = storageCells.Count; i > 0; i--)
                {
                    if (storageCells[i-1] == cell)
                    {
                        storageCells.RemoveAt(i-1);
                        numCells--;
                        break;
                    }
                }
                break;
            case 1:
                for (int i = foragerCells.Count; i > 0; i--)
                {
                    if (foragerCells[i-1] == cell)
                    {
                        foragerCells.RemoveAt(i - 1);
                        numCells--;
                        break;
                    }
                }
                break;
            case 2:
                for (int i = builderCells.Count; i > 0; i--)
                {
                    if (builderCells[i - 1] == cell)
                    {
                        builderCells.RemoveAt(i - 1);
                        numCells--;
                        break;
                    }
                }
                break;
            case 3:
                for (int i = guardCells.Count; i > 0; i--)
                {
                    if (guardCells[i - 1] == cell)
                    {
                        guardCells.RemoveAt(i - 1);
                        numCells--;
                        break;
                    }
                }
                break;
            case 4:
                for (int i = nurseCells.Count; i > 0; i--)
                {
                    if (nurseCells[i - 1] == cell)
                    {
                        nurseCells.RemoveAt(i - 1);
                        numCells--;
                        break;
                    }
                }
                break;
        }
        cellsCounted = false;
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
