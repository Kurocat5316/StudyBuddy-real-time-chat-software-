import { Component } from '@angular/core';
import { NavController, NavParams, ToastController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import {Http, Headers} from '@angular/http';

/*
  Generated class for the ChangePassword page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-change-password',
  templateUrl: 'change-password.html'
})
export class ChangePasswordPage {

  id: '';
  password1:'';
  password2:'';
  url: any;

  constructor(public navCtrl: NavController, 
  public navParams: NavParams, 
  private http: Http,
  private sharedata: ShareData,
  public toastCtrl: ToastController) {
    this.url = sharedata.geturl();
    this.id = sharedata.getId();
    this.password1 = null;
    this.password2 = null;
  }

  NewPassword(){
    if( this.password1 == this.password2)
    {
        var link = this.url + 'Profile/ChangPassWord';
        var data = JSON.stringify({
          id: this.id,
          password: this.password1
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
      else{
        let toast = this.toastCtrl.create({
                message: 'Your Password and password confirm is different.',
                duration: 3000
              });
          toast.present();
      }
  }

}
