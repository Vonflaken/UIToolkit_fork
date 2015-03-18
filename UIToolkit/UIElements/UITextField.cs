using UnityEngine;
using System;


public class UITextField : UITouchableSprite
{
	protected UITextInstance _textInstance;

	protected string _text = "";
	protected int _margin = 3; // In pixels
	protected float currentLineWidth = 0f;
	
	public UIUVRect highlightedUVframe;
	
	
	public Action<UITextField,string> onInput;
	public Action<UITextField> onEnter;

	public string text
	{
		get { return _text; }
		set
		{
			if ( _text != value )
			{
				_text = value;
				_textInstance.text = _text;
			}
		}
	}
	
	public int margin
	{
		get { return _margin; }
		set
		{
			if ( _margin != value )
			{
				_margin = value;
				_textInstance.pixelsFromTopLeft( _margin, _margin );
			}
		}
	}
	
	
	#region Constructors/Destructor
	
	public static UITextField create( string filename, string highlightedFilename, UITextInstance textInstance, int xPos, int yPos )
	{
		return UITextField.create( UI.firstToolkit, filename, highlightedFilename, textInstance, xPos, yPos );
	}
	
	
	public static UITextField create( UIToolkit manager, string filename, string highlightedFilename, UITextInstance textInstance, int xPos, int yPos )
	{
		return UITextField.create( manager, filename, highlightedFilename, textInstance, xPos, yPos, 1 );
	}
	
	public static UITextField create( UIToolkit manager, string filename, string highlightedFilename, UITextInstance textInstance, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UITextField( manager, filename, frame, textInstance, depth, normalTI.uvRect, highlightedTI.uvRect );
	}


	public UITextField( UIToolkit manager, string filename, Rect frame, UITextInstance textInstance, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame )
	{
		_textInstance = textInstance;
		_textInstance.parentUIObject = this;
		_textInstance.depth = depth - 1; // Text must be in top of background
		_textInstance.pixelsFromTopLeft( _margin, _margin ); // minimum margin
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
		
		manager.addTouchableSprite( this, filename );
	}
	
	#endregion;
	
	
	public override void onKeyboardEntry( string inputString, string deltaInputString )
	{
		if ( inputString.Length > text.Length )
			text += inputString.Substring( text.Length ); // Add delta input text
		else if ( inputString.Length < text.Length )
			text = text.Substring( 0, inputString.Length ); // Removes delta input backspace
		
		// Calculate currentLineWidth
		if ( text.Contains( "\n" ) )
			currentLineWidth = _textInstance.parentText.sizeForText( text.Substring( text.LastIndexOf( '\n' )+1 ), _textInstance.textScale ).x;
		else
			currentLineWidth = _textInstance.width;
		// Check if have to write in new line
		if ( currentLineWidth + _margin * 4 > width )
		{
			string tempText = text.Remove( text.Length-1, 1 );
			text = tempText + "\n" + inputString[inputString.Length-1];
			currentLineWidth = 0; // Reset line width
		}
		
		// Check if reach container height limit
		if ( _textInstance.height + margin * 2 > height )
		{
			// TODO: handle this situation
		}
			
		if ( onInput != null )
			onInput(this,inputString);
	}

	public override void onKeyboardEnter()
	{
		if ( onEnter != null )
			onEnter(this);
	}
	
	
	public override bool highlighted
	{
		set
		{
			// Only set if it is different than our current value
			if( _highlighted != value )
			{			
				_highlighted = value;
				
				if ( value )
					base.uvFrame = highlightedUVframe;
				else
					base.uvFrame = _tempUVframe;
			}
		}
	}
	
	// Sets the uvFrame of the original UISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _clipped ? _uvFrameClipped : _uvFrame; }
		set
		{
			_uvFrame = value;
			manager.updateUV( this );
		}
	}
	
	
	public override bool hidden
	{
		get {
			return base.hidden;
		}
		set {
			base.hidden = value;

			if ( _textInstance != null && _textInstance.hidden != value )
				_textInstance.hidden = value;
		}
	}
	
	
	public override void destroy()
	{
		base.destroy();
		
		_textInstance.destroy();
	}
}
