-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetLaneSession] @laneCode varchar(10)
AS
BEGIN
	select top 1 *
	from sessions
END
