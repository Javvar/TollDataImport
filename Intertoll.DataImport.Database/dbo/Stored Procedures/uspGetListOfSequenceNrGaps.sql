
CREATE PROCEDURE [dbo].[uspGetListOfSequenceNrGaps]
	@from date, 
	@to date 
AS
BEGIN

	SET NOCOUNT ON;

    IF OBJECT_ID('tempdb..#TrxBatch') IS NOT NULL 
		DROP TABLE #TrxBatch

	IF OBJECT_ID('tempdb..#LaneCounts') IS NOT NULL 
		DROP TABLE #LaneCounts

	DECLARE @MissingSequenceNumbers TABLE  (Lane VARCHAR(5),SequenceNr INT NOT NULL) 
	DECLARE @MonthFirstDay date = CONVERT(DATE,DATEADD(d,-(DAY(GETDATE()-1)),GETDATE()))
	DECLARE @MonthLastDay date = CONVERT(DATE,DATEADD(d,-(DAY(DATEADD(m,1,GETDATE()))),DATEADD(m,1,GETDATE())))


	SELECT ln_id,		   
		   min(tx_seq_nr) StartSeq,
		   max(tx_seq_nr) endSeq,
		   count(*) cnt
	INTO #LaneCounts
	FROM StagingTransactions WITH (NOLOCK)
	WHERE  dt_concluded between @from and @to and len(tx_seq_nr) > 2
	GROUP BY ln_id

	SELECT T.*
	INTO #TrxBatch 
	FROM StagingTransactions T WITH (NOLOCK)
	WHERE dt_concluded between @from and @to 

	declare @lanecode varchar(5),@StartNumber int,@EndNumber int,@TransCount INT
	DECLARE db_cursor CURSOR FOR 
	SELECT * 
	FROM #LaneCounts
	order by ln_id  

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @lanecode, @StartNumber, @EndNumber ,@TransCount 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		IF((@EndNumber - @StartNumber + (case when @EndNumber = @StartNumber then 0 else 1 end)) <> @TransCount)
		BEGIN
			print @lanecode
			print @StartNumber
			print @EndNumber
			print @EndNumber - @StartNumber + (case when @EndNumber = @StartNumber then 0 else 1 end)
			print @TransCount

			;WITH SequenceTab (seq) 
			 AS (SELECT @StartNumber 
				 UNION ALL 
				 SELECT ( SequenceTab.seq + 1 ) seq 
				 FROM   SequenceTab 
				 WHERE  SequenceTab.seq < @EndNumber) 

			INSERT INTO @MissingSequenceNumbers 
			SELECT @lanecode,seq 
			FROM   SequenceTab 
				   LEFT JOIN #TrxBatch T WITH (NOLOCK) ON ln_id = @lanecode 
														  and seq = T.tx_seq_nr 
														  AND T.tx_seq_nr BETWEEN @StartNumber AND @EndNumber 
			WHERE  T.tx_seq_nr IS NULL 
			OPTION (maxrecursion 0) 
		END
		

		FETCH NEXT FROM db_cursor INTO @lanecode, @StartNumber, @EndNumber ,@TransCount 
	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

	 IF OBJECT_ID('tempdb..#TrxBatch') IS NOT NULL 
		DROP TABLE #TrxBatch

	IF OBJECT_ID('tempdb..#LaneCounts') IS NOT NULL 
		DROP TABLE #LaneCounts

	SELECT Lane, Sequencenr
	FROM @MissingSequenceNumbers
	order by Lane, sequencenr
END