using UnityEngine;
using System.Collections;

/**
 * This class will be used as parent for Coin and Key
 * Gives objects ability to be collected with poof and sound, also adds object rotation
 */
public class Collectible : MonoBehaviour {

    protected bool _isCollected = false;
    protected Vector3 _rotationVector = new Vector3(0f, 1f, 0f);

    // public settings
    public GameObject poof;
    public float rotationSpeed = 150.0f;

    [Header("Sounds")]
    public AudioClip collect_sound = null;

    protected void Update()
    {
        // rotate object
        gameObject.transform.Rotate(_rotationVector * rotationSpeed * Time.deltaTime);
    }

    public void Collect()
    {
        // Create poof
        GameObject poofInstance = (GameObject)Object.Instantiate(poof, gameObject.transform.position, poof.transform.rotation);
        
        // Play sounds using poof's audio source (because our original object will be destroyed)
        AudioSource poofSound = poofInstance.GetComponent<AudioSource>();
        poofSound.clip = collect_sound;
        poofSound.Play();
        
        // destroy collectible
        Object.Destroy(gameObject);

        // set collected flag
        _isCollected = true;
    }

    public bool isCollected()
    {
        return _isCollected;
    }

    // Use this for initialization
    void Start () {
	
	}
	
}
