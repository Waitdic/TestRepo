<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:template match="/">




    <Results>
      <Properties>
        <xsl:for-each select="Results/Results/HtSearchRq/session/Hotel">
          <Property>
            <TPKey>
              <xsl:value-of select="@ID"/>
            </TPKey>
            <CurrencyCode>
              <xsl:value-of select="../Currency"/>
            </CurrencyCode>
            <Rooms>
              <xsl:for-each select="Room_1/Room">

                <xsl:variable name="RoomType1">
                  <!--<xsl:if test="contains(Type, '(')">
                    <xsl:value-of select="concat(substring-before(Type, '('),substring-after(Type, ')'))"/>
                  </xsl:if>
                  <xsl:if test="not(contains(Type, '('))">
                    <xsl:value-of select="Type"/>
                  </xsl:if>-->
					
					<!--The above was removed as it was unknown why it was there, if this needs to be re-added, comment why-->

					<xsl:value-of select="Type"/>

				</xsl:variable>
				  <xsl:variable name="NonRefundable">
					  <xsl:choose>
						  <xsl:when test="@Refundable='false'">
							  <xsl:value-of select="'true'"/>
						  </xsl:when>
						  <xsl:otherwise>
							  <xsl:value-of select="'false'"/>
						  </xsl:otherwise>
					  </xsl:choose>
				  </xsl:variable>


				  <Room>
                  <PropertyRoomBookingID>1</PropertyRoomBookingID>
                  <RoomType>
                    <xsl:value-of select="$RoomType1"/>
                  </RoomType>
                  <MealBasisCode>
                    <xsl:value-of select="Board"/>
                  </MealBasisCode>
                  <Adults>
                    <xsl:value-of select="../Passengers/@Adults"/>
                  </Adults>
                  <Children>
                    <xsl:value-of select="../Passengers/@Children"/>
                  </Children>
                  <Infants>
                    <xsl:value-of select="../Passengers/@Infants"/>
                  </Infants>
                  <hlpChildAgeCSV>
                    <xsl:value-of select="/Results/ChildAges/Room_1/hlpChildAgeCSV"/>
                  </hlpChildAgeCSV>
                  <Amount>
                    <xsl:value-of select="Rates/@Final_Rate"/>
                  </Amount>
                  <TPReference>
                    <xsl:value-of select="../../../@id"/>|<xsl:value-of select="@Id"/>|<xsl:value-of select="CanxPolicy/@token"/>
                  </TPReference>
				  <NonRefundable>
					<xsl:value-of select="$NonRefundable"/>
				</NonRefundable>
                </Room>
              </xsl:for-each>
              <xsl:for-each select="Room_2/Room">

                <xsl:variable name="RoomType2">
                  <!--<xsl:if test="contains(Type, '(')">
                    <xsl:value-of select="concat(substring-before(Type, '('),substring-after(Type, ')'))"/>
                  </xsl:if>
                  <xsl:if test="not(contains(Type, '('))">
                    <xsl:value-of select="Type"/>
                  </xsl:if>-->
					
					<!--The above was removed as it was unknown why it was there, if this needs to be re-added, comment why-->

					<xsl:value-of select="Type"/>
                </xsl:variable>
                 <xsl:variable name="NonRefundable">
					  <xsl:choose>
						  <xsl:when test="@Refundable='false'">
							  <xsl:value-of select="'true'"/>
						  </xsl:when>
						  <xsl:otherwise>
							  <xsl:value-of select="'false'"/>
						  </xsl:otherwise>
					  </xsl:choose>
				  </xsl:variable>
                <Room>
                  <PropertyRoomBookingID>2</PropertyRoomBookingID>
                  <RoomType>
                    <xsl:value-of select="$RoomType2"/>
                  </RoomType>
                  <MealBasisCode>
                    <xsl:value-of select="Board"/>
                  </MealBasisCode>
                  <Adults>
                    <xsl:value-of select="../Passengers/@Adults"/>
                  </Adults>
                  <Children>
                    <xsl:value-of select="../Passengers/@Children"/>
                  </Children>
                  <Infants>
                    <xsl:value-of select="../Passengers/@Infants"/>
                  </Infants>
                  <hlpChildAgeCSV>
                    <xsl:value-of select="/Results/ChildAges/Room_2/hlpChildAgeCSV"/>
                  </hlpChildAgeCSV>
                  <Amount>
                    <xsl:value-of select="Rates/@Final_Rate"/>
                  </Amount>
                  <TPReference>
                    <xsl:value-of select="../../../@id"/>|<xsl:value-of select="@Id"/>|<xsl:value-of select="CanxPolicy/@token"/>
                  </TPReference>
				<NonRefundable>
					<xsl:value-of select="$NonRefundable"/>
				</NonRefundable>
                </Room>
              </xsl:for-each>
              <xsl:for-each select="Room_3/Room">

                <xsl:variable name="RoomType3">
                  <!--<xsl:if test="contains(Type, '(')">
                    <xsl:value-of select="concat(substring-before(Type, '('),substring-after(Type, ')'))"/>
                  </xsl:if>
                  <xsl:if test="not(contains(Type, '('))">
                    <xsl:value-of select="Type"/>
                  </xsl:if>-->

					<!--The above was removed as it was unknown why it was there, if this needs to be re-added, comment why-->

					<xsl:value-of select="Type"/>
                </xsl:variable>
                 <xsl:variable name="NonRefundable">
					  <xsl:choose>
						  <xsl:when test="@Refundable='false'">
							  <xsl:value-of select="'true'"/>
						  </xsl:when>
						  <xsl:otherwise>
							  <xsl:value-of select="'false'"/>
						  </xsl:otherwise>
					  </xsl:choose>
				  </xsl:variable>
                <Room>
                  <PropertyRoomBookingID>3</PropertyRoomBookingID>
                  <RoomType>
                    <xsl:value-of select="$RoomType3"/>
                  </RoomType>
                  <MealBasisCode>
                    <xsl:value-of select="Board"/>
                  </MealBasisCode>
                  <Adults>
                    <xsl:value-of select="../Passengers/@Adults"/>
                  </Adults>
                  <Children>
                    <xsl:value-of select="../Passengers/@Children"/>
                  </Children>
                  <Infants>
                    <xsl:value-of select="../Passengers/@Infants"/>
                  </Infants>
                  <hlpChildAgeCSV>
                    <xsl:value-of select="/Results/ChildAges/Room_3/hlpChildAgeCSV"/>
                  </hlpChildAgeCSV>
                  <Amount>
                    <xsl:value-of select="Rates/@Final_Rate"/>
                  </Amount>
                  <TPReference>
                    <xsl:value-of select="../../../@id"/>|<xsl:value-of select="@Id"/>|<xsl:value-of select="CanxPolicy/@token"/>
                  </TPReference>
				<NonRefundable>
						<xsl:value-of select="$NonRefundable"/>
					</NonRefundable>
                </Room>
              </xsl:for-each>
            </Rooms>
          </Property>
        </xsl:for-each>
      </Properties>
    </Results>


  </xsl:template>


</xsl:stylesheet>