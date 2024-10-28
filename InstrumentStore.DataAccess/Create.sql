USE MySpaceDB;

CREATE TABLE Customer (
	CustomerID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	FIO NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
	Email NVARCHAR(250)
);

CREATE TABLE Supplier (
	SupplierID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(250) NOT NULL,
);

CREATE TABLE Country (
	CountryID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE InstrumentType (
	InstrumentTypeID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
);

CREATE TABLE PaymentMethod (
	PaymentMethodID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL
);

CREATE TABLE DeliveryMethod (
	DeliveryMethodID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Price DECIMAL NOT NULL
);

CREATE TABLE Instrument (
	InstrumentID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(250) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Price DECIMAL NOT NULL,
	Quantity INT NOT NULL,
	Image IMAGE NOT NULL,

	InstrumentType INT NOT NULL
		CONSTRAINT cs_type FOREIGN KEY (InstrumentType) 
		REFERENCES InstrumentType(InstrumentTypeID)
		ON UPDATE CASCADE,
	Country INT NOT NULL
		CONSTRAINT cs_country FOREIGN KEY (Country) 
		REFERENCES Country(CountryID)
		ON UPDATE CASCADE,
	Supplier INT NOT NULL 
		CONSTRAINT cs_supplier FOREIGN KEY (Supplier) 
		REFERENCES Supplier(SupplierID)
		ON UPDATE CASCADE
);

CREATE TABLE tbl_Order (
	OrderID INT IDENTITY(1,1) PRIMARY KEY,
	CustomerCity VARCHAR(100) NOT NULL,
	CustomerAddress VARCHAR(150) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	DeliveryDate DATETIME,
	Cost MONEY,

	DeliveryMethod INT NOT NULL
		CONSTRAINT cs_delivery FOREIGN KEY (DeliveryMethod)
		REFERENCES DeliveryMethod(DeliveryMethodID)
		ON UPDATE CASCADE,
	PaymentMethod INT NOT NULL
		CONSTRAINT cs_payment FOREIGN KEY (PaymentMethod)
		REFERENCES PaymentMethod(PaymentMethodID)
		ON UPDATE CASCADE,
	Customer INT NOT NULL
		CONSTRAINT cs_customer FOREIGN KEY (Customer)
		REFERENCES Customer(CustomerID)
		ON UPDATE CASCADE,
);



CREATE TABLE OrderItem (
	OrderID INT NOT NULL
		CONSTRAINT cs_order FOREIGN KEY (OrderID) 
		REFERENCES tbl_Order(OrderID)
		ON UPDATE CASCADE,
	InstrumentID INT NOT NULL
		CONSTRAINT cs_instrument FOREIGN KEY (InstrumentID) 
		REFERENCES Instrument(InstrumentID)
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
	--	WHERE i.InstrumentID = inserted.InstrumentID)
	--WHERE OrderID = inserted.OrderID
END;

--INSERT INTO tbl_Order (CustomerCity, CustomerAddress, )
