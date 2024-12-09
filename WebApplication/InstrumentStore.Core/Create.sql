USE MySpaceDB;

CREATE TABLE Customer (
	CustomerId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	FIO NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
	Email NVARCHAR(250)
);

CREATE TABLE Supplier (
	SupplierId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(250) NOT NULL,
);

CREATE TABLE Country (
	CountryId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE ProductType (
	ProductTypeId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
);

CREATE TABLE PaymentMethod (
	PaymentMethodId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE DeliveryMethod (
	DeliveryMethodId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Price DECIMAL NOT NULL
);

CREATE TABLE Product (
	ProductId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Price DECIMAL NOT NULL,
	Quantity INT NOT NULL,
	Image IMAGE NOT NULL,

	ProductType INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (ProductType) 
		REFERENCES ProductType(ProductTypeId)
		ON UPDATE CASCADE,
	Country INT NOT NULL
		CONSTRAINT cs_country FOREIGN KEY (Country) 
		REFERENCES Country(CountryId)
		ON UPDATE CASCADE,
	Supplier INT NOT NULL 
		CONSTRAINT cs_supplier FOREIGN KEY (Supplier) 
		REFERENCES Supplier(SupplierId)
		ON UPDATE CASCADE
);

CREATE TABLE tbl_Order (
	OrderId INT IdENTITY(1,1) PRIMARY KEY,
	CustomerCity VARCHAR(100) NOT NULL,
	CustomerAddress VARCHAR(150) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	DeliveryDate DATETIME,
	Cost MONEY,

	DeliveryMethod INT NOT NULL
		CONSTRAINT cs_delivery FOREIGN KEY (DeliveryMethod)
		REFERENCES DeliveryMethod(DeliveryMethodId)
		ON UPDATE CASCADE,
	PaymentMethod INT NOT NULL
		CONSTRAINT cs_payment FOREIGN KEY (PaymentMethod)
		REFERENCES PaymentMethod(PaymentMethodId)
		ON UPDATE CASCADE,
	Customer INT NOT NULL
		CONSTRAINT cs_customer FOREIGN KEY (Customer)
		REFERENCES Customer(CustomerId)
		ON UPDATE CASCADE,
);



CREATE TABLE OrderItem (
	OrderId INT NOT NULL
		CONSTRAINT cs_order FOREIGN KEY (OrderId) 
		REFERENCES tbl_Order(OrderId)
		ON UPDATE CASCADE,
	ProductId INT NOT NULL
		CONSTRAINT cs_Product FOREIGN KEY (ProductId) 
		REFERENCES Product(ProductId)
		ON UPDATE CASCADE,
	Quantity INT NOT NULL
);


INSERT INTO ProductType (Name) VALUES ('Молоток');
INSERT INTO Country(Name) VALUES ('Беларусь');
INSERT INTO Supplier(Name, Email, Phone) VALUES ('Молоток', 'Email', '+12345678');


GO
CREATE OR ALTER TRIGGER CalculateOrderCost
ON OrderItem
AFTER INSERT
AS
BEGIN
	SELECT * FROM inserted;
	--UPDATE tbl_Order
	--SET Cost = Cost + 
	--	(SELECT Price 
	--	FROM Product i 
	--	WHERE i.ProductId = inserted.ProductId)
	--WHERE OrderId = inserted.OrderId
END;

--INSERT INTO tbl_Order (CustomerCity, CustomerAddress, )





