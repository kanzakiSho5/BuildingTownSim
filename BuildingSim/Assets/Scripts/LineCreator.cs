using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public static LineCreator Instance { get; private set; }

    [SerializeField] private GameObject LoadObj;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void OnCreateLoad(Node endNode)
    {
        GameObject obj = Instantiate(LoadObj, endNode.position, Quaternion.identity);
        obj.GetComponent<Line>().SetNode(NodesManager.Instance.ActiveNode, endNode);
    }
}
