type User(name: str, userType: str)

func createAdmin(name: str)
{
	r = new User(name, "admin")
	return r
}
                    
u = createAdmin("Vlad")

print(u)