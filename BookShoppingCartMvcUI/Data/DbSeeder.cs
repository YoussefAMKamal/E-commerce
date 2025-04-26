using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Data;

public class DbSeeder
{
    public static async Task SeedDefaultData(IServiceProvider service)
    {
        try
        {
            var context = service.GetService<ApplicationDbContext>();

            // this block will check if there are any pending migrations and apply them
            if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            // create admin role if not exists
            var adminRoleExists = await roleMgr.RoleExistsAsync(Roles.Admin.ToString());

            if (!adminRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }

            // create user role if not exists
            var userRoleExists = await roleMgr.RoleExistsAsync(Roles.User.ToString());

            if (!userRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

            // create admin user

            var admin = new IdentityUser
            {
                UserName = "The Admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }


            if (!context.Genres.Any())
            {
                await SeedGenreAsync(context);
            }

            if (!context.Books.Any())
            {
                await SeedBooksAsync(context);
                // update stock table
                await context.Database.ExecuteSqlRawAsync(@"
                     INSERT INTO Stock(BookId,Quantity) 
                     SELECT 
                     b.Id,
                     10 
                     FROM Book b
                     WHERE NOT EXISTS (
                     SELECT * FROM [Stock]
                     );
                ");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static async Task SeedGenreAsync(ApplicationDbContext context)
    {
        var genres = new[]
         {
            new Genre { GenreName = "Fantasy" },
            new Genre { GenreName = "Science Fiction" },
            new Genre { GenreName = "Mystery" },
            new Genre { GenreName = "Thriller" },
            new Genre { GenreName = "Romance" },
            new Genre { GenreName = "Horror" },
            new Genre { GenreName = "Historical Fiction" },
            new Genre { GenreName = "Non-Fiction" },
            new Genre { GenreName = "Biography" },
            new Genre { GenreName = "Self-Help" },
            new Genre { GenreName = "Young Adult" },
            new Genre { GenreName = "Children" },
            new Genre { GenreName = "Graphic Novel" },
            new Genre { GenreName = "Classic" },
            new Genre { GenreName = "Adventure" },
            new Genre { GenreName = "Dystopian" },
            new Genre { GenreName = "Poetry" },
            new Genre { GenreName = "Drama" },
            new Genre { GenreName = "Crime" },
            new Genre { GenreName = "Memoir" }
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBooksAsync(ApplicationDbContext context)
    {
        var books = new List<Book>
        {
            // Fantasy
            new Book { BookName = "The Dragon Reborn", AuthorName = "Robert Jordan", Price = 12.99,Image = "The Dragon Reborn.jpg", GenreId = 1 },
            new Book { BookName = "The Way of Kings", AuthorName = "Brandon Sanderson", Price = 14.99,Image = "The Way of Kings.jpg", GenreId = 1 },
            new Book { BookName = "Eragon", AuthorName = "Christopher Paolini", Price = 10.50,Image = "Eragon.jpg", GenreId = 1 },
            new Book { BookName = "The Name of the Wind", AuthorName = "Patrick Rothfuss", Price = 13.75,Image = "The Name of the Wind.jpg", GenreId = 1 },
            new Book { BookName = "The Last Wish", AuthorName = "Andrzej Sapkowski", Price = 11.20,Image = "The Last Wish.jpg", GenreId = 1 },

            // Science Fiction
            new Book { BookName = "Ender's Game", AuthorName = "Orson Scott Card", Price = 9.99,Image = "Ender's Game.jpg", GenreId = 2 },
            new Book { BookName = "Snow Crash", AuthorName = "Neal Stephenson", Price = 12.50,Image = "Snow Crash.jpg", GenreId = 2 },
            new Book { BookName = "The Martian", AuthorName = "Andy Weir", Price = 13.00, GenreId = 2 },
            new Book { BookName = "Foundation", AuthorName = "Isaac Asimov", Price = 15.20, GenreId = 2 },
            new Book { BookName = "Red Mars", AuthorName = "Kim Stanley Robinson", Price = 11.45, GenreId = 2 },

            // Mystery
            new Book { BookName = "Big Little Lies", AuthorName = "Liane Moriarty", Price = 9.80, GenreId = 3 },
            new Book { BookName = "The Cuckoo's Calling", AuthorName = "Robert Galbraith", Price = 10.95, GenreId = 3 },
            new Book { BookName = "Sharp Objects", AuthorName = "Gillian Flynn", Price = 11.60, GenreId = 3 },
            new Book { BookName = "The Girl on the Train", AuthorName = "Paula Hawkins", Price = 13.25, GenreId = 3 },
            new Book { BookName = "In the Woods", AuthorName = "Tana French", Price = 12.10, GenreId = 3 },

            // Thriller
            new Book { BookName = "The Da Vinci Code", AuthorName = "Dan Brown", Price = 10.25, GenreId = 4 },
            new Book { BookName = "I Am Watching You", AuthorName = "Teresa Driscoll", Price = 9.50, GenreId = 4 },
            new Book { BookName = "The Silent Patient", AuthorName = "Alex Michaelides", Price = 11.70, GenreId = 4 },
            new Book { BookName = "Behind Closed Doors", AuthorName = "B.A. Paris", Price = 10.80, GenreId = 4 },
            new Book { BookName = "The Couple Next Door", AuthorName = "Shari Lapena", Price = 12.00, GenreId = 4 },

           // Romance
            new Book { BookName = "Me Before You", AuthorName = "Jojo Moyes", Price = 9.99, GenreId = 5 },
            new Book { BookName = "Outlander", AuthorName = "Diana Gabaldon", Price = 14.00, GenreId = 5 },
            new Book { BookName = "The Notebook", AuthorName = "Nicholas Sparks", Price = 11.25, GenreId = 5 },
            new Book { BookName = "It Ends With Us", AuthorName = "Colleen Hoover", Price = 13.90, GenreId = 5 },
            new Book { BookName = "Twilight", AuthorName = "Stephenie Meyer", Price = 10.60, GenreId = 5 },

            // Horror
            new Book { BookName = "Pet Sematary", AuthorName = "Stephen King", Price = 10.80, GenreId = 6 },
            new Book { BookName = "The Haunting of Hill House", AuthorName = "Shirley Jackson", Price = 9.50, GenreId = 6 },
            new Book { BookName = "Bird Box", AuthorName = "Josh Malerman", Price = 11.25, GenreId = 6 },
            new Book { BookName = "The Troop", AuthorName = "Nick Cutter", Price = 12.15, GenreId = 6 },
            new Book { BookName = "The Exorcist", AuthorName = "William Peter Blatty", Price = 13.40, GenreId = 6 },

            // Historical Fiction
            new Book { BookName = "The Book Thief", AuthorName = "Markus Zusak", Price = 9.99, GenreId = 7 },
            new Book { BookName = "All the Light We Cannot See", AuthorName = "Anthony Doerr", Price = 14.75, GenreId = 7 },
            new Book { BookName = "The Nightingale", AuthorName = "Kristin Hannah", Price = 13.10, GenreId = 7 },
            new Book { BookName = "The Tattooist of Auschwitz", AuthorName = "Heather Morris", Price = 12.90, GenreId = 7 },
            new Book { BookName = "Beneath a Scarlet Sky", AuthorName = "Mark Sullivan", Price = 11.60, GenreId = 7 },

            // Non-Fiction
            new Book { BookName = "Educated", AuthorName = "Tara Westover", Price = 11.20, GenreId = 8 },
            new Book { BookName = "Thinking, Fast and Slow", AuthorName = "Daniel Kahneman", Price = 13.50, GenreId = 8 },
            new Book { BookName = "A Brief History of Time", AuthorName = "Stephen Hawking", Price = 12.40, GenreId = 8 },
            new Book { BookName = "The Body", AuthorName = "Bill Bryson", Price = 15.30, GenreId = 8 },
            new Book { BookName = "Outliers", AuthorName = "Malcolm Gladwell", Price = 10.95, GenreId = 8 },

            // Biography
            new Book { BookName = "Steve Jobs", AuthorName = "Walter Isaacson", Price = 14.80, GenreId = 9 },
            new Book { BookName = "Becoming", AuthorName = "Michelle Obama", Price = 12.99, GenreId = 9 },
            new Book { BookName = "Elon Musk", AuthorName = "Ashlee Vance", Price = 13.75, GenreId = 9 },
            new Book { BookName = "Alexander Hamilton", AuthorName = "Ron Chernow", Price = 15.40, GenreId = 9 },
            new Book { BookName = "Long Walk to Freedom", AuthorName = "Nelson Mandela", Price = 11.30, GenreId = 9 },

            // Self-Help
            new Book { BookName = "Atomic Habits", AuthorName = "James Clear", Price = 11.99, GenreId = 10 },
            new Book { BookName = "The Power of Now", AuthorName = "Eckhart Tolle", Price = 10.50, GenreId = 10 },
            new Book { BookName = "The Subtle Art of Not Giving a F*ck", AuthorName = "Mark Manson", Price = 12.75, GenreId = 10 },
            new Book { BookName = "You Are a Badass", AuthorName = "Jen Sincero", Price = 9.99, GenreId = 10 },
            new Book { BookName = "Can't Hurt Me", AuthorName = "David Goggins", Price = 13.80, GenreId = 10 },

            // Young Adult
            new Book { BookName = "The Hunger Games", AuthorName = "Suzanne Collins", Price = 10.75, GenreId = 11 },
            new Book { BookName = "Divergent", AuthorName = "Veronica Roth", Price = 11.25, GenreId = 11 },
            new Book { BookName = "The Fault in Our Stars", AuthorName = "John Green", Price = 9.99, GenreId = 11 },
            new Book { BookName = "Thirteen Reasons Why", AuthorName = "Jay Asher", Price = 10.50, GenreId = 11 },
            new Book { BookName = "Eleanor & Park", AuthorName = "Rainbow Rowell", Price = 9.60, GenreId = 11 },

            // Children
            new Book { BookName = "Where the Wild Things Are", AuthorName = "Maurice Sendak", Price = 8.25, GenreId = 12 },
            new Book { BookName = "Charlotte's Web", AuthorName = "E.B. White", Price = 9.50, GenreId = 12 },
            new Book { BookName = "Matilda", AuthorName = "Roald Dahl", Price = 10.10, GenreId = 12 },
            new Book { BookName = "The Cat in the Hat", AuthorName = "Dr. Seuss", Price = 7.90, GenreId = 12 },
            new Book { BookName = "Goodnight Moon", AuthorName = "Margaret Wise Brown", Price = 6.75, GenreId = 12 },

            // Graphic Novel
            new Book { BookName = "Watchmen", AuthorName = "Alan Moore", Price = 14.60, GenreId = 13 },
            new Book { BookName = "V for Vendetta", AuthorName = "Alan Moore", Price = 13.80, GenreId = 13 },
            new Book { BookName = "Maus", AuthorName = "Art Spiegelman", Price = 12.99, GenreId = 13 },
            new Book { BookName = "Saga", AuthorName = "Brian K. Vaughan", Price = 11.70, GenreId = 13 },
            new Book { BookName = "Persepolis", AuthorName = "Marjane Satrapi", Price = 10.90, GenreId = 13 },

            // Classic
            new Book { BookName = "1984", AuthorName = "George Orwell", Price = 9.60, GenreId = 14 },
            new Book { BookName = "To Kill a Mockingbird", AuthorName = "Harper Lee", Price = 10.40, GenreId = 14 },
            new Book { BookName = "The Great Gatsby", AuthorName = "F. Scott Fitzgerald", Price = 11.00, GenreId = 14 },
            new Book { BookName = "Frankenstein", AuthorName = "Mary Shelley", Price = 8.95, GenreId = 14 },
            new Book { BookName = "Moby-Dick", AuthorName = "Herman Melville", Price = 9.80, GenreId = 14 },

            // Adventure
            new Book { BookName = "Treasure Island", AuthorName = "Robert Louis Stevenson", Price = 8.75, GenreId = 15 },
            new Book { BookName = "The Call of the Wild", AuthorName = "Jack London", Price = 9.20, GenreId = 15 },
            new Book { BookName = "The Lost World", AuthorName = "Arthur Conan Doyle", Price = 10.35, GenreId = 15 },
            new Book { BookName = "Journey to the Center of the Earth", AuthorName = "Jules Verne", Price = 11.60, GenreId = 15 },
            new Book { BookName = "Life of Pi", AuthorName = "Yann Martel", Price = 12.30, GenreId = 15 },

            // Dystopian
            new Book { BookName = "Brave New World", AuthorName = "Aldous Huxley", Price = 10.20, GenreId = 16 },
            new Book { BookName = "Fahrenheit 451", AuthorName = "Ray Bradbury", Price = 9.60, GenreId = 16 },
            new Book { BookName = "The Road", AuthorName = "Cormac McCarthy", Price = 11.10, GenreId = 16 },
            new Book { BookName = "The Giver", AuthorName = "Lois Lowry", Price = 10.45, GenreId = 16 },
            new Book { BookName = "Oryx and Crake", AuthorName = "Margaret Atwood", Price = 12.00, GenreId = 16 },

            // Poetry
            new Book { BookName = "Milk and Honey", AuthorName = "Rupi Kaur", Price = 9.20, GenreId = 17 },
            new Book { BookName = "The Sun and Her Flowers", AuthorName = "Rupi Kaur", Price = 10.00, GenreId = 17 },
            new Book { BookName = "Leaves of Grass", AuthorName = "Walt Whitman", Price = 11.75, GenreId = 17 },
            new Book { BookName = "The Waste Land", AuthorName = "T.S. Eliot", Price = 9.90, GenreId = 17 },
            new Book { BookName = "Ariel", AuthorName = "Sylvia Plath", Price = 10.60, GenreId = 17 },

            // Drama
            new Book { BookName = "Hamlet", AuthorName = "William Shakespeare", Price = 8.50, GenreId = 18 },
            new Book { BookName = "Death of a Salesman", AuthorName = "Arthur Miller", Price = 9.25, GenreId = 18 },
            new Book { BookName = "A Streetcar Named Desire", AuthorName = "Tennessee Williams", Price = 10.40, GenreId = 18 },
            new Book { BookName = "Waiting for Godot", AuthorName = "Samuel Beckett", Price = 9.90, GenreId = 18 },
            new Book { BookName = "Othello", AuthorName = "William Shakespeare", Price = 8.80, GenreId = 18 },

            // Crime
            new Book { BookName = "The Godfather", AuthorName = "Mario Puzo", Price = 11.50, GenreId = 19 },
            new Book { BookName = "In Cold Blood", AuthorName = "Truman Capote", Price = 12.75, GenreId = 19 },
            new Book { BookName = "The Big Sleep", AuthorName = "Raymond Chandler", Price = 10.80, GenreId = 19 },
            new Book { BookName = "American Taboo", AuthorName = "Philip Weiss", Price = 9.60, GenreId = 19 },
            new Book { BookName = "The Black Dahlia", AuthorName = "James Ellroy", Price = 11.30, GenreId = 19 },

            // Memoir
            new Book { BookName = "Educated", AuthorName = "Tara Westover", Price = 11.99, GenreId = 20 },
            new Book { BookName = "The Glass Castle", AuthorName = "Jeannette Walls", Price = 10.60, GenreId = 20 },
            new Book { BookName = "Wild", AuthorName = "Cheryl Strayed", Price = 12.25, GenreId = 20 },
            new Book { BookName = "Eat, Pray, Love", AuthorName = "Elizabeth Gilbert", Price = 10.80, GenreId = 20 },
            new Book { BookName = "When Breath Becomes Air", AuthorName = "Paul Kalanithi", Price = 13.20, GenreId = 20 }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
    }
}
