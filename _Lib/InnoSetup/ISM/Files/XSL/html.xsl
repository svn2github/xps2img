<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  	xmlns:xhtml="http://www.w3.org/1999/xhtml">
<xsl:import href="_common.xsl" />
<xsl:output
  	method="xml"
	indent="yes"
	doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>
<!--
<xsl:namespace-alias stylesheet-prefix="#default" result-prefix="xhtml"/>
-->
<xsl:template match="info">
	<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
	<head>
		<title><xsl:apply-templates select="header"/></title>
		<style>
		<xsl:text disable-output-escaping="yes">&lt;!--</xsl:text>
		body
		{
			font-family: <xsl:apply-templates select="style/fonts"/>;
			font-size: <xsl:value-of select="style/fonts/@size"/>pt;
		}
		<xsl:text disable-output-escaping="yes">--&gt;</xsl:text>
		</style>
	</head>
	<body>
		<xsl:apply-templates select="text"/>
		<hr/>
		<xsl:apply-templates select="footer"/>
	</body>
	</html>
</xsl:template>
<xsl:template match="style/fonts">
	<xsl:value-of select="string-join(*, ', ')" />
</xsl:template>
<xsl:template match="strong">
	<xsl:element name="strong" namespace="http://www.w3.org/1999/xhtml">
		<xsl:apply-templates/>
	</xsl:element>
</xsl:template>
<xsl:template match="copy"><xsl:text disable-output-escaping="yes">&amp;</xsl:text>copy;</xsl:template>
</xsl:stylesheet><!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="2HTML" userelativepaths="no" externalpreview="no" url="file:///f:/Games/Installs/_Lib/ISM/Files/dummy.xml" htmlbaseurl="" outputurl="" processortype="saxon8" useresolver="no" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator="MSXML4SAX"/></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no" ><SourceSchema srcSchemaPath="dummy.xml" srcSchemaRoot="dummy" AssociatedInstance="" loaderFunction="document" loaderFunctionUsesURI="no"/></MapperInfo><MapperBlockPosition><template match="/"></template><template name="NewTemplate0"></template><template match="text"></template><template match="strong"><block path="strong/xsl:apply&#x2D;templates" x="167" y="18"/></template></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->