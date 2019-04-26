using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckConversions : MonoBehaviour {

    
    private float minDistance = 0;
    private float maxDistance = 0;
    private bool foundCubes = false;
    private List<GameObject> m_AllCubes;


    void Update () {
        if(m_AllCubes != null && !foundCubes)
        {
            for (int i = 0; i < m_AllCubes.Count; i++)
            {
                float dist = m_AllCubes[i].GetComponent<MS_Block>().distanceToBomb;
                if (minDistance > dist)
                {
                    minDistance = dist;
                }
                else if (maxDistance < dist)
                {
                    maxDistance = dist;
                }
            }
            foundCubes = true;
        }
    }


    public float ColorToFreqency(float distanceToBomb)
    {

        float minFreq = 200;
        float maxFreq = 600;        

        float normalizedVal = Remap(distanceToBomb, minDistance, maxDistance, maxFreq, minFreq);

        return normalizedVal;
    }

    public void SetAllCubes(List<GameObject> list)
    {
        m_AllCubes = list;
    }

    public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
}
