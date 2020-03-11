using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assessment5Runner : MonoBehaviour
{
    //  All variables needed to create a Decision Tree.
    public EnemyNearby enemCheck;
    public basicNavScript navAgent;
    IDecision curDec;
    public GameObject enemy;
    public GameObject[] targets;
    public int curTar = 0;
    public int foodCount = 0;
    public int foodCap = 500;

    //  Start void to create the decision tree.
    void Start()
    {
        enemCheck = new EnemyNearby(this,
                            new CheckNearSaferoom(this,
                                new runToSafeRoomScript(this),
                                new MoveTowardsArea(this)),
                            new FullOfFood(this,
                                new CheckNearSilo(this, 
                                    new FoodDepositScript(this),
                                    new MoveTowardsArea(this)),
                                new CheckNearFarm(this,
                                    new FoodCollectScript(this),
                                    new MoveTowardsArea(this))));
    }

    //  Every frame makes a decision on what to do;
    private void Update()
    {
        curDec = enemCheck;
        while (curDec != null)
        {
            curDec = curDec.MakeDecision();
        }
    }
}


//  Decision that determines if the Enemy is close by.
public class EnemyNearby : IDecision
{
    Assessment5Runner runner;
    IDecision yes;
    IDecision no;

    public EnemyNearby()
    {
        runner = null;
    }

    public EnemyNearby(Assessment5Runner runner, IDecision yes, IDecision no)
    {
        this.runner = runner;
        this.yes = yes;
        this.no = no;
    }

    public IDecision MakeDecision()
    {
        float distance = Vector3.Distance(runner.transform.position, runner.enemy.transform.position);
        return (distance < 5f ? yes : no);
    }
}

//  Decision that checks how much food the player is holding.
public class FullOfFood : IDecision
{
    Assessment5Runner runner;
    IDecision yes;
    IDecision no;

    public FullOfFood()
    {
        runner = null;
    }

    public FullOfFood(Assessment5Runner runner, IDecision yes, IDecision no)
    {
        this.runner = runner;
        this.yes = yes;
        this.no = no;
    }

    public IDecision MakeDecision()
    {
        if (runner.foodCount >= runner.foodCap)
        {
            runner.curTar = 1;
            return yes;
        }

        runner.curTar = 2;
        return no;
    }
}

//  Makes a decision based on how far you are from the Saferoom.
public class CheckNearSaferoom : IDecision
{
    Assessment5Runner runner;
    IDecision yes;
    IDecision no;

    public CheckNearSaferoom()
    {
        runner = null;
        yes = null;
        no = null;
    }

    public CheckNearSaferoom(Assessment5Runner runner, IDecision yes, IDecision no)
    {
        this.runner = runner;
        this.yes = yes;
        this.no = no;
    }

    public IDecision MakeDecision()
    {
        runner.curTar = 0;
        float distance = Vector3.Distance(runner.transform.position, runner.targets[0].transform.position);
        return (distance < 2f ? yes : no);
    }
}

//  Makes a decision based on how far you are from the Silo.
public class CheckNearSilo : IDecision
{
    Assessment5Runner runner;
    IDecision yes;
    IDecision no;

    public CheckNearSilo()
    {
        runner = null;
        yes = null;
        no = null;
    }

    public CheckNearSilo(Assessment5Runner runner, IDecision yes, IDecision no)
    {
        this.runner = runner;
        this.yes = yes;
        this.no = no;
    }

    public IDecision MakeDecision()
    {
        float distance = Vector3.Distance(runner.transform.position, runner.targets[1].transform.position);
        return (distance < 2f ? yes : no);
    }
}

//  Makes a decision based on how far you are from the Farm.
public class CheckNearFarm : IDecision
{
    Assessment5Runner runner;
    IDecision yes;
    IDecision no;

    public CheckNearFarm()
    {
        runner = null;
        yes = null;
        no = null;
    }

    public CheckNearFarm(Assessment5Runner runner, IDecision yes, IDecision no)
    {
        this.runner = runner;
        this.yes = yes;
        this.no = no;
    }

    public IDecision MakeDecision()
    {
        float distance = Vector3.Distance(runner.transform.position, runner.targets[2].transform.position);
        return (distance < 2f ? yes : no);
    }
}

//  Sets the NavScript target to this scripts current target and moves toward it.
public class MoveTowardsArea : IDecision
{
    Assessment5Runner runner;

    public MoveTowardsArea()
    {
        runner = null;
    }

    public MoveTowardsArea(Assessment5Runner runner)
    {
        this.runner = runner;
    }

    public IDecision MakeDecision()
    {
        runner.navAgent.target = runner.targets[runner.curTar].transform;
        runner.navAgent.MoveToTarget();
        return null;
    }
}

//  Collects food while at the Farm.
public class FoodCollectScript : IDecision
{
    Assessment5Runner runner;

    public FoodCollectScript()
    {
        runner = null;
    }

    public FoodCollectScript(Assessment5Runner runner)
    {
        this.runner = runner;
    }

    public IDecision MakeDecision()
    {
        runner.foodCount++;
        return null;
    }
}

//  Deposits food while close to the silo.
public class FoodDepositScript : IDecision
{
    Assessment5Runner runner;

    public FoodDepositScript()
    {
        runner = null;
    }

    public FoodDepositScript(Assessment5Runner runner)
    {
        this.runner = runner;
    }

    public IDecision MakeDecision()
    {
        runner.foodCount = 0;
        return null;
    }
}

//  When they are close to the safehouse, teleport them to the trapdoor.
public class runToSafeRoomScript : IDecision
{
    Assessment5Runner runner;

    public runToSafeRoomScript()
    {
        runner = null;
    }

    public runToSafeRoomScript(Assessment5Runner runner)
    {
        this.runner = runner;
    }

    public IDecision MakeDecision()
    {
        runner.gameObject.transform.position = new Vector3(-10, 2, -10);
        return null;
    }
}
