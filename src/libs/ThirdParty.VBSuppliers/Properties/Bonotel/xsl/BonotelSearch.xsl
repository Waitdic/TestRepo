<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" omit-xml-declaration="yes"/>

    <xsl:template match="/">

        <Results>
            <Properties>
				<xsl:for-each select="Results/availabilityResponse">
					<xsl:for-each select="hotelList/hotel">
						<Property>
						
							<!-- sum the total tax as we need to pass back in the book request as they are cheapskates -->
							<xsl:variable name="totalTax" select="sum(roomInformation/rateInformation/taxInformation/tax/taxAmount)"/>
						
							<MasterID><xsl:value-of select="hotelCode"/></MasterID>
						  <TPKey><xsl:value-of select="hotelCode"/></TPKey>
							<CurrencyCode><xsl:value-of select="rateCurrencyCode"/></CurrencyCode>

							<Rooms>
								<xsl:for-each select="roomInformation">
									<xsl:if test="confirmationType = 'CON'">
										<Room>
											<PropertyRoomBookingID><xsl:value-of select="roomNo"/></PropertyRoomBookingID>
											<RoomType><xsl:value-of select="roomType"/></RoomType>
											<MealBasisCode><xsl:value-of select="rateInformation/ratePlanCode"/></MealBasisCode>
											<Amount><xsl:value-of select="translate(rateInformation/totalRate,',','')"/></Amount>
											<TPReference><xsl:value-of select="concat(roomCode,'|',roomTypeCode,'|',bedTypeCode,'|',../rateCurrencyCode,'|',rateInformation/ratePlanCode,'|',$totalTax)"/></TPReference>
										</Room>
									</xsl:if>
								</xsl:for-each>
							</Rooms>
						</Property>


					</xsl:for-each>
				</xsl:for-each>
            </Properties>
        </Results>

    </xsl:template>





</xsl:stylesheet>
