USE Tomasos;

INSERT INTO Matratt VALUES('Pasta Bolognese','Färsk pasta med gräddig köttfärsås',99,2),
	('Potatissallad','En kall sallad på färskpotatis',89,3);
INSERT INTO Produkt VALUES
	('Potatis'),
	('Köttfärs'),
	('Grädde');
GO
INSERT INTO MatrattProdukt VALUES
	(101,3),
	(101,4),
	(101,9),
	(101,10),
	(102,1),
	(102,4),
	(102,6),
	(102,9);

ALTER TABLE Kund ADD Poang int

USE [Identity];
INSERT INTO AspNetRoles VALUES(3,NULL,'PremiumUser',1);