CREATE TRIGGER move_to_archive
ON Product
AFTER DELETE
AS
BEGIN
    INSERT INTO ProductArchive(ProductArchiveId, Name, Description, Price, Quantity, Image, ProductTypeId, CountryId, BrandId)
    SELECT 
        ProductId, Name, Description, Price, Quantity, Image, ProductTypeId, CountryId, BrandId
    FROM deleted;
END;
