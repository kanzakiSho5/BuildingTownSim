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
        
        // TODO: 一番親のBoneの角度が0になる問題の修正
        var dir = Mathf.Atan2(nodePos.z - linePos.z, nodePos.x - linePos.x);
        info.ChangeDirection(dir);
        _conectLineInfo[(int)info.lineCount] = info;
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
        
        if (_conectLineInfo.Count == 1)
        {
            // 終点処理
            endRoad.SetActive(true);
            lineInfo lineInfo = _conectLineInfo[0];
            var dir = lineInfo.direction * Mathf.Rad2Deg;
            endRoad.transform.eulerAngles = Vector3.down * dir;
            Vector3 pos = transform.position;
            pos.y = .1f;
            lineInfo.bone.position = pos;
            return;
        }
        
        // 道が続くとき
        endRoad.SetActive(false);
    }
    
    private List<Vector3> _intersectionVertex = new List<Vector3>();
    
    private void SetIntersection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _conectLineInfo.Sort(new lineInfoComparer());
        _intersectionVertex.Clear();
        for (var i = 0; i < _conectLineInfo.Count; i++)
        {
            //Debug.Log(_conectLineInfo[i].bone.gameObject.name +": "+ lineInfo.direction);
            // Boneの方向調整
            // TODO: 道の端のBone位置と角度の設定
            var dir = _conectLineInfo[i].direction * Mathf.Rad2Deg;
            // _conectLineInfo[i].bone.eulerAngles = Vector3.down * dir;
            Vector3 pos = transform.position;
            // pos.y = .1f;
            // _conectLineInfo[i].bone.position = pos;
            Vector3 linepos = new Vector3(Mathf.Cos(_conectLineInfo[i].direction + Mathf.PI), 0, Mathf.Sin(_conectLineInfo[i].direction + Mathf.PI)) + pos;
            
            // Meshの張る頂点の計算
            if (i == 0)
            {
                // 傾き
                dir = _conectLineInfo[i].direction + Mathf.PI * .5f;
                float nextDir = _conectLineInfo[_conectLineInfo.Count - 1].direction + Mathf.PI * .5f;
                
                // ベクトル
                Vector3 vec = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
                Vector3 nextVec = new Vector3(Mathf.Cos(nextDir),0,Mathf.Sin(nextDir));
                Vector3 nextLinePos = new Vector3(Mathf.Cos(_conectLineInfo[_conectLineInfo.Count -1].direction), 0, Mathf.Sin(_conectLineInfo[_conectLineInfo.Count - 1].direction)) + pos;
                
                Vector3 vertex = GameManager.GetIntersection(
                    vec * _conectLineInfo[i].lineLength + pos,
                    vec * _conectLineInfo[i].lineLength + linepos,
                    nextVec * -_conectLineInfo[_conectLineInfo.Count - 1].lineLength + pos,
                    nextVec * -_conectLineInfo[_conectLineInfo.Count - 1].lineLength + nextLinePos
                    );
                
                _intersectionVertex.Add(vertex - transform.position);
            }
            else
            {
                // 傾き
                dir = _conectLineInfo[i - 1].direction + Mathf.PI * .5f;
                float nextDir = _conectLineInfo[i].direction + Mathf.PI * .5f;
                
                // ベクトル
                Vector3 vec = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
                Vector3 nextVec = new Vector3(Mathf.Cos(nextDir),0,Mathf.Sin(nextDir));
                Vector3 prevLinePos = new Vector3(Mathf.Cos(_conectLineInfo[i -1].direction), 0, Mathf.Sin(_conectLineInfo[i - 1].direction)) + pos;

                Vector3 vertex = GameManager.GetIntersection(
                    vec * -_conectLineInfo[i - 1].lineLength + pos,
                    vec * -_conectLineInfo[i - 1].lineLength + prevLinePos,
                    nextVec * _conectLineInfo[i].lineLength + pos,
                    nextVec * _conectLineInfo[i].lineLength + linepos
                );

                _intersectionVertex.Add(vertex - transform.position);
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
    }
}
