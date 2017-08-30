

export class ShareData {  
  
    private url: any;
    private id: '';
    private logOutFlag1: number = 0;
    private logOutFlag2: number = 0;
    private logOutFlag3: number = 0;

    constructor() {
        this.url = 'http://localhost:5423/api/'
    }
  
    setId(value) {
        this.id = value;     
    }
  
    getId() {
        return this.id;
    }

    geturl(){
        return this.url;
    }

    setLogOut1(value){
        this.logOutFlag1 = value;
    }

    getLogOut1(){
        return this.logOutFlag1;
    }

    setLogOut2(value){
        this.logOutFlag2 = value;
    }

    getLogOut2(){
        return this.logOutFlag2;
    }

    setLogOut3(value){
        this.logOutFlag3 = value;
    }

    getLogOut3(){
        return this.logOutFlag3;
    }
}