﻿using System;
using System.Reflection;

// Common Public License Copyright Notice
// /**************************************************************************\
// * Neumont Object-Role Modeling Architect for Visual Studio                 *
// *                                                                          *
// * Copyright © Neumont University. All rights reserved.                     *
// * Copyright © The ORM Foundation. All rights reserved.                     *
// *                                                                          *
// * The use and distribution terms for this software are covered by the      *
// * Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
// * can be found in the file CPL.txt at the root of this distribution.       *
// * By using this software in any fashion, you are agreeing to be bound by   *
// * the terms of this license.                                               *
// *                                                                          *
// * You must not remove this notice, or any other, from this software.       *
// \**************************************************************************/

namespace Neumont.Tools.ORM.OIALModel
{
	#region Attach rules to OIALDomainModel model
	partial class OIALDomainModel : ORMSolutions.ORMArchitect.Framework.Shell.IDomainModelEnablesRulesAfterDeserialization
	{
		private static Type[] myCustomDomainModelTypes;
		private static Type[] CustomDomainModelTypes
		{
			get
			{
				Type[] retVal = OIALDomainModel.myCustomDomainModelTypes;
				if (retVal == null)
				{
					// No synchronization is needed here.
					// If accessed concurrently, the worst that will happen is the array of Types being created multiple times.
					// This would have a slightly negative impact on performance, but the result would still be correct.
					// Given the low likelihood of this ever happening, the extra overhead of synchronization would outweigh any possible gain from it.
					retVal = new Type[]{
						typeof(OIALModel).GetNestedType("ModelHasObjectTypeAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ModelHasObjectTypeDeletingRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ObjectTypeChangeRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ObjectTypePlaysRoleAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ObjectTypePlaysRoleDeletingRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ModelHasFactTypeAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ModelHasFactTypeDeletingRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ModelHasSetConstraintAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ModelHasSetConstraintDeletingRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ConstraintRoleSequenceHasRoleAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("ConstraintRoleSequenceHasRoleDeletingRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("UniquenessConstraintChangeRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("MandatoryConstraintChangeRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("RoleBaseChangeRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("SetConstraintChangeRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("CheckConceptTypeParentExclusiveMandatory", BindingFlags.Public | BindingFlags.NonPublic).GetNestedType("OIALModelHasConceptTypeAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("CheckConceptTypeParentExclusiveMandatory", BindingFlags.Public | BindingFlags.NonPublic).GetNestedType("OIALModelHasConceptTypeDeleteRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("CheckConceptTypeParentExclusiveMandatory", BindingFlags.Public | BindingFlags.NonPublic).GetNestedType("ConceptTypeAbsorbedConceptTypeAddRule", BindingFlags.Public | BindingFlags.NonPublic),
						typeof(OIALModel).GetNestedType("CheckConceptTypeParentExclusiveMandatory", BindingFlags.Public | BindingFlags.NonPublic).GetNestedType("ConceptTypeAbsorbedConceptTypeDeleteRule", BindingFlags.Public | BindingFlags.NonPublic)};
					OIALDomainModel.myCustomDomainModelTypes = retVal;
					System.Diagnostics.Debug.Assert(Array.IndexOf<Type>(retVal, null) < 0, "One or more rule types failed to resolve. The file and/or package will fail to load.");
				}
				return retVal;
			}
		}
		/// <summary>Generated code to attach <see cref="Microsoft.VisualStudio.Modeling.Rule"/>s to the <see cref="Microsoft.VisualStudio.Modeling.Store"/>.</summary>
		/// <seealso cref="Microsoft.VisualStudio.Modeling.DomainModel.GetCustomDomainModelTypes"/>
		protected override Type[] GetCustomDomainModelTypes()
		{
			if (ORMSolutions.ORMArchitect.Framework.FrameworkDomainModel.InitializingToolboxItems)
			{
				return Type.EmptyTypes;
			}
			Type[] retVal = base.GetCustomDomainModelTypes();
			int baseLength = retVal.Length;
			Type[] customDomainModelTypes = OIALDomainModel.CustomDomainModelTypes;
			if (baseLength <= 0)
			{
				return customDomainModelTypes;
			}
			else
			{
				Array.Resize<Type>(ref retVal, baseLength + customDomainModelTypes.Length);
				customDomainModelTypes.CopyTo(retVal, baseLength);
				return retVal;
			}
		}
		/// <summary>Implements IDomainModelEnablesRulesAfterDeserialization.EnableRulesAfterDeserialization</summary>
		protected void EnableRulesAfterDeserialization(Microsoft.VisualStudio.Modeling.Store store)
		{
			Microsoft.VisualStudio.Modeling.RuleManager ruleManager = store.RuleManager;
			Type[] disabledRuleTypes = OIALDomainModel.CustomDomainModelTypes;
			for (int i = 0; i < 19; ++i)
			{
				ruleManager.EnableRule(disabledRuleTypes[i]);
			}
		}
		void ORMSolutions.ORMArchitect.Framework.Shell.IDomainModelEnablesRulesAfterDeserialization.EnableRulesAfterDeserialization(Microsoft.VisualStudio.Modeling.Store store)
		{
			this.EnableRulesAfterDeserialization(store);
		}
	}
	#endregion // Attach rules to OIALDomainModel model
	#region Initially disable rules
	partial class OIALModel
	{
		partial class ModelHasObjectTypeAddRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasObjectTypeAddRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ModelHasObjectTypeDeletingRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasObjectTypeDeletingRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ObjectTypeChangeRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ObjectTypeChangeRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ObjectTypePlaysRoleAddRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ObjectTypePlaysRoleAddRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ObjectTypePlaysRoleDeletingRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ObjectTypePlaysRoleDeletingRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ModelHasFactTypeAddRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasFactTypeAddRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ModelHasFactTypeDeletingRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasFactTypeDeletingRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ModelHasSetConstraintAddRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasSetConstraintAddRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ModelHasSetConstraintDeletingRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ModelHasSetConstraintDeletingRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ConstraintRoleSequenceHasRoleAddRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ConstraintRoleSequenceHasRoleAddRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class ConstraintRoleSequenceHasRoleDeletingRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public ConstraintRoleSequenceHasRoleDeletingRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class UniquenessConstraintChangeRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public UniquenessConstraintChangeRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class MandatoryConstraintChangeRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public MandatoryConstraintChangeRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class RoleBaseChangeRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public RoleBaseChangeRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class SetConstraintChangeRule
		{
			[System.Diagnostics.DebuggerStepThrough()]
			public SetConstraintChangeRule()
			{
				base.IsEnabled = false;
			}
		}
	}
	partial class OIALModel
	{
		partial class CheckConceptTypeParentExclusiveMandatory
		{
			partial class OIALModelHasConceptTypeAddRule
			{
				[System.Diagnostics.DebuggerStepThrough()]
				public OIALModelHasConceptTypeAddRule()
				{
					base.IsEnabled = false;
				}
			}
		}
	}
	partial class OIALModel
	{
		partial class CheckConceptTypeParentExclusiveMandatory
		{
			partial class OIALModelHasConceptTypeDeleteRule
			{
				[System.Diagnostics.DebuggerStepThrough()]
				public OIALModelHasConceptTypeDeleteRule()
				{
					base.IsEnabled = false;
				}
			}
		}
	}
	partial class OIALModel
	{
		partial class CheckConceptTypeParentExclusiveMandatory
		{
			partial class ConceptTypeAbsorbedConceptTypeAddRule
			{
				[System.Diagnostics.DebuggerStepThrough()]
				public ConceptTypeAbsorbedConceptTypeAddRule()
				{
					base.IsEnabled = false;
				}
			}
		}
	}
	partial class OIALModel
	{
		partial class CheckConceptTypeParentExclusiveMandatory
		{
			partial class ConceptTypeAbsorbedConceptTypeDeleteRule
			{
				[System.Diagnostics.DebuggerStepThrough()]
				public ConceptTypeAbsorbedConceptTypeDeleteRule()
				{
					base.IsEnabled = false;
				}
			}
		}
	}
	#endregion // Initially disable rules
}
