CREATE PROCEDURE [dbo].[uspGetAccountDetails] 
@AccountUserIdentifier VARCHAR(50),@AccountUserGUID uniqueidentifier output,@AccountGUID uniqueidentifier output
AS
BEGIN	
	--SELECT @AccountUserGUID = AccountUserGUID,@AccountGUID = AccountGUID
	--FROM PCS.dbo.AccountUsers
	--WHERE AccountUserIdentifier = @AccountUserIdentifier   
	
	SET @AccountUserGUID = NULL --NEWID()
	SET @AccountGUID = NULL--NEWID()
END