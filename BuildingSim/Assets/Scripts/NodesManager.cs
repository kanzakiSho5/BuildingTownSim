using System;
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
                NodeCreator.Instance.isOnCreate = true;
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

        if (!active)
        {
            NodeCreator.Instance.isOnCreate = false;
        }
    }

    public void OnCreatedNode(Node createdNode)
    {
        
        Nodes.Add(createdNode);
        // 道がつくれるとき
        if (ActiveNode || Nodes.Count > 1)
        {
            LineCreator.Instance.OnCreateLoad(ActiveNode, createdNode);
            AllNodeOnActive(false);
        }

        selectNode = Nodes[Nodes.Count - 1];
        Nodes[Nodes.Count - 1].OnActive(true);
        
    }
    
}

public class lineInfo
{
    public Vector3 position;
    public float lineLength;
    public int? lineCount;

    public lineInfo(Vector3 position, float lineLength)
    {
        this.position = position;
        this.lineLength = lineLength;
        lineCount = null;
    }

    public lineInfo(Vector3 position, float lineLength, int? lineCount)
    {
        this.position = position;
        this.lineLength = lineLength;
        this.lineCount = lineCount;
    }

    public void ChangeCount(int count)
    {
        lineCount = count;
    }
}
