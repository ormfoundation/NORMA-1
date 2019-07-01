#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
* Copyright � The ORM Foundation. All rights reserved.                     *
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

using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using ORMSolutions.ORMArchitect.Framework;
using ORMSolutions.ORMArchitect.Framework.Design;
using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace ORMSolutions.ORMArchitect.Core.ObjectModel.Design
{
    /// <summary>
    /// <see cref="ElementTypeDescriptor"/> for <see cref="ORMModelElement"/>s of type <typeparamref name="TModelElement"/>.
    /// </summary>
    /// <typeparam name="TModelElement">
    /// The type of the <see cref="ORMModelElement"/> that this <see cref="ORMModelElementTypeDescriptor{TModelElement}"/> is for.
    /// </typeparam>
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class ORMModelElementTypeDescriptor<TModelElement> : BlockRelationshipPropertiesElementTypeDescriptor<TModelElement>
		where TModelElement : ORMModelElement
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ORMModelElementTypeDescriptor{TModelElement}"/> for
		/// the instance of <typeparamref name="TModelElement"/> specified by <paramref name="selectedElement"/>.
		/// </summary>
		public ORMModelElementTypeDescriptor(ICustomTypeDescriptor parent, TModelElement selectedElement)
			: base(parent, selectedElement)
		{
		}

		/// <summary>
		/// Adds extension properties to the <see cref="PropertyDescriptorCollection"/> before returning it.
		/// </summary>
		/// <seealso cref="ElementTypeDescriptor{TModelElement}.GetProperties(Attribute[])"/>
		/// <seealso cref="ICustomTypeDescriptor.GetProperties(Attribute[])"/>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(attributes);

            // NOR-145: Incorporate Matt's changes to make sure the property window doesn't throw unnecessary exceptions
            TModelElement element = ModelElement;
            Store store = Utility.ValidateStore(element?.Store);
            if (store != null)
            {
                ExtendableElementUtility.GetExtensionProperties(ModelElement, properties, typeof(TModelElement));
            }

            return properties;
		}
	}
}
