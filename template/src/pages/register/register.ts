import { Component } from '@angular/core';
import { NavController, LoadingController, AlertController } from 'ionic-angular';
import {Http,Headers} from '@angular/http';
import { ShareData } from '../sharedata/sharedata'

/*
  Generated class for the Register page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-register',
  templateUrl: 'register.html'
})
export class RegisterPage {


    studentId: any;
    password: any;
    password2: any;
    url: string;

    flag: boolean;

  constructor(public navCtrl: NavController,
   public alertCtrl: AlertController,
    public http: Http, 
    private sharedata: ShareData,
     public loadingCtrl: LoadingController) {
    this.url = sharedata.geturl();
  }

  Register(){

    if(this.password != this.password2)
    {
      let alert = this.alertCtrl.create({
        title: 'Error!',
        subTitle: 'Your password confirm is different with your password',
       buttons: [{
                text: 'OK',
                role: 'OK',
                handler: () => {
                  return;
                }
              }]
      });
      alert.present();
    }
    let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Register/createAccount';
    var data = JSON.stringify({
      id: this.studentId,
      password: this.password,
    });
    var headers = new Headers();
		headers.append('Content-Type', 'application/json');

        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
          loader.dismiss();
          this.flag = data.json();
          if(this.flag)
              this.RegisterSuccess();
          else
              this.RegisterFail();
        }, error => {
            let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'Opps, connection seem unstable',
            buttons: ["Ok"]
          });
          loader.dismiss();
          alert.present();
        });
  }

  RegisterSuccess(){
      let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'Confirm message have send to your email.\n Please check it.',
            buttons: [{
                text: 'OK',
                handler: () => {
                this.navCtrl.pop();
                }
              }]
            });
            alert.present();
  }

  RegisterFail(){
      let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'The account have exist.',
            buttons: [{
                text: 'OK',
              }]
            });
            alert.present();
  }

}
