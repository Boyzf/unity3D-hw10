using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGame;

public class GenGameObject : MonoBehaviour {
    bool isAuto = false;
    int targetDir = 1;
    public float speed = 8.0f;//船只移动速度
    public float objectSpeed = 5.0f;//牧师与魔鬼上下船的速度
    List<GameObject> priest_left = new List<GameObject>();//左岸牧师
    List<GameObject> priest_right = new List<GameObject>();
    List<GameObject> devil_left = new List<GameObject>();//左岸魔鬼
    List<GameObject> devil_right = new List<GameObject>();
    
    //存储河岸和船的位置
    Vector3 shore_left = new Vector3(-7, 0, 0);
    Vector3 shore_right = new Vector3(2, 0, 0);
    Vector3 boat_left = new Vector3(-4.9f, 0, 0);
    Vector3 boat_right = new Vector3(-0.1f, 0, 0);

    //存储牧师与魔鬼的位置
    Vector3 priLeftPos = new Vector3(-8.2f, 0.9f, 0);
    Vector3 priRightPos = new Vector3(2.2f, 0.9f, 0);
    Vector3 devilLeftPos = new Vector3(-7, 0.9f, 0);
    Vector3 devilRightPos = new Vector3(1, 0.9f, 0);
    GameObject[] Boat = new GameObject[2];//船上的乘客
    GameObject boat;//船
    int boatPos = 1; 
    Director dir = Director.getInstance();//导演
    ActionManager actionManager = ActionManager.getInstance();
    
    void Awake()
    {
        dir = Director.getInstance();
        dir.setGenGameObject(this);
        load();//加载游戏场景
    }
    void Update()
    {
        check();
        autoGetOff();
        if (autoMove())
        {
            moveBoat();
        }
    }

