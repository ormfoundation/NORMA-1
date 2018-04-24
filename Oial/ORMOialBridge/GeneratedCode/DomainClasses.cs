﻿#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright © Neumont University. All rights reserved.                     *
* Copyright © The ORM Foundation. All rights reserved.                     *
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
namespace ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge
{
	/// <summary>
	/// DomainClass AbstractionModelGenerationSetting
	/// </summary>
	[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.AbstractionModelGenerationSetting.DisplayName", typeof(global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.ORMToORMAbstractionBridgeDomainModel), "ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GeneratedCode.ORMToORMAbstractionBridgeDomainModelResx")]
	[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.AbstractionModelGenerationSetting.Description", typeof(global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.ORMToORMAbstractionBridgeDomainModel), "ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GeneratedCode.ORMToORMAbstractionBridgeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("c7e6b42c-c827-4b39-8fee-e3d52aa3d186")]
	public partial class AbstractionModelGenerationSetting : global::ORMSolutions.ORMArchitect.Core.ObjectModel.GenerationSetting
	{
		#region Constructors, domain class Id
	
		/// <summary>
		/// AbstractionModelGenerationSetting domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0xc7e6b42c, 0xc827, 0x4b39, 0x8f, 0xee, 0xe3, 0xd5, 0x2a, 0xa3, 0xd1, 0x86);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public AbstractionModelGenerationSetting(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public AbstractionModelGenerationSetting(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
		#region AlgorithmVersion domain property code
		
		/// <summary>
		/// AlgorithmVersion domain property Id.
		/// </summary>
		public static readonly global::System.Guid AlgorithmVersionDomainPropertyId = new global::System.Guid(0x64197312, 0x6561, 0x4e25, 0x9d, 0xea, 0x7a, 0xd9, 0x74, 0x7d, 0x91, 0x32);
		
		/// <summary>
		/// Storage for AlgorithmVersion
		/// </summary>
		private global::System.String algorithmVersionPropertyStorage = string.Empty;
		
		/// <summary>
		/// Gets or sets the value of AlgorithmVersion domain property.
		/// Description for
		/// ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.AbstractionModelGenerationSetting.Depth
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.AbstractionModelGenerationSetting/AlgorithmVersion.DisplayName", typeof(global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.ORMToORMAbstractionBridgeDomainModel), "ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GeneratedCode.ORMToORMAbstractionBridgeDomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.AbstractionModelGenerationSetting/AlgorithmVersion.Description", typeof(global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.ORMToORMAbstractionBridgeDomainModel), "ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GeneratedCode.ORMToORMAbstractionBridgeDomainModelResx")]
		[DslModeling::DomainObjectId("64197312-6561-4e25-9dea-7ad9747d9132")]
		public global::System.String AlgorithmVersion
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return algorithmVersionPropertyStorage;
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				AlgorithmVersionPropertyHandler.Instance.SetValue(this, value);
			}
		}
		/// <summary>
		/// Value handler for the AbstractionModelGenerationSetting.AlgorithmVersion domain property.
		/// </summary>
		internal sealed partial class AlgorithmVersionPropertyHandler : DslModeling::DomainPropertyValueHandler<AbstractionModelGenerationSetting, global::System.String>
		{
			private AlgorithmVersionPropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the AbstractionModelGenerationSetting.AlgorithmVersion domain property value handler.
			/// </summary>
			public static readonly AlgorithmVersionPropertyHandler Instance = new AlgorithmVersionPropertyHandler();
		
			/// <summary>
			/// Gets the Id of the AbstractionModelGenerationSetting.AlgorithmVersion domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return AlgorithmVersionDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.String GetValue(AbstractionModelGenerationSetting element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				return element.algorithmVersionPropertyStorage;
			}
		
			/// <summary>
			/// Sets property value on an element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <param name="newValue">New property value.</param>
			public override sealed void SetValue(AbstractionModelGenerationSetting element, global::System.String newValue)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
		
				global::System.String oldValue = GetValue(element);
				if (newValue != oldValue)
				{
					ValueChanging(element, oldValue, newValue);
					element.algorithmVersionPropertyStorage = newValue;
					ValueChanged(element, oldValue, newValue);
				}
			}
		}
		
		#endregion
		#region GeneratedAbstractionModel opposite domain role accessor
		/// <summary>
		/// Gets or sets GeneratedAbstractionModel.
		/// Description for
		/// ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GenerationSettingTargetsAbstractionModel.GenerationSetting
		/// </summary>
		public virtual global::ORMSolutions.ORMArchitect.ORMAbstraction.AbstractionModel GeneratedAbstractionModel
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return DslModeling::DomainRoleInfo.GetLinkedElement(this, global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GenerationSettingTargetsAbstractionModel.GenerationSettingDomainRoleId) as global::ORMSolutions.ORMArchitect.ORMAbstraction.AbstractionModel;
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				DslModeling::ModelElement existingSource;
				if (null != value &&
					null != (existingSource = DslModeling::DomainRoleInfo.GetLinkedElement(value, global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GenerationSettingTargetsAbstractionModel.GeneratedAbstractionModelDomainRoleId)))
				{
					if (existingSource != value)
					{
						DslModeling::DomainRoleInfo.SetLinkedElement(value, global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GenerationSettingTargetsAbstractionModel.GeneratedAbstractionModelDomainRoleId, this);
					}
				}
				else
				{
					DslModeling::DomainRoleInfo.SetLinkedElement(this, global::ORMSolutions.ORMArchitect.ORMToORMAbstractionBridge.GenerationSettingTargetsAbstractionModel.GenerationSettingDomainRoleId, value);
				}
			}
		}
		#endregion
	}
}
