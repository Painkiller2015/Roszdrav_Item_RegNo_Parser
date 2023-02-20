IF OBJECT_ID('tempdb..#TmpSubQry') IS NOT NULL DROP TABLE #TmpSubQry

SELECT ILE.[Item No_] AS ItemNo, SUM(ILE.Quantity) AS QTY
INTO #TmpSubQry
FROM [dbo].[TABLE] AS ILE WITH(NOLOCK)
WHERE ILE.[Location Code] IN ('ЦЕНТР')
GROUP BY ILE.[Item No_]

SELECT DISTINCT it.[Reg_ Certif_ Medical Product], it.[No_]
FROM #TmpSubQry AS tmp
JOIN [dbo].Item AS it
	ON it.No_ = tmp.ItemNo
WHERE tmp.QTY > 0
	AND it.[Reg_ Certif_ Medical Product] <> ''

IF OBJECT_ID('tempdb..#TmpSubQry') IS NOT NULL DROP TABLE #TmpSubQry