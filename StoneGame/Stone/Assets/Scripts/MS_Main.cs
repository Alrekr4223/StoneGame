using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Main : MonoBehaviour {

    public GameObject m_Player;
    [Space]
    public GameObject m_CubeHolder;
    public GameObject m_CubePref;
    public int m_cubeSize;
    [Space]
    public int m_BombAmount;
    public GameObject m_FlagHolder;
    public GameObject m_FlagPref;
    [Space]
    public Material m_ActiveCubeMat;
    public Material m_InactiveCubeMat;
    [Space]
    public GameObject m_MSText;
    public GameObject m_StartBtn;
    [Space]
    public GameObject m_ColorEffect;

    private GameObject m_CurrentBlock;
    private List<GameObject> m_AllCubes;
    private List<GameObject> m_RemovedCubes;
    private List<GameObject> m_AllColorEffects;
    private List<GameObject> m_Flags;
    private bool m_GameStarted = false;
    private bool m_ResetParticles = false;
    private float m_RotSpeed = 1;
    private int m_MinesFound = 0;
    


    void Start () {


        m_RemovedCubes = new List<GameObject>();
        m_AllColorEffects = new List<GameObject>();
        m_Flags = new List<GameObject>();
    }
	
	void Update () {


        if (FP_Raycast.Update() != null)
        {
            if (m_StartBtn == FP_Raycast.Update() && Input.GetButtonDown("Fire1")) //Start Game
            {
                m_GameStarted = true;       //Move forward in Update.
                m_MSText.SetActive(true);   //Overlay Text. Not currently avalible
                m_AllCubes = this.GetComponent<MS_CubeMaker>().MakeCubes(m_CubePref, m_CubeHolder, m_cubeSize, m_BombAmount); //Make Cubes

                MakeFlags();
                
                print("Amount of Cubes: " + m_AllCubes.Count);

                for (var i = 0; i < m_AllCubes.Count; i++)
                {
                    m_AllCubes[i].GetComponent<MS_Block>().arrayIndex = i;


                    GameObject PS = Instantiate(m_AllCubes[i].GetComponent<MS_Block>().particleEffect, m_CubeHolder.transform);
                    PS.name = "ParticleSystem for #" + i;
                    PS.GetComponent<ParticleSystem>().startColor = m_AllCubes[i].GetComponent<MS_Block>().materialColor;
                    PS.GetComponent<ParticleSystem>().Stop();
                    PS.transform.localPosition = m_AllCubes[i].transform.localPosition;
                    m_AllColorEffects.Add(PS);

                    m_AllCubes[i].GetComponent<MS_Block>().particleEffect = PS;

                }
            }
        }

        

        if (m_GameStarted) //Game has begun
        {
            if (FP_Raycast.Update() != null)
            {
                //Mouse To Block interaction
                if (FP_Raycast.Update().GetComponent<MS_Block>() != null)
                {
                    /* HOVER EFFECT
                    for (var i = 0; i < m_AllCubes.Count; i++)
                    {
                        if (m_AllCubes[i] != null)
                        {
                            //m_AllCubes[i].GetComponent<MeshRenderer>().material = m_InactiveCubeMat;
                            //need to set to original material instead.
                        }

                    }*/

                    GameObject activeBlock = FP_Raycast.Update().GetComponent<MS_Block>().gameObject;

                    //activeBlock.GetComponent<MeshRenderer>().material = m_ActiveCubeMat;
                    //Need to set to variant of original material instead. 

                    
                    bool foundBlock = false;

                    if (Input.GetButtonDown("Fire1"))
                    {
                        if(activeBlock.GetComponent<MS_Bomb>())
                        {
                            //TRIGGERED A BOMB!
                            activeBlock.GetComponent<MS_Block>().particleEffect.GetComponent<ParticleSystem>().Play();
                            StartCoroutine(ShowSolution());
                        }
                        m_CurrentBlock = activeBlock;
                        foundBlock = true;
                    }
                    if (Input.GetButton("Fire1") && foundBlock)
                    {
                        m_CurrentBlock.GetComponent<MeshRenderer>().material.color = activeBlock.GetComponent<MS_Block>().materialColor;
                    }
                    if (Input.GetButtonUp("Fire1"))
                    {
                        if (m_CurrentBlock != null && m_CurrentBlock.activeSelf)
                        {
                            m_CurrentBlock.SetActive(false);
                            m_RemovedCubes.Add(m_CurrentBlock);
                        }                        
                    }
                }


                //Mouse to Flag Interaction
                if (FP_Raycast.Update() != null && FP_Raycast.Update().GetComponent<MS_Flag>() != null)
                {
                    GameObject Flag = FP_Raycast.Update();

                    if (Input.GetButton("Fire1"))
                    {
                        //grabbed flag
                        Flag.GetComponent<Rigidbody>().isKinematic = true;
                        Flag.transform.parent = m_Player.GetComponentInChildren<Camera>().gameObject.transform;
                        

                    }
                    else
                    {
                        Flag.transform.parent = m_FlagHolder.transform;
                        if (Flag.GetComponent<Rigidbody>())
                        {
                            Flag.GetComponent<Rigidbody>().isKinematic = false;
                        }                        
                    }
                }
            }

            

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StartCoroutine(ShowParticles()); //Shows particles for X seconds, then turns off particles 
            }
            if (Input.GetKey(KeyCode.E)) //Rotates the game along the +X axis
            {
                Vector3 newRot = new Vector3(
                    m_CubeHolder.transform.rotation.x + m_RotSpeed, 
                    m_CubeHolder.transform.rotation.y, 
                    m_CubeHolder.transform.rotation.z);
                m_CubeHolder.transform.rotation = Quaternion.Euler(newRot);
            }

            if (Input.GetKey(KeyCode.Q)) //Rotates the game along the -X axis
            {
                Vector3 newRot = new Vector3(
                    m_CubeHolder.transform.rotation.x - m_RotSpeed,
                    m_CubeHolder.transform.rotation.y,
                    m_CubeHolder.transform.rotation.z);
                m_CubeHolder.transform.rotation = Quaternion.Euler(newRot);
            }

            if (Input.GetKeyDown(KeyCode.F)) //End Game
            {
                EndGame();
            }

            if (Input.GetKeyDown(KeyCode.U)){
                ShowTempSolution();
            }
        }
    }



    private void MakeFlags()
    {
        float flagOffset = 0.5f;

        for (var i = 0; i < m_BombAmount; i++)
        {
            GameObject currentFlag = Instantiate(m_FlagPref, m_FlagHolder.transform);
            currentFlag.name = "Flag #" + i;
            currentFlag.transform.localPosition = new Vector3(0, 0, i * flagOffset * -1);
            m_Flags.Add(currentFlag);
        }        

    }




    private IEnumerator ShowParticles()
    {
        for (var i = 0; i < m_RemovedCubes.Count; i++) //Turn On Particles 
        {
            m_RemovedCubes[i].GetComponent<MS_Block>().particleEffect.GetComponent<ParticleSystem>().Play();
        }

        yield return new WaitForSeconds(2); //wait
        
        for (var i = 0; i < m_RemovedCubes.Count; i++) //Turn off Particles
        {
            m_RemovedCubes[i].GetComponent<MS_Block>().particleEffect.GetComponent<ParticleSystem>().Stop();
        }
    }


    private IEnumerator ShowSolution()
    {
        StartCoroutine(ShowParticles());

        for(var i = 0; i < m_AllCubes.Count; i++)
        {
            m_AllCubes[i].GetComponent<MeshRenderer>().material.color = m_AllCubes[i].GetComponent<MS_Block>().materialColor;
        }

        yield return new WaitForSeconds(5);
        EndGame();
    }

    private void ShowTempSolution() //cheats
    {
        for (var i = 0; i < m_AllCubes.Count; i++)
        {
            m_AllCubes[i].GetComponent<MeshRenderer>().material.color = m_AllCubes[i].GetComponent<MS_Block>().materialColor;
        }
    }


    private void EndGame()
    {
        m_MSText.SetActive(false);
        this.GetComponent<MS_CubeMaker>().ResetPlacedMine();
        foreach (GameObject obj in m_AllCubes)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in m_RemovedCubes)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in m_AllColorEffects)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in m_Flags)
        {
            Destroy(obj);
        }
        m_Flags.Clear();
        m_AllCubes.Clear();
        m_RemovedCubes.Clear();
        m_AllColorEffects.Clear();
    }





    public void FlagFoundBomb()
    {
        m_MinesFound++;

        if (m_MinesFound == m_BombAmount)
        {
            print("YOU WON THE GAME");
        }
    }
}
