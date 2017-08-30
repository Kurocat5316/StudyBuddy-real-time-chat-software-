import { Component } from '@angular/core';
import { NavController, NavParams, ToastController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import {Http, Headers} from '@angular/http';
/*
  Generated class for the ChangeNickName page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-change-nick-name',
  templateUrl: 'change-nick-name.html'
})
export class ChangeNickNamePage {

  id: '';
  nickName: '';
  url: any;

  constructor(public navCtrl: NavController,
   public navParams: NavParams,
    private sharedata: ShareData,
    private http: Http,
    public toastCtrl: ToastController) {
      this.url = sharedata.geturl();
      this.id = sharedata.getId();
      this.GetNickName()
  }

  NewNickName(){
      var link = this.url + 'Profile/ChangeNickName';
      var data = JSON.stringify({
        id: this.id,
        nickName: this.nickName
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post(link, data, { headers: headers })
          .subscribe(data => {
            let toast = this.toastCtrl.create({
              message: 'Change success.',
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

  GetNickName(){
      var link = this.url + 'Profile/GetNickName';
      var data = JSON.stringify({
        id: this.id
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post(link, data, { headers: headers })
          .subscribe(data => {
            this.nickName = data.json();
          }, error => {
              let toast = this.toastCtrl.create({
              message: 'Ooop, connection seem unstable.',
              duration: 3000
            });
          toast.present();
      });
  }
}
