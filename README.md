# Unity Save and Load Service
Tool to save and load data with JSON.

```csharp
    // Save location
    var root = $"{Application.streamingAssetsPath}";
    
    // Init new service and set root
    var service = new SaveService();
    var service.Init(_root);
    
    // Set custom settings with:
    service.Settings = new Settings();
  
    // Save
    var dataSaved = new Data();
    service.Save("campaign1", "save1", dataSaved)
    
    // Load
    service.Load("campaign1", "save1", out Data dataLoaded)

    // Autosave
    service.AutoSave("campaign1", dataSaved));
```