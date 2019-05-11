using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{

    public GameObject NearMinion;
    public GameObject LongDistanceMinion;
    public bool isGameEnd = false;

    #region Singleton

    public static MinionManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
