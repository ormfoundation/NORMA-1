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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Neumont.Tools.Modeling.Design;
using Neumont.Tools.ORM.ObjectModel;

namespace Neumont.Tools.ORM.ObjectModel.Design
{
	/// <summary>
	/// <see cref="ElementTypeDescriptor"/> for <see cref="ExclusionConstraint"/>s.
	/// </summary>
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class ExclusionConstraintTypeDescriptor<TModelElement> : ORMModelElementTypeDescriptor<TModelElement>
		where TModelElement : ExclusionConstraint
	{
		/// <summary>
		/// Initializes a new instance of <see cref="MandatoryConstraintTypeDescriptor{TModelElement}"/>
		/// for <paramref name="selectedElement"/>.
		/// </summary>
		public ExclusionConstraintTypeDescriptor(ICustomTypeDescriptor parent, TModelElement selectedElement)
			: base(parent, selectedElement)
		{
		}

		/// <summary>
		/// Distinguish between disjunctive and simple <see cref="MandatoryConstraint"/>s in the property grid display.
		/// </summary>
		public override string GetClassName()
		{
			return (ModelElement.ExclusiveOrMandatoryConstraint == null) ? base.GetClassName() : ResourceStrings.ExclusiveOrConstraint;
		}
		/// <summary>
		/// Modify the display of the Name properties for constraints with an ExclusiveOrCoupler
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection descriptors = base.GetProperties(attributes);
			MandatoryConstraint mandatoryConstraint = ModelElement.ExclusiveOrMandatoryConstraint;
			if (mandatoryConstraint != null)
			{
				int descriptorCount = descriptors.Count;
				for (int i = 0; i < descriptorCount; ++i)
				{
					ElementPropertyDescriptor currentDescriptor = descriptors[i] as ElementPropertyDescriptor;
					if (currentDescriptor != null && currentDescriptor.DomainPropertyInfo.Id == ExclusionConstraint.NameDomainPropertyId)
					{
						descriptors.RemoveAt(i);
						descriptors.Add(new CustomDisplayPropertyDescriptor(currentDescriptor, ResourceStrings.ExclusiveOrConstraintExclusionConstraintNameDisplayName, null, null));
						descriptors.Add(new CustomDisplayPropertyDescriptor(CreatePropertyDescriptor(mandatoryConstraint, currentDescriptor.DomainPropertyInfo, null), ResourceStrings.ExclusiveOrConstraintMandatoryConstraintNameDisplayName, null, null));
						break;
					}
				}
			}
			return descriptors;
		}
	}
}