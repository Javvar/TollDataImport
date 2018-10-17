-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [uspGetTransactionBatchGreaterThanTime] @dateFrom datetime
AS
BEGIN
	select *
	from transactions
END
