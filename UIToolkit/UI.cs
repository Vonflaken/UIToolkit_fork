using UnityEngine;
using System.Collections;


public class UI : MonoBehaviour
{
	// All access should go through instance and you are trusted not to set it :)
	static public UI instance = null;
	static public UIToolkit firstToolkit = null; // we stick the first available toolkit so when using one atlas the API remains simple
	
	public int drawDepth = 100;	
	public LayerMask UILayer = 0;
	[HideInInspector]
	public int layer;
	
	private AudioSource _audioSource;
	private Camera _uiCamera;
	private GameObject _uiCameraHolder;
	private UIToolkit[] _toolkitInstances;
	
	// if set to true, the texture will be chosen and loaded from textureName
	// the base texture to use. If HD/retina, textureName2x will be loaded.  Both need to be in Resources.
	// this also doubles as the name of the json config file generated by Texture Packer
	public bool autoTextureSelectionForHD = true;
	public bool allowPod4GenHD = true; // if false, iPod touch 4G will not take part in HD mode
	public int maxWidthOrHeightForSD = 960; // if the width/height of the screen equals or exceeds this size HD mode will be triggered
	public int maxWidthOrHeightForHD = 2048;
	
	
	public static bool isHD = false;
	public static int scaleFactor = 1; // we use int's for ease of life. this allows enough flexibility (1x, 2x, 3x, etc) and keeps things simple
	public string hdExtension = "2x";
	
	
	public Camera uiCamera { get { return _uiCamera; } }
	

	#region Unity MonoBehaviour Functions
	
	private void Awake()
	{
		// Set instance to this so we can be accessed from anywhere
		instance = this;
		
		// add the audio source if we dont have one and cache it
		_audioSource = GetComponent<AudioSource>();
		if( _audioSource == null )
			_audioSource = gameObject.AddComponent<AudioSource>();

		// Create the camera
		_uiCameraHolder = new GameObject();
		_uiCameraHolder.transform.parent = gameObject.transform;
		_uiCameraHolder.AddComponent( "Camera" );
		
		_uiCamera = _uiCameraHolder.camera;
		_uiCamera.name = "UICamera";
		_uiCamera.clearFlags = CameraClearFlags.Depth;
		_uiCamera.nearClipPlane = 0.3f;
		_uiCamera.farClipPlane = 50.0f;
		_uiCamera.depth = drawDepth;
		_uiCamera.rect = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
		_uiCamera.orthographic = true;
		_uiCamera.orthographicSize = Screen.height / 2;

		// Set the camera position based on the screenResolution/orientation
		_uiCamera.transform.position = new Vector3( Screen.width / 2, -Screen.height / 2, -10.0f );
		_uiCamera.cullingMask = UILayer;
		
		// Cache the layer for later use when adding sprites
		// UILayer.value is a mask, find which bit is set 
		for( int i = 0; i < sizeof( int ) * 8; i++ )
		{
			if( ( UILayer.value & (1 << i) ) == (1 << i) )
			{
				layer = i;
				break;
			}
		}
		
		// setup the HD flag
		// handle texture loading if required
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_ANDROID || UNITY_STANDALONE_LINUX
		var deviceAllowsHD = true;
#else
		var deviceAllowsHD = ( allowPod4GenHD && iPhone.generation == iPhoneGeneration.iPodTouch4Gen ) || iPhone.generation != iPhoneGeneration.iPodTouch4Gen;
#endif
		if( autoTextureSelectionForHD && deviceAllowsHD )
		{
			// are we loading up a 4x texture?
			if( Screen.width >= maxWidthOrHeightForHD || Screen.height >= maxWidthOrHeightForHD )
			{
#if UNITY_EDITOR
				Debug.Log( "switching to 4x GUI texture" );
#endif
				isHD = true;
				scaleFactor = 4;
				hdExtension = "4x";
			}
			// are we loading up a 2x texture?
			else if( Screen.width >= maxWidthOrHeightForSD || Screen.height >= maxWidthOrHeightForSD )
			{
#if UNITY_EDITOR
				Debug.Log( "switching to 2x GUI texture" );
#endif
				isHD = true;
				scaleFactor = 2;
			}
		}
		
		// grab all our child UIToolkits
		_toolkitInstances = GetComponentsInChildren<UIToolkit>();
		firstToolkit = _toolkitInstances[0];
#if UNITY_EDITOR
		if( _toolkitInstances.Length == 0 )
			throw new System.Exception( "Could not find any UIToolkit instances in children of UI" );
#endif
		
		// kick off the loading of texture for any UIToolkits we have
		foreach( var toolkit in _toolkitInstances )
			toolkit.loadTextureAndPrepareForUse();
	}
	
	
	// Ensures that the instance is destroyed when the game is stopped
	protected void OnApplicationQuit()
	{
		instance = null;
		firstToolkit = null;
	}
	
	
	protected void OnDestroy()
	{
		instance = null;
		firstToolkit = null;
	}

	#endregion


	public void playSound( AudioClip clip )
	{
		_audioSource.PlayOneShot( clip );
	}

	/***************************************************************************************************/
	/*                                     CUSTOM IMPLEMENTATION                                       */
	/***************************************************************************************************/

	/// <summary>
	/// Gets the user interface toolkit.
	/// </summary>
	/// <returns>The user interface toolkit.</returns>
	/// <param name="atlasFilename">Atlas filename.</param>
	public UIToolkit GetUIToolkit( string atlasFilename )
	{
		foreach ( UIToolkit toolkit in _toolkitInstances )
		{
			if ( toolkit.GetAtlasCgiFilename() == atlasFilename )
			{
				return toolkit;
			}
		}

		return null;
	}

	/// <summary>
	/// Gets the toolkits.
	/// </summary>
	/// <returns>The toolkits.</returns>
	public UIToolkit[] GetToolkits()
	{
		return _toolkitInstances;
	}

	/// <summary>
	/// Determines whether this instance is touched.
	/// </summary>
	/// <returns><c>true</c> if this instance is touched; otherwise, <c>false</c>.</returns>
	public bool IsPointerHoverUI()
	{
		bool isHovered = false;
		foreach ( var toolkit in _toolkitInstances )
		{
			isHovered = isHovered || toolkit.Hovered;
			if ( isHovered )
				return isHovered; // For perf
		}

		return isHovered;
	}

}
