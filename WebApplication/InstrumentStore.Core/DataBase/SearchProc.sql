CREATE OR ALTER PROC SearchByName
    @input NVARCHAR(MAX),
    @page INT
AS
BEGIN
    DECLARE @productOnPage INT = 10;
    DECLARE @startRow INT = (@page - 1) * @productOnPage + 1;
    DECLARE @endRow INT = @page * @productOnPage;

    WITH ProductCTE AS
    (
        SELECT 
            p.ProductId,
            p.Name,
            p.Price,
            p.Quantity,
            p0.Name AS ProductCategory, 
            b.Name AS Brand, 
            c.Name AS Country,
			p.IsArchive,
            (SELECT TOP 1 i.Name 
             FROM Image AS i 
             WHERE i.ProductId = p.ProductId AND i.[Index] = 0) AS Image,
            ROW_NUMBER() OVER (ORDER BY p.Name) AS RowNum
        FROM Product AS p
        INNER JOIN ProductCategory AS p0 ON p.ProductCategoryId = p0.ProductCategoryId
        INNER JOIN Brand AS b ON p.BrandId = b.BrandId
        INNER JOIN Country AS c ON p.CountryId = c.CountryId
        WHERE LOWER(p.Name) LIKE '%' + LOWER(@input) + '%'
    )
    SELECT 
        ProductId,
        Name, 
        Price, 
        Quantity, 
		Image,
        ProductCategory, 
        Brand, 
        Country,
		IsArchive
    FROM ProductCTE
    WHERE RowNum BETWEEN @startRow AND @endRow;
END

EXEC SearchByName 'À', 1;
