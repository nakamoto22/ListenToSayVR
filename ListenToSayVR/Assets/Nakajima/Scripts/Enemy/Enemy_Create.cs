﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Create : MonoBehaviour
{
    
    public Mode mode;

    //生成されるワニの座標
    Vector3 B_Pos;

    public enum Mode
    {
        Easy,
        Nomal,
        Anger
    }

    //ワニの種類
    [SerializeField]
    GameObject Easy;   //易しいワニ
    [SerializeField]
    GameObject Nomal;　//普通のワニ
    [SerializeField]
    GameObject Anger;  //怒っているワニ

    //ターゲット(原点)
    [SerializeField, Space]
    Vector3 targetPos;

    //インターバル
    float interval;

	// Use this for initialization
	void Start ()
    {
        //敵生成
        EnemyBorn();
        StartCoroutine(EnemyCreater());
    }

    // Update is called once per frame
    void Update ()
    {
        
    }



    /// ワニ生成のコルーチン
    IEnumerator EnemyCreater()
    {
        yield return new WaitForSeconds(interval);

        //敵生成
        EnemyBorn();
        
        StartCoroutine(EnemyCreater());
    }

    //ワニ生成メソッド
    void EnemyBorn()
    {
        //敵スクリプト用意
        EnemyController enemy = new EnemyController();
        //出現位置
        B_Pos = new Vector3(Random.Range(-16.0f, 16.0f), 0, 35.0f);

        switch (mode)
        {
            case Mode.Easy:

                Easy.transform.position = B_Pos;
                enemy = Instantiate(Easy).transform.GetComponent<EnemyController>();
                interval = 4.0f;

                break;
            case Mode.Nomal:

                Nomal.transform.position = B_Pos;
                enemy = Instantiate(Nomal).transform.GetComponent<EnemyController>();
                interval = 3.0f;

                break;
            case Mode.Anger:

                Anger.transform.position = B_Pos;
                enemy = Instantiate(Anger).transform.GetComponent<EnemyController>();
                interval = 1.0f;

                break;
        }

        //敵にターゲットを与える
        enemy.SetTarget(targetPos);
    }

    public void ChangeMode(int modeNo)
    {
        mode = (Mode)modeNo;
    }
}
