#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
*                                                                          *
* The use and distribution terms for this software are covered by the      *
* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
* can be found in the file CPL.txt at the root of this distribution.       *
* By using this software in any fashion, you are agreeing to be bound by   *
* the terms of this license.                                               *
*                                                                          *
* You must not remove this notice, or any other, from this software.       *
\**************************************************************************/
#endregion

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Neumont.Tools.ORM.ObjectModel;

namespace Neumont.Tools.ORM.ShapeModel
{
	#region ModelNoteConnectAction class
	/// <summary>
	/// A connect action for attaching a base type to its derived type
	/// </summary>
	public class ModelNoteConnectAction : ConnectAction
	{
		#region ExternalConstraintConnectionType class
		/// <summary>
		/// The ConnectionType used with this ConnectAction. The type
		/// is a singleton, holds all of the context-independent logic,
		/// and operates directly on shape elements.
		/// </summary>
		protected class ModelNoteConnectionType : ConnectionType
		{
			/// <summary>
			/// The singleton ModelNoteConnectionType instance
			/// </summary>
			public static new readonly ModelNoteConnectionType Instance = new ModelNoteConnectionType();
			/// <summary>
			/// An array of one element containing the singleton ModelNoteConnectionType instance
			/// </summary>
			public static readonly ConnectionType[] InstanceArray = { Instance };
			/// <summary>
			/// Called as the pointer is moved over potential targets after a source is selected
			/// So should be pretty quick
			/// Gets called with a target of null when the cursor leaves the view.
			/// Can't find a use for this at present.
			/// </summary>
			/// <param name="sourceShapeElement">ShapeElement</param>
			/// <param name="targetShapeElement">ShapeElement</param>
			public override bool IsOfInterest(ShapeElement sourceShapeElement, ShapeElement targetShapeElement)
			{
				return false; // Always or'd with IsValidSourceAndTarget
			}
			/// <summary>
			/// Called as the pointer is moved over potential targets after a source is selected
			/// So should be pretty quick
			/// </summary>
			/// <remarks>
			/// The cursor can change dependant on CanCreateConnection when this returns true
			/// </remarks>
			/// <param name="sourceShapeElement">ShapeElement</param>
			/// <param name="targetShapeElement">ShapeElement</param>
			/// <returns></returns>
			public override bool IsValidSourceAndTarget(ShapeElement sourceShapeElement, ShapeElement targetShapeElement)
			{
				ModelNoteConnectAction action = (sourceShapeElement.Diagram as ORMDiagram).ModelNoteConnectAction;
				ModelNote sourceModelNote;
				if (null != (sourceModelNote = action.mySourceModelNote))
				{
					FactType targetFactType;
					ObjectType targetObjectType;
					if (null != (targetFactType = ElementFromShape<FactType>(targetShapeElement)))
					{
						return !sourceModelNote.FactTypeCollection.Contains(targetFactType);
					}
					else if (null != (targetObjectType = ElementFromShape<ObjectType>(targetShapeElement)))
					{
						return !sourceModelNote.ObjectTypeCollection.Contains(targetObjectType);
					}
				}
				return false;
			}
			/// <summary>
			/// Called after IsValidSourceAndTarget allows the shapes through. Used for
			/// more in-depth checking before ConnectionType.CreateConnection is called, and
			/// to display warning messages on the design surface.
			/// </summary>
			/// <param name="sourceShapeElement">The source of the requested connection</param>
			/// <param name="targetShapeElement">The target of the requested connection</param>
			/// <param name="connectionWarning">A location to write the warning string</param>
			/// <returns>true if the connection can proceed</returns>
			public override bool CanCreateConnection(ShapeElement sourceShapeElement, ShapeElement targetShapeElement, ref string connectionWarning)
			{
				// Everything is handled in IsValidSourceAndTarget.
				return true;
			}
			/// <summary>
			/// Create a connection between an ExternalConstraintShape and a FactType. Roles
			/// used in the connection are stored with the currently active connect action.
			/// </summary>
			/// <param name="sourceShapeElement">The source of the requested connection</param>
			/// <param name="targetShapeElement">The target of the requested connection</param>
			/// <param name="paintFeedbackArgs">PaintFeedbackArgs</param>
			public override void CreateConnection(ShapeElement sourceShapeElement, ShapeElement targetShapeElement, PaintFeedbackArgs paintFeedbackArgs)
			{
				ModelNoteConnectAction action = (sourceShapeElement.Diagram as ORMDiagram).ModelNoteConnectAction;
				ModelNote sourceModelNote;
				if (null != (sourceModelNote = action.mySourceModelNote))
				{
					FactType targetFactType;
					ObjectType targetObjectType;
					if (null != (targetFactType = ElementFromShape<FactType>(targetShapeElement)))
					{
						new ModelNoteReferencesFactType(sourceModelNote, targetFactType);
					}
					else if (null != (targetObjectType = ElementFromShape<ObjectType>(targetShapeElement)))
					{
						new ModelNoteReferencesObjectType(sourceModelNote, targetObjectType);
					}
				}
			}
			/// <summary>
			/// Provide the transaction name. The name is displayed in the undo and redo lists.
			/// </summary>
			public override string GetConnectTransactionName(ShapeElement sourceShape, ShapeElement targetShape)
			{
				return ResourceStrings.ModelNoteConnectActionTransactionName;
			}
		}
		#endregion // ModelNoteConnectionType class
		#region Member variables
		// The following cursors are built as embedded resources. Pick them up by their file name.
		private static Cursor myAllowedCursor = new Cursor(typeof(ModelNoteConnectAction), "ConnectModelNoteAllowed.cur");
		private static Cursor mySearchingCursor = new Cursor(typeof(ModelNoteConnectAction), "ConnectModelNoteSearching.cur");
		private static readonly ConnectionType[] EmptyConnectionTypes = {};
		private ModelNote mySourceModelNote;
		private bool myEmulateDrag;
		#endregion // Member variables
		#region Constructors
		/// <summary>
		/// Create a model note connector action for the given diagram. One
		/// action per diagram should be sufficient.
		/// </summary>
		/// <param name="diagram">The hosting diagram</param>
		public ModelNoteConnectAction(Diagram diagram)
			: base(diagram, true)
		{
			Reset();
		}
		#endregion // Constructors
		#region Base overrides
		/// <summary>
		/// Retrieve all connect types associated with this connect action.
		/// Returns an empty array unless the sourceShapeElement is an ExternalConstraintShape
		/// </summary>
		/// <param name="sourceShapeElement">The source element</param>
		/// <param name="targetShapeElement">The target element. Currently ignored.</param>
		/// <returns></returns>
		protected override ConnectionType[] GetConnectionTypes(ShapeElement sourceShapeElement, ShapeElement targetShapeElement)
		{
			return ModelNoteConnectionType.InstanceArray;
		}
		/// <summary>
		/// Reset the member variables
		/// </summary>
		/// <param name="e">DiagramEventArgs</param>
		protected override void OnMouseActionDeactivated(DiagramEventArgs e)
		{
			base.OnMouseActionDeactivated(e);

			// Set the selection back to pointer after connect action
			DiagramView activeView;
			IToolboxService toolbox;
			if ((null != (activeView = Diagram.ActiveDiagramView)) &&
				(null != (toolbox = activeView.Toolbox)))
			{
				toolbox.SelectedToolboxItemUsed();
			}

			Reset();
		}
		/// <summary>
		/// Get the source object type or role after the
		/// base class sets the source shape
		/// </summary>
		/// <param name="e">MouseActionEventArgs</param>
		protected override void OnClicked(MouseActionEventArgs e)
		{
			base.OnClicked(e);
			if (mySourceModelNote == null)
			{
				mySourceModelNote = ElementFromShape<ModelNote>(MouseDownHitShape);
			}
		}
		/// <summary>
		/// If we're emulating a drag (occurs when we're chained from RoleDragPendingAction),
		/// then complete the action on mouse up
		/// </summary>
		/// <param name="e">DiagramMouseEventArgs</param>
		protected override void OnMouseUp(DiagramMouseEventArgs e)
		{
			if (myEmulateDrag && mySourceModelNote != null)
			{
				myEmulateDrag = false;
				DiagramClientView clientView = e.DiagramClientView;
				if (CanComplete(clientView))
				{
					// Establish the mouse down destinations
					MouseDown(e);
					Complete(clientView);
				}
			}
			base.OnMouseUp(e);
		}
		/// <summary>
		/// Get a cursor from the cursor type. Returns the searching cursor for
		/// everything except the allowed action.
		/// </summary>
		/// <param name="connectActionCursor">The requrested cursor styl</param>
		/// <returns></returns>
		protected override Cursor GetCursorFromCursorType(ConnectActionCursor connectActionCursor)
		{
			Cursor cursor;
			switch (connectActionCursor)
			{
				case ConnectActionCursor.Allowed:
					cursor = myAllowedCursor;
					break;
				//case ConnectActionCursor.Searching:
				//case ConnectActionCursor.Disallowed:
				//case ConnectActionCursor.Warning:
				default:
					cursor = mySearchingCursor;
					break;
			}
			return cursor;
		}
		#endregion // Base overrides
		#region ModelNoteConnectAction specific
		private void Reset()
		{
			mySourceModelNote = null;
			myEmulateDrag = false;
		}
		/// <summary>
		/// Set this mouse action as the active action on the
		/// diagram of the given shape, and activate its drag line
		/// centered on the shape.
		/// </summary>
		/// <param name="chainFromPoint">The point to begin the mouse action</param>
		/// <param name="clientView">The active DiagramClientView</param>
		/// <param name="emulateDrag">true if this should emulate a drag, meaning
		/// that the mouse up acts like a click.</param>
		public void ChainMouseAction(PointD chainFromPoint, DiagramClientView clientView, bool emulateDrag)
		{
			DiagramView activeView = Diagram.ActiveDiagramView;
			if (activeView != null)
			{
				// Move on to the selection action
				clientView.ActiveMouseAction = this;

				// Now emulate a mouse click in the middle of the added constraint. The click
				// actions provide a starting point for the connect action, so a mouse move
				// provides a drag line.
				Point emulateClickPoint = clientView.WorldToDevice(chainFromPoint);
				DiagramMouseEventArgs mouseEventArgs = new DiagramMouseEventArgs(new MouseEventArgs(MouseButtons.Left, 1, emulateClickPoint.X, emulateClickPoint.Y, 0), clientView);
				MouseDown(mouseEventArgs);
				Click(new DiagramPointEventArgs(emulateClickPoint.X, emulateClickPoint.Y, PointRelativeTo.Client, clientView));
				MouseUp(mouseEventArgs);

				// An extra move lets us chain when the mouse is not on the design surface,
				// such as when we are being activated via the task list.
				MouseMove(mouseEventArgs);

				myEmulateDrag = emulateDrag;
				ORMDiagram.SelectToolboxItem(activeView, ResourceStrings.ToolboxModelNoteConnectorItemId);
			}
		}
		/// <summary>
		/// Get the underlying element for a presentation element
		/// </summary>
		/// <typeparam name="ElementType">The type of the element to retrieve</typeparam>
		/// <param name="shape">A presentation element.</param>
		/// <returns>An ElementType element, or null</returns>
		protected static ElementType ElementFromShape<ElementType>(PresentationElement shape) where ElementType : ModelElement
		{
			return ((shape != null) ? shape.ModelElement : null) as ElementType;
		}
		#endregion // ModelNoteConnectAction specific
	}
	#endregion // ModelNoteConnectAction class
}