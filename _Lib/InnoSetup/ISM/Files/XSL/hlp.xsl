<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:import href="_common.xsl" />
<xsl:output method="text" encoding="windows-1252" />
<xsl:template match="info">[OPTIONS]
REVISIONS=No
LCID=0x409 0x0 0x0 ; English (United States)
REPORT=Yes
TITLE=<xsl:apply-templates select="header"/>
COPYRIGHT=<xsl:apply-templates select="footer"/>
HLP=dummy.hlp

[FILES]
dummy.rtf
</xsl:template>
</xsl:stylesheet><!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="2HLP" userelativepaths="no" externalpreview="no" url="file:///f:/Games/Installs/_Lib/ISM/Files/dummy.xml" htmlbaseurl="" outputurl="" processortype="saxon8" useresolver="no" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator="MSXML4SAX"/></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no" ><SourceSchema srcSchemaPath="dummy.xml" srcSchemaRoot="dummy" AssociatedInstance="" loaderFunction="document" loaderFunctionUsesURI="no"/></MapperInfo><MapperBlockPosition><template match="/"></template><template name="NewTemplate0"></template><template match="text"></template><template match="strong"><block path="strong/xsl:apply&#x2D;templates" x="167" y="18"/></template></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->