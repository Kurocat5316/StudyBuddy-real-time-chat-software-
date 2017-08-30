import { Component } from '@angular/core';
import { NavController, MenuController, ToastController, AlertController, LoadingController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import { Http, Headers } from '@angular/http';
import { SearchViewPage } from '../search-view/search-view'

/*
  Generated class for the Search page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-search',
  templateUrl: 'search.html'
})
export class SearchPage {

  id: '';
  key: string;
  range: number;
  searchMethod: boolean;
  units: Array<{
        unitCode: string,
        unitName: string,
        selected: boolean
    }>

  selectedUnit: Array<{
    unitCode: string;
    unitName: string;
  }>

  
  url: string;

  constructor(public navCtrl: NavController,
    public menu: MenuController,
    public http: Http,
    public sharedata: ShareData, 
    public toastCtrl: ToastController,
    public alertCtrl: AlertController,
    public loadingCtrl: LoadingController) {
    this.id = sharedata.getId();
    this.url = sharedata.geturl();
    this.menu.swipeEnable(false);
    this.searchMethod = true;
    this.range = 100;
    this.GetUnit();
  }

  GetUnit(){
    let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Profile/GetUnit';
        var data = JSON.stringify({
          id: this.id,
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post(link, data, { headers: headers })
            .subscribe(data => {
              loader.dismiss();
              this.units = data.json()
            }, error => {
              loader.dismiss();
                let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
        });
  }

  getItems(ev: any){
    this.key = ev.target.value;
  }
  
  back(){
    this.menu.swipeEnable(true);
    this.navCtrl.pop();
  }

  search(){

    //this.unit = {unitCode: "1", unitName: "2"};
    this.selectedUnit = [{unitCode: "", unitName: ""}];
    this.selectedUnit.splice(0, 1);
    //this.selectedUnit = null;
    for(var i = 0; i < this.units.length; i++){
      if(this.units[i].selected){
        let unit:{
              unitCode: string;
              unitName: string;
            };
        unit = {unitCode: "init", unitName: "init"};
        unit.unitCode = this.units[i].unitCode;
        unit.unitName = this.units[i].unitName;
        this.selectedUnit.push(unit);
      }
    }
    if(this.searchMethod){
      this.navCtrl.push(SearchViewPage, { method: this.searchMethod, key: this.key});
    }
    else{
      this.navCtrl.push(SearchViewPage, { method: this.searchMethod, key: this.key, units: this.selectedUnit, range: this.range});
    }
  }

  unitDetect(e){
    if(!this.searchMethod){
      if(this.units.length == 0){
        let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'You havent select unit, please go to proile page to select your units.',
            buttons: [{
                text: 'OK',
                handler: () => {
                 this.searchMethod = true;
                }
              }]
            });
            alert.present();
      }
    }
  }

}
