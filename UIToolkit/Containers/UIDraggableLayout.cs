using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDraggableLayout : UIAbstractTouchableContainer
{
	public UISprite background { set; get; }
	
	public UIDraggableLayout( string bgfilename ) : base( UIAbstractContainer.UILayoutType.AbsoluteLayout, 0 )
	{
		background = UI.firstToolkit.addSprite( bgfilename, 0, 0 );
		setSize( background.width, background.height );
		background.parentUIObject = this;
		background.positionCenter();
	}
	
	
	protected override void clipChild( UISprite child )
	{}
	
	
	#region ITouchable
	
	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{	
		// increment deltaTouch so we can pass on the touch if necessary
		_deltaTouch += touch.deltaPosition.y;
		_lastTouch = touch;
		
		// Follow touch
		Vector3 newPos = position + new Vector3( touch.deltaPosition.x, touch.deltaPosition.y, 0f );
		position = newPos;
		
		// once we move too far unhighlight and stop tracking the touchable
		if( _activeTouchable != null && Mathf.Abs( _deltaTouch ) > TOUCH_MAX_DELTA_FOR_ACTIVATION )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable = null;
		}
	}
	
	#endregion
	
	
	public void destroy()
	{
		background.destroy();
		List<UIObject> bin = _children;
		for ( int i = 0; i < bin.Count; i++ ) 
		{
			removeChild( _children[i], true );
		}
		bin.Clear();
		_manager.removeFromTouchables( this );
	}
}

