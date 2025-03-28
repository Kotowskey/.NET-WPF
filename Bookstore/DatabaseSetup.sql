-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BookstoreDB')
BEGIN
    CREATE DATABASE BookstoreDB;
END
GO

USE BookstoreDB;
GO

-- Create test data
-- Add test users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'User')
BEGIN
    -- Tables will be created by EF Core migrations
    PRINT 'Database structure has not been created yet. Run migrations first.';
END
ELSE
BEGIN
    -- Insert test authors
    IF NOT EXISTS (SELECT TOP 1 * FROM Author)
    BEGIN
        INSERT INTO Author (FirstName, LastName, StateEnum)
        VALUES 
            ('Stephen', 'King', 0),
            ('J.K.', 'Rowling', 0),
            ('Andrzej', 'Sapkowski', 0),
            ('Olga', 'Tokarczuk', 0);
    END

    -- Insert test publishers
    IF NOT EXISTS (SELECT TOP 1 * FROM Publisher)
    BEGIN
        INSERT INTO Publisher (Name, Country, StateEnum)
        VALUES 
            ('Znak', 'Polska', 0),
            ('Albatros', 'Polska', 0),
            ('Bloomsbury', 'Wielka Brytania', 0),
            ('SuperNowa', 'Polska', 0);
    END

    -- Insert test genres
    IF NOT EXISTS (SELECT TOP 1 * FROM Genre)
    BEGIN
        INSERT INTO Genre (Name, StateEnum)
        VALUES 
            ('Horror', 0),
            ('Fantasy', 0),
            ('Science Fiction', 0),
            ('Literatura piękna', 0),
            ('Kryminał', 0);
    END

    -- Insert test series
    IF NOT EXISTS (SELECT TOP 1 * FROM Series)
    BEGIN
        INSERT INTO Series (Name, Description, StateEnum)
        VALUES 
            ('Harry Potter', 'Seria o młodym czarodzieju', 0),
            ('Wiedźmin', 'Saga o Geralcie z Rivii', 0);
    END

    -- Insert test books
    IF NOT EXISTS (SELECT TOP 1 * FROM Book)
    BEGIN
        INSERT INTO Book (Title, Description, Isbn, PublicationYear, PublisherId, SeriesId, StateEnum)
        VALUES 
            ('Harry Potter i Kamień Filozoficzny', 'Pierwsza część przygód Harry''ego Pottera', '9788380082118', '1997-06-26', 3, 1, 0),
            ('Ostatnie życzenie', 'Pierwszy tom sagi o Wiedźminie', '9788375780635', '1993-01-01', 4, 2, 0),
            ('Lśnienie', 'Kultowy horror', '9788376486918', '1977-01-28', 2, NULL, 0),
            ('Księgi Jakubowe', 'Powieść historyczna', '9788308049396', '2014-10-23', 1, NULL, 0);
    END

    -- Insert test book-author relationships
    IF NOT EXISTS (SELECT TOP 1 * FROM BookAuthor)
    BEGIN
        INSERT INTO BookAuthor (BookId, AuthorId, StateEnum)
        VALUES 
            (1, 2, 0), -- Harry Potter - J.K. Rowling
            (2, 3, 0), -- Wiedźmin - Andrzej Sapkowski
            (3, 1, 0), -- Lśnienie - Stephen King
            (4, 4, 0); -- Księgi Jakubowe - Olga Tokarczuk
    END

    -- Insert test book-genre relationships
    IF NOT EXISTS (SELECT TOP 1 * FROM BookGenre)
    BEGIN
        INSERT INTO BookGenre (BookId, GenreId, StateEnum)
        VALUES 
            (1, 2, 0), -- Harry Potter - Fantasy
            (2, 2, 0), -- Wiedźmin - Fantasy
            (3, 1, 0), -- Lśnienie - Horror
            (4, 4, 0); -- Księgi Jakubowe - Literatura piękna
    END

    -- Insert test users
    IF NOT EXISTS (SELECT TOP 1 * FROM [User])
    BEGIN
        INSERT INTO [User] (Id, Email, Username, FirstName, LastName, Password, IsAdmin, StateEnum)
        VALUES 
            ('8F7D6E5C-4B3A-2D1E-0F9G-8H7I6J5K4L3M', 'admin@bookstore.com', 'admin', 'Admin', 'Admin', 'AdminPass123', 1, 0),
            ('1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P', 'user@bookstore.com', 'user', 'Jan', 'Kowalski', 'UserPass123', 0, 0);
    END

    -- Insert test files
    IF NOT EXISTS (SELECT TOP 1 * FROM [File])
    BEGIN
        INSERT INTO [File] (Name, Type, Source, StateEnum)
        VALUES 
            ('harry_potter.jpg', 'image/jpeg', '/images/books/harry_potter.jpg', 0),
            ('wiedzmin.jpg', 'image/jpeg', '/images/books/wiedzmin.jpg', 0),
            ('lsnienie.jpg', 'image/jpeg', '/images/books/lsnienie.jpg', 0),
            ('ksiegi_jakubowe.jpg', 'image/jpeg', '/images/books/ksiegi_jakubowe.jpg', 0);
    END

    -- Insert test offers
    IF NOT EXISTS (SELECT TOP 1 * FROM Offer)
    BEGIN
        INSERT INTO Offer (Name, Description, Price, BookId, CreatedDate, RequesterId, FileId, OfferStateEnum, StateEnum)
        VALUES 
            ('Harry Potter i Kamień Filozoficzny - nowa', 'Nowa książka, nieczytana', 49.99, 1, GETDATE(), '1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P', 1, 10, 0),
            ('Ostatnie życzenie - używana', 'Książka w dobrym stanie, czytana raz', 29.99, 2, DATEADD(day, -5, GETDATE()), '1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P', 2, 10, 0),
            ('Lśnienie - używana', 'Lekkie zagięcia rogów, poza tym stan dobry', 24.50, 3, DATEADD(day, -10, GETDATE()), '1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P', 3, 10, 0),
            ('Księgi Jakubowe - nowa', 'Książka nowa, zafoliowana', 89.99, 4, DATEADD(day, -2, GETDATE()), '1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P', 4, 10, 0);
    END

    -- Insert test providers
    IF NOT EXISTS (SELECT TOP 1 * FROM Provider)
    BEGIN
        INSERT INTO Provider (Name, StateEnum)
        VALUES 
            ('PayU', 0),
            ('Przelewy24', 0),
            ('Blik', 0);
    END

    PRINT 'Test data has been added successfully.';
END