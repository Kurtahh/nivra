

class Session {
    private username: String
    private loginStatus: bool;
   
   public constructor()  {
        this.username = ""
        this.loginStatus = 0;
   }
    
    public getUsername(): String {
        return this.username;
    }
    
    public login( username: String) {
        
       this.loginStatus = 1;
       this.username = username;
       
       if (this.loginStatus) {
           console.log("I have logged in");
       }
       
    }
}