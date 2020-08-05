using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Node startNode;
    private int? startNodeNumber;
    private Node endNode;
    private int? endNodeNumber;
    private Vector3 handlePos;

    private Vector3[] lineNodePosition;

    [SerializeField] private int detail = 10;
    [SerializeField] private float loadLength;
    
    public void SetNode(Node start, Node end)
    {
        Debug.Log(startNode);
        startNode = start;
        endNode = end;
        handlePos = Vector3.Lerp(start.position, end.position, 0.5f);
        startNode.OnChangePosition += OnChangePositionHandler;
        endNode.OnChangePosition += OnChangePositionHandler;
        lineNodePosition = new Vector3[detail];
    }
    
    public void SetNode(Node start, Node end, Vector3 handlePos)
    {
        // Debug.Log(startNode);
        startNode = start;
        endNode = end;
        this.handlePos = handlePos;
        startNode.OnChangePosition += OnChangePositionHandler;
        endNode.OnChangePosition += OnChangePositionHandler;
        
        lineNodePosition = new Vector3[detail];
    }

    private void OnChangePositionHandler()
    {
        Transform roadBone = transform.GetChild(0);
        
        string str = "";
        for (int i = 0; i < detail; i++)
        {
            Vector3 pos = BezierPoint(startNode.position, endNode.position, handlePos, 1 / (float)detail * i);
            str += pos + ", ";
            lineNodePosition[i] = pos;
            // 最初以外のbone
            if (i != 0)
            {
                float rad = Mathf.Atan2(lineNodePosition[i - 1].z - lineNodePosition[i].z,
                    lineNodePosition[i - 1].x - lineNodePosition[i].x);
                float deg = Mathf.Rad2Deg * rad;
                roadBone.parent.eulerAngles = Vector3.down * deg;
            }
            roadBone.position = pos;

            if (i == detail - 1)
            {
                if(startNodeNumber == null)
                    startNodeNumber = startNode.AddLine(new lineInfo(transform.GetChild(0), loadLength));
                else
                    startNode.ChangeLine(new lineInfo(transform.GetChild(0), loadLength, startNodeNumber));
                
                if(endNodeNumber == null)
                    endNodeNumber = endNode.AddLine(new lineInfo(roadBone.GetChild(0), loadLength));
                else
                    endNode.ChangeLine(new lineInfo(roadBone.GetChild(0), loadLength, endNodeNumber));
            }
            
            roadBone = roadBone.GetChild(0);
        }
        // Debug.Log(str);
    }

    private void OnDrawGizmos()
    {
        if(!startNode || !endNode) return;
        Gizmos.color = Color.red;
        foreach (var nodePos in lineNodePosition)
        {
            Gizmos.DrawSphere(nodePos,0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(handlePos, .2f);
    }

    private Vector3 BezierPoint(Vector3 startPos, Vector3 endPos, Vector3 handle, float t)
    {
        var startPoint = Vector3.Lerp(startPos, handle, t);
        var endPoint = Vector3.Lerp(handle, endPos, t);
        return Vector3.Lerp(startPoint, endPoint, t);
    }
}
