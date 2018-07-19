﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehavior : MonoBehaviour {

    public GameObject front;

    GameObject head;
    //int bodyCount;
 

    void Start ()
    {
        //bodyCount = GameplayData.gd.bodyCount;
        head = ObjectManager.om.transform.GetChild(0).gameObject;
	}

    float currentTimer;
	void Update ()
    {
        GameObject go;
        Transform followObj;
        for (int i = 0; i < ObjectManager.om.bodyAmount; i++)
        {
            go = ObjectManager.om.bodyObjects[i];
            
            if (i == 0)
            {
                // ObjectManager -> Head (0) -> Head.Spawner (0)
                followObj = head.transform.GetChild(0).transform;
            }
            else
            {
                // ObjectManager -> Body (i-1) -> Body.Spawner (0)
                followObj = ObjectManager.om.bodyObjects[i - 1].transform.GetChild(0).transform;
            }
            go.transform.position = followObj.position;
            go.transform.rotation = Quaternion.Lerp(go.transform.rotation, followObj.rotation, GameplayData.gd.dTime);
        }
    }
}