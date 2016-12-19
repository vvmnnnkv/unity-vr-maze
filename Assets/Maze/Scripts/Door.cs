using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour 
{
    private enum State
    {
        Closed,
        Opening,
        Closing,
        Opened
    }

    [Header("Sounds")]
    public AudioClip open_fail_sound = null;
    public AudioClip open_success_sound = null;

    private bool locked = true;
    private State _state = State.Closed;
    private float _slide_speed = 2.0f;
    private float _opened_position_offset = 7.0f;
    private Vector3 _initial_position;
    private Vector3 _slide_vector = new Vector3(0f, 1f, 0f);

    void Awake() {
        // save initial door position
        _initial_position = gameObject.transform.position;
    }

    void Update() {
        switch (_state)
        {
            case State.Closed:
                break;

            case State.Opening:
                if (gameObject.transform.position.y < _initial_position.y + _opened_position_offset)
                {
                    // slide the door to open
                    gameObject.transform.position = gameObject.transform.position + _slide_vector * _slide_speed * Time.deltaTime;
                } else
                {
                    _state = State.Opened;
                }
                break;

            case State.Closing:
                if (gameObject.transform.position.y > _initial_position.y)
                {
                    // slide the door to open
                    gameObject.transform.position = gameObject.transform.position - _slide_vector * _slide_speed * Time.deltaTime;
                }
                else
                {
                    _state = State.Opened;
                }
                break;

            case State.Opened:
                break;
        }
    }

    /**
     * Method that opens the door
     */
    public void Open()
    {
        // only affects closed door
        if (_state == State.Closed && !locked)
        {
            // start opening
            PlaySound(open_success_sound);
            _state = State.Opening;
        } else {
            // cannot open
            PlaySound(open_fail_sound);
        }
    }

    /**
     * Method that closes the door
     */
    public void Close()
    {
        // only affects opened or opening door
        if (_state == State.Opened || _state == State.Opening)
        {
            // start closing
            PlaySound(open_success_sound);
            _state = State.Closing;
        }
    }

    /**
     * Unlock the door
     */
    public void Unlock()
    {
        locked = false;
    }

    /**
     * Utility method for convenience
     */
    protected void PlaySound(AudioClip clip)
    {
        AudioSource player = gameObject.GetComponent<AudioSource>();
        player.clip = clip;
        player.Play();
    }

}
