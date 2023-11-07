CREATE TABLE Vehicles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Type NVARCHAR(50),
    RegistrationNumber NVARCHAR(50) UNIQUE,
    Color NVARCHAR(50),
    Brand NVARCHAR(50),
    Model NVARCHAR(50),
    NumberOfWheels INT,
    ArrivalTime DATETIME
)