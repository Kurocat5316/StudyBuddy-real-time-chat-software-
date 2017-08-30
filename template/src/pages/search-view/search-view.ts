import { Component } from '@angular/core';
import { NavController, NavParams, ToastController, LoadingController, AlertController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import { Http, Headers } from '@angular/http';

/*
  Generated class for the SearchView page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-search-view',
  templateUrl: 'search-view.html'
})
export class SearchViewPage {

  id: "";
  key: string;
  range: number;
  method: boolean;
  units: Array<{
        unitCode: string,
        unitName: string,
    }>
  url: string;

  friendSearch: Array<{
    id: string;
    nickName: string;
    distance: string;
    units: Array<{
      unitCode:string,
      unitName: string
    }>
  }>

  loader = this.loadingCtrl.create({
      content: "Please wait...",
    });

  constructor(public navCtrl: NavController, 
  public navParams: NavParams,
  public http: Http,
  public sharedata: ShareData, 
  public toastCtrl: ToastController,
  public loadingCtrl: LoadingController,
  public alertCtrl: AlertController) {
    
    this.loader.present();
    this.id = sharedata.getId();
    this.method = this.navParams.get("method")
    this.key = this.navParams.get("key");
    this.url = sharedata.geturl();
    if(this.method){
      this.GetFriendListWithId();
    }
    else{
      this.range = this.navParams.get("range");
      this.units = this.navParams.get("units");
      console.log(this.units);
      this.GetFriendListWithName();
    }
  }


  GetFriendListWithId(){
        var link = this.url + 'FriendSearch/SearchWithID';
        var data = JSON.stringify({
          id: this.id,
          key: this.key
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post(link, data, { headers: headers })
            .subscribe(data => {
              this.loader.dismiss();
              this.friendSearch = data.json();
              if(this.friendSearch.length == 0){
                let alert = this.alertCtrl.create({
                title: 'Empty!',
                subTitle: 'Search result is empty, please reset search condition.',
                buttons: [{
                    text: 'OK',
                    handler: () => {
                    this.navCtrl.pop();
                    }
                  }]
                });
                alert.present();
              }
            }, error => {
              this.loader.dismiss();
                let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
        });
  }

  GetFriendListWithName(){
    var link = this.url + 'FriendSearch/GetFriendListWithName';
    var data = JSON.stringify({
      id: this.id,
      key: this.key,
      range: this.range,
      units: this.units
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
       this.http.post(link, data, { headers: headers })
       .subscribe(data => {
         this.loader.dismiss();
          this.friendSearch = data.json();
          if(this.friendSearch.length == 0){
                let alert = this.alertCtrl.create({
                title: 'Empty!',
                subTitle: 'Search result is empty, please reset search condition.',
                buttons: [{
                    text: 'OK',
                    handler: () => {
                    this.navCtrl.pop();
                    }
                  }]
                });
                alert.present();
              }
       }, error => {
          this.loader.dismiss();
            let toast = this.toastCtrl.create({
            message: 'Ooop, connection seem unstable.',
            duration: 3000
          });
        toast.present();
   });
  }

  EditApplyInfor(friend){
      let prompt = this.alertCtrl.create({
                  title: 'Send Add Friend Require',
                  subTitle: friend.nickName,
                  inputs: [{
                              name: 'title',
                              placeholder: 'Input the message you want to send to sender'
                            }],
                  buttons: [{
                    text: 'Confirm',
                    handler: data => { this.SendRequire(friend.id, data['title'])}
                  },
                  {
                    text: "Cancel"
                  }]
                });
              prompt.present();
  }

  SendRequire( id, content ){
      //this.loader.present();
      console.log("post apply");
      var link = this.url + 'FriendSearch/SendFriendApply';
      var data = JSON.stringify({
        senderId: this.id,
        receiverId: id,
        content: content
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
          console.log("post success");
          //this.loader.dismiss();
        }, error => {
            //this.loader.dismiss();
              let toast = this.toastCtrl.create({
              message: 'Ooop, connection seem unstable.',
              duration: 3000
            });
          toast.present();
    });
  }

}
