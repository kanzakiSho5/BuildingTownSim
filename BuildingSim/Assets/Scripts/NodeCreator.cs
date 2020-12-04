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
    
    /// <summary>
    /// ActiveNodeから新たな地点へ道を作る関数
    /// </summary>
    /// <param name="position">終点位置</param>
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
        var bezier = new Bezier(activeNodePos, position, handlePos);
        var bezierLength = bezier.Length;

        var nodeCount = Mathf.FloorToInt(bezierLength / 30) + 1;
        var tmpSecHandlePos = handlePos;
        var tmpT = 0f;
        
        for (int i = 1; i <= nodeCount; i++)
        {
            // 中間点更新されたActiveNodeの位置
            var currentActiveNode = NodesManager.Instance.ActiveNode;
            
            // 長さからの等分位置
            var tPos = bezier.ConstantBezierT((float) i / nodeCount); 

            // 中間ノード位置
            var createNodePos = bezier.BezierPosition(tPos);
            
            //Debug.LogFormat("tmpT : {0}, tPos: {1}",tmpT, tPos);
            // 中間ハンドル位置
            var createHandlePos = bezier.SeparateBezierHandlePos(tmpT, tPos);
            
            tmpT = tPos;
            
            GameObject node = Instantiate(NodeObj, createNodePos, Quaternion.identity);
            LineCreator.Instance.OnCreateRoad(
                currentActiveNode, 
                node.GetComponent<Node>(),
                createHandlePos);
            NodesManager.Instance.CreatedNode(node.GetComponent<Node>());
        }
        
        
        
    }

    /// <summary>
    /// ActiveNodeからすでに存在するNodeへの間を補完する道を作る関数
    /// </summary>
    /// <param name="endNode">終点Node</param>
    public void CreateNode(Node endNode)
    {
        if(GameManager.Instance.CullentCreateType != CreateType.CreateLoad) return;

        var activeNodePos = NodesManager.Instance.ActiveNode.position;
        var handlePos = LineCreator.Instance.CurrentHandlePos;
        var bezier = new Bezier(activeNodePos, endNode.position, handlePos);
        var bezierLength = bezier.Length;

        Debug.Log("LineLength: "+ bezierLength);
        var nodeCount = Mathf.FloorToInt(bezierLength / 30) + 1;
        var tmpT = 0f;

        for (int i = 1; i <= nodeCount; i++)
        {
            // 中間点更新されたActiveNodeの位置
            var currentActiveNode = NodesManager.Instance.ActiveNode;
            
            // 長さからの等分位置
            var tPos = bezier.ConstantBezierT((float) i / nodeCount); 

            // 中間ノード位置
            var createNodePos = bezier.BezierPosition(tPos);
            
            // 中間ハンドル位置
            var createHandlePos = bezier.SeparateBezierHandlePos(tmpT, tPos);

            tmpT = tPos;

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
