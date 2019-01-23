using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_CubeMaker : MonoBehaviour {

    public float m_OffsetVal = 3;
    public Material testingMat;
    [Space]
    public GameObject m_ColorEffect;
    public GameObject m_BoomEffect;


    private GameObject m_holder;

    private List<GameObject> m_AllCubes;
    private List<GameObject> m_AllEffects;

    private int m_MinesPlaced = 0;
    private int m_MaxMines;
    private float m_GameDiameter;

    /*
         * 
         * 
         * COLORFUL Minesweeper
         * 
         * Farther away from a singular bomb, 
         * More red-shifted.
         * 
         * Start out everyone black, hover over is dark grey. 
         * 
         * 
         * 
         * 
         */


    // Use this for initialization
    void Start () {

        m_AllCubes = new List<GameObject>();
        m_AllEffects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<GameObject> MakeCubes(GameObject f_cube, GameObject f_holder, int f_size, int f_mines)
    {
        print("Found Cube Prefab: " + f_cube.name + "  Cube size is: " + f_cube.transform.localScale);
        
        m_MaxMines = f_mines;
        float cubeOffset = (f_cube.transform.localScale.x / m_OffsetVal);

        for (var i = 0; i < f_size; i++)        {
            for (var j = 0; j < f_size; j++)            {
                for (var k = 0; k < f_size; k++)                {

                    GameObject newCube = Instantiate(f_cube, f_holder.transform);
                    //GameObject particleColor = Instantiate(m_ColorEffect, f_holder.transform);
                    
                    m_AllCubes.Add(newCube);
                    newCube.name = "Cube " + (m_AllCubes.Count - 1);
                    newCube.transform.localPosition = new Vector3(i * cubeOffset, j * cubeOffset, k * cubeOffset);
                    newCube.GetComponent<MeshRenderer>().material = this.gameObject.GetComponent<MS_Main>().m_InactiveCubeMat;

                    newCube.GetComponent<MS_Block>().SetParticleSystem(m_ColorEffect);

                    //m_AllEffects.Add(particleColor);
                    //particleColor.name = "Effect System " + (m_AllCubes.Count - 1);
                    //particleColor.transform.localPosition = new Vector3(i * cubeOffset, j * cubeOffset, k * cubeOffset);
                    //particleColor.GetComponent<ParticleSystem>().Stop();
                }
            }
        }

        /*
        //translate to center - 
        //get size of cube. 
        print("One Cube Bounds: " + f_cube.GetComponent<MeshRenderer>().bounds);

        float objSize = f_cube.GetComponent<MeshRenderer>().bounds.extents.x;

        print("Given Size: " + objSize);
        print("Given Offset: " + cubeOffset);

        float length = (objSize + cubeOffset) * f_size;

        print("Calculated Length: " + length);


        for (var i = 0; i < m_allCubes.Count; i++)
        {
            Vector3 PositionCorrection = new Vector3(
            //m_allCubes[i].transform.localPosition.x - length,
            m_allCubes[i].transform.localPosition.x,
            //m_allCubes[i].transform.localPosition.y - length,
            m_allCubes[i].transform.localPosition.y,
            m_allCubes[i].transform.localPosition.z - length
            //m_allCubes[i].transform.localPosition.z
            );

            m_allCubes[i].transform.localPosition = PositionCorrection;

            print("Cube #" + i + " corrected position: " + PositionCorrection);
        }
        */



        m_GameDiameter = Vector3.Distance(m_AllCubes[0].transform.position, m_AllCubes[m_AllCubes.Count - 1].transform.position);
        print("Farthest distance possible from block to block: " + m_GameDiameter);

        PlaceMines();
        ColorCalculation();
        
        return m_AllCubes;
        
    }



    private void PlaceMines()
    {
        for (var i = 0; i < m_MaxMines; i++)
        {
            List<int> bombNumArray = new List<int>();
            int bombNum = Random.Range(0, m_AllCubes.Count);

            for (var j = 0; j <= bombNumArray.Count; j++)
            {
                if (bombNumArray.Contains(bombNum))
                {
                    continue;
                }
                else
                {
                    bombNumArray.Add(bombNum);
                    m_AllCubes[bombNum].AddComponent<MS_Bomb>();
                    m_AllCubes[bombNum].name = "Bomb! #" + bombNum;
                    m_AllCubes[bombNum].GetComponent<MS_Block>().SetParticleSystem(m_BoomEffect);
                    m_MinesPlaced++;
                }
            }
        }
    }


    private void ColorCalculation()
    {
        MS_Bomb[] bombBlockArray = GameObject.FindObjectsOfType<MS_Bomb>(); //Grabs all bombs
        
        for (var i = 0; i < m_AllCubes.Count; i++)
        {
            //Calculate and store the distance from bomb to each block.
            float distToClosestBomb = float.MaxValue;
            for (var j = 0; j < bombBlockArray.Length; j++)
            {
                float bombDist = Vector3.Distance(m_AllCubes[i].transform.position, bombBlockArray[j].transform.position);
                if(bombDist < distToClosestBomb)
                {
                    distToClosestBomb = bombDist;
                    m_AllCubes[i].GetComponent<MS_Block>().closestBomb = bombBlockArray[j].gameObject;
                    m_AllCubes[i].GetComponent<MS_Block>().distanceToBomb = bombDist;
                }
            }

            //Normalize distance to each bomb and add color gradient 
            //if (m_AllCubes[i].GetComponent<MS_Bomb>() == true)
            //{
                //m_AllCubes[i].GetComponent<MeshRenderer>().material.color = Color.red;
            //}
            //else
            //{
                float normalizedVal; //Distance value clamped to range of AlphaColor to BetaColor
                float AlphaColor = 0f; //Lower values = more red shifted all cubes are.
                float BetaColor = 1f; //Higher values = more blue shifted all cubes are.
                
                normalizedVal =  // result = (input - input.max) * ((output.min - output.max) / (output.min - input.max)) + output.max;
                    (m_AllCubes[i].GetComponent<MS_Block>().distanceToBomb - m_GameDiameter) *
                    ((AlphaColor - BetaColor) / (AlphaColor - m_GameDiameter)) + BetaColor;

                Color currentColor = Color.HSVToRGB(normalizedVal, 1f, 1f); //HSV Conversion, HSV uses 0-1 scale on single value to run through entire rainbow.

                //m_AllCubes[i].GetComponent<MeshRenderer>().material.color = currentColor; //Setting color
                m_AllCubes[i].GetComponent<MS_Block>().materialColor = currentColor; //Setting color value for control later.
            //}
        }
    }

    public void ResetPlacedMine()
    {
        m_MinesPlaced = 0;
    }

    public List<GameObject> GrabEffects()
    {
        return m_AllEffects;
    }
}
