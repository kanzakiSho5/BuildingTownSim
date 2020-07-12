﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Node startNode;
    private Node endNode;
    private Vector3 handlePos;

    private Vector3[] lineNodePosition;

    [SerializeField] private int detail = 10;
    
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

    private void OnChangePositionHandler()
    {
        
        string str = "";
        for (int i = 0; i < detail; i++)
        {
            Vector3 pos = BezierPoint(startNode.position, endNode.position, handlePos, 1 / (float)detail * i);
            str += pos + ", ";
            lineNodePosition[i] = pos;
        }
        Debug.Log(str);
    }

    private void OnDrawGizmos()
    {
        if(!startNode || !endNode) return;
        Gizmos.color = Color.red;
        foreach (var nodePos in lineNodePosition)
        {
            Gizmos.DrawSphere(nodePos,0.2f);
        }
    }

    private Vector3 BezierPoint(Vector3 startPos, Vector3 endPos, Vector3 handle, float t)
    {
        var startPoint = Vector3.Lerp(startPos, handle, t);
        var endPoint = Vector3.Lerp(handle, endPos, t);
        return Vector3.Lerp(startPoint, endPoint, t);
    }
}
