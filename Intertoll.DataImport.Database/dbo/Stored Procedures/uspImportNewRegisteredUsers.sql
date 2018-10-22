-- =============================================
-- Author:		Sthabiso Jaffar
-- Create date: 2018-10-18
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspImportNewRegisteredUsers]
AS
BEGIN
	SET XACT_ABORT, NOCOUNT ON

	DECLARE @starttrancount int

	BEGIN TRY	

		DECLARE @LatestTransactionsDate DATE
	
		SELECT @LatestTransactionsDate = MAX(TransDate) 
		FROM Transactions

		IF(@LatestTransactionsDate IS NULL)
		BEGIN
			SELECT @LatestTransactionsDate = CAST(MIN(dt_concluded) AS DATE)
			FROM StagingTransactions
		END

		DECLARE @AccountIdentifiers TABLE(accountidentifier VARCHAR(20))

		INSERT INTO @AccountIdentifiers
		SELECT FullCardNumber -- TODO: create new field that has whole card number
		FROM Transactions 
		LEFT JOIN MappingRegisteredUsers  ON Identifier = FullCardNumber
		WHERE CAST(TransDate AS DATE) = @LatestTransactionsDate AND FullCardNumber like '707924%'
		GROUP BY FullCardNumber

		-- call new fu user stored procedure
		DECLARE @AccountIdentifier VARCHAR(50)

		DECLARE @IdentifierCursor as CURSOR;
		SET @IdentifierCursor = CURSOR FOR
		SELECT accountidentifier
		FROM @AccountIdentifiers
		
		OPEN @IdentifierCursor;
		
		FETCH NEXT FROM @IdentifierCursor INTO @AccountIdentifier;
		WHILE @@FETCH_STATUS = 0
		BEGIN

			DECLARE @assignedAccountNumber VARCHAR(10)
			DECLARE @AccountGuid UNIQUEIDENTIFIER
			DECLARE @AccountUserGuid UNIQUEIDENTIFIER

			DECLARE @dt DATETIME
			SET @dt = GETDATE()		

			DECLARE @RandomIdNo VARCHAR(100)
			SET @RandomIdNo = CAST(NEWID() AS VARCHAR(100))

			DECLARE @plGuid UNIQUEIDENTIFIER
			SELECT TOP 1 @plGuid = PlazaGuid
			FROM [PCS].[dbo].[Plazas]

			EXEC @assignedAccountNumber = [PCS].[dbo].[uspAccount_RegisterNewAccount]  
											@Title = 'Mr.', 
											@Initials = 'I', 
											@FirstName = 'NoFirstName' , 
											@LastName = 'NoLastNAme', 
											@IDNumber = @RandomIdNo, 
											@AccountType = '023FE992-05CD-DE11-8608-001517C991CF', -- prepaid
											@CellPhoneNumber = '0820000000', 
											@HomeTelephoneNumber = '0100000000',
											@WorkTelephoneNumber = '0100000000', 
											@EmailAddress = 'noemail@noemail,com', 
											@PreferredContactMethodGuid = 'DE0B907D-BA0D-44BC-8293-24BA5B448BC0', -- cellphone
											@HideBalanceInLane = 0 , 
											@PhysicalAddressLine = 'NoPhysicalAddressLine',  
											@PhysicalAddressGuid = '2F2CD14F-DB13-4CF2-89D0-2828B735D83A', 
											@PostalAddressLine = 'NoPhysicalAddressLine ',  
											@PostalAddressGuid = '2F2CD14F-DB13-4CF2-89D0-2828B735D83A', 
											@PlazaGUID = @plGuid, 
											@EffectiveDate = @dt, 
											@Username = '' , 
											@Password = '' 
											
			SELECT @AccountGuid = AccountGuid
			FROM [PCS].[dbo].Accounts
			WHERE AccountNr =  RIGHT('00000'+ CONVERT(VARCHAR,CONVERT(INT,@assignedAccountNumber)),6)  

			EXEC @AccountUserGuid = [PCS].[dbo].[uspAccount_AddAccountISOCardUser] 
			@AccountGUID = @AccountGuid, 
			@ClassGUID = '57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74', 
			@LicensePlate = 'NoLicensePlate', 
			@VehicleMakeGUID = 'EF63CECF-9262-4553-B299-0F8B58C85EB6', 
			@VehicleColourGUID = '9258FB0D-C68D-44CC-945D-5EB392451987', 
			@EffectiveDate = @dt, 
			@AccountUserIdentifier = @AccountIdentifier,
			@VirtualPlazas = default

			FETCH NEXT FROM @IdentifierCursor INTO @AccountIdentifier;
		END
		

		INSERT INTO MappingRegisteredUsers
		SELECT AU.AccountUserIdentifier,AU.AccountGuid,AU.AccountUserGuid
		FROM [PCS].[dbo].AccountUsers AU
		LEFT JOIN MappingRegisteredUsers ON MappingRegisteredUsers.Identifier = AU.AccountUserIdentifier COLLATE DATABASE_DEFAULT
		WHERE MappingRegisteredUsers.Identifier IS NULL 	

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