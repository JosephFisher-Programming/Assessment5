using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineTreeRunner : MonoBehaviour
{
    public FullOfGold goldCheck;
    IDecision curDec;
    public GameObject[] targets;
    public int curTar = 0;
    public int goldCount = 0;
    public int goldCap = 500;
    int goldSave = 500;

    void Start()
    {
        goldCheck = new FullOfGold(this,
            new CheckMinerLocation(this,
                new GoldDepositScript(this),
                new MoveTowardsLocation(this)),
            new CheckMinerLocation(this,
                new GoldCollectScript(this),
                new MoveTowardsLocation(this)
                ));
    }

    private void Update()
    {
        curDec = goldCheck;
        while (curDec != null)
        {
            curDec = curDec.MakeDecision();
        }
    }
}

    public class FullOfGold : IDecision
    {
        mineTreeRunner miner;
        IDecision yes;
        IDecision no;

        public FullOfGold()
        {
            miner = null;
        }

        public FullOfGold(mineTreeRunner miner, IDecision yes, IDecision no)
        {
            this.miner = miner;
            this.yes = yes;
            this.no = no;
        }

        public IDecision MakeDecision()
        {
            if (miner.goldCount >= miner.goldCap)
            {
                miner.curTar = 0;
                return yes;
            }

            miner.curTar = 1;
            return no;
        }
    }

    public class CheckMinerLocation : IDecision
    {
        mineTreeRunner miner;
        IDecision yes;
        IDecision no;

        public CheckMinerLocation()
        {
            miner = null;
            yes = null;
            no = null;
        }

        public CheckMinerLocation(mineTreeRunner miner, IDecision yes, IDecision no)
        {
            this.miner = miner;
            this.yes = yes;
            this.no = no;
        }

        public IDecision MakeDecision()
        {
            float distance = Vector3.Distance(miner.transform.position, miner.targets[miner.curTar].transform.position);
            return (distance < .5f ? yes : no);
        }
    }


    public class MoveTowardsLocation : IDecision
    {
        mineTreeRunner miner;

        public MoveTowardsLocation()
        {
            miner = null;
        }

        public MoveTowardsLocation(mineTreeRunner miner)
        {
            this.miner = miner;
        }

        public IDecision MakeDecision()
        {
            miner.transform.position += (miner.targets[miner.curTar].transform.position - miner.transform.position) * .05f;
            return null;
        }
    }

    public class GoldCollectScript : IDecision
    {
        mineTreeRunner miner;

        public GoldCollectScript()
        {
            miner = null;
        }

        public GoldCollectScript(mineTreeRunner miner)
        {
            this.miner = miner;
        }

        public IDecision MakeDecision()
        {
            miner.goldCount++;
            return null;
        }
    }

    public class GoldDepositScript : IDecision
    {
        mineTreeRunner miner;

        public GoldDepositScript()
        {
            miner = null;
        }

        public GoldDepositScript(mineTreeRunner miner)
        {
            this.miner = miner;
        }

        public IDecision MakeDecision()
        {
            miner.goldCount = 0;
            return null;
        }
    }
