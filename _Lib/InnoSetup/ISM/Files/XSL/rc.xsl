<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:import href="_common.xsl" />
<xsl:output method="text" encoding="windows-1252" />
<xsl:template match="info">// Version.h

#pragma once

#define VER_VALUE_VERSION	"<xsl:value-of select="$version"/>"
#define VER_HEADER_VERSION	 <xsl:value-of select="$versionComma"/>

#define LEGAL_COPYRIGHT		"<xsl:apply-templates select="footer/author" />"
#define PRODUCT_NAME		"<xsl:value-of select="$productName" />"
#define COMMENTS			"<xsl:apply-templates select="footer/subject" />"

#ifdef EMPTY
	#define FILE_DESCRIPTION	"<xsl:value-of select="$productName"/> GUI Empty Dummy Application."
#elif defined(_WINDOWS)
	#define FILE_DESCRIPTION	"<xsl:value-of select="$productName"/> GUI Dummy Application."
#elif defined(_CONSOLE)
	#define FILE_DESCRIPTION	"<xsl:value-of select="$productName"/> Console Dummy Application."
#elif defined(_WINDLL)
	#define FILE_DESCRIPTION	"<xsl:value-of select="$productName"/> Dummy DLL."
#endif

#define INTERNAL_NAME			FILE_NAME
#define ORIGINAL_FILENAME		FILE_NAME

#define DUMMY_TEXT				"<xsl:apply-templates select="text"/>\n\n<xsl:apply-templates select="footer"/>"

// Version.h
</xsl:template>

</xsl:stylesheet><!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="2RC" userelativepaths="no" externalpreview="no" url="file:///f:/Games/Installs/_Lib/ISM/Files/dummy.xml" htmlbaseurl="" outputurl="" processortype="saxon8" useresolver="no" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator="MSXML4SAX"/></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no" ><SourceSchema srcSchemaPath="dummy.xml" srcSchemaRoot="dummy" AssociatedInstance="" loaderFunction="document" loaderFunctionUsesURI="no"/></MapperInfo><MapperBlockPosition><template match="/"></template><template name="NewTemplate0"></template><template match="text"></template><template match="strong"><block path="strong/xsl:apply&#x2D;templates" x="167" y="18"/></template></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->