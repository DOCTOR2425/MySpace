CREATE TRIGGER move_to_archive
ON Product
AFTER DELETE
AS
BEGIN
    INSERT INTO ProductArchive(ProductArchiveId, Name, Description, Price, Quantity, ProductCategoryId, Country, Brand, )
    SELECT 
        ProductId, Name, Description, Price, Quantity, ProductTypeId, CountryId, BrandId
    FROM deleted;
END;
