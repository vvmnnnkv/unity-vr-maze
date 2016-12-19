using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Crystal : MonoBehaviour {

    private Vector3 _rotationVector = Vector3.forward;
    private float _rotationSpeed = 50f;
    private float _blinkSpeed = 0.2f;
    private Color _emissionColor;
    private AudioSource _audio;
    private bool _touched = false;
    private bool _hide_dialog = true;
    private bool _teleporting = false;

    public GameObject speechBubble;

    [Header("Sounds")]
    public AudioClip teleport_sound;

    [Header("Dialogs")]
    public GameObject dialog_hi;
    public GameObject dialog_teleport;
    public GameObject dialog_end;
    public GameObject dialog_fail;

    // Use this for initialization
    void Start () {
        // save material emission color
        _emissionColor = gameObject.GetComponentInChildren<MeshRenderer>().material.GetColor("_EmissionColor");
        
        // initially, do not allow to click on crystal
        // player needs to enter the temple
        gameObject.GetComponent<MeshCollider>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        // rotate
        gameObject.transform.Rotate(_rotationVector * _rotationSpeed * Time.deltaTime);

        // change emission color
        float emission = Mathf.PingPong(Time.time * _blinkSpeed, 1.0f);
        gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", _emissionColor * Mathf.LinearToGammaSpace(emission));

        if (_touched) {
            // size pulsation
            gameObject.transform.localScale = Vector3.one + 0.2f * Vector3.one * Mathf.Abs(Mathf.Sin(Time.time * 3f));
        }

        // show dialogs after greetings audio is played
        if (!_hide_dialog && !_audio.isPlaying) {
            speechBubble.GetComponent<Canvas>().enabled = true;
        }

        // show oops dialog after teleport audio is played
        if (_teleporting && !_audio.isPlaying) {
            dialog_end.SetActive(true);
            _teleporting = false;
        }

        // vibrate while teleporting
        if (_teleporting)
        {
            Handheld.Vibrate();
        }
    }

    public void Awake()
    {
        speechBubble.GetComponent<Canvas>().enabled = false;
        _audio = gameObject.GetComponent<AudioSource>();

        // init state values
        _hide_dialog = true;
        _touched = false;

        // init dialogs
        dialog_hi.SetActive(true);
        dialog_teleport.SetActive(false);
        dialog_end.SetActive(false);
        dialog_fail.SetActive(false);
    }

    public void Touch()
    {
        // play sound and then show dialog
        _audio.Play();
        _hide_dialog = false;
        _touched = true;
    }

    public void TakeCoins()
    {
        dialog_hi.SetActive(false);
        // check if coins collected
        if (GameObject.FindGameObjectsWithTag("Coin").Length == 0) {
            // all coins collected
            dialog_teleport.SetActive(true);
        } else {
            dialog_fail.SetActive(true);
        }
    }

    public void Teleport()
    {
        dialog_teleport.SetActive(false);
        // play sound and then show end dialog
        _audio.clip = teleport_sound;
        _audio.Play();
        _teleporting = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    // called when player enters temple
    public void MakeClickable()
    {
        gameObject.GetComponent<MeshCollider>().enabled = true;
    }
}
