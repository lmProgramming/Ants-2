using Unity.Mathematics;
using UnityEngine;

public static class VecMathematicsExt
{
    // Vector2
    public static float2 SetX(this float2 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static float2 SetY(this Vector2 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    // Vector3
    public static float3 SetX(this float3 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static float3 AddX(this float3 aVec, float aXValue)
    {
        aVec.x += aXValue;
        return aVec;
    }
    public static float3 SetY(this float3 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }
    public static float3 AddY(this float3 aVec, float aYValue)
    {
        aVec.y += aYValue;
        return aVec;
    }
    public static float3 SetZ(this float3 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }
    public static float3 AddZ(this float3 aVec, float aZValue)
    {
        aVec.z += aZValue;
        return aVec;
    }

    // Vector4
    public static float4 SetX(this float4 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static float4 SetY(this float4 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }
    public static float4 SetZ(this float4 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }
    public static float4 SetW(this float4 aVec, float aWValue)
    {
        aVec.w = aWValue;
        return aVec;
    }
}
