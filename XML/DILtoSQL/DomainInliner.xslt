﻿<?xml version="1.0" encoding="utf-8"?>
<!--
	Copyright © Neumont University. All rights reserved.
	Copyright © The ORM Foundation. All rights reserved.

	This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.
	Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:
	1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
	2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
	3. This notice may not be removed or altered from any source distribution.
-->
<!-- Contributors: Kevin M. Owen, Corey Kaylor -->
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:exsl="http://exslt.org/common"
	xmlns:dil="http://schemas.orm.net/DIL/DIL"
	xmlns:ddt="http://schemas.orm.net/DIL/DILDT"
	xmlns:dep="http://schemas.orm.net/DIL/DILEP"
	xmlns:dms="http://schemas.orm.net/DIL/DILMS"
	xmlns:dml="http://schemas.orm.net/DIL/DMIL"
	xmlns:ddl="http://schemas.orm.net/DIL/DDIL"
	extension-element-prefixes="exsl">

	<xsl:output method="xml" encoding="utf-8" media-type="text/xml" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<!-- ddt:domain can occur only inside of ddl:columnDefinition and dep:castSpecification -->

	<xsl:template match="*" mode="DomainInliner">
		<xsl:param name="domainDefinitions" select="//ddl:domainDefinition"/>
		<xsl:param name="columnName"/>
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates mode="DomainInliner">
				<xsl:with-param name="domainDefinitions" select="$domainDefinitions"/>
				<xsl:with-param name="columnName" select="$columnName"/>
			</xsl:apply-templates>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="ddl:domainDefinition" mode="DomainInliner"/>

	<xsl:template match="dep:castSpecification[ddt:domain]" mode="DomainInliner">
		<xsl:param name="domainDefinitions" select="//ddl:domainDefinition"/>
		<xsl:param name="columnName"/>

		<xsl:variable name="domain" select="ddt:domain"/>
		<xsl:variable name="domainDefinition" select="$domainDefinitions[@name=$domain/@name and (string-length(@schema)=0 or @schema=$domain/@schema or string-length($domain/@schema)=0) and (string-length(@catalog)=0 or @catalog=$domain/@catalog or string-length($domain/@catalog)=0)]"/>
		<xsl:if test="not(count($domainDefinition)=1)">
			<xsl:message terminate="yes">
				<xsl:text>Too many or too few ddl:domainDefinition elements were found for a ddt:domain.</xsl:text>
			</xsl:message>
		</xsl:if>

		<!-- UNDONE: It would be nice to do something more than just replacing the DOMAIN reference with the underlying type... --> 
		
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*[position() != last()]" mode="DomainInliner">
				<xsl:with-param name="domainDefinitions" select="$domainDefinitions"/>
				<xsl:with-param name="columnName" select="$columnName"/>
			</xsl:apply-templates>
			<xsl:copy-of select="$domainDefinition/ddt:*[1]"/>
		</xsl:copy>
		
	</xsl:template>

	<xsl:template match="ddl:columnDefinition[ddt:domain]" mode="DomainInliner">
		<xsl:param name="domainDefinitions" select="//ddl:domainDefinition"/>

		<xsl:variable name="columnName" select="@name"/>
		<xsl:variable name="domain" select="ddt:domain"/>
		<xsl:variable name="domainDefinition" select="$domainDefinitions[@name=$domain/@name and (string-length(@schema)=0 or @schema=$domain/@schema or string-length($domain/@schema)=0) and (string-length(@catalog)=0 or @catalog=$domain/@catalog or string-length($domain/@catalog)=0)]"/>
		<xsl:if test="not(count($domainDefinition)=1)">
			<xsl:message terminate="yes">
				<xsl:text>Too many or too few ddl:domainDefinition elements were found for a ddt:domain.</xsl:text>
			</xsl:message>
		</xsl:if>
		
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:copy-of select="$domainDefinition/ddt:*[1]"/>
			<xsl:copy-of select="$domainDefinition/ddl:defaultClause"/>
			<xsl:for-each select="$domainDefinition/ddl:domainConstraintDefinition">
				<ddl:columnConstraintDefinition>
					<xsl:copy-of select="@*[not(name()='name')]"/>
					<!-- The constraint names need to be unique, but more than one COLUMN in the same TABLE can share the same DOMAIN. -->
					<!-- That means constraint names would overlap once inlined, so we don't copy the name over. -->
					<xsl:apply-templates mode="DomainInliner">
						<xsl:with-param name="domainDefinitions" select="$domainDefinitions"/>
						<xsl:with-param name="columnName" select="$columnName"/>
					</xsl:apply-templates>
				</ddl:columnConstraintDefinition>
			</xsl:for-each>
			<xsl:copy-of select="ddl:columnConstraintDefinition"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="dep:valueKeyword" mode="DomainInliner">
		<xsl:param name="columnName"/>
		<dep:columnReference name="{$columnName}"/>
	</xsl:template>


</xsl:stylesheet>
