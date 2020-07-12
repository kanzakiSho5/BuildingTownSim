using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node : MonoBehaviour
{
    public delegate void OnChangePositionDelegate();
    public event OnChangePositionDelegate OnChangePosition;

    
    
    Vector3 temp;
    public void Update()
    {
        
        if (temp != position)
        {
            if(OnChangePosition != null)
                OnChangePosition();
        }

        temp = position;
    }

    public Vector3 position
    {
        get { return transform.position; }
    }
    
    public bool isActive
    {
        get;
        private set;
    }

    public void OnClick()
    {
        NodesManager.Instance.AllNodeOnActive(false);
        OnActive(true);
    }

    public void OnActive(bool active)
    {
        isActive = active;
        NodesManager.Instance.OnChangeSelectNode();
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        Debug.Log(pos);
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.gray;
        if(isActive) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, .5f);
    }
}
