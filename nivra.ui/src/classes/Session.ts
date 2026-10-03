

export class Session {
    static #instance: Session; 
    private username: String
    private loginStatus: Boolean;
    private id: Number;
    private todaySteps: Number;
   
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
    
    public login( username: String, id: Number) {
        
       this.loginStatus = true;
       this.username = username;
       this.id = id;
       this.getTodaySteps(id)
    }
    
    public async  getTodaySteps(id: Number) {
        
       var items;
       await fetch(`http://localhost:5207/api/User/TodaySteps/${id}`).then(
           res => res.json()
       ).then(
           json => {
               items = json
           }
       )
        
        
        if (items != null) {
            console.log(items)
        }
    }
}