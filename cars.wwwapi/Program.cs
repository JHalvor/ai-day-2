var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS policy
app.UseCors("AllowAll");

var cars = new List<Car>
{
    new Car { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2020 },
    new Car { Id = 2, Make = "Honda", Model = "Civic", Year = 2019 }
};

// Get all cars
app.MapGet("/cars", () => cars)
   .WithName("GetAllCars")
   .WithOpenApi();

// Get car by ID
app.MapGet("/cars/{id}", (int id) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    return car is not null ? Results.Ok(car) : Results.NotFound();
})
.WithName("GetCarById")
.WithOpenApi();

// Create a new car
app.MapPost("/cars", (Car newCar) =>
{
    newCar.Id = cars.Max(c => c.Id) + 1;
    cars.Add(newCar);
    return Results.Created($"/cars/{newCar.Id}", newCar);
})
.WithName("CreateCar")
.WithOpenApi();

// Update an existing car
app.MapPut("/cars/{id}", (int id, Car updatedCar) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    if (car is null) return Results.NotFound();

    car.Make = updatedCar.Make;
    car.Model = updatedCar.Model;
    car.Year = updatedCar.Year;

    return Results.NoContent();
})
.WithName("UpdateCar")
.WithOpenApi();

// Delete a car
app.MapDelete("/cars/{id}", (int id) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    if (car is null) return Results.NotFound();

    cars.Remove(car);
    return Results.NoContent();
})
.WithName("DeleteCar")
.WithOpenApi();

app.Run();

record Car
{
    public int Id { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
}
