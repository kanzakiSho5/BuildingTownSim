using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
    [SerializeField] private GameObject NodeObj;
    public bool isOnCreate = false;

    public static NodeCreator Instance { get; private set; }
    
    private void Awake()
    {
        if (!Instance) Instance = this;
    }
    
    public void CreateNode(Vector3 position)
    {
        if(GameManager.Instance.CullentCreateType != CreateType.CreateLoad) return;
        // Debug.Log(position);
        GameObject node = Instantiate(NodeObj, position, Quaternion.identity);
        NodesManager.Instance.OnCreatedNode(node.GetComponent<Node>());
        
    }
}
