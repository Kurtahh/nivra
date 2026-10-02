

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
       
       
    }
    
    public async  getTodaySteps(id: Number) {
       let items
       let dataLoaded
       await fetch("http://localhost:5027/api/User/TodaySteps/" + id, {
           method: "GET",
           headers: {'Content-type': 'json/application'}
       }).then( (res) => 
           res.json()
       ).then((json) => {
           items = json
           dataLoaded = true 
       }
          
       )
        if (dataLoaded && items != null) {
            this.todaySteps = items[0]
        }
    }
}