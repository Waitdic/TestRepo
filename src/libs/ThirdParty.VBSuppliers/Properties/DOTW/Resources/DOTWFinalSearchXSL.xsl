<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" omit-xml-declaration="yes"/>

    <xsl:template match="/">

        <Results>
            <xsl:for-each select="Results/Properties/Property">

                <xsl:variable name="MasterID" select="MasterID"/>
                <xsl:variable name="TPKey" select="TPKey" />

                <xsl:for-each select="Rooms/Room">
                    <Result MID="{$MasterID}" TPK="{$TPKey}" CC="{CurrencyCode}" PRBID="{PropertyRoomBookingID}"
							RT="{RoomType}" RTC="{RoomTypeCode}" MBC="{MealBasisCode}"
							AMT="{Amount}" TPR="{TPReference}" DP="{DynamicProperty}" />
                </xsl:for-each>
            </xsl:for-each>
        </Results>

    </xsl:template>


</xsl:stylesheet>