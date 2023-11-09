using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosIsPlayerPos : MonoBehaviour
{
    // Start is called before the first frame update
    public Player Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position;
    }
}
