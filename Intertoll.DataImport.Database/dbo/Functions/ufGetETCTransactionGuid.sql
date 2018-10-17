-- =============================================
-- Author:		SJ
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION ufGetETCTransactionGuid(@TransactionSeqNo INT,@TransactionDateTime DATETIME,@LaneCode VARCHAR(5))
RETURNS uniqueidentifier
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result uniqueidentifier
	
	DECLARE @VPCode VARCHAR(10)
	DECLARE @Direction VARCHAR(10)
	
	SELECT @VPCode = VPCode,@Direction = ETCDirectionCode
	FROM MappingLanes
	WHERE LaneCode = @LaneCode

	SET @Result =   RIGHT('00' + CONVERT(VARCHAR(2),DATEPART(mm, @TransactionDateTime), 2),2) 
				  + RIGHT('00' + CONVERT(VARCHAR(2),DATEPART(dd, @TransactionDateTime), 2),2) 
				  + RIGHT('00' + CONVERT(VARCHAR(2),DATEPART(hh, @TransactionDateTime), 2),2) 
				  + RIGHT('00' + CONVERT(VARCHAR(2),DATEPART(mi, @TransactionDateTime), 2),2)  
				  + '-0016-' + RIGHT('0000' + ISNULL(@VPCode,''), 4) 
				  + '-' + [dbo].ufRemoveNonNumericCharacters(@LaneCode)
				  + @Direction 
				  +'-'
				  + RIGHT('000000000000' + CONVERT(VARCHAR(12),@TransactionSeqNo, 12),12)	
	RETURN @Result

END