USE MySpaceDB;

CREATE TABLE Customer (
	CustomerID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	FIO NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
	Email NVARCHAR(250)
);

CREATE TABLE Supplier (
	SupplierID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(250) NOT NULL,
);

CREATE TABLE Country (
	CountryID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE InstrumentType (
	InstrumentTypeID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
);

CREATE TABLE PaymentMethod (
	PaymentMethodID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE DeliveryMethod (
	DeliveryMethodID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Price DECIMAL NOT NULL
);

CREATE TABLE Instrument (
	InstrumentID INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Price DECIMAL NOT NULL,
	Quantity INT NOT NULL,
	Image IMAGE NOT NULL,

	InstrumentType INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (InstrumentType) 
		REFERENCES InstrumentType(InstrumentTypeID),
	Country INT NOT NULL
		CONSTRAINT cs_country FOREIGN KEY (Country) 
		REFERENCES Country(CountryID),
	Supplier INT NOT NULL 
		CONSTRAINT cs_supplier FOREIGN KEY (Supplier) 
		REFERENCES Supplier(SupplierID),
);

CREATE TABLE Basket(
	BasketID INT IDENTITY(0,1) PRIMARY KEY,
	Cost MONEY DEFAULT(0),

	Customer INT NOT NULL
		CONSTRAINT cs_customerb FOREIGN KEY (Customer)
		REFERENCES Customer(CustomerID),
);

CREATE TABLE tbl_Order (
	OrderID INT IDENTITY(0,1) PRIMARY KEY,
	CustomerCity VARCHAR(100) NOT NULL,
	CustomerAddress VARCHAR(150) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	DeliveryDate DATETIME,
	Cost MONEY,

	Basket INT NOT NULL
		CONSTRAINT cs_basketo FOREIGN KEY (Basket)
		REFERENCES Basket(BasketID),
	DeliveryMethod INT NOT NULL
		CONSTRAINT cs_delivery FOREIGN KEY (DeliveryMethod)
		REFERENCES DeliveryMethod(DeliveryMethodID),
	PaymentMethod INT NOT NULL
		CONSTRAINT cs_payment FOREIGN KEY (PaymentMethod)
		REFERENCES PaymentMethod(PaymentMethodID),
	Customer INT NOT NULL
		CONSTRAINT cs_customero FOREIGN KEY (Customer)
		REFERENCES Customer(CustomerID),
);

CREATE TABLE BasketItem (
	BasketID INT NOT NULL
		CONSTRAINT cs_basketi FOREIGN KEY (BasketID) 
		REFERENCES Basket(BasketID),
	InstrumentID INT NOT NULL
		CONSTRAINT cs_instrument FOREIGN KEY (InstrumentID) 
		REFERENCES Instrument(InstrumentID),
	Quantity INT NOT NULL
);


INSERT INTO InstrumentType (Name) VALUES ('Молоток');
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
	--	FROM Instrument i 
	--	WHERE i.InstrumentID = inserted.InstrumentID)
	--WHERE OrderID = inserted.OrderID
END;

--INSERT INTO tbl_Order (CustomerCity, CustomerAddress, )

