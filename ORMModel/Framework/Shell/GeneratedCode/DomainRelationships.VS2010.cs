﻿#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright © Neumont University. All rights reserved.                     *
* Copyright © The ORM Foundation. All rights reserved.                     *
* Copyright © ORM Solutions, LLC. All rights reserved.                     *
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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
namespace ORMSolutions.ORMArchitect.Framework.Shell
{
	/// <summary>
	/// DomainRelationship DiagramDisplayHasDiagramOrder
	/// Description for
	/// ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder
	/// </summary>
	[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
	[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
	[DslModeling::DomainModelOwner(typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel))]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainRelationship()]
	[DslModeling::DomainObjectId("6ccd00ce-62d2-420d-805f-791019a2c127")]
	public partial class DiagramDisplayHasDiagramOrder : DslModeling::ElementLink
	{
		#region Constructors, domain class Id
		
		/// <summary>
		/// DiagramDisplayHasDiagramOrder domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x6ccd00ce, 0x62d2, 0x420d, 0x80, 0x5f, 0x79, 0x10, 0x19, 0xa2, 0xc1, 0x27);
	
				
		/// <summary>
		/// Constructor
		/// Creates a DiagramDisplayHasDiagramOrder link in the same Partition as the given DiagramDisplay
		/// </summary>
		/// <param name="source">DiagramDisplay to use as the source of the relationship.</param>
		/// <param name="target">Diagram to use as the target of the relationship.</param>
		public DiagramDisplayHasDiagramOrder(DiagramDisplay source, global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram target)
			: base((source != null ? source.Partition : null), new DslModeling::RoleAssignment[]{new DslModeling::RoleAssignment(DiagramDisplayHasDiagramOrder.DiagramDisplayDomainRoleId, source), new DslModeling::RoleAssignment(DiagramDisplayHasDiagramOrder.DiagramDomainRoleId, target)}, null)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new link is to be created.</param>
		/// <param name="roleAssignments">List of relationship role assignments.</param>
		public DiagramDisplayHasDiagramOrder(DslModeling::Store store, params DslModeling::RoleAssignment[] roleAssignments)
			: base(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, roleAssignments, null)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new link is to be created.</param>
		/// <param name="roleAssignments">List of relationship role assignments.</param>
		/// <param name="propertyAssignments">List of properties assignments to set on the new link.</param>
		public DiagramDisplayHasDiagramOrder(DslModeling::Store store, DslModeling::RoleAssignment[] roleAssignments, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, roleAssignments, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new link is to be created.</param>
		/// <param name="roleAssignments">List of relationship role assignments.</param>
		public DiagramDisplayHasDiagramOrder(DslModeling::Partition partition, params DslModeling::RoleAssignment[] roleAssignments)
			: base(partition, roleAssignments, null)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new link is to be created.</param>
		/// <param name="roleAssignments">List of relationship role assignments.</param>
		/// <param name="propertyAssignments">List of properties assignments to set on the new link.</param>
		public DiagramDisplayHasDiagramOrder(DslModeling::Partition partition, DslModeling::RoleAssignment[] roleAssignments, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, roleAssignments, propertyAssignments)
		{
		}
		#endregion
		#region DiagramDisplay domain role code
		
		/// <summary>
		/// DiagramDisplay domain role Id.
		/// </summary>
		public static readonly global::System.Guid DiagramDisplayDomainRoleId = new global::System.Guid(0x8b27865a, 0xf62a, 0x4583, 0x99, 0x8e, 0x94, 0x25, 0x93, 0x15, 0xb5, 0xbe);
		
		/// <summary>
		/// DomainRole DiagramDisplay
		/// Description for
		/// ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DiagramDisplay
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/DiagramDisplay.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/DiagramDisplay.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslModeling::DomainRole(DslModeling::DomainRoleOrder.Source, PropertyName = "OrderedDiagramCollection", PropertyDisplayNameKey="ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/DiagramDisplay.PropertyDisplayName",  PropagatesCopy = DslModeling::PropagatesCopyOption.DoNotPropagateCopy, Multiplicity = DslModeling::Multiplicity.ZeroMany)]
		[DslModeling::DomainObjectId("8b27865a-f62a-4583-998e-94259315b5be")]
		public virtual DiagramDisplay DiagramDisplay
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return (DiagramDisplay)DslModeling::DomainRoleInfo.GetRolePlayer(this, DiagramDisplayDomainRoleId);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				DslModeling::DomainRoleInfo.SetRolePlayer(this, DiagramDisplayDomainRoleId, value);
			}
		}
				
		#endregion
		#region Static methods to access DiagramDisplay of a Diagram
		/// <summary>
		/// Gets DiagramDisplay.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static DiagramDisplay GetDiagramDisplay(global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram element)
		{
			return DslModeling::DomainRoleInfo.GetLinkedElement(element, DiagramDomainRoleId) as DiagramDisplay;
		}
		
		/// <summary>
		/// Sets DiagramDisplay.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static void SetDiagramDisplay(global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram element, DiagramDisplay newDiagramDisplay)
		{
			DslModeling::DomainRoleInfo.SetLinkedElement(element, DiagramDomainRoleId, newDiagramDisplay);
		}
		#endregion
		#region Diagram domain role code
		
		/// <summary>
		/// Diagram domain role Id.
		/// </summary>
		public static readonly global::System.Guid DiagramDomainRoleId = new global::System.Guid(0xa22a0cfa, 0x632c, 0x4aaa, 0x8a, 0x88, 0xe8, 0x13, 0xda, 0xe7, 0xcb, 0xb2);
		
		/// <summary>
		/// DomainRole Diagram
		/// Description for
		/// ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.Diagram
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/Diagram.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/Diagram.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslModeling::DomainRole(DslModeling::DomainRoleOrder.Target, PropertyName = "DiagramDisplay", PropertyDisplayNameKey="ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/Diagram.PropertyDisplayName",  PropagatesCopy = DslModeling::PropagatesCopyOption.DoNotPropagateCopy, Multiplicity = DslModeling::Multiplicity.ZeroOne)]
		[DslModeling::DomainObjectId("a22a0cfa-632c-4aaa-8a88-e813dae7cbb2")]
		public virtual global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram Diagram
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return (global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram)DslModeling::DomainRoleInfo.GetRolePlayer(this, DiagramDomainRoleId);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				DslModeling::DomainRoleInfo.SetRolePlayer(this, DiagramDomainRoleId, value);
			}
		}
				
		#endregion
		#region Static methods to access OrderedDiagramCollection of a DiagramDisplay
		/// <summary>
		/// Gets a list of OrderedDiagramCollection.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static DslModeling::LinkedElementCollection<global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram> GetOrderedDiagramCollection(DiagramDisplay element)
		{
			return GetRoleCollection<DslModeling::LinkedElementCollection<global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram>, global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram>(element, DiagramDisplayDomainRoleId);
		}
		#endregion
		#region CenterPoint domain property code
		
		/// <summary>
		/// CenterPoint domain property Id.
		/// </summary>
		public static readonly global::System.Guid CenterPointDomainPropertyId = new global::System.Guid(0xdf1ffee4, 0xac41, 0x4210, 0x9a, 0x9a, 0x5b, 0x47, 0x08, 0xb7, 0x25, 0xb6);
		
		/// <summary>
		/// Gets or sets the value of CenterPoint domain property.
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/CenterPoint.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/CenterPoint.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[global::System.ComponentModel.Browsable(false)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.CustomStorage)]
		[DslModeling::DomainObjectId("df1ffee4-ac41-4210-9a9a-5b4708b725b6")]
		public global::Microsoft.VisualStudio.Modeling.Diagrams.PointD CenterPoint
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return CenterPointPropertyHandler.Instance.GetValue(this);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				CenterPointPropertyHandler.Instance.SetValue(this, value);
			}
		}
		/// <summary>
		/// Value handler for the DiagramDisplayHasDiagramOrder.CenterPoint domain property.
		/// </summary>
		internal sealed partial class CenterPointPropertyHandler : DslModeling::DomainPropertyValueHandler<DiagramDisplayHasDiagramOrder, global::Microsoft.VisualStudio.Modeling.Diagrams.PointD>
		{
			private CenterPointPropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the DiagramDisplayHasDiagramOrder.CenterPoint domain property value handler.
			/// </summary>
			public static readonly CenterPointPropertyHandler Instance = new CenterPointPropertyHandler();
		
			/// <summary>
			/// Gets the Id of the DiagramDisplayHasDiagramOrder.CenterPoint domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return CenterPointDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::Microsoft.VisualStudio.Modeling.Diagrams.PointD GetValue(DiagramDisplayHasDiagramOrder element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for CenterPoint because its Kind is
				// set to CustomStorage. Please provide the GetCenterPointValue()
				// method on the domain class.
				return element.GetCenterPointValue();
			}
		
			/// <summary>
			/// Sets property value on an element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <param name="newValue">New property value.</param>
			public override sealed void SetValue(DiagramDisplayHasDiagramOrder element, global::Microsoft.VisualStudio.Modeling.Diagrams.PointD newValue)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
		
				global::Microsoft.VisualStudio.Modeling.Diagrams.PointD oldValue = GetValue(element);
				if (newValue != oldValue)
				{
					ValueChanging(element, oldValue, newValue);
					// There is no storage for CenterPoint because its Kind is
					// set to CustomStorage. Please provide the SetCenterPointValue()
					// method on the domain class.
					element.SetCenterPointValue(newValue);
					//ValueChanged(element, oldValue, GetValue(element));
					ValueChanged(element, oldValue, newValue);
				}
			}
		}
		
		#endregion
		#region ZoomFactor domain property code
		
		/// <summary>
		/// ZoomFactor domain property Id.
		/// </summary>
		public static readonly global::System.Guid ZoomFactorDomainPropertyId = new global::System.Guid(0x9182eb46, 0x20f6, 0x4445, 0x8f, 0xda, 0x70, 0x52, 0xba, 0xe5, 0x4f, 0x45);
		
		/// <summary>
		/// Gets or sets the value of ZoomFactor domain property.
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/ZoomFactor.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/ZoomFactor.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[global::System.ComponentModel.Browsable(false)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.CustomStorage)]
		[DslModeling::DomainObjectId("9182eb46-20f6-4445-8fda-7052bae54f45")]
		public global::System.Single ZoomFactor
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return ZoomFactorPropertyHandler.Instance.GetValue(this);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				ZoomFactorPropertyHandler.Instance.SetValue(this, value);
			}
		}
		/// <summary>
		/// Value handler for the DiagramDisplayHasDiagramOrder.ZoomFactor domain property.
		/// </summary>
		internal sealed partial class ZoomFactorPropertyHandler : DslModeling::DomainPropertyValueHandler<DiagramDisplayHasDiagramOrder, global::System.Single>
		{
			private ZoomFactorPropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the DiagramDisplayHasDiagramOrder.ZoomFactor domain property value handler.
			/// </summary>
			public static readonly ZoomFactorPropertyHandler Instance = new ZoomFactorPropertyHandler();
		
			/// <summary>
			/// Gets the Id of the DiagramDisplayHasDiagramOrder.ZoomFactor domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return ZoomFactorDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.Single GetValue(DiagramDisplayHasDiagramOrder element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for ZoomFactor because its Kind is
				// set to CustomStorage. Please provide the GetZoomFactorValue()
				// method on the domain class.
				return element.GetZoomFactorValue();
			}
		
			/// <summary>
			/// Sets property value on an element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <param name="newValue">New property value.</param>
			public override sealed void SetValue(DiagramDisplayHasDiagramOrder element, global::System.Single newValue)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
		
				global::System.Single oldValue = GetValue(element);
				// float type precision is guaranteed only to 7th digit.
				if (global::System.Math.Abs(newValue - oldValue) > 1e-7)
				{
					ValueChanging(element, oldValue, newValue);
					// There is no storage for ZoomFactor because its Kind is
					// set to CustomStorage. Please provide the SetZoomFactorValue()
					// method on the domain class.
					element.SetZoomFactorValue(newValue);
					//ValueChanged(element, oldValue, GetValue(element));
					ValueChanged(element, oldValue, newValue);
				}
			}
		}
		
		#endregion
		#region IsActiveDiagram domain property code
		
		/// <summary>
		/// IsActiveDiagram domain property Id.
		/// </summary>
		public static readonly global::System.Guid IsActiveDiagramDomainPropertyId = new global::System.Guid(0x0327999b, 0x60e2, 0x4119, 0x8f, 0xac, 0xa3, 0x47, 0x08, 0x85, 0x1d, 0x0a);
		
		/// <summary>
		/// Gets or sets the value of IsActiveDiagram domain property.
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/IsActiveDiagram.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder/IsActiveDiagram.Description", typeof(global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayDomainModel), "ORMSolutions.ORMArchitect.Core.GeneratedCode.DiagramDisplayDomainModelResx")]
		[global::System.ComponentModel.Browsable(false)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.CustomStorage)]
		[DslModeling::DomainObjectId("0327999b-60e2-4119-8fac-a34708851d0a")]
		public global::System.Boolean IsActiveDiagram
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return IsActiveDiagramPropertyHandler.Instance.GetValue(this);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				IsActiveDiagramPropertyHandler.Instance.SetValue(this, value);
			}
		}
		/// <summary>
		/// Value handler for the DiagramDisplayHasDiagramOrder.IsActiveDiagram domain property.
		/// </summary>
		internal sealed partial class IsActiveDiagramPropertyHandler : DslModeling::DomainPropertyValueHandler<DiagramDisplayHasDiagramOrder, global::System.Boolean>
		{
			private IsActiveDiagramPropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the DiagramDisplayHasDiagramOrder.IsActiveDiagram domain property value handler.
			/// </summary>
			public static readonly IsActiveDiagramPropertyHandler Instance = new IsActiveDiagramPropertyHandler();
		
			/// <summary>
			/// Gets the Id of the DiagramDisplayHasDiagramOrder.IsActiveDiagram domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return IsActiveDiagramDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.Boolean GetValue(DiagramDisplayHasDiagramOrder element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for IsActiveDiagram because its Kind is
				// set to CustomStorage. Please provide the GetIsActiveDiagramValue()
				// method on the domain class.
				return element.GetIsActiveDiagramValue();
			}
		
			/// <summary>
			/// Sets property value on an element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <param name="newValue">New property value.</param>
			public override sealed void SetValue(DiagramDisplayHasDiagramOrder element, global::System.Boolean newValue)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
		
				global::System.Boolean oldValue = GetValue(element);
				if (newValue != oldValue)
				{
					ValueChanging(element, oldValue, newValue);
					// There is no storage for IsActiveDiagram because its Kind is
					// set to CustomStorage. Please provide the SetIsActiveDiagramValue()
					// method on the domain class.
					element.SetIsActiveDiagramValue(newValue);
					//ValueChanged(element, oldValue, GetValue(element));
					ValueChanged(element, oldValue, newValue);
				}
			}
		}
		
		#endregion
		#region DiagramDisplay link accessor
		/// <summary>
		/// Get the list of DiagramDisplayHasDiagramOrder links to a DiagramDisplay.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static global::System.Collections.ObjectModel.ReadOnlyCollection<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> GetLinksToOrderedDiagramCollection ( global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplay diagramDisplayInstance )
		{
			return DslModeling::DomainRoleInfo.GetElementLinks<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder>(diagramDisplayInstance, global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DiagramDisplayDomainRoleId);
		}
		#endregion
		#region Diagram link accessor
		/// <summary>
		/// Get the DiagramDisplayHasDiagramOrder link to a Diagram.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder GetLinkToDiagramDisplay (global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram diagramInstance)
		{
			global::System.Collections.Generic.IList<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> links = DslModeling::DomainRoleInfo.GetElementLinks<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder>(diagramInstance, global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DiagramDomainRoleId);
			global::System.Diagnostics.Debug.Assert(links.Count <= 1, "Multiplicity of Diagram not obeyed.");
			if ( links.Count == 0 )
			{
				return null;
			}
			else
			{
				return links[0];
			}
		}
		#endregion
		#region DiagramDisplayHasDiagramOrder instance accessors
		
		/// <summary>
		/// Get any DiagramDisplayHasDiagramOrder links between a given DiagramDisplay and a Diagram.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static global::System.Collections.ObjectModel.ReadOnlyCollection<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> GetLinks( global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplay source, global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram target )
		{
			global::System.Collections.Generic.List<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> outLinks = new global::System.Collections.Generic.List<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder>();
			global::System.Collections.Generic.IList<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> links = DslModeling::DomainRoleInfo.GetElementLinks<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder>(source, global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DiagramDisplayDomainRoleId);
			foreach ( global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder link in links )
			{
				if ( target.Equals(link.Diagram) )
				{
					outLinks.Add(link);
				}
			}
			return outLinks.AsReadOnly();
		}
		/// <summary>
		/// Get the one DiagramDisplayHasDiagramOrder link between a given DiagramDisplayand a Diagram.
		/// </summary>
		[global::System.Diagnostics.DebuggerStepThrough]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
		public static global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder GetLink( global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplay source, global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram target )
		{
			global::System.Collections.Generic.IList<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder> links = DslModeling::DomainRoleInfo.GetElementLinks<global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder>(source, global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder.DiagramDisplayDomainRoleId);
			foreach ( global::ORMSolutions.ORMArchitect.Framework.Shell.DiagramDisplayHasDiagramOrder link in links )
			{
				if ( target.Equals(link.Diagram) )
				{
					return link;
				}
			}
			return null;
		}
		
		#endregion
	}
}
