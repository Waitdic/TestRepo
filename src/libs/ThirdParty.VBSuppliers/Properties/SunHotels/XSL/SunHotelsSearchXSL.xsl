<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml"/>
    <xsl:param name="Currency" />
    <xsl:template match="/">

        <Results>

            <xsl:for-each select="/Results/Guests/Room">

                <xsl:variable name="Adults" select="Adults" />
                <xsl:variable name="Children" select="Children" />
                <xsl:variable name="Infants" select="Infants" />
                <xsl:variable name="hlpChildAgeCSV" select="hlpChildAgeCSV" />

                <xsl:for-each select="/Results/searchresult/hotels/hotel">

                    <xsl:variable name="MasterID" select="hotel.id"/>
                    <xsl:for-each select="roomtypes/roomtype">

                        <xsl:variable name="RoomTypeID" select="roomtype.ID"/>
                        <xsl:variable name="RoomTypeName" select="/Results/getRoomTypesResult/roomTypes/roomType[id=$RoomTypeID]/name"/>

                        <xsl:for-each select="rooms/room">
                            <xsl:variable name="RoomID" select="id"/>
                            <xsl:variable name="PaymentMethod" select="paymentMethods/paymentMethod/@id" />
                            <xsl:variable name="NonRef">
                                <xsl:choose>
                                    <xsl:when test="count(cancellation_policies/cancellation_policy[deadline/@nil='true' and percentage = 100]) > 0 ">
                                        <xsl:text>1</xsl:text>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <xsl:text>0</xsl:text>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:variable>
                            <xsl:for-each select="meals/meal">

                                <Result MID="{$MasterID}" TPK="{$MasterID}" CC="{$Currency}" PRBID="1"  RT="{$RoomTypeName}" MBC="{id}" AD="{$Adults}"
										CH="{$Children}" hlpCHA="{$hlpChildAgeCSV}" INF="{$Infants}" AMT="{prices/price}" TPR="{concat($RoomID, '_', id, '_',$RoomTypeID,'_',$PaymentMethod)}" NRF="{$NonRef}" />

                            </xsl:for-each>
                        </xsl:for-each>
                    </xsl:for-each>
                </xsl:for-each>
            </xsl:for-each>
        </Results>

    </xsl:template>
</xsl:stylesheet>