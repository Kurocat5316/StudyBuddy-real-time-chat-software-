import { Component } from '@angular/core';
import { NavController , NavParams, ToastController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import {Http, Headers} from '@angular/http';
/*
  Generated class for the AddUnit page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-add-unit',
  templateUrl: 'add-unit.html'
})
export class AddUnitPage {

  units: Array<{
    unitCode: string,
    unitName: string
  }>
  url: any;
  callback: any;

  constructor(public navCtrl: NavController, 
  public navParams: NavParams,
   private http: Http, 
   private sharedata: ShareData,
   public toastCtrl: ToastController){
     this.callback = this.navParams.get("callback")
     this.url = sharedata.geturl();
     //this.units = [{ unitCode: null, unitName: null}];
   }

  SearchUnit(ev: any){
     var link = this.url + 'Profile/SearchUnit';
        var data = JSON.stringify({
          unitCode: ev.target.value,
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post(link, data, { headers: headers })
            .subscribe(data => {
              this.units = data.json();
            }, error => {
                let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
        });
  }

  AddUnit(unit){
    this.callback(unit).then(()=>{
       this.navCtrl.pop();
   });
  }
}