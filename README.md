# Android Support / AndroidX for Xamarin.Android

Xamarin creates and maintains Xamarin.Android bindings for the Google Android Support Libraries and AndroidX.

# AndroidX Roadmap
To view our progress on AndroidX bindings and to see more information about our roadmap, check out the [AndroidX Branch](https://github.com/xamarin/AndroidSupportComponents/tree/AndroidX#android-support---androidx-roadmap)


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
