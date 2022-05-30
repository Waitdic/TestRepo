<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version = "1.0" xmlns:xsl = "http://www.w3.org/1999/XSL/Transform" xmlns:msxsl = "urn:schemas-microsoft-com:xslt" exclude-result-prefixes = "msxsl">
  <xsl:output method = "xml"/>

  <xsl:param name="RoomCount" />

  <xsl:template match = "/">
    <Results>

      <xsl:variable name="currCode" select="/Results/AvailabilitySearchResult/Currency"/>

      <xsl:for-each select = "/Results/AvailabilitySearchResult/HotelAvailability/Result">

        <xsl:for-each select="Room"><!--Split out to second loop because otherwise the position code doesnt work when there is more than one resort in the search-->          
          <xsl:variable name="prbid">
            <xsl:choose>
              <xsl:when test="$RoomCount = '1'">
                <xsl:value-of select="1"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="position()"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>

          <xsl:variable name="nrf">
            <xsl:choose>
              <xsl:when test="CancellationPolicyStatus = 'NonRefundable'">
                <xsl:value-of select="1"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="0"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>

          <Result
              MID="{../../Hotel/@id}"
              PRBID="{$prbid}"
              MBC="{MealType/@code}"
              TPK="{../../Hotel/@id}"
              CC="{$currCode}"
              RT="{RoomType/@text}"
              AMT="{Price/@amt}"
              TPR="{concat(../../@hotelQuoteId, '|', ../@id)}"
              RTC="{RoomType/@code}"
              NRF="{$nrf}"/>
          <!--TPR = "{concat(../QuoteId, '|', RoomType/Code)}" />-->
        </xsl:for-each>

      </xsl:for-each>
    </Results>

  </xsl:template>
</xsl:stylesheet>