using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

    //supplies
    float food;
    int maxFood;
    int wax;
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
    int maxForagers;
    int builders;
    int maxBuilders;
    int guards;
    int maxGuards;
    int nurses;
    int maxNurses;

    //Hive size variables
    public float left;
    public float right;
    public float top;
    public float bottom;
    Vector2 centre;

    //modifiers
    float forageSpeed;
    int buildHunger;
    float guardHunger;
    float nurseHunger;
    int busyBuilders;

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
    public int eSpawnInterval;
    int eSCounter;

    //Cell Lists
    List<GameObject> foragerCells = new List<GameObject>();
    List<GameObject> storageCells = new List<GameObject>();
    List<GameObject> builderCells = new List<GameObject>();
    List<GameObject> guardCells = new List<GameObject>();
    List<GameObject> nurseCells = new List<GameObject>();

    List<GameObject> fRepairList = new List<GameObject>();
    List<GameObject> sRepairList = new List<GameObject>();
    List<GameObject> bRepairList = new List<GameObject>();
    List<GameObject> gRepairList = new List<GameObject>();
    List<GameObject> nRepairList = new List<GameObject>();

    public int numCells;
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
        eSCounter = 0;

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
                int broken = 0;
                for (int i = 0; i < fRepairList.Count; i++)
                {
                    if (fRepairList[i].GetComponent<CellManager>().getBroken()) broken++;
                }
                maxForagers = counter * fNestSize;
                foragers = maxForagers - (broken * fNestSize);

                counter = storageCells.Count;
                maxFood = (counter * foodStorage) + queenStorage;
                maxWax = (counter * waxStorage) + queenStorage;

                counter = builderCells.Count;
                broken = 0;
                for (int i = 0; i < bRepairList.Count; i++)
                {
                    if (bRepairList[i].GetComponent<CellManager>().getBroken()) broken++;
                }
                maxBuilders = counter * bNestSize;
                builders = maxBuilders - (broken * bNestSize) - busyBuilders;

                counter = guardCells.Count;
                maxGuards = counter * gNestSize;

                counter = nurseCells.Count;
                broken = 0;
                for (int i = 0; i < nRepairList.Count; i++)
                {
                    if (nRepairList[i].GetComponent<CellManager>().getBroken()) broken++;
                }
                maxNurses = counter * nNestSize;
                nurses = maxNurses - (broken * nNestSize / 2);
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
                    guardList[guardList.Count - 1].GetComponent<GuardController>().setSM(gameObject);
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

            eSCounter++;
            //ENEMY SPAWNING
            int jeff = (numCells-1) / 7;
            Debug.Log(numCells);
            if (eSCounter >= 5 && jeff >= 1)
            {
                eSCounter = 0;
                for (int i = 0; i < jeff; i++)
                {
                    newEnemy(numCells);
                }
            }

            //ENEMY TARGETING
            enemyTargeting();

            //NURSE REPAIR CHECK
            nurseCheck();
        }



        foodLabel.text = "Food: " + (int)food + "/" + maxFood;
        foragerLabel.text = "Foragers: " + foragers;
        builderLabel.text = "Builders:  " + builders + "/" + maxBuilders;
        guardLabel.text = "Guards: " + guards + "/" + maxGuards;
        nurseLabel.text = "Nurses: " + nurses;
        waxLabel.text = "Wax:  " + wax + "/" + maxWax;
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
        //maxFood += foodStorage;
        //maxWax += waxStorage;
    }

    public void newForager(GameObject cell)
    {
        foragerCells.Add(cell);
        cellsCounted = false;
    }

    public void newBuilder(GameObject cell)
    {
        builderCells.Add(cell);
        cellsCounted = false;
    }

    public void newGuard(GameObject cell)
    {
        guardCells.Add(cell);
        cellsCounted = false;
    }

    public void newNurse(GameObject cell)
    {
        nurseCells.Add(cell);
        cellsCounted = false;
    }

    public void newEmpty()
    {
        numCells++;
    }
    //END OF NEW CELLS

    //CELL LIST CLEANUP
    public void deadCell(GameObject cell)
    {
        switch (cell.GetComponent<CellManager>().getCellType())
        {
            case 0:
                for (int i = storageCells.Count; i > 0; i--)
                {
                    if (storageCells[i - 1] == cell)
                    {
                        storageCells.RemoveAt(i - 1);
                        numCells--;
                        break;
                    }
                }
                break;
            case 1:
                for (int i = foragerCells.Count; i > 0; i--)
                {
                    if (foragerCells[i - 1] == cell)
                    {
                        foragerCells.RemoveAt(i - 1);
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

    //REPAIR LIST CLEANUP
    public void cellFixed(GameObject cell, bool remove)
    {
        switch (cell.GetComponent<CellManager>().getCellType())
        {
            case 0:
                if (!remove)
                { sRepairList.Add(cell); }
                else
                    for (int i = sRepairList.Count; i > 0; i--)
                    {
                        if (sRepairList[i - 1] == cell)
                        {
                            sRepairList.RemoveAt(i - 1);
                            break;
                        }
                    }
                break;
            case 1:
                if (!remove)
                { fRepairList.Add(cell); }
                else
                    for (int i = fRepairList.Count; i > 0; i--)
                    {
                        if (fRepairList[i - 1] == cell)
                        {
                            Debug.Log("SNARF!");
                            fRepairList.RemoveAt(i - 1);
                            break;
                        }
                    }
                break;
            case 2:
                if (!remove)
                { bRepairList.Add(cell); }
                else
                    for (int i = bRepairList.Count; i > 0; i--)
                    {
                        if (bRepairList[i - 1] == cell)
                        {
                            bRepairList.RemoveAt(i - 1);
                            break;
                        }
                    }
                break;
            case 3:
                if (!remove)
                { gRepairList.Add(cell); }
                else
                    for (int i = gRepairList.Count; i > 0; i--)
                    {
                        if (gRepairList[i - 1] == cell)
                        {
                            gRepairList.RemoveAt(i - 1);
                            break;
                        }
                    }
                break;
            case 4:
                if (!remove)
                { nRepairList.Add(cell); }
                else
                    for (int i = nRepairList.Count; i > 0; i--)
                    {
                        if (nRepairList[i - 1] == cell)
                        {
                            nRepairList.RemoveAt(i - 1);
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
        foreach (GameObject enemy in enemyList)
        {
            enemy.GetComponent<EnemyManager>().setDilation(period);
        }
        foreach (GameObject bumble in guardList)
        {
            bumble.GetComponent<GuardController>().setDilation(period);
        }
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

    //ENEMY SPAWNING
    public void newEnemy(int strength)
    {
        int temp = Random.Range(0, 2);
        Vector3 spawnPos = new Vector3(centre.x, bottom, 0);

        float width = Mathf.Abs(right - left);

        if (width < 10) width = 12;
        if (temp == 0)
        {
            spawnPos.x += width / 1.5f;
        }
        else spawnPos.x -= width / 1.5f;

        //ADD ENEMY TYPE SELECTION, AND Y POS EXCEPTION FOR GROUND-BOUND ENEMIES
        float height = Mathf.Abs(top - bottom);
        spawnPos.y = bottom + Random.Range(height / 4, height * 2.0f);

        GameObject newE = (GameObject)Instantiate(enemyTypes[0], spawnPos, Quaternion.identity);
        newE.GetComponent<EnemyManager>().setDilation(period);
        enemyList.Add(newE);
        
    }

    //ENEMY TARGETING
    public void enemyTargeting()
    {
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

    public void nurseCheck()
    {
        int i = 0;
        int repairers = builders + nurses;
        bool norb = false; //false == nurses avaiable, true = builders used;
        busyBuilders = 0;

        foreach (GameObject gObj in fRepairList)
        {
            if (!norb && i >= nurses) norb = true;
            if (norb && i >= repairers) break;
            fixCell(gObj, norb);
            i++;
        }
        foreach (GameObject gObj in sRepairList)
        {
            if (!norb && i >= nurses) norb = true;
            if (norb && i >= repairers) break;
            fixCell(gObj, norb);
            i++;
        }
        foreach (GameObject gObj in bRepairList)
        {
            if (!norb && i >= nurses) norb = true;
            if (norb && i >= repairers) break;
            fixCell(gObj, norb);
            i++;
        }
        foreach (GameObject gObj in gRepairList)
        {
            if (!norb && i >= nurses) norb = true;
            if (norb && i >= repairers) break;
            fixCell(gObj, norb);
            i++;
        }
        foreach (GameObject gObj in nRepairList)
        {
            if (!norb && i >= nurses) norb = true;
            if (norb && i >= repairers) break;
            fixCell(gObj, norb);
            i++;
        }
    }

    void fixCell(GameObject cell, bool nOrB)
    {
        float MHP = cell.GetComponent<CellManager>().getMaxHealth();

        if (cell.GetComponent<CellManager>().getHealth() >= MHP) return;

        int repair = Mathf.CeilToInt(MHP / 100);

        if (repair > wax) return;
        else
        {
            wax -= repair;
            cell.GetComponent<CellManager>().deltaHealth(repair);
            if (nOrB) busyBuilders++;
        }
    }

    public void setHiveSize(float l, float r, float t, float b)
    {
        left = l; right = r; top = t; bottom = b;
        centre.x = (left + right) / 2; centre.y = (top + bottom) / 2;
    }

    public float getLeft()
    { return left; }

    public float getRight()
    { return right; }

    public float getTop()
    { return top; }

    public float getBot()
    { return bottom; }

}
