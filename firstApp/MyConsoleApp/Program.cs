Console.WriteLine("Hello, World!");
Console.WriteLine($"The current time is {DateTime.Now}");
DateTime christmas = new DateTime(2026, 12, 25);
TimeSpan diference = christmas - DateTime.Now;
Console.WriteLine($"There are {diference.Days} days until the next Christmas");
