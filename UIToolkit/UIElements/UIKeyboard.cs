using System;
using UnityEngine;
using System.Collections.Generic;

public class UIKeyboard
{
	/************************************************************************/
	/*								DISCLAIMER								*/
	/*	Currently UIKeyboard only support one uitoolkit at the same time	*/
	/*			Unexpected behavior if there are more than one				*/
	/************************************************************************/


	static public UIKeyboard instance = null;

	protected string _inputString = "";
	protected bool _enabled = true;
	protected bool enterPushed = false;

	public bool enabled { get { return _enabled; } set { if ( _enabled != value ) _enabled = value; } }

	public string inputString { get { return _inputString; } set { if ( _inputString != value )	_inputString = value; } }


	static public void Start()
	{
		instance = new UIKeyboard();

		instance.InitVars(); 
	}

	public void InitVars()
	{
		
	}

	public void Update()
	{
		if ( _enabled )
		{
			enterPushed = false;
			string frameInputString = Input.inputString;
			if ( frameInputString != "" )
			{
				// update input string
				foreach ( char c in frameInputString )
				{
					// check backspace
					if ( c == "\b"[0] )
					{
						if (_inputString.Length > 0 )
						{
							_inputString = _inputString.Substring( 0, _inputString.Length - 1 ); // delete last character
						}
					}
					else
					{
						if ( c == "\n"[0] || c == "\r"[0] ) // check return or enter
						{
							enterPushed = true;
						}
						else
						{					
							_inputString += c;
				
						}
					}
				}
				
				
				// call available listener
				if ( UI.firstToolkit.textFieldWithFocus != null ) // FIXME: just working with first uitoolkit added
				{
					UI.firstToolkit.textFieldWithFocus.onKeyboardEntry( _inputString, frameInputString );
					if ( enterPushed ) UI.firstToolkit.textFieldWithFocus.onKeyboardEnter();
				}
		    }
		}
	}
	
}
