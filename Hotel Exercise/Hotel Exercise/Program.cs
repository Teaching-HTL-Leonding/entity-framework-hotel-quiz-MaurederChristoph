using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

//solved with Kronsteiner Philip 

var factory = new HotelContextFactory();
var context = factory.CreateDbContext();

if (args[0] == "add") {
    await AddData(); 
} else if (args[0] == "query") {
    await QueryData();
}

async Task AddData() {
    HotelSpecial dogFriendly, organicFood, spa, sauna, indoorPool, outdoorPool;
    await context.HotelSpecials.AddRangeAsync(new[]
    {
        dogFriendly = new HotelSpecial() {
            Specials = Specials.DogFriendly,
        },
        organicFood = new HotelSpecial() {
            Specials = Specials.OrganicFood,
        },
        spa = new HotelSpecial() {
            Specials = Specials.Spa,
        },
        sauna = new HotelSpecial() {
            Specials = Specials.Sauna,
        },
        indoorPool = new HotelSpecial(){
            Specials = Specials.IndoorPool,
        },
        outdoorPool = new HotelSpecial(){
            Specials = Specials.OutdoorPool,
        }
    });

    RoomType singleRoom10, doubleRoom15, singleRoom15, doubleRoom20, juniorSuites45, honeymoon100;
    await context.RoomTypes.AddRangeAsync(new[]
    {
        singleRoom10 = new RoomType(){
            Title = "Single Room 10",
            NummberOfAvailableRooms = 3,
            Size = 10,
            disabilityAccessible = false,
        },
        doubleRoom15 = new RoomType(){
            Title = "Double Room 15",
            NummberOfAvailableRooms = 10,
            Size = 15,
            disabilityAccessible = false,
        },
        singleRoom15 = new RoomType(){
            Title = "Single Room 15",
            NummberOfAvailableRooms = 10,
            Size = 15,
            disabilityAccessible = true,
        },
        doubleRoom20 = new RoomType(){
            Title = "Double Room 20",
            NummberOfAvailableRooms = 25,
            Size = 20,
            disabilityAccessible = true,
        },
        juniorSuites45 = new RoomType(){
            Title = "Junior Suites 45",
            NummberOfAvailableRooms = 5,
            Size = 45,
            disabilityAccessible = true,
        },
        honeymoon100 = new RoomType(){
            Title = "Honeymoon 100",
            NummberOfAvailableRooms = 1,
            Size = 100,
            disabilityAccessible = true,
        },
    });

    await context.Price.AddRangeAsync(new[]
    {
        new Price(){
            PriceEUR = 40,
            RoomType = singleRoom10,
        },
        new Price(){
            PriceEUR = 60,
            RoomType = doubleRoom15,
        },
        new Price(){
            PriceEUR = 70,
            RoomType = singleRoom15
        },
        new Price(){
            PriceEUR = 120,
            RoomType = doubleRoom20,
        },
        new Price(){
            PriceEUR = 190,
            RoomType = juniorSuites45,
        },
        new Price(){
            PriceEUR = 300,
            RoomType = honeymoon100,
        },
    });

    await context.SaveChangesAsync();

    Hotel GrandHotelGoldenerHirsch, PensionMarianne;
    await context.Hotels.AddRangeAsync(new[]
    {
        GrandHotelGoldenerHirsch = new Hotel(){ 
            Name = "Grand Hotel Goldener Hirsch",
            Address = "Am Hausberg 17, 1234 Irgendwo",
            RoomTypes = { singleRoom10, doubleRoom15 },
            Specials ={ dogFriendly, organicFood }
        },
        PensionMarianne = new Hotel(){ 
            Name = "Pension Marianne",
            Address ="Im stillen Tal 42, 4711 Schönberg",
            RoomTypes = { singleRoom15, doubleRoom20, juniorSuites45, honeymoon100 },
            Specials = { spa, sauna, indoorPool, outdoorPool }
        }
     });

    await context.SaveChangesAsync();

}

async Task QueryData() { 
    
}

#region Model
enum Specials {
    Spa,
    Sauna,
    DogFriendly,
    IndoorPool,
    OutdoorPool,
    BikeRental,
    eCarChargingStation,
    VegetarianCuisine,
    OrganicFood
}

class Hotel {
    public int ID { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public List<HotelSpecial> Specials { get; set; }

    public List<RoomType> RoomTypes { get; set; }
}

class RoomType {
    public int Id { get; set; }

    [MaxLength(75)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; }

    public int Size { get; set; }

    public bool disabilityAccessible { get; set; }

    public int NummberOfAvailableRooms { get; set; }

    public Hotel Hotel { get; set; }

    public int HotelId { get; set; }

    public Price Price { get; set; }
}

class Price {
    public int Id { get; set; }

    public RoomType RoomType { get; set; }

    public int RoomTypeId { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    [Column(TypeName = "decimal(8, 2)")]
    public decimal PriceEUR { get; set; }
}

class HotelSpecial {
    public int Id { get; set; }

    public Specials Specials { get; set; }

    public Hotel Hotels { get; set; }
}
#endregion

#region Context
class HotelContext : DbContext {
    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options) { }

    public DbSet<Hotel> Hotels { get; set; }

    public DbSet<HotelSpecial> HotelSpecials { get; set; }

    public DbSet<RoomType> RoomTypes { get; set; }

    public DbSet<Price> Price { get; set; }
}

class HotelContextFactory : IDesignTimeDbContextFactory<HotelContext> {
    public HotelContext CreateDbContext(string[] args = null) {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<HotelContext>();
        optionsBuilder
            // Uncomment the following line if you want to print generated
            // SQL statements on the console.
            // .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

        return new HotelContext(optionsBuilder.Options);
    }
}
#endregion