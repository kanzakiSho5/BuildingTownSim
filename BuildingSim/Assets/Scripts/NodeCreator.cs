using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
        
        // TODO: nodecountの計算

        if (!NodesManager.Instance.ActiveNode)
        {
            GameObject node = Instantiate(NodeObj, position, Quaternion.identity);
            NodesManager.Instance.CreatedNode(node.GetComponent<Node>());
            return;
        }
            
        var activeNodePos = NodesManager.Instance.ActiveNode.position;
        var handlePos = LineCreator.Instance.CurrentHandlePos;
        var lineLength = 
            Mathf.Sqrt(
                Mathf.Pow(activeNodePos.x - handlePos.x, 2f) + 
                Mathf.Pow(activeNodePos.y - handlePos.y, 2f) + 
                Mathf.Pow(activeNodePos.z - handlePos.z, 2f)) + 
            Mathf.Sqrt(
                Mathf.Pow(handlePos.x - position.x, 2f) + 
                Mathf.Pow(handlePos.y - position.y, 2f) + 
                Mathf.Pow(handlePos.z - position.z, 2f));

        Debug.Log("LineLength: "+ lineLength);
        var nodeCount = Mathf.FloorToInt(lineLength / 20) + 1;
        var tmpSecHandlePos = handlePos;
        
        for (int i = 1; i <= nodeCount; i++)
        {
            // 中間点更新されたActiveNodeの位置
            var currentActiveNode = NodesManager.Instance.ActiveNode;
            
            /*
            Debug.Log(
                "i / nodeCount: "+ ((float)(i-1) / (nodeCount - (i - 1))) + 
                        ", i: "          + i + 
                        ", Count: "      + nodeCount);
            */
            
            // 中間ノード位置
            var createNodePos = 
                Vector3.Lerp(
                    Vector3.Lerp(activeNodePos,handlePos, (float)i / nodeCount),
                    Vector3.Lerp(handlePos, position, (float)i/ nodeCount),
                    (float)i/nodeCount);
            
            // 中間ハンドル位置
            var createHandlePos = Vector3.Lerp(currentActiveNode.position, tmpSecHandlePos, (float)1 / (nodeCount - (i - 1)));
            // 残った側のベジェハンドル位置
            tmpSecHandlePos = Vector3.Lerp(tmpSecHandlePos, position, (float) 1 / (nodeCount - (i - 1)));
            
            GameObject node = Instantiate(NodeObj, createNodePos, Quaternion.identity);
            LineCreator.Instance.OnCreateRoad(
                currentActiveNode, 
                node.GetComponent<Node>(),
                createHandlePos);
            NodesManager.Instance.CreatedNode(node.GetComponent<Node>());
        }
        
        
        
    }

    public void CreateNode(Vector3 position, Node endNode)
    {
        if(GameManager.Instance.CullentCreateType != CreateType.CreateLoad) return;
        // Debug.Log(position);
        
        // TODO: nodecountの計算

        if (!NodesManager.Instance.ActiveNode)
        {
            GameObject node = Instantiate(NodeObj, position, Quaternion.identity);
            NodesManager.Instance.CreatedNode(node.GetComponent<Node>());
            return;
        }
            
        var activeNodePos = NodesManager.Instance.ActiveNode.position;
        var handlePos = LineCreator.Instance.CurrentHandlePos;
        var bezier = new Bezier(activeNodePos, position, handlePos);
        var bezierLength = bezier.Length;

        Debug.Log("LineLength: "+ bezierLength);
        var nodeCount = Mathf.FloorToInt(bezierLength / 20) + 1;
        var tmpSecHandlePos = handlePos;
        
        for (int i = 1; i <= nodeCount; i++)
        {
            // 中間点更新されたActiveNodeの位置
            var currentActiveNode = NodesManager.Instance.ActiveNode;
            
            // 長さからの等分位置
            var tPos = bezier.ConstantBezierT((float) i / nodeCount); 

            // 中間ノード位置
            var createNodePos = bezier.BezierPosition(tPos);
            
            // 中間ハンドル位置
            var createHandlePos = Vector3.Lerp(currentActiveNode.position, tmpSecHandlePos, tPos);
            
            // 残った側のベジェハンドル位置
            tmpSecHandlePos = Vector3.Lerp(tmpSecHandlePos, position, 1 - tPos);

            if (i == nodeCount)
            {
                LineCreator.Instance.OnCreateRoad(
                    currentActiveNode, 
                    endNode,
                    createHandlePos);
                break;
            }
            
            GameObject node = Instantiate(NodeObj, createNodePos, Quaternion.identity);
            LineCreator.Instance.OnCreateRoad(
                currentActiveNode, 
                node.GetComponent<Node>(),
                createHandlePos);
            NodesManager.Instance.CreatedNode(node.GetComponent<Node>());
        }
    }
}
