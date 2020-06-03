using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

using Unigine;

namespace UnigineApp
{
	class AppWorldLogic : WorldLogic
	{
		// World logic, it takes effect only when the world is loaded.
		// These methods are called right after corresponding world script's (UnigineScript) methods.

		public AppWorldLogic()
		{
		}

		public override bool Init()
		{
			// getting a GUI pointer
			Gui gui = Gui.Get();

			WidgetEditText editText = new WidgetEditText(gui, "XeonSector : Flight Simulator");
			editText.SetPosition(500,400);
			editText.Editable = false;
			editText.FontSize = 50;
			editText.FontColor = new Unigine.vec4(255, 255, 255, 1);
			editText.BackgroundColor = new Unigine.vec4(0, 0, 0, 0);

			gui.AddChild(editText, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
			
			
			WidgetEditText enterText = new WidgetEditText(gui, "Press Enter to continue");
			enterText.SetPosition(500,600);
			enterText.Editable = false;
			enterText.FontSize = 40;
			enterText.FontColor = new Unigine.vec4(255, 255, 255, 1);
			enterText.BackgroundColor = new Unigine.vec4(0, 0, 0, 0);

			

			gui.AddChild(editText, Gui.ALIGN_OVERLAP | Gui.ALIGN_FIXED);
			gui.AddChild(enterText, Gui.ALIGN_OVERLAP | Gui.ALIGN_FIXED);

			return true;	
		}

		// start of the main loop
		public override bool Update()
		{
			// Write here code to be called before updating each render frame: specify all graphics-related functions you want to be called every frame while your application executes.

			return true;
		}

		public override bool PostUpdate()
		{
			// The engine calls this function after updating each render frame: correct behavior after the state of the node has been updated.

			return true;
		}

		public override bool UpdatePhysics()
		{
			// Write here code to be called before updating each physics frame: control physics in your application and put non-rendering calculations.
			// The engine calls UpdatePhysics() with the fixed rate (60 times per second by default) regardless of the FPS value.
			// WARNING: do not create, delete or change transformations of nodes here, because rendering is already in progress.

			return true;
		}
		// end of the main loop

		public override bool Shutdown()
		{
			// Write here code to be called on world shutdown: delete resources that were created during world script execution to avoid memory leaks.

			return true;
		}

		public override bool Save(Stream stream)
		{
			// Write here code to be called when the world is saving its state (i.e. state_save is called): save custom user data to a file.

			return true;
		}

		public override bool Restore(Stream stream)
		{
			// Write here code to be called when the world is restoring its state (i.e. state_restore is called): restore custom user data to a file here.

			return true;
		}
	}
}
