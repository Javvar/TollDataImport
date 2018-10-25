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
		DECLARE @AccountIdentifiers TABLE(accountidentifier VARCHAR(50))

		INSERT INTO @AccountIdentifiers
		SELECT TOP 10000 FullIdentifier 
		FROM StagingAccountIdentifiers SAI
		JOIN StagingAccountDetails SAD ON SAD.ac_nr = SAI.ac_nr -- we are looking for identifiers whose details have been imported
		LEFT JOIN MappingRegisteredUsers ON Identifier = SAI.FullIdentifier COLLATE DATABASE_DEFAULT
		WHERE Identifier IS NULL 

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

			IF(NOT EXISTS(SELECT * FROM [PCS].[dbo].AccountUsers WHERE AccountUserIdentifier = @AccountIdentifier)) -- prevent duplicates
			BEGIN
				DECLARE @assignedAccountNumber VARCHAR(10)
				DECLARE @AccountGuid UNIQUEIDENTIFIER
				DECLARE @AccountUserGuid UNIQUEIDENTIFIER

				-- check if account exists for this user
				SELECT @AccountGuid = A.AccountGUID
				FROM [PCS].[dbo].AccountUsers AU
				JOIN [PCS].[dbo].Accounts A ON A.AccountGuid = AU.AccountGUID
				WHERE AU.AccountUserIdentifier = @AccountIdentifier

				SET @AccountGuid = NULL

				SELECT TOP 1 @AccountGuid = AU.AccountGUID
				FROM StagingAccountDetails SAD
				JOIN StagingAccountIdentifiers SAI ON SAD.ac_nr = SAI.ac_nr AND 
										SAI.FullIdentifier = @AccountIdentifier AND
											SAI.ac_nr = (SELECT TOP 1 ac_nr 
														FROM StagingAccountIdentifiers SAI1 
														WHERE SAI.FullIdentifier = SAI1.FullIdentifier)
				JOIN [PCS].[dbo].AccountUsers AU ON AU.AccountUserGUID = (SELECT TOP 1 AccountUserGUID 
																		  FROM [PCS].[dbo].AccountUsers 
																		  WHERE AccountUserIdentifier IN (SELECT FullIdentifier
																										  FROM StagingAccountIdentifiers
																										  WHERE ac_nr = SAD.ac_nr COLLATE DATABASE_DEFAULT))

				IF(@AccountGuid IS NULL) -- if accountguid is null it means that there is no identifier that has been imported that belongd to this account
				BEGIN
					DECLARE @Title VARCHAR(10)
 					DECLARE @Initials VARCHAR(10)
 					DECLARE @FirstName VARCHAR(100)
 					DECLARE @LastName VARCHAR(100)
 					DECLARE @CellPhoneNumber VARCHAR(15)
 					DECLARE @HomeTelephoneNumber VARCHAR(15)
 					DECLARE @WorkTelephoneNumber VARCHAR(15)
 					DECLARE @EmailAddress VARCHAR(100)

					-- populate account information, will not be null since we inner join at the top
					SELECT @Title = CASE ac_title WHEN '' THEN 'NoT' ELSE ac_title END,
						   @Initials = CASE ac_initial WHEN '' THEN 'NoI' ELSE ac_initial END,
						   @FirstName = CASE ac_surname WHEN '' THEN 'NoName' ELSE ac_surname END,
						   @LastName = CASE ac_surname WHEN '' THEN 'NoLastName' ELSE ac_surname END,
						   @CellPhoneNumber = CASE ac_cell_ph WHEN '' THEN '0812345678' ELSE ac_cell_ph END, 
						   @HomeTelephoneNumber = ac_home_ph,
						   @WorkTelephoneNumber = ac_work_ph,
						   @EmailAddress = ac_email
					FROM StagingAccountIdentifiers SAI
					JOIN StagingAccountDetails SAD ON SAD.ac_nr = SAI.ac_nr 	
					WHERE sai.FullIdentifier = @AccountIdentifier		

					DECLARE @dt DATETIME
					SET @dt = GETDATE()		

					DECLARE @RandomIdNo VARCHAR(100)
					SET @RandomIdNo = CAST(NEWID() AS VARCHAR(100))

					DECLARE @plGuid UNIQUEIDENTIFIER
					SELECT TOP 1 @plGuid = PlazaGuid
					FROM [PCS].[dbo].[Plazas]

					EXEC @assignedAccountNumber = [PCS].[dbo].[uspAccount_RegisterNewAccount]  
													@Title = @Title, 
													@Initials = @Initials, 
													@FirstName = @FirstName , 
													@LastName = @LastName, 
													@IDNumber = @RandomIdNo, 
													@AccountType = '023FE992-05CD-DE11-8608-001517C991CF', -- prepaid
													@CellPhoneNumber = @CellPhoneNumber, 
													@HomeTelephoneNumber = @HomeTelephoneNumber,
													@WorkTelephoneNumber = @WorkTelephoneNumber, 
													@EmailAddress = @EmailAddress, 
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
				END

				IF(@AccountGuid IS NOT NULL)
				BEGIN	
					DECLARE @RandomLicenseNo VARCHAR(100)
					SET @RandomLicenseNo = CAST(NEWID() AS VARCHAR(100))

					EXEC @AccountUserGuid = [PCS].[dbo].[uspAccount_AddAccountISOCardUser] 
											@AccountGUID = @AccountGuid, 
											@ClassGUID = '57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74', 
											@LicensePlate = @RandomLicenseNo, 
											@VehicleMakeGUID = 'EF63CECF-9262-4553-B299-0F8B58C85EB6', 
											@VehicleColourGUID = '9258FB0D-C68D-44CC-945D-5EB392451987', 
											@EffectiveDate = @dt, 
											@AccountUserIdentifier = @AccountIdentifier,
											@VirtualPlazas = default
				END				
			END

			FETCH NEXT FROM @IdentifierCursor INTO @AccountIdentifier;
		END
		

		INSERT INTO MappingRegisteredUsers
		SELECT AU.AccountUserIdentifier,AU.AccountGuid,AU.AccountUserGuid,0
		FROM [PCS].[dbo].AccountUsers AU
		LEFT JOIN MappingRegisteredUsers ON MappingRegisteredUsers.Identifier = AU.AccountUserIdentifier COLLATE DATABASE_DEFAULT
		WHERE MappingRegisteredUsers.Identifier IS NULL 	

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