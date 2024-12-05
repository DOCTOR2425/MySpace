USE MySpaceDB;

EXEC DropMySpaceDBTables;

CREATE TABLE Customer (
	CustomerId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	FIO NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
	Email NVARCHAR(250)
);

CREATE TABLE Supplier (
	SupplierId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(250) NOT NULL,
);

CREATE TABLE Country (
	CountryId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE ProductType (
	ProductTypeId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
);

CREATE TABLE PaymentMethod (
	PaymentMethodId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE DeliveryMethod (
	DeliveryMethodId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Price MONEY NOT NULL
);

CREATE TABLE Product (
	ProductId INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Price MONEY NOT NULL,
	Quantity INT NOT NULL,

	ProductTypeId INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (ProductTypeId) 
		REFERENCES ProductType(ProductTypeId),
	CountryId INT NOT NULL
		CONSTRAINT cs_country FOREIGN KEY (CountryId) 
		REFERENCES Country(CountryId),
	SupplierId INT NOT NULL 
		CONSTRAINT cs_supplier FOREIGN KEY (SupplierId) 
		REFERENCES Supplier(SupplierId),
);

CREATE TABLE ProductImage(
	ProductId INT NOT NULL,
	ImageName VARCHAR(100) NOT NULL
);

CREATE TABLE Property (
	PropertyId INT IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(150) NOT NULL,

	ProductTypeId INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (ProductTypeId) 
		REFERENCES ProductType(ProductTypeId)
);

CREATE TABLE PropertyProduct(
	Value VARCHAR(255),

	PropertyId INT NOT NULL
		CONSTRAINT cs_property FOREIGN KEY (PropertyId) 
		REFERENCES Property(PropertyId),
	ProductId INT NOT NULL
		CONSTRAINT cs_propertyproduct FOREIGN KEY (ProductId) 
		REFERENCES Product(ProductId)
);

CREATE TABLE Basket(
	BasketId INT IDENTITY(0,1) PRIMARY KEY,
	Cost MONEY DEFAULT(0),

	CustomerId INT NOT NULL
		CONSTRAINT cs_customerb FOREIGN KEY (CustomerId)
		REFERENCES Customer(CustomerId),
);

CREATE TABLE tbl_Order (
	tbl_OrderId INT IDENTITY(0,1) PRIMARY KEY,
	CustomerCity VARCHAR(100) NOT NULL,
	CustomerAddress VARCHAR(150) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	DeliveryDate DATETIME,
	Cost MONEY,

	BasketId INT NOT NULL
		CONSTRAINT cs_basketo FOREIGN KEY (BasketId)
		REFERENCES Basket(BasketId),
	DeliveryMethodId INT NOT NULL
		CONSTRAINT cs_delivery FOREIGN KEY (DeliveryMethodId)
		REFERENCES DeliveryMethod(DeliveryMethodId),
	PaymentMethodId INT NOT NULL
		CONSTRAINT cs_payment FOREIGN KEY (PaymentMethodId)
		REFERENCES PaymentMethod(PaymentMethodId),
	CustomerId INT NOT NULL
		CONSTRAINT cs_customero FOREIGN KEY (CustomerId)
		REFERENCES Customer(CustomerId),
);

CREATE TABLE BasketItem (
	BasketId INT NOT NULL
		CONSTRAINT cs_basketi FOREIGN KEY (BasketId) 
		REFERENCES Basket(BasketId),
	ProductId INT NOT NULL
		CONSTRAINT cs_Product FOREIGN KEY (ProductId) 
		REFERENCES Product(ProductId),
	Quantity INT NOT NULL
);


INSERT INTO ProductType (Name) VALUES ('Молоток');
INSERT INTO Country(Name) VALUES ('Беларусь');
INSERT INTO Supplier(Name, Email, Phone) VALUES ('Молоток', 'Email', '+12345678');


GO
CREATE OR ALTER TRIGGER CalculateOrderCost
ON BasketItem
AFTER INSERT
AS
BEGIN
	SELECT * FROM inserted;
	--UPDATE tbl_Order
	--SET Cost = Cost + 
	--	(SELECT Price 
	--	FROM Product i 
	--	WHERE i.ProductId = inserted.ProductId)
	--WHERE OrderId = inserted.OrderID
END;

--INSERT INTO tbl_Order (CustomerCity, CustomerAddress, )
