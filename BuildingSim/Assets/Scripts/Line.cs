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

    [Tooltip("道の分割数")] 
    [SerializeField] private int detail = 4;
    
    [Tooltip("道の幅")]
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
        
        lineNodePosition = new Vector3[detail * 2 + 1];
    }

    private void OnChangePositionHandler()
    {
        Transform startSideRoadBone = transform.GetChild(0).GetChild(1);
        Transform endSideRoadBone = transform.GetChild(0).GetChild(0);
        // string str = "";
        
        // TODO: 道を更新する際にNodeの更新がずれる
        lineNodePosition[detail] = BezierPoint(startNode.position, endNode.position, handlePos, (detail + 1) / (float) (detail * 2 + 2));
        transform.GetChild(0).position = lineNodePosition[detail];
        float dir = GetDirection(startNode.position, endNode.position) * Mathf.Rad2Deg;
        transform.GetChild(0).eulerAngles = Vector3.down * dir;
        
        for (int i = 0; i < detail; i++)
        {
            var startSideIndex = i + detail + 1;
            var endSideIndex = -i + detail - 1;
            
            Vector3 startPos = BezierPoint(startNode.position, endNode.position, handlePos,
                (startSideIndex + 1) / (float) (detail * 2 + 2));
            Vector3 endPos = BezierPoint(startNode.position, endNode.position, handlePos,
                (endSideIndex + 1) / (float) (detail * 2 + 2));
            
            // Debug.Log(
            //     startSideIndex + ", " + 
            //     startPos +", "+ 
            //     endSideIndex + ", "+ 
            //     endPos + "," +
            //     (detail * 2 + 2));
            
            lineNodePosition[startSideIndex] = startPos;
            lineNodePosition[endSideIndex] = endPos;
            startSideRoadBone.position = startPos;
            endSideRoadBone.position = endPos;
            
            startSideRoadBone.eulerAngles = Vector3.down * (Mathf.Rad2Deg * GetDirection(startPos, lineNodePosition[startSideIndex - 1]));
            endSideRoadBone.eulerAngles = Vector3.down * (Mathf.Rad2Deg * GetDirection(endPos, lineNodePosition[endSideIndex + 1]));
            
            if (i == detail - 1)
            {
                //startSideRoadBone.eulerAngles += Vector3.up * 90;
                //endSideRoadBone.eulerAngles += Vector3.down * 90;
                
                if(startNodeNumber == null)
                    startNodeNumber = startNode.AddLine(new lineInfo(endSideRoadBone, loadLength));
                else
                    startNode.ChangeLine(new lineInfo(endSideRoadBone, loadLength, startNodeNumber));
                
                if(endNodeNumber == null)
                    endNodeNumber = endNode.AddLine(new lineInfo(startSideRoadBone, loadLength));
                else
                    endNode.ChangeLine(new lineInfo(startSideRoadBone, loadLength, endNodeNumber));
            }
            
            startSideRoadBone = startSideRoadBone.GetChild(0);
            endSideRoadBone = endSideRoadBone.GetChild(0);
        }
        // Debug.Log(str);
    }

    private float GetDirection(Vector3 start, Vector3 lookAt)
    {
        float rad = Mathf.Atan2(lookAt.z - start.z, lookAt.x - start.x);
        return rad;
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
