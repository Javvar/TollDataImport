-- =============================================
-- Author:		SJ
-- Create date: 25/10/2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspUpdateStagingHotlist]
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @AddedCards TABLE(CardNr VARCHAR(50))
    DECLARE @DeletedCards TABLE(CardNr VARCHAR(50))

	INSERT INTO @AddedCards
	SELECT HL.CardNr
	FROM [PCS].[dbo].Hotlist HL
	LEFT JOIN StagingMISHotlist SHL ON HL.CardNr = HL.CardNr
	WHERE SHL.CardNr IS NULL

	INSERT INTO @DeletedCards
	SELECT SHL.CardNr
	FROM StagingMISHotlist SHL
	LEFT JOIN [PCS].[dbo].Hotlist HL ON HL.CardNr = HL.CardNr
	WHERE HL.CardNr IS NULL

	INSERT INTO StagingMISHotlistUpdates
	SELECT CardNr,'Add',NULL
	FROM @AddedCards

	INSERT INTO StagingMISHotlistUpdates
	SELECT CardNr,'Delete',NULL
	FROM @DeletedCards

	INSERT INTO StagingMISHotlist
	SELECT CardNr
	FROM @AddedCards

	DELETE StagingMISHotlist
	WHERE CardNr IN (SELECT CardNr FROM @DeletedCards)
END