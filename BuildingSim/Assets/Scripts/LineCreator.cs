using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public static LineCreator Instance { get; private set; }
    public bool isOnCreate = false;


    private Vector3 lastHandlePos = Vector3.zero;
    private Vector3 handlePos = Vector3.zero;

    [SerializeField] private GameObject LoadObj;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void OnCreateLoad(Node startNode, Node endNode)
    {
        lastHandlePos = handlePos;
        GameObject obj = Instantiate(LoadObj, endNode.position, Quaternion.identity);
        
        // handleの位置
        if (handlePos == Vector3.zero)
        {
            handlePos = Vector3.Lerp(startNode.position, endNode.position, .5f);
        }
        obj.GetComponent<Line>().SetNode(startNode, endNode, handlePos);
        handlePos = endNode.position - (handlePos - endNode.position);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(lastHandlePos, handlePos - lastHandlePos);
    }
}
