x = (a: int, b: int) => a + b // lambda definition

x: (int, int)->int // lambda type

lstMap = func(lst: List<int>, map: int->int) // anon func
{
	foreach(x in lst)
	{
		yield return map(x)
	}
}

type List(elements: params<int>) // ? params
{
	func map(map: int->int)
	{
		return lstMap(self, map);
	}
}

list(1, 2, 3).map((x: int) => x + 1)

// type members

type User(name: str, type: str)
{
	this FullName = name + " " + type

	this func GetName() => name 
	
	static func Admin(name: str) => new User(name, "admin")
	
	static Empty = new User("", "")
}

// ADT

type User = 
	Simple: (Age: int * Name: string) |
	Admin: ()

arr = User[10]

arr = int[10]

arr[0] = 1