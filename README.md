# DotNET.Tools.DataParser
A .NET library for populate an Array of Objects from DataTable

~~~~
//getting an array of objects
var users = DotNET.Tools.DataParser.Populate<UserData>(dt);

//getting an single object
var users = DotNET.Tools.DataParser.Populate<UserData>(dt).First();

//getting an single object or null if DataTable is empty.
var users = DotNET.Tools.DataParser.Populate<UserData>(dt).FirstOrDefault();
~~~~
