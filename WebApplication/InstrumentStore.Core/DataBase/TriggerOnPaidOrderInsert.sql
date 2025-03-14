CREATE OR ALTER TRIGGER trg_MoveFromCartToPaidOrder
ON PaidOrder
AFTER INSERT
AS
BEGIN
    DECLARE @UserId uniqueidentifier;
    DECLARE @PaidOrderId uniqueidentifier;

    SELECT @UserId = UserId, @PaidOrderId = PaidOrderId
    FROM INSERTED;

    INSERT INTO PaidOrderItem (PaidOrderItemId, Quantity, Price, PaidOrderId, ProductId)
    SELECT
        NEWID(),
        ci.Quantity,
        p.Price,
        @PaidOrderId,
        ci.ProductId
    FROM
        CartItem ci
    INNER JOIN
        Product p ON ci.ProductId = p.ProductId
    WHERE
        ci.UserId = @UserId;

	UPDATE p
	SET p.Quantity = p.Quantity - ci.Quantity
	FROM Product p
	JOIN CartItem ci ON p.ProductId = ci.ProductId
	WHERE ci.UserId = @UserId;

    DELETE FROM CartItem
    WHERE UserId = @UserId;
END;