using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

    //supplies
    float food;
    int maxFood;
    float wax;
    int maxWax;
    public int storageSize;
    public int nestSize;

    //bees
    int foragers;
    int builders;

    //modifiers
    float forageSpeed;
    float buildHunger;

    float lastUpdate;

    public Text foodLabel;
    public Text waxLabel;
    public Text foragerLabel;
    public Text builderLabel;

    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;

	// Use this for initialization
	void Start ()
    {
        food = 500;
        wax = 0;
        foragers = 0;
        builders = 0;
        forageSpeed = 2;
        buildHunger = 1;
        maxFood = 500;
        maxWax = 500;
        lastUpdate = Time.time;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Update every x seconds.
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;

            //Food drain
            food -= builders * buildHunger;

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
        foragers += nestSize;
    }

    public void newBuilder()
    {
        builders += nestSize;
    }

    public void timeDilation(int multiplier)
    {
        period = 1.0f / multiplier;
        Debug.Log(period);
    }

}
