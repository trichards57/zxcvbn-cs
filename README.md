Zxcvbn C#/.NET
==============

[![Build status](https://ci.appveyor.com/api/projects/status/90iaonivnh97yfda?svg=true)](https://ci.appveyor.com/project/trichards57/zxcvbn-cs-62692)
[![Coverage Status](https://coveralls.io/repos/github/trichards57/zxcvbn-cs/badge.svg)](https://coveralls.io/github/trichards57/zxcvbn-cs)
[![NuGet](https://img.shields.io/nuget/v/zxcvbn-core.svg)](https://www.nuget.org/packages/zxcvbn-core)

This is a port of the `Zxcvbn` JavaScript password strength estimation library at
https://github.com/lowe/zxcvbn to .NET, written in C#.  This fork moves the library
to support .Net Core.

From the `Zxcvbn` readme:

> `zxcvbn`, named after a crappy password, is a JavaScript password strength
> estimation library. Use it to implement a custom strength bar on a
> signup form near you!
>
> `zxcvbn` attempts to give sound password advice through pattern matching
> and conservative entropy calculations. It finds 10k common passwords,
> common American names and surnames, common English words, and common
> patterns like dates, repeats (aaa), sequences (abcd), and QWERTY
> patterns.
> 
> For full motivation, see:
>
> http://tech.dropbox.com/?p=165

This port aims to produce comparable results with the Typescript version of `Zxcvbn` which I have also put out and is here https://github.com/trichards57/zxcvbn. 
The results structure that is returned can be interpreted in the same way as with JS `Zxcvbn` and this port has been tested with a variety of passwords to ensure 
that it return the same score as the JS version (some other details vary a little).

I have tried to keep the implementation as close as possible, but there is still a chance of some small changes.  Let me know if you find any differences
and I can investigate.

### Using `Zxcvbn-cs`

The included Visual Studio project will create a single assembly, Zxcvbn.dll, which is all that is
required to be included in your project.

To evaluate a password:

``` C#
using Zxcvbn;

//...

var result = Zxcvbn.Core.EvaluatePassword("p@ssw0rd");
```

`EvaluatePassword` takes an optional second parameter that contains an enumerable of
user data strings to also match the password against.

### Interpreting Results

The `Result` structure returned from password evaluation is interpreted the same way as with JS `Zxcvbn`.

- `result.Score`: 0-4 indicating the estimated strength of the password.

### Licence

Since `Zxcvbn-cs` is a port of the original `Zxcvbn` the original copyright and licensing applies. Cf. the LICENSE file.
