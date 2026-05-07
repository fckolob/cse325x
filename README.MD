Evidence of the new record created:

[
  {
    "id": 1,
    "name": "Classic Italian",
    "isGlutenFree": false
  },
  {
    "id": 2,
    "name": "Veggie",
    "isGlutenFree": true
  },
  {
    "id": 3,
    "name": "Hawaii",
    "isGlutenFree": false
  }
]

Function for create sales report file:

void createSalesReport(){

    // Create the beginning of the report.

    File.WriteAllText(Path.Combine(salesTotalDir, "salesReport.txt"), $"Sales summary \n -------------\n Total sales: {salesTotal} \n \n Details: \n");

    foreach (var file in salesFiles)
    {
         // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Store the total in a variable
        var salesSubTotal = data?.Total ?? 0;

        File.AppendAllText(Path.Combine(salesTotalDir, "salesReport.txt"), $"{file}: ${salesSubTotal}{Environment.NewLine}");
    }


}