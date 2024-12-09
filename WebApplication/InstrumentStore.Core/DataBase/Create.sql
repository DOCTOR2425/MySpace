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

CREATE TABLE InstrumentType (
	InstrumentTypeId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
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

CREATE TABLE Instrument (
	InstrumentId INT NOT NULL IdENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Price DECIMAL NOT NULL,
	Quantity INT NOT NULL,
	Image IMAGE NOT NULL,

	InstrumentType INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (InstrumentType) 
		REFERENCES InstrumentType(InstrumentTypeId)
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
	InstrumentId INT NOT NULL
		CONSTRAINT cs_instrument FOREIGN KEY (InstrumentId) 
		REFERENCES Instrument(InstrumentId)
		ON UPDATE CASCADE,
	Quantity INT NOT NULL
);


INSERT INTO InstrumentType (Name) VALUES ('Молоток');
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
	--	FROM Instrument i 
	--	WHERE i.InstrumentId = inserted.InstrumentId)
	--WHERE OrderId = inserted.OrderId
END;

--INSERT INTO tbl_Order (CustomerCity, CustomerAddress, )
