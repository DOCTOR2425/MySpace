CREATE OR ALTER PROC SearchByName
    @input NVARCHAR(MAX),
    @page INT
AS
BEGIN
	DECLARE @productOnPage INT = 5
    DECLARE @startRow INT = (@page - 1) * @productOnPage + 1;
    DECLARE @endRow INT = @page * @productOnPage;

    WITH ProductCTE AS
    (
        SELECT 
            [p].[ProductId], 
            [p].[BrandId], 
            [p].[CountryId], 
            [p].[Description], 
            [p].[Image], 
            [p].[Name], 
            [p].[Price], 
            [p].[ProductCategoryId], 
            [p].[Quantity], 
            [p0].[ProductCategoryId] AS ProductCategoryId2, 
            [p0].[Name] AS ProductCategoryName, 
            [b].[BrandId] AS BrandId2, 
            [b].[Name] AS BrandName, 
            [c].[CountryId] AS CountryId2, 
            [c].[Name] AS CountryName,
            ROW_NUMBER() OVER (ORDER BY [p].[Name]) AS RowNum
        FROM [Product] AS [p]
        INNER JOIN [ProductCategory] AS [p0] ON [p].[ProductCategoryId] = [p0].[ProductCategoryId]
        INNER JOIN [Brand] AS [b] ON [p].[BrandId] = [b].[BrandId]
        INNER JOIN [Country] AS [c] ON [p].[CountryId] = [c].[CountryId]
        WHERE LOWER(p.Name) LIKE LOWER(@input) + '%'
    )
    SELECT 
        [ProductId], 
        [BrandId], 
        [CountryId], 
        [Description], 
        [Image], 
        [Name], 
        [Price], 
        [ProductCategoryId], 
        [Quantity], 
        [ProductCategoryId2], 
        [ProductCategoryName], 
        [BrandId2], 
        [BrandName], 
        [CountryId2], 
        [CountryName]
    FROM ProductCTE
    WHERE RowNum BETWEEN @startRow AND @endRow;
END

EXEC SearchByName '¿  ”', 2;
