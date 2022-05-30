<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version = "1.0" xmlns:xsl = "http://www.w3.org/1999/XSL/Transform" xmlns:msxsl = "urn:schemas-microsoft-com:xslt" exclude-result-prefixes = "msxsl">

    <xsl:output method = "xml" omit-xml-declaration = "yes"/>

    <xsl:template match = "/">

        <Results>

            <xsl:for-each select = "Results/Main/Hotel">

                <Result MID = "{HotelCode}"
                        PRBID = "1"
                        MBC = "{RoomBasis}"
                        TPK = "{HotelCode}"
                        CC = "{Currency}"
                        RT = "{RoomType}"
                        AMT = "{TotalPrice}"
                        TPR = "{concat(HotelSearchCode, '|', ../Types, '|',  Remark)}" />
            </xsl:for-each>
        </Results>
    </xsl:template>
</xsl:stylesheet>