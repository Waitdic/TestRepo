<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" omit-xml-declaration="yes" />

    <xsl:template match="/">

        <Results>
            <Properties>

                <xsl:for-each select="Envelope/Body/availableHotelsByMultiQueryV12Response/result/availableHotels">
                    <Property>

                        <xsl:variable name="MasterID" select="establishment/id" />

                        <MasterID>
                            <xsl:value-of select="$MasterID" />
                        </MasterID>
                        <TPKey>
                            <xsl:value-of select="$MasterID" />
                        </TPKey>
                        <CurrencyCode>
                            <xsl:value-of select="roomCombinations/prices/amount/currencyCode" />
                        </CurrencyCode>

                        <Rooms>

                            <xsl:for-each select="roomCombinations/rooms">

                                <xsl:variable name="typeCode" select="typeCode" />
                                <xsl:variable name="paxes" select="adults + children" />
                                <xsl:variable name="Room" select="." />

                                <xsl:for-each select="../prices">

                                    <xsl:variable name="price">
                                        <xsl:for-each select="roomPrices[typeCode=$typeCode and paxes=$paxes]">
                                            <xsl:if test ="position() = 1">
                                                <xsl:value-of select="price" />
                                            </xsl:if>
                                        </xsl:for-each>
                                    </xsl:variable>

                                    <xsl:variable name="NonRefundable">
                                        <xsl:choose>
                                            <xsl:when test="roomPrices/comments[type = 'Cancellation term' and (text = '999 - 100.00%' or text = '365 - 100.00%')] != ''">
                                                <xsl:text>true</xsl:text>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>false</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </xsl:variable>

                                    <Room>
                                        <PropertyRoomBookingID>
                                            <xsl:text>1</xsl:text>
                                        </PropertyRoomBookingID>
                                        <RoomType>
                                            <xsl:value-of select="$Room/typeName" />
                                        </RoomType>
                                        <MealBasisCode>
                                            <xsl:value-of select="boardTypeCode" />
                                        </MealBasisCode>
                                        <Adults>
                                            <xsl:value-of select="$Room/adults" />
                                        </Adults>
                                        <Children>
                                            <xsl:value-of select="count($Room/childrenAges[number(.) > 0])" />
                                        </Children>
                                        <hlpChildAgeCSV>
                                            <xsl:for-each select="$Room/childrenAges">
                                                <xsl:variable name="pos" select="position()" />
                                                <xsl:value-of select="." />
                                                <xsl:if test="count($Room/childrenAges[position() &gt; $pos])!=0">
                                                    <xsl:text>,</xsl:text>
                                                </xsl:if>
                                            </xsl:for-each>
                                        </hlpChildAgeCSV>
                                        <Infants>
                                            <xsl:value-of select="count($Room/childrenAges[number(.) = 0])" />
                                        </Infants>
                                        <Amount>
                                            <xsl:value-of select="$price" />
                                        </Amount>
                                        <TPReference>
                                            <xsl:value-of select ="$Room/typeCode" />
                                            <xsl:text>|</xsl:text>
                                            <xsl:value-of select ="boardTypeCode" />
                                        </TPReference>

                                        <NonRefundable>
                                            <xsl:value-of select="$NonRefundable"/>
                                        </NonRefundable>

                                    </Room>
                                </xsl:for-each>
                            </xsl:for-each>
                        </Rooms>
                    </Property>
                </xsl:for-each>
            </Properties>
        </Results>
    </xsl:template>
</xsl:stylesheet>