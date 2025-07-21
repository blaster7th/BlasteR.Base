using System.Data;
using Dapper;

namespace BlasteR.Base.Tests
{
    public static class TestDatabase
    {
        public static void Initialize(IDbConnection db)
        {
            // Create tables
            db.Execute(@"
                CREATE TABLE FirstEntities (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IntValue INTEGER,
                    StringValue TEXT,
                    CreatedAt DATETIME,
                    ModifiedAt DATETIME,
                    CreatedBy TEXT,
                    ModifiedBy TEXT
                );

                CREATE TABLE SecondEntities (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IntValue INTEGER,
                    StringValue TEXT,
                    FirstEntityId INTEGER,
                    CreatedAt DATETIME,
                    ModifiedAt DATETIME,
                    CreatedBy TEXT,
                    ModifiedBy TEXT,
                    FOREIGN KEY (FirstEntityId) REFERENCES FirstEntities(Id)
                );

                CREATE TABLE SoftDeletableTestEntities (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IntValue INTEGER,
                    StringValue TEXT,
                    FirstEntityId INTEGER,
                    CreatedAt DATETIME,
                    ModifiedAt DATETIME,
                    CreatedBy TEXT,
                    ModifiedBy TEXT,
                    IsDeleted INTEGER,
                    DeletedAt DATETIME,
                    DeletedBy TEXT
                );
            ");
        }
    }
} 