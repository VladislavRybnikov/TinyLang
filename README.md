# TinyLang
Simple, tiny language compiler.

**TinyLang** (TL) is an imperative and functional language created for an educational purposes.

TL already has next features: 
  - **Variables** 
  
    `x = 123`  
    `str = "Hello World"`
  - **Type annotations**
    
    `x: int = 123`
  - **Operator**
    ```
    add = 2 + 2
    mull = 2 * 2 
    or = true || false
    ternary = true ? 1 : 0
    ```
  
  - **Records**
  
    ```
    type User(name: str, age: int)
    
    myUser = new User("Vlad", 20)
    ```
    Note: Defining types in that way creating toString method and constructor for all propertirs.
  - **Functions**
  
    ```
    func add(x: int, y: int)
    {
      return x + y
    }
    
    func mul(x: int, y: int) => x * y
    
    add(2, mull(2, 2))
  - **Lambda functions**
  
    ```
    add = (a: int, b: int) => a + b
    
    print(add(2, 2))
    ```
  - **If else**
  
    ```
    func validateAge(user: User)
    {
        msg = "";
        
        if(u.Age >= 16 && u.Age < 18)
        {
          msg = "age is more than 16 and less than 18"
        }
        elif(u.Age >= 18)
        {
          msg = "age is more than 18"
        }
        else
        {
          msg = "age is less than 18"
        }
        
        return msg
    }
    
    print(validateAge(new User("Vlad", 20)))
    ```
