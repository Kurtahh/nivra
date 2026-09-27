

export class Session {
    static #instance: Session; 
    private username: String
    private loginStatus: Boolean;
   
   private constructor()  {
        this.username = ""
        this.loginStatus = false;
   }
   
   
   public static get instance(): Session {
       if (!Session.#instance) {
           Session.#instance = new Session()
       }
       return Session.#instance;
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