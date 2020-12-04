using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    private Vector3[] rootPositions {
        get
        {
            return new[] {start, end, handle};
        } 
    }
    private Vector3 start;
    private Vector3 end;
    private Vector3 handle;
    private Vector3[] positions;
    
    public int Detail = 10;
    public float Length => GetLength(rootPositions);
    public Vector3[] Positions
    {
        get { return positions; }
    }

    #region public Method
    
    public Bezier(Vector3 start, Vector3 end, Vector3 handle)
    {
        this.start = start;
        this.end = end;
        this.handle = handle;
        SetPositions();
    }
    
    public Bezier(Vector3 start, Vector3 end, Vector3 handle, int detail)
    {
        this.start = start;
        this.end = end;
        this.handle = handle;
        this.Detail = detail;
        SetPositions();
    }

    public void SetRoot(Vector3 start, Vector3 end, Vector3 handle)
    {
        this.start = start;
        this.end = end;
        this.handle = handle;
        SetPositions();
    }

    /// <summary>
    /// ベジェの長さに対応したベジェの分割位置
    /// </summary>
    /// <param name="t">0から1までの分割位置</param>
    /// <returns></returns>
    public Vector3 ConstantBezierPosition(float t)
    {
        return BezierPosition(GetTByDistance(t));
    }

    /// <summary>
    /// 分割したときのハンドル位置
    /// </summary>
    /// <param name="startT">0～1までの開始地点</param>
    /// <param name="endT">0～1までの終了直後</param>
    /// <returns></returns>
    public Vector3 SeparateBezierHandlePos(float startT, float endT)
    {
        // 開始地点を切り取ったハンドル位置
        var secHandlePos = Vector3.Lerp(handle, end, startT);
        var startPos = BezierPosition(this.rootPositions, startT);
        
        var secT = ((endT - startT) /(1 - startT));
        //Debug.LogFormat("secT: {0}",secT);
        return Vector3.Lerp(startPos, secHandlePos, secT);
    }

    public float ConstantBezierT(float t)
    {
        var tl = Mathf.Lerp(0, Length, t);
        return GetTByDistance(tl);
    }
    
    public Vector3 BezierPosition(float t)
    {
        return BezierPosition(this.rootPositions, t);
    }
    #endregion

    #region private Method

    private void SetPositions()
    {
        positions = new Vector3[Detail];
        for (int i = 0; i < Detail; i++)
        {
            positions[i] = BezierPosition(i/(float)Detail);
        }
    }

    private float GetTByDistance(float tl)
    {
        var total = 0f;
        var prev = BezierPosition(rootPositions, 0f);
        var c = Vector3.zero;
        var diff = Vector3.zero;

        var a = 0.01f / Length;

        for (float t = a; t < 1f; t += a)
        {
            c = BezierPosition(rootPositions, t);
            diff = prev - c;
            total += (float) Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);

            if (total >= tl) return t;
            prev = c;
        }
        return 1f;
    }
    #endregion
    
    #region static Method

    private static Vector3 BezierPosition(Vector3[] positions, float t)
    {
        return BezierPosition(positions[0], positions[1], positions[2], t);
    }
    
    
    /// <summary>
    /// 二次ベジェ曲線を取得
    /// </summary>
    /// <param name="start">開始点</param>
    /// <param name="end">終点</param>
    /// <param name="handle">ハンドル点</param>
    /// <param name="t">0から1までの分割位置</param>
    /// <returns></returns>
    private static Vector3 BezierPosition(Vector3 start, Vector3 end, Vector3 handle, float t)
    {
        var startPoint = Vector3.Lerp(start, handle, t);
        var endPoint = Vector3.Lerp(handle, end, t);
        return Vector3.Lerp(startPoint, endPoint, t);
    }

    public static float GetLength(Vector3[] positions)
    {
        const float loopResolution = .05f;

        var total = 0f;
        var prev = BezierPosition(positions[0],positions[1],positions[2], 0f);
        var c = Vector3.zero;
        var diff = Vector3.zero;
        for (var t = loopResolution; t < 1f; t += loopResolution)
        {
            c = BezierPosition(positions, t);
            diff = prev - c;
            total += (float) Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z );
            prev = c;
        }
        c = BezierPosition(positions, 1f);
        diff = prev - c;
        total += Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);

        return total;
    }
    #endregion
}
