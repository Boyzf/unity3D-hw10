using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGame;

namespace Com.MyGame
{
    public interface GameStatus
    {
        bool getState();
        void setState(bool state);
        string getMessage();
        void setMessage(string message);
    }
    public interface Interfaces
    {
        void priestOn();
        void devilOn();
        void moveBoat();
        void getOffBoat();
        void autoNext();
        void reset();//重置
    }
    public interface ActionCompleted
    {
        void OnActionCompleted(Action action);
    }

    public class Director : System.Object,Interfaces, GameStatus
    {
        //初始化相关操作
        private static Director _instance;
        private BaseCode _base;
        private GenGameObject genGameobj;
        private bool state = false;
        private string message = "";//提示消息为空

        public static Director getInstance()
        {
            if (_instance == null)
            {
                _instance = new Director();
            }
            return _instance;
        }
        internal void setBaseCode(BaseCode b)
        {
            if (_base == null)
            {
                _base = b;
            }
        }
        internal void setGenGameObject(GenGameObject obj)
        {
            if (genGameobj == null)
            {
                genGameobj = obj;
            }
        }
        public void priestOn()
        {
            genGameobj.priestOn();
        }
        public void devilOn()
        {
            genGameobj.devilOn();
        }
        public void moveBoat()
        {
            genGameobj.moveBoat();
        }
        public void getOffBoat()
        {
            genGameobj.getOffBoat();
        }
        public void autoNext()
        {
            genGameobj.getNextBoatAction();
        }
        public bool getState() {
            return state;
        }
        public void setState(bool s) {
            this.state = s;
        }
        public string getMessage() {
            return message;
        }
        public void setMessage(string message) {
            this.message = message;
        }
        public void reset()
        {
            state = false;
            message = "";
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    public class Action : MonoBehaviour
    {
        protected Action() { }
        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }
        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }
   

    public class MoveToAction : Action
    {
        public Vector3 target;
        public float speed;
        private ActionCompleted monitor = null;
        
        public void getAction(Vector3 target, float speed, ActionCompleted monitor)
        {
            Director.getInstance().setState(true);
            this.target = target;
            this.speed = speed;
            this.monitor = monitor;
        }

        public override void Update()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);//移动位置
            if (transform.position == target)
            {
                Director.getInstance().setState(false);
                if (monitor != null)
                {
                    monitor.OnActionCompleted(this);
                }
                Destroy(this);
            }
        }
    }

    public class MoveToYZAction : Action, ActionCompleted
    {
        public GameObject obj;
        public Vector3 target;
        public float speed;
        ActionManager acM = ActionManager.getInstance();
        private ActionCompleted monitor = null;

        public void getAction(GameObject obj, Vector3 target, float speed, ActionCompleted monitor)
        {
            this.obj = obj;
            this.target = target;
            this.speed = speed;
            this.monitor = monitor;
            Director.getInstance().setState(true);
            this.MoveYZ();
        }
        public void MoveYZ()
        {
            if (target.y < obj.transform.position.y)
            {
                Vector3 targetZ = new Vector3(target.x, obj.transform.position.y, target.z);
                acM.ApplyMoveToAction(obj, targetZ, speed, this);
            }
            else
            {
                Vector3 targetY = new Vector3(target.x, target.y, obj.transform.position.z);
                acM.ApplyMoveToAction(obj, targetY, speed, this);
            }
        }

        public void OnActionCompleted(Action action)
        {
            acM.ApplyMoveToAction(obj, target, speed, null);
        }

        public override void Update()
        {
            if (obj.transform.position == target)
            {
                Director.getInstance().setState(false);
                if (monitor != null)
                {
                    monitor.OnActionCompleted(this);
                }
                Destroy(this);
            }
        }
    }


    public class ActionManager : System.Object
    {
        private static ActionManager _instance;

        public static ActionManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new ActionManager();
            }
            return _instance;
        }

        public Action ApplyMoveToAction(GameObject obj, Vector3 target, float speed)
        {
            return ApplyMoveToAction(obj, target, speed, null);
        }

        public Action ApplyMoveToAction(GameObject obj, Vector3 target, float speed, ActionCompleted completed)
        {
            MoveToAction action = obj.AddComponent<MoveToAction>();
            action.getAction(target, speed, completed);
            return action;
        }

        public Action ApplyMoveToYZAction(GameObject obj, Vector3 target, float speed)
        {
            return ApplyMoveToYZAction(obj, target, speed, null);
        }

        public Action ApplyMoveToYZAction(GameObject obj, Vector3 target, float speed, ActionCompleted completed)
        {
            MoveToYZAction action = obj.AddComponent<MoveToYZAction>();
            action.getAction(obj, target, speed, completed);
            return action;
        }
    }
}

public class BaseCode : MonoBehaviour
{
    void Awake()
    {
        Director pai = Director.getInstance();
        pai.setBaseCode(this);
    }
}