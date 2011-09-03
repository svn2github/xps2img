<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:import href="_common.xsl" />
<xsl:output method="text"/>
<xsl:template match="info">{\rtf1\ansi\ansicpg1251\deff0\deflang1033{\fonttbl{\f0\fswiss\fprq2\fcharset0 <xsl:value-of select="style/fonts/font[1]" />;}{\f1\fnil\fcharset0 ;}}{\info{\title <xsl:apply-templates select="header"/>}{\subject <xsl:apply-templates select="footer/subject"/>}{\author <xsl:apply-templates select="footer/author"/>}}
\viewkind4\uc1\pard\lang1033\f0\fs22 <xsl:apply-templates select="text"/>\par
\par
\pard\sb100\sa100 <xsl:apply-templates select="footer"/>\par
\pard\f1\fs20
}</xsl:template>
<xsl:template match="strong">\b <xsl:apply-templates/> \b0</xsl:template>
<xsl:template match="copy">\'a9</xsl:template>
</xsl:stylesheet>
<!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="2RTF" userelativepaths="no" externalpreview="no" url="file:///f:/Games/Installs/_Lib/ISM/Files/dummy.xml" htmlbaseurl="" outputurl="" processortype="saxon8" useresolver="yes" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator="" ><advancedProp name="sInitialMode" value=""/><advancedProp name="bXsltOneIsOkay" value="true"/><advancedProp name="bSchemaAware" value="true"/><advancedProp name="bXml11" value="false"/><advancedProp name="iValidation" value="0"/><advancedProp name="bExtensions" value="true"/><advancedProp name="iWhitespace" value="0"/><advancedProp name="sInitialTemplate" value=""/><advancedProp name="bTinyTree" value="true"/><advancedProp name="bWarnings" value="true"/><advancedProp name="bUseDTD" value="false"/></scenario></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no" ><SourceSchema srcSchemaPath="dummy.xml" srcSchemaRoot="dummy" AssociatedInstance="" loaderFunction="document" loaderFunctionUsesURI="no"/></MapperInfo><MapperBlockPosition><template match="/"></template><template name="NewTemplate0"></template><template match="text"></template><template match="strong"><block path="strong/xsl:apply&#x2D;templates" x="167" y="18"/></template></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->