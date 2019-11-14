a = "Hello World";
print(a);

record User(name: str, age: int); // class with all fields equality. Have state but not behavoiur

data record UserDto(name: string, age: int); // readonly struct

type User // create new class or add behaviour to existing record
{
	function GetFullName()
	{

	}
}

type User(name: string, age: int)
{
			
}

type Factory<T>
{

}

function GetUser(name: string): string
{
	new User(name);
}

// pattern matching
a = 12;

b = a match
{
	1 => a * 2
	12 => a * 3
	_ => 0
}

// loops
for(i = 1; i < 10; i++)
{
	#
}

// curring

function a(a1, a2, a3){}
b => a(1, 2) // function b(b3) => a(1, 2, b3)