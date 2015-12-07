using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DFTGames.Tools;

/// <summary>
/// Demo game manager. This is the place where
/// we create the pools we need
/// </summary>
public class DemoGameManager : MonoBehaviour {

	/// <summary>
    /// The sphere prefab to be pooled
    /// </summary>
	public GameObject spherePrefab;
    /// <summary>
    /// The cube prefab to be pooled
    /// </summary>
    public GameObject cubePrefab;
    /// <summary>
    /// The capsule prefab to be pooled
    /// </summary>
    public GameObject capsulePrefab;


    // GUI Text elements to sho counters
    public Text cubesInUse;
    public Text cubesAvailable;
    public Text spheresInUse;
    public Text spheresAvailable;
    public Text capsulesInUse;
    public Text capsulesAvailable;
	
// We treat the first two as GameObjects
	/// <summary>
	/// The spheres' pool.
	/// </summary>
	private PoolingManager<GameObject> spheres = null;

    /// <summary>
    /// The cubes' pool.
    /// </summary>
    private PoolingManager<GameObject> cubes = null;

    // And we pool the capsules as Rigidbody 
    // to see how the typed pooling works. The sole
    // constraint is that the component used to
    // pool the objects must be in the hierarchy
    // at the first level, not in any children in the 
    // given prefab.
    /// <summary>
    /// The capsules' pool.
    /// </summary>
    private PoolingManager<Rigidbody> capsules = null;
	
	// Spawning counter
	private float counter = 0f;
	// Temporary GameObject holder
	private GameObject tmpObj = null;
	// Temporary Rigidbody holder
	private Rigidbody tmpRB = null;

	// Use this for initialization
	void Start () {
		// Initialise the spheres' pool
		spheres = new PoolingManager<GameObject>(spherePrefab);
		// Initialise the cubes' pool
		cubes = new PoolingManager<GameObject>(cubePrefab);	
		// Initialise the capsules' pool
		capsules = new PoolingManager<Rigidbody>(capsulePrefab);
	}
	
	// Update is called once per frame
	void Update () {
        cubesInUse.text = cubes.InUse.ToString();
        cubesAvailable.text = cubes.Available.ToString();
        spheresInUse.text = spheres.InUse.ToString();
        spheresAvailable.text = spheres.Available.ToString();
        capsulesInUse.text = capsules.InUse.ToString();
        capsulesAvailable.text = capsules.Available.ToString();
        // Let's spawn at random every 75ms
		counter += Time.deltaTime;
		if (counter >= 0.075f)
		{
			counter = 0f;
			// Get an element from the pools
			int tmpInt = Random.Range(1, 10000);
			if ((tmpInt % 2) == 0)
				tmpObj = spheres.GetElement ();
			else if ((tmpInt % 3) == 0)
				tmpObj = cubes.GetElement ();
			else 
			{
				tmpRB = capsules.GetElement ();
				tmpObj = tmpRB.gameObject;
			}
			// Position the element
			tmpObj.transform.position = new Vector3(Random.Range(-6f, 6f), Random.Range(10f, 14f), Random.Range(-6f, 6f));
		}
        // Let's optimize our pools
        cubes.Optimize();
        spheres.Optimize();
        capsules.Optimize();
    }
}
