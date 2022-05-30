<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!--xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">-->
	<xsl:output method="xml"/>

	<xsl:param name="NumberOfRooms"/>

	<xsl:template match="/">

		<Results>

			<xsl:for-each select="/Results/OTA_HotelAvailRS/HotelStays/HotelStay">
			
				<xsl:variable name="MasterID" select="concat(BasicPropertyInfo/@HotelCode, '|', BasicPropertyInfo/@AreaID)" />

				<xsl:for-each select="/Results/OTA_HotelAvailRS/RoomStays/RoomStay[BasicPropertyInfo/@HotelCode = substring-before($MasterID, '|')]">

					<xsl:variable name="PRBID" select="@RoomStayCandidateRPH"/>

					<xsl:variable name="CountryCode" select ="substring-after($MasterID, '|')"/>
					<xsl:variable name="Country" select="/Results/OTA_HotelAvailRS/Areas/Area[@AreaID=$CountryCode]/AreaDescription[@Name='Country']/Text"/>

					<xsl:variable name="Currency" select ="RoomRates/RoomRate/Rates/Rate/Total/@CurrencyCode"/>
					<xsl:variable name="Price" select ="RoomRates/RoomRate/Rates/Rate/Total/@AmountAfterTax"/>
					<xsl:variable name="MealBasis" select="RoomRates/RoomRate/Features/Feature/Description/Text"/>
					<xsl:variable name="RoomType" select="RoomTypes/RoomType/RoomDescription/Text"/>
					<xsl:variable name="TPR" select="concat(RoomTypes/RoomType/@RoomTypeCode, '|',  RoomRates/RoomRate/Features/Feature/Description/Text, '|', $Country)" />
					<xsl:variable name="NumberOfUnits" select="RoomRates/RoomRate/@NumberOfUnits"/>
					<xsl:variable name="RoomTypeCode" select="RoomRates/RoomRate/@RoomTypeCode"/>
	
					<xsl:variable name="NRF">
						<xsl:choose>
							<xsl:when test ="substring(RoomTypes/RoomType/@RoomTypeCode, 10)='N'">
								<xsl:value-of select ="'1'"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select ="'0'"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>		

					<xsl:if test="not($NumberOfRooms > $NumberOfUnits) and $Price != ''">

						<Result MID="0" TPK="{substring-before($MasterID, '|')}" CC="{$Currency}" PRBID="{$PRBID}" NRF="{$NRF}" RT="{$RoomType}" MBC="{$MealBasis}" AD="0"
						CH="0" hlpCHA="" INF="0" AMT="{$Price}" TPR="{$TPR}" RTC="{$RoomTypeCode}" />

					</xsl:if>

				</xsl:for-each>
			</xsl:for-each>

		</Results>

	</xsl:template>
</xsl:stylesheet>