-- =============================================
-- Author:		SJ
-- Create date: 03-05-2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspImportNewStaff] 
AS
BEGIN
	
	SET XACT_ABORT, NOCOUNT ON
	
	DECLARE @starttrancount int
	
	BEGIN TRY	
		SELECT @starttrancount = @@TRANCOUNT

		IF @starttrancount = 0
			BEGIN TRANSACTION
			
		DECLARE @Date DATE
	
		SELECT @Date = MAX(TransDate) --ISNULL(MAX(TransDate),'2017-01-24 19:00:00')
		FROM Transactions
		
		IF(NOT EXISTS(SELECT * FROM MappingStaff WHERE StaffID = 'NULL'))
		BEGIN
			INSERT INTO MappingStaff
			SELECT TOP 1 'NULL',StaffGUID
			FROM [PCS].[dbo].Staff
			WHERE LastName = 'Automated'
		END
		
		IF(@Date IS NULL)
		BEGIN
			SELECT @Date = CAST(MIN(dt_concluded) AS DATE)
			FROM StagingTransactions
		END
		
		DECLARE @StaffIDs TABLE(StaffID VARCHAR(20))
		DECLARE @DistinctStaffIDs TABLE(StaffID VARCHAR(20))
			
		INSERT INTO @StaffIDs
		SELECT us_id
		FROM StagingIncidents 
		LEFT JOIN MappingStaff  ON StaffID = us_id
		WHERE StaffGuid is null AND CAST(dt_generated AS DATE) = @Date
		GROUP BY us_id
		
		INSERT INTO @StaffIDs
		SELECT us_id
		FROM StagingTransactions 
		LEFT JOIN MappingStaff  ON StaffID = us_id
		WHERE StaffGuid is null AND CAST(dt_concluded AS DATE) = @Date
		GROUP BY us_id
		
		INSERT INTO @DistinctStaffIDs
		SELECT DISTINCT StaffID
		FROM @StaffIDs
		WHERE StaffID IS NOT NULL AND StaffID <> 'NULL' 
		
		INSERT INTO [PCS].[dbo].[Staff]
		SELECT NEWID(),'3D102F16-871C-4F29-8BD1-340178C0C501','Coll',0,'C','Coll1',StaffID
			   ,GETDATE(),NULL,GETDATE(),NULL,1,StaffID,'67B07B168E30A1D8BB7644BE8B53011D47A04BE0',NULL,GETDATE(),NULL
		FROM @DistinctStaffIDs


		INSERT INTO [PCS].[dbo].StaffRoles
		SELECT NEWID(),Staff.StaffGUID,'882ABFF5-1FF9-46DC-B130-44EFF2A2337F',0
		FROM [PCS].[dbo].Staff
		LEFT JOIN MappingStaff  ON StaffID = LaneUserID
		WHERE MappingStaff.StaffGuid IS NULL AND Staff.FirstName = 'Coll1'

		INSERT INTO [PCS].[dbo].StaffPreviousPasswords
		SELECT NEWID(),Staff.StaffGUID,GETDATE(),'67B07B168E30A1D8BB7644BE8B53011D47A04BE0','8CEB92C4-5628-47D2-B641-2668FEADA13A',DATEADD(MONTH,1,GETDATE())
		FROM [PCS].[dbo].Staff
		LEFT JOIN [PCS].[dbo].StaffPreviousPasswords  ON StaffPreviousPasswords.StaffGUID = Staff.StaffGUID 
		WHERE StaffPreviousPasswords.StaffGuid IS NULL AND Staff.FirstName = 'Coll1'

		INSERT INTO MappingStaff
		SELECT LaneUserID,Staff.StaffGUID
		FROM [PCS].[dbo].Staff
		LEFT JOIN MappingStaff ON MappingStaff.StaffID = Staff.LaneUserID
		WHERE MappingStaff.StaffGuid IS NULL AND Staff.FirstName = 'Coll1'	
		
		SELECT  StaffID
		FROM @DistinctStaffIDs
				
				
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