# AdbLogger

A Xamarin Android Adb logger for the Microsoft.Extensions.Logging framework.

### Non-DI example:

```
ILoggerFactory loggerFactory = new LoggerFactory()
            .AddAdb("MyApp", LogLevel.Debug);
            
 ...
 
ILogger logger = loggerFactory.CreateLogger<SomeClass>();
 
 ...
 
logger.LogInformation("Log some information here!");
```

The logs can then be viewed from an attached `adb` instance:

`adb logcat -s MyApp`

### TODO:

* Create Nuget package

* Add DI example
