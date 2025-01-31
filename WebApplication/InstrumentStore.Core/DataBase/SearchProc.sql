CREATE OR ALTER PROC SearchByName
    @input NVARCHAR(MAX)
AS
BEGIN
    SELECT TOP(10) 
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
        [c].[Name] AS CountryName
    FROM [Product] AS [p]
    INNER JOIN [ProductCategory] AS [p0] ON [p].[ProductCategoryId] = [p0].[ProductCategoryId]
    INNER JOIN [Brand] AS [b] ON [p].[BrandId] = [b].[BrandId]
    INNER JOIN [Country] AS [c] ON [p].[CountryId] = [c].[CountryId]
    WHERE LOWER(p.Name) LIKE LOWER(@input) + '%';
END

EXEC SearchByName '¿  ”';
