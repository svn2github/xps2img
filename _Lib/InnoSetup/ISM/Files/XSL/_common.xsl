<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template name="CRLF"><xsl:text>&#13;&#10;</xsl:text></xsl:template>
<xsl:template match="copy"><xsl:text>&#169;</xsl:text></xsl:template>

<xsl:variable name="productName"> 
	<xsl:value-of select="info/product/name" />
</xsl:variable>
<xsl:template match="product">
	<xsl:value-of select="$productName" />
</xsl:template>

<xsl:variable name="version"> 
	<xsl:value-of select="info/product/version/major" />.<xsl:value-of select="info/product/version/minor" />.<xsl:value-of select="info/product/version/bgfix" />.<xsl:value-of select="info/product/version/build" />
</xsl:variable>
<xsl:template match="version">
	<xsl:value-of select="$version" />
</xsl:template>

<xsl:variable name="versionComma">
	<xsl:value-of select="translate($version, '.', ',')" />
</xsl:variable>
</xsl:stylesheet>
<!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios/><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no" ><SourceSchema srcSchemaPath="dummy.xml" srcSchemaRoot="dummy" AssociatedInstance="" loaderFunction="document" loaderFunctionUsesURI="no"/></MapperInfo><MapperBlockPosition><template match="/"></template><template name="NewTemplate0"></template><template match="text"></template><template match="strong"><block path="strong/xsl:apply&#x2D;templates" x="167" y="18"/></template></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->