using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
	private enum State
	{
		Idle,
		Focused,
		Clicked,
		Approaching,
		Moving,
		Collect,
		Collected,
		Occupied,
		Open,
		Hidden
	}

	[SerializeField]
	private State  		_state					= State.Idle;
	private Color		_color_origional		= new Color(0.0f, 1.0f, 0.0f, 0.5f);
	private Color		_color					= Color.white;
	private float 		_scale					= 1.0f;
    private float 		_displacement			= 0.0f;
    private Vector3     _displacementVector     = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3     _rotationVector         = new Vector3(0.0f, 1.0f, 0.0f);
    private float 		_animated_lerp			= 1.0f;
	private AudioSource _audio_source			= null;
	private Material	_material				= null;
    private Vector3 _initial_position;
    // camera should be slightly above
    private Vector3 _cameraDisplacement = new Vector3(0f, 0.7f, 0f);
    private GameObject cam;

    [Header("Material")]
	public Material	material					= null;
	public Color color_hilight					= new Color(0.8f, 0.8f, 1.0f, 0.125f);	
	
	[Header("State Blend Speeds")]
	public float lerp_idle 						= 0.0f;
	public float lerp_focus 					= 0.0f;
	public float lerp_hide						= 0.0f;
	public float lerp_clicked					= 0.0f;
	
	[Header("State Animation Scales")]
	public float scale_clicked_max				= 0.0f;
	public float scale_animation				= 3.0f;	
	public float scale_idle_min 				= 0.0f;
	public float scale_idle_max 				= 0.0f;
	public float scale_focus_min				= 0.0f;
	public float scale_focus_max				= 0.0f;
    public float displacement_idle_min = -2f;
    public float displacement_idle_max = 2f;

    [Header("Misc")]
    public float rotation_speed = 2f;
    
    [Header("Sounds")]
	public AudioClip clip_click					= null;				

	[Header("Hide Distance")]
	public float threshold						= 0.125f;



	void Awake()
	{		
		_material					= Instantiate(material);
		_color_origional			= _material.color;
		_color						= _color_origional;
		_audio_source				= gameObject.GetComponent<AudioSource>();	
		_audio_source.clip		 	= clip_click;
		_audio_source.playOnAwake 	= false;
        _initial_position = gameObject.transform.position;
        // modifying camera's position does not work on Android for some reason (but works in unity)
        // so i had to wrap camera in empty object and move this object
        cam = GameObject.Find("Camera Holder");
    }


	void Update()
	{
		bool occupied = cam.transform.position == (_initial_position + _cameraDisplacement);
		
		switch(_state)
		{
			case State.Idle:
				Idle();
				
				_state 		= occupied ? State.Occupied : _state;
				break;

			case State.Focused:
				Focus();
				break;

			case State.Clicked:
				Clicked();
                bool scaled = _scale >= scale_clicked_max * .95f;
                _state 		= scaled ? State.Approaching : _state;
				break;

			case State.Approaching:
                Hide();
                Approach();
				_state 		= occupied ? State.Occupied : _state;
				break;

            case State.Occupied:
				Hide();
				_state = !occupied ? State.Idle : _state;
				break;
			
			case State.Hidden:
				Hide();
				break;

			default:
				break;
		}

		gameObject.GetComponentInChildren<MeshRenderer>().material.color 	= _color;
		gameObject.transform.localScale 									= Vector3.one * _scale;
        _animated_lerp														= Mathf.Abs(Mathf.Cos(Time.time * scale_animation));

        // levitate
        gameObject.transform.position = _initial_position + _displacementVector * _displacement;

        // rotate waypoint object
        gameObject.transform.Rotate(Time.deltaTime * rotation_speed * _rotationVector);
    }


    public void Enter()
	{
		_state = _state == State.Idle ? State.Focused : _state;
	}


	public void Exit()
	{
        // only go idle from focued state, to avoid non-working clicks
        _state = _state == State.Focused ? State.Idle : _state;
    }

    public void Approach()
    {
        cam.transform.localPosition = _initial_position + _cameraDisplacement;
        cam.transform.rotation = Quaternion.identity;
    }

    public void Click()
	{
		_state = State.Clicked;
        _audio_source.Play();
    }


    private void Idle()
	{
		float scale				= Mathf.Lerp(scale_idle_min, scale_idle_max, _animated_lerp);
        float displacement      = Mathf.Lerp(displacement_idle_min, displacement_idle_max, _animated_lerp);
        Color color				= Color.Lerp(_color_origional, 	  color_hilight, _animated_lerp);

		_scale					= Mathf.Lerp(_scale, scale, lerp_idle);
		_color					= Color.Lerp(_color, color, lerp_idle);
        _displacement           = Mathf.Lerp(_displacement, displacement, lerp_idle);
    }


	public void Focus()
	{
		float scale				= Mathf.Lerp(scale_focus_min, scale_focus_max, _animated_lerp);
        float displacement = Mathf.Lerp(displacement_idle_min, displacement_idle_max, _animated_lerp);
        Color color				= Color.Lerp(   _color_origional,   color_hilight, _animated_lerp);

		_scale					= Mathf.Lerp(_scale, scale, lerp_focus);
		_color					= Color.Lerp(_color, color,	lerp_focus);
        _displacement = Mathf.Lerp(_displacement, displacement, lerp_focus);
    }


	public void Clicked()
	{	
		_scale					= Mathf.Lerp(_scale, scale_clicked_max, lerp_clicked);
        _color					= Color.Lerp(_color,     color_hilight, lerp_clicked);
	}


	public void Hide()
	{
        _scale = Mathf.Lerp(_scale, 		0.0f, lerp_hide);
        _color = Color.Lerp(_color, Color.clear, lerp_hide);
	}
}
