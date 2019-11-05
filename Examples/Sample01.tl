a = "Hello World";
print(a);

record User(name: string, age: int);

readonly record UserDto(name: string, age: int);

function GetUser(name: string): string
{
	new User(name);
}