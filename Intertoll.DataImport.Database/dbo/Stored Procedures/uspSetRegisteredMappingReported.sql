-- =============================================
-- Author:		Sthabiso Jaffar
-- Create date: 23/10/2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspSetRegisteredMappingReported](@Identifier VARCHAR(50))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE MappingRegisteredUsers
	SET Reported = 1
	WHERE Identifier = @Identifier
END