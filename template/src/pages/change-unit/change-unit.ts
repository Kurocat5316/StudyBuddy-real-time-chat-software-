import { Component } from '@angular/core';
import { NavController, NavParams, ToastController } from 'ionic-angular';
import { ShareData } from '../sharedata/sharedata';
import {Http, Headers} from '@angular/http';
import { AddUnitPage } from '../add-unit/add-unit'

/*
  Generated class for the ChangeUnit page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-change-unit',
  templateUrl: 'change-unit.html'
})
export class ChangeUnitPage {

  id: '';
  units: Array<{
    unitCode: string;
    unitName: string;
  }>
  url: any;
  constructor(public navCtrl: NavController,
  public navParams: NavParams, 
  private sharedata: ShareData,
  public http: Http, 
  public toastCtrl: ToastController, ) {
    this.url = sharedata.geturl();
    this.id = sharedata.getId();
    //this.units = [{ unitCode: null, unitName: null}]
    this.GetUnit();
  }

  GetUnit(){
      var link = this.url + 'Profile/GetUnit';
        var data = JSON.stringify({
          id: this.id,
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

  AddNewUnit(){
    this.navCtrl.push(AddUnitPage, { callback: this.myCallbackFunction });
  }

  myCallbackFunction = (unit) => {
 return new Promise((resolve, reject) => {
    for(var i = 0; i < this.units.length; i++){
      if(unit.unitCode == this.units[i].unitCode){
        return ;
      }
    }
    
    this.units.push(unit);
    resolve();
  });
}

RemoveUnit(unit){
    let index = this.units.indexOf(unit);
    this.units.splice(index, 1);
}

  ClearUnit(){
    for(var i = this.units.length - 1; i >= 0; i--){
      this.units.splice(i, 1);
    }
  }

  UploadUnit(){
    var link = this.url + 'Profile/ChangeUnit';
        var data = JSON.stringify({
          id: this.id,
          unit: this.units
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post(link, data, { headers: headers })
            .subscribe(data => {
              let toast = this.toastCtrl.create({
                message: 'Change Successful.',
                duration: 3000
              });
            toast.present();
            }, error => {
                let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
        });
  }

}