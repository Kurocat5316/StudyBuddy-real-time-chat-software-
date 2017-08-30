import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { ToastController, AlertController, LoadingController } from 'ionic-angular';
import {Http,Headers} from '@angular/http';
import { ShareData } from '../sharedata/sharedata'
/*
  Generated class for the Pwret page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-pwret',
  templateUrl: 'pwret.html'
})
export class PwretPage {

  studentId: '';
  url: any;

  constructor(public navCtrl: NavController,
   public navParams: NavParams,
  public toastCtrl: ToastController, 
  public http: Http,
  private sharedata: ShareData,
  public alertCtrl: AlertController,
   public loadingCtrl: LoadingController) {
      this.url = sharedata.geturl();
   }

  ResetPassword(){
    let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Register/ResetPassword';
      var data = JSON.stringify({
        studentId: this.studentId,
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post(link, data, { headers: headers })
          .subscribe(data => {
            loader.dismiss();
            let alert = this.alertCtrl.create({
            title: 'Error!',
            subTitle: 'Confirm message have send to your email.\n Please check it.',
            buttons: [{
                text: 'OK',
                role: 'OK',
                handler: () => {
                this.navCtrl.pop();
                }
              }]
            });
            alert.present();
          }, error => {
            loader.dismiss();
              let toast = this.toastCtrl.create({
              message: 'Ooop, connection seem unstable.',
              duration: 3000
            });
          toast.present();
      });
  }

}
