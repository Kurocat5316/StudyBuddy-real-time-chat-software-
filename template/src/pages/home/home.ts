import { Component } from '@angular/core';
import { RegisterPage } from "../register/register"
import { FriendListPage } from "../friend-list/friend-list"
import { PwretPage } from "../pwret/pwret"
import { NavController, NavParams, MenuController, AlertController, LoadingController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import {Http,Headers} from '@angular/http';

import { LocationTracker } from '../../providers/location-tracker';

@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
})
export class HomePage {
  id: '';
  password: '';
  response: boolean;
  teststring: string;
  testvar: '';
  url: any;

  firendInfor: Array<{
    friend: string,
    friendNickName: string,
    chatId: string,
    lastMessage: string
  }>;
  
  constructor(public navCtrl: NavController,
   public navParams: NavParams,
    public menu: MenuController,
      private sharedata: ShareData,
      public http: Http,
      public alertCtrl: AlertController,
      public loadingCtrl: LoadingController,
      public locationTracker: LocationTracker) {
    this.menu.swipeEnable(false);
    this.response = false;
    this.url = sharedata.geturl();
  }

  ToRegister(){
    this.navCtrl.push(RegisterPage);
  }

  Login(){
     let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Login/Login';
    console.log(link);
    var data = JSON.stringify({
      id: this.id,
      password: this.password,
    });
    var headers = new Headers();
		headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
         this.response = data.json();
         loader.dismiss();
         if(this.response){
              this.sharedata.setId(this.id);
              this.navCtrl.setRoot(FriendListPage);
            }else{
              if(!this.response){
                let alert = this.alertCtrl.create({
                    title: 'Error!',
                    subTitle: 'Incorrect information or inactive account or forbid by manager',
                    buttons: ["Ok"]
                  });
                  alert.present();
              }
            }
            
        }, error => {
          loader.dismiss();
            let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'Opps, connection seem unstable',
            buttons: ["Ok"]
          });
          alert.present();
        });
  }

  ToResetPassword(){
    this.navCtrl.push(PwretPage);
  }
}
