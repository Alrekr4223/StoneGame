using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Main : MonoBehaviour {

    public GameObject m_Player;
    [Space]
    public GameObject m_CubePrefab;
    public Material m_idleCubeMat;
    public Material m_hoverCubeMat;
    //public GameObject m_FlagPref;
    public int m_Gamesize = 5;
    public int m_BombAmount = 1;
    public float m_RotationSpeed = 5;

    private GameObject m_CurrentBlock;
    private GameObject m_AllCubesHolder;
    private List<GameObject> m_AllCubes;
    private List<GameObject> m_RemovedCubes;
    private List<GameObject> m_AllColorEffects;
    private List<GameObject> m_Flags;
    private bool m_GameStarted = false;
    private bool m_ResetParticles = false;
    private bool m_GameEnded = false;
    private float m_RotSpeed = 1;
    private int m_MinesFound = 0;

    private Vector2 mouseStartDragPos;
    private Vector2 mouseCursDragPos;
    private bool mouseDragging = false;



    void Start () {
        m_RemovedCubes = new List<GameObject>();
        m_AllColorEffects = new List<GameObject>();
        m_Flags = new List<GameObject>();

        m_AllCubesHolder = new GameObject();
        m_AllCubesHolder.transform.parent = this.transform;
        m_AllCubesHolder.name = "Cube Holder";

        Camera.main.transform.LookAt(this.transform);
    }

    private void StartGame()
    {
        m_GameStarted = true;       //Move forward in Update.

        m_AllCubesHolder.transform.localPosition = Vector3.zero; //Reset holder back to zero, for when game restarts
        m_AllCubes = this.GetComponent<MS_CubeMaker>().MakeCubes(m_CubePrefab, m_AllCubesHolder, m_Gamesize, m_BombAmount); //Make Cubes
        float width = this.GetComponent<MS_CubeMaker>().GetCubeHolderBoundsExtents(); //Grab the size of the cubes as a whole
        Vector3 cur = m_AllCubesHolder.transform.localPosition; //easier to read cur in below line than the whole.
        m_AllCubesHolder.transform.localPosition = new Vector3(cur.x - width, cur.y - width, cur.z - width);  // Reset holder position to center of parent

        //MakeFlags();

        print("Amount of Cubes: " + m_AllCubes.Count);

        for (var i = 0; i < m_AllCubes.Count; i++)
        {
            m_AllCubes[i].GetComponent<MS_Block>().arrayIndex = i;

            m_AllCubes[i].tag = "GameCube";


            GameObject PS = Instantiate(m_AllCubes[i].GetComponent<MS_Block>().particleEffect, m_AllCubesHolder.transform);
            PS.name = "ParticleSystem for #" + i;
            PS.GetComponent<ParticleSystem>().startColor = m_AllCubes[i].GetComponent<MS_Block>().materialColor;
            PS.GetComponent<ParticleSystem>().Stop();
            PS.transform.localPosition = m_AllCubes[i].transform.localPosition;
            m_AllColorEffects.Add(PS);

            m_AllCubes[i].GetComponent<MS_Block>().particleEffect = PS;

        }
    }

    void Update()
    {

        //Run Game if game isn't created yet.
        if (!m_GameStarted)
        {
            Debug.Log("Starting Game");
            StartGame();
        }


        RotateGame();



        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowParticles(); //Shows particles for X seconds, then turns off particles 
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Showing Solution");
            ShowObjectSolution(); //Cheats. Shows solution to game by swapping all objects to their color values.
            m_GameEnded = true;
        }
        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << 9; //Bitmasking layer 9, which is the layer for the audio trigger boxes.
        layerMask = ~layerMask; //inverting the mask so the raycast affects everything except layer 9.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.gameObject.tag == "GameCube")
            {
                FindObjectOfType<MS_Main>().RaycastCube(hit.transform.gameObject);
            }
        }


        this.GetComponent<ChuckConversions>().SetAllCubes(m_AllCubes);
    }



    public void ChunityStrum(float note, float length) //TODO, send in color value to SelectionFX().
    {


        Debug.Log("Note: " + note + ", length: " + length);
        //Chuck + Unity -> Chunity, Interactive Audio
        this.GetComponent<ChuckSubInstance>().RunCode( string.Format(@"

            Mandolin m => dac;
            
            //Dorian Scale
            [0,0,2,3,3,5,5,7,7,9,9,10] @=> int Dorian[];

            //Lydian
            [0,2,4,6,7,9,11,12,14,16,18,19]  @=> int Lydian[];
            
            {0} $ int => int MidiCasted;
            MidiCasted % 12 => int MidiModulus;
            MidiCasted / 12 => int octave;
            
            Lydian[MidiModulus] => int ScaleNote;
            ScaleNote + (12*octave) => int OctiveNote;

            Std.mtof(OctiveNote) => float Freq;
            Freq $ int => int NoteToPlay;

            function void strumMandolin ( float freq, float detune, float body, float pluckpos, float length )
            {{
                freq => m.freq;
                detune => m.stringDetune;
                body => m.bodySize;
                pluckpos => m.pluckPos;  //good to change

                0.4 => m.noteOn;

                length::second => now;
            }}

            function void SelectionFX(float masterNote, float length){{

                strumMandolin (masterNote, 0, 0.25, 0.5, length);
            }}


            SelectionFX(NoteToPlay, {1});

        ", note, length));
    }



    public void ChunityFAIL() //TODO, send in color value to SelectionFX().
    {
        //Chuck + Unity -> Chunity, Interactive Audio
        this.GetComponent<ChuckSubInstance>().RunCode(@"

            Mandolin m => dac;
            

            function void strumMandolin ( float freq, float detune, float body, float pluckpos, float length )
            {{
                freq => m.freq;
                detune => m.stringDetune;
                body => m.bodySize;
                pluckpos => m.pluckPos;  //good to change

                1.0 => m.noteOn;

                length::second => now;
            }}

            function void SelectionFX(float masterNote, float length){{

                strumMandolin (masterNote, 1, 0.25, 0.5, length);
                strumMandolin (masterNote - 19, 1, 0.25, 0.5, length);
                strumMandolin (masterNote - 61, 1, 0.25, 0.5, length + 0.8);
            }}


            SelectionFX(220, 0.3);

        ");
    }


    public void RaycastCube(GameObject cube)
    {
        
        for (var i = 0; i < m_AllCubes.Count; i++)
        {
            if(cube == m_AllCubes[i])
            {
                if (!m_GameEnded)
                {
                    m_AllCubes[i].GetComponent<MeshRenderer>().material = m_hoverCubeMat; // HOVER EFFECT
                }

                if (Input.GetKeyDown(KeyCode.Space)) //Set 'Flag'
                {
                    Debug.Log("Setting Flag");
                }
                

                if (Input.GetButtonUp("Fire1")) //Click on cube
                {
                    

                    if (m_AllCubes[i].GetComponent<MS_Bomb>())
                    {
                        //TRIGGERED A BOMB!
                        m_AllCubes[i].GetComponent<MS_Block>().RunParticleSystem();
                        StartCoroutine(ShowSolutionAndEnd());
                        m_AllCubes[i].SetActive(false); //Turn off cube

                        ChunityFAIL();
                        return;
                    }

                    m_AllCubes[i].GetComponent<MeshRenderer>().material.color = cube.GetComponent<MS_Block>().materialColor;
                    //Debug.Log("Clicked cube: " + cube.name + ", set color: " + m_AllCubes[i].GetComponent<MeshRenderer>().material.color);

                    m_AllCubes[i].GetComponent<MS_Block>().RunParticleSystem(); //Activate particle 'fairy' 

                    m_AllCubes[i].SetActive(false); //Turn off cube
                    m_RemovedCubes.Add(m_AllCubes[i]); //send cube to array containing all cubes that have been turned off.

                    //Make noise on object clicked
                    float note = this.GetComponent<ChuckConversions>().ColorToFreqency(m_AllCubes[i].GetComponent<MS_Block>().distanceToBomb);
                    ChunityStrum(note, 1.2f);
                }
            }
            else //Set all other active cubes to reset material after hovered and not clicked
            {
                if (!m_GameEnded)
                {
                    m_AllCubes[i].GetComponent<MeshRenderer>().material = m_idleCubeMat;
                }
                
            }
        }
    }


    private void RotateGame()
    {

        if (Input.GetButtonDown("Fire2"))
        {
            mouseDragging = true;
            mouseStartDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            mouseDragging = false;
        }

        if (mouseDragging)
        {
            mouseCursDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dist = mouseCursDragPos - mouseStartDragPos;

        }
    }

    

    private void ShowParticles()
    {
        for (var i = 0; i < m_RemovedCubes.Count; i++) //Turn On Particles 
        {
            m_RemovedCubes[i].GetComponent<MS_Block>().particleEffect.GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator ShowSolutionAndEnd()
    {
        ShowParticles(); //Show already clicked on objects

        ShowObjectSolution(); //Show color values of objects that have not been clicked

        yield return new WaitForSeconds(5);
        EndGame();
    }

    private void ShowObjectSolution() //Paints objects based on their color values. Used at end of game
    {
        for (var i = 0; i < m_AllCubes.Count; i++)
        {
            m_AllCubes[i].GetComponent<MeshRenderer>().material.color = m_AllCubes[i].GetComponent<MS_Block>().materialColor;
        }
    }

    private void EndGame()
    {
        m_GameEnded = true;
        m_GameStarted = false;
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

    public int GetGameSize()
    {
        return m_Gamesize;
    }
}
