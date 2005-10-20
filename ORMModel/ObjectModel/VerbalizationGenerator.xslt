<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:plx="http://schemas.neumont.edu/CodeGeneration/PLiX"
	xmlns:ve="http://schemas.neumont.edu/ORM/SDK/Verbalization"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
>
	<!-- Indenting is useful for debugging the transform, but a waste of memory at generation time -->
	<!--<xsl:output indent="yes"/>-->
	<xsl:preserve-space elements="ve:Snippet"/>
	<!-- Pick up param value supplied automatically by plix loader -->
	<xsl:param name="CustomToolNamespace" select="'TestNamespace'"/>

	<!-- Names of the different classes we generate -->
	<xsl:param name="VerbalizationTextSnippetType" select="'VerbalizationTextSnippetType'"/>
	<xsl:param name="VerbalizationSet" select="'VerbalizationSet'"/>
	<xsl:param name="VerbalizationSets" select="'VerbalizationSets'"/>

	<!-- Parts of variable names used in generated code. These
		 names are decorated with position numbers and allow multiple
		 spits of the same template in the same function without name
		 collision -->
	<xsl:param name="FormatVariablePart" select="'Format'"/>
	<xsl:param name="ReplaceVariablePart" select="'Replace'"/>
	<xsl:param name="RoleIterVariablePart" select="'RoleIter'"/>
	<xsl:param name="FactRoleIterVariablePart" select="'FactRoleIter'"/>

	<!-- Include templates to generate the shared verbalization classes -->
	<xsl:include href="VerbalizationGenerator.Sets.xslt"/>
	<xsl:template match="ve:VerbalizationRoot">
		<plx:root>
			<plx:namespaceImport name="System"/>
			<plx:namespaceImport name="System.IO"/>
			<plx:namespaceImport name="System.Text"/>
			<plx:namespace name="{$CustomToolNamespace}">
				<!-- Generate verbalization set classes and default populations -->
				<xsl:call-template name="GenerateVerbalizationSets"/>
				<!-- Generate verbalization implementations for all constructs -->
				<xsl:call-template name="GenerateVerbalizationClasses"/>
			</plx:namespace>
		</plx:root>
	</xsl:template>
	<xsl:template name="GenerateVerbalizationClasses">
		<xsl:apply-templates select="ve:Constructs/child::*" mode="GenerateClasses"/>
	</xsl:template>
	<xsl:template match="ve:Constraints" mode="GenerateClasses">
		<xsl:apply-templates select="ve:Constraint" mode="ConstraintVerbalization"/>
	</xsl:template>
	<xsl:template match="ve:FactType" mode="GenerateClasses">
		<plx:class name="FactType" visibility="public" partial="true">
			<plx:leadingInfo>
				<plx:pragma type="region" data="FactType verbalization"/>
			</plx:leadingInfo>
			<plx:trailingInfo>
				<plx:pragma type="closeRegion" data="FactType verbalization"/>
			</plx:trailingInfo>
			<plx:implementsInterface dataTypeName="IVerbalize"/>
			<plx:function name="GetVerbalization" visibility="protected">
				<plx:leadingInfo>
					<plx:docComment>
						<summary>IVerbalize.GetVerbalization implementation</summary>
					</plx:docComment>
				</plx:leadingInfo>
				<plx:interfaceMember memberName="GetVerbalization" dataTypeName="IVerbalize"/>
				<plx:param name="writer" dataTypeName="TextWriter"/>
				<plx:param name="beginVerbalization" dataTypeName="NotifyBeginVerbalization"/>
				<plx:param name="isNegative" dataTypeName=".boolean"/>
				<plx:returns dataTypeName=".boolean"/>

				<!-- Verbalizing a fact type is a simple case of verbalizing a constraint.
					 Leverage the code snippets we use for constraints by setting the right
					 variable names and calling the constraint verbalization templates -->
				<xsl:call-template name="CheckErrorConditions"/>
				<plx:local name="factRoles" dataTypeName="RoleMoveableCollection">
					<plx:initialize>
						<plx:callThis name="RoleCollection" type="property"/>
					</plx:initialize>
				</plx:local>
				<plx:local name="factArity" dataTypeName=".i4">
					<plx:initialize>
						<plx:callInstance name="Count" type="property">
							<plx:callObject>
								<plx:nameRef name="factRoles"/>
							</plx:callObject>
						</plx:callInstance>
					</plx:initialize>
				</plx:local>
				<plx:local name="allReadingOrders" dataTypeName="ReadingOrderMoveableCollection">
					<plx:initialize>
						<plx:callThis name="ReadingOrderCollection" type="property"/>
					</plx:initialize>
				</plx:local>
				<plx:local name="isDeontic" dataTypeName=".boolean" const="true">
					<plx:initialize>
						<plx:falseKeyword/>
					</plx:initialize>
				</plx:local>
				<plx:local name="readingOrder" dataTypeName="ReadingOrder"/>
				<plx:local name="snippets" dataTypeName="{$VerbalizationSets}">
					<plx:initialize>
						<plx:callStatic dataTypeName="{$VerbalizationSets}" name="Default" type="property"/>
					</plx:initialize>
				</plx:local>
				<xsl:call-template name="PopulateBasicRoleReplacements"/>
				<xsl:variable name="factMockup">
					<ve:Fact/>
				</xsl:variable>
				<xsl:apply-templates select="msxsl:node-set($factMockup)/child::*" mode="ConstraintVerbalization">
						<xsl:with-param name="TopLevel" select="true()"/>
				</xsl:apply-templates>
				<plx:return>
					<plx:trueKeyword/>
				</plx:return>
			</plx:function>
		</plx:class>	
	</xsl:template>
	<xsl:template match="ve:Constraint" mode="ConstraintVerbalization">
		<xsl:variable name="patternGroup" select="@patternGroup"/>
		<xsl:variable name="isInternal" select="$patternGroup='InternalConstraint'"/>
		<xsl:variable name="isSingleColumn" select="$patternGroup='SingleColumnExternalConstraint'"/>
		<plx:class name="{@type}" visibility="public" partial="true">
			<plx:leadingInfo>
				<plx:pragma type="region" data="{@type} verbalization"/>
			</plx:leadingInfo>
			<plx:trailingInfo>
				<plx:pragma type="closeRegion" data="{@type} verbalization"/>
			</plx:trailingInfo>
			<plx:implementsInterface dataTypeName="IVerbalize"/>
			<plx:function name="GetVerbalization" visibility="protected">
				<plx:leadingInfo>
					<plx:docComment>
						<summary>IVerbalize.GetVerbalization implementation</summary>
					</plx:docComment>
				</plx:leadingInfo>
				<plx:interfaceMember memberName="GetVerbalization" dataTypeName="IVerbalize"/>
				<plx:param name="writer" dataTypeName="TextWriter"/>
				<plx:param name="beginVerbalization" dataTypeName="NotifyBeginVerbalization"/>
				<plx:param name="isNegative" dataTypeName=".boolean"/>
				<plx:returns dataTypeName=".boolean"/>

				<!-- Don't proceed with verbalization if errors are present -->
				<xsl:call-template name="CheckErrorConditions"/>

				<xsl:variable name="subscriptConditionsFragment">
					<!-- UNDONE: Better subscript handling. The conditional processing needs
						 to be moved inside each pattern, but we need to prepare for the situation
						 up front. Consider getting the generator out of the subscripting business
						 altogether. We're basically just spitting an inline function. For now,
						 keep the conditional checks in place so we don't lose the work. The TrueKeyword
						 spit here will be compiled out and not appear in code. -->
					<!--<xsl:apply-templates select="ve:EnableSubscripts" mode="SubscriptConditions"/>-->
					<plx:trueKeyword/>
				</xsl:variable>
				<xsl:variable name="subscriptConditions" select="msxsl:node-set($subscriptConditionsFragment)/child::*"/>

				<!-- Pick up standard code we'll need for any constraint -->
				<plx:local name="snippets" dataTypeName="{$VerbalizationSets}">
					<plx:initialize>
						<plx:callStatic dataTypeName="{$VerbalizationSets}" name="Default" type="property"/>
					</plx:initialize>
				</plx:local>
				<plx:local name="isDeontic" dataTypeName=".boolean">
					<plx:initialize>
						<plx:binaryOperator type="equality">
							<plx:left>
								<plx:callInstance name="Modality" type="property">
									<plx:callObject>
										<plx:cast dataTypeName="IConstraint" type="testCast">
											<plx:thisKeyword/>
										</plx:cast>
									</plx:callObject>
								</plx:callInstance>
							</plx:left>
							<plx:right>
								<plx:callStatic dataTypeName="ConstraintModality" name="Deontic" type="field"/>
							</plx:right>
						</plx:binaryOperator>
						<!--<plx:falseKeyword/>-->
					</plx:initialize>
				</plx:local>
				<plx:local name="sbTemp" dataTypeName="StringBuilder">
					<plx:initialize>
						<plx:nullKeyword/>
					</plx:initialize>
				</plx:local>
				<plx:local name="parentFact" dataTypeName="FactType">
					<xsl:if test="$isInternal">
						<plx:initialize>
							<plx:callThis name="FactType" type="property"/>
						</plx:initialize>
					</xsl:if>
				</plx:local>
				<xsl:if test="$isInternal">
					<plx:local name="includedRoles" dataTypeName="RoleMoveableCollection">
						<plx:initialize>
							<plx:callThis name="RoleCollection" type="property"/>
						</plx:initialize>
					</plx:local>
				</xsl:if>
				<plx:local name="factRoles" dataTypeName="RoleMoveableCollection">
					<xsl:if test="$isInternal">
						<plx:initialize>
							<plx:callInstance name="RoleCollection" type="property">
								<plx:callObject>
									<plx:nameRef name="parentFact"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:initialize>
					</xsl:if>
				</plx:local>
				<plx:local name="factArity" dataTypeName=".i4">
					<xsl:if test="$isInternal">
						<plx:initialize>
							<plx:callInstance name="Count" type="property">
								<plx:callObject>
									<plx:nameRef name="factRoles"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:initialize>
					</xsl:if>
				</plx:local>
				<plx:local name="allReadingOrders" dataTypeName="ReadingOrderMoveableCollection">
					<xsl:if test="$isInternal">
						<plx:initialize>
							<plx:callInstance name="ReadingOrderCollection" type="property">
								<plx:callObject>
									<plx:nameRef name="parentFact"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:initialize>
					</xsl:if>
				</plx:local>
				<xsl:if test="not($isInternal)">
					<plx:local name="allConstraintRoles" dataTypeName="RoleMoveableCollection">
						<plx:initialize>
							<plx:callThis name="RoleCollection" type="property"/>
						</plx:initialize>
					</plx:local>
					<plx:local name="allFacts" dataTypeName="FactTypeMoveableCollection">
						<plx:initialize>
							<plx:callThis name="FactTypeCollection" type="property"/>
						</plx:initialize>
					</plx:local>
					<plx:local name="allFactsCount" dataTypeName=".i4">
						<plx:initialize>
							<plx:callInstance name="Count" type="property">
								<plx:callObject>
									<plx:nameRef name="allFacts"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:initialize>
					</plx:local>
					<plx:branch>
						<plx:condition>
							<plx:binaryOperator type="equality">
								<plx:left>
									<plx:nameRef name="allFactsCount"/>
								</plx:left>
								<plx:right>
									<plx:value type="i4" data="0"/>
								</plx:right>
							</plx:binaryOperator>
						</plx:condition>
						<plx:body>
							<plx:return>
								<!-- This should be an error on the constraint, but be defensive and bail
									if we have no facts -->
								<plx:falseKeyword/>
							</plx:return>
						</plx:body>
					</plx:branch>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="$isInternal">
						<plx:local name="includedArity" dataTypeName=".i4">
							<plx:initialize>
								<plx:callInstance name="Count" type="property">
									<plx:callObject>
										<plx:nameRef name="includedRoles"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>
						<!-- No included roles is not an error, but we can't verbalize it -->
						<plx:branch>
							<plx:condition>
								<plx:binaryOperator type="booleanOr">
									<plx:left>
										<!-- No readings is an error on the parent, so we can get past the error check without them -->
										<plx:binaryOperator type="equality">
											<plx:left>
												<plx:callInstance name="Count" type="property">
													<plx:callObject>
														<plx:nameRef name="allReadingOrders"/>
													</plx:callObject>
												</plx:callInstance>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="0"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:left>
									<plx:right>
										<plx:binaryOperator type="equality">
											<plx:left>
												<plx:nameRef name="includedArity"/>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="0"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:right>
								</plx:binaryOperator>
							</plx:condition>
							<plx:body>
								<plx:return>
									<plx:falseKeyword/>
								</plx:return>
							</plx:body>
						</plx:branch>
					</xsl:when>
					<xsl:when test="$isSingleColumn">
						<plx:local name="allBasicRoleReplacements" dataTypeName=".string">
							<plx:arrayDescriptor rank="1">
								<plx:arrayDescriptor rank="1"/>
							</plx:arrayDescriptor>
							<plx:initialize>
								<plx:callNew dataTypeName=".string">
									<plx:arrayDescriptor rank="1">
										<plx:arrayDescriptor rank="1"/>
									</plx:arrayDescriptor>
									<plx:passParam>
										<plx:nameRef name="allFactsCount"/>
									</plx:passParam>
								</plx:callNew>
							</plx:initialize>
						</plx:local>
						<plx:local name="minFactArity" dataTypeName=".i4">
							<plx:initialize>
								<plx:callStatic name="MaxValue" dataTypeName=".i4" type="field"/>
							</plx:initialize>
						</plx:local>
						<plx:local name="maxFactArity" dataTypeName=".i4">
							<plx:initialize>
								<plx:callStatic name="MinValue" dataTypeName=".i4" type="field"/>
							</plx:initialize>
						</plx:local>
						<plx:local name="iFact" dataTypeName=".i4">
							<plx:initialize>
								<plx:value type="i4" data="0"/>
							</plx:initialize>
						</plx:local>
						<plx:loop>
							<plx:condition>
								<plx:binaryOperator type="lessThan">
									<plx:left>
										<plx:nameRef name="iFact"/>
									</plx:left>
									<plx:right>
										<plx:nameRef name="allFactsCount"/>
									</plx:right>
								</plx:binaryOperator>
							</plx:condition>
							<plx:beforeLoop>
								<plx:increment>
									<plx:nameRef name="iFact"/>
								</plx:increment>
							</plx:beforeLoop>
							<plx:body>
								<!-- Return if there are no readings. We need readings for all facts
									 to verbalize the constraint -->
								<plx:local name="currentFact" dataTypeName="FactType">
									<plx:initialize>
										<plx:callInstance name=".implied" type="arrayIndexer">
											<plx:callObject>
												<plx:nameRef name="allFacts"/>
											</plx:callObject>
											<plx:passParam>
												<plx:nameRef name="iFact"/>
											</plx:passParam>
										</plx:callInstance>
									</plx:initialize>
								</plx:local>
								<plx:branch>
									<plx:condition>
										<plx:binaryOperator type="equality">
											<plx:left>
												<plx:callInstance name="Count" type="property">
													<plx:callObject>
														<plx:callInstance name="ReadingOrderCollection" type="property">
															<plx:callObject>
																<plx:nameRef name="currentFact"/>
															</plx:callObject>
														</plx:callInstance>
													</plx:callObject>
												</plx:callInstance>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="0"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:condition>
									<plx:body>
										<plx:return>
											<plx:falseKeyword/>
										</plx:return>
									</plx:body>
								</plx:branch>
								<!-- Get the roles and role count for the current fact -->
								<plx:assign>
										<plx:left>
										<plx:nameRef name="factRoles"/>
									</plx:left>
									<plx:right>
										<plx:callInstance name="RoleCollection" type="property">
											<plx:callObject>
												<plx:nameRef name="currentFact"/>
											</plx:callObject>
										</plx:callInstance>
									</plx:right>
								</plx:assign>
								<plx:assign>
										<plx:left>
										<plx:nameRef name="factArity"/>
									</plx:left>
									<plx:right>
										<plx:callInstance name="Count" type="property">
											<plx:callObject>
												<plx:nameRef name="factRoles"/>
											</plx:callObject>
										</plx:callInstance>
									</plx:right>
								</plx:assign>
								<!-- Track the min and max values for our current fact arity -->
								<plx:branch>
									<plx:condition>
										<plx:binaryOperator type="lessThan">
											<plx:left>
												<plx:nameRef name="factArity"/>
											</plx:left>
											<plx:right>
												<plx:nameRef name="minFactArity"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:condition>
									<plx:body>
										<plx:assign>
												<plx:left>
												<plx:nameRef name="minFactArity"/>
											</plx:left>
											<plx:right>
												<plx:nameRef name="factArity"/>
											</plx:right>
										</plx:assign>
									</plx:body>
								</plx:branch>
								<plx:branch>
									<plx:condition>
										<plx:binaryOperator type="greaterThan">
											<plx:left>
												<plx:nameRef name="factArity"/>
											</plx:left>
											<plx:right>
												<plx:nameRef name="maxFactArity"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:condition>
									<plx:body>
										<plx:assign>
											<plx:left>
												<plx:nameRef name="maxFactArity"/>
											</plx:left>
											<plx:right>
												<plx:nameRef name="factArity"/>
											</plx:right>
										</plx:assign>
									</plx:body>
								</plx:branch>
								<!-- Populate the basic replacements for this fact -->
								<xsl:call-template name="PopulateBasicRoleReplacements">
									<xsl:with-param name="SubscriptConditions" select="$subscriptConditions"/>
								</xsl:call-template>
								<plx:assign>
									<plx:left>
										<plx:callInstance name=".implied" type="arrayIndexer">
											<plx:callObject>
												<plx:nameRef name="allBasicRoleReplacements"/>
											</plx:callObject>
											<plx:passParam>
												<plx:nameRef name="iFact"/>
											</plx:passParam>
										</plx:callInstance>
									</plx:left>
									<plx:right>
										<plx:nameRef name="basicRoleReplacements"/>
									</plx:right>
								</plx:assign>
							</plx:body>
						</plx:loop>
						<plx:local name="constraintRoleArity" dataTypeName=".i4">
							<plx:initialize>
								<plx:callInstance name="Count" type="property">
									<plx:callObject>
										<plx:nameRef name="allConstraintRoles"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>
						<plx:local name="allConstraintRoleReadingOrders" dataTypeName="ReadingOrder" dataTypeIsSimpleArray="true">
							<plx:initialize>
								<plx:callNew dataTypeName="ReadingOrder" dataTypeIsSimpleArray="true">
									<plx:passParam>
										<plx:nameRef name="constraintRoleArity"/>
									</plx:passParam>
								</plx:callNew>
							</plx:initialize>
						</plx:local>
					</xsl:when>
				</xsl:choose>
				<xsl:if test="$isInternal">
					<xsl:call-template name="PopulateBasicRoleReplacements">
						<xsl:with-param name="SubscriptConditions" select="$subscriptConditions"/>
					</xsl:call-template>
				</xsl:if>
				<plx:local name="roleReplacements" dataTypeName=".string" dataTypeIsSimpleArray="true">
					<plx:initialize>
						<plx:callNew dataTypeName=".string" dataTypeIsSimpleArray="true">
							<plx:passParam>
								<plx:nameRef name="factArity">
									<xsl:if test="not($isInternal)">
										<xsl:attribute name="name">
											<xsl:text>maxFactArity</xsl:text>
										</xsl:attribute>
									</xsl:if>
								</plx:nameRef>
							</plx:passParam>
						</plx:callNew>
					</plx:initialize>
				</plx:local>
				<plx:local name="readingOrder" dataTypeName="ReadingOrder"/>
				<xsl:for-each select="ve:ConstrainedRoles">
					<xsl:if test="position()=1">
						<xsl:call-template name="ConstraintConditions">
							<xsl:with-param name="PatternGroup" select="$patternGroup"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>
				<plx:return>
					<plx:trueKeyword/>
				</plx:return>
			</plx:function>
		</plx:class>
	</xsl:template>
	<xsl:template name="CheckErrorConditions">
		<!-- Don't proceed with verbalization if errors are present -->
		<plx:local name="errorOwner" dataTypeName="IModelErrorOwner">
			<plx:initialize>
				<plx:cast type="testCast" dataTypeName="IModelErrorOwner">
					<plx:thisKeyword/>
				</plx:cast>
			</plx:initialize>
		</plx:local>
		<plx:branch>
			<plx:condition>
				<plx:binaryOperator type="identityInequality">
					<plx:left>
						<plx:nameRef name="errorOwner"/>
					</plx:left>
					<plx:right>
						<plx:nullKeyword/>
					</plx:right>
				</plx:binaryOperator>
			</plx:condition>
			<plx:body>
				<plx:local name="firstElement" dataTypeName=".boolean">
					<plx:initialize>
						<plx:trueKeyword/>
					</plx:initialize>
				</plx:local>
				<plx:iterator localName="error" dataTypeName="ModelError">
					<plx:initialize>
						<plx:callInstance name="ErrorCollection" type="property">
							<plx:callObject>
								<plx:nameRef name="errorOwner"/>
							</plx:callObject>
						</plx:callInstance>
					</plx:initialize>
					<plx:branch>
						<plx:condition>
							<plx:nameRef name="firstElement"/>
						</plx:condition>
						<plx:body>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="firstElement"/>
								</plx:left>
								<plx:right>
									<plx:falseKeyword/>
								</plx:right>
							</plx:assign>
							<plx:callInstance name=".implied" type="delegateCall">
								<plx:callObject>
									<plx:nameRef type="parameter" name="beginVerbalization"/>
								</plx:callObject>
								<plx:passParam>
									<plx:callStatic name="ErrorReport" dataTypeName="VerbalizationContent" type="field"/>
								</plx:passParam>
							</plx:callInstance>
						</plx:body>
						<plx:fallbackBranch>
							<plx:callInstance name="WriteLine">
								<plx:callObject>
									<plx:nameRef type="parameter" name="writer"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:fallbackBranch>
					</plx:branch>
					<plx:callInstance name="Write">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
						<plx:passParam>
							<plx:callInstance name="Name" type="property">
								<plx:callObject>
									<plx:nameRef name="error"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:passParam>
					</plx:callInstance>
				</plx:iterator>
				<plx:branch>
					<plx:condition>
						<plx:unaryOperator type="booleanNot">
							<plx:nameRef name="firstElement"/>
						</plx:unaryOperator>
					</plx:condition>
					<plx:body>
						<plx:return>
							<plx:falseKeyword/>
						</plx:return>
					</plx:body>
				</plx:branch>
			</plx:body>
		</plx:branch>
	</xsl:template>
	<!-- Handle the span constraint condition attribute -->
	<xsl:template match="@span" mode="ConstraintConditionOperator">
		<xsl:choose>
			<xsl:when test=".='all'">
				<plx:binaryOperator type="equality">
					<plx:left>
						<plx:nameRef name="factArity"/>
					</plx:left>
					<plx:right>
						<plx:nameRef name="includedArity"/>
					</plx:right>
				</plx:binaryOperator>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="TerminateForInvalidAttribute">
					<xsl:with-param name="MessageText">Unrecognized value for span condition attribute</xsl:with-param>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Handle the factArity constraint condition attribute -->
	<xsl:template match="@factArity" mode="ConstraintConditionOperator">
		<plx:binaryOperator type="equality">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the minFactArity constraint condition attribute -->
	<xsl:template match="@minFactArity" mode="ConstraintConditionOperator">
		<plx:binaryOperator type="greaterThanOrEqual">
			<plx:left>
				<plx:nameRef name="minFactArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the maxFactArity constraint condition attribute -->
	<xsl:template match="@maxFactArity" mode="ConstraintConditionOperator">
		<plx:binaryOperator type="lessThanOrEqual">
			<plx:left>
				<plx:nameRef name="maxFactArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the sign constraint condition attributes -->
	<xsl:template match="@sign" mode="ConstraintConditionOperator">
		<xsl:choose>
			<xsl:when test=".='negative'">
				<plx:nameRef name="isNegative"/>
			</xsl:when>
			<xsl:when test=".='positive'">
				<plx:unaryOperator type="booleanNot">
					<plx:nameRef name="isNegative"/>
				</plx:unaryOperator>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="TerminateForInvalidAttribute">
					<xsl:with-param name="MessageText">Unrecognized value for sign condition attribute</xsl:with-param>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Terminate processing if we see an unrecognized operator -->
	<xsl:template match="@*" mode="ConstraintConditionOperator">
		<xsl:call-template name="TerminateForInvalidAttribute">
			<xsl:with-param name="MessageText">Unrecognized constraint condition attribute</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!-- Terminate processing for unrecognized attribute or attribute value-->
	<xsl:template name="TerminateForInvalidAttribute">
		<xsl:param name="MessageText"/>
		<xsl:message terminate="yes">
			<xsl:value-of select="$MessageText"/>
			<xsl:text>: </xsl:text>
			<xsl:value-of select="local-name()"/>
			<xsl:text>="</xsl:text>
			<xsl:value-of select="."/>
			<xsl:text>"</xsl:text>
		</xsl:message>
	</xsl:template>
	<!-- Helper template for spitting conditions based on specified conditions. All conditions
		 are combined with an and operator, and are given priority based on the order they
		 appear in the data file. The assumption is made that the unconstrained condition
		 is sorted last. -->
	<xsl:template name="ConstraintConditions">
		<xsl:param name="PatternGroup"/>
		<xsl:param name="fallback" select="false()"/>
		<xsl:variable name="conditionTestFragment">
			<xsl:variable name="conditionOperatorsFragment">
				<xsl:apply-templates select="@*" mode="ConstraintConditionOperator"/>
			</xsl:variable>
			<xsl:for-each select="msxsl:node-set($conditionOperatorsFragment)/child::*">
				<xsl:if test="position()=1">
					<xsl:call-template name="CombineElements">
						<xsl:with-param name="OperatorType" select="'booleanAnd'"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="conditionTest" select="msxsl:node-set($conditionTestFragment)/child::*"/>
		<xsl:choose>
			<xsl:when test="$fallback">
				<xsl:choose>
					<xsl:when test="$conditionTest">
						<plx:alternateBranch>
							<plx:condition>
								<xsl:copy-of select="$conditionTest"/>
							</plx:condition>
							<plx:body>
								<xsl:call-template name="ConstraintBodyContent">
									<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
								</xsl:call-template>
							</plx:body>
						</plx:alternateBranch>
					</xsl:when>
					<xsl:otherwise>
						<plx:fallbackBranch>
							<xsl:call-template name="ConstraintBodyContent">
								<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
							</xsl:call-template>
						</plx:fallbackBranch>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$conditionTest">
						<plx:branch>
							<plx:condition>
								<xsl:copy-of select="$conditionTest"/>
							</plx:condition>
							<plx:body>
								<xsl:call-template name="ConstraintBodyContent">
									<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
								</xsl:call-template>
							</plx:body>
							<xsl:for-each select="following-sibling::*">
								<xsl:call-template name="ConstraintConditions">
									<xsl:with-param name="fallback" select="true()"/>
									<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
								</xsl:call-template>
							</xsl:for-each>
						</plx:branch>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="ConstraintBodyContent">
							<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Helper template to combine expressions using the specified OperatorType. An external
		 call should fire this from inside a for each for the first element, it will then
		 recurse to pick up remaining elements -->
	<xsl:template name="CombineElements">
		<xsl:param name="OperatorType"/>
		<xsl:choose>
			<xsl:when test="position()=last()">
				<xsl:copy-of select="."/>
			</xsl:when>
			<xsl:otherwise>
				<plx:binaryOperator type="{$OperatorType}">
					<plx:left>
						<xsl:copy-of select="."/>
					</plx:left>
					<plx:right>
						<xsl:for-each select="following-sibling::*">
							<xsl:if test="position()=1">
								<xsl:call-template name="CombineElements">
									<xsl:with-param name="OperatorType" select="$OperatorType"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:for-each>
					</plx:right>
				</plx:binaryOperator>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Declare the basicRoleReplacements variable for a single fact and populate the basic
		 replacement fields. The fact's roles will be in the factRoles variable
		 and the fact arity in the factArity variable -->
	<xsl:template name="PopulateBasicRoleReplacements">
		<xsl:param name="SubscriptConditions"/>
		<plx:local name="basicRoleReplacements" dataTypeName=".string" dataTypeIsSimpleArray="true">
			<plx:initialize>
				<plx:callNew dataTypeName=".string" dataTypeIsSimpleArray="true">
					<plx:passParam>
						<plx:nameRef name="factArity"/>
					</plx:passParam>
				</plx:callNew>
			</plx:initialize>
		</plx:local>
		<plx:local name="i" dataTypeName=".i4">
			<plx:initialize>
				<plx:value type="i4" data="0"/>
			</plx:initialize>
		</plx:local>
		<plx:loop>
			<plx:condition>
				<plx:binaryOperator type="lessThan">
					<plx:left>
						<plx:nameRef name="i"/>
					</plx:left>
					<plx:right>
						<plx:nameRef name="factArity"/>
					</plx:right>
				</plx:binaryOperator>
			</plx:condition>
			<plx:beforeLoop>
				<plx:increment>
					<plx:nameRef name="i"/>
				</plx:increment>
			</plx:beforeLoop>
			<plx:body>
				<plx:local name="rolePlayer" dataTypeName="ObjectType">
					<plx:initialize>
						<plx:callInstance name="RolePlayer" type="property">
							<plx:callObject>
								<plx:callInstance name=".implied" type="indexerCall">
									<plx:callObject>
										<plx:nameRef name="factRoles"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="i"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:callObject>
						</plx:callInstance>
					</plx:initialize>
				</plx:local>
				<plx:local name="basicReplacement" dataTypeName=".string"/>
				<plx:branch>
					<plx:condition>
						<plx:binaryOperator type="identityInequality">
							<plx:left>
								<plx:nameRef name="rolePlayer"/>
							</plx:left>
							<plx:right>
								<plx:nullKeyword/>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:body>
						<!-- Generation of the non-subscripted basic replacement -->
						<xsl:variable name="nonSubscriptedObjectTypeBody">
							<plx:assign>
								<plx:left>
									<plx:nameRef name="basicReplacement"/>
								</plx:left>
								<plx:right>
									<plx:callStatic name="Format" dataTypeName=".string">
										<plx:passParam>
											<plx:callInstance name="FormatProvider" type="property">
												<plx:callObject>
													<plx:nameRef type="parameter" name="writer"/>
												</plx:callObject>
											</plx:callInstance>
										</plx:passParam>
										<plx:passParam>
											<xsl:call-template name="SnippetFor">
												<xsl:with-param name="SnippetType" select="'ObjectType'"/>
											</xsl:call-template>
										</plx:passParam>
										<plx:passParam>
											<plx:callInstance name="Name" type="property">
												<plx:callObject>
													<plx:nameRef name="rolePlayer"/>
												</plx:callObject>
											</plx:callInstance>
										</plx:passParam>
									</plx:callStatic>
								</plx:right>
							</plx:assign>
						</xsl:variable>
						<xsl:choose>
							<xsl:when test="$SubscriptConditions">
								<!-- Portions of the subscripting code that are conditional placed in
									 different spots -->
								<xsl:variable name="subscriptBody">
									<plx:local name="j" dataTypeName=".i4">
										<plx:initialize>
											<plx:value type="i4" data="0"/>
										</plx:initialize>
									</plx:local>
									<plx:loop>
										<plx:condition>
											<plx:binaryOperator type="lessThan">
												<plx:left>
													<plx:nameRef name="j"/>
												</plx:left>
												<plx:right>
													<plx:nameRef name="i"/>
												</plx:right>
											</plx:binaryOperator>
										</plx:condition>
										<plx:beforeLoop>
											<plx:increment>
												<plx:nameRef name="j"/>
											</plx:increment>
										</plx:beforeLoop>
										<plx:body>
											<plx:branch>
												<plx:condition>
													<plx:callStatic name="ReferenceEquals" dataTypeName=".object">
														<plx:passParam>
															<plx:nameRef name="rolePlayer"/>
														</plx:passParam>
														<plx:passParam>
															<plx:callInstance name="RolePlayer" type="property">
																<plx:callObject>
																	<plx:callInstance name=".implied" type="indexerCall">
																		<plx:callObject>
																			<plx:nameRef name="factRoles"/>
																		</plx:callObject>
																		<plx:passParam>
																			<plx:nameRef name="j"/>
																		</plx:passParam>
																	</plx:callInstance>
																</plx:callObject>
															</plx:callInstance>
														</plx:passParam>
													</plx:callStatic>
												</plx:condition>
												<plx:body>
													<plx:assign>
														<plx:left>
															<plx:nameRef name="useSubscript"/>
														</plx:left>
														<plx:right>
															<plx:trueKeyword/>
														</plx:right>
													</plx:assign>
													<plx:assign>
														<plx:left>
															<plx:nameRef name="subscript"/>
														</plx:left>
														<plx:right>
															<plx:binaryOperator type="add">
																<plx:left>
																	<plx:nameRef name="subscript"/>
																</plx:left>
																<plx:right>
																	<plx:value type="i4" data="1"/>
																</plx:right>
															</plx:binaryOperator>
														</plx:right>
													</plx:assign>
												</plx:body>
											</plx:branch>
										</plx:body>
									</plx:loop>
									<plx:loop>
										<plx:initializeLoop>
											<plx:assign>
												<plx:left>
													<plx:nameRef name="j"/>
												</plx:left>
												<plx:right>
													<plx:binaryOperator type="add">
														<plx:left>
															<plx:nameRef name="i"/>
														</plx:left>
														<plx:right>
															<plx:value type="i4" data="1"/>
														</plx:right>
													</plx:binaryOperator>
												</plx:right>
											</plx:assign>
										</plx:initializeLoop>
										<plx:condition>
											<plx:binaryOperator type="booleanAnd">
												<plx:left>
													<plx:unaryOperator type="booleanNot">
														<plx:nameRef name="useSubscript"/>
													</plx:unaryOperator>
												</plx:left>
												<plx:right>
													<plx:binaryOperator type="lessThan">
														<plx:left>
															<plx:nameRef name="j"/>
														</plx:left>
														<plx:right>
															<plx:nameRef name="factArity"/>
														</plx:right>
													</plx:binaryOperator>
												</plx:right>
											</plx:binaryOperator>
										</plx:condition>
										<plx:beforeLoop>
											<plx:increment>
												<plx:nameRef name="j"/>
											</plx:increment>
										</plx:beforeLoop>
										<plx:body>
											<plx:branch>
												<plx:condition>
													<plx:callStatic name="ReferenceEquals" dataTypeName=".object">
														<plx:passParam>
															<plx:nameRef name="rolePlayer"/>
														</plx:passParam>
														<plx:passParam>
															<plx:callInstance name="RolePlayer" type="property">
																<plx:callObject>
																	<plx:callInstance name=".implied" type="indexerCall">
																		<plx:callObject>
																			<plx:nameRef name="factRoles"/>
																		</plx:callObject>
																		<plx:passParam>
																			<plx:nameRef name="j"/>
																		</plx:passParam>
																	</plx:callInstance>
																</plx:callObject>
															</plx:callInstance>
														</plx:passParam>
													</plx:callStatic>
												</plx:condition>
												<plx:body>
													<plx:assign>
														<plx:left>
															<plx:nameRef name="useSubscript"/>
														</plx:left>
														<plx:right>
															<plx:trueKeyword/>
														</plx:right>
													</plx:assign>
												</plx:body>
											</plx:branch>
										</plx:body>
									</plx:loop>
								</xsl:variable>
								<!-- See if we need a subscript by comparing to other role players before and after this one -->
								<plx:local name="subscript" dataTypeName=".i4">
									<plx:initialize>
										<plx:value type="i4" data="0"/>
									</plx:initialize>
								</plx:local>
								<plx:local name="useSubscript" dataTypeName=".boolean">
									<plx:initialize>
										<plx:falseKeyword/>
									</plx:initialize>
								</plx:local>
								<xsl:choose>
									<xsl:when test="count($SubscriptConditions)=1 and local-name($SubscriptConditions)='TrueKeyword'">
										<xsl:copy-of select="$subscriptBody"/>
									</xsl:when>
									<xsl:otherwise>
										<plx:branch>
											<plx:condition>
												<xsl:copy-of select="$SubscriptConditions"/>
											</plx:condition>
											<plx:body>
												<xsl:copy-of select="$subscriptBody"/>
											</plx:body>
										</plx:branch>
									</xsl:otherwise>
								</xsl:choose>
								<plx:branch>
									<plx:condition>
										<plx:nameRef name="useSubscript"/>
									</plx:condition>
									<plx:body>
										<plx:assign>
											<plx:left>
												<plx:nameRef name="basicReplacement"/>
											</plx:left>
											<plx:right>
												<plx:callStatic name="Format" dataTypeName=".string">
													<plx:passParam>
														<plx:callInstance name="FormatProvider" type="property">
															<plx:callObject>
																<plx:nameRef type="parameter" name="writer"/>
															</plx:callObject>
														</plx:callInstance>
													</plx:passParam>
													<plx:passParam>
														<xsl:call-template name="SnippetFor">
															<xsl:with-param name="SnippetType" select="'ObjectTypeWithSubscript'"/>
														</xsl:call-template>
													</plx:passParam>
													<plx:passParam>
														<plx:callInstance name="Name" type="property">
															<plx:callObject>
																<plx:nameRef name="rolePlayer"/>
															</plx:callObject>
														</plx:callInstance>
													</plx:passParam>
													<plx:passParam>
														<plx:binaryOperator type="add">
															<plx:left>
																<plx:nameRef name="subscript"/>
															</plx:left>
															<plx:right>
																<plx:value type="i4" data="1"/>
															</plx:right>
														</plx:binaryOperator>
													</plx:passParam>
												</plx:callStatic>
											</plx:right>
										</plx:assign>
									</plx:body>
									<plx:fallbackBranch>
										<xsl:copy-of select="$nonSubscriptedObjectTypeBody"/>
									</plx:fallbackBranch>
								</plx:branch>
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy-of select="$nonSubscriptedObjectTypeBody"/>
							</xsl:otherwise>
						</xsl:choose>
					</plx:body>
					<plx:fallbackBranch>
						<plx:assign>
								<plx:left>
								<plx:nameRef name="basicReplacement"/>
							</plx:left>
							<plx:right>
								<plx:callStatic name="Format" dataTypeName=".string">
									<plx:passParam>
										<plx:callInstance name="FormatProvider" type="property">
											<plx:callObject>
												<plx:nameRef type="parameter" name="writer"/>
											</plx:callObject>
										</plx:callInstance>
									</plx:passParam>
									<plx:passParam>
										<xsl:call-template name="SnippetFor">
											<xsl:with-param name="SnippetType" select="'ObjectTypeMissing'"/>
										</xsl:call-template>
									</plx:passParam>
									<plx:passParam>
										<plx:binaryOperator type="add">
											<plx:left>
												<plx:nameRef name="i"/>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="1"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:passParam>
								</plx:callStatic>
							</plx:right>
						</plx:assign>
					</plx:fallbackBranch>
				</plx:branch>
				<plx:assign>
						<plx:left>
						<plx:callInstance name=".implied" type="arrayIndexer">
							<plx:callObject>
								<plx:nameRef name="basicRoleReplacements"/>
							</plx:callObject>
							<plx:passParam>
								<plx:nameRef name="i"/>
							</plx:passParam>
						</plx:callInstance>
					</plx:left>
					<plx:right>
						<plx:nameRef name="basicReplacement"/>
					</plx:right>
				</plx:assign>
			</plx:body>
		</plx:loop>
	</xsl:template>
	<xsl:template name="ConstraintBodyContent">
		<xsl:param name="PatternGroup"/>
		<!-- At this point we'll either have ConditionalReading or Snippet children -->
		<xsl:apply-templates select="child::*" mode="ConstraintVerbalization">
			<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
			<xsl:with-param name="TopLevel" select="true()"/>
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template match="ve:ConditionalReading" mode="ConstraintVerbalization">
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:param name="PatternGroup"/>
		<xsl:for-each select="ve:ReadingChoice">
			<xsl:if test="position()=1">
				<xsl:call-template name="ProcessConditionalReadingChoice">
					<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
					<xsl:with-param name="TopLevel" select="$TopLevel"/>
					<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
				</xsl:call-template>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="ProcessConditionalReadingChoice">
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:param name="VariableDecorator" select="'1'"/>
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="Match" select="@match"/>
		<xsl:param name="PatternGroup"/>
		<xsl:choose>
			<xsl:when test="contains($Match,'All')">
				<xsl:variable name="singleMatch" select="concat(substring-before($Match,'All'), substring-after($Match,'All'))"/>
				<plx:local name="missingReading{$VariableDecorator}" dataTypeName=".boolean">
					<plx:initialize>
						<plx:falseKeyword/>
					</plx:initialize>
				</plx:local>
				<plx:local name="readingMatchIndex{$VariableDecorator}" dataTypeName=".i4">
					<plx:initialize>
						<plx:value type="i4" data="0"/>
					</plx:initialize>
				</plx:local>
				<plx:loop>
					<plx:condition>
						<plx:binaryOperator type="booleanAnd">
							<plx:left>
								<plx:unaryOperator type="booleanNot">
									<plx:nameRef name="missingReading{$VariableDecorator}"/>
								</plx:unaryOperator>
							</plx:left>
							<plx:right>
								<plx:binaryOperator type="lessThan">
									<plx:left>
										<plx:nameRef name="readingMatchIndex{$VariableDecorator}"/>
									</plx:left>
									<plx:right>
										<plx:nameRef name="constraintRoleArity"/>
									</plx:right>
								</plx:binaryOperator>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:beforeLoop>
						<plx:increment>
							<plx:nameRef name="readingMatchIndex{$VariableDecorator}"/>
						</plx:increment>
					</plx:beforeLoop>
					<plx:body>
						<plx:local name="primaryRole" dataTypeName="Role">
							<plx:initialize>
								<plx:callInstance name=".implied" type="indexerCall">
									<plx:callObject>
										<plx:nameRef name="allConstraintRoles"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="readingMatchIndex{$VariableDecorator}"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>
						<plx:assign>
								<plx:left>
								<plx:nameRef name="parentFact"/>
							</plx:left>
							<plx:right>
								<plx:callInstance name="FactType" type="property">
									<plx:callObject>
										<plx:nameRef name="primaryRole"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:right>
						</plx:assign>
						<plx:assign>
								<plx:left>
								<plx:nameRef name="factRoles"/>
							</plx:left>
							<plx:right>
								<plx:callInstance name="RoleCollection" type="property">
									<plx:callObject>
										<plx:nameRef name="parentFact"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:right>
						</plx:assign>
						<plx:assign>
								<plx:left>
								<plx:nameRef name="allReadingOrders"/>
							</plx:left>
							<plx:right>
								<plx:callInstance name="ReadingOrderCollection" type="property">
									<plx:callObject>
										<plx:nameRef name="parentFact"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:right>
						</plx:assign>
						<xsl:call-template name="PopulateReadingOrder">
							<xsl:with-param name="ReadingChoice" select="$singleMatch"/>
							<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
						</xsl:call-template>
						<plx:branch>
							<plx:condition>
								<plx:binaryOperator type="identityEquality">
									<plx:left>
										<plx:nameRef name="readingOrder"/>
									</plx:left>
									<plx:right>
										<plx:nullKeyword/>
									</plx:right>
								</plx:binaryOperator>
							</plx:condition>
							<plx:body>
								<plx:assign>
									<plx:left>
										<plx:nameRef name="missingReading{$VariableDecorator}"/>
									</plx:left>
									<plx:right>
										<plx:trueKeyword/>
									</plx:right>
								</plx:assign>
							</plx:body>
							<plx:fallbackBranch>
								<plx:assign>
									<plx:left>
										<plx:callInstance name=".implied" type="arrayIndexer">
											<plx:callObject>
												<plx:nameRef name="allConstraintRoleReadingOrders"/>
											</plx:callObject>
											<plx:passParam>
												<plx:nameRef name="readingMatchIndex{$VariableDecorator}"/>
											</plx:passParam>
										</plx:callInstance>
									</plx:left>
									<plx:right>
										<plx:nameRef name="readingOrder"/>
									</plx:right>
								</plx:assign> 
							</plx:fallbackBranch>
						</plx:branch>
					</plx:body>
				</plx:loop>
				<plx:branch>
					<plx:condition>
						<plx:unaryOperator type="booleanNot">
							<plx:nameRef name="missingReading{$VariableDecorator}"/>
						</plx:unaryOperator>
					</plx:condition>
					<!-- The rest of this block is duplicated in otherwise condition, keep in sync -->
					<plx:body>
						<xsl:apply-templates select="child::*" mode="ConstraintVerbalization">
							<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
							<xsl:with-param name="TopLevel" select="$TopLevel"/>
							<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
						</xsl:apply-templates>
					</plx:body>
					<xsl:if test="position()!=last()">
						<plx:fallbackBranch>
							<xsl:for-each select="following-sibling::*">
								<xsl:if test="position()=1">
									<xsl:call-template name="ProcessConditionalReadingChoice">
										<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
										<xsl:with-param name="VariableDecorator" select="$VariableDecorator + 1"/>
										<xsl:with-param name="TopLevel" select="$TopLevel"/>
										<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:for-each>
						</plx:fallbackBranch>
					</xsl:if>
				</plx:branch>
			</xsl:when>
			<xsl:when test="$PatternGroup='SingleColumnExternalConstraint'">
				<xsl:apply-templates select="child::*" mode="ConstraintVerbalization">
					<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
					<xsl:with-param name="TopLevel" select="$TopLevel"/>
					<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="PopulateReadingOrder">
					<xsl:with-param name="ReadingChoice" select="$Match"/>
					<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
				</xsl:call-template>
				<plx:branch>
					<plx:condition>
						<plx:binaryOperator type="identityInequality">
							<plx:left>
								<plx:nameRef name="readingOrder"/>
							</plx:left>
							<plx:right>
								<plx:nullKeyword/>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<!-- The rest of this block is duplicated in when condition, keep in sync -->
					<plx:body>
						<xsl:apply-templates select="child::*" mode="ConstraintVerbalization">
							<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
							<xsl:with-param name="TopLevel" select="$TopLevel"/>
							<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
						</xsl:apply-templates>
					</plx:body>
					<xsl:if test="position()!=last()">
						<plx:fallbackBranch>
							<xsl:for-each select="following-sibling::*">
								<xsl:if test="position()=1">
									<xsl:call-template name="ProcessConditionalReadingChoice">
										<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
										<xsl:with-param name="VariableDecorator" select="$VariableDecorator + 1"/>
										<xsl:with-param name="TopLevel" select="$TopLevel"/>
										<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:for-each>
						</plx:fallbackBranch>
					</xsl:if>
				</plx:branch>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="ve:Snippet" mode="ConstraintVerbalization">
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'snippet'"/>
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:param name="PatternGroup"/>
		<xsl:if test="$TopLevel">
			<xsl:choose>
				<xsl:when test="position()&gt;1">
					<plx:callInstance name="WriteLine">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
					</plx:callInstance>
				</xsl:when>
				<xsl:otherwise>
					<plx:callInstance name=".implied" type="delegateCall">
						<plx:callObject>
							<plx:nameRef type="parameter" name="beginVerbalization"/>
						</plx:callObject>
						<plx:passParam>
							<plx:callStatic name="Normal" dataTypeName="VerbalizationContent" type="field"/>
						</plx:passParam>
					</plx:callInstance>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
		<plx:local name="{$VariablePrefix}{$FormatVariablePart}{$VariableDecorator}" dataTypeName=".string">
			<plx:initialize>
				<xsl:call-template name="SnippetFor">
					<xsl:with-param name="SnippetType" select="@ref"/>
				</xsl:call-template>
			</plx:initialize>
		</plx:local>
		<xsl:for-each select="child::*">
			<plx:local name="{$VariablePrefix}{$VariableDecorator}{$ReplaceVariablePart}{position()}" dataTypeName=".string">
				<plx:initialize>
					<plx:nullKeyword/>
				</plx:initialize>
			</plx:local>
			<xsl:apply-templates select="."  mode="ConstraintVerbalization">
				<xsl:with-param name="VariablePrefix" select="concat($VariablePrefix,$VariableDecorator,$ReplaceVariablePart)"/>
				<!-- The position will jump back to 1 with this call, so pick up the real position before jumping -->
				<xsl:with-param name="VariableDecorator" select="position()"/>
				<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
				<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
			</xsl:apply-templates>
		</xsl:for-each>
		<xsl:variable name="formatCall">
			<plx:callStatic name="Format" dataTypeName=".string">
				<plx:passParam>
					<plx:callInstance name="FormatProvider" type="property">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
					</plx:callInstance>
				</plx:passParam>
				<plx:passParam>
					<plx:nameRef name="{$VariablePrefix}{$FormatVariablePart}{$VariableDecorator}"/>
				</plx:passParam>
				<xsl:for-each select="child::*">
					<plx:passParam>
						<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}{$ReplaceVariablePart}{position()}"/>
					</plx:passParam>
				</xsl:for-each>
			</plx:callStatic>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$TopLevel">
				<!-- Write the snippet directly to the text writer after sentence modification -->
				<plx:callStatic name="WriteVerbalizerSentence" dataTypeName="FactType">
					<plx:passParam>
						<plx:nameRef type="parameter" name="writer"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:copy-of select="$formatCall"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:call-template name="SnippetFor">
							<xsl:with-param name="SnippetType" select="'CloseVerbalizationSentence'"/>
						</xsl:call-template>
					</plx:passParam>
				</plx:callStatic>
			</xsl:when>
			<xsl:otherwise>
				<!-- Snippet is used as a replacement field in another snippet -->
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:left>
					<plx:right>
						<xsl:copy-of select="$formatCall"/>
					</plx:right>
				</plx:assign>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="ve:Fact" mode="ConstraintVerbalization">
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'factText'"/>
		<xsl:param name="PatternGroup"/>
		<xsl:param name="FirstPassVariable"/>
		<!-- all, included or excluded are the supported IteratorContext -->
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:if test="$TopLevel">
			<xsl:choose>
				<xsl:when test="position()&gt;1">
					<plx:callInstance name="WriteLine">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
					</plx:callInstance>
				</xsl:when>
				<xsl:otherwise>
					<plx:callInstance name=".implied" type="delegateCall">
						<plx:callObject>
							<plx:nameRef type="parameter" name="beginVerbalization"/>
						</plx:callObject>
						<plx:passParam>
							<plx:callStatic name="Normal" dataTypeName="VerbalizationContent" type="field"/>
						</plx:passParam>
					</plx:callInstance>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
		<xsl:variable name="complexReplacement" select="0!=count(ve:PredicateReplacement)"/>
		<xsl:call-template name="PopulateReadingOrder">
			<xsl:with-param name="ReadingChoice" select="@readingChoice"/>
			<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
			<xsl:with-param name="ConditionalReadingOrderIndex">
				<xsl:if test="$IteratorContext='singleColumnConstraintRoles'">
					<xsl:value-of select="concat($VariablePrefix,$RoleIterVariablePart,$VariableDecorator)"/>
				</xsl:if>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:choose>
			<xsl:when test="$complexReplacement">
				<xsl:variable name="iterVarName" select="concat($VariablePrefix,$FactRoleIterVariablePart,$VariableDecorator)"/>
				<plx:local name="{$iterVarName}" dataTypeName=".i4">
					<plx:initialize>
						<plx:value type="i4" data="0"/>
					</plx:initialize>
				</plx:local>
				<plx:loop>
					<plx:condition>
						<plx:binaryOperator type="lessThan">
							<plx:left>
								<plx:nameRef name="{$iterVarName}"/>
							</plx:left>
							<plx:right>
								<plx:nameRef name="factArity"/>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:beforeLoop>
						<plx:increment>
							<plx:nameRef name="{$iterVarName}"/>
						</plx:increment>
					</plx:beforeLoop>
					<plx:body>
						<!-- Initialize variables used for all styles of predicate replacement -->
						<plx:local name="currentRole" dataTypeName="Role">
							<plx:initialize>
								<plx:callInstance name=".implied" type="arrayIndexer">
									<plx:callObject>
										<plx:nameRef name="factRoles"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="{$iterVarName}"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>
						<plx:local name="roleReplacement" dataTypeName=".string">
							<plx:initialize>
								<plx:nullKeyword/>
							</plx:initialize>
						</plx:local>
						<plx:local name="basicReplacement" dataTypeName=".string">
							<plx:initialize>
								<plx:callInstance name=".implied" type="arrayIndexer">
									<plx:callObject>
										<plx:nameRef name="basicRoleReplacements"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="{$iterVarName}"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>

						<!-- Do specialized replacement for different role matches -->
						<xsl:for-each select="ve:PredicateReplacement">
							<!-- The assumption is made here that predicate replacement quantifiers
								 are single-valued. -->
							<xsl:if test="position()=1">
								<xsl:choose>
									<xsl:when test="(position()!=last()) or (string-length(@match) and not(@match='all'))">
										<plx:branch>
											<plx:condition>
												<xsl:call-template name="PredicateReplacementConditionTest">
													<xsl:with-param name="Match" select="@match"/>
													<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
													<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
													<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
													<xsl:with-param name="VariableDecorator" select="$VariableDecorator"/>
													<xsl:with-param name="FirstPassVariable" select="$FirstPassVariable"/>
												</xsl:call-template>
											</plx:condition>
											<plx:body>
												<xsl:call-template name="PredicateReplacementBody"/>
											</plx:body>
											<xsl:for-each select="following-sibling::*">
												<xsl:choose>
													<xsl:when test="(position()!=last()) or (string-length(@match) and not(@match='all'))">
														<plx:alternateBranch>
															<plx:condition>
																<xsl:call-template name="PredicateReplacementConditionTest">
																	<xsl:with-param name="Match" select="@match"/>
																	<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
																	<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
																	<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
																	<xsl:with-param name="VariableDecorator" select="$VariableDecorator"/>
																	<xsl:with-param name="FirstPassVariable" select="$FirstPassVariable"/>
																</xsl:call-template>
															</plx:condition>
															<plx:body>
																<xsl:call-template name="PredicateReplacementBody"/>
															</plx:body>
														</plx:alternateBranch>
													</xsl:when>
													<xsl:otherwise>
														<plx:fallbackBranch>
																<xsl:call-template name="PredicateReplacementBody"/>
															</plx:fallbackBranch>
													</xsl:otherwise>
												</xsl:choose>
											</xsl:for-each>
										</plx:branch>
									</xsl:when>
									<xsl:otherwise>
										<xsl:call-template name="PredicateReplacementBody"/>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:if>
						</xsl:for-each>

						<!-- Use the default replacement for the predicate text if nothing was specified -->
						<plx:branch>
							<plx:condition>
								<plx:binaryOperator type="identityEquality">
									<plx:left>
										<plx:nameRef name="roleReplacement"/>
									</plx:left>
									<plx:right>
										<plx:nullKeyword/>
									</plx:right>
								</plx:binaryOperator>
							</plx:condition>
							<plx:body>
								<plx:assign>
									<plx:left>
										<plx:nameRef name="roleReplacement"/>
									</plx:left>
									<plx:right>
										<plx:nameRef name="basicReplacement"/>
									</plx:right>
								</plx:assign>
							</plx:body>
						</plx:branch>
						<plx:assign>
							<plx:left>
								<plx:callInstance name=".implied" type="arrayIndexer">
									<plx:callObject>
										<plx:nameRef name="roleReplacements"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="{$iterVarName}"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:left>
							<plx:right>
								<plx:nameRef name="roleReplacement"/>
							</plx:right>
						</plx:assign>
					</plx:body>
				</plx:loop>
			</xsl:when>
		</xsl:choose>
		<xsl:variable name="predicateText">
			<plx:callStatic name="PopulatePredicateText" dataTypeName="FactType">
				<plx:passParam>
					<plx:nameRef name="readingOrder"/>
				</plx:passParam>
				<plx:passParam>
					<plx:nameRef name="factRoles"/>
				</plx:passParam>
				<plx:passParam>
					<xsl:variable name="replacementSet">
						<xsl:choose>
							<xsl:when test="$complexReplacement">
								<xsl:text>roleReplacements</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>basicRoleReplacements</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<plx:nameRef name="{$replacementSet}"/>
				</plx:passParam>
			</plx:callStatic>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$TopLevel">
				<plx:callStatic name="WriteVerbalizerSentence" dataTypeName="FactType">
					<plx:passParam>
						<plx:nameRef type="parameter" name="writer"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:copy-of select="$predicateText"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:call-template name="SnippetFor">
							<xsl:with-param name="SnippetType" select="'CloseVerbalizationSentence'"/>
						</xsl:call-template>
					</plx:passParam>
				</plx:callStatic>
			</xsl:when>
			<xsl:otherwise>
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:left>
					<plx:right>
						<xsl:copy-of select="$predicateText"/>
					</plx:right>
				</plx:assign>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Handle the minFactArity EnableSubscripts condition attribute -->
	<xsl:template match="@minFactArity" mode="SubscriptFilterOperators">
		<plx:binaryOperator type="greaterThanOrEqual">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the maxFactArity EnableSubscripts condition attribute -->
	<xsl:template match="@maxFactArity" mode="SubscriptFilterOperators">
		<plx:binaryOperator type="lessThanOrEqual">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the factArity EnableSubscripts condition attribute -->
	<xsl:template match="@factArity" mode="SubscriptFilterOperators">
		<plx:binaryOperator type="equality">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Terminate processing if we see an unrecognized operator -->
	<xsl:template match="@*" mode="IterateRolesFilterOperator">
		<xsl:call-template name="TerminateForInvalidAttribute">
			<xsl:with-param name="MessageText">Unrecognized subscript condition iterator filter attribute</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="ve:EnableSubscripts" mode="SubscriptConditions">
		<xsl:variable name="conditionalsFragment">
			<xsl:apply-templates select="@*" mode="SubscriptFilterOperators"/>
		</xsl:variable>
		<xsl:variable name="conditions" select="msxsl:node-set($conditionalsFragment)/child::*"/>
		<xsl:choose>
			<xsl:when test="count($conditions)">
				<xsl:for-each select="$conditions">
					<xsl:if test="position()=1">
						<xsl:call-template name="CombineElements">
							<xsl:with-param name="OperatorType" select="'booleanAnd'"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<plx:trueKeyword/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="ve:EnableSubscripts" mode="ConstraintVerbalization">
		<!-- Don't do anything in this mode. We preprocess these directive elements -->
	</xsl:template>
	<xsl:template name="PredicateReplacementConditionTest">
		<xsl:param name="Match"/>
		<xsl:param name="IteratorContext"/>
		<xsl:param name="PatternGroup"/>
		<xsl:param name="VariablePrefix"/>
		<xsl:param name="VariableDecorator"/>
		<xsl:param name="FirstPassVariable"/>
		<xsl:variable name="operatorsFragment">
			<xsl:choose>
				<xsl:when test="$Match='primary'">
					<plx:binaryOperator type="identityEquality">
						<plx:left>
							<plx:nameRef name="primaryRole"/>
						</plx:left>
						<plx:right>
							<plx:nameRef name="currentRole"/>
						</plx:right>
					</plx:binaryOperator>
				</xsl:when>
				<xsl:when test="$Match='secondary'">
					<xsl:choose>
						<xsl:when test="$IteratorContext='included'">
							<plx:binaryOperator type="identityInequality">
								<plx:left>
									<plx:nameRef name="primaryRole"/>
								</plx:left>
								<plx:right>
									<plx:nameRef name="currentRole"/>
								</plx:right>
							</plx:binaryOperator>
							<plx:unaryOperator type="booleanNot">
								<plx:callInstance name="Contains">
									<plx:callObject>
										<plx:nameRef name="includedRoles"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="currentRole"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:unaryOperator>
						</xsl:when>
						<xsl:when test="$IteratorContext='excluded'">
							<plx:binaryOperator type="identityInequality">
								<plx:left>
									<plx:nameRef name="primaryRole"/>
								</plx:left>
								<plx:right>
									<plx:nameRef name="currentRole"/>
								</plx:right>
							</plx:binaryOperator>
							<plx:callInstance name="Contains">
								<plx:callObject>
									<plx:nameRef name="includedRoles"/>
								</plx:callObject>
								<plx:passParam>
									<plx:nameRef name="currentRole"/>
								</plx:passParam>
							</plx:callInstance>
						</xsl:when>
						<xsl:otherwise>
							<plx:binaryOperator type="identityInequality">
								<plx:left>
									<plx:nameRef name="primaryRole"/>
								</plx:left>
								<plx:right>
									<plx:nameRef name="currentRole"/>
								</plx:right>
							</plx:binaryOperator>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="$Match='included'">
					<xsl:choose>
						<xsl:when test="$IteratorContext='singleColumnConstraintRoles'">
							<!-- For the single column case, the included role is always a set consisting of the primary role only -->
							<plx:binaryOperator type="identityEquality">
								<plx:left>
									<plx:nameRef name="currentRole"/>
								</plx:left>
								<plx:right>
									<plx:nameRef name="primaryRole"/>
								</plx:right>
							</plx:binaryOperator>
						</xsl:when>
						<xsl:otherwise>
							<plx:callInstance name="Contains">
								<plx:callObject>
									<plx:nameRef name="includedRoles"/>
								</plx:callObject>
								<plx:passParam>
									<plx:nameRef name="currentRole"/>
								</plx:passParam>
							</plx:callInstance>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="$Match='excluded'">
					<xsl:choose>
						<xsl:when test="$IteratorContext='singleColumnConstraintRoles'">
							<!-- For the single column case, the included role is always a set consisting of the primary role only -->
							<plx:binaryOperator type="identityInequality">
								<plx:left>
									<plx:nameRef name="currentRole"/>
								</plx:left>
								<plx:right>
									<plx:nameRef name="primaryRole"/>
								</plx:right>
							</plx:binaryOperator>
						</xsl:when>
						<xsl:otherwise>
							<plx:unaryOperator type="booleanNot">
								<plx:callInstance name="Contains">
									<plx:callObject>
										<plx:nameRef name="includedRoles"/>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="currentRole"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:unaryOperator>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
			</xsl:choose>
			<xsl:if test="@pass='first'">
				<plx:nameRef name="{$FirstPassVariable}"/>
			</xsl:if>
		</xsl:variable>
		<xsl:for-each select="msxsl:node-set($operatorsFragment)/child::*">
			<xsl:if test="position()=1">
				<xsl:call-template name="CombineElements">
					<xsl:with-param name="OperatorType" select="'booleanAnd'"/>
				</xsl:call-template>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="PredicateReplacementBody">
		<xsl:for-each select="ve:Snippet">
			<plx:assign>
				<plx:left>
					<plx:nameRef name="roleReplacement"/>
				</plx:left>
				<plx:right>
					<xsl:choose>
						<xsl:when test="@ref='null'">
							<!-- Special case so that we can eliminate replacement text fields -->
							<plx:string/>
						</xsl:when>
						<xsl:otherwise>
							<plx:callStatic name="Format" dataTypeName=".string">
								<plx:passParam>
									<plx:callInstance name="FormatProvider" type="property">
										<plx:callObject>
											<plx:nameRef type="parameter" name="writer"/>
										</plx:callObject>
									</plx:callInstance>
								</plx:passParam>
								<plx:passParam>
									<xsl:call-template name="SnippetFor">
										<xsl:with-param name="SnippetType" select="@ref"/>
									</xsl:call-template>
								</plx:passParam>
								<plx:passParam>
									<plx:nameRef name="basicReplacement"/>
								</plx:passParam>
							</plx:callStatic>
						</xsl:otherwise>
					</xsl:choose>
				</plx:right>
			</plx:assign>
		</xsl:for-each>
		<xsl:for-each select="ve:ConditionalSnippet">
			<xsl:for-each select="ve:Snippet">
				<xsl:if test="position()=1">
					<xsl:call-template name="ProcessConditionalSnippet"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="ProcessConditionalSnippet">
		<xsl:param name="fallback" select="false()"/>
		<xsl:variable name="conditionFragment">
			<xsl:variable name="match" select="@match"/>
			<xsl:if test="string-length($match)">
				<xsl:choose>
					<xsl:when test="$match='IsPersonal'">
						<plx:callInstance name="IsPersonal" type="property">
							<plx:callObject>
								<plx:callInstance name="RolePlayer" type="property">
									<plx:callObject>
										<plx:nameRef name="currentRole"/>
									</plx:callObject>
								</plx:callInstance>
							</plx:callObject>
						</plx:callInstance>
					</xsl:when>
					<xsl:otherwise>
						<xsl:message terminate="yes">
							<xsl:text>Unrecognized conditional snippet pattern '</xsl:text>
							<xsl:value-of select="$match"/>
							<xsl:text>'.</xsl:text>
						</xsl:message>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="condition" select="msxsl:node-set($conditionFragment)/child::*"/>
		<xsl:choose>
			<xsl:when test="$fallback">
				<xsl:choose>
					<xsl:when test="$condition">
						<plx:alternateBranch>
							<plx:condition>
								<xsl:copy-of select="$condition"/>
							</plx:condition>
							<plx:body>
								
							</plx:body>
						</plx:alternateBranch>
					</xsl:when>
					<xsl:otherwise>
						<plx:fallbackBranch>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="conditionalSnippet"/>
								</plx:left>
								<plx:right>
									<plx:callStatic type="field" dataTypeName="{$VerbalizationTextSnippetType}" name="ImpersonalPronoun"/>
								</plx:right>
							</plx:assign>
						</plx:fallbackBranch>
					</xsl:otherwise>
				</xsl:choose>
				<!-- Get the snippet, assign to variable -->

			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$condition">
						<plx:local name="conditionalSnippet" dataTypeName="VerbalizationTextSnippetType"/>
						<plx:branch>
							<plx:condition>
								<xsl:copy-of select="$condition"/>
							</plx:condition>
							<plx:body>
								<!-- Get the snippet, assign to variable -->
								<plx:assign>
									<plx:left>
										<plx:nameRef name="conditionalSnippet"/>
									</plx:left>
									<plx:right>
										<plx:callStatic type="field" dataTypeName="{$VerbalizationTextSnippetType}" name="PersonalPronoun"/>
									</plx:right>
								</plx:assign>
							</plx:body>
							<xsl:for-each select="following-sibling::*">
								<xsl:call-template name="ProcessConditionalSnippet">
									<xsl:with-param name="fallback" select="true()"/>
								</xsl:call-template>
							</xsl:for-each>
						</plx:branch>
						<plx:assign>
							<plx:left>
								<plx:nameRef name="roleReplacement"/>
							</plx:left>
							<plx:right>
								<xsl:choose>
									<xsl:when test="@ref='null'">
										<!-- Special case so that we can eliminate replacement text fields -->
										<plx:string/>
									</xsl:when>
									<xsl:otherwise>
										<plx:callStatic name="Format" dataTypeName=".string">
											<plx:passParam>
												<plx:callInstance name="FormatProvider" type="property">
													<plx:callObject>
														<plx:nameRef type="parameter" name="writer"/>
													</plx:callObject>
												</plx:callInstance>
											</plx:passParam>
											<plx:passParam>
												<xsl:call-template name="SnippetFor">
													<xsl:with-param name="VariableName" select="'conditionalSnippet'"/>
												</xsl:call-template>
											</plx:passParam>
											<plx:passParam>
												<plx:nameRef name="basicReplacement"/>
											</plx:passParam>
										</plx:callStatic>
									</xsl:otherwise>
								</xsl:choose>
							</plx:right>
						</plx:assign>
					</xsl:when>
					<xsl:otherwise>
						<!-- The conditional snippet wasn't actually needed, just process the snippet -->
						<xsl:for-each select="..">
							<xsl:call-template name="PredicateReplacementBody"/>
						</xsl:for-each>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- An IterateRoles tag is used to walk a set of roles and combine verbalizations for
		 the roles into a list. The type of verbalization depends on the match and filter attributes
		 specified on the list. The list separators are determined by the contents of the listStyle attribute. -->
	<xsl:template match="ve:IterateRoles" mode="ConstraintVerbalization">
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'factText'"/>
		<xsl:param name="CompositeCount"/>
		<xsl:param name="CompositeIterator"/>
		<xsl:param name="ListStyle" select="@listStyle"/>
		<xsl:param name="PatternGroup"/>
		<!-- Other parameters should be forwarded to IterateRolesConstraintVerbalizationBody -->
		
		<!-- Normalize the match data -->
		<xsl:variable name="contextMatchFragment">
			<xsl:choose>
				<xsl:when test="string-length(@match)">
					<xsl:value-of select="@match"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>all</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="contextMatch" select="string($contextMatchFragment)"/>
		<xsl:variable name="iterVarName" select="concat($VariablePrefix,$RoleIterVariablePart,$VariableDecorator)"/>
		<xsl:if test="$TopLevel">
			<xsl:choose>
				<xsl:when test="position()&gt;1">
					<plx:callInstance name="WriteLine">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
					</plx:callInstance>
				</xsl:when>
				<xsl:otherwise>
					<plx:callInstance name=".implied" type="delegateCall">
						<plx:callObject>
							<plx:nameRef type="parameter" name="beginVerbalization"/>
						</plx:callObject>
						<plx:passParam>
							<plx:callStatic name="Normal" dataTypeName="VerbalizationContent" type="field"/>
						</plx:passParam>
					</plx:callInstance>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
		<xsl:if test="0=string-length($CompositeCount)">
			<xsl:call-template name="EnsureTempStringBuilder"/>
		</xsl:if>
		<plx:local name="{$iterVarName}" dataTypeName=".i4">
			<plx:initialize>
				<plx:value type="i4" data="0"/>
			</plx:initialize>
		</plx:local>

		<!-- See if any filters are in place. If there are, then pre-walk the elements to
			 get a total count so we can build an accurate list during the main iterator -->
		<xsl:variable name="filterTestFragment">
			<xsl:variable name="filterOperatorsFragment">
				<xsl:apply-templates select="@*" mode="IterateRolesFilterOperator"/>
			</xsl:variable>
			<xsl:for-each select="msxsl:node-set($filterOperatorsFragment)/child::*">
				<xsl:if test="position()=1">
					<xsl:call-template name="CombineElements">
						<xsl:with-param name="OperatorType" select="'booleanAnd'"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="filterTest" select="msxsl:node-set($filterTestFragment)/child::*"/>
		<xsl:variable name="filteredCountVarName" select="concat($VariablePrefix,'FilteredCount',$VariableDecorator)"/>
		<xsl:variable name="filteredIterVarName" select="concat($VariablePrefix,'FilteredIter',$VariableDecorator)"/>
		<xsl:variable name="trackFirstPass" select="0!=count(descendant::ve:PredicateReplacement[@pass='first'])"/>
		<xsl:variable name="trackFirstPassVarName" select="concat($VariablePrefix,'IsFirstPass',$VariableDecorator)"/>
		<xsl:if test="$trackFirstPass">
			<plx:local name="{$trackFirstPassVarName}" dataTypeName=".boolean">
				<plx:initialize>
					<plx:trueKeyword/>
				</plx:initialize>
			</plx:local>
		</xsl:if>
		<xsl:if test="$filterTest">
			<xsl:if test="0=string-length($CompositeCount)">
				<plx:local name="{$filteredCountVarName}" dataTypeName=".i4">
					<plx:initialize>
						<plx:value type="i4" data="0"/>
					</plx:initialize>
				</plx:local>
				<plx:local name="{$filteredIterVarName}" dataTypeName=".i4"/>
				<xsl:apply-templates select="." mode="CompositeOrFilteredListCount">
					<xsl:with-param name="TotalCountCollectorVariable" select="$filteredCountVarName"/>
					<xsl:with-param name="IteratorVariableName" select="$filteredIterVarName"/>
					<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
				</xsl:apply-templates>
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$filteredIterVarName}"/>
					</plx:left>
					<plx:right>
						<plx:value type="i4" data="0"/>
					</plx:right>
				</plx:assign>
			</xsl:if>
		</xsl:if>
		<plx:loop>
			<plx:condition>
				<plx:binaryOperator type="lessThan">
					<plx:left>
						<plx:nameRef name="{$iterVarName}"/>
					</plx:left>
					<plx:right>
						<xsl:choose>
							<xsl:when test="@pass='first'">
								<plx:value type="i4" data="1"/>
							</xsl:when>
							<xsl:otherwise>
								<plx:nameRef>
									<xsl:attribute name="name">
										<xsl:choose>
											<xsl:when test="$contextMatch='all'">
												<xsl:text>factArity</xsl:text>
											</xsl:when>
											<xsl:when test="$contextMatch='included'">
												<xsl:text>includedArity</xsl:text>
											</xsl:when>
											<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
												<xsl:text>constraintRoleArity</xsl:text>
											</xsl:when>
											<!-- UNDONE: Support excluded match -->
										</xsl:choose>
									</xsl:attribute>
								</plx:nameRef>
							</xsl:otherwise>
						</xsl:choose>
					</plx:right>
				</plx:binaryOperator>
			</plx:condition>
			<plx:beforeLoop>
				<plx:increment>
					<plx:nameRef name="{$iterVarName}"/>
				</plx:increment>
			</plx:beforeLoop>
			<plx:body>
				<plx:local name="primaryRole" dataTypeName="Role">
					<plx:initialize>
						<plx:callInstance name=".implied" type="arrayIndexer">
							<plx:callObject>
								<plx:nameRef>
									<xsl:attribute name="name">
										<xsl:choose>
											<xsl:when test="$contextMatch='all'">
												<xsl:text>factRoles</xsl:text>
											</xsl:when>
											<xsl:when test="$contextMatch='included'">
												<xsl:text>includedRoles</xsl:text>
											</xsl:when>
											<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
												<xsl:text>allConstraintRoles</xsl:text>
											</xsl:when>
											<!-- UNDONE: Support excluded match -->
										</xsl:choose>
									</xsl:attribute>
								</plx:nameRef>
							</plx:callObject>
							<plx:passParam>
								<plx:nameRef name="{$iterVarName}"/>
							</plx:passParam>
						</plx:callInstance>
					</plx:initialize>
				</plx:local>
				<xsl:if test="$contextMatch='singleColumnConstraintRoles'">
					<plx:assign>
						<plx:left>
							<plx:nameRef name="parentFact"/>
						</plx:left>
						<plx:right>
							<plx:callInstance name="FactType" type="property">
								<plx:callObject>
									<plx:nameRef name="primaryRole"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:right>
					</plx:assign>
					<plx:assign>
						<plx:left>
							<plx:nameRef name="factRoles"/>
						</plx:left>
						<plx:right>
							<plx:callInstance name="RoleCollection" type="property">
								<plx:callObject>
									<plx:nameRef name="parentFact"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:right>
					</plx:assign>
					<plx:assign>
						<plx:left>
							<plx:nameRef name="factArity"/>
						</plx:left>
						<plx:right>
							<plx:callInstance name="Count" type="property">
								<plx:callObject>
									<plx:nameRef name="factRoles"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:right>
					</plx:assign>
					<plx:assign>
						<plx:left>
							<plx:nameRef name="allReadingOrders"/>
						</plx:left>
						<plx:right>
							<plx:callInstance name="ReadingOrderCollection" type="property">
								<plx:callObject>
									<plx:nameRef name="parentFact"/>
								</plx:callObject>
							</plx:callInstance>
						</plx:right>
					</plx:assign>
					<plx:local name="currentFactIndex" dataTypeName=".i4">
						<plx:initialize>
							<plx:callInstance name="IndexOf">
								<plx:callObject>
									<plx:nameRef name="allFacts"/>
								</plx:callObject>
								<plx:passParam>
									<plx:nameRef name="parentFact"/>
								</plx:passParam>
							</plx:callInstance>
						</plx:initialize>
					</plx:local>
					<plx:local name="basicRoleReplacements" dataTypeName=".string" dataTypeIsSimpleArray="true">
						<plx:initialize>
							<plx:callInstance name=".implied" type="arrayIndexer">
								<plx:callObject>
									<plx:nameRef name="allBasicRoleReplacements"/>
								</plx:callObject>
								<plx:passParam>
									<plx:nameRef name="currentFactIndex"/>
								</plx:passParam>
							</plx:callInstance>
						</plx:initialize>
					</plx:local>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="$filterTest">
						<plx:branch>
							<plx:condition>
								<xsl:copy-of select="$filterTest"/>
							</plx:condition>
							<plx:body>
								<xsl:variable name="passCompositeCount">
									<xsl:choose>
										<xsl:when test="string-length($CompositeCount)">
											<xsl:value-of select="$CompositeCount"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$filteredCountVarName"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<xsl:variable name="passCompositeIterator">
									<xsl:choose>
										<xsl:when test="string-length($CompositeIterator)">
											<xsl:value-of select="$CompositeIterator"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$filteredIterVarName"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<xsl:call-template name="IterateRolesConstraintVerbalizationBody">
									<xsl:with-param name="TopLevel" select="$TopLevel"/>
									<xsl:with-param name="VariableDecorator" select="$VariableDecorator"/>
									<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
									<xsl:with-param name="CompositeCount" select="string($passCompositeCount)"/>
									<xsl:with-param name="CompositeIterator" select="string($passCompositeIterator)"/>
									<xsl:with-param name="ListStyle" select="$ListStyle"/>
									<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
									<xsl:with-param name="FirstPassVariable" select="$trackFirstPassVarName"/>
									<!-- Forwarded local parameters -->
									<xsl:with-param name="contextMatch" select="$contextMatch"/>
									<xsl:with-param name="iterVarName" select="$iterVarName"/>
								</xsl:call-template>
								<plx:assign>
									<plx:left>
										<plx:nameRef name="{$passCompositeIterator}"/>
									</plx:left>
									<plx:right>
										<plx:binaryOperator type="add">
											<plx:left>
												<plx:nameRef name="{$passCompositeIterator}"/>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="1"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:right>
								</plx:assign>
								<xsl:if test="$trackFirstPass">
									<plx:assign>
										<plx:left>
											<plx:nameRef name="{$trackFirstPassVarName}"/>
										</plx:left>
										<plx:right>
											<plx:falseKeyword/>
										</plx:right>
									</plx:assign>
								</xsl:if>
							</plx:body>
						</plx:branch>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="IterateRolesConstraintVerbalizationBody">
							<xsl:with-param name="TopLevel" select="$TopLevel"/>
							<xsl:with-param name="VariableDecorator" select="$VariableDecorator"/>
							<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
							<xsl:with-param name="CompositeCount" select="$CompositeCount"/>
							<xsl:with-param name="CompositeIterator" select="$CompositeIterator"/>
							<xsl:with-param name="ListStyle" select="$ListStyle"/>
							<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
							<xsl:with-param name="FirstPassVariable" select="$trackFirstPassVarName"/>
							<!-- Forwarded local parameters -->
							<xsl:with-param name="contextMatch" select="$contextMatch"/>
							<xsl:with-param name="iterVarName" select="$iterVarName"/>
						</xsl:call-template>
						<xsl:if test="$trackFirstPass">
							<plx:assign>
								<plx:left>
									<plx:nameRef name="{$trackFirstPassVarName}"/>
								</plx:left>
								<plx:right>
									<plx:falseKeyword/>
								</plx:right>
							</plx:assign>
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
			</plx:body>
		</plx:loop>
		<xsl:variable name="getListText">
			<plx:callInstance name="ToString">
				<plx:callObject>
					<plx:nameRef name="sbTemp"/>
				</plx:callObject>
			</plx:callInstance>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$TopLevel">
				<plx:callStatic name="WriteVerbalizerSentence" dataTypeName="FactType">
					<plx:passParam>
						<plx:nameRef type="parameter" name="writer"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:copy-of select="$getListText"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:call-template name="SnippetFor">
							<xsl:with-param name="SnippetType" select="'CloseVerbalizationSentence'"/>
						</xsl:call-template>
					</plx:passParam>
				</plx:callStatic>
			</xsl:when>
			<xsl:when test="0=string-length($CompositeIterator)">
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:left>
					<plx:right>
						<xsl:copy-of select="$getListText"/>
					</plx:right>
				</plx:assign>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<!-- A helper template to spit the body of an IterateRoles iteration
		 either inside a filter conditional or directly -->
	<xsl:template name="IterateRolesConstraintVerbalizationBody">
		<!-- Primary forward parameters -->
		<xsl:param name="TopLevel"/>
		<xsl:param name="VariableDecorator"/>
		<xsl:param name="VariablePrefix"/>
		<xsl:param name="CompositeCount"/>
		<xsl:param name="CompositeIterator"/>
		<xsl:param name="ListStyle"/>
		<xsl:param name="PatternGroup"/>
		<xsl:param name="FirstPassVariable"/>
		<!-- Forwarded local parameters -->
		<xsl:param name="contextMatch"/>
		<xsl:param name="iterVarName"/>
		<!-- Use the current snippets data to open the list -->
		<plx:local name="listSnippet" dataTypeName="{$VerbalizationTextSnippetType}"/>
		<xsl:choose>
			<xsl:when test="@pass='first' and 0=string-length($CompositeCount)">
				<xsl:call-template name="SetSnippetVariable">
					<xsl:with-param name="SnippetType" select="concat($ListStyle,'Open')"/>
					<xsl:with-param name="VariableName" select="'listSnippet'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<plx:branch>
					<plx:condition>
						<plx:binaryOperator type="equality">
							<plx:left>
								<plx:nameRef name="{$iterVarName}">
									<xsl:if test="string-length($CompositeIterator)">
										<xsl:attribute name="name">
											<xsl:value-of select="$CompositeIterator"/>
										</xsl:attribute>
									</xsl:if>
								</plx:nameRef>
							</plx:left>
							<plx:right>
								<plx:value type="i4" data="0"/>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:body>
						<xsl:call-template name="SetSnippetVariable">
							<xsl:with-param name="SnippetType" select="concat($ListStyle,'Open')"/>
							<xsl:with-param name="VariableName" select="'listSnippet'"/>
						</xsl:call-template>
					</plx:body>
					<!-- UNDONE: We could spit less code here if we pass the arity
						 in from the ConstrainedRoles tag. -->
					<plx:alternateBranch>
						<plx:condition>
							<plx:binaryOperator type="equality">
								<plx:left>
									<plx:nameRef name="{$iterVarName}">
										<xsl:if test="string-length($CompositeIterator)">
											<xsl:attribute name="name">
												<xsl:value-of select="$CompositeIterator"/>
											</xsl:attribute>
										</xsl:if>
									</plx:nameRef>
								</plx:left>
								<plx:right>
									<plx:binaryOperator type="subtract">
										<plx:left>
											<plx:nameRef>
												<xsl:attribute name="name">
													<xsl:choose>
														<xsl:when test="string-length($CompositeCount)">
															<xsl:value-of select="$CompositeCount"/>
														</xsl:when>
														<xsl:when test="$contextMatch='all'">
															<xsl:text>factArity</xsl:text>
														</xsl:when>
														<xsl:when test="$contextMatch='included'">
															<xsl:text>includedArity</xsl:text>
														</xsl:when>
														<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
															<xsl:text>constraintRoleArity</xsl:text>
														</xsl:when>
														<!-- UNDONE: Support excluded match -->
													</xsl:choose>
												</xsl:attribute>
											</plx:nameRef>
										</plx:left>
										<plx:right>
											<plx:value type="i4" data="1"/>
										</plx:right>
									</plx:binaryOperator>
								</plx:right>
							</plx:binaryOperator>
						</plx:condition>
						<plx:body>
							<plx:branch>
								<plx:condition>
									<plx:binaryOperator type="equality">
										<plx:left>
											<plx:nameRef name="{$iterVarName}">
												<xsl:if test="string-length($CompositeIterator)">
													<xsl:attribute name="name">
														<xsl:value-of select="$CompositeIterator"/>
													</xsl:attribute>
												</xsl:if>
											</plx:nameRef>
										</plx:left>
										<plx:right>
											<plx:value type="i4" data="1"/>
										</plx:right>
									</plx:binaryOperator>
								</plx:condition>
								<plx:body>
									<xsl:call-template name="SetSnippetVariable">
										<xsl:with-param name="SnippetType" select="concat($ListStyle,'PairSeparator')"/>
										<xsl:with-param name="VariableName" select="'listSnippet'"/>
									</xsl:call-template>
								</plx:body>
								<plx:fallbackBranch>
									<xsl:call-template name="SetSnippetVariable">
										<xsl:with-param name="SnippetType" select="concat($ListStyle,'FinalSeparator')"/>
										<xsl:with-param name="VariableName" select="'listSnippet'"/>
									</xsl:call-template>
								</plx:fallbackBranch>
							</plx:branch>
						</plx:body>
					</plx:alternateBranch>					
					<plx:fallbackBranch>
						<xsl:call-template name="SetSnippetVariable">
							<xsl:with-param name="SnippetType" select="concat($ListStyle,'Separator')"/>
							<xsl:with-param name="VariableName" select="'listSnippet'"/>
						</xsl:call-template>
					</plx:fallbackBranch>
				</plx:branch>
			</xsl:otherwise>
		</xsl:choose>
		<plx:callInstance name="Append">
			<plx:callObject>
				<plx:nameRef name="sbTemp"/>
			</plx:callObject>
			<plx:passParam>
				<xsl:call-template name="SnippetFor">
					<xsl:with-param name="VariableName" select="'listSnippet'"/>
				</xsl:call-template>
			</plx:passParam>
		</plx:callInstance>

		<!-- Process the child contents for this role -->
		<xsl:choose>
			<xsl:when test="count(child::*)">
				<xsl:for-each select="child::*">
					<!-- Let children assign directly to the normal replacement variable so
						 that we don't have to communicate down the stack that they should assign
						 directly to the temp string builder. -->
					<xsl:choose>
						<xsl:when test="$TopLevel">
							<plx:local name="{$VariablePrefix}{$VariableDecorator}" dataTypeName=".string">
								<plx:initialize>
									<plx:nullKeyword/>
								</plx:initialize>
							</plx:local>
						</xsl:when>
						<xsl:otherwise>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
								</plx:left>
								<plx:right>
									<plx:nullKeyword/>
								</plx:right>
							</plx:assign>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:apply-templates select="." mode="ConstraintVerbalization">
						<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
						<!-- Pass the position in here or it will always be 1 -->
						<xsl:with-param name="VariableDecorator" select="$VariableDecorator"/>
						<xsl:with-param name="IteratorContext" select="$contextMatch"/>
						<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
						<xsl:with-param name="FirstPassVariable" select="$FirstPassVariable"/>
					</xsl:apply-templates>
					<plx:callInstance name="Append">
						<plx:callObject>
							<plx:nameRef name="sbTemp"/>
						</plx:callObject>
						<plx:passParam>
							<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
						</plx:passParam>
					</plx:callInstance>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<plx:callInstance name="Append">
					<plx:callObject>
						<plx:nameRef name="sbTemp"/>
					</plx:callObject>
					<plx:passParam>
						<plx:callInstance name=".implied" type="arrayIndexer">
							<plx:callObject>
								<plx:nameRef name="basicRoleReplacements"/>
							</plx:callObject>
							<plx:passParam>
								<xsl:choose>
									<xsl:when test="@match='included' or @match='singleColumnConstraintRoles'">
										<!-- The role index needs to be retrieved from the all roles list -->
										<plx:callInstance name="IndexOf">
											<plx:callObject>
												<plx:nameRef name="factRoles"/>
											</plx:callObject>
											<plx:passParam>
												<plx:callInstance name=".implied" type="arrayIndexer">
													<plx:callObject>
														<plx:nameRef name="includedRoles">
															<xsl:if test="@match='singleColumnConstraintRoles'">
																<xsl:attribute name="name">
																	<xsl:text>allConstraintRoles</xsl:text>
																</xsl:attribute>
															</xsl:if>
														</plx:nameRef>
													</plx:callObject>
													<plx:passParam>
														<plx:nameRef name="{$iterVarName}"/>
													</plx:passParam>
												</plx:callInstance>
											</plx:passParam>
										</plx:callInstance>
									</xsl:when>
									<!-- UNDONE: Support excluded match -->
									<xsl:otherwise>
										<plx:nameRef name="{$iterVarName}"/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
						</plx:callInstance>
					</plx:passParam>
				</plx:callInstance>
			</xsl:otherwise>
		</xsl:choose>

		<!-- Use the current snippets data to close the list -->
		<xsl:choose>
			<xsl:when test="@pass='first' and 0=string-length($CompositeCount)">
				<plx:callInstance name="Append">
					<plx:callObject>
						<plx:nameRef name="sbTemp"/>
					</plx:callObject>
					<plx:passParam>
						<xsl:call-template name="SnippetFor">
							<xsl:with-param name="SnippetType" select="concat($ListStyle,'Close')"/>
						</xsl:call-template>
					</plx:passParam>
				</plx:callInstance>
			</xsl:when>
			<xsl:otherwise>
				<plx:branch>
					<plx:condition>
						<plx:binaryOperator type="equality">
							<plx:left>
								<plx:nameRef name="{$iterVarName}"/>
							</plx:left>
							<plx:right>
								<plx:binaryOperator type="subtract">
									<plx:left>
										<plx:nameRef>
											<xsl:attribute name="name">
												<xsl:choose>
													<xsl:when test="string-length($CompositeCount)">
														<xsl:value-of select="$CompositeCount"/>
													</xsl:when>
													<xsl:when test="$contextMatch='all'">
														<xsl:text>factArity</xsl:text>
													</xsl:when>
													<xsl:when test="$contextMatch='included'">
														<xsl:text>includedArity</xsl:text>
													</xsl:when>
													<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
														<xsl:text>constraintRoleArity</xsl:text>
													</xsl:when>
													<!-- UNDONE: Support excluded match -->
												</xsl:choose>
											</xsl:attribute>
										</plx:nameRef>
									</plx:left>
									<plx:right>
										<plx:value type="i4" data="1"/>
									</plx:right>
								</plx:binaryOperator>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:body>
						<plx:callInstance name="Append">
							<plx:callObject>
								<plx:nameRef name="sbTemp"/>
							</plx:callObject>
							<plx:passParam>
								<xsl:call-template name="SnippetFor">
									<xsl:with-param name="SnippetType" select="concat($ListStyle,'Close')"/>
								</xsl:call-template>
							</plx:passParam>
						</plx:callInstance>
					</plx:body>
				</plx:branch>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- A CompositeList tag is used to combine one or more IterateRoles lists into
		 a single list. The listStyle parameter is ignored on IterateRoles if this is set. -->
	<xsl:template match="ve:CompositeList" mode="ConstraintVerbalization">
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'list'"/>
		<xsl:param name="PatternGroup"/>
		<xsl:if test="$TopLevel">
			<xsl:choose>
				<xsl:when test="position()&gt;1">
					<plx:callInstance name="WriteLine">
						<plx:callObject>
							<plx:nameRef type="parameter" name="writer"/>
						</plx:callObject>
					</plx:callInstance>
				</xsl:when>
				<xsl:otherwise>
					<plx:callInstance name=".implied" type="delegateCall">
						<plx:callObject>
							<plx:nameRef type="parameter" name="beginVerbalization"/>
						</plx:callObject>
						<plx:passParam>
							<plx:callStatic name="Normal" dataTypeName="VerbalizationContent" type="field"/>
						</plx:passParam>
					</plx:callInstance>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
		<xsl:variable name="compositeCountVarName" select="concat($VariablePrefix,'CompositeCount',$VariableDecorator)"/>
		<plx:local name="{$compositeCountVarName}" dataTypeName=".i4">
			<plx:initialize>
				<plx:value type="i4" data="0"/>
			</plx:initialize>
		</plx:local>
		<xsl:variable name="iteratorVarName" select="concat($VariablePrefix,'CompositeIterator',$VariableDecorator)"/>
		<plx:local name="{$iteratorVarName}" dataTypeName=".i4"/>
		<xsl:apply-templates select="child::*" mode="CompositeOrFilteredListCount">
			<xsl:with-param name="TotalCountCollectorVariable" select="$compositeCountVarName"/>
			<xsl:with-param name="IteratorVariableName" select="$iteratorVarName"/>
			<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
		</xsl:apply-templates>
		<plx:assign>
			<plx:left>
				<plx:nameRef name="{$iteratorVarName}"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="0"/>
			</plx:right>
		</plx:assign>
		<xsl:variable name="ListStyle" select="@listStyle"/>
		<xsl:call-template name="EnsureTempStringBuilder"/>
		<xsl:for-each select="child::*">
			<plx:local name="{$VariablePrefix}{$VariableDecorator}Item{position()}" dataTypeName=".string">
				<plx:initialize>
					<plx:nullKeyword/>
				</plx:initialize>
			</plx:local>
			<xsl:apply-templates select="." mode="ConstraintVerbalization">
				<xsl:with-param name="VariablePrefix" select="concat($VariablePrefix,$VariableDecorator,'Item')"/>
				<xsl:with-param name="VariableDecorator" select="position()"/>
				<xsl:with-param name="CompositeCount" select="$compositeCountVarName"/>
				<xsl:with-param name="CompositeIterator" select="$iteratorVarName"/>
				<xsl:with-param name="ListStyle" select="$ListStyle"/>
				<xsl:with-param name="PatternGroup" select="$PatternGroup"/>
			</xsl:apply-templates>
		</xsl:for-each>
		<xsl:variable name="getListText">
			<plx:callInstance name="ToString">
				<plx:callObject>
					<plx:nameRef name="sbTemp"/>
				</plx:callObject>
			</plx:callInstance>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$TopLevel">
				<!-- Write the snippet directly to the text writer after sentence modification -->
				<plx:callStatic name="WriteVerbalizerSentence" dataTypeName="FactType">
					<plx:passParam>
						<plx:nameRef type="parameter" name="writer"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:copy-of select="$getListText"/>
					</plx:passParam>
					<plx:passParam>
						<xsl:call-template name="SnippetFor">
							<xsl:with-param name="SnippetType" select="'CloseVerbalizationSentence'"/>
						</xsl:call-template>
					</plx:passParam>
				</plx:callStatic>
			</xsl:when>
			<xsl:otherwise>
				<!-- Snippet is used as a replacement field in another snippet -->
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:left>
					<plx:right>
						<xsl:copy-of select="$getListText"/>
					</plx:right>
				</plx:assign>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Handle the minFactArity IterateRoles filter attribute -->
	<xsl:template match="@minFactArity" mode="IterateRolesFilterOperator">
		<plx:binaryOperator type="greaterThanOrEqual">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the maxFactArity IterateRoles filter attribute -->
	<xsl:template match="@maxFactArity" mode="IterateRolesFilterOperator">
		<plx:binaryOperator type="lessThanOrEqual">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Handle the factArity IterateRoles filter attribute -->
	<xsl:template match="@factArity" mode="IterateRolesFilterOperator">
		<plx:binaryOperator type="equality">
			<plx:left>
				<plx:nameRef name="factArity"/>
			</plx:left>
			<plx:right>
				<plx:value type="i4" data="{.}"/>
			</plx:right>
		</plx:binaryOperator>
	</xsl:template>
	<!-- Ignore the match attribute, it is not used as a filter -->
	<xsl:template match="@match|@listStyle|@pass" mode="IterateRolesFilterOperator"/>
	<!-- Terminate processing if we see an unrecognized operator -->
	<xsl:template match="@*" mode="IterateRolesFilterOperator">
		<xsl:call-template name="TerminateForInvalidAttribute">
			<xsl:with-param name="MessageText">Unrecognized role iterator filter attribute</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="ve:IterateRoles" mode="CompositeOrFilteredListCount">
		<xsl:param name="TotalCountCollectorVariable"/>
		<xsl:param name="IteratorVariableName"/>
		<xsl:param name="PatternGroup"/>
		<xsl:variable name="contextMatchFragment">
			<xsl:choose>
				<xsl:when test="string-length(@match)">
					<xsl:value-of select="@match"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>all</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="contextMatch" select="string($contextMatchFragment)"/>
		<xsl:variable name="filterTestFragment">
			<xsl:variable name="filterOperatorsFragment">
				<xsl:apply-templates select="@*" mode="IterateRolesFilterOperator"/>
			</xsl:variable>
			<xsl:for-each select="msxsl:node-set($filterOperatorsFragment)/child::*">
				<xsl:if test="position()=1">
					<xsl:call-template name="CombineElements">
						<xsl:with-param name="OperatorType" select="'booleanAnd'"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="filterTest" select="msxsl:node-set($filterTestFragment)/child::*"/>
		
		<!-- Get the count for the set in a value, which will be used either to
			 increment the full count or as a filter upper bound -->
		<xsl:variable name="setCountValueFragment">
			<xsl:choose>
				<xsl:when test="@pass='first'">
					<plx:value type="i4" data="1"/>
				</xsl:when>
				<xsl:otherwise>
					<plx:nameRef>
						<xsl:attribute name="name">
							<xsl:choose>
								<xsl:when test="$contextMatch='all'">
									<xsl:text>factArity</xsl:text>
								</xsl:when>
								<xsl:when test="$contextMatch='included'">
									<xsl:text>includedArity</xsl:text>
								</xsl:when>
								<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
									<xsl:text>constraintRoleArity</xsl:text>
								</xsl:when>
								<!-- UNDONE: Support excluded match -->
							</xsl:choose>
						</xsl:attribute>
					</plx:nameRef>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="setCountValue" select="msxsl:node-set($setCountValueFragment)/child::*"/>
		<xsl:choose>
			<xsl:when test="$filterTest">
				<plx:loop>
					<plx:initializeLoop>
						<plx:assign>
							<plx:left>
								<plx:nameRef name="{$IteratorVariableName}"/>
							</plx:left>
							<plx:right>
								<plx:value type="i4" data="0"/>
							</plx:right>
						</plx:assign>
					</plx:initializeLoop>
					<plx:condition>
						<plx:binaryOperator type="lessThan">
							<plx:left>
								<plx:nameRef name="{$IteratorVariableName}"/>
							</plx:left>
							<plx:right>
								<xsl:copy-of select="$setCountValue"/>
							</plx:right>
						</plx:binaryOperator>
					</plx:condition>
					<plx:beforeLoop>
						<plx:increment>
							<plx:nameRef name="{$IteratorVariableName}"/>
						</plx:increment>
					</plx:beforeLoop>
					<plx:body>
						<!-- UNDONE: We may not need this for all cases, it depends on the
							 filters in place. -->
						<plx:local name="primaryRole" dataTypeName="Role">
							<plx:initialize>
								<plx:callInstance name=".implied" type="arrayIndexer">
									<plx:callObject>
										<plx:nameRef>
											<xsl:attribute name="name">
												<xsl:choose>
													<xsl:when test="$contextMatch='all'">
														<xsl:text>factRoles</xsl:text>
													</xsl:when>
													<xsl:when test="$contextMatch='included'">
														<xsl:text>includedRoles</xsl:text>
													</xsl:when>
													<xsl:when test="$contextMatch='singleColumnConstraintRoles'">
														<xsl:text>allConstraintRoles</xsl:text>
													</xsl:when>
													<!-- UNDONE: Support excluded match -->
												</xsl:choose>
											</xsl:attribute>
										</plx:nameRef>
									</plx:callObject>
									<plx:passParam>
										<plx:nameRef name="{$IteratorVariableName}"/>
									</plx:passParam>
								</plx:callInstance>
							</plx:initialize>
						</plx:local>
						<xsl:if test="$contextMatch='singleColumnConstraintRoles'">
							<plx:assign>
								<plx:left>
									<plx:nameRef name="parentFact"/>
								</plx:left>
								<plx:right>
									<plx:callInstance name="FactType" type="property">
										<plx:callObject>
											<plx:nameRef name="primaryRole"/>
										</plx:callObject>
									</plx:callInstance>
								</plx:right>
							</plx:assign>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="factRoles"/>
								</plx:left>
								<plx:right>
									<plx:callInstance name="RoleCollection" type="property">
										<plx:callObject>
											<plx:nameRef name="parentFact"/>
										</plx:callObject>
									</plx:callInstance>
								</plx:right>
							</plx:assign>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="factArity"/>
								</plx:left>
								<plx:right>
									<plx:callInstance name="Count" type="property">
										<plx:callObject>
											<plx:nameRef name="factRoles"/>
										</plx:callObject>
									</plx:callInstance>
								</plx:right>
							</plx:assign>
							<plx:assign>
								<plx:left>
									<plx:nameRef name="allReadingOrders"/>
								</plx:left>
								<plx:right>
									<plx:callInstance name="ReadingOrderCollection" type="property">
										<plx:callObject>
											<plx:nameRef name="parentFact"/>
										</plx:callObject>
									</plx:callInstance>
								</plx:right>
							</plx:assign>
						</xsl:if>
						<plx:branch>
							<plx:condition>
								<xsl:copy-of select="$filterTest"/>
							</plx:condition>
							<plx:body>
								<plx:assign>
									<plx:left>
										<plx:nameRef name="{$TotalCountCollectorVariable}"/>
									</plx:left>
									<plx:right>
										<plx:binaryOperator type="add">
											<plx:left>
												<plx:nameRef name="{$TotalCountCollectorVariable}"/>
											</plx:left>
											<plx:right>
												<plx:value type="i4" data="1"/>
											</plx:right>
										</plx:binaryOperator>
									</plx:right>
								</plx:assign>
							</plx:body>
						</plx:branch>
					</plx:body>
				</plx:loop>
			</xsl:when>
			<xsl:otherwise>
				<!-- No filter is in place, just use the total count for the matching set -->
				<plx:assign>
					<plx:left>
						<plx:nameRef name="{$TotalCountCollectorVariable}"/>
					</plx:left>
					<plx:right>
						<plx:binaryOperator type="add">
							<plx:left>
								<plx:nameRef name="{$TotalCountCollectorVariable}"/>
							</plx:left>
							<plx:right>
								<xsl:copy-of select="$setCountValue"/>
							</plx:right>
						</plx:binaryOperator>
					</plx:right>
				</plx:assign>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Get the snippet value from the current snippets set.
		 This assumes snippets, isDeontic and isNegative local
		 variables are defined. Alternately, a VariableName
		 containing the name of a local variable containing the
		 text can be passed in instead of SnippetType. -->
	<xsl:template name="SnippetFor">
		<xsl:param name="SnippetType"/>
		<xsl:param name="VariableName"/>
		<plx:callInstance name="GetSnippet">
			<plx:callObject>
				<plx:nameRef name="snippets"/>
			</plx:callObject>
			<plx:passParam>
				<xsl:choose>
					<xsl:when test="string-length($VariableName)">
						<plx:nameRef name="{$VariableName}"/>
					</xsl:when>
					<xsl:otherwise>
						<plx:callStatic name="{$SnippetType}" dataTypeName="{$VerbalizationTextSnippetType}" type="field"/>
					</xsl:otherwise>
				</xsl:choose>
			</plx:passParam>
			<plx:passParam>
				<plx:nameRef name="isDeontic"/>
			</plx:passParam>
			<plx:passParam>
				<plx:nameRef name="isNegative"/>
			</plx:passParam>
		</plx:callInstance>
	</xsl:template>
	<!-- Assign the specified snippet type to a local variable. -->
	<xsl:template name="SetSnippetVariable">
		<xsl:param name="SnippetType"/>
		<xsl:param name="VariableName"/>
		<plx:assign>
			<plx:left>
				<plx:nameRef name="{$VariableName}"/>
			</plx:left>
			<plx:right>
				<plx:callStatic name="{$SnippetType}" dataTypeName="{$VerbalizationTextSnippetType}" type="field"/>
			</plx:right>
		</plx:assign>
	</xsl:template>
	<!-- Helper function to create an initialized string builder in the sbTemp local variable -->
	<xsl:template name="EnsureTempStringBuilder">
		<plx:branch>
			<plx:condition>
				<plx:binaryOperator type="identityEquality">
					<plx:left>
						<plx:nameRef name="sbTemp"/>
					</plx:left>
					<plx:right>
						<plx:nullKeyword/>
					</plx:right>
				</plx:binaryOperator>
			</plx:condition>
			<plx:body>
				<plx:assign>
					<plx:left>
						<plx:nameRef name="sbTemp"/>
					</plx:left>
					<plx:right>
						<plx:callNew dataTypeName="StringBuilder"/>
					</plx:right>
				</plx:assign>
			</plx:body>
			<plx:fallbackBranch>
				<plx:assign>
					<plx:left>
						<plx:callInstance name="Length" type="property">
							<plx:callObject>
								<plx:nameRef name="sbTemp"/>
							</plx:callObject>
						</plx:callInstance>
					</plx:left>
					<plx:right>
						<plx:value type="i4" data="0"/>
					</plx:right>
				</plx:assign>
			</plx:fallbackBranch>
		</plx:branch>
	</xsl:template>
	<xsl:template name="PopulateReadingOrder">
		<!-- Support readings for {Conditional, {Prefer|Require}[Non][Primary]LeadReading[NoForwardText], null} ReadingChoice values -->
		<xsl:param name="ReadingChoice"/>
		<xsl:param name="PatternGroup"/>
		<xsl:param name="ConditionalReadingOrderIndex"/>
		<xsl:choose>
			<xsl:when test="$ReadingChoice='Conditional' and $PatternGroup='SingleColumnExternalConstraint'">
				<plx:assign>
					<plx:left>
						<plx:nameRef name="readingOrder"/>
					</plx:left>
					<plx:right>
						<plx:callInstance name=".implied" type="arrayIndexer">
							<plx:callObject>
								<plx:nameRef name="allConstraintRoleReadingOrders"/>
							</plx:callObject>
							<plx:passParam>
								<plx:nameRef name="{$ConditionalReadingOrderIndex}"/>
							</plx:passParam>
						</plx:callInstance>
					</plx:right>
				</plx:assign>
			</xsl:when>
			<xsl:when test="not($ReadingChoice='Conditional')">
				<plx:assign>
					<plx:left>
						<plx:nameRef name="readingOrder"/>
					</plx:left>
					<plx:right>
						<plx:callStatic name="GetMatchingReadingOrder" dataTypeName="FactType">
							<plx:passParam>
								<!-- The readingOrders param-->
								<plx:nameRef name="allReadingOrders"/>
							</plx:passParam>
							<plx:passParam>
								<!-- The ignoreReadingOrder param -->
								<plx:nullKeyword/>
							</plx:passParam>
							<plx:passParam>
								<!-- The matchLeadRole param -->
								<xsl:choose>
									<xsl:when test="contains($ReadingChoice,'PrimaryLeadReading')">
										<plx:nameRef name="primaryRole"/>
									</xsl:when>
									<xsl:when test="contains($ReadingChoice, 'LeadReading')">
										<!-- LeadReading and PrimaryLeadReading should be treated the same for single column external constraints -->
										<xsl:choose>
											<xsl:when test="$PatternGroup='SingleColumnExternalConstraint'">
												<plx:nameRef name="primaryRole"/>
											</xsl:when>
											<xsl:otherwise>
												<plx:nullKeyword/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:when>
									<xsl:otherwise>
										<plx:callInstance name=".implied" type="arrayIndexer">
											<plx:callObject>
												<plx:nameRef name="factRoles"/>
											</plx:callObject>
											<plx:passParam>
												<plx:value type="i4" data="0"/>
											</plx:passParam>
										</plx:callInstance>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
							<plx:passParam>
								<!-- The matchAnyLeadRole param -->
								<xsl:choose>
									<xsl:when test="not($PatternGroup='SingleColumnExternalConstraint') and contains($ReadingChoice,'LeadReading') and not(contains($ReadingChoice,'PrimaryLeadReading'))">
										<plx:nameRef name="includedRoles"/>
									</xsl:when>
									<xsl:otherwise>
										<plx:nullKeyword/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
							<plx:passParam>
								<!-- The invertLeadRoles param -->
								<xsl:choose>
									<xsl:when test="contains($ReadingChoice,'Non')">
										<plx:trueKeyword/>
									</xsl:when>
									<xsl:otherwise>
										<plx:falseKeyword/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
							<plx:passParam>
								<!-- The noForwardText param -->
								<xsl:choose>
									<xsl:when test="contains($ReadingChoice,'NoForwardText')">
										<plx:trueKeyword/>
									</xsl:when>
									<xsl:otherwise>
										<plx:falseKeyword/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
							<plx:passParam>
								<!-- The defaultRoleOrder param -->
								<plx:nameRef name="factRoles"/>
							</plx:passParam>
							<plx:passParam>
								<!-- The allowAnyOrder param -->
								<xsl:choose>
									<xsl:when test="not(starts-with($ReadingChoice,'Require'))">
										<plx:trueKeyword/>
									</xsl:when>
									<xsl:otherwise>
										<plx:falseKeyword/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:passParam>
						</plx:callStatic>
					</plx:right>
				</plx:assign>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>