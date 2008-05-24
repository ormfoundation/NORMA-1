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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;
using Neumont.Tools.Modeling;
using System.Collections.ObjectModel;

namespace Neumont.Tools.ORM.ObjectModel
{
	public partial class Objectification
	{
		// UNDONE: Handle unary objectifications (both implied and explicit)
		#region Implied Objectification creation, removal, and pattern enforcement
		#region ImpliedObjectificationConstraintRoleSequenceHasRoleAddRule
		/// <summary>
		/// AddRule: typeof(ConstraintRoleSequenceHasRole)
		/// Creates a new implied Objectification if the implied objectification pattern is now present.
		/// </summary>
		private static void ImpliedObjectificationConstraintRoleSequenceHasRoleAddRule(ElementAddedEventArgs e)
		{
			ConstraintRoleSequenceHasRole link = e.ModelElement as ConstraintRoleSequenceHasRole;
			UniquenessConstraint constraint;
			FactType factType;
			if (null != (constraint = link.ConstraintRoleSequence as UniquenessConstraint) &&
				constraint.IsInternal &&
				null != (factType = link.Role.FactType))
			{
				FrameworkDomainModel.DelayValidateElement(factType, DelayProcessFactTypeForImpliedObjectification);
				ObjectType nestingType = factType.NestingType;
				if (nestingType != null)
				{
					FrameworkDomainModel.DelayValidateElement(nestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
				}
			}
		}
		#endregion // ImpliedObjectificationConstraintRoleSequenceHasRoleAddRule
		#region ImpliedObjectificationConstraintRoleSequenceHasRoleDeletingRule
		/// <summary>
		/// DeletingRule: typeof(ConstraintRoleSequenceHasRole)
		/// Removes an existing implied Objectification if the implied objectification pattern is no longer present.
		/// </summary>
		private static void ImpliedObjectificationConstraintRoleSequenceHasRoleDeletingRule(ElementDeletingEventArgs e)
		{
			ConstraintRoleSequenceHasRole link = e.ModelElement as ConstraintRoleSequenceHasRole;
			UniquenessConstraint constraint;
			FactType factType;
			if (null != (constraint = link.ConstraintRoleSequence as UniquenessConstraint) &&
				constraint.IsInternal &&
				null != (factType = link.Role.FactType))
			{
				FrameworkDomainModel.DelayValidateElement(factType, DelayProcessFactTypeForImpliedObjectification);
			}
		}
		#endregion // ImpliedObjectificationConstraintRoleSequenceHasRoleDeletingRule
		#region ImpliedObjectificationFactTypeHasRoleAddRule
		/// <summary>
		/// AddRule: typeof(FactTypeHasRole)
		/// 1) Creates a new implied Objectification if the implied objectification pattern is now present.
		/// 2) Changes an implied Objectification to being explicit if a Role in a non-implied FactType is played.
		/// </summary>
		private static void ImpliedObjectificationFactTypeHasRoleAddRule(ElementAddedEventArgs e)
		{
			FactTypeHasRole factTypeHasRole = e.ModelElement as FactTypeHasRole;
			FrameworkDomainModel.DelayValidateElement(factTypeHasRole.FactType, DelayProcessFactTypeForImpliedObjectification);
			ProcessNewPlayedRoleForImpliedObjectification(factTypeHasRole.Role as Role);
		}
		#endregion // ImpliedObjectificationFactTypeHasRoleAddRule
		#region ImpliedObjectificationFactTypeHasRoleDeletingRule
		/// <summary>
		/// DeletingRule: typeof(FactTypeHasRole)
		/// Removes an existing implied Objectification if the implied objectification pattern is no longer present.
		/// </summary>
		private static void ImpliedObjectificationFactTypeHasRoleDeletingRule(ElementDeletingEventArgs e)
		{
			FactType factType = (e.ModelElement as FactTypeHasRole).FactType;
			if (!factType.IsDeleting)
			{
				FrameworkDomainModel.DelayValidateElement(factType, DelayProcessFactTypeForImpliedObjectification);
				Objectification objectification;
				if (null != (objectification = factType.Objectification) &&
					!objectification.IsDeleting)
				{
					FrameworkDomainModel.DelayValidateElement(objectification.NestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
				}
			}
		}
		#endregion // ImpliedObjectificationFactTypeHasRoleDeletingRule
		#region ImpliedObjectificationUniquenessConstraintChangeRule
		/// <summary>
		/// ChangeRule: typeof(UniquenessConstraint)
		/// Adds or removes an implied Objectification if necessary as well as ensuring
		/// that an objectifying type with a single candidate internal uniqueness
		/// constraint on the objectified fact uses that constraint as its preferred identifier.
		/// </summary>
		private static void ImpliedObjectificationUniquenessConstraintChangeRule(ElementPropertyChangedEventArgs e)
		{
			UniquenessConstraint constraint = e.ModelElement as UniquenessConstraint;
			if (!constraint.IsDeleted)
			{
				Guid attributeId = e.DomainProperty.Id;
				if (attributeId == UniquenessConstraint.IsInternalDomainPropertyId)
				{
					ProcessUniquenessConstraintForImpliedObjectification(constraint, true, false);
				}
				else if (attributeId == UniquenessConstraint.ModalityDomainPropertyId)
				{
					if (constraint.IsInternal)
					{
						ProcessUniquenessConstraintForImpliedObjectification(constraint, false, true);
					}
				}
			}
		}
		#endregion // ImpliedObjectificationUniquenessConstraintChangeRule
		#region UniquenessConstraintAddRule
		/// <summary>
		/// AddRule: typeof(ModelHasSetConstraint)
		/// Ensure that an objectifying type with a single candidate internal uniqueness
		/// constraint on the objectified fact uses that constraint as its preferred identifier.
		/// </summary>
		private static void UniquenessConstraintAddRule(ElementAddedEventArgs e)
		{
			UniquenessConstraint constraint = (e.ModelElement as ModelHasSetConstraint).SetConstraint as UniquenessConstraint;
			if (constraint != null && constraint.IsInternal)
			{
				LinkedElementCollection<FactType> facts = constraint.FactTypeCollection;
				if (facts.Count != 0)
				{
					ObjectType nestingType = facts[0].NestingType;
					if (nestingType != null)
					{
						FrameworkDomainModel.DelayValidateElement(nestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
					}
				}
			}
		}
		#endregion // UniquenessConstraintAddRule
		#region UniquessConstraintDeletingRule
		/// <summary>
		/// DeletingRule: typeof(ModelHasSetConstraint)
		/// Ensure that an objectifying type with a single candidate internal uniqueness
		/// constraint on the objectified fact uses that constraint as its preferred identifier.
		/// </summary>
		private static void UniquenessConstraintDeletingRule(ElementDeletingEventArgs e)
		{
			UniquenessConstraint constraint = (e.ModelElement as ModelHasSetConstraint).SetConstraint as UniquenessConstraint;
			if (constraint != null && constraint.IsInternal)
			{
				LinkedElementCollection<FactType> facts = constraint.FactTypeCollection;
				FactType testFact;
				Objectification objectification;
				if (facts.Count != 0 &&
					!(testFact = facts[0]).IsDeleting &&
					null != (objectification = testFact.Objectification) &&
					!objectification.IsDeleting)
				{
					FrameworkDomainModel.DelayValidateElement(objectification.NestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
				}
			}
		}
		#endregion // UniquessConstraintDeletingRule
		#region PreferredIdentifierDeletingRule
		/// <summary>
		/// DeletingRule: typeof(EntityTypeHasPreferredIdentifier)
		/// Make sure than an objectifying type gets a preferred identifier if
		/// the existing preferred identifier is deleted and a single internal
		/// uniqueness constraint on the objectified fact is available.
		/// </summary>
		private static void PreferredIdentifierDeletingRule(ElementDeletingEventArgs e)
		{
			ProcessPreferredIdentifierDeleting(e.ModelElement as EntityTypeHasPreferredIdentifier, null);
		}
		/// <summary>
		/// Rule helper method
		/// </summary>
		private static void ProcessPreferredIdentifierDeleting(EntityTypeHasPreferredIdentifier link, ObjectType objectType)
		{
			if (objectType == null)
			{
				objectType = link.PreferredIdentifierFor;
			}
			Objectification objectification;
			if (!objectType.IsDeleting &&
				null != (objectification = objectType.Objectification) &&
				!objectification.IsDeleting)
			{
				FrameworkDomainModel.DelayValidateElement(objectType, DelayProcessObjectifyingTypeForPreferredIdentifier);
			}
		}
		#endregion // PreferredIdentifierDeletingRule
		#region PreferredIdentifierRolePlayerChangeRule
		/// <summary>
		/// RolePlayerChangeRule: typeof(EntityTypeHasPreferredIdentifier)
		/// RolePlayerChangeRule corresponding to <see cref="PreferredIdentifierDeletingRule"/>
		/// </summary>
		private static void PreferredIdentifierRolePlayerChangeRule(RolePlayerChangedEventArgs e)
		{
			Guid changedRoleGuid = e.DomainRole.Id;
			ObjectType oldObjectType = null;
			if (changedRoleGuid == EntityTypeHasPreferredIdentifier.PreferredIdentifierForDomainRoleId)
			{
				oldObjectType = (ObjectType)e.OldRolePlayer;
			}
			ProcessPreferredIdentifierDeleting(e.ElementLink as EntityTypeHasPreferredIdentifier, oldObjectType);
		}
		#endregion // PreferredIdentifierRolePlayerChangeRule
		#region ImpliedObjectificationIsImpliedChangeRule
		/// <summary>
		/// ChangeRule: typeof(Objectification)
		/// Validates that an objectification that is implied matches the implied objectification pattern.
		/// </summary>
		private static void ImpliedObjectificationIsImpliedChangeRule(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == Objectification.IsImpliedDomainPropertyId && (bool)e.NewValue)
			{
				Objectification objectification = e.ModelElement as Objectification;

				// Check the implication pattern
				ProcessFactTypeForImpliedObjectification(objectification.NestedFactType, true);

				// If we're still objectified, check that we are only doing things that are allowed
				if (objectification.IsImplied)
				{
					ObjectType nestingType = objectification.NestingType;
					if (nestingType != null && ((!nestingType.IsIndependent && nestingType.AllowIsIndependent()) || nestingType.PlayedRoleCollection.Count != objectification.ImpliedFactTypeCollection.Count))
					{
						throw InvalidImpliedObjectificationException();
					}
				}
			}
		}
		#endregion // ImpliedObjectificationIsImpliedChangeRule
		#region ImpliedObjectificationObjectifyingTypeIsIndependentChangeRule
		/// <summary>
		/// ChangeRule: typeof(ObjectType)
		/// Changes an implied Objectification to being explicit if IsIndependent is changed.
		/// </summary>
		private static void ImpliedObjectificationObjectifyingTypeIsIndependentChangeRule(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == ObjectType.IsIndependentDomainPropertyId && !(bool)e.NewValue)
			{
				ObjectType nestingType = e.ModelElement as ObjectType;
				FactType nestedFact = nestingType.NestedFactType;
				Objectification objectification;
				UniquenessConstraint preferredIdentifier;
				if (nestedFact != null &&
					(objectification = nestedFact.Objectification).IsImplied &&
					null != (preferredIdentifier = nestingType.PreferredIdentifier) &&
					preferredIdentifier.RoleCollection.Count == nestedFact.RoleCollection.Count)
				{
					objectification.IsImplied = false;
				}
			}
		}
		#endregion // ImpliedObjectificationObjectifyingTypeIsIndependentChangeRule
		#region ImpliedObjectificationObjectifyingTypePlaysRoleAddRule
		/// <summary>
		/// AddRule: typeof(ObjectTypePlaysRole)
		/// Changes an implied Objectification to being explicit if a Role in a non-implied FactType is played.
		/// </summary>
		private static void ImpliedObjectificationObjectifyingTypePlaysRoleAddRule(ElementAddedEventArgs e)
		{
			ProcessNewPlayedRoleForImpliedObjectification((e.ModelElement as ObjectTypePlaysRole).PlayedRole);
		}
		#endregion // ImpliedObjectificationObjectifyingTypePlaysRoleAddRule
		#endregion // Implied Objectification creation, removal, and pattern enforcement
		#region Objectification implied facts and constraints pattern enforcement
		#region ObjectificationAddRule
		/// <summary>
		/// AddRule: typeof(Objectification)
		/// Create implied facts and constraints when an item is objectified
		/// </summary>
		private static void ObjectificationAddRule(ElementAddedEventArgs e)
		{
			ProcessObjectificationAdded(e.ModelElement as Objectification, null, null);
		}
		/// <summary>
		/// Create implied facts and constraints as needed
		/// </summary>
		/// <param name="objectification">The objectification relationship to process</param>
		/// <param name="nestedFactType">The nested fact to process. Pulled from objectification.NestedFactType if null.</param>
		/// <param name="nestingType">The nesting object type to process. Pulled from objectification.NestingType if null.</param>
		private static void ProcessObjectificationAdded(Objectification objectification, FactType nestedFactType, ObjectType nestingType)
		{
			if (nestedFactType == null)
			{
				nestedFactType = objectification.NestedFactType;
			}
			if (nestedFactType.ImpliedByObjectification != null)
			{
				throw new InvalidOperationException(ResourceStrings.ModelExceptionObjectificationImpliedFactObjectified);
			}
			if (nestingType == null)
			{
				nestingType = objectification.NestingType;
			}
			FrameworkDomainModel.DelayValidateElement(nestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
			Store store = nestedFactType.Store;
			ORMModel model = nestedFactType.Model;

			// Comments in this and other related procedures will refer to
			// the 'near' end and 'far' end of the implied elements. The
			// near end refers to the nested fact type and its role players, the
			// far end is the nesting type. The pattern specifies that a binary fact type
			// is implied between each role player and the objectified type. The far
			// role players on the implied fact types always have a single role internal
			// constraint and a simple mandatory constraint.

			// Once we have a model, we can begin to add implied
			// facts and constraints. Note that we do not set the
			// ImpliedByObjectification property on any object
			// until all are completed because any modifications
			// to these implied elements is strictly monitored once
			// this relationship is established.

			// Add implied fact types, one for each role
			LinkedElementCollection<RoleBase> roles = nestedFactType.RoleCollection;
			int roleCount = roles.Count;
			if (roleCount != 0)
			{
				bool ruleDisabled = false;
				try
				{
					int? unaryRoleIndex = FactType.GetUnaryRoleIndex(roles);
					Role unaryRole = (unaryRoleIndex.HasValue) ? roles[unaryRoleIndex.Value].Role : null;
					for (int i = 0; i < roleCount; ++i)
					{
						Role role = roles[i].Role;
						RoleProxy proxy = role.Proxy;
						if (proxy == null)
						{
							if (unaryRole == null || role == unaryRole)
							{
								CreateImpliedFactTypeForRole(model, nestingType, role, objectification, unaryRole != null);
							}
						}
						else
						{
							RoleBase oppositeRoleBase;
							Role oppositeRole;
							if (null != (oppositeRoleBase = proxy.OppositeRole) &&
								null != (oppositeRole = oppositeRoleBase as Role) &&
								(nestingType != oppositeRole.RolePlayer))
							{
								// Move an existing proxy fact to the correct nesting type
								if (!ruleDisabled)
								{
									store.RuleManager.DisableRule(typeof(RolePlayerAddRuleClass));
									ruleDisabled = true;
								}
								oppositeRole.RolePlayer = nestingType;
							}
						}
					}
				}
				finally
				{
					if (ruleDisabled)
					{
						store.RuleManager.EnableRule(typeof(RolePlayerAddRuleClass));
					}
				}
			}
		}
		#endregion // ObjectificationAddRule
		#region ObjectificationDeleteRule
		/// <summary>
		/// DeletingRule: typeof(Objectification)
		/// Remove the implied objectifying ObjectType when Objectification is removed.
		/// </summary>
		private static void ObjectificationDeletingRule(ElementDeletingEventArgs e)
		{
			ProcessObjectificationDeleting(e.ModelElement as Objectification, null, null);
		}
		/// <summary>
		/// Remove the implied objectifying ObjectType when Objectification is removed
		/// and delay validated the previously nested fact type.
		/// </summary>
		/// <param name="objectification">The objectification relationship to process</param>
		/// <param name="nestedFactType">The nested fact to process. Pulled from objectification.NestedFactType if null.</param>
		/// <param name="nestingType">The nesting object type to process. Pulled from objectification.NestingType if null.</param>
		private static void ProcessObjectificationDeleting(Objectification objectification, FactType nestedFactType, ObjectType nestingType)
		{
			if (nestingType == null)
			{
				nestingType = objectification.NestingType;
			}
			if (objectification.IsImplied)
			{
				if (nestedFactType == null)
				{
					nestedFactType = objectification.NestedFactType;
				}
				if (nestingType.IsDeleting)
				{
					if (!(nestedFactType.IsDeleting || nestedFactType.IsDeleted))
					{
						// If the fact type should still be explicitly objectified, we
						// have two choices at this point:
						// 1) Check the pattern here and throw if it is still needed
						// 2) Check the pattern later and regenerated the implied objectification
						// Checking the implicit objectification pattern here is difficult due
						// to the 'deleting' state, and the exception is not technically necessary
						// given that the objects being lost are auto-generated in the first place.
						FrameworkDomainModel.DelayValidateElement(nestedFactType, DelayProcessFactTypeForImpliedObjectification);
					}
				}
				if (!(nestingType.IsDeleting || nestingType.IsDeleted))
				{
					nestingType.Delete();
				}
			}
			else
			{
				if (nestedFactType == null)
				{
					nestedFactType = objectification.NestedFactType;
				}
				// Treat an objectification relationship as a two way
				// delete propagation relationship if either end is deleted. This
				// allows us to treat the two objects as one except when they are
				// explicitly disconnected.
				if (nestedFactType.IsDeleting || nestedFactType.IsDeleted)
				{
					if (!(nestingType.IsDeleting || nestingType.IsDeleted))
					{
						nestingType.Delete();
					}
				}
				else if (nestingType.IsDeleting || nestingType.IsDeleted)
				{
					if (!(nestedFactType.IsDeleting || nestedFactType.IsDeleted))
					{
						nestedFactType.Delete();
					}
				}
				else
				{
					FrameworkDomainModel.DelayValidateElement(nestedFactType, DelayProcessFactTypeForImpliedObjectification);
				}
			}
		}
		#endregion // ObjectificationDeletingRule
		#region ObjectificationRolePlayerChangeRule
		/// <summary>
		/// RolePlayerChangeRule: typeof(Objectification)
		/// Process Objectification role player changes
		/// </summary>
		private static void ObjectificationRolePlayerChangeRule(RolePlayerChangedEventArgs e)
		{
			Objectification link = e.ElementLink as Objectification;
			if (link.IsDeleted)
			{
				return;
			}
			Guid changedRoleGuid = e.DomainRole.Id;
			ObjectType oldObjectType = null;
			FactType oldFactType = null;
			if (changedRoleGuid == Objectification.NestingTypeDomainRoleId)
			{
				oldObjectType = (ObjectType)e.OldRolePlayer;
			}
			else
			{
				oldFactType = (FactType)e.OldRolePlayer;
			}
			RuleManager ruleManager = link.Store.RuleManager;
			try
			{
				ruleManager.DisableRule(typeof(RoleDeletingRuleClass));
				link.ImpliedFactTypeCollection.Clear();
			}
			finally
			{
				ruleManager.EnableRule(typeof(RoleDeletingRuleClass));
			}
			ProcessObjectificationDeleting(link, oldFactType, oldObjectType);
			ProcessObjectificationAdded(link, null, null);
		}
		#endregion // ObjectificationRolePlayerChangeRule
		#region ImpliedFactTypeAddRule
		/// <summary>
		/// AddRule: typeof(ObjectificationImpliesFactType)
		/// Rule to block objectification of implied facts
		/// </summary>
		private static void ImpliedFactTypeAddRule(ElementAddedEventArgs e)
		{
			ObjectificationImpliesFactType link = e.ModelElement as ObjectificationImpliesFactType;
			if (link.ImpliedFactType.Objectification != null)
			{
				throw new InvalidOperationException(ResourceStrings.ModelExceptionObjectificationImpliedFactObjectified);
			}
		}
		#endregion // ImpliedFactTypeAddRule
		#region RoleAddRule
		/// <summary>
		/// AddRule: typeof(FactTypeHasRole)
		/// AddRule: typeof(ConstraintRoleSequenceHasRole)
		/// AddRule: typeof(FactSetConstraint)
		/// Synchronize implied fact types when a role is added
		/// to the nested fact type or an attached constraint.
		/// </summary>
		private static void RoleAddRule(ElementAddedEventArgs e)
		{
			ModelElement element = e.ModelElement;
			ConstraintRoleSequenceHasRole sequenceRoleLink;
			FactSetConstraint internalConstraintLink;
			FactType fact;
			Objectification objectificationLink;
			bool disallowed = false;
			if (null != (sequenceRoleLink = element as ConstraintRoleSequenceHasRole))
			{
				ConstraintRoleSequence modifiedSequence = sequenceRoleLink.ConstraintRoleSequence;
				IConstraint constraint = modifiedSequence.Constraint;
				if (constraint != null)
				{
					ConstraintType constraintType = constraint.ConstraintType;
					switch (constraintType)
					{
						case ConstraintType.SimpleMandatory:
						case ConstraintType.InternalUniqueness:
							// Do not allow direct modification. This rule is disabled
							// when constraints on existing fact types are modified
							LinkedElementCollection<FactType> facts = (constraint as SetConstraint).FactTypeCollection;
							if (facts.Count == 1)
							{
								fact = facts[0];
								if (null != fact.ImpliedByObjectification)
								{
									disallowed = true; // We don't trigger adds when this rule is active
								}
							}
							break;
					}
				}
			}
			else if (null != (internalConstraintLink = element as FactSetConstraint))
			{
				if (internalConstraintLink.SetConstraint.Constraint.ConstraintIsInternal)
				{
					disallowed = null != internalConstraintLink.FactType.ImpliedByObjectification;
				}
			}
			else
			{
				FactTypeHasRole factRoleLink = element as FactTypeHasRole;
				fact = factRoleLink.FactType;
				if (null != (objectificationLink = fact.ImpliedByObjectification))
				{
					// Our code only adds these before linking the implied objectification,
					// so we always throw at this point
					disallowed = true;
				}
				else if (null != (objectificationLink = fact.Objectification))
				{
					ObjectType nestingType = objectificationLink.NestingType;
					Role nestedRole = factRoleLink.Role.Role;

					// Create and populate new fact type
					if (nestedRole.Proxy == null)
					{
						Role unaryRole = fact.UnaryRole;
						if (unaryRole == null || nestedRole == unaryRole)
						{
							CreateImpliedFactTypeForRole(nestingType.Model, nestingType, nestedRole, objectificationLink, unaryRole != null);
						}
					}
				}
			}

			// Throw if the modification was disallowed by the objectification pattern
			if (disallowed)
			{
				throw BlockedByObjectificationPatternException();
			}
		}
		#endregion // RoleAddRule
		#region RoleDeletingRule
		partial class RoleDeletingRuleClass
		{
			private bool myAllowModification;
			/// <summary>
			/// DeletingRule: typeof(FactTypeHasRole)
			/// DeletingRule: typeof(ConstraintRoleSequenceHasRole)
			/// Synchronize implied fact types when a role is removed from
			/// the nested type.
			/// </summary>
			private void RoleDeletingRule(ElementDeletingEventArgs e)
			{
				ModelElement element = e.ModelElement;
				ConstraintRoleSequenceHasRole sequenceRoleLink;
				FactType fact;
				Objectification objectificationLink;
				bool disallowed = false;
				if (null != (sequenceRoleLink = element as ConstraintRoleSequenceHasRole))
				{
					if (!sequenceRoleLink.Role.IsDeleting)
					{
						ConstraintRoleSequence sequence = sequenceRoleLink.ConstraintRoleSequence;
						IConstraint constraint = sequence.Constraint;
						if (constraint != null)
						{
							ConstraintType constraintType = constraint.ConstraintType;
							switch (constraintType)
							{
								case ConstraintType.SimpleMandatory:
								case ConstraintType.InternalUniqueness:
									// Do not allow direct modification. This rule is disabled
									// when constraints on existing fact types are modified
									LinkedElementCollection<FactType> facts = (constraint as SetConstraint).FactTypeCollection;
									if (facts.Count == 1)
									{
										fact = facts[0];
										if (null != (objectificationLink = fact.ImpliedByObjectification))
										{
											disallowed = !myAllowModification && !objectificationLink.IsDeleting;
										}
									}
									break;
							}
						}
					}
				}
				else
				{
					FactTypeHasRole factRoleLink = element as FactTypeHasRole;
					fact = factRoleLink.FactType;
					if (null != (objectificationLink = fact.ImpliedByObjectification))
					{
						// Our code only adds these before linking the implied objectification,
						// so we always throw at this point
						if (!myAllowModification && !objectificationLink.IsDeleting)
						{
							disallowed = true;
							RoleBase nestedRoleBase = factRoleLink.Role;
							RoleProxy proxy;
							ObjectifiedUnaryRole objectifiedUnaryRole;
							Role targetRole;
							if ((null != (proxy = nestedRoleBase as RoleProxy) &&
								null != (targetRole = proxy.Role)) ||
								(null != (objectifiedUnaryRole = nestedRoleBase as ObjectifiedUnaryRole) &&
								null != (targetRole = objectifiedUnaryRole.TargetRole)))
							{
								disallowed = !targetRole.IsDeleting;
							}
						}
					}
					else if (null != (objectificationLink = fact.Objectification))
					{
						if (!objectificationLink.IsDeleting)
						{
							try
							{
								myAllowModification = true;
								Role nestedRole = factRoleLink.Role.Role;
								RoleProxy proxyRole;
								ObjectifiedUnaryRole objectifiedUnaryRole;
								if (null != (proxyRole = nestedRole.Proxy))
								{
									proxyRole.FactType.Delete();
								}
								else if (null != (objectifiedUnaryRole = nestedRole.ObjectifiedUnaryRole))
								{
									objectifiedUnaryRole.FactType.Delete();
								}
							}
							finally
							{
								myAllowModification = false;
							}
						}
					}
				}

				// Throw if the modification was disallowed by the objectification pattern
				if (disallowed)
				{
					throw BlockedByObjectificationPatternException();
				}
			}
		}
		#endregion // RoleDeletingRule
		#region RolePlayerAddRule
		/// <summary>
		/// AddRule: typeof(ObjectTypePlaysRole)
		/// Synchronize implied fact types when a role player is
		/// set on an objectified role
		/// </summary>
		private static void RolePlayerAddRule(ElementAddedEventArgs e)
		{
			ObjectTypePlaysRole link = e.ModelElement as ObjectTypePlaysRole;
			Role role = link.PlayedRole;
			FactType factType = role.FactType;
			if (factType != null)
			{
				ObjectifiedUnaryRole objectifiedUnaryRole;
				if (null != factType.ImpliedByObjectification)
				{
					if (null == (objectifiedUnaryRole = role as ObjectifiedUnaryRole) ||
						null == (role = objectifiedUnaryRole.TargetRole) ||
						role.RolePlayer != link.RolePlayer)
					{
						throw BlockedByObjectificationPatternException();
					}
				}
				else if (null != (objectifiedUnaryRole = role.ObjectifiedUnaryRole))
				{
					objectifiedUnaryRole.RolePlayer = link.RolePlayer;
				}
			}
		}
		#endregion // RolePlayerAddRule
		#region RolePlayerDeletingRule
		/// <summary>
		/// DeletingRule: typeof(ObjectTypePlaysRole)
		/// Synchronize implied fact types when a role player is
		/// being removed from an objectified role
		/// </summary>
		private static void RolePlayerDeletingRule(ElementDeletingEventArgs e)
		{
			ObjectTypePlaysRole link = e.ModelElement as ObjectTypePlaysRole;
			Role role = link.PlayedRole;
			ObjectType rolePlayer = link.RolePlayer;
			FactType factType;
			Objectification objectification;
			// Note if the roleplayer is removed, then the links all go away
			// automatically. There is no additional work to do or checks to make.
			if (rolePlayer.IsDeleted || rolePlayer.IsDeleting)
			{
				if (rolePlayer.IsImplicitBooleanValue &&
					null != (factType = role.FactType) &&
					null != (objectification = factType.Objectification))
				{
					foreach (RoleBase otherRoleBase in factType.RoleCollection)
					{
						// Find the old unary role and modify its implied FactType
						Role objectifiedUnaryRole;
						FactType impliedFactType;
						Role otherRole;
						if (otherRoleBase != role &&
							!role.IsDeleting &&
							null != (objectifiedUnaryRole = (otherRole = otherRoleBase.Role).ObjectifiedUnaryRole) &&
							!objectifiedUnaryRole.IsDeleting &&
							null != (impliedFactType = objectifiedUnaryRole.FactType))
						{
							ORMModel model = impliedFactType.Model;
							RuleManager ruleManager = model.Store.RuleManager;
							try
							{
								ruleManager.DisableRule(typeof(RoleDeletingRuleClass));
								impliedFactType.Delete();
							}
							finally
							{
								ruleManager.EnableRule(typeof(RoleDeletingRuleClass));
							}
							ObjectType nestingType = objectification.NestingType;
							CreateImpliedFactTypeForRole(model, nestingType, otherRole, objectification, false);
							if (!role.IsDeleting && role.Proxy == null)
							{
								CreateImpliedFactTypeForRole(model, nestingType, role, objectification, false);
							}
							break;
						}
					}
				}
			}
			else if (null != (factType = role.FactType))
			{
				SubtypeMetaRole subtypeRole;
				ObjectifiedUnaryRole objectifiedUnaryRole;
				if (null != (objectification = factType.ImpliedByObjectification))
				{
					Role targetRole;
					ObjectTypePlaysRole targetLink;
					if (!(objectification.IsDeleting || role.IsDeleting) &&
						(null == (objectifiedUnaryRole = role as ObjectifiedUnaryRole) ||
						null == (targetRole = objectifiedUnaryRole.TargetRole) ||
						(null != (targetLink = ObjectTypePlaysRole.GetLinkToRolePlayer(targetRole)) && !targetLink.IsDeleting)))
					{
						throw BlockedByObjectificationPatternException();
					}
				}
				else if (null != (subtypeRole = role as SubtypeMetaRole))
				{
					// We don't force preferred identifiers when supertypes are in place.
					// If this is the last supertype then we need to revalidate for preferred identification.
					FrameworkDomainModel.DelayValidateElement(rolePlayer, DelayProcessObjectifyingTypeForPreferredIdentifier);
				}
				else if (null != (objectifiedUnaryRole = role.ObjectifiedUnaryRole))
				{
					objectifiedUnaryRole.RolePlayer = null;
				}
			}
		}
		#endregion // RolePlayerDeletingRule
		#region RolePlayerRolePlayerChangeRule
		/// <summary>
		/// RolePlayerChangeRule: typeof(ObjectTypePlaysRole)
		/// </summary>
		private static void RolePlayerRolePlayerChangeRule(RolePlayerChangedEventArgs e)
		{
			ObjectTypePlaysRole link = e.ElementLink as ObjectTypePlaysRole;
			Role role = link.PlayedRole;
			FactType factType;
			if (null != (factType = role.FactType))
			{
				bool disallowed = false;
				Objectification objectification;
				ObjectifiedUnaryRole objectifiedUnaryRole;
				if (null != (objectification = factType.ImpliedByObjectification))
				{
					if (null != (objectifiedUnaryRole = role.ObjectifiedUnaryRole))
					{
						objectifiedUnaryRole.RolePlayer = link.RolePlayer;
					}
					else if (null != (objectifiedUnaryRole = role as ObjectifiedUnaryRole))
					{
						if (objectifiedUnaryRole.TargetRole.RolePlayer != link.RolePlayer)
						{
							disallowed = true;
						}
					}
					else
					{
						disallowed = true;
					}
				}
				else if (null != (objectification = factType.Objectification))
				{
					RoleProxy proxy;
					Role impliedOppositeRole;
					if (null != (proxy = role.Proxy) &&
						null != (impliedOppositeRole = proxy.OppositeRole as Role))
					{
						if (impliedOppositeRole.RolePlayer != objectification.NestingType)
						{
							disallowed = true;
						}
					}
					else if (null != (objectifiedUnaryRole = role.ObjectifiedUnaryRole))
					{
						if (null != (impliedOppositeRole = objectifiedUnaryRole.OppositeRole as Role) &&
							impliedOppositeRole.RolePlayer != objectification.NestingType)
						{
							disallowed = true;
						}
						else
						{
							objectifiedUnaryRole.RolePlayer = link.RolePlayer;
						}
					}
				}
				if (disallowed)
				{
					throw BlockedByObjectificationPatternException();
				}
			}
		}
		#endregion // RolePlayerRolePlayerChangeRule
		#region InternalConstraintChangeRule
		/// <summary>
		/// ChangeRule: typeof(SetConstraint)
		/// Ensure that implied internal constraints cannot change the Modality property
		/// </summary>
		private static void InternalConstraintChangeRule(ElementPropertyChangedEventArgs e)
		{
			Guid attributeId = e.DomainProperty.Id;
			if (attributeId == SetConstraint.ModalityDomainPropertyId)
			{
				SetConstraint constraint = e.ModelElement as SetConstraint;
				if (constraint.Constraint.ConstraintIsInternal)
				{
					LinkedElementCollection<FactType> facts;
					if (1 == (facts = constraint.FactTypeCollection).Count &&
						facts[0].ImpliedByObjectification != null)
					{
						throw BlockedByObjectificationPatternException();
					}
				}
			}
		}
		#endregion // InternalConstraintChangeRule
		#endregion // Objectification implied facts and constraints pattern enforcement
		#region Helper functions
		/// <summary>
		/// Determines if the specified <see cref="FactType"/> requires an implied <see cref="Objectification"/>,
		/// ignoring any <see cref="Objectification"/> that is already present.
		/// </summary>
		/// <remarks>
		/// A <see cref="FactType"/> is considered to require an implied <see cref="Objectification"/> if it has
		/// more than two <see cref="Role"/>s or if it has an <see cref="ConstraintModality.Alethic"/>
		/// <see cref="UniquenessConstraint"/> that spans more than one role.
		/// </remarks>
		private static bool IsImpliedObjectificationRequired(FactType factType)
		{
			// See if we have more than two roles
			if (factType.RoleCollection.Count > 2)
			{
				return true;
			}

			// See if we have an "internal" alethic uniqueness constraint that spans more than one role
			foreach (UniquenessConstraint uniquenessConstraint in factType.GetInternalConstraints<UniquenessConstraint>())
			{
				if (uniquenessConstraint.Modality == ConstraintModality.Alethic &&
					uniquenessConstraint.RoleCollection.Count > 1)
				{
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// If a newly played <see cref="Role"/> is in a non-implied <see cref="FactType"/>, then the role player
		/// cannot be an implied objectifying <see cref="ObjectType"/>.
		/// </summary>
		private static void ProcessNewPlayedRoleForImpliedObjectification(Role playedRole)
		{
			if (playedRole != null)
			{
				FactType playedFact = playedRole.FactType;
				if (playedFact != null)
				{
					LinkedElementCollection<RoleBase> roles;
					// If the fact is implied, we don't need to do anything else
					if (playedFact.ImpliedByObjectification != null)
					{
						return;
					}
					else if ((roles = playedFact.RoleCollection).Count > 0)
					{
						foreach (RoleBase testRole in roles)
						{
							if (testRole is RoleProxy || testRole is ObjectifiedUnaryRole)
							{
								return;
							}
						}
					}

					ObjectType rolePlayer = playedRole.RolePlayer;
					Objectification objectification;
					if (rolePlayer != null)
					{
						if (rolePlayer.IsImplicitBooleanValue)
						{
							if (null != (objectification = playedFact.Objectification) &&
								!objectification.IsImplied)
							{
								// Note that implied objectification will be cleared out with a different
								// code path because unary objectification is never implied
								LinkedElementCollection<FactType> impliedFactTypes = objectification.ImpliedFactTypeCollection;
								bool haveImplicitForUnary = false;
								RuleManager disabledByRuleManager = null;
								try
								{
									for (int i = impliedFactTypes.Count - 1; i >= 0; --i)
									{
										FactType impliedFactType = impliedFactTypes[i];
										bool sawProxy = false;
										foreach (RoleBase role in impliedFactType.RoleCollection)
										{
											if (role is RoleProxy)
											{
												sawProxy = true;
												if (disabledByRuleManager == null)
												{
													disabledByRuleManager = impliedFactType.Store.RuleManager;
													disabledByRuleManager.DisableRule(typeof(RoleDeletingRuleClass));
													impliedFactType.Delete();
												}
												break;
											}
										}
										if (!sawProxy)
										{
											haveImplicitForUnary = true;
										}
									}
								}
								finally
								{
									if (disabledByRuleManager != null)
									{
										disabledByRuleManager.EnableRule(typeof(RoleDeletingRuleClass));
									}
								}
								if (!haveImplicitForUnary)
								{
									if (roles.Count == 2)
									{
										foreach (RoleBase role in roles)
										{
											if (role != playedRole)
											{
												CreateImpliedFactTypeForRole(playedFact.Model, objectification.NestingType, role.Role, objectification, true);
												break;
											}
										}
									}
								}
							}
						}
						else if (null != (objectification = rolePlayer.Objectification) &&
							objectification.IsImplied)
						{
							// The newly-played role is in a non-implied fact, so the objectification is no longer implied
							objectification.IsImplied = false;
						}
					}

				}
			}
		}
		private static void ProcessUniquenessConstraintForImpliedObjectification(UniquenessConstraint uniquenessConstraint, bool changingIsInternal, bool verifyPreferredIdentifier)
		{
			if (uniquenessConstraint == null || (!uniquenessConstraint.IsInternal && !changingIsInternal))
			{
				return;
			}
			LinkedElementCollection<FactType> facts = uniquenessConstraint.FactTypeCollection;
			int factsCount = facts.Count;
			for (int i = 0; i < factsCount; ++i)
			{
				FactType factType = facts[i];
				FrameworkDomainModel.DelayValidateElement(factType, DelayProcessFactTypeForImpliedObjectification);
				if (verifyPreferredIdentifier)
				{
					ObjectType nestingType = factType.NestingType;
					if (nestingType != null)
					{
						FrameworkDomainModel.DelayValidateElement(nestingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
					}
				}
			}
		}
		/// <summary>
		/// Delay validation callback for implied objectification
		/// </summary>
		private static void DelayProcessFactTypeForImpliedObjectification(ModelElement element)
		{
			ProcessFactTypeForImpliedObjectification(element as FactType, false);
		}
		/// <summary>
		/// Check for the implied objectification pattern, and add or remove it as appropriate.
		/// </summary>
		private static void ProcessFactTypeForImpliedObjectification(FactType factType, bool throwOnFailure)
		{
			// We don't need to process implied FactTypes, since they can never be objectified
			if (factType == null || factType.IsDeleted || factType.ImpliedByObjectification != null)
			{
				return;
			}

			Objectification objectification = factType.Objectification;
			if (objectification != null)
			{
				if (objectification.IsImplied && !IsImpliedObjectificationRequired(factType))
				{
					// The objectification is no longer implied, so get rid of it.
					if (throwOnFailure)
					{
						throw InvalidImpliedObjectificationException();
					}
					else
					{
						objectification.NestingType.Delete();
					}
				}
			}
			else
			{
				if (IsImpliedObjectificationRequired(factType))
				{
					// An objectification is now implied, so create it.
					CreateObjectificationForFactTypeInternal(factType, true, null);
				}
			}
		}
		/// <summary>
		/// Creates an <see cref="Objectification"/> for the specified <see cref="FactType"/>.
		/// </summary>
		public static void CreateObjectificationForFactType(FactType factType, bool isImplied, INotifyElementAdded notifyAdded)
		{
			if (factType == null)
			{
				throw new ArgumentNullException("factType");
			}
			if (isImplied && !IsImpliedObjectificationRequired(factType))
			{
				throw InvalidImpliedObjectificationException();
			}
			CreateObjectificationForFactTypeInternal(factType, isImplied, notifyAdded);
		}
		/// <summary>
		/// Creates an <see cref="Objectification"/> for the specified <see cref="FactType"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="isImplied"/> is <see langword="true"/>, it is the caller's responsibility to ensure that the
		/// specified <see cref="FactType"/> matches the implied <see cref="Objectification"/> pattern.
		/// </remarks>
		private static void CreateObjectificationForFactTypeInternal(FactType factType, bool isImplied, INotifyElementAdded notifyAdded)
		{
			// If the implied fact has a single internal uniqueness constraint
			// then automatically use it as the preferred identifier.
			// We need the preferred identifier to determine whether or not
			// the IsIndependent property should be set for implied objectifications.
			UniquenessConstraint preferredConstraint = null;
			bool isIndependent;
			if (notifyAdded == null)
			{
				Role unaryRole = factType.UnaryRole;
				if (unaryRole == null)
				{
					foreach (UniquenessConstraint candidateConstraint in factType.GetInternalConstraints<UniquenessConstraint>())
					{
						if (candidateConstraint.Modality == ConstraintModality.Alethic)
						{
							if (preferredConstraint != null)
							{
								// We found more than one, don't use it
								preferredConstraint = null;
								break;
							}
							else if (candidateConstraint.PreferredIdentifierFor != null)
							{
								break;
							}
							preferredConstraint = candidateConstraint;
						}
					}
				}
				isIndependent = isImplied && preferredConstraint != null && preferredConstraint.RoleCollection.Count == factType.RoleCollection.Count;
			}
			else
			{
				// This is called during fixup potentially before the fact/constraint relationships
				// are established, so we need to use the primary relationship (the role) to
				// find what we're after.
				LinkedElementCollection<RoleBase> factRoles = factType.RoleCollection;
				int factRoleCount = factRoles.Count;
				bool breakOut = false;
				for (int i = 0; i < factRoleCount && !breakOut; ++i)
				{
					Role role = (Role)factRoles[i]; // This must be a role, not a proxy, use exception cast
					LinkedElementCollection<ConstraintRoleSequence> sequences = role.ConstraintRoleSequenceCollection;
					int sequenceCount = sequences.Count;
					for (int j = 0; j < sequenceCount; ++j)
					{
						UniquenessConstraint candidateConstraint;
						if (null != (candidateConstraint = sequences[j] as UniquenessConstraint) &&
							candidateConstraint.IsInternal &&
							candidateConstraint.Modality == ConstraintModality.Alethic)
						{
							if (preferredConstraint != null)
							{
								if (candidateConstraint == preferredConstraint)
								{
									continue;
								}
								// We found more than one, don't use it
								preferredConstraint = null;
								breakOut = true;
								break;
							}
							else if (candidateConstraint.PreferredIdentifierFor != null)
							{
								breakOut = true;
								break;
							}
							preferredConstraint = candidateConstraint;
						}
					}
				}
				isIndependent = isImplied && preferredConstraint != null && preferredConstraint.RoleCollection.Count == factRoleCount;
			}
			Store store = factType.Store;
			ObjectType objectifyingType = new ObjectType(store,
				new PropertyAssignment(ObjectType.NameDomainPropertyId, factType.Name),
				new PropertyAssignment(ObjectType.IsIndependentDomainPropertyId, isIndependent));
			new Objectification(store,
				new RoleAssignment[]
				{
					new RoleAssignment(Objectification.NestedFactTypeDomainRoleId, factType),
					new RoleAssignment(Objectification.NestingTypeDomainRoleId, objectifyingType)
				},
				new PropertyAssignment[]
				{
					new PropertyAssignment(Objectification.IsImpliedDomainPropertyId, isImplied)
				});

			if (notifyAdded == null)
			{
				Dictionary<object, object> contextInfo = store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo;
				try
				{
					contextInfo[ORMModel.AllowDuplicateNamesKey] = null;
					objectifyingType.Model = factType.Model;
				}
				finally
				{
					contextInfo.Remove(ORMModel.AllowDuplicateNamesKey);
				}
				if (preferredConstraint != null)
				{
					objectifyingType.PreferredIdentifier = preferredConstraint;
				}
			}
			else
			{
				objectifyingType.Model = factType.Model;
				// The true addLinks parameter here will pick up both the Objectification and
				// the ModelHasObjectType links, so is sufficient to get all of the elements we
				// created here.
				notifyAdded.ElementAdded(objectifyingType, true);
				if (preferredConstraint != null)
				{
					notifyAdded.ElementAdded(new EntityTypeHasPreferredIdentifier(objectifyingType, preferredConstraint), false);
				}
			}
		}
		/// <summary>
		/// Create an implied fact type, set its far constraints, and add
		/// it to the model. Associating the implied fact with the objectifying
		/// relationship is delayed and left to the caller to avoid triggering
		/// rules prematurely.
		/// </summary>
		/// <param name="model">The model to include the fact in</param>
		/// <param name="nestingType">The objectifying object type</param>
		/// <param name="nestedRole">The role associated with this element</param>
		/// <param name="objectification">The Objectification that implies the FactType</param>
		/// <param name="roleIsUnary">The <paramref name="nestedRole"/> parameter is a unary role, create a pattern with no proxy and an equality constraint</param>
		/// <returns>The created fact type</returns>
		private static FactType CreateImpliedFactTypeForRole(ORMModel model, ObjectType nestingType, Role nestedRole, Objectification objectification, bool roleIsUnary)
		{
			// Create the implied fact and attach roles to it
			Store store = model.Store;
			FactType impliedFact = new FactType(store);
			RoleBase nearRole;
			if (roleIsUnary)
			{
				ObjectifiedUnaryRole objectifiedNearRole = new ObjectifiedUnaryRole(store);
				objectifiedNearRole.TargetRole = nestedRole;
				nearRole = objectifiedNearRole;
			}
			else
			{
				RoleProxy nearRoleProxy = new RoleProxy(store);
				nearRoleProxy.TargetRole = nestedRole;
				nearRole = nearRoleProxy;
			}
			Role farRole = new Role(store);
			LinkedElementCollection<RoleBase> impliedRoles = impliedFact.RoleCollection;
			impliedRoles.Add(nearRole);
			impliedRoles.Add(farRole);

			// Add standard constraints and role players
			MandatoryConstraint.CreateSimpleMandatoryConstraint(farRole);
			UniquenessConstraint.CreateInternalUniquenessConstraint(store).RoleCollection.Add(farRole);
			farRole.RolePlayer = nestingType;

			if (roleIsUnary)
			{
				Role nearRoleRole = nearRole.Role;
				UniquenessConstraint unaryUniqueness = UniquenessConstraint.CreateInternalUniquenessConstraint(store);
				unaryUniqueness.RoleCollection.Add(nearRoleRole);
				if (nestingType.PreferredIdentifier == null)
				{
					nestingType.PreferredIdentifier = unaryUniqueness;
				}
				nearRoleRole.RolePlayer = nestedRole.RolePlayer;
			}

			// UNDONE: Each of the readings need to be modified if we're in
			// a ring situation. The replacement values needs to be the (1-based)
			// number of the occurrence of that role player type in the collection.
			// Alternately, the readings we'll be able to generate are so ugly anyway
			// that the validation error with a direct jump to improve them will actually
			// be beneficial, not harmful, so we should not try to automatically repair
			// the readings at this point.

			// Add forward reading
			LinkedElementCollection<ReadingOrder> readingOrders = impliedFact.ReadingOrderCollection;
			ReadingOrder order = new ReadingOrder(store);
			LinkedElementCollection<RoleBase> orderRoles;
			readingOrders.Add(order);
			orderRoles = order.RoleCollection;
			orderRoles.Add(nearRole);
			orderRoles.Add(farRole);
			Reading reading = new Reading(store);
			reading.ReadingOrder = order;
			reading.Text = ResourceStrings.ImpliedFactTypePredicateReading;

			// Add inverse reading
			order = new ReadingOrder(store);
			readingOrders.Add(order);
			orderRoles = order.RoleCollection;
			orderRoles.Add(farRole);
			orderRoles.Add(nearRole);
			reading = new Reading(store);
			reading.ReadingOrder = order;
			reading.Text = ResourceStrings.ImpliedFactTypePredicateInverseReading;

			// Attach the objectification to the fact
			impliedFact.ImpliedByObjectification = objectification;

			// Attach the fact to the model
			impliedFact.Model = model;

			return impliedFact;
		}
		/// <summary>
		/// Creates an <see cref="Objectification"/> between the specified <see cref="FactType"/> and <see cref="ObjectType"/>. If the
		/// <see cref="FactType"/> already has an implied <see cref="Objectification"/>, it will be merged with the new objectifying
		/// <see cref="ObjectType"/>.
		/// </summary>
		public static void CreateExplicitObjectification(FactType nestedFactType, ObjectType nestingType)
		{
			if (nestedFactType == null)
			{
				throw new ArgumentNullException("nestedFactType");
			}
			Objectification objectification = nestedFactType.Objectification;
			if (objectification != null && objectification.IsImplied)
			{
				// Ignore the nestingType being set to null if the current Objectification is implied
				if (nestingType != null)
				{
					// We already have an implied Objectification, so modify it for the new ObjectType
					RuleManager ruleManager = objectification.Store.RuleManager;
					ObjectType impliedObjectifyingType = objectification.NestingType;
					Objectification oldObjectification = nestingType.Objectification;
					if (oldObjectification != null)
					{
						oldObjectification.Delete();
					}
					objectification.NestingType = nestingType;
					bool addRuleDisabled = false;
					bool removingRuleDisabled = false;
					try
					{
						ruleManager.DisableRule(typeof(RolePlayerAddRuleClass));
						addRuleDisabled = true;
						ruleManager.DisableRule(typeof(RolePlayerDeletingRuleClass));
						removingRuleDisabled = true;
						foreach (FactType impliedFactType in objectification.ImpliedFactTypeCollection)
						{
							LinkedElementCollection<RoleBase> roles = impliedFactType.RoleCollection;
							Debug.Assert(roles.Count == 2,
								"When this method is called, we should be at a stable point, so implied Fact Types should always have exactly two Roles.");
							Role role = roles[1] as Role;
							if (role == null)
							{
								role = roles[0] as Role;
								Debug.Assert(role != null);
							}
							role.RolePlayer = nestingType;
						}
					}
					finally
					{
						if (addRuleDisabled)
						{
							ruleManager.EnableRule(typeof(RolePlayerAddRuleClass));
						}
						if (removingRuleDisabled)
						{
							ruleManager.EnableRule(typeof(RolePlayerDeletingRuleClass));
						}
					}
					objectification.IsImplied = false;
					impliedObjectifyingType.Delete();
				}
			}
			else
			{
				// We don't have an implied Objectification, so we don't need to do anything special.
				nestedFactType.NestingType = nestingType;
			}
		}
		/// <summary>
		/// Removes the explicit <see cref="Objectification"/> specified by <paramref name="explicitObjectification"/>.
		/// </summary>
		/// <remarks>
		/// If it is determined that the <see cref="ObjectType"/> that results from the <see cref="Objectification"/> is
		/// used elsewhere in the model, it will be separated from the <see cref="Objectification"/>. Otherwise, the
		/// <see cref="ObjectType"/> will be used as part of the implied <see cref="Objectification"/> (if one is needed),
		/// or it will be deleted.
		/// </remarks>
		/// <param name="explicitObjectification">The <see cref="Objectification"/> to be removed.</param>
		public static void RemoveExplicitObjectification(Objectification explicitObjectification)
		{
			if (explicitObjectification == null)
			{
				throw new ArgumentNullException("explicitObjectification");
			}
			if (explicitObjectification.IsImplied)
			{
				throw new InvalidOperationException(ResourceStrings.ModelExceptionObjectificationImpliedObjectificationNotAllowed);
			}

			// There are two questions that we need to consider when unobjectifying, which lead to four possible scenarios:
			// 1) Will the fact type need an implied objectification once the explicit objectification has been removed?
			// 2) Does the objectifing object type still need to exist when the process is complete?
			// Currently, we determine the answer to the second question based on whether the object type plays roles in
			// any fact types other than those implied by the objectification. In the future, we may want to consider other
			// criteria as well (such as if the object type is specifically referred to by any constraints or rules, and,
			// depending on the answer to the first question, if it would make sense for these references to be to an
			// object type that results from an implied objectification).


			FactType factType = explicitObjectification.NestedFactType;
			ObjectType objectType = explicitObjectification.NestingType;
			Debug.Assert(factType != null && objectType != null);

			// Determine if the object type needs to survive this process (see comment above).
			bool objectTypeMustSurvive = false;
			foreach (Role playedRole in objectType.PlayedRoleCollection)
			{
				FactType playedFactType = playedRole.FactType;
				Debug.Assert(playedFactType != null);
				if (playedFactType.ImpliedByObjectification != explicitObjectification)
				{
					objectTypeMustSurvive = true;
					break;
				}
			}

			// Branch based on whether the fact type needs an implied objectification once the explicit objectification is removed.
			if (IsImpliedObjectificationRequired(factType))
			{
				LinkedElementCollection<RoleBase> factTypeRoles = factType.RoleCollection;
				int factTypeRolesCount = factTypeRoles.Count;

				// To determine if the implied objectification will result in an independent object type, we need check whether the
				// fact type has an alethic uniqueness constraint that spans all of its roles.
				bool isIndependent = false;
				foreach (UniquenessConstraint uniquenessConstraint in factType.GetInternalConstraints<UniquenessConstraint>())
				{
					if (uniquenessConstraint.Modality == ConstraintModality.Alethic &&
						uniquenessConstraint.RoleCollection.Count == factTypeRolesCount)
					{
						isIndependent = true;
						break;
					}
				}

				if (objectTypeMustSurvive)
				{
					// Since the existing object type needs to survive and the fact type needs an implied objectification,
					// we'll have to create a new object type to take over the objectification duties from the old one.
					ObjectType newObjectifyingType = new ObjectType(explicitObjectification.Partition,
						new PropertyAssignment(ObjectType.NameDomainPropertyId, factType.Name),
						new PropertyAssignment(ObjectType.IsIndependentDomainPropertyId, isIndependent));

					// For each fact type implied by the objectification, reassign the role played by the old object type
					// to the new object type.
					Type disabledRolePlayerChangeRuleType = null;
					try
					{
						foreach (FactType impliedFactType in explicitObjectification.ImpliedFactTypeCollection)
						{
							LinkedElementCollection<RoleBase> roles = impliedFactType.RoleCollection;
							Debug.Assert(roles.Count == 2);
							Role role = roles[1] as Role ?? roles[0] as Role;
							Debug.Assert(role != null && role.RolePlayer == objectType);

							if (disabledRolePlayerChangeRuleType == null)
							{
								disabledRolePlayerChangeRuleType = typeof(RolePlayerRolePlayerChangeRuleClass);
								explicitObjectification.Store.RuleManager.DisableRule(disabledRolePlayerChangeRuleType);
							}
							role.RolePlayer = newObjectifyingType;
						}
					}
					finally
					{
						if (disabledRolePlayerChangeRuleType != null)
						{
							explicitObjectification.Store.RuleManager.EnableRule(disabledRolePlayerChangeRuleType);
						}
					}

					// Reassign the objectification itself to the new object type.
					explicitObjectification.NestingType = newObjectifyingType;

					// Determine whether either of the object types can keep the preferred identifier, if we have one.
					EntityTypeHasPreferredIdentifier preferredIdentifierLink = EntityTypeHasPreferredIdentifier.GetLinkToPreferredIdentifier(objectType);
					if (preferredIdentifierLink != null)
					{
						LinkedElementCollection<Role> preferredIdentifierRoles = preferredIdentifierLink.PreferredIdentifier.RoleCollection;
						bool oldObjectTypeCanKeepPreferredIdentifier = true;
						bool newObjectTypeCanTakePreferredIdentifier = true;
						foreach (Role preferredIdentifierRole in preferredIdentifierRoles)
						{
							if (factTypeRoles.Contains(preferredIdentifierRole))
							{
								// If the preferred identifier role is in the fact type being objectified,
								// the old objectifying type cannot keep the identifier.
								oldObjectTypeCanKeepPreferredIdentifier = false;
							}
							else
							{
								// If the preferred identifier role is not in the fact type being objectified,
								// the new objectifying type cannot take the identifier.
								newObjectTypeCanTakePreferredIdentifier = false;
							}
							
							// If we've already determined that neither object type can have the preferred
							// identifier, we don't need to check the rest of the roles.
							if (!oldObjectTypeCanKeepPreferredIdentifier && !newObjectTypeCanTakePreferredIdentifier)
							{
								break;
							}
						}

						if (newObjectTypeCanTakePreferredIdentifier)
						{
							preferredIdentifierLink.PreferredIdentifierFor = newObjectifyingType;
							// If we previously determined that the new objectifying type should be independent,
							// but the new preferred identifier does not have the same number of roles as the
							// fact type being objectified, we need to set IsIndependent back to false.
							if (isIndependent && preferredIdentifierRoles.Count != factTypeRolesCount)
							{
								newObjectifyingType.IsIndependent = false;
							}
						}
						else
						{
							// In this case, we can just let our delay process method figure out which uniqueness constraint (if any)
							// should be used by the new objectifying type as its preferred identifier.
							FrameworkDomainModel.DelayValidateElement(newObjectifyingType, DelayProcessObjectifyingTypeForPreferredIdentifier);
							if (!oldObjectTypeCanKeepPreferredIdentifier)
							{
								preferredIdentifierLink.Delete();
							}
						}
					}

					explicitObjectification.IsImplied = true;

					Dictionary<object, object> contextInfo = explicitObjectification.Store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo;
					try
					{
						contextInfo[ORMModel.AllowDuplicateNamesKey] = null;
						newObjectifyingType.Model = factType.Model;
					}
					finally
					{
						contextInfo.Remove(ORMModel.AllowDuplicateNamesKey);
					}
				}
				else
				{
					if (isIndependent)
					{
						// If we previously determined that the implied objectifying type should be independent,
						// but the preferred identifier does not have the same number of roles as the
						// fact type being objectified, we should not set IsIndependent to true.
						UniquenessConstraint preferredIdentifier = objectType.PreferredIdentifier;
						if (preferredIdentifier != null && preferredIdentifier.RoleCollection.Count == factTypeRolesCount)
						{
							objectType.IsIndependent = true;
						}
					}
					// Since we determined the object type doesn't need to survive, we can just make the objectification implicit.
					explicitObjectification.IsImplied = true;
				}
			}
			else
			{
				// Since the fact type does not need an implied objectification, we can just delete the existing objectification.
				explicitObjectification.Delete();
				if (!objectTypeMustSurvive)
				{
					// Since we determined the object type doesn't need to survive, we can just delete it as well.
					objectType.Delete();
				}
			}
		}
		/// <summary>
		/// Make sure that an objectifying EntityType without a preferred identifier attached
		/// to a fact type with one internal uniqueness constraint is assigned that uniqueness
		/// constraint as its preferred identifier.
		/// </summary>
		/// <param name="element">ObjectType to process</param>
		private static void DelayProcessObjectifyingTypeForPreferredIdentifier(ModelElement element)
		{
			ObjectType objectType;
			Objectification objectification;
			if (null != (objectType = element as ObjectType) &&
				!objectType.IsDeleted &&
				null != (objectification = objectType.Objectification))
			{
				UniquenessConstraint preferredIdentifier = objectType.PreferredIdentifier;
				FactType nestedFactType = objectification.NestedFactType;
				if (preferredIdentifier == null)
				{
					UniquenessConstraint useConstraint = null;
					Role unaryRole;
					if (null != (unaryRole = nestedFactType.UnaryRole))
					{
						if (null != (unaryRole = unaryRole.ObjectifiedUnaryRole))
						{
							useConstraint = unaryRole.SingleRoleAlethicUniquenessConstraint;
						}
					}
					else
					{
						foreach (UniquenessConstraint testConstraint in nestedFactType.GetInternalConstraints<UniquenessConstraint>())
						{
							if (testConstraint.RoleCollection.Count != 0 && testConstraint.Modality == ConstraintModality.Alethic)
							{
								if (useConstraint == null)
								{
									useConstraint = testConstraint;
								}
								else
								{
									useConstraint = null;
									break;
								}
							}
						}
					}
					if (useConstraint != null)
					{
						bool haveSupertype = false;
						ObjectType.WalkSupertypeRelationships(
							objectType,
							delegate(SubtypeFact subtypeFact, ObjectType type, int depth)
							{
								// Note that we do not check if the supertype is a preferred
								// path here, only that supertypes are available.
								haveSupertype = true;
								return ObjectTypeVisitorResult.Stop;
							});
						if (!haveSupertype)
						{
							objectType.PreferredIdentifier = useConstraint;
							preferredIdentifier = useConstraint;
						}
					}
				}
				if (preferredIdentifier != null &&
					objectification.IsImplied &&
					preferredIdentifier.RoleCollection.Count == nestedFactType.RoleCollection.Count)
				{
					objectType.IsIndependent = true;
				}
			}
		}
		#region Exception Helpers
		/// <summary>
		/// Returns an exception indicating that the current modification is
		/// not allowed by the objectification pattern.
		/// </summary>
		private static InvalidOperationException BlockedByObjectificationPatternException()
		{
			return new InvalidOperationException(ResourceStrings.ModelExceptionObjectificationImpliedElementModified);
		}
		/// <summary>
		/// Returns an exception indicating that the implied objectification is not actually
		/// implied or the nesting type is doing things that are not allowed.
		/// </summary>
		private static InvalidOperationException InvalidImpliedObjectificationException()
		{
			return new InvalidOperationException(ResourceStrings.ModelExceptionObjectificationImpliedMustBeImpliedAndIndependentAndCannotPlayRoleInNonImpliedFact);
		}
		#endregion // Exception Helpers
		#endregion // Helper functions
		#region Deserialization Fixup
		/// <summary>
		/// Return a deserialization fixup listener. The listener
		/// validates all model errors and adds errors to the task provider.
		/// </summary>
		public static IDeserializationFixupListener FixupListener
		{
			get
			{
				return new ObjectificationFixupListener();
			}
		}
		/// <summary>
		/// A listener class to enforce valid subtype facts on load.
		/// Invalid subtype patterns will either be fixed up or completely
		/// removed.
		/// </summary>
		private sealed class ObjectificationFixupListener : DeserializationFixupListener<Objectification>
		{
			/// <summary>
			/// Create a new ObjectificationFixupListener
			/// </summary>
			public ObjectificationFixupListener()
				: base((int)ORMDeserializationFixupPhase.ValidateImplicitStoredElements)
			{
			}
			/// <summary>
			/// Make sure the objectification pattern is present and correct.
			/// </summary>
			/// <param name="element">An Objectification instance</param>
			/// <param name="store">The context store</param>
			/// <param name="notifyAdded">The listener to notify if elements are added during fixup</param>
			protected sealed override void ProcessElement(Objectification element, Store store, INotifyElementAdded notifyAdded)
			{
				// Note that this assumes xsd validation has occurred (RoleProxy is only on an ImpliedFact, there
				// is 1 Role and 1 RoleProxy or two Roles for a unary objectification, and implied facts must be
				// attached to an Objectification relationship).
				FactType nestedFact = element.NestedFactType;
				ORMModel model = nestedFact.Model;
				ObjectType nestingType = element.NestingType;
				LinkedElementCollection<RoleBase> factRoles = nestedFact.RoleCollection;
				int factRolesCount = factRoles.Count;
				int? unaryRoleIndex = FactType.GetUnaryRoleIndex(factRoles);
				Role unaryRole = unaryRoleIndex.HasValue ? factRoles[unaryRoleIndex.Value].Role : null;

				// Make sure each of the facts has a properly constructed role proxy or objectified unary role
				for (int i = 0; i < factRolesCount; ++i)
				{
					Role factRole = (Role)factRoles[i];
					Role farRole = null; // The role on the implied fact with the nesting type as a role player
					FactType impliedFact = null;
					RoleBase nearRole = null;
					RoleProxy proxy = factRole.Proxy;
					ObjectifiedUnaryRole objectifiedUnaryRole = factRole.ObjectifiedUnaryRole;
					FactType mismatchedFactType;
					if (unaryRole != null)
					{
						if (proxy != null &&
							null != (mismatchedFactType = proxy.FactType))
						{
							UniquenessConstraint pid;
							LinkedElementCollection<Role> pidRoles;
							if (null != (pid = nestingType.PreferredIdentifier) &&
								1 == (pidRoles = pid.RoleCollection).Count &&
								pidRoles[0] == factRole)
							{
								// The preferred identifier points straight to
								// the unary role via the proxy. We'll hook it
								// back up to the uniqueness constraint on the
								// objectified unary role below.
								nestingType.PreferredIdentifier = null;
							}
							RemoveFactType(mismatchedFactType);
						}
						nearRole = objectifiedUnaryRole;
					}
					else
					{
						if (objectifiedUnaryRole != null &&
							null != (mismatchedFactType = objectifiedUnaryRole.FactType))
						{
							RemoveFactType(mismatchedFactType);
						}
						nearRole = proxy;
					}
					if (nearRole != null)
					{
						// Make sure the element is appropriate
						impliedFact = nearRole.FactType;
						if (impliedFact == null)
						{
							nearRole.Delete();
							nearRole = null;
						}
						else if ((unaryRole != null && unaryRole != factRole) || impliedFact.ImpliedByObjectification != element)
						{
							RemoveFactType(impliedFact);
							impliedFact = null;
							Debug.Assert(nearRole.IsDeleted); // Goes away with delete propagation on the fact
							nearRole = null;
						}
						else
						{
							LinkedElementCollection<RoleBase> impliedRoles = impliedFact.RoleCollection;
							if (impliedRoles.Count != 2)
							{
								RemoveFactType(impliedFact);
								impliedFact = null;
								nearRole = null;
							}
							else
							{
								farRole = impliedRoles[0] as Role;
								if (farRole == null || farRole is ObjectifiedUnaryRole)
								{
									farRole = impliedRoles[1] as Role;
									Debug.Assert(farRole != null);
									if (farRole == null)
									{
										RemoveFactType(impliedFact);
										impliedFact = null;
										nearRole = null;
									}
								}
								if (farRole != null)
								{
									ObjectType testRolePlayer = farRole.RolePlayer;
									if (testRolePlayer != nestingType)
									{
										farRole.RolePlayer = nestingType;
										notifyAdded.ElementAdded(ObjectTypePlaysRole.GetRolePlayer(farRole));
									}
								}
							}
						}
					}
					if (nearRole == null)
					{
						if (unaryRole != null)
						{
							if (factRole != unaryRole)
							{
								continue;
							}

							// Create the objectified unary role
							objectifiedUnaryRole = new ObjectifiedUnaryRole(store);
							objectifiedUnaryRole.TargetRole = factRole;
							objectifiedUnaryRole.RolePlayer = factRole.RolePlayer;
							notifyAdded.ElementAdded(objectifiedUnaryRole, true);
							nearRole = objectifiedUnaryRole;
						}
						else
						{
							// Create the proxy role
							proxy = new RoleProxy(store);
							proxy.TargetRole = factRole;
							notifyAdded.ElementAdded(proxy, true);
							nearRole = proxy;
						}

						// Create the non-proxy role
						farRole = new Role(store);
						farRole.RolePlayer = nestingType;
						notifyAdded.ElementAdded(nearRole, true);

						// Create the implied fact and set relationships to existing objects
						impliedFact = new FactType(store);
						nearRole.FactType = impliedFact;
						farRole.FactType = impliedFact;
						impliedFact.ImpliedByObjectification = element;
						impliedFact.Model = model;
						notifyAdded.ElementAdded(impliedFact, true);

						if (objectifiedUnaryRole != null)
						{
							UniquenessConstraint objectifiedUnaryUniqueness = UniquenessConstraint.CreateInternalUniquenessConstraint(impliedFact);
							objectifiedUnaryUniqueness.RoleCollection.Add(objectifiedUnaryRole);
							if (nestingType.PreferredIdentifier == null)
							{
								nestingType.PreferredIdentifier = objectifiedUnaryUniqueness;
							}
							objectifiedUnaryUniqueness.Model = model;
							notifyAdded.ElementAdded(objectifiedUnaryUniqueness, true);
						}

						// Add forward reading
						LinkedElementCollection<ReadingOrder> readingOrders = impliedFact.ReadingOrderCollection;
						ReadingOrder order = new ReadingOrder(store);
						LinkedElementCollection<RoleBase> orderRoles;
						readingOrders.Add(order);
						orderRoles = order.RoleCollection;
						orderRoles.Add(nearRole);
						orderRoles.Add(farRole);
						Reading reading = new Reading(store);
						reading.ReadingOrder = order;
						reading.Text = ResourceStrings.ImpliedFactTypePredicateReading;
						notifyAdded.ElementAdded(order, true);
						notifyAdded.ElementAdded(reading, false);

						// Add inverse reading
						order = new ReadingOrder(store);
						readingOrders.Add(order);
						orderRoles = order.RoleCollection;
						orderRoles.Add(farRole);
						orderRoles.Add(nearRole);
						reading = new Reading(store);
						reading.ReadingOrder = order;
						reading.Text = ResourceStrings.ImpliedFactTypePredicateInverseReading;
						notifyAdded.ElementAdded(order, true);
						notifyAdded.ElementAdded(reading, false);
					}
					else if (unaryRole != null && unaryRole != factRole)
					{
						continue;
					}

					// Make sure the internal constraint pattern is correct on the far role
					EnsureSingleColumnUniqueAndMandatory(store, model, farRole, notifyAdded);
				}

				// Verify that that are no innapropriate implied facts are attached to the objectification
				LinkedElementCollection<FactType> impliedFacts = element.ImpliedFactTypeCollection;
				int impliedFactCount = impliedFacts.Count;
				int expectedImpliedCount = (unaryRole == null) ? factRolesCount : 1;
				if (impliedFactCount != expectedImpliedCount)
				{
					int leftToRemove = impliedFactCount - expectedImpliedCount;
					Debug.Assert(impliedFactCount > expectedImpliedCount); // We verified and/or added at least this many earlier
					for (int i = impliedFactCount - 1; i >= 0 && leftToRemove != 0; --i)
					{
						FactType impliedFact = impliedFacts[i];
						LinkedElementCollection<RoleBase> impliedRoles = impliedFact.RoleCollection;
						if (impliedRoles.Count != 2)
						{
							RemoveFactType(impliedFact);
							--leftToRemove;
						}
						else
						{
							RoleBase testRole = impliedRoles[0];
							RoleProxy proxy;
							ObjectifiedUnaryRole objectifiedUnaryRole = null;
							if (null == (proxy = testRole as RoleProxy) &&
								null == (objectifiedUnaryRole = testRole as ObjectifiedUnaryRole) &&
								null == (proxy = (testRole = impliedRoles[1]) as RoleProxy))
							{
								objectifiedUnaryRole = testRole as ObjectifiedUnaryRole;
								Debug.Assert(objectifiedUnaryRole != null, "At least one role in an implied fact should be a proxy or objectified unary role.");
							}

							Role targetRole;
							if ((proxy == null && objectifiedUnaryRole == null) ||
								null == (targetRole = (proxy != null ? proxy.Role : objectifiedUnaryRole.TargetRole)) ||
								nestedFact != targetRole.FactType)
							{
								RemoveFactType(impliedFact);
								--leftToRemove;
							}
						}
					}
				}

				// Verify the implication pattern
				if (element.IsImplied)
				{
					bool canBeImplied = true;
					UniquenessConstraint preferredIdentifier;
					if (factRolesCount < 2 ||
						unaryRole != null ||
						nestingType.PlayedRoleCollection.Count > factRolesCount ||
						(!nestingType.IsIndependent &&
						(null != (preferredIdentifier = nestingType.PreferredIdentifier) &&
						preferredIdentifier.RoleCollection.Count == factRolesCount)))
					{
						canBeImplied = false;
					}
					else if (factRolesCount == 2)
					{
						// We require a multi-role internal uniqueness constraint to be implied
						// The internal constraints may not be attached to the fact yet
						// as this also happens during deserialization fixup, so get the
						// constraints from the non-derivable role connections
						canBeImplied = false;
						for (int i = 0; i < 2 && !canBeImplied; ++i)
						{
							Role role = (Role)factRoles[i].Role;
							LinkedElementCollection<ConstraintRoleSequence> constraints = role.ConstraintRoleSequenceCollection;
							int constraintCount = constraints.Count;
							for (int j = 0; j < constraintCount; ++j)
							{
								UniquenessConstraint uc = constraints[j] as UniquenessConstraint;
								if (uc != null && uc.IsInternal && uc.RoleCollection.Count > 1)
								{
									canBeImplied = true;
									break;
								}
							}
						}
					}
					if (!canBeImplied)
					{
						element.IsImplied = false;
					}
				}
			}
			/// <summary>
			/// Internal constraints are not fully connected at this point (FactSetConstraint instances
			/// are not implicitly constructed until a later phase), so we need to work a little harder
			/// to remove them.
			/// </summary>
			/// <param name="factType">The fact to clear of external constraints</param>
			private static void RemoveFactType(FactType factType)
			{
				LinkedElementCollection<RoleBase> factRoles = factType.RoleCollection;
				int roleCount = factRoles.Count;
				for (int i = 0; i < roleCount; ++i)
				{
					Role role = factRoles[i] as Role;
					if (role != null)
					{
						LinkedElementCollection<ConstraintRoleSequence> sequences = role.ConstraintRoleSequenceCollection;
						int sequenceCount = sequences.Count;
						for (int j = sequenceCount - 1; j >= 0; --j)
						{
							SetConstraint ic = sequences[j] as SetConstraint;
							if (ic != null && ic.Constraint.ConstraintIsInternal)
							{
								ic.Delete();
							}
						}
					}
				}
				factType.Delete();
			}
			private static void EnsureSingleColumnUniqueAndMandatory(Store store, ORMModel model, Role role, INotifyElementAdded notifyAdded)
			{
				LinkedElementCollection<ConstraintRoleSequence> sequences = role.ConstraintRoleSequenceCollection;
				int sequenceCount = sequences.Count;
				bool haveUniqueness = false;
				bool haveMandatory = false;
				SetConstraint ic;
				for (int i = sequenceCount - 1; i >= 0; --i)
				{
					ic = sequences[i] as SetConstraint;
					if (ic != null && ic.Constraint.ConstraintIsInternal)
					{
						if (ic.RoleCollection.Count == 1 && ic.Modality == ConstraintModality.Alethic)
						{
							switch (ic.Constraint.ConstraintType)
							{
								case ConstraintType.InternalUniqueness:
									if (haveUniqueness)
									{
										ic.Delete();
									}
									else
									{
										haveUniqueness = true;
									}
									break;
								case ConstraintType.SimpleMandatory:
									if (haveMandatory)
									{
										ic.Delete();
									}
									else
									{
										haveMandatory = true;
									}
									break;
							}
						}
						else
						{
							ic.Delete();
						}
					}
				}
				if (!haveUniqueness)
				{
					ic = UniquenessConstraint.CreateInternalUniquenessConstraint(store);
					ic.RoleCollection.Add(role);
					ic.Model = model;
					notifyAdded.ElementAdded(ic, true);
				}
				if (!haveMandatory)
				{
					ic = MandatoryConstraint.CreateSimpleMandatoryConstraint(store);
					ic.RoleCollection.Add(role);
					ic.Model = model;
					notifyAdded.ElementAdded(ic, true);
				}
			}
		}
		/// <summary>
		/// Return a deserialization fixup listener. The listener
		/// verifies that all facts that should have implied objectifications
		/// have implied objectifications
		/// </summary>
		public static IDeserializationFixupListener ImpliedFixupListener
		{
			get
			{
				return new ImpliedObjectificationFixupListener();
			}
		}
		/// <summary>
		/// A listener class to enforce valid implied objectification patterns on load.
		/// Any fact that does not have a required implied objectification but should have
		/// one will be fixed. Unnecessary implied objectification elements are removed
		/// during Objectification validation.
		/// </summary>
		private sealed class ImpliedObjectificationFixupListener : DeserializationFixupListener<FactType>
		{
			/// <summary>
			/// Create a new SubtypeFactFixupListener
			/// </summary>
			public ImpliedObjectificationFixupListener()
				: base((int)ORMDeserializationFixupPhase.ValidateImplicitStoredElements)
			{
			}
			/// <summary>
			/// Make sure that any fact type that does not have an implied objectification
			/// but needs one gets an implied objectification. Note that populating the implied
			/// objectification is left up to the other fixup listener.
			/// </summary>
			/// <param name="element">An FactType instance</param>
			/// <param name="store">The context store</param>
			/// <param name="notifyAdded">The listener to notify if elements are added during fixup</param>
			protected sealed override void ProcessElement(FactType element, Store store, INotifyElementAdded notifyAdded)
			{
				if (!element.IsDeleted &&
					null == element.Objectification &&
					null == element.ImpliedByObjectification)
				{
					bool impliedRequired = false;
					LinkedElementCollection<RoleBase> roles = element.RoleCollection;
					int roleCount = roles.Count;
					if (roleCount > 2)
					{
						impliedRequired = true;
					}
					else
					{
						// The internal constraints may not be attached to the fact yet
						// as this also happens during deserialization fixup, so get the
						// constraints from the non-derived role connections
						for (int i = 0; i < roleCount && !impliedRequired; ++i)
						{
							Role role = roles[i].Role;
							LinkedElementCollection<ConstraintRoleSequence> constraints = role.ConstraintRoleSequenceCollection;
							int constraintCount = constraints.Count;
							for (int j = 0; j < constraintCount; ++j)
							{
								UniquenessConstraint uc = constraints[j] as UniquenessConstraint;
								if (uc != null && uc.IsInternal && uc.RoleCollection.Count > 1)
								{
									impliedRequired = true;
									break;
								}
							}
						}
					}
					if (impliedRequired)
					{
						CreateObjectificationForFactTypeInternal(element, true, notifyAdded);
					}
				}
			}
		}
		#endregion Deserialization Fixup
	}
}
