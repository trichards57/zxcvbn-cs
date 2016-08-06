Zxcvbn C#/.NET
==============

This is a port of the `Zxcvbn` JavaScript password strength estimation library at
https://github.com/lowe/zxcvbn to .NET, written in C#.

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

This port aims to produce comparable results with the JS version of `Zxcvbn`. The results
structure that is returned can be interpreted in the same way as with JS `Zxcvbn` and this
port has been tested with a variety of passwords to ensure that it return the same results
as the JS version.

There are some implementation differences, however, so exact results are not guaranteed.


### Using `Zxcvbn-cs`

The included Visual Studio project will create a single assembly, Zxcvbn.dll, which is all that is
required to be included in your project.

To evaluate a single password:

``` C#
using Zxcvbn;

//...

var result = Zxcvbn.MatchPassword("p@ssw0rd");
```

To evaluate many passwords, create an instance of `Zxcvbn` and then use that to evaluate your passwords. 
This avoids reloading dictionaries etc. for every password:

``` C#
using Zxcvbn;

//...

var zx = new Zxcvbn();

foreach (var password in passwords)
{
	var result = zx.EvaluatePassword(password);

	//...
}
```

Both `MatchPassword` and `EvaluatePassword` take an optional second parameter that contains an enumerable of
user data strings to also match the password against.

### Interpreting Results

The `Result` structure returned from password evaluation is interpreted the same way as with JS `Zxcvbn`:

- `result.Entropy`: bits of entropy for the password
- `result.CrackTime`: an estimation of actual crack time, in seconds.
- `result.CrackTimeDisplay`: the crack time, as a friendlier string: "instant", "6 minutes", "centuries", etc.
- `result.Score`: [0,1,2,3,4] if crack time is less than [10\*\*2, 10\*\*4, 10\*\*6, 10\*\*8, Infinity]. (useful for implementing a strength bar.)
- `result.MatchSequence`: the list of pattern matches that was used to calculate Entropy.
- `result.CalculationTime`: how long `Zxcvbn` took to calculate the results.

### More Information

For more information on why password entropy is calculated as it is, refer to `Zxcvbn`s originators:

https://github.com/lowe/zxcvbn

http://tech.dropbox.com/?p=165


### Licence

Since `Zxcvbn-cs` is a port of the original `Zxcvbn` the original copyright and licensing applies. Cf. the LICENSE file.
