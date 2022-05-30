<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" omit-xml-declaration="yes"/>

	<xsl:template match="/">

		<Results>
			<xsl:for-each select="Results/Properties/Property">

				<xsl:variable name="MasterID" select="MasterID"/>
				<xsl:variable name="TPKey" select="TPKey" />
				<xsl:variable name="CurrencyCode" select="CurrencyCode" />

				<xsl:for-each select="Rooms/Room">

					<xsl:variable name="Discount">
						<xsl:if test="Discount = ''">
							<xsl:text>0.00</xsl:text>
						</xsl:if>
						<xsl:if test="Discount != ''">
							<xsl:value-of select="Discount"/>
						</xsl:if>

					</xsl:variable>

					<xsl:variable name="NRF">
						<xsl:choose>
							<xsl:when test ="NonRefundable = 'true'">
								<xsl:value-of select ="'1'"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select ="'0'"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<Result MID="0" TPK="{$TPKey}" CC="{$CurrencyCode}" PRBID="{PropertyRoomBookingID}"
									  RT="{RoomType}" RTC="0" MBC="{MealBasisCode}" AD="0" CH="0"
									  hlpCHA="0" INF="0" AMT="{Amount}" TPR="{TPReference}" NRF="{$NRF}">

						<xsl:copy-of select="Adjustments" />

					</Result>
				</xsl:for-each>
			</xsl:for-each>
		</Results>

	</xsl:template>


</xsl:stylesheet>