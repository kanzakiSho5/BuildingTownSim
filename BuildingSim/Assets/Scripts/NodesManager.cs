﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesManager : MonoBehaviour
{
    public static NodesManager Instance { get; private set; }

    public List<Node> Nodes = new List<Node>();
    private Node selectNode;

    public Node ActiveNode
    {
        get
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].isActive) return Nodes[i];
            }
            return null;
        }
    }
    
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void MoveSelectNode(Vector3 pos)
    {
        selectNode.Move(pos);
    }

    public void OnChangeSelectNode()
    {
        foreach (var node in Nodes)
        {
            if (node.isActive)
            {
                selectNode = node;
                return;
            }
        }
    }

    public void AllNodeOnActive(bool active)
    {
        foreach (var node in Nodes)
        {
            node.OnActive(active);
        }
    }

    public void OnCreatedNode(Node createdNode)
    {
        
        Nodes.Add(createdNode);
        if (ActiveNode || Nodes.Count > 1)
        {
            LineCreator.Instance.OnCreateLoad(createdNode);
            AllNodeOnActive(false);
        }

        selectNode = Nodes[Nodes.Count - 1];
        Nodes[Nodes.Count - 1].OnActive(true);
        
    }
    
}