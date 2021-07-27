using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNav : MonoBehaviour {

	// Use this for initialization
	void Start () {

        
         GetComponent<NavMeshAgent>().
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(3, 0, 3));

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
