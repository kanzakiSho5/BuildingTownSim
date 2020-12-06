using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    [SerializeField] private LoadBuilding[] buildingObj;

    private GameObject[] loadBuildings;

    private Bezier[] loadSideBezier = new Bezier[2];
    private bool isInit = false;

    #region UnityEvent Method

    private void OnDrawGizmos()
    {
        if(loadSideBezier[0] != null)
            return;
        
        Gizmos.color = Color.cyan;
        for (var i = 0; i < loadSideBezier.Length; i++)
        {
            Gizmos.DrawSphere(loadSideBezier[i].RootPositions[0], .5f);
            Gizmos.DrawSphere(loadSideBezier[i].RootPositions[1], .5f);
            Gizmos.DrawSphere(loadSideBezier[i].RootPositions[2], .5f);
            for (var j = 1; j < loadSideBezier[i].Positions.Length; j++)
            {
                Gizmos.DrawLine(loadSideBezier[i].Positions[j-1],loadSideBezier[i].Positions[j]);
            }
        }
    }

    #endregion
        
    #region public Method

    /// <summary>
    /// ベジェに沿って建物を建てる関数
    /// </summary>
    /// <param name="bezier">ベジェ</param>
    /// <param name="loadLength">建物の間隔</param>
    public void CreateBuilding(Bezier bezier, float loadLength)
    {
        // TODO:道が移動したときに建物の位置も変更させる機構の追加
        if(isInit)
            return;
        isInit = true;
        
        loadBuildings = new GameObject[bezier.Detail * 2];

        var startAngle  = GetAngle(bezier.RootPositions[0], bezier.RootPositions[2]);
        var endAngle    = GetAngle(bezier.RootPositions[2], bezier.RootPositions[1]);
        var handleAngle = GetAngle(bezier.RootPositions[0], bezier.RootPositions[1]);
        var leftStartVector  = PolarToVector(startAngle  + (90 * Mathf.Deg2Rad), loadLength / 2);
        var leftEndVector    = PolarToVector(endAngle    + (90 * Mathf.Deg2Rad), loadLength / 2);
        var leftHandleVector = PolarToVector(handleAngle + (90 * Mathf.Deg2Rad), loadLength / 2);
        var rightStartVector  = PolarToVector(startAngle  + (-90 * Mathf.Deg2Rad), loadLength / 2);
        var rightEndVector    = PolarToVector(endAngle    + (-90 * Mathf.Deg2Rad), loadLength / 2);
        var rightHandleVector = PolarToVector(handleAngle + (-90 * Mathf.Deg2Rad), loadLength / 2);
        loadSideBezier[0] = new Bezier(
            bezier.RootPositions[0] + leftStartVector,
            bezier.RootPositions[1] + leftEndVector,
            bezier.RootPositions[2] + leftHandleVector);
        loadSideBezier[1] = new Bezier(
            bezier.RootPositions[0] + rightStartVector,
            bezier.RootPositions[1] + rightEndVector,
            bezier.RootPositions[2] + rightHandleVector);

        for(int i = 0; i < loadSideBezier.Length; i++)
        {
            var buildDistance = 0f;
            for (int j = 0; j < 10; j++)
            {
                if (buildDistance > loadSideBezier[i].Length) break;
                var rand = Random.Range(0, buildingObj.Length-1);
                var length = buildDistance +  buildingObj[rand].Length / 2;
                Debug.LogFormat("buildDistance: {0}, bezierLength: {1}", buildDistance, loadSideBezier[i].Length);
                var dir = (loadSideBezier[i].ConstantBezierPosition(buildDistance) -
                           bezier.ConstantBezierPosition(buildDistance)).normalized;
                var rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg, Vector3.down);
                loadBuildings[j + (i * 10)] = Instantiate<GameObject>(
                    buildingObj[rand].Building,
                    loadSideBezier[i].ConstantBezierPosition(length),
                    rotation,
                    transform);
                buildDistance += buildingObj[rand].Length;
            }
        }
    }
    
    #endregion

    #region private Method

    private float GetAngle(Vector3 start, Vector3 end)
    {
        return Mathf.Atan2(end.z - start.z, end.x - start.x);
    }

    private Vector3 PolarToVector(float radian, float radius)
    {
        return new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius;
    }

    #endregion
}

[System.Serializable]
public class LoadBuilding
{
    [SerializeField]
    private GameObject building;
    [SerializeField]
    private float length;

    public GameObject Building
    {
        get => building;
    }

    public float Length
    {
        get => length;
    }
    
}
