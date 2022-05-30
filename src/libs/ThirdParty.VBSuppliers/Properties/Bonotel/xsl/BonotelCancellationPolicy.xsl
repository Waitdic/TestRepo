<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" omit-xml-declaration="yes"/>

	<xsl:template match="/">

		<Results>
			<Policies>
				<xsl:for-each select ="/availabilityResponse/hotelList/hotel/roomInformation/roomBookingPolicy">
					<Policy>
						<PolicyFrom>
							<xsl:value-of select="policyFrom"/>
						</PolicyFrom>
						
						<PolicyTo>
							<xsl:value-of select="policyTo"/>
						</PolicyTo>
						
						<AmendmentType>
							<xsl:value-of select="amendmentType"/>
						</AmendmentType>
						
						<PolicyBasedOn>
							<xsl:value-of select="policyBasedOn"/>
						</PolicyBasedOn>
						
						<PolicyBasedOnValue>
							<xsl:value-of select="policyBasedOnValue"/>
						</PolicyBasedOnValue>

						<CancellationType>
							<xsl:value-of select="cancellationType"/>
						</CancellationType>

						<StayDateRequirement>
							<xsl:value-of select="stayDateRequirement"/>
						</StayDateRequirement>

						<ArrivalRange>
							<xsl:value-of select="arrivalRange"/>
						</ArrivalRange>

						<ArrivalRangeValue>
							<xsl:value-of select="arrivalRangeValue"/>
						</ArrivalRangeValue>

						<PolicyFee>
							<xsl:value-of select="policyFee"/>
						</PolicyFee>

						<NoShowPolicy>
							
							<NoShowBasedOn>
								<xsl:value-of select="noShowBasedOn"/>
							</NoShowBasedOn>

							<NoShowBasedOnValue>
								<xsl:value-of select="noShowBasedOnValue"/>
							</NoShowBasedOnValue>

							<NoShowPolicyFee>
								<xsl:value-of select="noShowPolicyFee"/>
							</NoShowPolicyFee>
							
						</NoShowPolicy>
						
					</Policy>
					
				</xsl:for-each>
				
			</Policies>
			
		</Results>
		
	</xsl:template>
	
</xsl:stylesheet>