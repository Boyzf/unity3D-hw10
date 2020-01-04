using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGame;

public class UI : MonoBehaviour {

    Interfaces userInterface;
    GameStatus state;
    private int flag = 0;//flag作为一个标志判断游戏是否结束
    private string str;


    void Awake()
    {
        userInterface = Director.getInstance() as Interfaces;
        state = Director.getInstance() as GameStatus;
    }
    void Update()
    {
    }

    void OnGUI()
    {

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        string message = state.getMessage(); 

        if (message != "")
        {
            flag = 1;
            GUIStyle word = new GUIStyle();
            word.normal.textColor = new Color(0, 0, 0);//设置字体颜色 
            word.fontSize = 35;
            GUI.TextField(new Rect(280, 20, 80, 50), message, word);
            if (GUI.Button(new Rect(280, 240, 80, 50), "重新开始"))
            {
                userInterface.reset();
            }
        }
        else if(!state.getState())//设定其他状态不可以点击
        {    
            if (GUI.Button(new Rect(400, 60, 50, 50), "move"))
            {
                userInterface.moveBoat();
            }
            if (GUI.Button(new Rect(280, 60, 100, 50), "Tips"))
            {
                userInterface.autoNext();
            }
        }
    }
}
