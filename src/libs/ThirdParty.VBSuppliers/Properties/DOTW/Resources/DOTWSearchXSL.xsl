<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version = "1.0" xmlns:xsl = "http://www.w3.org/1999/XSL/Transform">
    <xsl:output method = "xml" omit-xml-declaration = "yes"/>

    <xsl:param name ="ExcludeDOTWThirdParties" />
    <xsl:param name ="UseMinimumSellingPrice" />
    <xsl:param name="Duration" />

    <xsl:template match = "/">

        <Results>
            <Properties>
                <xsl:for-each select = "Results/result/hotels/hotel">

                    <Property>
                        <MasterID>
                            <xsl:value-of select = "@hotelid"/>
                        </MasterID>
                        <TPKey>
                            <xsl:value-of select = "@hotelid"/>
                        </TPKey>
                        <Rooms>
                            <xsl:for-each select = "rooms/room/roomType/rateBases/rateBasis">

                                <xsl:variable name="minStay" select="minStay" />
                                <!--Some DOTW Rooms have a minimum stay Length so I need to remove any that don't fulfil the criteria-->
                                <xsl:if test="$minStay = ''">

                                    <!--exclude DOTW's third parties if necessary. 1 is their own static stock, 2 is their own dynamic stock and 
									  3 is their third party dynamic stock-->
                                    <xsl:if test="rateType = '1' or rateType = '2' or $ExcludeDOTWThirdParties = 'False'">
                                        <xsl:variable name="PropertyRoomBookingID">
                                            <xsl:value-of select="((../../../@runno+1))"/>
                                        </xsl:variable>
                                            
                                        <xsl:variable name="DynamicProperty">
                                            
                                            <xsl:if test ="withinCancellationDeadline = 'yes'">1</xsl:if>

                                            <xsl:if test ="withinCancellationDeadline != 'yes'">0</xsl:if>

                                        </xsl:variable>
                                        <Room>
                                            <CurrencyCode>
                                                <xsl:value-of select="rateType/@currencyid"/>
                                            </CurrencyCode>
                                            <PropertyRoomBookingID>
                                                <xsl:value-of select = "$PropertyRoomBookingID"/>
                                            </PropertyRoomBookingID>
                                            <RoomType>
                                                <xsl:value-of select = "../../name"/>
                                            </RoomType>
                                            <MealBasisCode>
                                                <xsl:value-of select = "@id"/>
                                            </MealBasisCode>

                                            <DynamicProperty>
                                                <xsl:value-of select="$DynamicProperty"/>
                                            </DynamicProperty>
                                            <!--check whether we are going to take the minimum selling price which is used for B2C. It can be absent and in that case we want to just
						take the normal total-->
                                            <Amount>
                                                <xsl:choose>
                                                    <xsl:when test="$UseMinimumSellingPrice = 'True' and totalMinimumSelling != ''">
                                                        <xsl:value-of select="totalMinimumSelling/text()"/>
                                                    </xsl:when>
                                                    <xsl:otherwise>
                                                        <xsl:value-of select="total/text()"/>
                                                    </xsl:otherwise>
                                                </xsl:choose>
                                            </Amount>
                                            <Adults>
                                                <xsl:value-of select="/Results/PropertyBooking/PropertyRoomBookings/PropertyRoomBooking[PropertyRoomBookingID=$PropertyRoomBookingID]/Adults"/>
                                            </Adults>
                                            <Children>
                                                <xsl:value-of select="/Results/PropertyBooking/PropertyRoomBookings/PropertyRoomBooking[PropertyRoomBookingID=$PropertyRoomBookingID]/Children"/>
                                            </Children>
                                            <hlpChildAgeCSV>
                                                <xsl:value-of select="/Results/PropertyBooking/PropertyRoomBookings/PropertyRoomBooking[PropertyRoomBookingID=$PropertyRoomBookingID]/hlpChildAgeCSV"/>
                                            </hlpChildAgeCSV>
                                            <Infants>
                                                <xsl:value-of select="/Results/PropertyBooking/PropertyRoomBookings/PropertyRoomBooking[PropertyRoomBookingID=$PropertyRoomBookingID]/Infants"/>
                                            </Infants>
                                            <TPReference>
                                                <xsl:value-of select="concat(../../@roomtypecode,'|',@id)" />
                                            </TPReference>
                                        </Room>
                                    
                                </xsl:if>
                                </xsl:if>
                            </xsl:for-each>
                        </Rooms>
                    </Property>
                </xsl:for-each>
            </Properties>
        </Results>
    </xsl:template>
</xsl:stylesheet>