    void load()
    {
        Instantiate(Resources.Load("Prefabs/Shore"), shore_left, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/Shore"), shore_right, Quaternion.identity);
        boat = Instantiate(Resources.Load("Prefabs/Boat"), boat_left, Quaternion.identity) as GameObject;
        for (int i = 0;i < 3; i++)
        {
            GameObject p = Instantiate(Resources.Load("Prefabs/Priest")) as GameObject;
            GameObject d = Instantiate(Resources.Load("Prefabs/Devil")) as GameObject;
            p.transform.position = getPosition(priLeftPos, i);
            d.transform.position = getPosition(devilLeftPos, i);
            priest_left.Add(p);
            devil_left.Add(d);
        }
        Instantiate(Resources.Load("Prefabs/Light"));
    }

   //根据两岸牧师/魔鬼的数目以及船的位置来选择方案
   public void getNextBoatAction() 
    {
        isAuto = true;
        if (priest_right.Count == 3 && devil_right.Count == 3)
        {
            check();
        }
        else if (boatPos == 1 && priest_left.Count == 3 && devil_left.Count == 3)
        {
            float num = Random.Range(0, 1f);//若有多种可能，则直接random随机选取一个
            if (num < 0.5f)
            {
                devilOn();
                devilOn();
            }
            else
            {
                priestOn();
                devilOn();
            }
        }
        else if (boatPos == 2 && priest_left.Count == 2 && devil_left.Count == 2)
        {
            priestOn();
        }
        else if (boatPos == 2 && priest_left.Count == 3 && (devil_left.Count == 2 || devil_left.Count == 1 || devil_left.Count == 0))
        {
            devilOn();
        }
        else if (boatPos == 1 && priest_left.Count == 3 && devil_left.Count == 2)
        {
            devilOn();
            devilOn();
        }
        else if (boatPos == 1 && priest_left.Count == 3 && devil_left.Count == 1)
        {
            priestOn();
            priestOn();
        }
        else if (boatPos == 2 && priest_left.Count == 1 && devil_left.Count == 1)
        {
            priestOn();
            devilOn();
        }
        else if (boatPos == 1 && priest_left.Count == 2 && devil_left.Count == 2)
        {
            priestOn();
            priestOn();
        }
        else if (boatPos == 2 && priest_left.Count == 0 && devil_left.Count == 2)
        {
            devilOn();
        }
        else if (boatPos == 1 && priest_left.Count == 0 && devil_left.Count == 3)
        {
            devilOn();
            devilOn();
        }
        else if (boatPos == 2 && priest_left.Count == 0 && devil_left.Count == 1)
        {
            float num = Random.Range(0, 1f);
            if (num < 0.5f)
            {
                priestOn();
            }
            else
            {
                devilOn();
            }
        }
        else if (boatPos == 1 && priest_left.Count == 2 && devil_left.Count == 1)
        {
            priestOn();
        }
        else if (boatPos == 1 && priest_left.Count == 0 && devil_left.Count == 2)
        {
            devilOn();
            devilOn();
        }
        else if (boatPos == 1 && priest_left.Count == 1 && devil_left.Count == 1)
        {
            priestOn();
            devilOn();
        }
        targetDir = boatPos;
    }

    public bool autoMove()//判断何时开始移动小船
    {
        if (isAuto)//开启自动
        {
            if (boatPos == 1)
            {

                if (Boat[0] != null && Boat[1] != null)
                {
                    if (Boat[0].transform.position.x == -5.2f && Boat[1].transform.position.x == -4.6f)
                        return true;
                }
                else if (Boat[0] != null && Boat[1] == null)
                {
                    if (Boat[0].transform.position.x == -5.2f)
                        return true;
                }
                else if (Boat[1] != null && Boat[0] == null)
                {
                    if (Boat[1].transform.position.x == -4.6f)
                        return true;
                }
                return false;
            }
            else if (boatPos == 2)
            {
                if (Boat[0] != null && Boat[1] != null)
                {

                    if (Boat[0].transform.position.x == -0.3f && Boat[1].transform.position.x == 0.2)
                        return true;

                }
                else if (Boat[0] != null && Boat[1] == null)
                {
                    if (Boat[0].transform.position.x == -0.3f)
                        return true;
                }
                else if (Boat[1] != null && Boat[0] == null)
                {
                    if (Boat[1].transform.position.x == 0.2f)
                        return true;
                }
                return false;
            }
            return false;
        }
        return false;

    }
    public void autoGetOff()//判断合适自动下船
    {
        if (isAuto)//开启自动
        {
            if (targetDir == 1)
            {
                if (boat.transform.position == boat_right)//船到达右方
                {
                    getOffBoat();
                    getOffBoat();
                    isAuto = false;
                }
            }
            else
            {
                if (boat.transform.position == boat_left)//船抵达左方
                {
                    getOffBoat();
                    getOffBoat();
                    isAuto = false;
                }
            }
        }
    }

    public Vector3 getPosition(Vector3 pos, int i)
    {
        float distance = 0.4f;
        return new Vector3(pos.x+distance*i, pos.y, pos.z);
    }
    public bool isBoatEmpty()//判断船只是否为空
    {
        for(int i = 0;i < 2; i++)
        {
            if (Boat[i] != null)
            {
                return false;
            }
        }
        return true;
    }
    public bool isBoatFull()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Boat[i] == null)
            {
                return false;
            }
        }
        return true;
    }
    public void moveBoat()//移动船只
    {
        if (!isBoatEmpty())
        {
            if (boatPos == 1)
            {
                actionManager.ApplyMoveToAction(boat, boat_right, speed);
                boatPos = 2;
            }
            else if (boatPos == 2)
            {
                actionManager.ApplyMoveToAction(boat, boat_left, speed);
                boatPos = 1;
            }
        }
    }

    public void priestOn()//牧师上船
    {
        if (!isBoatFull())
        {
            Vector3 target;
            if (boatPos == 1 && priest_left.Count != 0)//左岸
            {
                GameObject p = priest_left[priest_left.Count - 1];
                priest_left.RemoveAt(priest_left.Count - 1);
                p.transform.parent = boat.transform;//跟随船移动
                
                if (Boat[0] == null)
                {
                    Boat[0] = p;
                    target = new Vector3(-5.2f, 0.3f, 0);
                }
                else
                {
                    Boat[1] = p;
                    target = new Vector3(-4.6f, 0.3f, 0);
                }
                actionManager.ApplyMoveToYZAction(p, target, objectSpeed);
            }
            else if(boatPos == 2 && priest_right.Count != 0)//右岸
            {
                GameObject p = priest_right[priest_right.Count - 1];
                priest_right.RemoveAt(priest_right.Count - 1);
                p.transform.parent = boat.transform;
                if (Boat[0] == null)
                {
                    Boat[0] = p;
                    target = new Vector3(-0.3f, 0.3f, 0);
                }
                else
                {
                    Boat[1] = p;
                    target = new Vector3(0.2f, 0.3f, 0);
                }
                actionManager.ApplyMoveToYZAction(p, target, objectSpeed);
            }
        }
    }
    public void devilOn()//魔鬼上船
    {
        if (!isBoatFull())
        {
            Vector3 target;
            if (boatPos == 1 && devil_left.Count != 0)
            {
                GameObject p = devil_left[devil_left.Count - 1];
                devil_left.RemoveAt(devil_left.Count - 1);
                p.transform.parent = boat.transform;
                if (Boat[0] == null)
                {
                    Boat[0] = p;
                    target = new Vector3(-5.2f, 0.3f, 0);
                }
                else
                {
                    Boat[1] = p;
                    target = new Vector3(-4.6f, 0.3f, 0);
                }
                actionManager.ApplyMoveToYZAction(p, target, objectSpeed);
            }
            else if(boatPos == 2 && devil_right.Count != 0)
            {
                GameObject p = devil_right[devil_right.Count - 1];
                devil_right.RemoveAt(devil_right.Count - 1);
                p.transform.parent = boat.transform;
                if (Boat[0] == null)
                {
                    Boat[0] = p;
                    target = new Vector3(-0.3f, 0.3f, 0);
                }
                else
                {
                    Boat[1] = p;
                    target = new Vector3(0.2f, 0.3f, 0);
                }
                actionManager.ApplyMoveToYZAction(p, target, objectSpeed);
            }
        }
    }
    public void getOffBoat()
    {
        for(int i = 0;i < 2; i++)
        {
            if (Boat[i] != null)
            {
                Vector3 target = new Vector3();
                Boat[i].transform.parent = null;
                if (boatPos == 1)
                {
                    if (Boat[i].tag == "Priest")
                    {
                        priest_left.Add(Boat[i]);
                        target = getPosition(priLeftPos, priest_left.Count - 1);
                    }
                    else if (Boat[i].tag == "Devil")
                    {
                        devil_left.Add(Boat[i]);
                        target = getPosition(devilLeftPos, devil_left.Count - 1);
                    }
                        
                }
                else if (boatPos == 2)
                {
                    if (Boat[i].tag == "Priest")
                    {
                        priest_right.Add(Boat[i]);
                        target = getPosition(priRightPos, priest_right.Count - 1);
                    }
                        
                    else if (Boat[i].tag == "Devil")
                    {
                        devil_right.Add(Boat[i]);
                        target = getPosition(devilRightPos, devil_right.Count - 1);
                    }   
                }
                actionManager.ApplyMoveToYZAction(Boat[i], target, objectSpeed);
                Boat[i] = null;
                break;
            }
        }
    }
    public void check()
    {
        if (priest_right.Count == 3 && devil_right.Count == 3)
        {
            dir.setMessage("WIN");
            return;
        }
        int priestBoat = 0, devilBoat = 0, priest_Left = 0, priest_Right = 0, devil_Left = 0, devil_Right = 0;
        for(int i = 0;i < 2; i++)
        {
            if (Boat[i] != null)
            {
                if(Boat[i].tag == "Priest")
                {
                    priestBoat++;
                }
                else if(Boat[i].tag == "Devil")
                {
                    devilBoat++;
                }
            }
        }
        if(boatPos == 1)
        {
            priest_Left = priest_left.Count + priestBoat;
            devil_Left = devil_left.Count + devilBoat;
            devil_Right = devil_right.Count;
            priest_Right = priest_right.Count;
        }
        else if(boatPos == 2)
        {
            priest_Left = priest_left.Count;
            devil_Left = devil_left.Count;
            devil_Right = devil_right.Count + devilBoat;
            priest_Right = priest_right.Count + priestBoat;
        }
        if ((priest_Left < devil_Left&&priest_Left!=0) || (priest_Right < devil_Right&&priest_Right!=0))
        {
            dir.setMessage("LOSE");
        }
    }
}