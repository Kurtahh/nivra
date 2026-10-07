

export class Session {
    static #instance: Session; 
    private username: String
    private loginStatus: Boolean;
    private id: Number;
    private todaySteps: number;
   
   private constructor()  {
        this.username = ""
        this.loginStatus = false;
        this.id = 0;
        this.todaySteps = 0;
   }
   
   
   public static get instance(): Session {
       if (!Session.#instance) {
           Session.#instance = new Session()
       }
       return Session.#instance;
   }
   
    public getId(): Number {
        return this.id;
    }

    public getUsername(): String {
        return this.username;
    }
    
    public getTodaySteps(): number {
       return this.todaySteps;
    }
    
    public async login( username: String, id: Number) {
        
       this.loginStatus = true;
       this.username = username;
       this.id = id;
       this.todaySteps = await this.fetchTodaySteps().then(num => {
           return num
       })
    }
    
    public async  fetchTodaySteps(): Promise<number> {

        var items;
        await fetch(`http://localhost:5207/api/User/TodaySteps/${Session.instance.id}`).then(
           res => res.json()
        ).then(
           json => {
               items = json
           }
        )
        
        if (items != null)
            return items           
        
        return 0;
    }
}