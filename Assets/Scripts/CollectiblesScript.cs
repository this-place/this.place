using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour {
    public int optionalNumber = 0;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
