-- =============================================
-- Author:		Sthabiso Jaffar
-- Create date: 2018-10-18
-- Description:	
-- =============================================
CREATE PROCEDURE uspImportNewRegisteredUsers
AS
BEGIN
	SET XACT_ABORT, NOCOUNT ON

	DECLARE @starttrancount int

	BEGIN TRY	

		DECLARE @Date DATE
	
		SELECT @Date = MAX(TransDate) 
		FROM Transactions

		IF(@Date IS NULL)
		BEGIN
			SELECT @Date = CAST(MIN(dt_concluded) AS DATE)
			FROM StagingTransactions
		END

		DECLARE @AccountIdentifiers TABLE(accountidentifier VARCHAR(20))
		DECLARE @DistinctAccountIdentifiers TABLE(accountidentifier VARCHAR(20))

		INSERT INTO @AccountIdentifiers
		SELECT CardNumber -- TODO: create new field that has whole card number
		FROM Transactions 
		LEFT JOIN MappingRegisteredUsers  ON Identifier = CardNumber
		WHERE CAST(TransDate AS DATE) = @Date /* AND fu transaction*/
		GROUP BY CardNumber

		-- call new fu user stored procedure

		--INSERT INTO MappingRegisteredUsers
		--SELECT AccountGuid
		--FROM [PCS].[dbo].AccountUsers AU
		--LEFT JOIN MappingRegisteredUsers ON MappingRegisteredUsers.Identifier = AU.Identifier
		--WHERE MappingRegisteredUsers.Identifier IS NULL 	

		SELECT *
		FROM @AccountIdentifiers

		IF @starttrancount = 0 
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH	
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION;	
		
		DECLARE @ErrorNumber INT = ERROR_NUMBER();
		DECLARE @ErrorLine INT = ERROR_LINE();
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		PRINT('Exception: ' + @ErrorMessage)
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH LOG;	
	END CATCH	
END