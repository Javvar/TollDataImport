-- =============================================
-- Author:		SJ
-- Create date: 19-04-2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspGetLaneAliveStatus]
AS
BEGIN
	SET NOCOUNT ON;

    select l.LaneGuid
	   ,(select COUNT(*) 
		 from StagingTimeSlices 
		 where dt_started >= DATEADD(MINUTE,-30,GETDATE())) CNT
	from MappingLanes l


END