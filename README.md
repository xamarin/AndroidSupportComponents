# Android Support for Xamarin.Android

Xamarin creates and maintains Xamarin.Android bindings for the Google Android Support Libraries.

# AndroidX
For AndroidX, please see https://github.com/xamarin/AndroidX

## Building

Building from source requires calling the cake script:

```
.\build.ps1 --target=binderate
.\build.ps1 --target=libs
```


## License

The license for this repository is specified in
[LICENSE.md](LICENSE.md)

The `externals` build task downloads some external dependencies from Google which are licensed under and subject to the terms of [Android Software Development Kit License Agreement](http://developer.android.com/sdk/terms.html)

## Contribution Guidelines
The Contribution Guidelines for this repository are listed in [CONTRIBUTING.md](.github/CONTRIBUTING.md)

## .NET Foundation
This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects)
