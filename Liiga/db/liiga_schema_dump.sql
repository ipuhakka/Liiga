PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS matches(
	hometeam TEXT,
	awayteam TEXT,
	homescore INTEGER,
	awayscore INTEGER,
	overtime INTEGER,
	played_date DATE,
	playoff INTEGER,
	season CHAR(5),
	CONSTRAINT PK_MATCH PRIMARY KEY (played_date, hometeam, awayteam)
);

COMMIT;
PRAGMA foreign_keys=ON;
