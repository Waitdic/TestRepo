CREATE PROCEDURE [dbo].[Currency_List]
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

select CurrencyID,
		Source,
		ThirdPartyCurrencyCode,
		ISOCurrency.CurrencyCode,
		ISOCurrency.ISOCurrencyID,
		isnull(ExchangeRates.ExchangeRate, 1.0) ExchangeRate
	from Currency
		inner join ISOCurrency
			on Currency.CurrencyCode = ISOCurrency.CurrencyCode
		left join ExchangeRates
			on ExchangeRates.ISOCurrencyID = ISOCurrency.ISOCurrencyID