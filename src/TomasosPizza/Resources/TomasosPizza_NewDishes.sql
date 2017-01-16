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
DELETE FROM AspNetRoles;
INSERT INTO AspNetRoles VALUES
	(1,NULL,'Administrator',1),
	(2,NULL,'RegularUser',2),
	(3,NULL,'PremiumUser',3);

--If something goes wrong, here are the SQL-commands to set role
INSERT INTO AspNetUserRoles VALUES('eb1b67db-7766-4d78-a96f-f1d23310c551',2);
