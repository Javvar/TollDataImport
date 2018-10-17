﻿-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Returns Payment Entity Guid
-- =============================================
CREATE FUNCTION [dbo].[ufGetTransactionPaymentFieldTypeGuid](@Payment varchar(10) ,@PaymentFieldType int)
RETURNS uniqueidentifier
AS
BEGIN
	DECLARE @result uniqueidentifier

	IF(@PaymentFieldType = 1) -- payment method
	BEGIN
		SET @result = (CASE @Payment
						WHEN 'C' THEN '52A4D862-2CCD-DE11-8608-001517C991CF' -- cash
						WHEN 'T' THEN 'A2FE5770-2CCD-DE11-8608-001517C991CF' -- Post Paid
						WHEN 'R' THEN 'A2FE5770-2CCD-DE11-8608-001517C991CF' -- Post Paid
						WHEN 'D' THEN 'A3FE5770-2CCD-DE11-8608-001517C991CF' -- Pre Paid
						WHEN 'V' THEN '2283AB7C-2CCD-DE11-8608-001517C991CF' -- Violation
						WHEN 'P' THEN '224C37BE-2CCD-DE11-8608-001517C991CF' -- Exempt
						--WHEN 'P' THEN '64EE77B2-AB28-4EFF-A0CF-1315D63DD034' -- Local User
						ELSE NULL
					   END)
	END
	ELSE IF(@PaymentFieldType = 2) -- payment mechanism
	BEGIN
		SET @result = (CASE @Payment
						WHEN 'C' THEN '54D18E7F-EB7C-459D-8CC1-C82D9EF45D2D' -- cash
						WHEN 'R' THEN '03C37244-619D-4F0D-BE54-D2CF95B7D861' -- ISO Credit Card - U 
						WHEN 'T' THEN '294E1664-B2E6-4007-86D5-FEBEF82C6C00' -- etc
						WHEN 'P' THEN '0984A05D-7E42-4B0C-80C8-26879613A310' -- exempt
						WHEN 'V' THEN 'EBC8AAAC-69FD-4656-B629-2A6E1A9CAC07' -- violation
						WHEN 'D' THEN 'A1A44C02-74ED-493E-AB14-1B193C471F89' -- ISO Credit Card - R
						--WHEN 3 THEN '7BAFAB0E-6E84-4D91-9184-AC31975FD534' -- fleet card
						--WHEN 7 THEN 'F5B43676-7079-46FC-AB32-F88F02DE59B1' -- Local User
						ELSE NULL
					   END)
	END
	ELSE IF(@PaymentFieldType = 3) -- payment types
	BEGIN
		SET @result = (CASE @Payment
						WHEN 'C' THEN 'F5F20606-BAEE-4308-862C-BEF25DCE4F53' -- cash
						WHEN 'V' THEN 'F5F20606-BAEE-4308-862C-BEF25DCE4F53' -- cash
						WHEN 'T' THEN 'F5F20606-BAEE-4308-862C-BEF25DCE4F53' -- cash
						WHEN 'D' THEN 'D45993FB-18DE-41D0-982D-C8E571A9DA65' -- Bank Issued ISO Credit Card 
						WHEN 'R' THEN 'D45993FB-18DE-41D0-982D-C8E571A9DA65' -- fleet card
						WHEN 'P' THEN '003C18DE-2972-DF11-A01F-001517C991CF' -- Exempt
						--WHEN 5 THEN '4E84A808-9241-4875-9708-280B57EEBBF7' -- Local User
						ELSE NULL
					   END)
	END
	ELSE IF(@PaymentFieldType = 4) -- payment group
	BEGIN
		SET @result = 'EA3840EA-C8F7-4D97-82C4-7B8568ECFCDB'
		SET @result = (CASE @Payment
						WHEN 'C' THEN 'EA3840EA-C8F7-4D97-82C4-7B8568ECFCDB' -- cash
						WHEN 'D' THEN 'B12B582D-EC09-4DC6-AC89-D71571F043AE' -- Pre Paid
						WHEN 'T' THEN '5E0F9CA3-5746-45E8-AAF5-D8F8CD97452D' -- Post Paid
						WHEN 'R' THEN '5E0F9CA3-5746-45E8-AAF5-D8F8CD97452D' -- Post Paid
						WHEN 'V' THEN '2ECF4769-324B-4600-AD71-3C11515440E3' -- Run-through Violation
						WHEN 'P' THEN '46E52AAC-8535-46FA-B7C5-1FA32906F3B6' -- Exempt User
						--WHEN 5 THEN 'A8C642B8-743B-4408-9391-BD11040428DC' -- Local User
						ELSE NULL
					   END)
	END
	
	RETURN @result
END