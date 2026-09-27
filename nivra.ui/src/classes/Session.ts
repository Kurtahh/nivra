

export class Session {
    private username: String
    private loginStatus: Boolean;
   
   public constructor()  {
        this.username = ""
        this.loginStatus = false;
   }
    
    public getUsername(): String {
        return this.username;
    }
    
    public login( username: String) {
        
       this.loginStatus = true;
       this.username = username;
       
       if (this.loginStatus) {
           console.log("I have logged in");
       }
       
    }
}