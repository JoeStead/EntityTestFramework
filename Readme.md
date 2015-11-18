# Entity Test Framework (Sorry for a terrible name...)

Entity Test Framework is a lightweight wrapper to sit around Entity Framework 7.

Currently I've only (manually) tested a couple of uses.

To use the framework in your test project, see the example below:

## Example
```csharp
[Fact]
public void SomeReallyBadTest()
{
  var configurableAuthContext = new ConfigurableContext<UserContext>(ctx =>
            {
                ctx.Setup(x => x.Users, new List<User>
                {
                    new User
                    {
                        UserId = 1014,
                        Username = "Joe",
                        Password = "SecurePassword1ThatIsHashed!"
                    }
                });
            });
			
	var subject = new Foo(configurableAuthContext); //Note the implicit conversion
	
	var usr = subject.FindUser("Joe");
	
	usr.Should().NotBeNull();
}	
```

```csharp

public class Foo
{
	private readonly UserContext _context;
	
	public Foo(UserContext context)
	{
		_context = context;
	}
	
	public async Task FindUser(string username)
	{
		var result = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);	
	}	
	
}
```

Please don't actually write a test like this, it is pointless, but you get the idea.

## Installing
Everything is on Nuget these days...

`PM> Install-Package EntityTestFramework -Pre`

## This wasn't my idea
This wasn't my original idea, I've just hacked together support for EntityFramework7. It is a result of discussions and original work between [Richard Tasker](https://twitter.com/ritasker) and [Mat McLoughlin](https://twitter.com/mat_mcloughlin).

There is a similar project for [Azure Storage available too](https://github.com/pier8software/Azure.Storage) 



