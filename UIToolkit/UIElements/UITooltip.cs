using UnityEngine;
using System;

public class UITooltip : UISprite
{
	public UITextInstance Text { get; set; }


	public static UITooltip create( UIToolkit manager, UIObject parent, string filename, UITextInstance text, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos + parent.position.x, yPos + parent.position.y, normalTI.frame.width, normalTI.frame.height );

		return new UITooltip( manager, filename, parent, frame, normalTI.uvRect, text, depth );
	}


	public UITooltip( UIToolkit manager, string filename, UIObject parent, Rect frame, UIUVRect uvFrame, UITextInstance text, int depth ):base( frame, depth, uvFrame )
	{
		this.manager = manager;
		parentUIObject = parent;
		Text = text;
		Text.depth = depth - 1; // Text must be in top of background
		Text.parentUIObject = this;
		Text.positionFromCenter( 0f, 0f );
		hidden = true;

		manager.addSprite( this, filename );
	}


	override public bool hidden
	{
		get { return ___hidden; }
		set
		{
			// No need to do anything if we're already in this state:
			if( value == ___hidden )
				return;
			
			if( value )
				manager.hideSprite( this );
			else
				manager.showSprite( this );

			Text.hidden = value;
		}
	}
}
