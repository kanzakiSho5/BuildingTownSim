using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(NodeMeshRenderer))]
public class Node : MonoBehaviour
{
    public delegate void OnChangePositionDelegate();
    public event OnChangePositionDelegate OnChangePosition;
    
    Vector3 temp;
    private List<lineInfo> _conectLineInfo = new List<lineInfo>();
    private NodeMeshRenderer _meshRenderer;

    public Vector3 position
    {
        get
        {
            Vector3 position = transform.position;
            position.y += .1f;
            return position;
        }
    }
    
    public bool isActive
    {
        get;
        private set;
    }

    private void OnEnable()
    {
        Debug.Log("OnCreatedRoad");
        _meshRenderer = GetComponent<NodeMeshRenderer>();

    }

    private void Update()
    {
        
        if (temp != position)
        {
            if(OnChangePosition != null)
                OnChangePosition();
        }

        temp = position;
    }

    public int AddLine(lineInfo info)
    {
        // Debug.Log(info.position);
        Vector3 linePos = info.bone.position;
        Vector3 nodePos = position;
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir); 
        info.ChangeCount(_conectLineInfo.Count);
        _conectLineInfo.Add(info);
        if(_conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            SetIntersection();
        return (int)info.lineCount;
    }

    public void ChangeLine(lineInfo info)
    {
        // Debug.Log("ChangeRoad"+ position +": "+ info.bone.position +", "+ info.lineCount);
        Vector3 linePos = info.bone.position;
        Vector3 nodePos = position;
        
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir);
        for (var i = 0; i < _conectLineInfo.Count; i++)
        {
            if (_conectLineInfo[i].lineCount != info.lineCount)
                continue;
            _conectLineInfo[i] = info;
            break;
        }
        if(_conectLineInfo.Count <= 2)
            SetWayAngle();
        else
            SetIntersection();
    }

    public void OnClick()
    {
        
        if (GameManager.Instance.CullentCreateType == CreateType.CreateLoad)
        {
            LineCreator.Instance.OnCreateLoad(NodesManager.Instance.ActiveNode, this);
            Debug.Log(NodesManager.Instance.ActiveNode.position +", "+ this.position);
            temp = Vector3.zero;// Road位置更新
        }

        NodesManager.Instance.AllNodeOnActive(false);
        OnActive(true);
        // ハンドルの方向がおかしくなるので、とりあえず初期状態に戻す
        LineCreator.Instance.OnChangeHandlePos();
    }

    private void OnMouseEnter()
    {
        Debug.Log("On Over Node!");
        GameManager.Instance.OnFriezeCoursorPos(this.position);
    }

    private void OnMouseExit()
    {
        GameManager.Instance.UnFriezeCousorPos();
    }

    public void OnActive(bool active)
    {
        isActive = active;
        NodesManager.Instance.OnChangeSelectNode();
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        // Debug.Log(pos);
    }

    private void SetWayAngle()
    {
        // TODO:　モデル差し替えに伴う処理の変更
        GameObject endRoad = transform.GetChild(0).gameObject;
        
        endRoad.SetActive(false);
        if(_conectLineInfo.Count == 0)
            return;
        
        
        // 終点処理
        endRoad.SetActive(true);
        lineInfo lineInfo = _conectLineInfo[0];
        var dir = lineInfo.direction;
        endRoad.transform.eulerAngles = Vector3.down * (dir * Mathf.Rad2Deg);
        
        
        lineInfo.bone.GetChild(1).position = new Vector3(Mathf.Cos(dir + (Mathf.PI * .5f)),0,Mathf.Sin(dir + (Mathf.PI * .5f)))*
            (lineInfo.lineLength * .5f) + position;
        lineInfo.bone.GetChild(0).position = new Vector3(Mathf.Cos(dir + (Mathf.PI * .5f)),0,Mathf.Sin(dir + (Mathf.PI * .5f)))*
            (-lineInfo.lineLength * .5f) + position;
        
        //lineInfo.bone.position = pos;
        if(_conectLineInfo.Count == 1)
            return;

        // 道が続くとき
        endRoad.SetActive(false);
        lineInfo = _conectLineInfo[1];
        lineInfo.bone.GetChild(1).position = new Vector3(Mathf.Cos(dir + (Mathf.PI * .5f)),0,Mathf.Sin(dir + (Mathf.PI * .5f)))*
            (-lineInfo.lineLength * .5f) + position;
        lineInfo.bone.GetChild(0).position = new Vector3(Mathf.Cos(dir + (Mathf.PI * .5f)),0,Mathf.Sin(dir + (Mathf.PI * .5f)))*
            (lineInfo.lineLength * .5f) + position;
    }
    
    private List<Vector3> _intersectionVertex = new List<Vector3>();
    
    private void SetIntersection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _conectLineInfo.Sort(new lineInfoComparer());
        _intersectionVertex.Clear();
        for (var i = 0; i < _conectLineInfo.Count; i++)
        {
            // Meshの張る頂点の計算
            Vector3 pos = transform.position;
            Vector3 linepos = new Vector3(Mathf.Cos(_conectLineInfo[i].direction + Mathf.PI), 0, Mathf.Sin(_conectLineInfo[i].direction + Mathf.PI)) + pos;
            if (i == 0)
            {
                // 傾き
                var dir = _conectLineInfo[i].direction + Mathf.PI * .5f;
                float nextDir = _conectLineInfo[_conectLineInfo.Count - 1].direction + Mathf.PI * .5f;
                
                // ベクトル
                Vector3 vec = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
                Vector3 nextVec = new Vector3(Mathf.Cos(nextDir),0,Mathf.Sin(nextDir));
                Vector3 nextLinePos = new Vector3(Mathf.Cos(_conectLineInfo[_conectLineInfo.Count -1].direction), 0, Mathf.Sin(_conectLineInfo[_conectLineInfo.Count - 1].direction)) + pos;

                float lineLength = _conectLineInfo[i].lineLength * .5f;
                float nextLineLength = -_conectLineInfo[_conectLineInfo.Count - 1].lineLength * .5f;
                
                Vector3 vertex = GameManager.GetIntersection(
                    vec * lineLength + pos,
                    vec * lineLength + linepos,
                    nextVec * nextLineLength + pos,
                    nextVec * nextLineLength + nextLinePos
                    );
                
                _intersectionVertex.Add(vertex - pos);
            }
            else
            {
                // 傾き
                var dir = _conectLineInfo[i - 1].direction + Mathf.PI * .5f;
                float nextDir = _conectLineInfo[i].direction + Mathf.PI * .5f;
                
                // ベクトル
                Vector3 vec = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
                Vector3 nextVec = new Vector3(Mathf.Cos(nextDir),0,Mathf.Sin(nextDir));
                Vector3 prevLinePos = new Vector3(Mathf.Cos(_conectLineInfo[i -1].direction), 0, Mathf.Sin(_conectLineInfo[i - 1].direction)) + pos;
                
                float lineLength = -_conectLineInfo[i - 1].lineLength * .5f;
                float nextLineLength = _conectLineInfo[i].lineLength * .5f;
                
                Vector3 vertex = GameManager.GetIntersection(
                    vec * lineLength + pos,
                    vec * lineLength + prevLinePos,
                    nextVec * nextLineLength + pos,
                    nextVec * nextLineLength + linepos
                );

                // Debug.LogFormat("info Count: {0}, vertex: {1}", _conectLineInfo[i].lineCount, vertex);
                _intersectionVertex.Add(vertex - pos);
            }
        }
        
        
        for(var i = 0; i < _conectLineInfo.Count;i++)
        {
            
            Vector3 pos = transform.position;
            // Lineの先端のBone処理
            if (i == _conectLineInfo.Count - 1)
            {
                var dir = (Mathf.Atan2(
                               _intersectionVertex[i].z - _intersectionVertex[0].z,
                               _intersectionVertex[i].x - _intersectionVertex[0].x) +
                           Mathf.PI * .5f) * Mathf.Rad2Deg; // モデル初期角度を引く
                _conectLineInfo[i].bone.eulerAngles = Vector3.down * (dir);
                _conectLineInfo[i].bone.GetChild(0).position = _intersectionVertex[0] + pos;
                _conectLineInfo[i].bone.GetChild(1).position = _intersectionVertex[i] + pos;
            }
            else
            {
                var dir = (Mathf.Atan2(
                               _intersectionVertex[i].z - _intersectionVertex[i+1].z,
                               _intersectionVertex[i].x - _intersectionVertex[i+1].x) +
                           Mathf.PI * .5f) * Mathf.Rad2Deg; // モデル初期角度を引く
                _conectLineInfo[i].bone.eulerAngles = Vector3.down * (dir);
                _conectLineInfo[i].bone.GetChild(0).position = _intersectionVertex[i+1] + pos;
                _conectLineInfo[i].bone.GetChild(1).position = _intersectionVertex[i] + pos;
            }
        }
        _meshRenderer.CreateMesh(_intersectionVertex);
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = position;
        Gizmos.color = Color.gray;
        if(isActive) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, .5f);
        Gizmos.color = Color.green;
        for (int i = 0; i < _conectLineInfo.Count; i++)
        {
            // Debug.Log(conectLineInfo[i].position +", "+ transform.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_conectLineInfo[i].bone.position, .1f);
            
            Vector3 linepos = new Vector3(Mathf.Cos(_conectLineInfo[i].direction + Mathf.PI), 0, Mathf.Sin(_conectLineInfo[i].direction + Mathf.PI)) + pos;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, linepos);
            
            if(_conectLineInfo.Count <= 2)
                continue;
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_intersectionVertex[i] + transform.position, 0.1f);
            
        }
        
        if(_conectLineInfo.Count != 2)
            return;
        
        pos = new Vector3(
            (Mathf.Cos(_conectLineInfo[0].direction) + Mathf.Cos(_conectLineInfo[1].direction)),
            0,
            (Mathf.Sin(_conectLineInfo[0].direction) + Mathf.Sin(_conectLineInfo[1].direction))).normalized;
        // Debug.Log(pos);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,pos + transform.position);

    }
}
