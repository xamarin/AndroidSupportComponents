# Android Support for Xamarin.Android

Xamarin.Android bindings for the now-deprecated Google Android Support Libraries.  These libraries have been replaced with AndroidX libraries.

# AndroidX
For AndroidX, please see https://github.com/xamarin/AndroidX

## Status

Please note that both these packages and the Google libraries contained within them have been deprecated, and are no longer maintained or supported:

https://developer.android.com/topic/libraries/support-library/packages

Existing bindings published from this repository will continue to be available on NuGet, but will not receive fixes or updates.

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
