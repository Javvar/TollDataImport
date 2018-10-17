-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Mapps class code to class guid
-- =============================================
CREATE FUNCTION ufGetClassGuid(@Class int)
RETURNS uniqueidentifier
AS
BEGIN
	DECLARE @Result uniqueidentifier
	
	SET @result = (CASE @Class
						WHEN 1 THEN '57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74' 
						WHEN 2 THEN 'A9FAB369-1871-4D07-9A5A-E6923C2EEB42' 
						WHEN 3 THEN '1A23A968-AF09-4B75-B973-2F010469FED2' 
						WHEN 4 THEN '7ED919E9-D588-4F70-8DF1-B07B3AA65C4F' 
						ELSE NULL
					   END)
					   
	RETURN @Result

END