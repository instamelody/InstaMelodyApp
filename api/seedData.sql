
INSERT INTO dbo.FileGroups (Name, DateCreated, DateModified)
	VALUES('Test Group', '2015-07-06', '2015-07-06')

INSERT INTO dbo.Categories (Name, DateCreated, DateModified)
	VALUES('Test Category', '2015-07-06', '2015-07-06')

INSERT INTO dbo.Melodies (Name, FileName, DateCreated, DateModified)
	VALUES('01 Bass', '01 Bass.wav', '2015-07-06', '2015-07-06')
INSERT INTO dbo.Melodies (Name, FileName, DateCreated, DateModified)
	VALUES('01 Drums', '01 Drums.wav', '2015-07-06', '2015-07-06')
INSERT INTO dbo.Melodies (Name, FileName, DateCreated, DateModified)
	VALUES('01 Melody', '01 Melody.wav', '2015-07-06', '2015-07-06')

INSERT INTO dbo.MelodyCategories (MelodyId, CategoryId, DateCreated)
	VALUES(1, 1, '2015-07-06')
INSERT INTO dbo.MelodyCategories (MelodyId, CategoryId, DateCreated)
	VALUES(2, 1, '2015-07-06')
INSERT INTO dbo.MelodyCategories (MelodyId, CategoryId, DateCreated)
	VALUES(3, 1, '2015-07-06')

INSERT INTO dbo.MelodyFileGroups (MelodyId, FileGroupId, DateCreated)
	VALUES(1, 1, '2015-07-06')
INSERT INTO dbo.MelodyFileGroups (MelodyId, FileGroupId, DateCreated)
	VALUES(2, 1, '2015-07-06')
INSERT INTO dbo.MelodyFileGroups (MelodyId, FileGroupId, DateCreated)
	VALUES(3, 1, '2015-07-06')