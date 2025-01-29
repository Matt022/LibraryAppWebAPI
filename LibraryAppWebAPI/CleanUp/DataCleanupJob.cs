using LibraryAppWebAPI.DataContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppWebAPI.CleanUp;

public class DataCleanupJob(LibraryContext libraryContext)
{
    public async Task ExecuteAsync()
    {
        Console.WriteLine("Začína premazávanie dát...\n");

        await libraryContext.Database.ExecuteSqlRawAsync(@"
            DELETE FROM Members;
            DELETE FROM Messages;
            DELETE FROM QueueItems;
            DELETE FROM RentalEntries;
            DELETE FROM Title;

            DBCC CHECKIDENT('Members', RESEED, 0);
            DBCC CHECKIDENT('Messages', RESEED, 0);
            DBCC CHECKIDENT('QueueItems', RESEED, 0);
            DBCC CHECKIDENT('RentalEntries', RESEED, 0);
            DBCC CHECKIDENT('Title', RESEED, 0);

            INSERT INTO Title (Author, Name, AvailableCopies, TotalAvailableCopies, NumberOfPages, ISBN, Discriminator, CanManipulate) VALUES
                ('J.K. Rowling', 'Harry Potter and the Philosopher''s Stone', 12, 12, 223, '9780747532699', 'Book', 0),
                ('George Orwell', '1984', 8, 8, 328, '9780451524935', 'Book', 0),
                ('J.R.R. Tolkien', 'The Lord of the Rings', 5, 5, 1178, '9780544003415', 'Book', 0),
                ('F. Scott Fitzgerald', 'The Great Gatsby', 7, 7, 180, '9780743273565', 'Book', 0),
                ('Harper Lee', 'To Kill a Mockingbird', 10, 10, 281, '9780061120084', 'Book', 0),
                ('Jane Austen', 'Pride and Prejudice', 6, 6, 279, '9781503290563', 'Book', 0),
                ('Mark Twain', 'Adventures of Huckleberry Finn', 9, 9, 366, '9780486280615', 'Book', 0),
                ('Mary Shelley', 'Frankenstein', 4, 4, 280, '9780486282114', 'Book', 0),
                ('Charlotte Brontë', 'Jane Eyre', 5, 5, 500, '9780141441146', 'Book', 0),
                ('Herman Melville', 'Moby-Dick', 3, 3, 635, '9781503280786', 'Book', 0),
                ('Leo Tolstoy', 'War and Peace', 2, 2, 1225, '9780198800545', 'Book', 0),
                ('Gabriel García Márquez', 'One Hundred Years of Solitude', 6, 6, 417, '9780060883287', 'Book', 0),
                ('Ernest Hemingway', 'The Old Man and the Sea', 8, 8, 127, '9780684801223', 'Book', 0),
                ('William Shakespeare', 'Hamlet', 10, 10,342,'9780140714548' ,'Book' ,0),
                ('Oscar Wilde','The Picture of Dorian Gray' ,7 ,7 ,254 ,'9780141439570' ,'Book' ,0),
                ('George R.R. Martin','A Game of Thrones' ,5 ,5 ,835 ,'9780553593716' ,'Book' ,0),
                ('Aldous Huxley','Brave New World' ,6 ,6 ,288 ,'9780060850524' ,'Book' ,0),
                ('Kurt Vonnegut','Slaughterhouse-Five' ,9 ,9 ,275 ,'9780440180296' ,'Book' ,0),
                ('Emily Brontë','Wuthering Heights' ,4 ,4 ,416 ,'9780141439556' ,'Book' ,0),
                ('J.D. Salinger','The Catcher in the Rye' ,8 ,8 ,214 ,'9780316769488' ,'Book' ,0);

            INSERT INTO Title (Author, Name, AvailableCopies, TotalAvailableCopies, PublishYear, NumberOfMinutes, Discriminator, CanManipulate) VALUES
                ('Christopher Nolan', 'Inception', 7, 7, 2010, 148, 'Dvd', 0),
                ('Steven Spielberg', 'Jurassic Park', 5, 5, 1993, 127, 'Dvd', 0),
                ('Peter Jackson', 'The Lord of the Rings: The Fellowship of the Ring', 8, 8, 2001, 178, 'Dvd', 0),
                ('Quentin Tarantino', 'Pulp Fiction', 6, 6, 1994, 154, 'Dvd', 0),
                ('James Cameron', 'Titanic', 10, 10, 1997, 195, 'Dvd', 0),
                ('Ridley Scott', 'Gladiator', 4, 4, 2000, 155, 'Dvd', 0),
                ('Francis Ford Coppola', 'The Godfather', 6, 6, 1972, 175, 'Dvd', 0),
                ('Martin Scorsese', 'The Wolf of Wall Street', 5, 5, 2013, 180, 'Dvd', 0),
                ('Robert Zemeckis', 'Forrest Gump', 9, 9, 1994, 142, 'Dvd', 0),
                ('Stanley Kubrick', 'The Shining', 10, 10, 1980, 146, 'Dvd', 0),
                ('George Lucas', 'Star Wars: Episode IV - A New Hope', 8, 8, 1977, 121, 'Dvd', 0),
                ('Christopher Nolan', 'The Dark Knight', 6, 6, 2008, 152, 'Dvd', 0),
                ('David Fincher', 'Fight Club', 5, 5, 1999,139,'Dvd' ,0),
                ('Alfonso Cuarón','Gravity' ,7 ,7 ,2013 ,91 ,'Dvd' ,0),
                ('Guillermo del Toro','Pan''s Labyrinth' ,4 ,4 ,2006 ,118 ,'Dvd' ,0),
                ('Baz Luhrmann','The Great Gatsby' ,6 ,6 ,2013 ,143 ,'Dvd' ,0),
                ('Jon Favreau','Iron Man' ,8 ,8 ,2008 ,126 ,'Dvd' ,0),
                ('James Cameron','Avatar' ,5 ,5 ,2009 ,162 ,'Dvd' ,0),
                ('Frank Darabont','The Shawshank Redemption' ,10 ,10 ,'1994' ,'142','Dvd' ,'0'),
                ('Damien Chazelle','La La Land' ,6 ,6 ,'2016' ,'128','Dvd' ,'0');

            INSERT INTO Members(FirstName, LastName, PersonalId, DateOfBirth, CanManipulate) VALUES
            	('John','Smith','123456789','1990-03-15', 0),
            	('Emily','Johnson','987654321','1985-07-22', 0),
            	('Michael','Williams','456123789','1978-11-03', 0),
            	('Sarah','Brown','852963741','1995-02-10', 0),
            	('David','Jones','159753486','1982-06-05', 0),
            	('Emma','Garcia','753159846','2000-12-19', 0),
            	('James','Martinez','951357486','1998-09-14', 0),
            	('Sophia','Hernandez','789654123','1993-05-08', 0),
            	('Christopher','Lopez','321654987','1975-01-27', 0),
            	('Olivia','Gonzalez','147258369','2001-10-02', 0),
            	('Daniel','Perez','963852741','1988-04-11', 0),
            	('Ava','Wilson','456789123','1992-08-24', 0),
            	('Matthew ','Anderson ','258147369 ','1980-07-18', 0),
            	('Isabella ','Thomas ','741852963 ','1999-03-29', 0),
            	('Ethan ','Taylor ','369147258 ','1983-12-07', 0),
            	('Mia ','Moore ','654987321 ','2002-11-25', 0),
            	('Alexander ','White ','987321654 ','1977-06-17', 0),
            	('Charlotte ','Harris ','123789456 ','1994-01-09', 0),
            	('Liam ','Clark ','951753852 ','1989-10-13', 0),
            	('Amelia ','Lewis ','789321654 ','2003-02-06', 0);
        ");

        Console.WriteLine("\nPremazávanie dát dokončené.");
    }
}
