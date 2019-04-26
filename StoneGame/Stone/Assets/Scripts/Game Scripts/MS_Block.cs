using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Block : MonoBehaviour {

    public enum BlockType { None, Mine, Flag, One, Two, Three, Four, Five, Six, Seven, EightorMore }
    BlockType currentType;

    [HideInInspector]
    public float distanceToBomb;
    [HideInInspector]
    public GameObject closestBomb;
    [HideInInspector]
    public Color materialColor;
    [HideInInspector]
    public GameObject particleEffect;

    public int arrayIndex;



    



    // Use this for initialization
    void Start () {
        currentType = BlockType.None;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetBlockType(BlockType type)
    {
        switch (type)
        {
            case BlockType.Flag:
                currentType = BlockType.Flag;
                break;
            case BlockType.Mine:
                currentType = BlockType.Mine;
                break;
            case BlockType.One:
                currentType = BlockType.One;
                break;
            case BlockType.Two:
                currentType = BlockType.Two;
                break;
            case BlockType.Three:
                currentType = BlockType.Three;
                break;
            case BlockType.Four:
                currentType = BlockType.Four;
                break;
            default:
                break;

        }
    }

    public BlockType GetBlockType()
    {
        return currentType;
    }


    public void SetParticleColor(Color color)
    {
        materialColor = color;
    }

    public void SetParticleSystem(GameObject system)
    {
        particleEffect = system;
    }

    public void RunParticleSystem()
    {
        particleEffect.GetComponent<ParticleSystem>().Play();
    }
}
