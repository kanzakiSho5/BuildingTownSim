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

    #region UnityEvent Method
    private void Awake()
    {
        if (!Instance) Instance = this;
    }
    #endregion
    
    #region public Method
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

    public void CreatedNode(Node createdNode)
    {
        
        Nodes.Add(createdNode);
        // 道がつくれるとき
        if (ActiveNode || Nodes.Count > 1)
        {
            AllNodeOnActive(false);
        }

        selectNode = Nodes[Nodes.Count - 1];
        Nodes[Nodes.Count - 1].OnActive(true);
        
    }
    #endregion
}

public class lineInfo
{
    public Transform bone;
    public float direction;
    public float lineLength;
    public int? lineCount;

    public lineInfo(Transform bone, float lineLength)
    {
        this.bone = bone;
        this.lineLength = lineLength;
        lineCount = null;
    }

    public lineInfo(Transform bone, float lineLength, int? lineCount)
    {
        this.bone = bone;
        this.lineLength = lineLength;
        this.lineCount = lineCount;
    }

    public void ChangeCount(int count)
    {
        lineCount = count;
    }

    public void ChangeDirection(float dir)
    {
        this.direction = dir;
    }
}

public class lineInfoComparer : IComparer<lineInfo>
{
    public int Compare(lineInfo x, lineInfo y)
    {
        return x.direction.CompareTo(y.direction);
    }
}
