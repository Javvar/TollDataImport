-- =============================================
-- Author:		SJ
-- Create date: 25/10/2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspUpdateStagingAccountBalances]
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @NewAccounts TABLE(AccGuid uniqueidentifier)

	INSERT INTO @NewAccounts
	SELECT AccountGUID
	FROM [PCS].[dbo].Accounts 
	LEFT JOIN StagingMISAccountBalances ON PCSAccountGuid = AccountGUID 
	WHERE PCSAccountGuid IS NULL

	INSERT INTO StagingMISAccountBalances
	SELECT ac_nr,A.AccountGUID,Balance
	FROM [PCS].[dbo].Accounts A
	JOIN [PCS].[dbo].AccountUsers AU ON AU.AccountUserGUID = (SELECT TOP 1 AccountUserGUID
															   FROM [PCS].[dbo].AccountUsers AU1
															   WHERE A.AccountGUID = AU1.AccountGUID)
	JOIN StagingAccountIdentifiers ON FullIdentifier = AU.AccountUserIdentifier COLLATE DATABASE_DEFAULT
	JOIN @NewAccounts ON AccGuid = A.AccountGUID


	INSERT INTO StagingMISAccountBalanceUpdate
	SELECT MISAccountNr,A.AccountGUID,SAB.MISBalance,A.Balance,GETDATE(),NULL
	FROM [PCS].[dbo].Accounts A
	JOIN [PCS].[dbo].AccountUsers AU ON AccountUserGUID = (SELECT TOP 1 AccountUserGUID
														   FROM [PCS].[dbo].AccountUsers AU1
														   WHERE A.AccountGUID = AU1.AccountGUID)
	JOIN StagingMISAccountBalances SAB ON PCSAccountGuid = A.AccountGUID 
	WHERE Balance <> MISBalance
    
END