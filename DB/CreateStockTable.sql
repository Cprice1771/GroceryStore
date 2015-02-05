CREATE TABLE Stock
(
	SKU int PRIMARY KEY,
	Brand varchar(255),
	DateReceived date,
	QuantityReceived int,
	CurrentStock int,
	ItemDescription varchar(255)
);